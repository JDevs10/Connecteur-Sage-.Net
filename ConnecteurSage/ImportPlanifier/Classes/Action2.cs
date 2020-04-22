using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Helpers;
using System.IO;
using System.Data.Odbc;
using System.Data;
using System.Globalization;
using Microsoft.Win32.TaskScheduler;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using ImportPlanifier.Classes;

namespace importPlanifier.Classes
{
    class Action2
    {
        //private static string filename = "";
        private string locationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string dir;
        private static int nbr = 0;
        //private static StreamWriter LogFile;
        private static List<string> MessageErreur;
        private static List<CustomMailRecapLines> recapLinesList_new;

        /* JL LOG */
        private static string logFileName_import;
        private string logDirectoryName_general = Directory.GetCurrentDirectory() + @"\" + "LOG";
        private string logDirectoryName_import = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Import";
        private string directoryName_SuccessFile = Directory.GetCurrentDirectory() + @"\" + "Success File";
        private string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
        private StreamWriter logFileWriter_general = null;
        //private StreamWriter logFileWriter_import = null;


        public static string ConvertDate(string date)
        {
            if (date.Length == 8)
            {
                return date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
            }
            return "";
        }

        public void ImportPlanifier()
        {

            //tester s'il existe des fichies .csv
            Boolean FileExiste = false;
            //Boolean insertAdressLivr = false;
            //int count = 0;
            int SaveSuccess = 0;
            //string[] tabCommande = {};
            List<string> tabCommande = new List<string>();
            List<string> tabCommandeError = new List<string>();
            List<Order> ordersList = new List<Order>();

            Classes.Path path = getPath();
            dir = path.path;
            Console.WriteLine("Import/Export planifier Sage!!");
            Console.WriteLine("Execution en cours..");
            Console.WriteLine("##############################################");
            Console.WriteLine("############ L'import planifier ##############");
            Console.WriteLine("##############################################");
            Console.WriteLine("");


            if (dir == null)
            {
                Console.WriteLine(DateTime.Now + " : *********** Erreur **********");
                Console.WriteLine(DateTime.Now + " : L'emplacement de l'import n'est pas enregistrer");
                Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }

            if (!Directory.Exists(dir))
            {
                Console.WriteLine(DateTime.Now + " : *********** Erreur **********");
                Console.WriteLine(DateTime.Now + " : L'emplacement n'est pas trouvé.");
                Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }

            //Console.WriteLine("Dossier : " + fileListing);
            //Console.WriteLine("");
            //Console.WriteLine(DateTime.Now + " : Scan du dossier ...");

             //Check if the Log directory exists
            if (!Directory.Exists(logDirectoryName_import))
            {
                //Create log directory
                Directory.CreateDirectory(logDirectoryName_import);
            }
            if (!Directory.Exists(directoryName_SuccessFile))
            {
                // Create Success File
                Directory.CreateDirectory(directoryName_SuccessFile);
            }
            if (!Directory.Exists(directoryName_ErrorFile))
            {
                // Create Error File
                Directory.CreateDirectory(directoryName_ErrorFile);
            }

            //Create log file
            logFileName_import = logDirectoryName_import + @"\" + string.Format("LOG_Import_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_import = File.Create(logFileName_import);


            //Get all .csv files in the folder
            DirectoryInfo fileListing = new DirectoryInfo(dir);
            FileInfo[] allFiles = fileListing.GetFiles("*.csv");
            // string[,] importErrorTable = new string[allFiles.Length, 3];

            //Get Tache Planifier
            string infoPlan = InfoTachePlanifier(logFileWriter_general);
            if (infoPlan == null)
            {
                infoPlan = "Tache Manuel";
                Console.WriteLine(DateTime.Now + " : Aucune importation planifiée trouvé");
                Console.WriteLine(DateTime.Now + " : Import annulée");
                logFileWriter_general.WriteLine(DateTime.Now + " : Aucune importation planifiée trouvé!");
                logFileWriter_general.WriteLine(DateTime.Now + " : Probablement executé manuellement ???");
            }

            // check the initDOC_Numerotation if initialized
            if (initDOC_Numerotation(logFileWriter_general))
            {
                logFileWriter_general.WriteLine(DateTime.Now + " : La table DOC_Numerotation est trouvé.");
                logFileWriter_general.WriteLine("");
            }
            else
            {
                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                logFileWriter_general.WriteLine(DateTime.Now + " : La table DOC_Numerotation n'est pas trouvé.");
                logFileWriter_general.WriteLine(DateTime.Now + " : La numérotation des nouveaux documents lors de l'importation sera la suite de la dernière numérotation la plus élevée dans chaque type de document.");
                logFileWriter_general.WriteLine("");
            }

            logFileWriter_general.Flush();
            logFileWriter_general.WriteLine(DateTime.Now + " : "+infoPlan);
            logFileWriter_general.WriteLine(DateTime.Now + " : Dossier : " + fileListing);
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine(DateTime.Now + " : Scan du dossier ...");
            logFileWriter_general.WriteLine("");

            recapLinesList_new = new List<CustomMailRecapLines>();

            using (StreamWriter logFileWriter_import = new StreamWriter(logFile_import))
            {
                logFileWriter_import.WriteLine("#####################################################################################");
                logFileWriter_import.WriteLine("################################ ConnecteurSage Sage ################################");
                logFileWriter_import.WriteLine("#####################################################################################");
                logFileWriter_import.WriteLine("");


                /* 
                    action : import documents
                */
                // Recherche des fichiers .csv
                //foreach (FileInfo filename in fileListing.GetFiles("*.csv"))
                int nbr_of_files = allFiles.Length;
                logFileWriter_general.WriteLine(DateTime.Now + " : Numbre de fichier \".csv\" trouvé : " + nbr_of_files);

                for (int index = 0; index < allFiles.Length; index++)
                {
                    FileInfo filename = allFiles[index];
                    Console.WriteLine(DateTime.Now + " : Fichier trouve ===> " + filename.Name);
                    string file_doc_reference = "";

                    try
                    {
                        nbr++;
                        FileExiste = true;
                        Console.WriteLine(DateTime.Now + " : un fichier \".csv\" trouvé :");
                        Console.WriteLine(DateTime.Now + " : -----> " + nbr + " / " + nbr_of_files + " - " + " + filename");
                        Console.WriteLine(DateTime.Now + " : Scan fichier...");

                        logFileWriter_general.WriteLine(DateTime.Now + " : -----> Fichier " + index + "/" + allFiles.Length + " : " + filename.Name);

                        logFileWriter_import.WriteLine(DateTime.Now + " : -----> Fichier " + index + "/" + allFiles.Length + " : " + filename.Name);
                        logFileWriter_import.WriteLine(DateTime.Now + " : Scan fichier...");

                        long pos = 1;

                        string[] lines = System.IO.File.ReadAllLines(fileListing + @"\" + filename, Encoding.Default);

                        if (lines[0].Split(';')[0] == "ORDERS" && filename.Name.Contains("EDI_ORDERS") && lines[0].Split(';').Length == 11)
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Orders Test Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Commande Manuel.");

                            Boolean prixDef = false;
                            //Boolean insertAdressLivr = false;
                            Order order = new Order();

                            order.Id = get_next_num_piece_commande(logFileWriter_import);

                            if (order.Id == "erreur")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : orderId erreur");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, lines[0].Split(';')[1], "Echec de l'import d'une commande", "orderId erreur", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }
                            
                            if (order.Id == null)
                            {
                                Console.WriteLine("Erreur [10] : numéro de piece non valide");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : orderId est null");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines("Null", lines[0].Split(';')[1], "Impossible de générer un nouveau numéro de piece", "Numéro de piece non valide - orderId est null", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            order.Lines = new List<OrderLine>();

                            order.NumCommande = lines[0].Split(';')[1];

                            if (order.NumCommande.Length > 10)
                            {
                                Console.WriteLine("Numéro de commande doit être < 10");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Numéro de commande doit être < 10");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée", "Numéro de commande doit être < 10 => " + order.NumCommande, "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (order.NumCommande == "")
                            {
                                Console.WriteLine("Le champ numéro de commande est vide.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ numéro de commande est vide.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée", "Le champ numéro de commande est vide.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (!IsNumeric(order.NumCommande))
                            {
                                Console.WriteLine("Le champ numéro de commande est invalide.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ numéro de commande est invalide.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée", "Le champ numéro de commande est invalide => " + order.NumCommande, "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            string existe = existeCommande(order.NumCommande, logFileWriter_import);

                            if (existe != null && existe != "erreur")
                            {
                                Console.WriteLine("La commande N° " + order.NumCommande + " existe deja dans la base.\nN° de pièce : " + existe + "");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La commande N° " + order.NumCommande + " existe deja dans la base.\nN° de pièce : " + existe + ".");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + ".", "La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + ".", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (existe == "erreur")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : N° de pièce : '" + existe + "' trouvée dans la Base de Données");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. N° de pièce '" + existe + "' est trouvée dans la Base de Données.", "N° de pièce : '" + existe + "' trouvée dans la Base de Données.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            file_doc_reference = order.NumCommande;

                            order.codeClient = lines[0].Split(';')[2];
                            order.codeAcheteur = lines[0].Split(';')[3];
                            order.codeFournisseur = lines[0].Split(';')[4];
                            //order.adresseLivraison = lines[0].Split(';')[7];

                            if(order.codeClient == "")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ du code client dans le fichier est vide, verifier le code client.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Le champ du code client dans le fichier est vide, verifier le code client.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (order.codeAcheteur == "")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ du code acheteur dans le fichier est vide, verifier le code client.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Le champ du code acheteur dans le fichier est vide, verifier le code client.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (order.codeFournisseur == "")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ du code fournisseur dans le fichier est vide, verifier le code client.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Le champ du code fournisseur dans le fichier est vide, verifier le code client.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            Client client = getClient(order.codeClient, 1, logFileWriter_import);
                            if (client == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Client trouvé est null, verifier le code client.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. le cLient (" + order.codeClient + ") n'est pas trouvé dans Sage !", "Client trouvé est null, verifier le code client (" + order.codeClient + ").", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }


                            Client client2 = getClient(order.codeAcheteur, 2, logFileWriter_import);
                            if (client2 == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Acheteur trouvé est null, verifier le code Acheteur.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. L'acheteur (" + order.codeAcheteur + ") n'est pas trouvé dans Sage !", "Acheteur trouvé est null, verifier le code Acheteur (" + order.codeAcheteur + ").", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }


                            if (existeFourniseur(order.codeFournisseur, logFileWriter_import) == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Fournisseur trouvé est null, verifier le code Fournisseur.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. Le fournisseur (" + order.codeFournisseur + ") n'est pas trouvé dans Sage !", "Fournisseur trouvé est null, verifier le code Fournisseur (" + order.codeFournisseur + ").", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }


                            order.adresseLivraison = lines[0].Split(';')[7];
                            string[] tab_adress = order.adresseLivraison.Split('.');

                            if (tab_adress.Length != 5)
                            {
                                Console.WriteLine("La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante :\nNom.Adresse.CodePostal.Ville.Pays");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante :\nNom.Adresse.CodePostal.Ville.Pays.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante :\nNom.Adresse.CodePostal.Ville.Pays.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }
                            order.nom_contact = tab_adress[0];
                            order.adresse = tab_adress[1].Replace("'", "''");
                            order.codepostale = tab_adress[2];
                            order.ville = tab_adress[3].Replace("'", "''");
                            order.pays = tab_adress[4];


                            List<AdresseLivraison> listAdress = get_adresse_livraison(new AdresseLivraison(1, client.CT_Num, order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays), logFileWriter_import);

                            // Ajouter ville dans la réference
                            //string[] part = order.adresseLivraison.Split('.');
                            //if (part.Length >= 2)
                            //{
                            order.Reference = order.ville;
                            //}

                            order.deviseCommande = lines[0].Split(';')[8];

                            if (order.deviseCommande != "")
                            {
                                order.deviseCommande = getDevise(order.deviseCommande, logFileWriter_import);
                            }


                            if (order.deviseCommande == "erreur")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : deviseCommande == erreur");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "deviseCommande == erreur.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                            if (lines[1].Split(';')[0] == "ORDHD1" && lines[1].Split(';').Length == 5)
                            {
                                logFileWriter_import.WriteLine(DateTime.Now + " : ORDHD1 trouvé.");
                                logFileWriter_import.WriteLine("");

                                if (lines[1].Split(';')[1].Length == 8)
                                {
                                    order.DateCommande = ConvertDate(lines[1].Split(';')[1]);
                                    order.conditionLivraison = lines[1].Split(';')[2];

                                    if (order.conditionLivraison != "")
                                    {
                                        order.conditionLivraison = get_condition_livraison(order.conditionLivraison, logFileWriter_import);
                                    }

                                    if (string.IsNullOrEmpty(order.conditionLivraison))
                                    {
                                        order.conditionLivraison = "1";
                                    }

                                    if (lines[2].Split(';')[0] == "ORDLIN" && lines[2].Split(';').Length == 23)
                                    {
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ORDLIN trouvé.");
                                        var totalLines = lines.Count();
                                        var currentIndexLine = 1;

                                        //decimal total = 0m;
                                        foreach (string ligneDuFichier in lines)
                                        {
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ORDLIN line " + currentIndexLine + " / " + totalLines + ".");

                                            string[] tab = ligneDuFichier.Split(';');


                                            switch (tab[0])
                                            {
                                                case "ORDLIN":
                                                    if (tab.Length == 23)
                                                    {
                                                        OrderLine line = new OrderLine();
                                                        line.NumLigne = tab[1];
                                                        line.article = getArticle(tab[2], client.CT_NumCentrale, logFileWriter_import);

                                                        if (line.article == null)
                                                        {
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                            tabCommandeError.Add(filename.Name);
                                                            recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée. Cette article (" + tab[2] + ") n'existe pas dans la base pour le client ("+ client.CT_Num + ").", "Cette article (" + tab[2] + ") n'existe pas dans la base.", "", filename.Name, logFileName_import));
                                                            goto goErrorLoop;
                                                        }

                                                        if (line.article.RP_CODEDEFAUT == null || line.article.RP_CODEDEFAUT == "")
                                                        {
                                                            line.article.RP_CODEDEFAUT = "NULL";
                                                        }


                                                        line.article.Conditionnement = getConditionnementArticle(line.article.AR_REF, logFileWriter_import);

                                                        logFileWriter_import.WriteLine(DateTime.Now + " : tab ===> line.article.Conditionnement");
                                                        if (line.article.AR_Nomencl == "2" || line.article.AR_Nomencl == "3")
                                                        {
                                                            line.article.AR_REFCompose = line.article.AR_REF;
                                                        }

                                                        if (line.article.gamme1 != "0")
                                                        {
                                                            line.article.gamme1 = testGamme(0, line.article.AR_REF, line.article.gamme1, logFileWriter_import);
                                                        }

                                                        if (line.article.gamme2 != "0")
                                                        {
                                                            line.article.gamme2 = testGamme(1, line.article.AR_REF, line.article.gamme2, logFileWriter_import);
                                                        }

                                                        line.Quantite = tab[9].Replace(",", ".");
                                                        decimal d = Decimal.Parse(line.Quantite, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                        if (d == 0)
                                                        {
                                                            line.Quantite = "1";
                                                        }


                                                        if (line.article.Conditionnement != null)
                                                        {
                                                            int quantite_Conditionnement = Calcule_conditionnement(d, line.article.Conditionnement.EC_QUANTITE, logFileWriter_import);
                                                            line.Calcule_conditionnement = quantite_Conditionnement.ToString();
                                                        }

                                                        line.PrixNetHT = tab[14].Replace(",", ".");
                                                        if(line.PrixNetHT == "")
                                                        {
                                                            line.PrixNetHT = "0.0";
                                                        }

                                                        line.MontantLigne = tab[11];
                                                        line.DateLivraison = ConvertDate(tab[21]);
                                                        if (line.DateLivraison.Length == 6)
                                                        {
                                                            line.DateLivraison = "Null";
                                                        }

                                                        try
                                                        {
                                                            if (!string.IsNullOrEmpty(line.article.AR_POIDSBRUT))
                                                            {
                                                                line.article.AR_POIDSBRUT = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                            }
                                                            if (!string.IsNullOrEmpty(line.article.AR_POIDSNET))
                                                            {
                                                                line.article.AR_POIDSNET = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            Console.WriteLine("Erreur de conversion de poids.");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Erreur de conversion de poids.");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                            tabCommandeError.Add(filename.Name);
                                                            recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Erreur de conversion de poids.", "", filename.Name, logFileName_import));
                                                            goto goErrorLoop;
                                                        }

                                                        line.codeAcheteur = tab[4].Replace(" ", "");
                                                        line.codeFournis = tab[5].Replace(" ", "");
                                                        //line.codeFournis = line.codeFournis.Replace(Environment.NewLine, String.Empty);
                                                        line.descriptionArticle = tab[8].Replace("'", "''");
                                                        if (string.IsNullOrEmpty(line.descriptionArticle))
                                                        {
                                                            line.descriptionArticle = line.article.AR_DESIGN;
                                                        }

                                                        //total = total + Decimal.Parse(tab[11].Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);


                                                        decimal prix = Decimal.Parse(line.PrixNetHT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);


                                                        decimal prixSage = Decimal.Parse(line.article.AR_PRIXVEN.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Prix *********************");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Prix trouvé dans le fichier EDI => " + prix + " && Prix trouvé dans Sage => " + prixSage);
                                                        logFileWriter_import.WriteLine("");

                                                        if (prixSage == 0.0m) //if sage article prie is 0.0 then use the EDI file price
                                                        {
                                                            line.PrixNetHT = prix.ToString().Replace(',', '.');
                                                        }
                                                        else if (prix != prixSage)
                                                        {
                                                            Console.WriteLine("Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + "\nIl est différent du prix envoyer par le client : " + prix + ".");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning Prix *********************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + "\nIl est différent du prix envoyer par le client : " + prix + ".");

                                                            //use the article prix from the db
                                                            if (prixSage != 0.000000m)
                                                            {
                                                                Console.WriteLine("Utilise le prix de la base : " + prixSage);
                                                                line.PrixNetHT = prixSage.ToString().Replace(',', '.');
                                                                logFileWriter_import.WriteLine(DateTime.Now + " : Utilise le prix de la base : " + prixSage + ".");
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Utilise le prix envoyer par le client : " + prix);
                                                                line.PrixNetHT = prix.ToString().Replace(',', '.');
                                                                logFileWriter_import.WriteLine(DateTime.Now + " : Utilise le prix envoyer par le client : " + prix + ".");
                                                            }
                                                            logFileWriter_import.WriteLine("");
                                                        }

                                                        order.Lines.Add(line);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Erreur dans la ligne " + pos + " du fichier.", "Erreur de lecture !!");

                                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                        logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur dans la ligne " + pos + " du fichier " + filename + ".", "Erreur de lecture.");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                        tabCommandeError.Add(filename.Name);
                                                        recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Erreur dans la ligne " + pos + " du fichier " + filename + ".", "", filename.Name, logFileName_import));
                                                        goto goErrorLoop;
                                                    }
                                                    break;
                                            }

                                            pos++;

                                        }

                                        //order.MontantTotal = total;

                                        order.DateLivraison = "Null";

                                        for (int i = 0; i < order.Lines.Count; i++)
                                        {
                                            if (order.Lines[i].DateLivraison.Length == 10)
                                            {
                                                order.DateLivraison = order.Lines[i].DateLivraison;
                                                goto jamp;
                                            }
                                        }

                                    jamp:


                                        if (order.codeClient != "")
                                        {

                                            if (string.IsNullOrEmpty(order.deviseCommande))
                                            {
                                                order.deviseCommande = client.N_Devise;
                                            }

                                            order.StockId = getStockId(logFileWriter_import);
                                            if (string.IsNullOrEmpty(order.StockId))
                                            {
                                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Stock ID est null ou vide.");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                tabCommandeError.Add(filename.Name);
                                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Stock ID est null ou vide.", "", filename.Name, logFileName_import));
                                                goto goErrorLoop;
                                            }

                                            if (!prixDef)
                                            {
                                                string Ref = order.Reference + "/" + order.NumCommande;

                                                if (Ref.Length <= 17)
                                                {
                                                    order.Reference = Ref;
                                                }
                                                else
                                                {
                                                    int reste = 16 - order.NumCommande.Length;

                                                    if (order.Reference.Length > reste)
                                                    {
                                                        order.Reference = order.Reference.Substring(0, reste) + "/" + order.NumCommande;
                                                    }
                                                }
                                            }

                                            if (prixDef)
                                            {
                                                string pr = "/AP";
                                                string Ref = order.Reference + "/" + order.NumCommande + pr;

                                                if (Ref.Length <= 17)
                                                {
                                                    order.Reference = Ref;
                                                }
                                                else
                                                {
                                                    int reste = 16 - order.NumCommande.Length - pr.Length;

                                                    if (order.Reference.Length > reste)
                                                    {
                                                        order.Reference = order.Reference.Substring(0, reste) + "/" + order.NumCommande + pr;
                                                    }
                                                }

                                            }

                                            if (order.Lines.Count == 0)
                                            {
                                                Console.WriteLine("Aucun ligne de commande enregistré.");

                                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Aucun ligne de commande enregistré. ligne = " + order.Lines.Count());
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                tabCommandeError.Add(filename.Name);
                                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Aucun ligne de commande enregistré. ligne = " + order.Lines.Count(), "", filename.Name, logFileName_import));
                                                goto goErrorLoop;
                                            }
                                            MessageErreur = new List<string>();

                                            order.adresseLivraison = getNumLivraison(client.CT_Num, logFileWriter_import);
                                            if (string.IsNullOrEmpty(order.adresseLivraison))
                                            {
                                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Adresse de livraison est null ou vide.");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                tabCommandeError.Add(filename.Name);
                                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Adresse de livraison est null ou vide.", "", filename.Name, logFileName_import));
                                                goto goErrorLoop;
                                            }

                                            //Get the list of all Taxes (TVA)
                                            //So i can calculate the ttc later
                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Récupére tous les tva");
                                            List<TVA> tvaList = null;
                                            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                                            {
                                                connexion.Open();
                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : SQL ===> " + QueryHelper.getAllTVA(true));
                                                using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connexion))
                                                {
                                                    using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                                    {
                                                        if (reader.Read()) // If any rows returned
                                                        {
                                                            tvaList = new List<TVA>();
                                                            tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                                            while (reader.Read())
                                                            {
                                                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                                            }
                                                        }
                                                        else// If no rows returned
                                                        {
                                                            //do nothing.
                                                            tvaList = null;
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Aucune reponse. ");
                                                        }
                                                    }
                                                }

                                                string taxCode = "";
                                                string taxValue = "";
                                                double totalHT = 0.0;
                                                double totalTTC = 0.0;

                                                if (insertCommande(client, order, logFileWriter_import, filename.Name))
                                                {
                                                    int nbr_ = 0;

                                                    for (int i = 0; i < order.Lines.Count; i++)
                                                    {
                                                        if (order.Lines[i].article.AR_SuiviStock == "0")
                                                        {
                                                            order.Lines[i].article.AR_StockId = "0";
                                                        }
                                                        else
                                                        {
                                                            order.Lines[i].article.AR_StockId = order.StockId;
                                                        }

                                                        //Get article taxes
                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : SQL ===> " + QueryHelper.getArticleTaxe(true, order.Lines[i].article.AR_REF));
                                                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticleTaxe(true, order.Lines[i].article.AR_REF), connexion))
                                                        {
                                                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                                            {
                                                                if (reader.Read()) // If any rows returned
                                                                {
                                                                    order.Lines[i].article.DL_CodeTaxe1 = reader[0].ToString();
                                                                    order.Lines[i].article.DL_CodeTaxe2 = reader[1].ToString();
                                                                    order.Lines[i].article.DL_CodeTaxe3 = reader[2].ToString();
                                                                    logFileWriter_import.WriteLine("DL_CodeTaxe1 : "+ order.Lines[i].article.DL_CodeTaxe1 + " || DL_CodeTaxe2 : " + order.Lines[i].article.DL_CodeTaxe2 + " || DL_CodeTaxe3 : " + order.Lines[i].article.DL_CodeTaxe3);
                                                                    logFileWriter_import.WriteLine("");
                                                                }
                                                                else// If no rows returned
                                                                {
                                                                    //do nothing.
                                                                    order.Lines[i].article.DL_CodeTaxe1 = "C00";
                                                                    order.Lines[i].article.DL_CodeTaxe2 = "C00";
                                                                    order.Lines[i].article.DL_CodeTaxe3 = "C00";
                                                                    logFileWriter_import.WriteLine("Aucune reponse.");
                                                                    logFileWriter_import.WriteLine("DL_CodeTaxe1 : " + order.Lines[i].article.DL_CodeTaxe1 + " || DL_CodeTaxe2 : " + order.Lines[i].article.DL_CodeTaxe2 + " || DL_CodeTaxe3 : " + order.Lines[i].article.DL_CodeTaxe3);
                                                                    logFileWriter_import.WriteLine("");
                                                                }
                                                            }
                                                        }


                                                        //Add TVA && Calculate product ttc
                                                        double product_ttc = 0.0;
                                                        try
                                                        {
                                                            logFileWriter_import.WriteLine("");
                                                            ConfigurationDNS dns = new ConfigurationDNS();
                                                            dns.LoadSQL();
                                                            if (tvaList != null)
                                                            {
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : List des TVA trouvé");
                                                                TVA tva__1 = null;
                                                                TVA tva__2 = null;
                                                                TVA tva__3 = null;
                                                                bool tva_1 = false;
                                                                bool tva_2 = false;
                                                                bool tva_3 = false;

                                                                //find and add CodeTaxe1
                                                                foreach (TVA tva_ in tvaList)
                                                                {
                                                                    if (order.Lines[i].article.DL_CodeTaxe1 != null && order.Lines[i].article.DL_CodeTaxe1 != "" && tva_.TA_Code == order.Lines[i].article.DL_CodeTaxe1)
                                                                    {
                                                                        tva__1 = tva_;
                                                                        tva_1 = true;
                                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 1 " + tva__1.TA_Code + " trouvé \"" + tva__1.TA_Taux + "\"");
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (order.Lines[i].article.DL_CodeTaxe1 == null)
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_1 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 1 NULL trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                        else if (order.Lines[i].article.DL_CodeTaxe1 == "")
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_1 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 1 VIDE trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                //find and add CodeTaxe2
                                                                foreach (TVA tva_ in tvaList)
                                                                {
                                                                    if (order.Lines[i].article.DL_CodeTaxe2 != null && order.Lines[i].article.DL_CodeTaxe2 != "" && tva_.TA_Code == order.Lines[i].article.DL_CodeTaxe2)
                                                                    {
                                                                        tva__2 = tva_;
                                                                        tva_2 = true;
                                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 2 " + tva__2.TA_Code + " trouvé \"" + tva__2.TA_Taux + "\"");
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (order.Lines[i].article.DL_CodeTaxe2 == null)
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_2 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 2 NULL trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                        else if (order.Lines[i].article.DL_CodeTaxe2 == "")
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_2 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 2 VIDE trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                //find and add CodeTaxe3
                                                                foreach (TVA tva_ in tvaList)
                                                                {
                                                                    if (order.Lines[i].article.DL_CodeTaxe3 != null && order.Lines[i].article.DL_CodeTaxe3 != "" && tva_.TA_Code == order.Lines[i].article.DL_CodeTaxe3)
                                                                    {
                                                                        tva__3 = tva_;
                                                                        tva_3 = true;
                                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 3 " + tva__3.TA_Code + " trouvé \"" + tva__3.TA_Taux + "\"");
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (order.Lines[i].article.DL_CodeTaxe3 == null)
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_3 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 3 NULL trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                        else if (order.Lines[i].article.DL_CodeTaxe3 == "")
                                                                        {
                                                                            //tva = tva_;
                                                                            tva_3 = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 3 VIDE trouvé, alors TVA mis à 0");
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                logFileWriter_import.WriteLine("");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : ******************** Calcule TVA ******************** ");
                                                                double endTVA = 0.0;

                                                                if (tva_1)
                                                                {
                                                                    endTVA += Convert.ToDouble(tva__1.TA_Taux);
                                                                    order.Lines[i].article.DL_Taxe1 = tva__1.TA_Taux.Replace(",", ".");
                                                                }
                                                                else
                                                                {
                                                                    order.Lines[i].article.DL_CodeTaxe1 = "C00";
                                                                    order.Lines[i].article.DL_Taxe1 = "0.000000";
                                                                    endTVA += 0.0;
                                                                }

                                                                if (tva_2)
                                                                {
                                                                    //endTVA += Convert.ToDouble(tva__2.TA_Taux);
                                                                    order.Lines[i].article.DL_Taxe2 = tva__2.TA_Taux.Replace(",", ".");
                                                                }
                                                                else
                                                                {
                                                                    order.Lines[i].article.DL_CodeTaxe2 = "C00";
                                                                    order.Lines[i].article.DL_Taxe2 = "0.000000";
                                                                    endTVA += 0.0;
                                                                }

                                                                if (tva_3)
                                                                {
                                                                    //endTVA += Convert.ToDouble(tva__3.TA_Taux);
                                                                    order.Lines[i].article.DL_Taxe3 = tva__3.TA_Taux.Replace(",", ".");
                                                                }
                                                                else
                                                                {
                                                                    order.Lines[i].article.DL_CodeTaxe3 = "C00";
                                                                    order.Lines[i].article.DL_Taxe3 = "0.000000";
                                                                    endTVA += 0.0;
                                                                }

                                                                double product_ht = Convert.ToDouble(order.Lines[i].PrixNetHT.Replace(".", ","));
                                                                double product_tva_P = (product_ht * Convert.ToDouble(endTVA)) / 100;
                                                                product_ttc = product_ht + product_tva_P;
                                                                order.Lines[i].article.DL_PUTTC = ("" + product_ttc).Replace(",", ".");

                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 1 => " + order.Lines[i].article.DL_Taxe1);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 2 => " + order.Lines[i].article.DL_Taxe2);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 3 => " + order.Lines[i].article.DL_Taxe3);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Prix HT => " + product_ht + " | TVA => " + endTVA + " % | Prix TTC => " + product_ttc);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Prix TTC créé");
                                                                logFileWriter_import.WriteLine("");
                                                            }
                                                            else
                                                            {
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : ******************** Warning TVA ********************");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BC seront 0");
                                                                logFileWriter_import.WriteLine("");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : ******************** Calcule TVA ******************** ");

                                                                double product_ht = Convert.ToDouble(order.Lines[i].PrixNetHT.Replace(".", ","));
                                                                double product_tva_P = (product_ht * 0.0) / 100;
                                                                product_ttc = product_ht + product_tva_P;
                                                                order.Lines[i].article.DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                                                order.Lines[i].article.DL_Taxe1 = "0.000000";
                                                                order.Lines[i].article.DL_CodeTaxe1 = "C00";
                                                                order.Lines[i].article.DL_Taxe2 = "0.000000";
                                                                order.Lines[i].article.DL_CodeTaxe2 = "C00";
                                                                order.Lines[i].article.DL_Taxe3 = "0.000000";
                                                                order.Lines[i].article.DL_CodeTaxe3 = "C00";


                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 1 => " + order.Lines[i].article.DL_Taxe1);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 2 => " + order.Lines[i].article.DL_Taxe2);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : TVA 3 => " + order.Lines[i].article.DL_Taxe3);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Prix HT => " + product_ht + " | TVA => 0.0 % | Prix TTC => " + product_ttc);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Prix TTC créé");
                                                                logFileWriter_import.WriteLine("");
                                                            }
                                                            order.Lines[i].PrixNetHT.Replace(",", ".");
                                                            logFileWriter_import.Flush();

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : ******************** Exception TVA ********************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : StackTrace :" + ex.StackTrace);
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Import annulée");
                                                            logFileWriter_import.Flush();
                                                            recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Erreur lors du calcule du prix d'article TTC, message : " + ex.Message, ex.StackTrace, filename.Name, logFileName_import));
                                                            nbr_ = 0;
                                                            break;
                                                        }

                                                        //add more info to BC
                                                        

                                                        //add BC history in the cmd lines
                                                        order.Lines[i].DO_Ref = order.NumCommande;
                                                        order.Lines[i].DL_PieceBC = order.Id;
                                                        order.Lines[i].DL_QteBC = order.Lines[i].Quantite;
                                                        order.Lines[i].DL_QteDE = order.Lines[i].Quantite;
                                                        order.Lines[i].DL_QtePL = order.Lines[i].Quantite;
                                                        order.Lines[i].DL_DateBC = order.DateCommande;
                                                        order.Lines[i].DL_DateDE = order.DateCommande;
                                                        order.Lines[i].DL_DatePL = "1753-01-01";

                                                        order.Lines[i].article.DL_PrixUnitaire_salePriceHT = order.Lines[i].PrixNetHT;

                                                        order.Lines[i].DL_MontantHT = (Convert.ToDouble(order.Lines[i].article.DL_PrixUnitaire_salePriceHT.Replace(".", ",")) * Convert.ToDouble(order.Lines[i].DL_QteBC.Replace(".", ","))).ToString().Replace(",",".");
                                                        order.Lines[i].DL_MontantTTC = (Convert.ToDouble(order.Lines[i].article.DL_PUTTC.Replace(".", ",")) * Convert.ToDouble(order.Lines[i].DL_QteBC.Replace(".", ","))).ToString().Replace(",", ".");

                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : DL_MontantHT : " + order.Lines[i].DL_MontantHT + " = DL_PrixUnitaire : " + order.Lines[i].article.DL_PrixUnitaire_salePriceHT + " X DL_QteBC : " + order.Lines[i].DL_QteBC);
                                                        logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : DL_MontantTTC : " + order.Lines[i].DL_MontantTTC + " = DL_PUTTC : " + order.Lines[i].article.DL_PUTTC + " X DL_QteBC : " + order.Lines[i].DL_QteBC);


                                                        if (insertCommandeLine(client, order, order.Lines[i], logFileWriter_import))
                                                        {
                                                            //Update docline artile stock
                                                            try
                                                            {
                                                                logFileWriter_import.WriteLine("");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Le Code TVA (" + order.Lines[i].article.DL_CodeTaxe1 + ") et la valuer du TVA (" + order.Lines[i].article.DL_Taxe1 + ")");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Additionner le total HT (" + order.Lines[i].PrixNetHT + ") et TTC (" + order.Lines[i].article.DL_PUTTC + ")");

                                                                taxCode = order.Lines[i].article.DL_CodeTaxe1;
                                                                taxValue = order.Lines[i].article.DL_Taxe1;
                                                                totalHT += Convert.ToDouble(order.Lines[i].PrixNetHT.Replace(".", ","));
                                                                totalTTC += Convert.ToDouble(order.Lines[i].article.DL_PUTTC.Replace(".", ","));


                                                                bool found_stock = false;
                                                                double AS_StockReel = 0.0;
                                                                double AS_StockReserve = 0.0;
                                                                double AS_StockMontant = 0.0;
                                                                double productPrixUnite = Convert.ToDouble(order.Lines[i].PrixNetHT.Replace('.', ','));

                                                                logFileWriter_import.WriteLine("");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : get current stock in F_ARTSTOCK table in the database");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : requette sql ===> " + QueryHelper.getArticleStock(true, order.Lines[i].article.AR_REF));
                                                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, order.Lines[i].article.AR_REF), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                                                                {
                                                                    using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                                                                    {
                                                                        if (reader.Read()) // If any rows returned
                                                                        {
                                                                            found_stock = true;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + ").");
                                                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                                                        }
                                                                        else // If no rows returned
                                                                        {
                                                                             //do nothing.
                                                                            found_stock = false;
                                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Aucune reponse.");
                                                                        }
                                                                    }
                                                                }


                                                                if (found_stock)
                                                                {
                                                                    //Calculate stock info
                                                                    //double AS_CMUP = AS_StockMontant / AS_StockReel;
                                                                    double new_AS_StockReel = AS_StockReel; // - Convert.ToDouble(order.Lines[i].Quantite.Replace('.', ','));
                                                                    double new_AS_StockReserve = AS_StockReserve + Convert.ToDouble(order.Lines[i].Quantite.Replace('.', ','));
                                                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                                                    logFileWriter_import.WriteLine("");
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : new_AS_StockReel: " + new_AS_StockReel);
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " + Quantite: " + Convert.ToDouble(order.Lines[i].Quantite.Replace('.', ',')));
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " * productPrixUnite: " + productPrixUnite);
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                                                    logFileWriter_import.WriteLine("");
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : update article " + order.Lines[i].article.AR_DESIGN + " (Ref:" + order.Lines[i].article.AR_REF + ") stock in F_ARTSTOCK table in the database");
                                                                    logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : requette sql ===> " + QueryHelper.updateArticleStock(true, order.Lines[i].article.AR_REF, new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant));

                                                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStock(true, order.Lines[i].article.AR_REF, new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant), connexion);
                                                                    command.ExecuteReader();
                                                                }

                                                                nbr_++;
                                                            }
                                                            catch (OdbcException ex)
                                                            {
                                                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                                                logFileWriter_import.WriteLine("");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Message :" + ex.Message);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : StackTrace :" + ex.StackTrace);
                                                                logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Import annulée");
                                                                logFileWriter_import.Flush();
                                                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", ex.Message, ex.StackTrace, filename.Name, logFileName_import));
                                                                deleteCommandeLigne(order.NumCommande);
                                                                nbr_ = 0;
                                                                break;
                                                            }

                                                        }

                                                    }
                                                    string mot = "";
                                                    for (int i = 0; i < MessageErreur.Count; i++)
                                                    {
                                                        mot = mot + MessageErreur[i] + "\n";
                                                    }

                                                    if (nbr_ == 0)
                                                    {
                                                        deleteCommande(false, order.NumCommande);

                                                        Console.WriteLine("" + nbr_ + "/" + order.Lines.Count + " ligne(s) Non enregistrée(s).\n" + mot);

                                                        logFileWriter_import.WriteLine(DateTime.Now + " : " + nbr_ + "/" + order.Lines.Count + " ligne(s) Non enregistrée(s), Document.\n" + mot);
                                                        tabCommande.Add(filename.Name);
                                                        recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", nbr_ + " / " + order.Lines.Count + " ligne(s) Non enregistrée(s).", mot, filename.Name, logFileName_import));
                                                        logFileWriter_import.WriteLine("");
                                                        connexion.Close();
                                                        //force to go at the end
                                                        goto goErrorLoop;
                                                    }
                                                    else
                                                    {
                                                        //Update BC with TVA, totalHT, totalTTC
                                                        try
                                                        {
                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : Ajouter : le code TVA, la valeur du tva, le montant HT et TTC dans le document " + order.NumCommande);
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() : SQL ===> " + QueryHelper.updateOrderValues(true, taxCode, taxValue, totalHT.ToString().Replace(",", "."), totalTTC.ToString().Replace(",", "."), order.Id));
                                                            OdbcCommand command_ = new OdbcCommand(QueryHelper.updateOrderValues(true, taxCode, taxValue, totalHT.ToString().Replace(",", "."), totalTTC.ToString().Replace(",", "."), order.Id), connexion);
                                                            IDataReader reader = command_.ExecuteReader(); // read rows of the executed query
                                                              
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine("*************** Exception updateOrderValues() ***************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() | Message : " + e.Message);
                                                            logFileWriter_import.WriteLine(DateTime.Now + " | insertOrder() | StackTrace : " + e.StackTrace);
                                                            tabCommande.Add(filename.Name);
                                                            recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Exception updateOrderValues()\n" +e.Message, e.StackTrace, filename.Name, logFileName_import));
                                                            logFileWriter_import.WriteLine("");
                                                            connexion.Close();
                                                            //force to go at the end
                                                            goto goErrorLoop;
                                                        }

                                                        logFileWriter_import.WriteLine("");

                                                        Console.WriteLine("" + nbr_ + "/" + order.Lines.Count + " ligne(s) enregistrée(s).\n" + mot);

                                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe du BC avec succès");
                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : " + nbr_ + "/" + order.Lines.Count + " ligne(s) enregistrée(s).\n" + mot);
                                                        logFileWriter_import.WriteLine("");

                                                        SaveSuccess++;

                                                        //deplacer les fichiers csv
                                                        string theFileName = filename.FullName;
                                                        string newFileLocation = directoryName_SuccessFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now) + "_" + file_doc_reference + "_" + System.IO.Path.GetFileName(theFileName);
                                                        File.Move(theFileName, newFileLocation);
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine("");
                                                        connexion.Close();
                                                        //force to go at the end
                                                        goto goErrorLoop;
                                                    }
                                                }
                                                else
                                                {
                                                    tabCommandeError.Add(filename.Name);
                                                    goto goErrorLoop;
                                                }
                                                
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Il faut mentionner le code client.");

                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Il faut mentionner le code client.");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                            tabCommandeError.Add(filename.Name);
                                            recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Il faut mentionner le code client.", "", filename.Name, logFileName_import));
                                            goto goErrorLoop;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erreur dans la troisième ligne du fichier.");

                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur dans la troisième ligne du fichier.");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Parametre: " + lines[2].Split(';')[0] + " || Size: " + lines[2].Split(';').Length);
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                        tabCommandeError.Add(filename.Name);
                                        recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Erreur dans la troisième ligne du fichier.", "", filename.Name, logFileName_import));
                                        goto goErrorLoop;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Date de la commande est incorrecte");

                                    logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Date de la commande est incorrecte.");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                    tabCommandeError.Add(filename.Name);
                                    recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Date de la commande est incorrecte.", "", filename.Name, logFileName_import));
                                    goto goErrorLoop;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Erreur dans la deuxième ligne du fichier.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Erreur dans la deuxième ligne du fichier.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", "Erreur dans la deuxième ligne du fichier.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }

                        }
                        else if (lines[0].Split(';')[0] == "INVPRT") //check if the document is an inventory stock document to handle further
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Stock Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Stock Inventaire.");

                            if (lines[0].Split(';').Length == 9) //check size of array to check if file format is correct
                            {
                                string reference_me_doc = get_next_num_piece_commande_v2("ME", logFileWriter_import); //lastNumberReference("ME", logFileWriter_import);    //"ME00004";//get last reference number for entry STOCK document MEXXXXX and increment it
                                string reference_ms_doc = get_next_num_piece_commande_v2("MS", logFileWriter_import); //lastNumberReference("MS", logFileWriter_import);    //"MS00007";//get last reference number for removal STOCK document MSXXXXX and increment it

                                int i = 0;
                                string totallines = "";
                                List<Stock> s = new List<Stock>();

                                foreach (string ligneDuFichier in lines) //read lines by line
                                {
                                    //MessageBox.Show("READING IMPORTED FILE");

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Lecture du fichier d'importation.");

                                    string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (tab[1] == "L") //checking if its an product line
                                    {
                                        Stock stock_info = new Stock("", tab[2], tab[3], tab[4], tab[5], "", ""); //creating new object type stock and storing values
                                        s.Add(stock_info); //adding the object into the list type stock
                                        i++;
                                    }

                                    if (tab[1] == "F") //checking if its end of file for control
                                    {
                                        totallines = tab[2];
                                    }
                                }

                                if (i != Convert.ToInt16(totallines)) //convert string to int : checking if number of items is equal to the number of items mentioned in the footer
                                {
                                    Console.WriteLine("Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page."); //display messagebox with error.

                                    logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le stock");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");


                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte.\r\nLa valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_me_doc + " | " + reference_ms_doc, "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le pied du page n'est pas en forme correcte.\r\nLa valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.", "", filename.Name, logFileName_import));
                                    goto goErrorLoop;
                                }
                                else
                                {
                                    //MessageBox.Show("INSERTSTOCK BEING CALLED");
                                    //insert or update the database with the values obtained from the document
                                    if (insertStock(s, reference_ms_doc, reference_me_doc, filename.Name, logFileWriter_import) != null)
                                    {
                                        Console.WriteLine("Le stock est importe avec succès");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information Fatale *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le stock est importe avec succès");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import succès");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine("");

                                        //deplacer les fichiers csv
                                        string theFileName = filename.FullName;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}_", DateTime.Now) + "_" + System.IO.Path.GetFileName(theFileName);
                                        File.Move(theFileName, newFileLocation);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine("");
                                        SaveSuccess++;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Nous n'avons pas pu importer le stock");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le stock");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine("");
                                        tabCommandeError.Add(filename.Name);
                                        goto goErrorLoop;
                                    }
                                }


                            }
                            else
                            {
                                Console.WriteLine("Le fichier n'est pas en bonne forme, merci de regarder son contenu."); //show error : content issue

                                logFileWriter_general.WriteLine("");
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;

                            }
                        }
                        else if (lines[0].Split(';')[0] == "L") //Import Veolog Stock doc
                        {
                            /*
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Veolog Stock Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Veolog Stock Inventaire.");

                            bool stockVeologCheck = false;
                            int lineCount = 0;
                            string totallines = "";
                            string[,] valid_info_stock_line = new string[lines.Length, 4];

                            //Loop the documment lines
                            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                            {
                                //Check if the document line is correct at each line
                                if (lines[lineIndex].Split(';').Length == 6)
                                {
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Verification Des Lignes Du Documment *********************");

                                    //  info_stock[0] == Type Ligne
                                    //  info_stock[1] == Code Article (reference)
                                    //  info_stock[2] == EAN (barcode)
                                    //  info_stock[3] == Stock

                                    string[] info_stock = lines[lineIndex].Split(';');

                                    Console.WriteLine("0: " + info_stock[0] + " | 1: " + info_stock[1] + " | 2: " + info_stock[2] + " | 3: " + info_stock[3] + " | 4: " + info_stock[4] + " | 5: " + info_stock[5]);

                                    if (info_stock[0] == "L")
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Type ligne => VALIDE ");


                                        try
                                        {
                                            if(info_stock[1] != "" && info_stock[3] != "")
                                            {
                                                logFileWriter_import.WriteLine(DateTime.Now + " :  Code acticle ("+ info_stock[1] + ") => TROUVE ");
                                                valid_info_stock_line[lineIndex, 0] = info_stock[1];

                                                logFileWriter_import.WriteLine(DateTime.Now + " : EAN (Code Barre) \"" + info_stock[2] + "\" => TROUVE ");
                                                valid_info_stock_line[lineIndex, 1] = info_stock[2];

                                                logFileWriter_import.WriteLine(DateTime.Now + " : Stock actuel ("+ info_stock[3] + ") => TROUVE ");
                                                valid_info_stock_line[lineIndex, 2] = info_stock[3];

                                                logFileWriter_import.WriteLine(DateTime.Now + " : Numéro de lot (" + info_stock[4] + ")  => TROUVE ");
                                                valid_info_stock_line[lineIndex, 3] = info_stock[4];
                                            }
                                            else
                                            {
                                                if (info_stock[1] == "")
                                                {
                                                    logFileWriter_import.WriteLine("");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Code acticle => PAS TROUVE ");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                                }
                                                if (info_stock[3] == "")
                                                {
                                                    logFileWriter_import.WriteLine("");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Quantités disponibles (Stock actuel) => PAS TROUVE ");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                    logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Dans le Documment *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1));
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Message |\n" + ex.Message);
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Stack |\n" + ex.StackTrace);
                                            tabCommandeError.Add(filename.Name);
                                            importErrorList.Add(new string[3] { filename.Name, "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", logFileName_import });
                                        }

                                        lineCount++;
                                        logFileWriter_import.Flush();
                                    }
                                    else
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Type ligne ===> PAS RECONNU / PAS TROUVE");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le champ n'est pas correct ou vide!!! ");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                        tabCommandeError.Add(filename.Name);
                                        importErrorList.Add(new string[3] { filename.Name, "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", logFileName_import });
                                    }
                                    
                                }
                                else
                                {
                                    if (lines[lineIndex].Split(';')[0] == "F" && lines[lineIndex].Split(';').Length == 2)
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Fin de la lecture du documment Veolog Stock.");
                                        totallines = lines[lineIndex].Split(';')[1];
                                        stockVeologCheck = true;
                                        logFileWriter_import.Flush();
                                        break;
                                    }
                                    logFileWriter_import.Flush();
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu. Ligne size == "+ lines[lineIndex].Split(';').Length);
                                    tabCommandeError.Add(filename.Name);
                                    importErrorList.Add(new string[3] { filename.Name, "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", logFileName_import });
                                    goto goErrorLoop;
                                }
                            }

                            //Valid documment END
                            //Reached to the end of the document
                            List<Stock> s = new List<Stock>();
                            if (stockVeologCheck)
                            {
                                //  valid_info_stock_line[0,0] == Code Article (reference)
                                //  valid_info_stock_line[0,1] == EAN (barcode)
                                //  valid_info_stock_line[0,2] == Stock
                                //  valid_info_stock_line[0,3] == Numéro de lot 

                                for (int x = 0; x < valid_info_stock_line.GetLength(0); x++)
                                {
                                    Stock stock_info = new Stock("", valid_info_stock_line[x, 0], valid_info_stock_line[x, 1], valid_info_stock_line[x, 2], valid_info_stock_line[x, 3], "", "");
                                    s.Add(stock_info); //adding the object into the list type stock
                                }

                                if (lineCount != Convert.ToInt16(totallines))
                                {
                                    logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Erreur du pied de page");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte.\r\nLa valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.\nCertain stock ne sera pas mit a jour!!!");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : La taille du Stock liste: " + lineCount + " || Nombre total du stock dans le fihier: " + totallines);

                                    logFileWriter_general.Flush();
                                    logFileWriter_import.Flush();
                                    tabCommandeError.Add(filename.Name);
                                    importErrorList.Add(new string[3] { filename.Name, "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", logFileName_import });
                                }
                                else
                                {
                                    logFileWriter_import.Flush();
                                    if (insertStockVeolog(s, logFileWriter_import) == true)
                                    {
                                        logFileWriter_general.Flush();
                                        logFileWriter_import.Flush();
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe avec succès");

                                        //deplacer les fichiers csv
                                        string theFileName = filename.FullName;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now) + "_" + System.IO.Path.GetFileName(theFileName);
                                        File.Move(theFileName, newFileLocation);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine("");
                                        SaveSuccess++;
                                    }
                                    else
                                    {
                                        logFileWriter_general.Flush();
                                        logFileWriter_import.Flush();
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le stock");

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le stock");
                                        logFileWriter_import.WriteLine("");
                                        tabCommandeError.Add(filename.Name);
                                        importErrorList.Add(new string[3] { filename.Name, "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", logFileName_import });
                                        goto goErrorLoop;
                                    }
                                }
                            }
                            else
                            {
                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Fin de la lecture du documment Veolog Stock, non valide.");
                            }
                            */
                        }
                        else if (lines[0].Split(';')[0] == "DESADV") //check if the document is an desadv stock document to handle further
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier DESADV Trouvé");
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import DESADV.");

                            if (lines[0].Split(';').Length == 9) //check size of array to check if file format is correct
                            {
                                int i = 0;
                                string totallines = "";
                                Desadv d = new Desadv(); //creating new object type desadv and storing values
                                List<DesadvLine> dl = new List<DesadvLine>(); //creating new object type desadvline and storing item values

                                foreach (string ligneDuFichier in lines) //read lines by line
                                {

                                    string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (tab[1] == "HEADER") //checking if its header of file for control
                                    {
                                        Desadv desadv_info = new Desadv();

                                        desadv_info.reference = tab[2];
                                        desadv_info.datelivraison = tab[3];
                                        desadv_info.datecreation = DateTime.UtcNow.ToString("yyyyMMddHHmmss").ToString();
                                        desadv_info.poids = tab[4];
                                        desadv_info.expeditiontype = tab[5];
                                        desadv_info.referenceclient = tab[6];

                                        d = desadv_info; //adding the object into the list type stock
                                    }

                                    if (tab[1] == "LINES") //checking if its line of document inside the file for control
                                    {
                                        DesadvLine desadvline_info = new DesadvLine();

                                        desadvline_info.position = tab[2];
                                        desadvline_info.libelle = tab[3];
                                        desadvline_info.barcode = tab[4];
                                        desadvline_info.qtecommandee = tab[5];
                                        desadvline_info.qtyexpediee = tab[6];
                                        desadvline_info.poidproduit = tab[7];
                                        desadvline_info.volumeproduit = tab[8];

                                        dl.Add(desadvline_info);
                                    }

                                    i++;
                                }

                                // *once list is filled with values, start executing queries for each line - one by one.*

                                if (i != Convert.ToInt16(totallines)) //convert string to int : checking if number of items is equal to the number of items mentioned in the footer (optional for desadv document)
                                {
                                    Console.WriteLine("Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page."); //display messagebox with error.

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.");
                                    logFileWriter_import.Flush(); 
                                    tabCommandeError.Add(filename.Name);
                                    recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.", "", filename.Name, logFileName_import));
                                    goto goErrorLoop;
                                }
                                else
                                {
                                    //insertDesadv(d, dl);//insert or update the database with the values obtained from the document
                                    SaveSuccess++;
                                }

                            }
                            else
                            {
                                Console.WriteLine("Le fichier n'est pas en bonne forme, merci de regarder son contenu."); //show error : content issue

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }
                        }
                        else if (lines[0].Split(';')[0] == "E" && filename.Name.Contains("CFP51") || filename.Name.Contains("TWP51")) //Import Veolog DESADV doc
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Veolog DESADV Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Veolog DESADV Inventaire.");

                            if (lines[0].Split(';').Length == 6)
                            {
                                file_doc_reference = lines[0].Split(';')[1];
                                string reference_DESADV_doc = get_next_num_piece_commande_v2("BL", logFileWriter_import); //lastNumberReference("BL", logFileWriter_import);    //get last reference number for desadv STOCK document MEXXXXX and increment it

                                int i = 0;
                                string totallines = "";
                                Veolog_DESADV dh = new Veolog_DESADV();
                                //Veolog_DESADV_Colis dc = new Veolog_DESADV_Colis();
                                Veolog_DESADV_Lines dll = new Veolog_DESADV_Lines();

                                List<String> doubleProductCheck = new List<String>();
                                List<Veolog_DESADV_Lines> dl = new List<Veolog_DESADV_Lines>(); //creating new object type desadvline and storing item values

                                foreach (string ligneDuFichier in lines) //read lines by line
                                {
                                    string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (tab[0] == "E") //checking if its header of file for control
                                    {
                                        Veolog_DESADV desadv_info = new Veolog_DESADV();
                                        desadv_info.Ref_Commande_Donneur_Ordre = tab[1];
                                        desadv_info.Ref_Commande_Client_Livre = tab[2];
                                        desadv_info.Date_De_Expedition = tab[3];
                                        desadv_info.Heure_De_Expedition = tab[4];
                                        desadv_info.Etat = tab[5];

                                        if (desadv_info.Etat == "X")
                                        {
                                            desadv_info.Etat = "1";
                                        }
                                        else if (desadv_info.Etat == "P")
                                        {
                                            desadv_info.Etat = "0";
                                        }
                                        else
                                        {
                                            //deplacer les fichiers csv
                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le DESADV");

                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Le champ 'Etat' dans l'entête du fichier n'est pas valide!\nUn Etat valide est soit X : Expédié ou P : Préparé.");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                            logFileWriter_import.Flush();
                                            tabCommandeError.Add(filename.Name);
                                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le champ 'Etat' dans l'entête du fichier n'est pas valide!\nUn Etat valide est soit X : Expédié ou P : Préparé.", "", filename.Name, logFileName_import));
                                            goto goErrorLoop;
                                        }

                                        dh = desadv_info;
                                    }
                                    /*
                                    if (tab[0] == "C") //checking if its colis of file for control
                                    {
                                        Veolog_DESADV_Colis desadvColis_info = new Veolog_DESADV_Colis();
                                        desadvColis_info.Numero_Colis = tab[1];
                                        desadvColis_info.ID_Tracking_Transporteur = tab[2];
                                        desadvColis_info.URL_Tracking_Transporteur = tab[3];

                                        dc = desadvColis_info; //adding the object into the list type stock
                                    }
                                    */
                                    if (tab[0] == "L") //checking if its line of document inside the file for control
                                    {
                                        //check if an article exist in my check list
                                        if (!doubleProductCheck.Contains(tab[2]))
                                        {
                                            Veolog_DESADV_Lines desadvLine_info = new Veolog_DESADV_Lines();

                                            desadvLine_info.Numero_Ligne_Order = tab[1];
                                            desadvLine_info.Code_Article = tab[2];
                                            desadvLine_info.Quantite_Colis = tab[3];
                                            desadvLine_info.Numero_Lot = tab[4];

                                            dl.Add(desadvLine_info);
                                            doubleProductCheck.Add(desadvLine_info.Code_Article);
                                        }
                                        else
                                        {
                                            logFileWriter_general.WriteLine("");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Nous avons trouvé cette \""+tab[2]+"\" encore!");
                                            logFileWriter_general.WriteLine("");
                                        }
                                        i++;
                                    }

                                    if (tab[0] == "F") //checking if its end of file for control
                                    {
                                        totallines = tab[1];
                                    }
                                }

                                // *once list is filled with values, start executing queries for each line - one by one.*

                                if (i != Convert.ToInt16(totallines)) //convert string to int : checking if number of items is equal to the number of items mentioned in the footer (optional for desadv document)
                                {
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.");
                                }
                                else
                                {
                                    if (insertDesadv_Veolog(reference_DESADV_doc, dh, dl, filename.Name, logFileWriter_import) != null) //insert or update the database with the values obtained from the document
                                    {
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe du DESADV avec succès");

                                        //deplacer les fichiers csv
                                        string theFileName = filename.FullName;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now) + "_" + reference_DESADV_doc + "_" + System.IO.Path.GetFileName(theFileName);
                                        File.Move(theFileName, newFileLocation);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine("");
                                        SaveSuccess++;
                                    }
                                    else
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le DESADV");
                                        tabCommandeError.Add(filename.Name);
                                        goto goErrorLoop;
                                    }
                                }
                            }
                            else
                            {
                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le fichier n'est pas en bonne forme, merci de regarder son contenu.", "", filename.Name, logFileName_import));
                                tabCommandeError.Add(filename.Name);
                                goto goErrorLoop;
                            }
                        }
                        else if (lines[0].Split(';')[0] == "E" && filename.Name.Contains("CFP41") || filename.Name.Contains("TWP41")) //Import Veolog BLF doc
                        {
                            /*
                            tabCommandeError.Add(filename.Name);
                            recapLinesList_new.Add(new CustomMailRecapLines("", "L'import de la commande est annulée. Erreur dans le fichier EDI", "L'import du BLF est désactivé !", "", filename.Name, logFileName_import));
                            goto goErrorLoop;
                            */
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Veolog Bon de Livraison Fournisseur BLF Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Veolog Bon de Livraison Fournisseur BLF Inventaire.");

                            file_doc_reference = lines[0].Split(';')[1];
                            if (lines[0].Split(';').Length == 8)
                            {
                                ConfigurationDNS dns = new ConfigurationDNS();
                                dns.LoadSQL();

                                string mask = "";
                                string prefix = dns.Prefix;
                                if(prefix == "CFCI")
                                {
                                    mask = "BLF";
                                }
                                else if (prefix == "TABLEWEAR")
                                {
                                    mask = "LF";
                                }
                                else
                                {
                                    mask = "BLF";
                                }

                                string reference_BLF_doc = get_next_num_piece_commande_v2(mask, logFileWriter_import); // lastNumberReference(mask, logFileWriter_import);

                                int i = 0;
                                string totallines = "";
                                Veolog_BCF dh = new Veolog_BCF();
                                Veolog_BCF_Lines dll = new Veolog_BCF_Lines();

                                List<String> doubleProductCheck = new List<String>();
                                List<Veolog_BCF_Lines> dl = new List<Veolog_BCF_Lines>(); //creating new object type BCF line and store item values

                                foreach (string ligneDuFichier in lines) //read lines by line
                                {
                                    string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (tab[0] == "E") //checking if its header of file for control
                                    {
                                        Veolog_BCF bcf_info = new Veolog_BCF();
                                        bcf_info.Ref_Commande_Donneur_Ordre = tab[1];
                                        bcf_info.Ref_Commande_Fournisseur = tab[2];
                                        bcf_info.Origine_Commande = tab[3];
                                        bcf_info.Code_Fournisseur = tab[4];
                                        bcf_info.Date_De_Reception = tab[5];
                                        bcf_info.Heure_De_Reception = tab[6];
                                        bcf_info.Etat = tab[7];

                                        if (bcf_info.Etat == "S") // S : Stocké
                                        {
                                            bcf_info.Etat = "1";
                                        }
                                        else if (bcf_info.Etat == "C") // C : Cloturé
                                        {
                                            bcf_info.Etat = "0";
                                        }
                                        else if(bcf_info.Etat == "")
                                        {
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Le "+ mask + " " + reference_BLF_doc + " n'a pas d'Etat ou Option. Les options depuis le cahier des charges sont : S => Stocké ou C => Cloturé");
                                        }
                                        else
                                        {
                                            //deplacer les fichiers csv
                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le "+ mask);

                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Le champ 'Etat' dans l'entête du fichier n'est pas valide!\nUn Etat valide est soit S : Stocké ou C : Cloturé.");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                            logFileWriter_import.Flush();
                                            tabCommandeError.Add(filename.Name);
                                            recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "Le champ 'Etat' dans l'entête du fichier n'est pas valide!\nUn Etat valide est soit S : Stocké ou C : Cloturé.", "", filename.Name, logFileName_import));
                                            goto goErrorLoop;
                                        }

                                        dh = bcf_info;
                                    }
                                    if (tab[0] == "L") //checking if its line of document inside the file for control
                                    {
                                        //check if an article exist in my check list
                                        if (!doubleProductCheck.Contains(tab[2]))
                                        {
                                            Veolog_BCF_Lines bcfLine_info = new Veolog_BCF_Lines();

                                            bcfLine_info.Type_Ligne = tab[0];
                                            bcfLine_info.Numero_Ligne_Donneur_Ordre = tab[1];
                                            bcfLine_info.Code_Article = tab[2];
                                            bcfLine_info.Libelle_Article = tab[3];
                                            bcfLine_info.Quantite = tab[4];
                                            bcfLine_info.Numero_Lot = tab[5];

                                            dl.Add(bcfLine_info);
                                            doubleProductCheck.Add(bcfLine_info.Code_Article);
                                        }
                                        else
                                        {
                                            logFileWriter_general.WriteLine("");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Nous avons trouvé cette \"" + tab[2] + "\" encore!");
                                            logFileWriter_general.WriteLine("");
                                        }
                                        i++;
                                    }

                                    if (tab[0] == "F") //checking if its end of file for control
                                    {
                                        totallines = tab[1];
                                    }
                                }


                                if (i != Convert.ToInt16(totallines)) //convert string to int : checking if number of items is equal to the number of items mentioned in the footer (optional for desadv document)
                                {
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte. La valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.");
                                }
                                else
                                {
                                    if (insertSupplierOrder(reference_BLF_doc, dh, dl, filename.Name, logFileWriter_import) != null) //insert the database with the values obtained from the document
                                    {
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe du "+ mask + " avec succès");

                                        //deplacer les fichiers csv
                                        string theFileName = filename.FullName;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now) + "_" + file_doc_reference + "_" + System.IO.Path.GetFileName(theFileName);
                                        File.Move(theFileName, newFileLocation);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine("");
                                        SaveSuccess++;
                                    }
                                    else
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le "+ mask);
                                        tabCommandeError.Add(filename.Name);
                                        goto goErrorLoop;
                                    }
                                }
                            }
                            else
                            {
                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La premier ligne du fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                logFileWriter_import.Flush();
                                tabCommandeError.Add(filename.Name);
                                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "La premier ligne du fichier n'est pas en bonne forme, merci de regarder son contenu.", "", filename.Name, logFileName_import));
                                goto goErrorLoop;
                            }
                            
                            //END   BLF/LF
                        }
                        else
                        {

                            //Console.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                            logFileWriter_import.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                            logFileWriter_import.Flush();
                            tabCommandeError.Add(filename.Name);
                            recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée. Erreur dans le fichier EDI", "La premier ligne du fichier n'est pas en bonne forme, merci de regarder son contenu.", "", filename.Name, logFileName_import));
                            goto goErrorLoop;
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                        logFileWriter_import.WriteLine("");
                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Exception *********************");
                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur StackTrace :: " + e.StackTrace);

                        logFileWriter_general.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                        logFileWriter_import.Flush();
                        tabCommandeError.Add(filename.Name);
                        recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import de la commande est annulée.", "Erreur[16] : " +e.Message, e.StackTrace, filename.Name, logFileName_import));
                    }

                goErrorLoop:;

                    //Deplaçer les fichier dans le dossier : Error File SI IL Y A DES ERREUR .....
                    if (File.Exists(dir + @"\" + filename) && tabCommandeError.Count > 0)
                    {
                        logFileWriter_general.Flush();
                        logFileWriter_import.Flush();
                        logFileWriter_import.WriteLine("");
                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Fichier *********************");
                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");

                        //deplacer les fichiers csv
                        string theFileName = filename.FullName;
                        string newFileLocation = directoryName_ErrorFile + @"\" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now) + "__" + file_doc_reference + "_" + System.IO.Path.GetFileName(theFileName);
                        File.Move(theFileName, newFileLocation);
                        logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);

                    }

                    //End .csv loop
                    tabCommandeError.Clear();
                    logFileWriter_general.WriteLine("");
                    logFileWriter_import.WriteLine("");
                    logFileWriter_general.Flush();
                    logFileWriter_import.Flush();
                }


                logFileWriter_import.Flush();
                logFileWriter_import.Close();
            }   //End of StreamWriter logFileWriter_import


            // create Mail_IMP.ml file for
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine(DateTime.Now + " | Save Mail file");

            try
            {
                ConfSendMail cMail = getInfoMail(logFileWriter_general);
                Console.WriteLine("recapLinesList_new size: "+ recapLinesList_new.Count);
                if (cMail != null && cMail.active && recapLinesList_new.Count != 0)
                {
                    string[] dateTime = string.Format("{0:yyyyMMdd_HHmm}", DateTime.Now).Split('_');
                    ConfigurationDNS dns = new ConfigurationDNS();
                    //dns.Load();
                    dns.Load();
                    CustomMailRecap recap_new = new CustomMailRecap();
                    List<string> attchmentsList = new List<string>();

                    recap_new.MailType = "Mail_IMP";
                    recap_new.Client = dns.Prefix;

                    if (recapLinesList_new.Count == 1)
                    {
                        recap_new.Subject = "Erreur d'import d'un document";
                    }
                    else if (recapLinesList_new.Count > 2)
                    {
                        recap_new.Subject = "Erreur d'import des documents";
                    }
                    recap_new.DateTimeCreated = string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now);
                    recap_new.DateTimeModified = "";

                    recap_new.Lines = new List<CustomMailRecapLines>();
                    for (int i = 0; i < recapLinesList_new.Count; i++)
                    {
                        if (!attchmentsList.Contains(recapLinesList_new[i].FilePath))
                        {
                            attchmentsList.Add(recapLinesList_new[i].FilePath);
                        }
                        recap_new.Lines.Add(recapLinesList_new[i]);
                    }
                    recap_new.Attachments = attchmentsList;
                    recap_new.saveInfo(recap_new, "Mail_IMP.ml");

                    logFileWriter_general.WriteLine(DateTime.Now + " | New Mail_IMP.ml file created!");


                    if (File.Exists("Mail_Recap.ml"))
                    {
                        logFileWriter_general.WriteLine("");
                        logFileWriter_general.WriteLine(DateTime.Now + " | Mail_Recap.ml file existe!");

                        List<string> recapLinesRef = new List<string>();
                        List<string> attchmentsList_1 = new List<string>();
                        CustomMailRecap newRecap = new CustomMailRecap();

                        CustomMailRecap recap = new CustomMailRecap();
                        recap.Load("Mail_Recap.ml");
                        recap.DateTimeModified = string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now);

                        //load list
                        for (int i = 0; i < recap.Lines.Count; i++)
                        {
                            recapLinesRef.Add(recap.Lines[i].DocumentReference);
                            if (!attchmentsList_1.Contains(recap.Lines[i].FilePath))
                            {
                                attchmentsList_1.Add(recap.Lines[i].FilePath);
                            }
                        }

                        for (int i = 0; i < recapLinesList_new.Count; i++)
                        {
                            //check if the doc is still in error
                            if (recapLinesRef.Contains(recapLinesList_new[i].DocumentReference))
                            {
                                recapLinesList_new[i].Increment ++;
                                newRecap.Lines.Add(recapLinesList_new[i]);
                            }
                            else
                            {
                                newRecap.Lines.Add(recapLinesList_new[i]);
                            }

                            if (!attchmentsList_1.Contains(recapLinesList_new[i].FilePath))
                            {
                                attchmentsList_1.Add(recapLinesList_new[i].FilePath);
                            }
                        }

                        newRecap.Attachments = attchmentsList_1;
                        newRecap.saveInfo(newRecap, "Mail_Recap.ml");
                        logFileWriter_general.WriteLine(DateTime.Now + " | Create Mail_Recap.ml file!");
                        Console.WriteLine("newRecap.Client: " + newRecap.Client);
                    }
                    else
                    {
                        logFileWriter_general.WriteLine("");
                        logFileWriter_general.WriteLine(DateTime.Now + " | New Mail_Recap.ml file created!");
                        CustomMailRecap recap = new CustomMailRecap();
                        List<string> attchmentsList_1 = new List<string>();

                        recap.MailType = "Mail_RECAP";
                        recap.Client = dns.Prefix;
                        recap.Subject = "Récapitulatif des erreurs de document éventuelles / restantes";
                        recap.DateTimeCreated = string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now);
                        recap.DateTimeModified = "";

                        Console.WriteLine("recap.Client: " + recap.Client);

                        recap.Lines = new List<CustomMailRecapLines>();
                        for (int i=0; i<recapLinesList_new.Count; i++)
                        {
                            if (!attchmentsList_1.Contains(recapLinesList_new[i].FilePath))
                            {
                                attchmentsList_1.Add(recapLinesList_new[i].FilePath);
                            }
                            recapLinesList_new[i].Increment += 1;
                            recap.Lines.Add(recapLinesList_new[i]);
                        }
                        recap_new.Attachments = attchmentsList_1;
                        recap.saveInfo(recap, "Mail_Recap.ml");
                        logFileWriter_general.WriteLine(DateTime.Now + " | Mail_Recap.ml file created!");
                    }

                    recapLinesList_new.Clear();
                }
                else if (recapLinesList_new.Count == 0)
                {
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine(DateTime.Now + " | No errors found to add in the mail !");
                }
                else if(cMail != null)
                {
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine(DateTime.Now + " | Mail configurations are null !");
                }
                else if(cMail.active != true)
                {
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine(DateTime.Now + " | Mail configuration is not active !");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("********** Execption Mail files **********");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            


            if (!FileExiste)
            {
                //Console.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
                goto goError;

            }
            if (FileExiste && tabCommandeError.Count == 0)
            {
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
                logFileWriter_general.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                logFileWriter_general.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
            }

            goError:;

            //logFileWriter_general.Flush();
            //logFileWriter_general.Close();
        }

        public static Boolean insertCommande(Client client, Order order, StreamWriter writer, string filename)
        {
            writer.WriteLine(DateTime.Now + " | insertCommande() : Called!");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | insertCommande() : SQL ===> " + QueryHelper.insertCommande(false, client, order));
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertCommande(false, client, order), connection);
                    
                    command.ExecuteReader();
                    connection.Close();

                    writer.WriteLine(DateTime.Now + " | insertCommande() : Commande enregisté!!!");
                    return true;


                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " | insertCommande() : ERREUR[4]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    recapLinesList_new.Add(new CustomMailRecapLines(order.Id, order.NumCommande, "L'import de la commande est annulée.", ex.Message, ex.StackTrace, filename, logFileName_import));

                    deleteCommande(true, order.Id);

                    return false;
                }
            }

        }

        public static Boolean insertCommandeLine(Client client, Order order, OrderLine orderLine, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | insertCommandeLine() : Called!");
            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | insertCommandeLine() : SQL ===> " + QueryHelper.insertLigneCommande(true, client, order, orderLine));
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertLigneCommande(true, client, order, orderLine), connection);
                    
                    command.ExecuteReader();
                    writer.WriteLine(DateTime.Now + " | insertCommandeLine() : Ligne enregistré!!! ");
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageErreur.Add(DateTime.Now + " | insertCommandeLine() : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + "." + "\n" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", "")+"\n");
                    return false;
                }
            }
        }

        public static string[,] insertStock(List<Stock> s, string reference_MS_doc, string reference_ME_doc, string fileName, StreamWriter logFileWriter)
        {
            logFileWriter.WriteLine(DateTime.Now + " | insertStock() ");

            //List<Stock> s : values obtained from the document received/imported.
            //reference_doc : the last reference of the document that is to be imported. format ME______ - "ME" because its an entry OR MS______ - "MS" because its a removal
            //string[][] list_of_products = new string[s.Count][];  ===> not how you declare 2 dimensional arrays
            string[,] list_of_products = new string[s.Count, 27];    // new string [x,y]
            int positive_item = 0;
            int negative_item = 0;
            DateTime d = DateTime.Now;
            string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_time = "000" + d.ToString("hhmmss");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            using (OdbcConnection connection = Connexion.CreateOdbcConnextion()) //connecting to database as handler
            {
                try
                {
                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Connexion ouverte.");

                    int counter = 0;

                    foreach (Stock line in s) //read item by item
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Lire la ligne de l'article.");

                        int total_negative = 0;
                        int total_positive = 0;
                        string name_article = "";
                        string DL_PoidsNet = "0";
                        string DL_PoidsBrut = "0";
                        string DL_PrixUnitaire = "0";

                        // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

                        //getProductNameByReference
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference(false, line.reference), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    name_article = (reader[0].ToString());  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[1].ToString()); // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[2].ToString()); // get unit weight BRUT - check query  
                                    DL_PrixUnitaire = (reader[3].ToString()); // get unit price  - check query 
                                }
                                else // If no rows returned
                                {
                                    //do nothing.
                                }
                            }
                        }

                        if (name_article != "")
                        {
                            using (OdbcConnection connectionSQL = Connexion.CreateOdbcConnexionSQL()) //connecting to database as handler
                            {
                                connectionSQL.Open();
                                using (OdbcCommand command = new OdbcCommand(QueryHelper.getNegativeStockOfAProduct(true, line.reference), connectionSQL)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Exécuter la requête");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : " + QueryHelper.getNegativeStockOfAProduct(true, line.reference));
                                        while (reader.Read()) // reads lines/rows from the query
                                        {
                                            char split = ',';
                                            string[] value = reader[0].ToString().Split(split);
                                            total_negative = Convert.ToInt16(value[0]);
                                            logFileWriter.WriteLine(DateTime.Now + " : total_negative = " + total_negative);
                                        }
                                    }
                                }
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : getNegativeStockOfAProduct OK.");
                                logFileWriter.WriteLine("");

                                using (OdbcCommand command = new OdbcCommand(QueryHelper.getPositiveStockOfAProduct(true, line.reference), connectionSQL)) //execute the function within this statement : getPositiveStockOfAProduct()
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Exécuter la requête");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : " + QueryHelper.getPositiveStockOfAProduct(true, line.reference));

                                    using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                    {
                                        while (reader.Read()) // reads lines/rows from the query
                                        {
                                            char split = ',';
                                            string[] value = reader[0].ToString().Split(split);
                                            total_positive = Convert.ToInt16(value[0]);
                                            logFileWriter.WriteLine(DateTime.Now + " : total_positive = " + total_positive);
                                        }
                                    }
                                }
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : getPositiveStockOfAProduct OK.");

                                connectionSQL.Close();
                            }

                            int current_stock = (total_positive - total_negative); // substract negative stock from the positive one = to obtain the initial current stock.
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Reference: " + line.reference + " total_positive: " + total_positive + " - total_negative: " + total_negative + " = current_stock : " + current_stock + ".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : current_stock OK = curr:" + current_stock + " === veo:" + Convert.ToInt16(line.stock) + ".");

                            if (current_stock > Convert.ToInt16(line.stock)) // if current stock in database is superior from the one received in file : means remove stock
                            {
                                //MessageBox.Show("current_stock : " + current_stock + " > " + "Line.stock : " + Convert.ToInt16(line.stock));
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : current_stock : " + current_stock + " > " + "Line.stock : " + Convert.ToInt16(line.stock) + ".");

                                try
                                {
                                    negative_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : negativ_item: " + negative_item + ".");

                                    // MS00000 : MS prefix will be used to create document
                                    list_of_products[counter, 0] = "2"; // DO_Domaine
                                    list_of_products[counter, 1] = "21"; //DO_Type
                                    list_of_products[counter, 2] = "21"; //DO_DocType
                                    list_of_products[counter, 3] = "1"; //CT_NUM
                                    list_of_products[counter, 4] = reference_MS_doc; //DO_Piece
                                    list_of_products[counter, 5] = curr_date; //DO_Date
                                    list_of_products[counter, 6] = curr_date; //DL_DateBC
                                    list_of_products[counter, 7] = (negative_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_products[counter, 8] = curr_date_seconds; // DO_Ref
                                    list_of_products[counter, 9] = line.reference; // AR_Ref
                                    list_of_products[counter, 10] = "1"; //DL_Valorise
                                    list_of_products[counter, 11] = "1"; //DE_NO
                                    list_of_products[counter, 12] = name_article.Replace("'","''"); // DL_Design
                                    list_of_products[counter, 13] = (Convert.ToInt16(current_stock) - Convert.ToInt16(line.stock)).ToString().Replace(",", ".").Replace("-", "");  //line.stock; // DL_Qte
                                    list_of_products[counter, 14] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PoidsNet)).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_products[counter, 14].Equals("0")) { list_of_products[counter, 14] = "0.000000"; } else if (!list_of_products[counter, 14].Contains(".")) { list_of_products[counter, 14] = list_of_products[counter, 14] + ".000000"; }
                                    list_of_products[counter, 15] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PoidsBrut)).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_products[counter, 15].Equals("0")) { list_of_products[counter, 15] = "0.000000"; } else if (!list_of_products[counter, 15].Contains(".")) { list_of_products[counter, 15] = list_of_products[counter, 15] + ".000000"; }
                                    list_of_products[counter, 16] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_products[counter, 16].Equals("0")) { list_of_products[counter, 16] = "0.000000"; } else if (!list_of_products[counter, 16].Contains(".")) { list_of_products[counter, 16] = list_of_products[counter, 16] + ".000000"; }
                                    list_of_products[counter, 17] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_products[counter, 17].Equals("0")) { list_of_products[counter, 17] = "0.000000"; } else if (!list_of_products[counter, 17].Contains(".")) { list_of_products[counter, 17] = list_of_products[counter, 17] + ".000000"; }
                                    list_of_products[counter, 18] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_products[counter, 19] = DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_products[counter, 20] = (Convert.ToInt16(current_stock) - Convert.ToInt16(line.stock)).ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
                                    if (list_of_products[counter, 20].Equals("0")) { list_of_products[counter, 20] = "0.000000"; } else if (!list_of_products[counter, 20].Contains(".")) { list_of_products[counter, 20] = list_of_products[counter, 20] + ".000000"; }
                                    list_of_products[counter, 21] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_products[counter, 22] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_products[counter, 20].Equals("0")) { list_of_products[counter, 20] = "0.000000"; } else if (!list_of_products[counter, 20].Contains(".")) { list_of_products[counter, 20] = list_of_products[counter, 20] + ".000000"; }
                                    if (list_of_products[counter, 21].Equals("0")) { list_of_products[counter, 21] = "0.0"; } else if (!list_of_products[counter, 21].Contains(".")) { list_of_products[counter, 21] = list_of_products[counter, 21] + ".0"; }
                                    if (list_of_products[counter, 22].Equals("0")) { list_of_products[counter, 22] = "0.000000"; } else if (!list_of_products[counter, 22].Contains(".")) { list_of_products[counter, 22] = list_of_products[counter, 22] + ".000000"; }
                                    list_of_products[counter, 23] = ""; //PF_Num
                                    list_of_products[counter, 24] = "0"; //DL_No
                                    list_of_products[counter, 25] = "0"; //DL_FactPoids
                                    list_of_products[counter, 26] = "0"; //DL_Escompte

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** Exception ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Le tableau 'MS' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                    logFileWriter.Flush();
                                    logFileWriter.Close();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", "Le tableau 'MS' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                //MessageBox.Show("Product added into the table list_of_products as MS index ");

                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Produit '" + name_article + "' est ajouté à la table list_of_products en tant qu'index MS.");
                            }


                            if (current_stock < Convert.ToInt16(line.stock)) // if current stock in database is inferior from the one received in file : means add stock
                            {
                                //MessageBox.Show("current_stock : " + current_stock + " < " + "Line.stock : " + Convert.ToInt16(line.stock));
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : current_stock : " + current_stock + " < " + "Line.stock : " + Convert.ToInt16(line.stock) + ".");


                                try
                                {
                                    positive_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    // ME00000 : ME prefix will be used to create document
                                    list_of_products[counter, 0] = "2"; // DO_Domaine
                                    list_of_products[counter, 1] = "20"; //DO_Type
                                    list_of_products[counter, 2] = "20"; //DO_DocType
                                    list_of_products[counter, 3] = "1"; //CT_NUM
                                    list_of_products[counter, 4] = reference_ME_doc; //DO_Piece
                                    list_of_products[counter, 5] = curr_date; //DO_Date
                                    list_of_products[counter, 6] = curr_date; //DL_DateBC
                                    list_of_products[counter, 7] = (positive_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_products[counter, 8] = curr_date_seconds; // DO_Ref
                                    list_of_products[counter, 9] = line.reference; // AR_Ref
                                    list_of_products[counter, 10] = "1"; //DL_Valorise
                                    list_of_products[counter, 11] = "1"; //DE_NO
                                    list_of_products[counter, 12] = name_article; // DL_Design
                                    list_of_products[counter, 13] = (Convert.ToInt16(current_stock) - Convert.ToInt16(line.stock)).ToString().Replace(",", ".").Replace("-", ""); //line.stock; // DL_Qte
                                    list_of_products[counter, 14] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PoidsNet)).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_products[counter, 14].Equals("0")) { list_of_products[counter, 14] = "0.000000"; } else if (!list_of_products[counter, 14].Contains(".")) { list_of_products[counter, 14] = list_of_products[counter, 14] + ".000000"; }
                                    list_of_products[counter, 15] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PoidsBrut)).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_products[counter, 15].Equals("0")) { list_of_products[counter, 15] = "0.000000"; } else if (!list_of_products[counter, 15].Contains(".")) { list_of_products[counter, 15] = list_of_products[counter, 15] + ".000000"; }
                                    list_of_products[counter, 16] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_products[counter, 16].Equals("0")) { list_of_products[counter, 16] = "0.000000"; } else if (!list_of_products[counter, 16].Contains(".")) { list_of_products[counter, 16] = list_of_products[counter, 16] + ".000000"; }
                                    list_of_products[counter, 17] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_products[counter, 17].Equals("0")) { list_of_products[counter, 17] = "0.000000"; } else if (!list_of_products[counter, 17].Contains(".")) { list_of_products[counter, 17] = list_of_products[counter, 17] + ".000000"; }
                                    list_of_products[counter, 18] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_products[counter, 19] = DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_products[counter, 20] = (Convert.ToInt16(current_stock) - Convert.ToInt16(line.stock)).ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
                                    if (list_of_products[counter, 20].Equals("0")) { list_of_products[counter, 20] = "0.000000"; } else if (!list_of_products[counter, 20].Contains(".")) { list_of_products[counter, 20] = list_of_products[counter, 20] + ".000000"; }
                                    list_of_products[counter, 21] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_products[counter, 22] = (Convert.ToDouble(line.stock) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_products[counter, 20].Equals("0")) { list_of_products[counter, 20] = "0.000000"; } else if (!list_of_products[counter, 20].Contains(".")) { list_of_products[counter, 20] = list_of_products[counter, 20] + ".000000"; }
                                    if (list_of_products[counter, 21].Equals("0")) { list_of_products[counter, 21] = "0.0"; } else if (!list_of_products[counter, 21].Contains(".")) { list_of_products[counter, 21] = list_of_products[counter, 21] + ".0"; }
                                    if (list_of_products[counter, 22].Equals("0")) { list_of_products[counter, 22] = "0.000000"; } else if (!list_of_products[counter, 22].Contains(".")) { list_of_products[counter, 22] = list_of_products[counter, 22] + ".000000"; }
                                    list_of_products[counter, 23] = ""; //PF_Num
                                    list_of_products[counter, 24] = "0"; //DL_No
                                    list_of_products[counter, 25] = "0"; //DL_FactPoids
                                    list_of_products[counter, 26] = "0"; //DL_Escompte

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** Exception ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Le tableau 'ME' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                    logFileWriter.Flush();
                                    logFileWriter.Close();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", "Le tableau 'ME' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Produit '" + name_article + "' est ajouté à la table list_of_products en tant qu'index ME.");
                            }

                            if (current_stock == Convert.ToInt16(line.stock))
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** Information ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : current_stock : " + current_stock+" == Stock Veolog : "+line.stock);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import non effectué");
                                logFileWriter.WriteLine("");
                            }

                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Compteur Produit ===> " + counter);
                            logFileWriter.WriteLine("");
                            counter++; // increment by 1 per product [multi-dimensional array]

                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** Erreur Référence ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : L'article \"" + line.reference + "\" n'existe pas dans la BDD.");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                            logFileWriter.Flush();
                            logFileWriter.Close();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc + " - " + reference_MS_doc, "", "L'import du stock est annulée. L'article \"" + line.reference + "\" n'est pas trouvé dans le champ CodeBare ou dans la base Sage", "L'article \"" + line.reference + "\" n'existe pas dans la BDD.", "", fileName, logFileName_import));
                            return null;
                        }

                    } // end foreach

                    connection.Close(); //disconnect from database
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Connexion fermée.");
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(" ERREUR[4]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    // return list_of_products[0][0];//return false because the query failed to execute

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ********************** Exception 1 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :: " + ex.StackTrace);
                    logFileWriter.Flush();
                    connection.Close(); //disconnect from database
                    recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc + " - " + reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }


                using (OdbcConnection connectionSQL = Connexion.CreateOdbcConnexionSQL()) //connecting to database as handler
                {
                    try
                    {
                        // testing
                        if (positive_item > 0) //check if any product for 20::addstock exists : this case > 0 ; if 1000+ then OK generate document ME_____.
                        {
                            connectionSQL.Open(); //opening the connection
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Connexion ODBC SQL");

                            //generate document ME_____ in database.
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Vérifier si un produit pour 20 = ME");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, curr_date_time));

                            try
                            {
                                OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, curr_date_time), connectionSQL); //calling the query and parsing the parameters into it
                                command.ExecuteReader(); // executing the query

                            }
                            catch (OdbcException ex)
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** OdbcException ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                logFileWriter.Flush();
                                logFileWriter.Close();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                            string[,] products_ME = new string[positive_item / 1000, 27]; // create array with enough space

                            //insert documentline into the database with articles having 20 as value @index 2
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : insert documentline into the database with articles having 20 as value @index 2");

                            for (int x = 0; x < list_of_products.GetLength(0); x++)
                            {
                                if (list_of_products[x, 1] == "20")
                                {
                                    for (int y = 0; y < list_of_products.GetLength(1); y++)
                                    {
                                        products_ME[x, y] = list_of_products[x, y];
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : products_ME[" + x + "," + y + "] = " + products_ME[x, y]);
                                    }

                                    //insert the article to documentline in the database
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : insert the article " + products_ME[x, 15] + " (Ref:" + products_ME[x, 10] + ") to documentline in the database");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : requette sql ===> " + QueryHelper.insertStockDocumentLine(true, products_ME, x));

                                        OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocumentLine(true, products_ME, x), connectionSQL);
                                        command.ExecuteReader();

                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : insert termine!");
                                    }
                                    catch (OdbcException ex)
                                    {
                                        Console.WriteLine(" ERREUR[1]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", ""));

                                        deleteLineAndHeaderOfDocument(true, reference_ME_doc, connectionSQL, logFileWriter);
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** OdbcException ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                        logFileWriter.Flush();
                                        logFileWriter.Close();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }

                        }


                        // Testing
                        if (negative_item > 0) //check if any product for 21::stockremoval exists : this case > 0 ; if 1000+ then OK generate document MS_____.
                        {
                            connectionSQL.Open();

                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Vérifier si un produit pour 21 = MS");
                            logFileWriter.Write(DateTime.Now + " | insertStock() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, curr_date_time));

                            //generate document MS_____. in database.
                            try
                            {
                                OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, curr_date_time), connectionSQL); //calling the query and parsing the parameters into it
                                command.ExecuteReader(); // executing the query
                            }
                            catch (OdbcException ex)
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                logFileWriter.Flush();
                                logFileWriter.Close();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }

                            string[,] products_MS = new string[negative_item / 1000, 27]; // create array with enough space

                            //insert documentline into the database with articles having 20 as value @index 2
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : insert documentline into the database with articles having 20 as value @index 2");

                            for (int x = 0; x < list_of_products.GetLength(0); x++)
                            {
                                if (list_of_products[x, 1] == "21")
                                {
                                    for (int y = 0; y < list_of_products.GetLength(1); y++)
                                    {
                                        products_MS[x, y] = list_of_products[x, y];
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : products_MS[" + x + "," + y + "] = " + products_MS[x, y]);
                                    }

                                    //insert the article to documentline in the database
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : insert the article " + products_MS[x, 15] + " (Ref:" + products_MS[x, 10] + ") to documentline in the database");

                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : requette sql ===> " + QueryHelper.insertStockDocumentLine(true, products_MS, x));

                                        OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocumentLine(true, products_MS, x), connectionSQL);
                                        command.ExecuteReader();
                                    }
                                    catch (OdbcException ex)
                                    {
                                        //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                        Console.WriteLine(" ERREUR[1]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", ""));
                                        deleteLineAndHeaderOfDocument(true, reference_MS_doc, connectionSQL, logFileWriter);
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ********************** OdbcException *********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                        logFileWriter.Flush();
                                        logFileWriter.Close();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return null;
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Exceptions pouvant survenir durant l'exécution de la requête SQL
                        Console.WriteLine(" ERREUR[4]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ********************** Exception 1 *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :: " + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :: " + ex.StackTrace);

                        connectionSQL.Close(); //disconnect from database
                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ConnexionSQL fermée.");
                        logFileWriter.Flush();
                        logFileWriter.Close();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    connectionSQL.Close(); //disconnect from database
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ConnexionSQL fermée.");
                }
            }

            logFileWriter.WriteLine("");
            logFileWriter.Flush();
            logFileWriter.Close();

            return list_of_products;
        }

        public static bool insertStockVeolog(List<Stock> s, string fileName, StreamWriter logFileWriter)
        {
            bool endResults = false;
            string[,] list_of_products_ME = new string[(s.Count - 1), 72];
            string[,] list_of_products_MS = new string[(s.Count - 1), 72];
            int positive_item = 0;
            int negative_item = 0;
            DateTime d = DateTime.Now;
            string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;
            double DO_TotalHT_ME = 0.0;
            double DO_TotalTTC_ME = 0.0;
            double DO_TotalHT_MS = 0.0;
            double DO_TotalTTC_MS = 0.0;
            //double DO_TotalPoid = 0.0;

            string reference_ME_doc = get_next_num_piece_commande_v2("ME", logFileWriter); //lastNumberReference("ME", logFileWriter);   //Doc ME
            if (reference_ME_doc == null)
            {
                logFileWriter.Flush();
                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import du stock est annulée.", "reference_ME_doc est null", "", fileName, logFileName_import));
                return false;
            }

            string reference_MS_doc = get_next_num_piece_commande_v2("MS", logFileWriter); //lastNumberReference("MS", logFileWriter);   //Doc MS
            if (reference_MS_doc == null)
            {
                logFileWriter.Flush();
                recapLinesList_new.Add(new CustomMailRecapLines("Null", "", "L'import du stock est annulée.", "reference_MS_doc est null", "", fileName, logFileName_import));
                return false;
            }

            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connexion.Open();
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Connexion ouverte.");

                    int counter_ME = 0;
                    int counter_MS = 0;
                    int product_count = 1;
                    bool create_ME_doc = false;
                    bool create_MS_doc = false;

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connexion))
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                tvaList = new List<TVA>();
                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                while (reader.Read())
                                {
                                    tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tvaList = null;
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. ");
                            }
                        }
                    }

                    foreach (Stock line in s)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Lire la ligne de l'article.");

                        int total_negative = 0;
                        int total_positive = 0;
                        string name_article = "";
                        string DL_PoidsNet = "0";
                        string DL_PoidsBrut = "0";
                        string DL_PrixUnitaire = "0";
                        string DL_PUTTC = "0";
                        string DL_PrixUnitaire_salePriceHT = "0";
                        string DL_CodeTaxe1 = "";
                        string DL_Taxe1 = "";


                        // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

                        //getProductNameByReference
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Exécuter la requête");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : " + QueryHelper.getProductNameByReference(true, line.reference));
                        logFileWriter.WriteLine("");
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference(true, line.reference), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    name_article = (reader[0].ToString());  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[1].ToString()); // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[2].ToString()); // get unit weight BRUT - check query  
                                    DL_PrixUnitaire = (reader[3].ToString()); // get unit price  - check query
                                    //DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    //COLIS_article = reader[5].ToString();
                                    //PCB_article = reader[6].ToString();
                                    //COMPLEMENT_article = reader[7].ToString();
                                }
                                else // If no rows returned
                                {
                                    //do nothing.
                                }
                            }
                        }

                        if(name_article != "")
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Article trouvé.");
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Exécuter la requête getNegativeStockOfAProduct");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : " + QueryHelper.getNegativeStockOfAProduct(true, line.reference));

                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getNegativeStockOfAProduct(true, line.reference), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    while (reader.Read()) // reads lines/rows from the query
                                    {
                                        if (reader[0].ToString() != null && reader[0].ToString() != "")
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Valeur de la BDD == " + reader[0].ToString());
                                            total_negative += Convert.ToInt32(reader[0].ToString().Split(',')[0]);  // sum up the total_negative variable.
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Valeur de la BDD == Null alors elle devient 0.");
                                            total_negative += Convert.ToInt32(0);  // sum up the total_positive variable.
                                        }
                                    }
                                }
                            }
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : getNegativeStockOfAProduct OK.");
                            logFileWriter.WriteLine("");


                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getPositiveStockOfAProduct(true, line.reference), connexion)) //execute the function within this statement : getPositiveStockOfAProduct()
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Exécuter la requête getPositiveStockOfAProduct");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : " + QueryHelper.getPositiveStockOfAProduct(true, line.reference));

                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    while (reader.Read()) // reads lines/rows from the query
                                    {
                                        if (reader[0].ToString() != null && reader[0].ToString() != "")
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Valeur de la BDD == " + reader[0].ToString());
                                            total_positive += Convert.ToInt32(reader[0].ToString().Split(',')[0]);  // sum up the total_positive variable.
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Valeur de la BDD == Null alors elle devient 0.");
                                            total_positive += Convert.ToInt32(0);  // sum up the total_positive variable.
                                        }
                                    }
                                }
                            }
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : getPositiveStockOfAProduct OK.");
                            logFileWriter.WriteLine("");


                            int current_stock = (total_positive - total_negative); // substract positive stock from the negative one = to obtain the initial current stock.
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Reference: " + line.reference + " total_positive: " + total_positive + " - total_negative: " + total_negative + " = current_stock : " + current_stock + ".");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current stock BDD:" + current_stock + " || current stock Veolog: " + line.stock.Replace(",",".") + " .");

                            //transform line.stock to int format
                            line.stock = line.stock.Split(',')[0];    //line.stock.Replace(",", ".");

                            if (current_stock < Convert.ToInt16(line.stock)) // if current stock in database is inferior from the one received in file : means add stock
                            {
                                //MessageBox.Show("current_stock : " + current_stock + " < " + "Line.stock : " + Convert.ToInt16(line.stock));
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " < " + "current_stock_Veolog : " + Convert.ToInt32(line.stock) + ".");

                                try
                                {
                                    create_ME_doc = true;
                                    positive_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : positive_item: " + positive_item + ".");

                                    int addAmount = Convert.ToInt32(line.stock) - Convert.ToInt32(current_stock);

                                    //calculate product ttc
                                    double product_ttc = 0.0;
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        if (tvaList != null)
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                            TVA tva = null;
                                            foreach (TVA tva_ in tvaList)
                                            {
                                                if (tva_.TA_Code == DL_CodeTaxe1)
                                                {
                                                    tva = tva_;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                    break;
                                                }
                                            }

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * Convert.ToDouble(tva.TA_Taux)) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Warning TVA ********************");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * 0.0) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                        }
                                        logFileWriter.Flush();
                                    }
                                    catch (Exception ex)
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import de la commande est annulée.", "Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return false;
                                    }


                                    // ME00000 : ME prefix will be used to create document
                                    list_of_products_ME[counter_ME, 0] = "2"; // DO_Domaine
                                    list_of_products_ME[counter_ME, 1] = "20"; //DO_Type
                                    list_of_products_ME[counter_ME, 2] = "20"; //DO_DocType
                                    list_of_products_ME[counter_ME, 3] = "1"; //CT_NUM
                                    list_of_products_ME[counter_ME, 4] = reference_ME_doc; //DO_Piece
                                    list_of_products_ME[counter_ME, 5] = curr_date; //DO_Date
                                    list_of_products_ME[counter_ME, 6] = curr_date; //DL_DateBC
                                    list_of_products_ME[counter_ME, 7] = (positive_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_products_ME[counter_ME, 8] = curr_date_seconds; // DO_Ref
                                    list_of_products_ME[counter_ME, 9] = line.reference; // AR_Ref
                                    list_of_products_ME[counter_ME, 10] = "1"; //DL_Valorise
                                    list_of_products_ME[counter_ME, 11] = "1"; //DE_NO
                                    list_of_products_ME[counter_ME, 12] = name_article.Replace("'", "''"); // DL_Design
                                    list_of_products_ME[counter_ME, 13] = (addAmount+"").Replace(",", ".").Replace("-", ""); //line.stock; // DL_Qte
                                    list_of_products_ME[counter_ME, 14] = (Convert.ToDouble(addAmount) * Convert.ToDouble(DL_PoidsNet)).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_products_ME[counter_ME, 14].Equals("0")) { list_of_products_ME[counter_ME, 14] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 14].Contains(".")) { list_of_products_ME[counter_ME, 14] = list_of_products_ME[counter_ME, 14] + ".000000"; }
                                    list_of_products_ME[counter_ME, 15] = (Convert.ToDouble(addAmount) * Convert.ToDouble(DL_PoidsBrut)).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_products_ME[counter_ME, 15].Equals("0")) { list_of_products_ME[counter_ME, 15] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 15].Contains(".")) { list_of_products_ME[counter_ME, 15] = list_of_products_ME[counter_ME, 15] + ".000000"; }
                                    list_of_products_ME[counter_ME, 16] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_products_ME[counter_ME, 16].Equals("0")) { list_of_products_ME[counter_ME, 16] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 16].Contains(".")) { list_of_products_ME[counter_ME, 16] = list_of_products_ME[counter_ME, 16] + ".000000"; }
                                    list_of_products_ME[counter_ME, 17] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_products_ME[counter_ME, 17].Equals("0")) { list_of_products_ME[counter_ME, 17] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 17].Contains(".")) { list_of_products_ME[counter_ME, 17] = list_of_products_ME[counter_ME, 17] + ".000000"; }
                                    list_of_products_ME[counter_ME, 18] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_products_ME[counter_ME, 19] = DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_products_ME[counter_ME, 20] = (addAmount+"").ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
                                    if (list_of_products_ME[counter_ME, 20].Equals("0")) { list_of_products_ME[counter_ME, 20] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 20].Contains(".")) { list_of_products_ME[counter_ME, 20] = list_of_products_ME[counter_ME, 20] + ".000000"; }
                                    list_of_products_ME[counter_ME, 21] = (Convert.ToDouble(addAmount) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_products_ME[counter_ME, 22] = (Convert.ToDouble(addAmount) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_products_ME[counter_ME, 20].Equals("0")) { list_of_products_ME[counter_ME, 20] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 20].Contains(".")) { list_of_products_ME[counter_ME, 20] = list_of_products_ME[counter_ME, 20] + ".000000"; }
                                    if (list_of_products_ME[counter_ME, 21].Equals("0")) { list_of_products_ME[counter_ME, 21] = "0.0"; } else if (!list_of_products_ME[counter_ME, 21].Contains(".")) { list_of_products_ME[counter_ME, 21] = list_of_products_ME[counter_ME, 21] + ".0"; }
                                    if (list_of_products_ME[counter_ME, 22].Equals("0")) { list_of_products_ME[counter_ME, 22] = "0.000000"; } else if (!list_of_products_ME[counter_ME, 22].Contains(".")) { list_of_products_ME[counter_ME, 22] = list_of_products_ME[counter_ME, 22] + ".000000"; }
                                    list_of_products_ME[counter_ME, 23] = ""; //PF_Num
                                    list_of_products_ME[counter_ME, 24] = "0"; //DL_No
                                    list_of_products_ME[counter_ME, 25] = "0"; //DL_FactPoids
                                    list_of_products_ME[counter_ME, 26] = "0"; //DL_Escompte

                                    list_of_products_ME[counter_ME, 27] = DL_PUTTC; //DL_PUTTC
                                    list_of_products_ME[counter_ME, 28] = "0";   //DL_TTC
                                    list_of_products_ME[counter_ME, 29] = "";   //DL_PieceBC
                                    list_of_products_ME[counter_ME, 30] = "";   //DL_PieceBL
                                    list_of_products_ME[counter_ME, 31] = curr_date;   // DL_DateBL
                                    list_of_products_ME[counter_ME, 32] = "0";   //DL_TNomencl
                                    list_of_products_ME[counter_ME, 33] = "0";   //DL_TRemPied
                                    list_of_products_ME[counter_ME, 34] = "0";   //DL_TRemExep
                                    list_of_products_ME[counter_ME, 35] = "" + addAmount;    //DL_QteBC
                                    list_of_products_ME[counter_ME, 36] = "" + addAmount;    //DL_QteBL
                                    list_of_products_ME[counter_ME, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                    list_of_products_ME[counter_ME, 38] = "0";           //DL_Remise01REM_Type
                                    list_of_products_ME[counter_ME, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                    list_of_products_ME[counter_ME, 40] = "0";           //DL_Remise02REM_Type
                                    list_of_products_ME[counter_ME, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                    list_of_products_ME[counter_ME, 42] = "0";           //DL_Remise03REM_Type
                                    list_of_products_ME[counter_ME, 43] = "1";                   //DL_NoRef
                                    list_of_products_ME[counter_ME, 44] = "0";                   //DL_TypePL
                                    list_of_products_ME[counter_ME, 45] = "0.000000";            //DL_PUDevise
                                    list_of_products_ME[counter_ME, 46] = "";                    //CA_Num
                                    list_of_products_ME[counter_ME, 47] = "0.000000";            //DL_Frais
                                    list_of_products_ME[counter_ME, 48] = "";                    //AC_RefClient
                                    list_of_products_ME[counter_ME, 49] = "0";                   //DL_PiecePL
                                    list_of_products_ME[counter_ME, 50] = curr_date;                    //DL_DatePL
                                    list_of_products_ME[counter_ME, 51] = "" + addAmount;                   //DL_QtePL
                                    list_of_products_ME[counter_ME, 52] = "";                    //DL_NoColis
                                    list_of_products_ME[counter_ME, 53] = "0";                   //DL_NoLink
                                    list_of_products_ME[counter_ME, 54] = "0";                    //CO_No
                                    list_of_products_ME[counter_ME, 55] = "0";                //DT_No
                                    list_of_products_ME[counter_ME, 56] = "";                    //DL_PieceDE
                                    list_of_products_ME[counter_ME, 57] = curr_date;                   //DL_DateDe
                                    list_of_products_ME[counter_ME, 58] = "" + addAmount;                    //DL_QteDE
                                    list_of_products_ME[counter_ME, 59] = "0";                  //DL_NoSousTotal
                                    list_of_products_ME[counter_ME, 60] = "0";                //CA_No
                                    list_of_products_ME[counter_ME, 61] = "0.000000";            // DL_PUBC
                                    list_of_products_ME[counter_ME, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                    list_of_products_ME[counter_ME, 63] = DL_Taxe1.ToString().Replace(",", ".");           // DL_Taxe1
                                    list_of_products_ME[counter_ME, 64] = "0.000000";            // DL_Taxe2
                                    list_of_products_ME[counter_ME, 65] = "0.000000";            // DL_Taxe3
                                    list_of_products_ME[counter_ME, 66] = "0";                   // DL_TypeTaux1
                                    list_of_products_ME[counter_ME, 67] = "0";                   // DL_TypeTaxe1
                                    list_of_products_ME[counter_ME, 68] = "0";                   // DL_TypeTaux2
                                    list_of_products_ME[counter_ME, 69] = "0";                   // DL_TypeTaxe2
                                    list_of_products_ME[counter_ME, 70] = "0";                   // DL_TypeTaux3
                                    list_of_products_ME[counter_ME, 71] = "0";                   // DL_TypeTaxe3

                                    /*
                                    list_of_products_ME[counter_ME, 72] = "3";                                           // DL_MvtStock
                                    list_of_products_ME[counter_ME, 73] = "";                                            // AF_RefFourniss
                                    list_of_products_ME[counter_ME, 74] = COLIS_article.ToString().Replace(",", ".");    // COLIS
                                    list_of_products_ME[counter_ME, 75] = PCB_article.ToString().Replace(",", ".");      // PCB
                                    list_of_products_ME[counter_ME, 76] = COMPLEMENT_article;                            // COMPLEMENT
                                    list_of_products_ME[counter_ME, 77] = "";                                            // PourVeolog
                                    list_of_products_ME[counter_ME, 78] = "";                                            // DL_PieceOFProd
                                    list_of_products_ME[counter_ME, 79] = "";                                            // DL_Operation
                                    */

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Exception Tableau ****************************************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Le tableau 'ME' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du stock est annulée.", "Le tableau 'ME' à 2 dimensions ne fonctionne pas correctement, message :\n" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return false;
                                }

                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Produit '" + name_article + "' est ajouté à la table list_of_products_ME en tant qu'index ME.");
                                counter_ME++;
                            }

                            if (current_stock > Convert.ToInt16(line.stock)) // if current stock in database is superior from the one received in file : means remove stock
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " > " + "current_stock_Veolog : " + line.stock + ".");

                                try
                                {
                                    create_MS_doc = true;
                                    negative_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : negativ_item: " + negative_item + ".");

                                    int removeAmount = Convert.ToInt32(current_stock) - Convert.ToInt32(line.stock);

                                    //calculate product ttc
                                    double product_ttc = 0.0;
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        if (tvaList != null)
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                            TVA tva = null;
                                            foreach (TVA tva_ in tvaList)
                                            {
                                                if (tva_.TA_Code == DL_CodeTaxe1)
                                                {
                                                    tva = tva_;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                    break;
                                                }
                                            }

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * Convert.ToDouble(tva.TA_Taux)) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Warning TVA ********************");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * 0.0) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                        }
                                        logFileWriter.Flush();
                                    }
                                    catch (Exception ex)
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", "Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return false;
                                    }

                                    // MS00000 : MS prefix will be used to create document
                                    list_of_products_MS[counter_MS, 0] = "2"; // DO_Domaine
                                    list_of_products_MS[counter_MS, 1] = "21"; //DO_Type
                                    list_of_products_MS[counter_MS, 2] = "21"; //DO_DocType
                                    list_of_products_MS[counter_MS, 3] = "1"; //CT_NUM
                                    list_of_products_MS[counter_MS, 4] = reference_MS_doc; //DO_Piece
                                    list_of_products_MS[counter_MS, 5] = curr_date; //DO_Date
                                    list_of_products_MS[counter_MS, 6] = curr_date; //DL_DateBC
                                    list_of_products_MS[counter_MS, 7] = (negative_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_products_MS[counter_MS, 8] = curr_date_seconds; // DO_Ref
                                    list_of_products_MS[counter_MS, 9] = line.reference; // AR_Ref
                                    list_of_products_MS[counter_MS, 10] = "1"; //DL_Valorise
                                    list_of_products_MS[counter_MS, 11] = "1"; //DE_NO
                                    list_of_products_MS[counter_MS, 12] = name_article.Replace("'", "''"); // DL_Design
                                    list_of_products_MS[counter_MS, 13] = (removeAmount).ToString().Replace(",", ".").Replace("-", "");  //line.stock; // DL_Qte
                                    list_of_products_MS[counter_MS, 14] = (Convert.ToDouble(removeAmount) * Convert.ToDouble(DL_PoidsNet)).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_products_MS[counter_MS, 14].Equals("0")) { list_of_products_MS[counter_MS, 14] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 14].Contains(".")) { list_of_products_MS[counter_MS, 14] = list_of_products_MS[counter_MS, 14] + ".000000"; }
                                    list_of_products_MS[counter_MS, 15] = (Convert.ToDouble(removeAmount) * Convert.ToDouble(DL_PoidsBrut)).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_products_MS[counter_MS, 15].Equals("0")) { list_of_products_MS[counter_MS, 15] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 15].Contains(".")) { list_of_products_MS[counter_MS, 15] = list_of_products_MS[counter_MS, 15] + ".000000"; }
                                    list_of_products_MS[counter_MS, 16] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_products_MS[counter_MS, 16].Equals("0")) { list_of_products_MS[counter_MS, 16] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 16].Contains(".")) { list_of_products_MS[counter_MS, 16] = list_of_products_MS[counter_MS, 16] + ".000000"; }
                                    list_of_products_MS[counter_MS, 17] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_products_MS[counter_MS, 17].Equals("0")) { list_of_products_MS[counter_MS, 17] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 17].Contains(".")) { list_of_products_MS[counter_MS, 17] = list_of_products_MS[counter_MS, 17] + ".000000"; }
                                    list_of_products_MS[counter_MS, 18] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_products_MS[counter_MS, 19] = DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_products_MS[counter_MS, 20] = (removeAmount).ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
                                    if (list_of_products_MS[counter_MS, 20].Equals("0")) { list_of_products_MS[counter_MS, 20] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 20].Contains(".")) { list_of_products_MS[counter_MS, 20] = list_of_products_MS[counter_MS, 20] + ".000000"; }
                                    list_of_products_MS[counter_MS, 21] = (Convert.ToDouble(removeAmount) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_products_MS[counter_MS, 22] = (Convert.ToDouble(removeAmount) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_products_MS[counter_MS, 20].Equals("0")) { list_of_products_MS[counter_MS, 20] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 20].Contains(".")) { list_of_products_MS[counter_MS, 20] = list_of_products_MS[counter_MS, 20] + ".000000"; }
                                    if (list_of_products_MS[counter_MS, 21].Equals("0")) { list_of_products_MS[counter_MS, 21] = "0.0"; } else if (!list_of_products_MS[counter_MS, 21].Contains(".")) { list_of_products_MS[counter_MS, 21] = list_of_products_MS[counter_MS, 21] + ".0"; }
                                    if (list_of_products_MS[counter_MS, 22].Equals("0")) { list_of_products_MS[counter_MS, 22] = "0.000000"; } else if (!list_of_products_MS[counter_MS, 22].Contains(".")) { list_of_products_MS[counter_MS, 22] = list_of_products_MS[counter_MS, 22] + ".000000"; }
                                    list_of_products_MS[counter_MS, 23] = ""; //PF_Num
                                    list_of_products_MS[counter_MS, 24] = "0"; //DL_No
                                    list_of_products_MS[counter_MS, 25] = "0"; //DL_FactPoids
                                    list_of_products_MS[counter_MS, 26] = "0"; //DL_Escompte

                                    list_of_products_MS[counter_MS, 27] = DL_PUTTC; //DL_PUTTC
                                    list_of_products_MS[counter_MS, 28] = "0";   //DL_TTC
                                    list_of_products_MS[counter_MS, 29] = "";   //DL_PieceBC
                                    list_of_products_MS[counter_MS, 30] = "";   //DL_PieceBL
                                    list_of_products_MS[counter_MS, 31] = curr_date;   // DL_DateBL
                                    list_of_products_MS[counter_MS, 32] = "0";   //DL_TNomencl
                                    list_of_products_MS[counter_MS, 33] = "0";   //DL_TRemPied
                                    list_of_products_MS[counter_MS, 34] = "0";   //DL_TRemExep
                                    list_of_products_MS[counter_MS, 35] = "" + removeAmount;    //DL_QteBC
                                    list_of_products_MS[counter_MS, 36] = "" + removeAmount;    //DL_QteBL
                                    list_of_products_MS[counter_MS, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                    list_of_products_MS[counter_MS, 38] = "0";           //DL_Remise01REM_Type
                                    list_of_products_MS[counter_MS, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                    list_of_products_MS[counter_MS, 40] = "0";           //DL_Remise02REM_Type
                                    list_of_products_MS[counter_MS, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                    list_of_products_MS[counter_MS, 42] = "0";           //DL_Remise03REM_Type
                                    list_of_products_MS[counter_MS, 43] = "1";                   //DL_NoRef
                                    list_of_products_MS[counter_MS, 44] = "0";                   //DL_TypePL
                                    list_of_products_MS[counter_MS, 45] = "0.000000";            //DL_PUDevise
                                    list_of_products_MS[counter_MS, 46] = "";                    //CA_Num
                                    list_of_products_MS[counter_MS, 47] = "0.000000";            //DL_Frais
                                    list_of_products_MS[counter_MS, 48] = "";                    //AC_RefClient
                                    list_of_products_MS[counter_MS, 49] = "0";                   //DL_PiecePL
                                    list_of_products_MS[counter_MS, 50] = curr_date;                    //DL_DatePL
                                    list_of_products_MS[counter_MS, 51] = "" + removeAmount;                   //DL_QtePL
                                    list_of_products_MS[counter_MS, 52] = "";                    //DL_NoColis
                                    list_of_products_MS[counter_MS, 53] = "0";                   //DL_NoLink
                                    list_of_products_MS[counter_MS, 54] = "0";                    //CO_No
                                    list_of_products_MS[counter_MS, 55] = "0";                //DT_No
                                    list_of_products_MS[counter_MS, 56] = "";                    //DL_PieceDE
                                    list_of_products_MS[counter_MS, 57] = curr_date;                   //DL_DateDe
                                    list_of_products_MS[counter_MS, 58] = "" + removeAmount;                    //DL_QteDE
                                    list_of_products_MS[counter_MS, 59] = "0";                  //DL_NoSousTotal
                                    list_of_products_MS[counter_MS, 60] = "0";                //CA_No
                                    list_of_products_MS[counter_MS, 61] = "0.000000";            // DL_PUBC
                                    list_of_products_ME[counter_ME, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                    list_of_products_ME[counter_ME, 63] = DL_Taxe1.ToString().Replace(",", ".");           // DL_Taxe1
                                    list_of_products_MS[counter_MS, 64] = "0.000000";            // DL_Taxe2
                                    list_of_products_MS[counter_MS, 65] = "0.000000";            // DL_Taxe3
                                    list_of_products_MS[counter_MS, 66] = "0";                   // DL_TypeTaux1
                                    list_of_products_MS[counter_MS, 67] = "0";                   // DL_TypeTaxe1
                                    list_of_products_MS[counter_MS, 68] = "0";                   // DL_TypeTaux2
                                    list_of_products_MS[counter_MS, 69] = "0";                   // DL_TypeTaxe2
                                    list_of_products_MS[counter_MS, 70] = "0";                   // DL_TypeTaux3
                                    list_of_products_MS[counter_MS, 71] = "0";                   // DL_TypeTaxe3

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Exception Tableau ****************************************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Le tableau 'MS' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", "Le tableau 'MS' à 2 dimensions ne fonctionne pas correctement, message :\n" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return false;
                                }

                                //MessageBox.Show("Product added into the table list_of_products as MS index ");

                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Produit '" + name_article + "' est ajouté à la table list_of_products_MS en tant qu'index MS.");
                                counter_MS++;
                            }

                            if (current_stock == Convert.ToInt16(line.stock))
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Information ****************************************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " == current_stock_Veolog : " + line.stock);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import non effectué");
                                logFileWriter.WriteLine("");
                                logFileWriter.Flush();
                            }

                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Compteur Produit ===> " + product_count + "");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Ligne du produit \""+name_article+"\" (" + line.reference + ") est terminé!!!");
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Warning Référence ****************************************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : La Référence (Ref: " + line.reference + ") du produit dans le fichier n'existe pas dans la BDD.");
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine("");
                            logFileWriter.Flush();
                        }
                        product_count++;
                    }   // end foreach


                    // add veolog import date
                    string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Ajouter la date d'import \"" + delivery_date_veolog + "\" de Veolog au Stock \"" + reference_ME_doc + "\" et \"" + reference_MS_doc + "\".");

                    if (create_ME_doc) //Generate document ME (Doc_Entete && DocLigne) if i need to.
                    {
                        //generate document ME_____ in database.
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Générer un document ME");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertVeologStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, (DO_TotalHT_ME+"").Replace(",", "."), (DO_TotalTTC_ME+"").Replace(",", "."), delivery_date_veolog));

                        try
                        {
                            OdbcCommand command = new OdbcCommand(QueryHelper.insertVeologStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, (DO_TotalHT_ME+"").Replace(",", "."), (DO_TotalTTC_ME+"").Replace(",", "."), delivery_date_veolog), connexion); //calling the query and parsing the parameters into it
                            command.ExecuteReader(); // executing the query

                        }
                        catch (OdbcException ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** OdbcException ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Message :" + ex.Message);
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du document est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return false;
                        }
                        string[,] products_ME = new string[(s.Count - 1), 72]; // create array with enough space

                        //insert documentline into the database with articles having 20 as value @index 2
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert documentline into the database with articles having 20 as value @index 2");

                        for (int x = 0; x < list_of_products_ME.GetLength(0); x++)
                        {
                            //logFileWriter.WriteLine("x: " + x + " || list_of_products.GetLength(0) : " + list_of_products.GetLength(0));
                            if (list_of_products_ME[x, 1] == "20")
                            {
                                for (int y = 0; y < list_of_products_ME.GetLength(1); y++)
                                {
                                    //logFileWriter.WriteLine("x: " + x + " ; y: " + y + " || list_of_products.GetLength(1) : " + list_of_products.GetLength(1));

                                    products_ME[x, y] = list_of_products_ME[x, y];
                                    //logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : products_ME[" + x + "," + y + "] = " + products_ME[x, y]);
                                }

                                //insert the article to documentline in the database
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert the article " + products_ME[x, 12] + " (Ref:" + products_ME[x, 9] + ") to documentline in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : requette sql ===> " + QueryHelper.insertVeologStockDocumentLine(true, products_ME, x));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertVeologStockDocumentLine(true, products_ME, x), connexion);
                                    command.ExecuteReader();

                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert termine!");
                                }
                                catch (OdbcException ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** OdbcException ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return false;
                                }
                            }
                        }

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Information ****************************************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Document ME insert avec succès");
                        logFileWriter.WriteLine("");

                        //update document numbering
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Mettre à jour la numérotation du document \"" + reference_ME_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "ME", reference_ME_doc));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "ME", reference_ME_doc), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return false;
                        }
                    }

                    if (create_MS_doc) //Generate document MS (Doc_Entete && DocLigne) if i need to.
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Générer un document MS");
                        logFileWriter.Write(DateTime.Now + " | insertStockVeolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertVeologStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, (DO_TotalHT_MS + "").Replace(",", "."), (DO_TotalTTC_MS + "").Replace(",", "."), delivery_date_veolog));

                        //generate document MS_____. in database.
                        try
                        {
                            OdbcCommand command = new OdbcCommand(QueryHelper.insertVeologStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, (DO_TotalHT_MS + "").Replace(",", "."), (DO_TotalTTC_MS + "").Replace(",", "."), delivery_date_veolog), connexion); //calling the query and parsing the parameters into it
                            command.ExecuteReader(); // executing the query
                        }
                        catch (OdbcException ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ********************** OdbcException *********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Message :" + ex.Message);
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return false;
                        }

                        string[,] products_MS = new string[(s.Count - 1), 72]; // create array with enough space

                        //insert documentline into the database with articles having 20 as value @index 2
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert documentline into the database with articles having 21 as value @index 2");

                        for (int x = 0; x < list_of_products_MS.GetLength(0); x++)
                        {
                            if (list_of_products_MS[x, 1] == "21")
                            {
                                for (int y = 0; y < list_of_products_MS.GetLength(1); y++)
                                {
                                    products_MS[x, y] = list_of_products_MS[x, y];
                                    //logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : products_MS[" + x + "," + y + "] = " + products_MS[x, y]);
                                }

                                //insert the article to documentline in the database
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert the article " + products_MS[x, 12] + " (Ref:" + products_MS[x, 9] + ") to documentline in the database");

                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : requette sql ===> " + QueryHelper.insertVeologStockDocumentLine(true, products_MS, x));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertVeologStockDocumentLine(true, products_MS, x), connexion);
                                    command.ExecuteReader();
                                }
                                catch (OdbcException ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ********************** OdbcException *********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | inseinsertStockVeologrtStock() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return false;
                                }
                            }
                        }

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : **************************************** Information ****************************************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Document MS insert avec succès");
                        logFileWriter.WriteLine("");

                        //update document numbering
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Mettre à jour la numérotation du document \"" + reference_MS_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "MS", reference_MS_doc));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "MS", reference_MS_doc), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return false;
                        }
                    }


                    connexion.Close(); //disconnect from database
                    endResults = true;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Connexion fermée.");

                }
                catch (Exception ex)
                {
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ********************** Exception 2 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :: " + ex.StackTrace);
                    logFileWriter.Flush();
                    connexion.Close();  //disconnect from database
                    recapLinesList_new.Add(new CustomMailRecapLines(reference_ME_doc + " | " + reference_MS_doc, "", "L'import du stock est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return false;
                }
            }

            logFileWriter.WriteLine("");

            return endResults;
        }

        private static string[,] insertDesadv_Veolog(string reference_DESADV_doc, Veolog_DESADV dh, List<Veolog_DESADV_Lines> dl, string fileName, StreamWriter logFileWriter)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_client_info = null;

            int position_item = 0;
            DateTime d = DateTime.Now;
            //string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            //string curr_time = "000" + d.ToString("hhmmss");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;


            // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Connexion ouverte.");

                    //Get ref client CMD, nature_OP_P && total ht
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string CO_No = "";

                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupérer des informations de la commande la référence " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getRefCMDClient(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Client_Livre = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                CO_No = reader[7].ToString();
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune commande trouvé!. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. La commande " +dh.Ref_Commande_Donneur_Ordre+" n'exist pas dans la base Sage", "La commande " + dh.Ref_Commande_Donneur_Ordre + " n'exist pas dans la BDD", "", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connection)) 
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                tvaList = new List<TVA>();
                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                while (reader.Read())
                                {
                                    tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tvaList = null;
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. ");
                            }
                        }
                    }

                    //get veolog delivery date and time
                    string veologDeliveryDateTime = "";
                    try
                    {
                        string year = dh.Date_De_Expedition.Substring(0, 4);
                        string month = dh.Date_De_Expedition.Substring(4, 2);
                        string day = dh.Date_De_Expedition.Substring(6, 2);

                        string hour = "00";
                        string mins = "00";
                        if (dh.Heure_De_Expedition != "" && dh.Heure_De_Expedition.Length == 4 )
                        {
                            hour = dh.Heure_De_Expedition.Substring(0, 2);
                            mins = dh.Heure_De_Expedition.Substring(2, 2);
                        }
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;
                    }
                    catch (Exception e)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur Date/Heure de livraison ********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message: "+e.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace: "+e.StackTrace);
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", e.Message, e.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    int counter = 0;

                    foreach (Veolog_DESADV_Lines line in dl) //read item by item
                    {
                        string ref_client = "";
                        string ref_article = "";
                        string name_article = "";
                        string DL_PoidsNet = "0";
                        string DL_PoidsBrut = "0";
                        //string DL_PrixUnitaire_buyPrice = "0";
                        string DL_PrixUnitaire_salePriceHT = "0";
                        string DL_PUTTC = "0";
                        string COLIS_article = "";
                        string PCB_article = "";
                        string COMPLEMENT_article = "";
                        string DL_Taxe1 = "";
                        string DL_CodeTaxe1 = "";
                        string DL_PieceBC = "";
                        string DL_DateBC = "";
                        string DL_QteBC = "";

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Lire la ligne de l'article.");

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_DESADV(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());                   // get product ref
                                    name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query  
                                    //DL_PrixUnitaire_buyPrice = (reader[4].ToString());      // get (Prix d'achat) unit price - check query 
                                    DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                    COLIS_article = reader[5].ToString();
                                    PCB_article = reader[6].ToString();
                                    COMPLEMENT_article = reader[7].ToString();
                                    DL_Taxe1 = reader[8].ToString();
                                    DL_CodeTaxe1 = reader[9].ToString();
                                    DL_PieceBC = reader[10].ToString();
                                    DL_DateBC = reader[11].ToString();
                                    DL_QteBC = reader[12].ToString().Replace(",", ".");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la BDD.");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'article \"" + line.Code_Article + "\" n'est pas trouvé dans le champ CodeBare ou dans la base Sage", "L'article \"" + line.Code_Article + "\" n'est pas trouvé dans la BDD.", "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference From CMD Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir la référence client depuis le BC.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Ref_Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_client = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Une reponse. Ref Client ===> "+ ref_client);
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "Le client n'est pas trouvé dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //get Client Reference by Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Obtenir la référence client par référence.");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_client));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_client_info = new string[15];
                                    list_of_client_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_client_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_client_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_client_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_client_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_client_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_client_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_client_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_client_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_client_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_client_info[10] = reader[10].ToString();    // CO_No
                                    list_of_client_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_client_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_client_info[13] = reader[13].ToString();    //  CT_Intitule
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                }
                            }
                        }

                        //get client delivery adress
                        if (list_of_client_info != null)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client));
                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    if (reader.Read()) // If any rows returned
                                    {
                                        list_of_client_info[14] = reader[0].ToString();    // LI_No
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Adresse de livraison ("+reader[0].ToString()+") trouvé!");
                                    }
                                    else// If no rows returned
                                    {
                                        //do nothing.
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "L'adresse de livraison du client " + ref_client + " n'est pas trouvé dans Sage", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Erreur ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucun client trouver.");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée. Le client " + ref_client + " n'existe pas dans Sage", "Le client " + ref_client + " n'existe pas dans la BDD", "Aucune reponse. list_of_client_info est null", fileName, logFileName_import));
                            return null;
                        }


                        if (ref_article != "" && name_article != "" && list_of_client_info != null)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Article trouvé.");
                            logFileWriter.WriteLine("");

                            try
                            {
                                //DL_Ligne
                                position_item += 1000;

                                //calculate product ttc
                                double product_ttc = 0.0;
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    if (tvaList != null)
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                        TVA tva = null;
                                        bool tva_error = false;
                                        foreach (TVA tva_ in tvaList)
                                        {
                                            if (DL_CodeTaxe1 != null && DL_CodeTaxe1 != "" && tva_.TA_Code == DL_CodeTaxe1)
                                            {
                                                tva = tva_;
                                                tva_error = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA "+tva.TA_Code+" trouvé \"" + tva.TA_Taux + "\"");
                                                break;
                                            }
                                            else
                                            {
                                                if(DL_CodeTaxe1 == null)
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA NULL trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                                else if (DL_CodeTaxe1 == "")
                                                {
                                                    //tva = tva_;
                                                    tva_error = false;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA VIDE trouvé, alors TVA mis à 0");
                                                    break;
                                                }
                                            }
                                        }

                                        string endTVA = null;
                                        if (tva_error)
                                        {
                                            endTVA = tva.TA_Taux;
                                        }
                                        else
                                        {
                                            DL_CodeTaxe1 = "C00";
                                            endTVA = "0,000000";
                                        }
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : endTVA = "+ endTVA);

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * Convert.ToDouble(endTVA)) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    else
                                    {
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Warning TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                        double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                        double product_20_P = (product_ht * 0.0) / 100;
                                        product_ttc = product_ht + product_20_P;
                                        DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Prix TTC créé");
                                    }
                                    logFileWriter.Flush();
                                }
                                catch(Exception ex)
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception TVA ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Erreur lors du calcule du prix d'article TTC, message : " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                string original_date = DL_DateBC;
                                string date = original_date.Split(' ')[0];
                                string time = original_date.Split(' ')[1];
                                DL_DateBC = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                // ouput DL_DateBC: 2020-01-03 00:00:00;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "0"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "3"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "3"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_client_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_DESADV_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = DL_DateBC; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Client_Livre; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
                                list_of_cmd_lines[counter, 14] = Convert.ToDouble(DL_PoidsNet).ToString().Replace(",", "."); // DL_PoidsNet
                                if (list_of_cmd_lines[counter, 14].Equals("0")) { list_of_cmd_lines[counter, 14] = "0.000000"; } else if (!list_of_cmd_lines[counter, 14].Contains(".")) { list_of_cmd_lines[counter, 14] = list_of_cmd_lines[counter, 14] + ".000000"; }

                                list_of_cmd_lines[counter, 15] = Convert.ToDouble(DL_PoidsBrut).ToString().Replace(",", "."); // DL_PoidsBrut
                                if (list_of_cmd_lines[counter, 15].Equals("0")) { list_of_cmd_lines[counter, 15] = "0.000000"; } else if (!list_of_cmd_lines[counter, 15].Contains(".")) { list_of_cmd_lines[counter, 15] = list_of_cmd_lines[counter, 15] + ".000000"; }

                                list_of_cmd_lines[counter, 16] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixUnitaire
                                if (list_of_cmd_lines[counter, 16].Equals("0")) { list_of_cmd_lines[counter, 16] = "0.000000"; } else if (!list_of_cmd_lines[counter, 16].Contains(".")) { list_of_cmd_lines[counter, 16] = list_of_cmd_lines[counter, 16] + ".000000"; }

                                list_of_cmd_lines[counter, 17] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixRU
                                if (list_of_cmd_lines[counter, 17].Equals("0")) { list_of_cmd_lines[counter, 17] = "0.000000"; } else if (!list_of_cmd_lines[counter, 17].Contains(".")) { list_of_cmd_lines[counter, 17] = list_of_cmd_lines[counter, 17] + ".000000"; }

                                list_of_cmd_lines[counter, 18] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_CMUP
                                list_of_cmd_lines[counter, 19] = "Heure";    //DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite_Colis) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                list_of_cmd_lines[counter, 29] = DL_PieceBC;   //DL_PieceBC
                                list_of_cmd_lines[counter, 30] = reference_DESADV_doc;   //DL_PieceBL
                                list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                list_of_cmd_lines[counter, 35] = DL_QteBC.Replace(",", ".");   //DL_QteBC
                                list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");   //DL_QteBL
                                list_of_cmd_lines[counter, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                list_of_cmd_lines[counter, 38] = "0";           //DL_Remise01REM_Type
                                list_of_cmd_lines[counter, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                list_of_cmd_lines[counter, 40] = "0";           //DL_Remise02REM_Type
                                list_of_cmd_lines[counter, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                list_of_cmd_lines[counter, 42] = "0";           //DL_Remise03REM_Type
                                list_of_cmd_lines[counter, 43] = "1";                   //DL_NoRef
                                list_of_cmd_lines[counter, 44] = "0";                   //DL_TypePL
                                list_of_cmd_lines[counter, 45] = "0.000000";            //DL_PUDevise
                                list_of_cmd_lines[counter, 46] = "";                    //CA_Num
                                list_of_cmd_lines[counter, 47] = "0.000000";            //DL_Frais
                                list_of_cmd_lines[counter, 48] = "";                    //AC_RefClient
                                list_of_cmd_lines[counter, 49] = "";                   //DL_PiecePL
                                list_of_cmd_lines[counter, 50] = "NULL"; //curr_date;                    //DL_DatePL
                                list_of_cmd_lines[counter, 51] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                   //DL_QtePL
                                list_of_cmd_lines[counter, 52] = "";                    //DL_NoColis
                                list_of_cmd_lines[counter, 53] = "0";                   //DL_NoLink
                                list_of_cmd_lines[counter, 54] = CO_No;                    //CO_No
                                list_of_cmd_lines[counter, 55] = "0";                //DT_No
                                list_of_cmd_lines[counter, 56] = "";                    //DL_PieceDE
                                list_of_cmd_lines[counter, 57] = "NULL";                   //DL_DateDe
                                list_of_cmd_lines[counter, 58] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                    //DL_QteDE
                                list_of_cmd_lines[counter, 59] = "0";                  //DL_NoSousTotal
                                list_of_cmd_lines[counter, 60] = "0";                //CA_No
                                list_of_cmd_lines[counter, 61] = "0.000000";            // DL_PUBC
                                list_of_cmd_lines[counter, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                list_of_cmd_lines[counter, 63] = DL_Taxe1.ToString().Replace(",", ".");         // DL_Taxe1
                                list_of_cmd_lines[counter, 64] = "0.000000";            // DL_Taxe2
                                list_of_cmd_lines[counter, 65] = "0.000000";            // DL_Taxe3
                                list_of_cmd_lines[counter, 66] = "0";                   // DL_TypeTaux1
                                list_of_cmd_lines[counter, 67] = "0";                   // DL_TypeTaxe1
                                list_of_cmd_lines[counter, 68] = "0";                   // DL_TypeTaux2
                                list_of_cmd_lines[counter, 69] = "0";                   // DL_TypeTaxe2
                                list_of_cmd_lines[counter, 70] = "0";                   // DL_TypeTaux3
                                list_of_cmd_lines[counter, 71] = "0";                   // DL_TypeTaxe3

                                list_of_cmd_lines[counter, 72] = "3";                                           // DL_MvtStock
                                list_of_cmd_lines[counter, 73] = "";                                            // AF_RefFourniss
                                list_of_cmd_lines[counter, 74] = ((COLIS_article == null || COLIS_article == "") ? "0.0" : COLIS_article).ToString().Replace(",", ".");    // COLIS
                                list_of_cmd_lines[counter, 75] = ((PCB_article == null || PCB_article == "") ? "0.0" : PCB_article).ToString().Replace(",", ".");      // PCB
                                list_of_cmd_lines[counter, 76] = COMPLEMENT_article;                            // COMPLEMENT
                                list_of_cmd_lines[counter, 77] = "";                                            // PourVeolog
                                list_of_cmd_lines[counter, 78] = "";                                            // DL_PieceOFProd
                                list_of_cmd_lines[counter, 79] = "";                                            // DL_Operation
                                list_of_cmd_lines[counter, 80] = veologDeliveryDateTime;    //DO_DateLivr
                                list_of_cmd_lines[counter, 81] = "0";    //DL_NonLivre

                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                            counter++;
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                        }
                        else
                        {
                            counter--;
                            logFileWriter.WriteLine("Aucun article trouvé ou Aucun information client trouvé !");
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Aucun article trouvé ou Aucun information client trouvé !", "", fileName, logFileName_import));
                            return null;
                        }

                        
                    }
                    // ===== End Foreach =====

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Vérifier si un produit pour 0 = BL");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No));

                    //generate document BL_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_client_info, dh.Etat, CO_No), connection); //calling the query and parsing the parameters into it
                        command.ExecuteReader(); // executing the query
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }


                    string[,] products_DESADV = new string[position_item / 1000, 82]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert documentline into the database with articles having 3 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "3")
                        {
                            logFileWriter.WriteLine("");
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_DESADV[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : products_BL_L[" + x + "," + y + "] = " + products_DESADV[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert the article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocumentLine_Veolog(true, products_DESADV, x), connection);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }


                            //Update docline artile stock
                            try
                            {
                                bool found_stock = false;
                                double AS_StockReel = 0.0;
                                double AS_StockReserve = 0.0;
                                double AS_StockMontant = 0.0;
                                double productPrixUnite = Convert.ToDouble(products_DESADV[x, 16].Replace('.', ','));

                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : get current stock in F_ARTSTOCK table in the database");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.getArticleStock(true, products_DESADV[x, 9]));
                                using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_DESADV[x, 9]), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                                {
                                    using (IDataReader reader = command_.ExecuteReader()) // read rows of the executed query
                                    {
                                        if (reader.Read()) // If any rows returned
                                        {
                                            found_stock = true;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + ").");
                                            AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                            AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                            AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                        }
                                        else// If no rows returned
                                        {
                                            //do nothing.
                                            found_stock = false;
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse.");
                                            logFileWriter.WriteLine("");
                                            deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter);
                                            logFileWriter.WriteLine("");
                                            logFileWriter.Flush();
                                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", "Echec à la récupértion du stock de l'article " + products_DESADV[x,9], "", fileName, logFileName_import));
                                            return null;
                                        }
                                    }
                                }


                                if (found_stock)
                                {
                                    //Calculate stock info
                                    //double AS_CMUP = AS_StockMontant / AS_StockReel;
                                    double new_AS_StockReel = AS_StockReel - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_DESADV[x, 13].Replace('.', ','));
                                    double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                    double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReel: "+ new_AS_StockReel + " = AS_StockReel: "+ AS_StockReel + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_DESADV[x, 13].Replace('.', ',')));
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " * productPrixUnite: " + productPrixUnite);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : update article " + products_DESADV[x, 12] + " (Ref:" + products_DESADV[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStock(true, products_DESADV[x, 9], new_AS_StockReel, new_AS_StockReserve, new_AS_StockMontant), connection);
                                    command.ExecuteReader();
                                }
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Flush();

                                logFileWriter.WriteLine("");
                                if(!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                                {
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                                }
                                logFileWriter.WriteLine("");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //set Veolog date time import
                    try
                    {
                        string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au DESADV \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_DESADV_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connection);
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Date de livraison veolog à jour !");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();
                        
                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //Delete the BC of the BL
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Supprimer le Bon de Commande (BC) \"" + dh.Ref_Commande_Donneur_Ordre+"\".");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                        OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Bon de Commande supprimé!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();
                        
                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                    //update document numbering
                    try
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Mettre à jour la numérotation du document \"" + reference_DESADV_doc + "\".");

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc));
                        OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, "BL", reference_DESADV_doc), connection);
                        IDataReader reader = command.ExecuteReader();
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Nouvelle numérotation à jour!");
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                        logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                        logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                        logFileWriter.Flush();

                        logFileWriter.WriteLine("");
                        if (!deleteLineAndHeaderOfDocument(true, reference_DESADV_doc, connection, logFileWriter))
                        {
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "", "Erreur lors de la suppression du document " + reference_DESADV_doc, "", fileName, logFileName_import));
                        }
                        logFileWriter.WriteLine("");
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                        return null;
                    }

                }
                catch (Exception ex)
                {
                    // Exceptions pouvant survenir durant l'exécution de la requête SQL
                    // return list_of_products[0][0];//return false because the query failed to execute

                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** Exception 3 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :: " + ex.StackTrace);
                    connection.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new CustomMailRecapLines(reference_DESADV_doc, "", "L'import du bon de livraison est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }

        private static string[,] insertSupplierOrder(string reference_BLF_doc, Veolog_BCF dh, List<Veolog_BCF_Lines> dl, string fileName, StreamWriter logFileWriter)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 82];    // new string [x,y]
            string[] list_of_supplier_info = null;
            int position_item = 0;
            DateTime d = DateTime.Now;
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connexion.Open();
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Connexion ouverte.");

                    //Get ref client CMD, nature_OP_P && total ht
                    string nature_op_p_ = "";
                    string do_totalHT_ = "";
                    string do_totalHTNet_ = "";
                    string do_totalTTC_ = "";
                    string do_NetAPayer_ = "";
                    string do_MontantRegle_ = "";
                    string ref_supplier = null;

                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Récupérer la référence Commande fournisseur livré de la commande " + dh.Ref_Commande_Donneur_Ordre);
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getCMDSupplierByRef(true, dh.Ref_Commande_Donneur_Ordre), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                dh.Ref_Commande_Fournisseur = reader[0].ToString();
                                nature_op_p_ = reader[1].ToString();
                                do_totalHT_ = reader[2].ToString().Replace(",", ".");
                                do_totalHTNet_ = reader[3].ToString().Replace(",", ".");
                                do_totalTTC_ = reader[4].ToString().Replace(",", ".");
                                do_NetAPayer_ = reader[5].ToString().Replace(",", ".");
                                do_MontantRegle_ = reader[6].ToString().Replace(",", ".");
                                ref_supplier = reader[7].ToString();
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                                logFileWriter.Flush();
                                recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Aucun BCF trouvé dans la BDD", "insertSupplierOrder() : Aucune reponse", fileName, logFileName_import));
                                return null;
                            }
                        }
                    }

                    //get Client Reference by Ref
                    logFileWriter.WriteLine("");
                    if (ref_supplier != null)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info = new string[16];
                                    list_of_supplier_info[0] = reader[0].ToString();      // CT_Num
                                    list_of_supplier_info[1] = reader[1].ToString();      // CA_Num 
                                    list_of_supplier_info[2] = reader[2].ToString();      // CG_NumPrinc
                                    list_of_supplier_info[3] = reader[3].ToString();      // CT_NumPayeur
                                    list_of_supplier_info[4] = reader[4].ToString();      // N_Condition
                                    list_of_supplier_info[5] = reader[5].ToString();      // N_Devise
                                    list_of_supplier_info[6] = reader[6].ToString();      // CT_Langue
                                    list_of_supplier_info[7] = reader[7].ToString();      // DO_NbFacture = CT_Facture
                                    list_of_supplier_info[8] = reader[8].ToString().Replace(',', '.');      // DO_TxEscompte = CT_Taux02
                                    list_of_supplier_info[9] = reader[9].ToString();      // N_CatCompta
                                    list_of_supplier_info[10] = reader[10].ToString();    // CO_No
                                    list_of_supplier_info[11] = reader[11].ToString();    //  DO_Tarif = N_CatTarif
                                    list_of_supplier_info[12] = reader[12].ToString();    //  DO_Expedit = N_Expedition du tier
                                    list_of_supplier_info[13] = reader[13].ToString();    //  CT_Intitule
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. list_of_fournisseur_info est null");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur \"" +ref_supplier+"\" n'existe pas dans Sage", "Le fournisseur \"" + ref_supplier + "\" n'existe pas dans Sage", "insertSupplierOrder() : Aucune reponse", fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientDeliveryAddress_DESADV(true, ref_supplier), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_supplier_info[14] = reader[0].ToString();    // LI_No
                                    list_of_supplier_info[15] = reader[0].ToString();    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Adresse de livraison (" + reader[0].ToString() + ") trouvé!");
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    list_of_supplier_info[14] = "0";    // LI_No
                                    list_of_supplier_info[15] = "NULL";    // cbLI_No
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ( list_of_supplier_info[14] = 0 && list_of_supplier_info[15] = NULL ) est null");
                                }
                            }
                        }
                    }
                    else
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** Exception Supplier *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Le fournisseur : " + ref_supplier + " n'existe pas dans la base!");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : import annulé!");
                        logFileWriter.Flush();
                        connexion.Close(); //disconnect from database
                        logFileWriter.Flush();
                        recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. Le fournisseur : " + ref_supplier + " n'existe pas dans Sage!", "Le fournisseur : " + ref_supplier + " n'existe pas dans la BDD", "insertSupplierOrder() : ref_supplier == null", fileName, logFileName_import));
                        return null;
                    }

                    //Get the list of all Taxes (TVA)
                    //So i can calculate the ttc later
                    List<TVA> tvaList = null;
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Récupére tous les tva");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getAllTVA(true));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getAllTVA(true), connexion))
                    {
                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                        {
                            if (reader.Read()) // If any rows returned
                            {
                                tvaList = new List<TVA>();
                                tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                while (reader.Read())
                                {
                                    tvaList.Add(new TVA(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                                }
                            }
                            else// If no rows returned
                            {
                                //do nothing.
                                tvaList = null;
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                            }
                        }
                    }


                    if (list_of_supplier_info != null)
                    {
                        //get veolog delivery date and time
                        string year = dh.Date_De_Reception.Substring(0, 4);
                        string month = dh.Date_De_Reception.Substring(4, 2);
                        string day = dh.Date_De_Reception.Substring(6, 2);
                        string hour = dh.Heure_De_Reception.Substring(0, 2);
                        string mins = dh.Heure_De_Reception.Substring(2, 2);
                        string veologDeliveryDate = year + "-" + month + "-" + day;
                        string veologDeliveryTime = hour + ":" + mins + ":00";
                        string veologDeliveryDateTime = veologDeliveryDate + " " + veologDeliveryTime;

                        int counter = 0;

                        foreach (Veolog_BCF_Lines line in dl) //read item by item
                        {
                            string ref_article = "";
                            string name_article = "";
                            string DL_PoidsNet = "0";
                            string DL_PoidsBrut = "0";
                            //string DL_PrixUnitaire_buyPrice = "0";
                            string DL_PrixUnitaire_salePriceHT = "0";
                            string DL_PUTTC = "0";
                            string COLIS_article = "";
                            string PCB_article = "";
                            string COMPLEMENT_article = "";
                            string DL_Taxe1 = "";
                            string DL_CodeTaxe1 = "";
                            string DL_PieceBCF = "";
                            string DL_DateBCF = "";
                            string DL_QteBCF = "";

                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Lire la ligne de l'article.");

                            //get Product Name By Reference
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article));
                            using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_BLF(true, dh.Ref_Commande_Donneur_Ordre, line.Code_Article), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                            {
                                using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                                {
                                    if (reader.Read()) // If any rows returned
                                    {
                                        ref_article = (reader[0].ToString());                   // get product ref
                                        name_article = (reader[1].ToString());                  // sum up the total_negative variable. - check query
                                        DL_PoidsNet = (reader[2].ToString());                   // get unit weight NET - check query
                                        DL_PoidsBrut = (reader[3].ToString());                  // get unit weight BRUT - check query
                                        DL_PrixUnitaire_salePriceHT = (reader[4].ToString());   // get (Prix de vente) unit price ht - check query
                                        COLIS_article = reader[5].ToString();
                                        PCB_article = reader[6].ToString();
                                        COMPLEMENT_article = reader[7].ToString();
                                        DL_Taxe1 = reader[8].ToString();
                                        DL_CodeTaxe1 = reader[9].ToString();
                                        DL_PieceBCF = reader[10].ToString();
                                        DL_DateBCF = reader[11].ToString();
                                        DL_QteBCF = reader[12].ToString().Replace(",", ".");
                                    }
                                    else// If no rows returned
                                    {
                                        //do nothing.
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse. ");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée. L'article \"" +line.Code_Article+"\" n'existe pas dans le BCF " + dh.Ref_Commande_Donneur_Ordre, "L'article \"" + line.Code_Article + "\" n'existe pas dans la commande " + dh.Ref_Commande_Donneur_Ordre, "", fileName, logFileName_import));
                                        return null;
                                    }
                                }
                            }


                            if (ref_article != "" && name_article != "" && list_of_supplier_info != null)
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Article trouvé.");
                                logFileWriter.WriteLine("");

                                try
                                {
                                    //DL_Ligne
                                    position_item += 1000;

                                    //calculate product ttc
                                    double product_ttc = 0.0;
                                    try
                                    {
                                        logFileWriter.WriteLine("");
                                        if (tvaList != null)
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : List des TVA trouvé");
                                            TVA tva = null;
                                            foreach (TVA tva_ in tvaList)
                                            {
                                                if (tva_.TA_Code == DL_CodeTaxe1)
                                                {
                                                    tva = tva_;
                                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : TVA trouvé \"" + tva.TA_Taux + "\"");
                                                    break;
                                                }
                                            }

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * Convert.ToDouble(tva.TA_Taux)) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Prix TTC créé");
                                        }
                                        else
                                        {
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Warning TVA ********************");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Liste des tva non trouvée, tous les tva et prix ttc de chaque produit dans ce BL seront 0");

                                            double product_ht = Convert.ToDouble(DL_PrixUnitaire_salePriceHT);
                                            double product_20_P = (product_ht * 0.0) / 100;
                                            product_ttc = product_ht + product_20_P;
                                            DL_PUTTC = ("" + product_ttc).Replace(",", ".");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Prix TTC créé");
                                        }
                                        logFileWriter.Flush();
                                    }
                                    catch (Exception ex)
                                    {
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Exception TVA ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Erreur lors du calcule du prix d'article TTC, message :\n" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                        logFileWriter.Flush();
                                        recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                        return null;
                                    }

                                    // input DL_DateBC: 03 - 01 - 2020 00:00:00;
                                    string original_date = DL_DateBCF;
                                    string date = original_date.Split(' ')[0];
                                    string time = original_date.Split(' ')[1];
                                    DL_DateBCF = date.Split('/')[2] + "-" + date.Split('/')[1] + "-" + date.Split('/')[0] + " " + time;
                                    // ouput DL_DateBC: 2020-01-03 00:00:00;

                                    // DESADV prefix will be used to create document
                                    list_of_cmd_lines[counter, 0] = "1"; // DO_Domaine
                                    list_of_cmd_lines[counter, 1] = "13"; //DO_Type
                                    list_of_cmd_lines[counter, 2] = "13"; //DO_DocType
                                    list_of_cmd_lines[counter, 3] = list_of_supplier_info[0]; //CT_NUM
                                    list_of_cmd_lines[counter, 4] = reference_BLF_doc; //DO_Piece
                                    list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                    list_of_cmd_lines[counter, 6] = DL_DateBCF; //DL_DateBC
                                    list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                    list_of_cmd_lines[counter, 8] = dh.Ref_Commande_Fournisseur; // DO_Ref
                                    list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                    list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                    list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                    list_of_cmd_lines[counter, 12] = name_article.Replace("'", "''"); // DL_Design
                                    list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
                                    list_of_cmd_lines[counter, 14] = Convert.ToDouble(DL_PoidsNet).ToString().Replace(",", "."); // DL_PoidsNet
                                    if (list_of_cmd_lines[counter, 14].Equals("0")) { list_of_cmd_lines[counter, 14] = "0.000000"; } else if (!list_of_cmd_lines[counter, 14].Contains(".")) { list_of_cmd_lines[counter, 14] = list_of_cmd_lines[counter, 14] + ".000000"; }

                                    list_of_cmd_lines[counter, 15] = Convert.ToDouble(DL_PoidsBrut).ToString().Replace(",", "."); // DL_PoidsBrut
                                    if (list_of_cmd_lines[counter, 15].Equals("0")) { list_of_cmd_lines[counter, 15] = "0.000000"; } else if (!list_of_cmd_lines[counter, 15].Contains(".")) { list_of_cmd_lines[counter, 15] = list_of_cmd_lines[counter, 15] + ".000000"; }

                                    list_of_cmd_lines[counter, 16] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixUnitaire
                                    if (list_of_cmd_lines[counter, 16].Equals("0")) { list_of_cmd_lines[counter, 16] = "0.000000"; } else if (!list_of_cmd_lines[counter, 16].Contains(".")) { list_of_cmd_lines[counter, 16] = list_of_cmd_lines[counter, 16] + ".000000"; }

                                    list_of_cmd_lines[counter, 17] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_PrixRU
                                    if (list_of_cmd_lines[counter, 17].Equals("0")) { list_of_cmd_lines[counter, 17] = "0.000000"; } else if (!list_of_cmd_lines[counter, 17].Contains(".")) { list_of_cmd_lines[counter, 17] = list_of_cmd_lines[counter, 17] + ".000000"; }

                                    list_of_cmd_lines[counter, 18] = DL_PrixUnitaire_salePriceHT.ToString().Replace(",", "."); // DL_CMUP
                                    list_of_cmd_lines[counter, 19] = "Heure";    //DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                    list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                    if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                    list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite) * Convert.ToDouble(DL_PrixUnitaire_salePriceHT)).ToString().Replace(",", "."); //DL_MontantHT
                                    list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite) * product_ttc).ToString().Replace(",", "."); //DL_MontantTTC
                                    if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                    if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                    if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                    list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                    list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                    list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                    list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte
                                    list_of_cmd_lines[counter, 27] = DL_PUTTC; //DL_PUTTC
                                    list_of_cmd_lines[counter, 28] = "0";   //DL_TTC

                                    list_of_cmd_lines[counter, 29] = DL_PieceBCF;   //DL_PieceBC
                                    list_of_cmd_lines[counter, 30] = reference_BLF_doc;   //DL_PieceBL
                                    list_of_cmd_lines[counter, 31] = curr_date;   // DL_DateBL
                                    list_of_cmd_lines[counter, 32] = "0";   //DL_TNomencl
                                    list_of_cmd_lines[counter, 33] = "0";   //DL_TRemPied
                                    list_of_cmd_lines[counter, 34] = "0";   //DL_TRemExep
                                    list_of_cmd_lines[counter, 35] = DL_QteBCF.Replace(",", ".");   //DL_QteBC
                                    list_of_cmd_lines[counter, 36] = Convert.ToInt16(line.Quantite).ToString().Replace(",", ".");   //DL_QteBL
                                    list_of_cmd_lines[counter, 37] = "0.000000";    //DL_Remise01REM_Valeur
                                    list_of_cmd_lines[counter, 38] = "0";           //DL_Remise01REM_Type
                                    list_of_cmd_lines[counter, 39] = "0.000000";    //DL_Remise02REM_Valeur
                                    list_of_cmd_lines[counter, 40] = "0";           //DL_Remise02REM_Type
                                    list_of_cmd_lines[counter, 41] = "0.000000";    //DL_Remise03REM_Valeur
                                    list_of_cmd_lines[counter, 42] = "0";           //DL_Remise03REM_Type
                                    list_of_cmd_lines[counter, 43] = "1";                   //DL_NoRef
                                    list_of_cmd_lines[counter, 44] = "0";                   //DL_TypePL
                                    list_of_cmd_lines[counter, 45] = "0.000000";            //DL_PUDevise
                                    list_of_cmd_lines[counter, 46] = "";                    //CA_Num
                                    list_of_cmd_lines[counter, 47] = "0.000000";            //DL_Frais
                                    list_of_cmd_lines[counter, 48] = "";                    //AC_RefClient
                                    list_of_cmd_lines[counter, 49] = "";                   //DL_PiecePL
                                    list_of_cmd_lines[counter, 50] = "NULL"; //curr_date;                    //DL_DatePL
                                    list_of_cmd_lines[counter, 51] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                   //DL_QtePL
                                    list_of_cmd_lines[counter, 52] = "";                    //DL_NoColis
                                    list_of_cmd_lines[counter, 53] = "0";                   //DL_NoLink
                                    list_of_cmd_lines[counter, 54] = "0";                    //CO_No
                                    list_of_cmd_lines[counter, 55] = "0";                //DT_No
                                    list_of_cmd_lines[counter, 56] = "";                    //DL_PieceDE
                                    list_of_cmd_lines[counter, 57] = "NULL";                   //DL_DateDe
                                    list_of_cmd_lines[counter, 58] = "0.000000"; //Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");                    //DL_QteDE
                                    list_of_cmd_lines[counter, 59] = "0";                  //DL_NoSousTotal
                                    list_of_cmd_lines[counter, 60] = "0";                //CA_No
                                    list_of_cmd_lines[counter, 61] = "0.000000";            // DL_PUBC
                                    list_of_cmd_lines[counter, 62] = DL_CodeTaxe1;                  // DL_CodeTaxe1
                                    list_of_cmd_lines[counter, 63] = DL_Taxe1.ToString().Replace(",", ".");         // DL_Taxe1
                                    list_of_cmd_lines[counter, 64] = "0.000000";            // DL_Taxe2
                                    list_of_cmd_lines[counter, 65] = "0.000000";            // DL_Taxe3
                                    list_of_cmd_lines[counter, 66] = "0";                   // DL_TypeTaux1
                                    list_of_cmd_lines[counter, 67] = "0";                   // DL_TypeTaxe1
                                    list_of_cmd_lines[counter, 68] = "0";                   // DL_TypeTaux2
                                    list_of_cmd_lines[counter, 69] = "0";                   // DL_TypeTaxe2
                                    list_of_cmd_lines[counter, 70] = "0";                   // DL_TypeTaux3
                                    list_of_cmd_lines[counter, 71] = "0";                   // DL_TypeTaxe3

                                    list_of_cmd_lines[counter, 72] = "1";                                           // DL_MvtStock
                                    list_of_cmd_lines[counter, 73] = "";                                            // AF_RefFourniss
                                    list_of_cmd_lines[counter, 74] = ((COLIS_article == null || COLIS_article == "") ? "0.0" : COLIS_article).ToString().Replace(",", ".");    // COLIS
                                    list_of_cmd_lines[counter, 75] = ((PCB_article == null || PCB_article == "") ? "0.0" : PCB_article).ToString().Replace(",", ".");      // PCB
                                    list_of_cmd_lines[counter, 76] = COMPLEMENT_article;                            // COMPLEMENT
                                    list_of_cmd_lines[counter, 77] = "";                                            // PourVeolog
                                    list_of_cmd_lines[counter, 78] = "";                                            // DL_PieceOFProd
                                    list_of_cmd_lines[counter, 79] = "";                                            // DL_Operation
                                    list_of_cmd_lines[counter, 80] = veologDeliveryDateTime;    //DO_DateLivr
                                    list_of_cmd_lines[counter, 81] = "0";    //DL_NonLivre

                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ******************** Exception Line ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Le tableau 'BLF' à 2 dimensions ne fonctionne pas correctement, message: " + ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }
                            }

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Compter => " + counter);
                            counter++;

                        }
                        // ===== End Foreach =====

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Vérifier si un produit pour 1 = BLF");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info));

                        //generate document BLF_____. in database.
                        try
                        {
                            OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocument_Veolog(true, "13", reference_BLF_doc, curr_date, veologDeliveryDateTime, dh, nature_op_p_, do_totalHT_, do_totalHTNet_, do_totalTTC_, do_NetAPayer_, do_MontantRegle_, list_of_supplier_info), connexion); //calling the query and parsing the parameters into it
                            command.ExecuteReader(); // executing the query
                        }
                        catch (OdbcException ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException *********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }


                        string[,] products_BCF = new string[position_item / 1000, 82]; // create array with enough space

                        //insert documentline into the database with articles having 20 as value @index 2
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : insert documentline into the database with articles having 13 as value @index 2");

                        for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                        {
                            if (list_of_cmd_lines[x, 1] == "13")
                            {
                                for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                                {
                                    products_BCF[x, y] = list_of_cmd_lines[x, y];
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : products_BLF_L[" + x + "," + y + "] = " + products_BCF[x, y]);
                                }

                                //insert the article to documentline in the database
                                try
                                {
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : insert the article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") to documentline in the database");

                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x));

                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertBcfDocumentLine_Veolog(true, products_BCF, x), connexion);
                                    command.ExecuteReader();
                                }
                                catch (OdbcException ex)
                                {
                                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException *********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }

                                //Update docline artile stock
                                try
                                {
                                    bool found_stock = false;
                                    double AS_StockReel = 0.0;
                                    double AS_StockReserve = 0.0;
                                    double AS_StockMontant = 0.0;
                                    double productPrixUnite = Convert.ToDouble(products_BCF[x, 16].Replace('.', ','));
                                    double AS_StockCommande = 0.0;

                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : get current stock in F_ARTSTOCK table in the database");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.getArticleStock(true, products_BCF[x, 9]));
                                    using (OdbcCommand command_ = new OdbcCommand(QueryHelper.getArticleStock(true, products_BCF[x, 9]), connexion)) //execute the function within this statement : getNegativeStockOfAProduct()
                                    {
                                        using (IDataReader reader = command_.ExecuteReader()) //read rows of the executed query
                                        {
                                            if (reader.Read()) // If any rows returned
                                            {
                                                found_stock = true;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Stock trouvé : AS_StockReel (" + reader[0].ToString() + "), AS_StockReserve (" + reader[1].ToString() + "), AS_StockMontant (" + reader[2].ToString() + "), AS_StockCommande (" + reader[3].ToString() + ").");
                                                AS_StockReel = Convert.ToDouble(reader[0].ToString().Replace(".", ","));
                                                AS_StockReserve = Convert.ToDouble(reader[1].ToString().Replace(".", ","));
                                                AS_StockMontant = Convert.ToDouble(reader[2].ToString().Replace(".", ","));
                                                AS_StockCommande = Convert.ToDouble(reader[3].ToString().Replace(".", ","));
                                            }
                                            else// If no rows returned
                                            {
                                                //do nothing.
                                                found_stock = false;
                                                logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Aucune reponse.");
                                                logFileWriter.Flush();
                                                recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", "Echec à la récupération du stock de l'article " + products_BCF[x,9], "insertSupplierOrder() : Aucune reponse.", fileName, logFileName_import));
                                                return null;
                                            }
                                        }
                                    }


                                    if (found_stock)
                                    {
                                        //Calculate stock info
                                        double AS_CMUP = AS_StockMontant / AS_StockReel;
                                        double new_AS_StockReel = AS_StockReel + Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        //double new_AS_StockReserve = AS_StockReserve - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        double new_AS_StockCommande = AS_StockCommande - Convert.ToDouble(products_BCF[x, 13].Replace('.', ','));
                                        double new_AS_StockMontant = new_AS_StockReel * productPrixUnite;
                                        double new_AS_CMUP = new_AS_StockMontant / new_AS_StockReel;

                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Calculation...");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockReel: " + new_AS_StockReel + " = AS_StockReel: " + AS_StockReel + " + products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        //logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockReserve: " + new_AS_StockReserve + " = AS_StockReserve: " + AS_StockReserve + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockCommande: " + new_AS_StockCommande + " = AS_StockCommande: " + AS_StockCommande + " - products_DESADV[x, 13]: " + Convert.ToDouble(products_BCF[x, 13].Replace('.', ',')));
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_StockMontant: " + new_AS_StockMontant + " = new_AS_StockReel: " + new_AS_StockReel + " X productPrixUnite: " + productPrixUnite);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : new_AS_CMUP: " + new_AS_CMUP + " = new_AS_StockMontant: " + new_AS_StockMontant + " / new_AS_StockReel: " + new_AS_StockReel);

                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : update article " + products_BCF[x, 12] + " (Ref:" + products_BCF[x, 9] + ") stock in F_ARTSTOCK table in the database");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : requette sql ===> " + QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant));

                                        OdbcCommand command = new OdbcCommand(QueryHelper.updateArticleStockBLF(true, products_BCF[x, 9], new_AS_StockReel, new_AS_StockCommande, new_AS_StockMontant), connexion);
                                        command.ExecuteReader();
                                    }
                                }
                                catch (OdbcException ex)
                                {
                                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                    logFileWriter.WriteLine("");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** OdbcException Update F_ARTSTOCK table *********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Import annulée");
                                    logFileWriter.Flush();
                                    recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                                    return null;
                                }
                            }
                        }

                        //set Veolog date time import
                        try
                        {
                            string delivery_date_veolog = string.Format("{0:dd/MM/yyyy hh:mm:ss}", DateTime.Now);
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Ajouter la date de livraision \"" + delivery_date_veolog + "\" de Veolog au BLF \"" + reference_BLF_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateVeologDeliveryDate(true, reference_BLF_doc, delivery_date_veolog + "   " + dh.Ref_Commande_Donneur_Ordre), connexion);
                            {
                                using (IDataReader reader = command.ExecuteReader())
                                {
                                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Date de livraison veolog à jour !");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur Veolog Date BCF ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }

                        //update document numbering
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Mettre à jour la numérotation du document \"" + reference_BLF_doc + "\".");

                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc));
                            OdbcCommand command = new OdbcCommand(QueryHelper.updateDOC_NumerotationTable(true, ((reference_BLF_doc == "BLF") ? "BLF" : "LF"), reference_BLF_doc), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Nouvelle numérotation à jour!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }

                        //Delete the BC of the BL
                        try
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Supprimer le Bon de Commande (BCF) \"" + dh.Ref_Commande_Donneur_Ordre + "\".");
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : SQL ===> " + QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre));
                            OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, dh.Ref_Commande_Donneur_Ordre), connexion);
                            IDataReader reader = command.ExecuteReader();
                            logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Bon de Commande Fournisseur supprimé!");
                        }
                        catch (Exception ex)
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " ********** Erreur ********** ");
                            logFileWriter.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                            logFileWriter.WriteLine(DateTime.Now + " Export Annuler.");
                            logFileWriter.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : ********************** Exception 1 *********************");
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : Message :: " + ex.Message);
                    logFileWriter.WriteLine(DateTime.Now + " | insertSupplierOrder() : StackTrace :: " + ex.StackTrace);
                    connexion.Close(); //disconnect from database
                    logFileWriter.Flush();
                    recapLinesList_new.Add(new CustomMailRecapLines(reference_BLF_doc, "", "L'import du bon de livraison fournisseur est annulée.", ex.Message, ex.StackTrace, fileName, logFileName_import));
                    return null;
                }
            }

            return list_of_cmd_lines;
        }
        public static string lastNumberReference(string mask, StreamWriter logFileWriter)
        {

            string db_result = "";
            string result = "";

            if (mask == "ME") // Mouvement Entree
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask ME");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if(checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "STK_ME"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "STK_ME"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "ME200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask ME : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }
                        
                        //Get the last doc reference in from the ME list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command1 = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion);
                            using (IDataReader reader = command1.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask ME : " + db_result);
                                }
                                else
                                {
                                    db_result = "ME00000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask ME : " + db_result);
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        Console.WriteLine("Message : " + ex.Message + ".");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  SQL ===> " + QueryHelper.getLastPieceNumberReference(false, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  Import annulée");
                        return null;
                    }

                    try
                    {
                        //ME00001
                        int chiffreTotal = 7;
                        int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                        int newMaskID = lastMaskID + 1;

                        result = mask; // put ME before adding '0'
                        string zeros = "";
                        string result_ = result + "" + newMaskID;

                        for (int i = result_.Length; i < chiffreTotal; i++)
                        {
                            zeros += "0";
                        }
                        result += zeros + "" + newMaskID;

                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask ME : " + result);
                    }
                    catch (Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** Exception *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : Nouveau mask ME ne peut pas etre cree");
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }

                    
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "MS") // Mouvement Sortie
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask MS");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if (checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "STK_MS"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "STK_MS"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "MS200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask MS : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }

                        //Get the last doc reference in from the ME list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion); 
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask MS : " + db_result);
                                }
                                else
                                {
                                    db_result = "MS00000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask MS : " + db_result);
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        Console.WriteLine("Message : " + ex.Message + ".");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(false, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }


                    try
                    {
                        //MS00001
                        int chiffreTotal = 7;
                        int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                        int newMaskID = lastMaskID + 1;

                        result = mask; // put ME before adding '0'
                        string zeros = "";
                        string result_ = result + "" + newMaskID;

                        for (int i = result_.Length; i < chiffreTotal; i++)
                        {
                            zeros += "0";
                        }
                        result += zeros + "" + newMaskID;

                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask MS : " + result);
                    }
                    catch(Exception ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** Exception *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : Nouveau mask ME ne peut pas etre cree");
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "BL") // Bonn de Livraison
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask BL");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if (checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "BL"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "BL"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "BL200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BL : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }

                        //Get the last doc reference in from the ME list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion);
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BL : " + db_result);
                                }
                                else
                                {
                                    db_result = "BL00000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask BL : " + db_result);
                                }
                            }
                        }

                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(true, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }

                    //ME00001
                    int chiffreTotal = 7;
                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | db_result.Replace(mask, '') == " + db_result.Replace(mask, ""));
                    int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                    int newMaskID = lastMaskID + 1;

                    result = mask; // put ME before adding '0'
                    string zeros = "";
                    string result_ = result + "" + newMaskID;

                    for (int i = result_.Length; i < chiffreTotal; i++)
                    {
                        zeros += "0";
                    }
                    result += zeros + "" + newMaskID;

                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask BL : " + result);
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "BC") // Bon de Commande
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask BC");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if (checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "BC"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "BC"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "BC200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BC : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }

                        //Get the last doc reference in from the ME list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion);
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BC : " + db_result);
                                }
                                else
                                {
                                    db_result = "BC00000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask BC : " + db_result);
                                }
                            }
                        }

                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(true, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }

                    //ME00001
                    int chiffreTotal = 7;
                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | db_result.Replace(mask, '') == " + db_result.Replace(mask, ""));
                    int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                    int newMaskID = lastMaskID + 1;

                    result = mask; // put ME before adding '0'
                    string zeros = "";
                    string result_ = result + "" + newMaskID;

                    for (int i = result_.Length; i < chiffreTotal; i++)
                    {
                        zeros += "0";
                    }
                    result += zeros + "" + newMaskID;

                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask BC : " + result);
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "BLF") // Bon de Commande Fournisseur
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Récupérer le dernier mask BLF");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if (checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "BLF"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "BLF"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "BLF200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BLF : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }

                        //Get the last doc reference in from the BCF list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion);
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask BLF : " + db_result);
                                }
                                else
                                {
                                    db_result = "BLF000000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask BLF : " + db_result);
                                }
                            }
                        }

                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(true, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }

                    int chiffreTotal = 7;
                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | db_result.Replace(mask, '') == " + db_result.Replace(mask, ""));
                    int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                    int newMaskID = lastMaskID + 1;

                    result = mask; // put ME before adding '0'
                    string zeros = "";
                    string result_ = result + "" + newMaskID;

                    for (int i = result_.Length; i < chiffreTotal; i++)
                    {
                        zeros += "0";
                    }
                    result += zeros + "" + newMaskID;

                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask BLF : " + result);
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "LF") // Bon de Commande Fournisseur
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Récupérer le dernier mask LF");

                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //check if DOC_Numerotation existe
                        //And get the last saved doc reference if differente from factory init mask
                        bool DOC_Numerotation_exist = false;
                        if (checkDOC_Numerotation(logFileWriter) == 1)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getDOC_NumerotationTable(true, "BLF"));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getDOC_NumerotationTable(true, "BLF"), connexion);
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    if (reader[0].ToString() != "LF200000")
                                    {
                                        DOC_Numerotation_exist = true;
                                        db_result = reader[0].ToString();
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial est changé, alors j'utilise le mask dans l'argument.");
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask LF : " + db_result);
                                    }
                                    else
                                    {
                                        DOC_Numerotation_exist = false;
                                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Le mask initial n'est pas changé, alors c'est la première utilisation.");
                                    }
                                }
                            }
                        }

                        //Get the last doc reference in from the BCF list 
                        if (!DOC_Numerotation_exist)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));
                            OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connexion);
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // reads lines/rows from the query
                                {
                                    db_result = reader[0].ToString();
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Mask LF : " + db_result);
                                }
                                else
                                {
                                    db_result = "LF000000";
                                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Premiere Mask LF : " + db_result);
                                }
                            }
                        }

                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(true, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        return null;
                    }

                    int chiffreTotal = 7;
                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | db_result.Replace(mask, '') == " + db_result.Replace(mask, ""));
                    int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                    int newMaskID = lastMaskID + 1;

                    result = mask; // put ME before adding '0'
                    string zeros = "";
                    string result_ = result + "" + newMaskID;

                    for (int i = result_.Length; i < chiffreTotal; i++)
                    {
                        zeros += "0";
                    }
                    result += zeros + "" + newMaskID;

                    logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask LF : " + result);
                }
                logFileWriter.WriteLine("");
                return result;
            }
            return null;
        }

        public static int checkDOC_Numerotation(StreamWriter writer)
        {
            int result = -1;
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : Vérifier si la table de numérotation existe");

            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connexion.Open();
                    writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : SQL => " + QueryHelper.checkDOC_NumerotationTable(true));
                    OdbcCommand command = new OdbcCommand(QueryHelper.checkDOC_NumerotationTable(true), connexion);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[0].ToString() == "1")
                            {
                                result = 1;
                                writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : La table de numérotation existe avec des données");
                            }
                            else
                            {
                                result = 0;
                                writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : La table de numérotation existe sans des données");
                            }
                        }
                        else
                        {
                            result = 0;
                            writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : Aucune donnée n'existe pour la ligne/colonne.");
                        }
                    }
                    connexion.Close();
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : ******************** Erreur ********************");
                    writer.WriteLine(DateTime.Now + " : Erreur[201]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    result = -1;
                    connexion.Close();
                }
            }
            writer.WriteLine("");
            return result;
        }

        public static bool initDOC_Numerotation(StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Init");
            bool result = false;
            int check = checkDOC_Numerotation(writer);

            if (check == 1)   //if the DOC_Numerotation do nothing
            {
                result = true;
                writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation existe!");
            }
            else if (check == 0 || check == -1)      //if the tDOC_Numerotation doesn't exist then create it 
            {
                writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation does not existe, so create the table!");
                using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connexion.Open();

                        //Create DOC_Numerotation Table
                        writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : SQL => "+ QueryHelper.createDOC_NumerotationTable(true));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.createDOC_NumerotationTable(true), connexion))
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation created!");
                            }
                        }

                        ConfigurationDNS dns = new ConfigurationDNS();
                        dns.LoadSQL();

                        string initMask = "";
                        if(dns.Prefix == "CFCI")
                        {
                            initMask = "BLF200000";
                        }
                        else if (dns.Prefix == "TABLEWEAR")
                        {
                            initMask = "LF200000";
                        }
                        else
                        {
                            initMask = "BLF200000";
                        }

                        //Set up the first init Numérotation
                        writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : SQL => " + QueryHelper.insertDOC_NumerotationTable(true, "BC200000", "BL200000", "BLF200000", "ME200000", "MS200000"));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.insertDOC_NumerotationTable(true, "BC200000", "BL200000", initMask, "ME200000", "MS200000"), connexion))
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                result = true;
                                writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation created!");
                            }
                        }
                        connexion.Close();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : ******************** Erreur ********************");
                        writer.WriteLine(DateTime.Now + " : Erreur[200]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    }
                }
            }
            return result;
        }

        public static Client getClient(string id, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(false, id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - Code Client " + id + " n'existe pas dans la base sage.");
                                writer.WriteLine(DateTime.Now + " : Erreur - Code Client " + id + " n'existe pas dans la base sage.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    writer.WriteLine(DateTime.Now + " : SQL ===> "+ QueryHelper.getClient(false, id));
                    writer.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    //Console.Read();
                    return null;
                }
            }

        }

        public static string getStockId(StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getStockId() : Called!");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getStockId() : SQL ===> " + QueryHelper.getStockId(false));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getStockId(false), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string id = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | getStockId() : Stock trouvé,  ID => "+id);
                                connection.Close();
                                return id;

                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | getStockId() : Erreur - Il n'y a pas de stock enregistré.");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " | getStockId() : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return null;
                }
            }

        }

        public static string getNumLivraison(string client_num, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getNumLivraison() : Client Numéro => "+client_num);
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getNumLivraison() : SQL ===> " + QueryHelper.getNumLivraison(false, client_num));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getNumLivraison(false, client_num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string num = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | getNumLivraison() : Numero de livraison existe pour le client!");
                                connection.Close();
                                return num;
                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | getNumLivraison() : Erreur - Numero de livraison n'existe pas pour le client!");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " | getNumLivraison() : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return null;
                }
            }

        }

        /*
        public static string insertCommande(Client client, Order order)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insertCommande(client, order), connection);
                    command.ExecuteReader();
                    connection.Close();
                    return "OK";


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    if (ex.Message.IndexOf("Ce numéro de pièce existe déjà") != -1)
                        return "ERROR_PIECE";
                    else
                        return "ERROR";
                }
            }

        }
         * */

        public static Boolean insertLigneCommande(Client client, Order order, OrderLine orderLine, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {

                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertLigneCommande(false, client, order, orderLine), connection);
                    command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");
                    writer.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");

                    //Console.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return false;
                }


                connection.Close();
                return true;



            }

        }

        public static Article getArticle(string code_article, string CT_NumCentrale, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getArticle() : Code Article : "+code_article);
            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getArticle() : SQL ===>  " + QueryHelper.getArticle(true, code_article, CT_NumCentrale));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticle(true, code_article, CT_NumCentrale), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Article article = new Article(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                                writer.WriteLine(DateTime.Now + " | getArticle() : Article trouvé!");
                                connection.Close();
                                return article;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - code article " + code_article + " n'existe pas dans la base.");
                                if(CT_NumCentrale != null && CT_NumCentrale != "")
                                {
                                    writer.WriteLine(DateTime.Now + " | getArticle() : Erreur - code article " + code_article + " n'existe pas dans la base pour le client ou client centrale " + CT_NumCentrale + ".");
                                }
                                else
                                {
                                    writer.WriteLine(DateTime.Now + " | getArticle() : Erreur - code article " + code_article + " n'existe pas dans la base.");
                                }
                                return null;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    writer.WriteLine(DateTime.Now + " | getArticle() : Erreur[2]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return null;
                }
            }

        }

        public static Classes.Path getPath()
        {
            try
            {
                Classes.Path path = new Classes.Path();
                path.Load();
                return path;
            }
            catch(Exception e)
            {
                Console.WriteLine(DateTime.Now + " : Error : " + e.Message);
                return null;
            }
        }


        public string InfoTachePlanifier(StreamWriter writer)
        {
            try
            {
                writer.WriteLine("");
                string taskName = "importCommandeSage";

                ConfigurationDNS settings = new ConfigurationDNS();
                settings.LoadSQL();

                if(settings.Prefix == "CFCI")
                {
                    taskName = "importCommandeSage_CFCI";
                }
                else if (settings.Prefix == "TABLEWEAR")
                {
                    taskName = "importCommandeSage_TW";
                }
                writer.WriteLine(DateTime.Now + " | InfoTachePlanifier() : Trouver le nom de la tache = " + taskName);

                TaskService ts = new TaskService();
                if (ts.FindTask(taskName, true) != null)
                {
                    Task t = ts.GetTask(taskName);
                    TaskDefinition td = t.Definition;
                    writer.WriteLine(DateTime.Now + " | InfoTachePlanifier() : taskName trigger[0] = "+ td.Triggers[0]);
                    writer.WriteLine("");
                    writer.WriteLine("");
                    return "" + td.Triggers[0];
                }
                else
                {
                    //Console.WriteLine("Il n'y a pas d'import planifiée enregistré.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.WriteLine("");
                writer.WriteLine("");
                return null;
            }
        }

        public static Boolean deleteCommande(bool typeSQL, string NumCommande)
        {
            if (typeSQL)
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    try
                    {
                        connection.Open();
                        OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(true, NumCommande), connection);
                        command.ExecuteReader();
                        connection.Close();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    try
                    {
                        connection.Open();
                        OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(false, NumCommande), connection);
                        command.ExecuteReader();
                        connection.Close();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
        public static Boolean deleteCommandeLigne(string NumCommande)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.deleteLigneDocument(false, NumCommande), connection);
                    command.ExecuteReader();
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        public static string testGamme(int type, string code_article, string gamme, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | testGamme() : Type '"+type+"', Code Article '"+code_article+"', Gamme '"+gamme+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | testGamme() : SQL ===> " + QueryHelper.getGAMME(false, type, code_article));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getGAMME(false, type, code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            List<string> list = new List<string>();
                            while (reader.Read())
                            {
                                writer.WriteLine(DateTime.Now + " | testGamme() : Gamme ("+reader[0].ToString()+") Trouvé!");
                                list.Add(reader[0].ToString());
                            }

                            Boolean ok = false;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (gamme == list[i])
                                    ok = true;
                            }

                            if (!ok && list.Count > 0)
                            {
                                writer.WriteLine(DateTime.Now + " | testGamme() : Return la 1ere dans la liste => "+list[0]);
                                return list[0];
                            }

                            writer.WriteLine(DateTime.Now + " | testGamme() : Return => "+gamme);
                            return gamme;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " | testGamme() : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return gamme;
                }
            }

        }


        public static string existeCommande(string num, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | existeCommande() : SQL ===> "+ QueryHelper.get_NumPiece_Motif(false, num));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_NumPiece_Motif(false, num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string numero = reader[0].ToString();
                                connection.Close();
                                return numero;

                            }
                            else
                            {
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static string existeCommande_v2(string num, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | existeCommande_v2() : Numéro du Doc '"+num+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_NumPiece_Motif(false, num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string numero = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | existeCommande_v2() : Doc '" + numero + "' trouvé!!");
                                connection.Close();
                                return numero;

                            }
                            else
                            {
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static string MaxNumPiece(StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.MaxNumPiece(false), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return "BC00000";
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }
        }

        public static string MaxNumPiece_v2(string mask, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | MaxNumPiece_v2() : Mask '"+mask+"'");
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.MaxNumPiece_v2(false, mask), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string num = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | MaxNumPiece_v2() : Mask '" + num + "'");
                                connection.Close();
                                return num;
                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | MaxNumPiece_v2() : Mask 'BC000000'");
                                return "BC00000";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }
        }

        public static string NextNumPiece(StreamWriter writer)
        {
            try
            {
                string NumCommande = MaxNumPiece(writer);

                NumCommande = NumCommande.Replace("BC", "");

                if (IsNumeric(NumCommande))
                {
                    int Nombre = int.Parse(NumCommande) + 1;
                    string num = Nombre.ToString();

                    while (num.Length < 5)
                    {
                        num = "0" + num;
                    }

                    NumCommande = "BC" + num;

                    return NumCommande;

                }

                return null;
            }
            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                //Console.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return "erreur";
            }
        }

        public static string NextNumPiece_v2(string mask, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | NextNumPiece_v2() : Mask '" + mask + "'");
            try
            {
                string NumCommande = MaxNumPiece_v2(mask, writer);

                NumCommande = NumCommande.Replace("BC", "");

                if (IsNumeric(NumCommande))
                {
                    int Nombre = int.Parse(NumCommande) + 1;
                    string num = Nombre.ToString();

                    while (num.Length < 5)
                    {
                        num = "0" + num;
                    }

                    NumCommande = "BC" + num;

                    writer.WriteLine(DateTime.Now + " | NextNumPiece_v2() : New Mask '" + NumCommande + "'");
                    return NumCommande;
                }
                else
                {
                    writer.WriteLine(DateTime.Now + " | NextNumPiece_v2() : ********** Erreur NextNumPiece_v2 **********");
                    writer.WriteLine(DateTime.Now + " | NextNumPiece_v2() : Mask number '"+ NumCommande + "' is not a number!!");
                }

                return null;
            }
            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                //Console.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return "erreur";
            }
        }

        public static string get_next_num_piece_commande(StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | get_next_num_piece_commande() : SQL - "+ QueryHelper.get_Next_NumPiece_BonCommande(false));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_Next_NumPiece_BonCommande(false), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;
                            }
                            else
                            {
                                return NextNumPiece(writer);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }
        }

        public static string get_next_num_piece_commande_v2(string mask, StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " | get_next_num_piece_commande_v2() | Mask '"+mask+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    string result = "";
                    string db_result = "";

                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | NextNumPiece_v2() : SQL ==> " + QueryHelper.get_Next_NumPiece_BonCommande_v2(true, mask));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_Next_NumPiece_BonCommande_v2(true, mask), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                db_result = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | get_next_num_piece_commande_v2() | New Mask '" + db_result + "'");
                                connection.Close();
                                //return num;
                            }
                            else
                            {
                                return NextNumPiece_v2(mask, writer);
                            }
                        }
                    }

                    int chiffreTotal = 7;
                    writer.WriteLine(DateTime.Now + " : lastNumberReference() | db_result.Replace(mask, '') == " + db_result.Replace(mask, ""));
                    int lastMaskID = Convert.ToInt32(db_result.Replace(mask, ""));
                    int newMaskID = lastMaskID + 1;

                    result = mask; // put ME before adding '0'
                    string zeros = "";
                    string result_ = result + "" + newMaskID;

                    for (int i = result_.Length; i < chiffreTotal; i++)
                    {
                        zeros += "0";
                    }
                    result += zeros + "" + newMaskID;

                    writer.WriteLine(DateTime.Now + " : lastNumberReference() | Nouveau mask : " + result);
                    writer.WriteLine("");
                    return result;
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine("");
                    return null;
                }
            }
        }



        public static Boolean IsNumeric(string Nombre)
        {
            try
            {
                long.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static string get_condition_livraison(string c_mode, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | get_condition_livraison() : C_Mode '"+c_mode+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | get_condition_livraison() : SQL ===> "+ QueryHelper.get_condition_livraison_indice(false, c_mode));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_condition_livraison_indice(false, c_mode), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string num = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | get_condition_livraison() : Condition Livraison trouvé ("+num+"). ");
                                connection.Close();
                                return num;
                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | get_condition_livraison() : Condition Livraison n'est pas trouvé! ");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[29] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " | get_condition_livraison() : Erreur[29] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }
        }

        public static List<AdresseLivraison> get_adresse_livraison(AdresseLivraison adresse, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | get_adresse_livraison() : Called1");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<AdresseLivraison> list = new List<AdresseLivraison>();
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | get_adresse_livraison() : SQL ===> "+ QueryHelper.get_adresse_livraison(false, adresse));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_adresse_livraison(false, adresse), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                writer.WriteLine(DateTime.Now + " | get_adresse_livraison() :  Adresse Livraison trouvé!");
                                list.Add(new AdresseLivraison(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), "", ""));
                            }
                            return list;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " | get_adresse_livraison() : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }

        }

        public static Boolean insert_adresse_livraison(string client, AdresseLivraison adresse, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insert_adresse_livraison(false, client, adresse), connection);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return false;
                }
            }

        }

        public static string existeFourniseur(string num, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | existeFourniseur() : id '"+num+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | existeFourniseur() : SQL ===> " + QueryHelper.fournisseurExiste(false, num));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.fournisseurExiste(false, num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string numero = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | existeFourniseur() : Fournisseur trouvé!");
                                connection.Close();
                                return numero;
                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur[35] - Code GLN fournisseur " + num + " n'existe pas.");
                                writer.WriteLine(DateTime.Now + " | existeFourniseur() : Code GLN fournisseur " + num + " n'existe pas.");
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(DateTime.Now + " : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " | existeFourniseur() : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }

        }

        public static string get_Last_insert_livraison(string client, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_last_Num_Livraison(false, client), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {

                                //Console.WriteLine(DateTime.Now + " : Erreur[39] - Numero de livraison n'existe pas");
                                writer.WriteLine(DateTime.Now + " : Erreur[39] - Numero de livraison n'existe pas");


                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[37] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[37] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static Client getClient(string id, int flag, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getClient() : Erreur[41] - SQL :: " + QueryHelper.getClient(false, id));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(false, id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                if (flag == 1)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " | getClient() : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                }
                                if (flag == 2)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " | getClient() : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                }
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Exceptions pouvant survenir durant l'exécution de la requête SQL
                    // Console.WriteLine(DateTime.Now + " : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    //writer.WriteLine(DateTime.Now + " | getClient() : SQL ===> " + QueryHelper.getClient(false, id));
                    writer.WriteLine(DateTime.Now + " | getClient() : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }
        }

        public static Client getClient_v2(string id, int flag, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getClient_v2() : id '"+id+"' and flag '"+flag+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getClient_v2() : SQL ===> " + QueryHelper.getClient(true, id));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(true, id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                                writer.WriteLine(DateTime.Now + " | getClient_v2() : Client trouvé!");
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                if (flag == 1)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " | getClient_v2() :  GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                }
                                if (flag == 2)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " | getClient_v2() : GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                }
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Exceptions pouvant survenir durant l'exécution de la requête SQL
                    writer.WriteLine(DateTime.Now + " : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }
        }

        public static string getDevise(string codeIso, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getDevise() : Code ISO '"+codeIso+"'");
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getDevise() : SQL ===> " + QueryHelper.getDevise(false, codeIso));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getDevise(false, codeIso), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string num = reader[0].ToString();
                                writer.WriteLine(DateTime.Now + " | getDevise() : Devise trouvé!");
                                connection.Close();
                                return num;
                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | getDevise() : Aucune reponse!");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    writer.WriteLine(DateTime.Now + " | getDevise() : Erreur[44] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }

        }

        public static ConfSendMail getInfoMail(StreamWriter writer)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " : getInfoMail() | File => " + Directory.GetCurrentDirectory() + @"\SettingMail.xml");
                if (File.Exists(Directory.GetCurrentDirectory() + @"\SettingMail.xml"))
                {
                    ConfSendMail confMail = new ConfSendMail();
                    confMail.Load();
                    return confMail;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now + " : getInfoMail() | Erreur[43] - " + ex.Message);
                return null;
            }
        }

        public static void EnvoiMail(ConfSendMail confMail, string subject, string body, string attachement)
        {
            try
            {
                // Objet mail
                MailMessage msg = new MailMessage();

                // Expéditeur (obligatoire). Notez qu'on peut spécifier le nom
                msg.From = new MailAddress("conneteur.sage@cs.app", "CONNECTEUR SAGE");

                // Destinataires (il en faut au moins un)
                if (!string.IsNullOrEmpty(confMail.dest1))
                {
                    msg.To.Add(new MailAddress(confMail.dest1, confMail.dest1));
                }
                if (!string.IsNullOrEmpty(confMail.dest2))
                {
                    msg.To.Add(new MailAddress(confMail.dest2, confMail.dest2));
                }
                if (!string.IsNullOrEmpty(confMail.dest3))
                {
                    msg.To.Add(new MailAddress(confMail.dest3, confMail.dest3));
                }
                //msg.To.Add(new MailAddress("agent.smith@matrix.com", "Agent Smith"));

                // Destinataire(s) en copie (facultatif)
                //msg.Cc.Add(new MailAddress("wonder.woman@superhero.com", "Wonder Woman"));

                msg.Subject = subject;

                // Texte du mail (facultatif)
                msg.Body = body;

                // Fichier joint si besoin (facultatif)
                if (attachement != "" && File.Exists(attachement))
                {
                    msg.Attachments.Add(new Attachment(attachement));
                }

                SmtpClient client;

                if (confMail.smtp != "" && confMail.login != "" && confMail.password != "")
                {
                    // Envoi du message SMTP
                    client = new SmtpClient(confMail.smtp, confMail.port);
                    client.Credentials = new NetworkCredential(confMail.login, confMail.password);
                }
                else
                {
                    client = new SmtpClient("smtp.gmail.com", 587);
                    client.Credentials = new NetworkCredential("connecteur.sage@gmail.com", "@Amyaj2013");
                }

                client.EnableSsl = true;
                //NetworkInformation s = new NetworkCredential;
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                // Envoi du mail
                client.Send(msg);

                Console.WriteLine(DateTime.Now + " : Envoi de Mail..OK");

            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " : " + e.Message);
            }
        }

        public static Boolean TestSiNumPieceExisteDeja(string num, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestSiNumPieceExisteDeja(false, num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                connection.Close();
                                return true;

                            }
                            else
                            {
                                return false;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return false;
                }
            }

        }

        public static List<string> TestIntituleLivraison(string Intitule, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<string> adresses = new List<string>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestIntituleLivraison(false, Intitule), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                adresses.Add(reader[0].ToString());
                            }

                            return adresses;

                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[45] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static Conditionnement getConditionnementArticle(string code_article, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | getConditionnementArticle() : Code Article : "+code_article);
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    writer.WriteLine(DateTime.Now + " | getConditionnementArticle() : SQL ===> " + QueryHelper.getConditionnementArticle(false, code_article));
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getConditionnementArticle(false, code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Conditionnement Conditionnement = new Conditionnement(reader[0].ToString(), reader[1].ToString());
                                writer.WriteLine(DateTime.Now + " | getConditionnementArticle() : Conditionnement trouvé! ");
                                connection.Close();
                                return Conditionnement;

                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | getConditionnementArticle() : Conditionnement pas trouvé!");
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    writer.WriteLine(DateTime.Now + " | getConditionnementArticle() : Erreur[47] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }

        }

        public void LancerPlanification()
        {
            //Write in the log file
            if (!Directory.Exists(logDirectoryName_general))
            {
                //Create log directory
                Directory.CreateDirectory(logDirectoryName_general);
            }
            var logFileName_general = logDirectoryName_general + @"\" + string.Format("LOG_General_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_general = File.Create(logFileName_general);
            logFileWriter_general = new StreamWriter(logFile_general);

            logFileWriter_general.WriteLine("#####################################################################################");
            logFileWriter_general.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_general.WriteLine("#####################################################################################");
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine("################################ Import des documents ###############################");
            logFileWriter_general.WriteLine("");
            logFileWriter_general.Flush();

            this.ImportPlanifier();

            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine("");

            //this.SendToVeolog();
            Console.WriteLine("");
            Classes.Path path = new Path();
            
            try
            {
                path.Load();
            }catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now + " : No EDI folder || " + ex.Message);
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + " :: LancerPlanification() | No EDI folder\n " + ex.Message);
            }

            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine("################################ Export des documents ###############################");
            logFileWriter_general.WriteLine("");
            ConfigurationExport export = new ConfigurationExport();
            export.Load();
            Console.WriteLine(DateTime.Now + " : Path => " + path.path);


            if (recapLinesList_new.Count != 0)
            {
                recapLinesList_new.Clear();
            }

            List<CustomMailRecapLines> recapLinesList_new___ = new List<CustomMailRecapLines>();

            //Export des commandes
            if (((export.exportBonsCommandes == "True") ? true : false))
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Export Bons Commandes");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Commandes activé !");
                logFileWriter_general.WriteLine("");

                Classes.ExportCommandes c = new Classes.ExportCommandes(path.path);
                recapLinesList_new___ = c.ExportCommande(recapLinesList_new);
                recapLinesList_new.AddRange(recapLinesList_new___);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Option => Export Bon de commande est désactivé");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Commandes désactivé !");
                logFileWriter_general.WriteLine("");
            }
            logFileWriter_general.Flush();

            //Export des DESADV
            if (((export.exportBonsLivraisons == "True") ? true : false))
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Export Bons Livraisons");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Bons de livraisons activé !");
                logFileWriter_general.WriteLine("");

                Classes.ExportBonLivraison b = new Classes.ExportBonLivraison(path.path);
                recapLinesList_new___ = b.ExportBonLivraisonAction(recapLinesList_new);
                recapLinesList_new.AddRange(recapLinesList_new___);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Option => Export Bon de livraison est désactivé");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Bons de livraisons désactivé !");
                logFileWriter_general.WriteLine("");
            }
            logFileWriter_general.Flush();

            //Export des factures
            if (((export.exportFactures == "True") ? true : false))
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Export Factures");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Factures activé !");
                logFileWriter_general.WriteLine("");

                Classes.ExportFactures a = new Classes.ExportFactures(path.path);
                recapLinesList_new___ = a.ExportFacture(recapLinesList_new);
                recapLinesList_new.AddRange(recapLinesList_new___);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Option => Export Factures est désactivé");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export des Factures désactivé !");
                logFileWriter_general.WriteLine("");
            }
            logFileWriter_general.Flush();

            //Export des stocks
            if (((export.exportStock == "True") ? true : false))
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Export Stock");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export du Stock activé !");
                logFileWriter_general.WriteLine("");

                Classes.ExportStocks s = new Classes.ExportStocks(path.path);
                recapLinesList_new___ = s.ExportStock(recapLinesList_new);
                recapLinesList_new.AddRange(recapLinesList_new___);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Option => Export du stock est désactivé");
                logFileWriter_general.WriteLine(DateTime.Now + " : Export du Stock désactivé !");
                logFileWriter_general.WriteLine("");
            }
            logFileWriter_general.Flush();

            Console.WriteLine(DateTime.Now + " : Export Terminé !");
            logFileWriter_general.WriteLine(DateTime.Now + " : Export Terminé !");
            logFileWriter_general.WriteLine("");

            // create Mail_EXP.ml file for 
            if (recapLinesList_new.Count != 0)
            {
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + "################################ Alert Mail Export ###############################");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + "Il y a " + recapLinesList_new.Count + " erreurs trouvé pendant l'export des documents.");
                CustomMailRecap recap_exp = new CustomMailRecap();
                List<string> attchmentsList = new List<string>();
                ConfigurationDNS dns = new ConfigurationDNS();
                dns.Load();

                recap_exp.MailType = "Mail_EXP";
                recap_exp.Client = dns.Prefix;
                recap_exp.Subject = "Erreur d'export des documents";
                recap_exp.DateTimeCreated = string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now);
                recap_exp.DateTimeModified = "";

                recap_exp.Lines = new List<CustomMailRecapLines>();
                for (int i = 0; i < recapLinesList_new.Count; i++)
                {
                    if (attchmentsList.Contains(recapLinesList_new[i].FilePath))
                    {
                        attchmentsList.Add(recapLinesList_new[i].FilePath);
                    }
                    recapLinesList_new[i].Increment += 1;
                    recap_exp.Lines.Add(recapLinesList_new[i]);
                }
                recap_exp.Attachments = attchmentsList;
                recap_exp.saveInfo(recap_exp, "Mail_EXP.ml");
            }
            recapLinesList_new.Clear();

            logFileWriter_general.Flush();

            try 
            {
                ConfSendMail cMail = getInfoMail(logFileWriter_general);
                if (cMail != null && cMail.active)
                {
                    Process emailExe = Process.Start(locationPath + @"\AlertMail.exe", "EndSoftwareExe");
                    emailExe.WaitForExit();

                    emailExe = Process.Start(locationPath + @"\AlertMail.exe", "CheckErrorFiles");
                    emailExe.WaitForExit();
                }
                Console.WriteLine(DateTime.Now + " : Mails Done");
            }
            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine(ex.Message);
                logFileWriter_general.WriteLine("*********** Erreur **********");
                logFileWriter_general.WriteLine("Erreur pendant l'execution d'AlertMail : \n" + ex.Message);
                logFileWriter_general.WriteLine("");
                logFileWriter_general.Flush();
            }
            logFileWriter_general.Flush();

            try
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Cleaning Files");
                logFileWriter_general.Flush();

                cleanFiles(logFileWriter_general, new string[12, 2] {
                    { "general_logs", logDirectoryName_general}, //log files
                    { "import_logs", logDirectoryName_import }, //log files
                    { "export_Logs", new ExportFactures(null).logDirectoryName_export }, //log files
                    { "export_Logs", new ExportBonLivraison(null).logDirectoryName_export }, //log files
                    { "export_Logs", new ExportCommandes(null).logDirectoryName_export }, //log files
                    { "export_Logs", new ExportStocks(null).logDirectoryName_export }, //log files
                    { "import_files_success", Directory.GetCurrentDirectory() + @"\Success File" }, //fichier import success
                    { "import_files_error", Directory.GetCurrentDirectory() + @"\Error File" }, //fichier import erreur
                    { "export_files_BC", path.path }, //backup export files
                    { "export_files_BL", path.path }, //backup export files
                    { "export_files_FA", path.path }, //backup export files
                    { "export_files_ME_MS", path.path } //backup export files
                });
            }catch(Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("###### cleanFiles ######");
                Console.WriteLine(ex.Message);
                logFileWriter_general.WriteLine("*********** Erreur **********");
                logFileWriter_general.WriteLine("" + ex.Message);
                logFileWriter_general.WriteLine("" + ex.StackTrace);
                logFileWriter_general.Flush();
            }

            //Console.ReadLine();

            Console.WriteLine("");
            for (int z = 5; z > 0; z--)
            {
                Console.WriteLine(DateTime.Now + " Closing in " + z + " seconds....");
                System.Threading.Thread.Sleep( (z * 1000) );
            }
            logFileWriter_general.Flush();

            Dlls.InitConfig x = new Dlls.InitConfig();
            x.resetWindowDisplay(logFileWriter_general);

            logFileWriter_general.Flush();
            logFileWriter_general.Close();

            //Console.ReadLine();
        }

        public static void cleanFiles(StreamWriter writer, string[,] directoriesList)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + @"\SettingBackup.xml"))
            {
                ConfigurationBackup configBackup = new ConfigurationBackup();
                configBackup.Load();
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres de nettoyage trouvés et chargés.");

                if (configBackup.activate)
                {
                    Console.WriteLine("Cleaning settings are activated !");
                    writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres de nettoyage activé.");
                    writer.WriteLine(DateTime.Now + " : cleanFiles() | " + directoriesList.GetLength(0) + " dossiers à nettoyer.");
                    writer.Flush();

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        if (configBackup.general_Log != 0 && directoriesList[x, 0] == "general_logs")
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Nettoyage des logs généraux ...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : cleanFiles() | Nettoyage des logs généraux ...");

                            string backUpFolderPath = directoriesList[x,1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.general_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers log après " + ago);
                                Console.WriteLine("");

                                writer.WriteLine(DateTime.Now + " : cleanFiles() | Chemin des logs généraux => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.txt").Length;

                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.txt")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                Console.WriteLine("");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_Log != 0 && directoriesList[x, 0] == "import_logs")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Import logs...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " : cleanFiles() | Nettoyage des logs import ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " : cleanFiles() | Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " : cleanFiles() | Chemin des logs import => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.txt").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.txt")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_Log != 0 && directoriesList[x, 0] == "export_Logs")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export logs...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des logs d'export ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);
                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des logs d'export => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.txt").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.txt")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_files_success != 0 && directoriesList[x, 0] == "import_files_success")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import réussi...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers d'import réussi ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_success);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago + "\n");
                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers d'import => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers d'import après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_files_error != 0 && directoriesList[x, 0] == "import_files_error")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import en erreur...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers d'import en erreur...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_error);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers d'import en erreur => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers d'import en erreur après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_BC != 0 && directoriesList[x, 0] == "export_files_BC")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BC...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers EDI BC...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_BC_type == "Export_Veolog")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Veolog_Commande";
                            }
                            else if (configBackup.export_files_BC_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_Commande";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BC);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers EDI BC => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers EDI BC après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_BL != 0 && directoriesList[x, 0] == "export_files_BL")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BL...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers EDI BL...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_BL_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_BonLivraison";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BL);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers EDI BL => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers EDI BL après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_FA != 0 && directoriesList[x, 0] == "export_files_FA")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files FA...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers EDI FA...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_FA_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_Facture";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_FA);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers EDI FA => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers EDI FA après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < 2 weeks ago: " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_ME_MS != 0 && directoriesList[x, 0] == "export_files_ME_MS")
                        {
                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files ME/MS...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " | cleanFiles() : Nettoyage des fichiers EDI ME/MS...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_ME_MS_type == "Veolog_Stock")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Veolog_Stock";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_ME_MS);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Chemin des fichiers EDI ME/MS => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers EDI ME/MS après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.csv").Length;
                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.csv")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    //writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File: " + filename.Name + "\nCreation Date: " + fileDateTime + "\nModify Date: " + fileDateTimeModif);
                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    else
                                    {
                                        //file was modified
                                        if (fileDateTimeModif < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                                            //writer.WriteLine("");
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                Console.WriteLine("");
                                writer.WriteLine(DateTime.Now + " : cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Les paramètres de nettoyage sont désactivés !");
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " : cleanFiles() | Les paramètres de nettoyage sont désactivés !");
                }
            }
            else
            {
                Console.WriteLine("Les paramètres de nettoyage sont manquants !!!");
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Les paramètres de nettoyage sont manquants !!!");
            }
            writer.WriteLine("");
        }


        public static int Calcule_conditionnement(decimal quantite, string quantite_conditionnement, StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " | Calcule_conditionnement() : Qte '"+quantite+"', Qte conditionnement '"+ quantite_conditionnement + "'");
            try
            {
                int qc = int.Parse(quantite_conditionnement);
                int q = (int)quantite;

                int valeur = q / qc;
                int reste = q % qc;
                writer.WriteLine(DateTime.Now + " | Calcule_conditionnement() : calcule... Valeur = "+valeur+", Reste = "+reste);

                if (reste == 0)
                {
                    writer.WriteLine(DateTime.Now + " | Calcule_conditionnement() : Reste == 0, alors return Valeur : " + valeur);
                    return valeur;
                }
                else 
                {
                    writer.WriteLine(DateTime.Now + " | Calcule_conditionnement() : Reste != 0, alors return Valeur : " + valeur);
                    return valeur + 1;
                }
            }
            catch (Exception e)
            {
                writer.WriteLine(DateTime.Now + " | Calcule_conditionnement() : Erreur[46] - " + "Erreur Calcule de conditionnement :" + e.Message);
                return 0;
            }
        }

        // Get the current time in milliseconds
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static bool deleteLineAndHeaderOfDocument(bool SqlConnexion, string reference, OdbcConnection connexion, StreamWriter writer)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : SQL ===> " + QueryHelper.deleteCommande(SqlConnexion, reference));
                OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(SqlConnexion, reference), connexion);
                command.ExecuteReader();
                return true;
            }
            catch (Exception ex)
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : ******************** OdbcException Delete Document ********************");
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : Message :" + ex.Message);
                writer.WriteLine(DateTime.Now + " | deleteLineAndHeaderOfDocument() : StackTrace :" + ex.StackTrace);
                writer.WriteLine("");
                return false;
            }
        }
    }
}
