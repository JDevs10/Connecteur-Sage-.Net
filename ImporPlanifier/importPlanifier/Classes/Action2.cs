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

namespace importPlanifier.Classes
{
    class Action2
    {


        //private static string filename = "";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string dir;
        private static int nbr = 0;
        //private static StreamWriter LogFile;
        private static string cheminLogFile;
        private static List<string> MessageErreur;

        /* JL LOG */
        private string logDirectoryName_general = Directory.GetCurrentDirectory() + @"\" + "LOG";
        private string logDirectoryName_import = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Import";
        private string directoryName_SuccessFile = Directory.GetCurrentDirectory() + @"\" + "Success File";
        private string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
        private StreamWriter logFileWriter_general = null;
        private StreamWriter logFileWriter_import = null;


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

            DirectoryInfo fileListing = new DirectoryInfo(dir);
            string infoPlan = InfoTachePlanifier();
            if (infoPlan == null)
            {
                Console.WriteLine(DateTime.Now + " : Aucune importation planifiée trouvé");
                Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }

            //Console.WriteLine("Dossier : " + fileListing);
            //Console.WriteLine("");
            //Console.WriteLine(DateTime.Now + " : Scan du dossier ...");

             //Check if the Log directory exists
            if (!Directory.Exists(logDirectoryName_general))
            {
                //Create log directory
                Directory.CreateDirectory(logDirectoryName_general);
            }
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
            var logFileName_general = logDirectoryName_general + @"\" + string.Format("LOG_General_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_general = File.Create(logFileName_general);
            var logFileName_import = logDirectoryName_import + @"\" + string.Format("LOG_Import_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_import = File.Create(logFileName_import);

            //Write in the log file 
            logFileWriter_general = new StreamWriter(logFile_general);
            logFileWriter_general.WriteLine("#####################################################################################");
            logFileWriter_general.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_general.WriteLine("#####################################################################################");
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine(DateTime.Now + " : "+infoPlan);
            logFileWriter_general.WriteLine(DateTime.Now + " : Dossier : " + fileListing);
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine(DateTime.Now + " : Scan du dossier ...");
            logFileWriter_general.WriteLine("");

            //Write in the log file 
            /*
            logFileWriter_import = new StreamWriter(logFile_import);
            logFileWriter_import.WriteLine("#####################################################################################");
            logFileWriter_import.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_import.WriteLine("#####################################################################################");
            logFileWriter_import.WriteLine("");
            */

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
                for (int index = 0; index < fileListing.GetFiles("*.csv").Length; index++)
                {
                    Console.WriteLine(DateTime.Now + " : Fichier trouve ===> " + fileListing.GetFiles("*.csv")[index].Name);
                    FileInfo filename = fileListing.GetFiles("*.csv")[index];

                    try
                    {
                        nbr++;
                        FileExiste = true;
                        Console.WriteLine(DateTime.Now + " : un fichier \".csv\" trouvé :");
                        Console.WriteLine(DateTime.Now + " : -----> " + nbr + " - " + filename);
                        Console.WriteLine(DateTime.Now + " : Scan fichier...");

                        logFileWriter_general.WriteLine(DateTime.Now + " : Numbre de fichier \".csv\" trouvé : " + fileListing.GetFiles("*.csv").Length);

                        logFileWriter_import.WriteLine(DateTime.Now + " : -----> Fichier " + index + " : " + filename.Name);
                        logFileWriter_import.WriteLine(DateTime.Now + " : Scan fichier...");


                        long pos = 1;
                        string[] lines = System.IO.File.ReadAllLines(fileListing + @"\" + filename, Encoding.Default);

                        if (lines[0].Split(';')[0] == "ORDERS" && lines[0].Split(';').Length == 11)
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Orders Trouvé");
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
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : orderId erreur");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }
                            
                            if (order.Id == null)
                            {
                                Console.WriteLine("Erreur [10] : numéro de piece non valide");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : orderId est null");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Numéro de commande doit être < 10");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }

                            if (order.NumCommande == "")
                            {
                                Console.WriteLine("Le champ numéro de commande est vide.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ numéro de commande est vide.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }

                            if (!IsNumeric(order.NumCommande))
                            {
                                Console.WriteLine("Le champ numéro de commande est invalide.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ numéro de commande est invalide.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }

                            string existe = existeCommande(order.NumCommande, logFileWriter_import);

                            if (existe != null && existe != "erreur")
                            {
                                Console.WriteLine("La commande N° " + order.NumCommande + " existe deja dans la base.\nN° de pièce : " + existe + "");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La commande N° " + order.NumCommande + " existe deja dans la base.\nN° de pièce : " + existe + ".");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }

                            if (existe == "erreur")
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : N° de pièce : '" + existe + "' trouvée dans la Base de Données");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }


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
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }

                            Client client = getClient(order.codeClient, 1, logFileWriter_import);
                            if (client == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Client trouvé est null, verifier le code client.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }


                            Client client2 = getClient(order.codeAcheteur, 2, logFileWriter_import);
                            if (client2 == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Acheteur trouvé est null, verifier le code Acheteur.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }


                            if (existeFourniseur(order.codeFournisseur, logFileWriter_import) == null)
                            {
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Fournisseur trouvé est null, verifier le code Fournisseur.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                //logFileWriter_general.Close();


                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante :\nNom.Adresse.CodePostal.Ville.Pays.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : deviseCommande == erreur");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
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

                                        decimal total = 0m;
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
                                                        line.article = getArticle(tab[2], logFileWriter_import);

                                                        if (line.article == null)
                                                        {
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Erreur *********************");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                            //logFileWriter_import.Close();
                                                            //return;

                                                            tabCommandeError.Add(filename.ToString());
                                                            goto goErrorLoop;
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
                                                        line.MontantLigne = tab[11];
                                                        line.DateLivraison = "'{d " + ConvertDate(tab[21]) + "}'";
                                                        if (line.DateLivraison.Length == 6)
                                                        {
                                                            line.DateLivraison = "Null";
                                                        }


                                                        //if (line.article.AR_UnitePoids == "2")
                                                        //{
                                                        //    line.article.AR_POIDSBRUT = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                        //    line.article.AR_POIDSNET = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                        //}

                                                        //if(line.article.AR_UnitePoids == "3")
                                                        //{
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
                                                            //logFileWriter_general.Close();

                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Erreur de conversion de poids.");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                            //logFileWriter_import.Close();
                                                            //return;
                                                            tabCommandeError.Add(filename.ToString());
                                                            goto goErrorLoop;
                                                        }
                                                        //}
                                                        //}
                                                        line.codeAcheteur = tab[4].Replace(" ", "");
                                                        line.codeFournis = tab[5].Replace(" ", "");
                                                        //line.codeFournis = line.codeFournis.Replace(Environment.NewLine, String.Empty);
                                                        line.descriptionArticle = tab[8].Replace("'", "''");
                                                        if (string.IsNullOrEmpty(line.descriptionArticle))
                                                        {
                                                            line.descriptionArticle = line.article.AR_DESIGN;
                                                        }
                                                        total = total + Decimal.Parse(tab[11].Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);


                                                        decimal prix = Decimal.Parse(line.PrixNetHT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                        decimal prixSage = Decimal.Parse(line.article.AR_PRIXVEN.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);


                                                        if (prix != prixSage)
                                                        {
                                                            Console.WriteLine("Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + "\nIl est différent du prix envoyer par le client : " + prix + ".");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                            //logFileWriter_general.Close();

                                                            logFileWriter_import.WriteLine("");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + "\nIl est différent du prix envoyer par le client : " + prix + ".");
                                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                            //logFileWriter_import.Close();
                                                            //return;
                                                            tabCommandeError.Add(filename.ToString());
                                                            goto goErrorLoop;
                                                        }

                                                        order.Lines.Add(line);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Erreur dans la ligne " + pos + " du fichier.", "Erreur de lecture !!");

                                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                        logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                        //logFileWriter_general.Close();

                                                        logFileWriter_import.WriteLine("");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur dans la ligne " + pos + " du fichier " + filename + ".", "Erreur de lecture.");
                                                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                        //logFileWriter_import.Close();
                                                        //return;
                                                        tabCommandeError.Add(filename.ToString());
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
                                            if (order.Lines[i].DateLivraison.Length == 16)
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
                                                //logFileWriter_general.Close();

                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Stock ID est null ou vide.");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                //logFileWriter_import.Close();
                                                //return;
                                                tabCommandeError.Add(filename.ToString());
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

                                                //if (order.Reference.Length > 13)
                                                //               {
                                                //                   order.Reference = order.Reference.Substring(0,12) + "../AP";
                                                //               }
                                                //               else
                                                //               {
                                                //                   order.Reference = order.Reference+"/AP";
                                                //               }
                                            }

                                            //order.FNT_MONTANTTOTALTAXES = (total * 19.6m) / 100.0m ;
                                            //return;

                                            if (order.Lines.Count == 0)
                                            {
                                                Console.WriteLine("Aucun ligne de commande enregistré.");

                                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                //logFileWriter_general.Close();

                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Aucun ligne de commande enregistré. ligne = " + order.Lines.Count());
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                //logFileWriter_import.Close();
                                                //return;
                                                tabCommandeError.Add(filename.ToString());
                                                goto goErrorLoop;
                                            }
                                            MessageErreur = new List<string>();

                                            //###################################################################
                                            //                    TEST ADRESSE DE LIVRAISION
                                            //###################################################################

                                            //if (listAdress.Count != 0)
                                            //{
                                            //    order.adresseLivraison = listAdress[0].Li_no;
                                            //    if (string.IsNullOrEmpty(order.adresseLivraison))
                                            //    {
                                            //        MessageBox.Show("Adresse de livraison invalide");
                                            //        return;
                                            //    }
                                            //}

                                            //if (listAdress.Count == 0)
                                            //{
                                            //    string intitule = client.CT_Num + " " + order.ville;

                                            //    List<string> listIntitules = TestIntituleLivraison(intitule);

                                            //    //if (listIntitules == null && listIntitules.Count == 0)
                                            //    //{
                                            //    //    tabCommandeError.Add(filename.ToString());
                                            //    //    goto goOut;
                                            //    //}

                                            //    int inc = 1;

                                            //incIntitule:

                                            //    if (listIntitules.Count != 0)
                                            //    {
                                            //        for (int i = 0; i < listIntitules.Count; i++)
                                            //        {
                                            //            if (intitule == listIntitules[i])
                                            //            {
                                            //                if (inc != 1)
                                            //                {
                                            //                    intitule = intitule.Substring(0, (intitule.Length - 4));
                                            //                }
                                            //                intitule = intitule + " N°" + inc;
                                            //                inc++;
                                            //                goto incIntitule;
                                            //            }
                                            //        }
                                            //    }
                                            //    if (insert_adresse_livraison(client.CT_Num, new AdresseLivraison("", order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays, order.conditionLivraison, intitule)))
                                            //    {
                                            //        order.adresseLivraison = get_Last_insert_livraison(client.CT_Num);
                                            //        if (string.IsNullOrEmpty(order.adresseLivraison))
                                            //        {
                                            //            return;
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        return;
                                            //    }
                                            //}

                                            //###################################################################
                                            //                   FIN DE TEST ADRESSE DE LIVRAISION
                                            //###################################################################


                                            //if (insertAdressLivr)
                                            //{
                                            //    order.adresseLivraison = get_Last_insert_livraison();
                                            //    if (string.IsNullOrEmpty(order.adresseLivraison))
                                            //    {
                                            //        return;
                                            //    }
                                            //}




                                            //if (!insertAdressLivr && listAdress.Count == 0)
                                            //{
                                            //order.adresseLivraison = getNumLivraison(client.CT_Num);
                                            //if (string.IsNullOrEmpty(order.adresseLivraison))
                                            //{
                                            //    return;
                                            //}
                                            //}

                                            order.adresseLivraison = getNumLivraison(client.CT_Num, logFileWriter_import);
                                            if (string.IsNullOrEmpty(order.adresseLivraison))
                                            {
                                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                                //logFileWriter_general.Close();

                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Adresse de livraison est null ou vide");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                                logFileWriter_import.WriteLine("");
                                                //logFileWriter_import.Close();
                                                //return;
                                                tabCommandeError.Add(filename.ToString());
                                                goto goErrorLoop;
                                            }

                                            if (insertCommande(client, order))
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

                                                    if (insertCommandeLine(client, order, order.Lines[i]))
                                                    {
                                                        nbr_++;
                                                    }
                                                }
                                                string mot = "";
                                                for (int i = 0; i < MessageErreur.Count; i++)
                                                {
                                                    mot = mot + MessageErreur[i] + "\n";
                                                }

                                                if (nbr_ == 0)
                                                {
                                                    deleteCommande(order.NumCommande);
                                                }

                                                //// Creer dossier sortie "LOG Directory" --------------------------
                                                //var dirName = string.Format("LogSage(manuelle) {0:dd-MM-yyyy HH.mm.ss}", DateTime.Now);
                                                //string outputFile = System.IO.Path.GetDirectoryName(filename) + @"\" + dirName;
                                                //System.IO.Directory.CreateDirectory(outputFile);
                                                ////deplacer les fichiers csv
                                                //System.IO.File.Move(filename, outputFile + @"\" + System.IO.Path.GetFileName(filename));


                                                Console.WriteLine("" + nbr_ + "/" + order.Lines.Count + " ligne(s) enregistrée(s).\n" + mot);

                                                logFileWriter_import.WriteLine(DateTime.Now + " : " + nbr_ + "/" + order.Lines.Count + " ligne(s) enregistrée(s).\n" + mot);
                                                logFileWriter_import.WriteLine("");
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("Il faut mentionner le code client.");

                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                            //logFileWriter_general.Close();

                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Il faut mentionner le code client.");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                            //logFileWriter_import.Close();
                                            tabCommandeError.Add(filename.ToString());
                                            goto goErrorLoop;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erreur dans la troisième ligne du fichier.");

                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                        //logFileWriter_general.Close();

                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Erreur dans la troisième ligne du fichier.");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                        //logFileWriter_import.Close();
                                        //return;
                                        tabCommandeError.Add(filename.ToString());
                                        goto goErrorLoop;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Date de la commande est incorrecte");

                                    logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                    logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                    //logFileWriter_general.Close();

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Date de la commande est incorrecte.");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                    //logFileWriter_import.Close();
                                    //return;
                                    tabCommandeError.Add(filename.ToString());
                                    goto goErrorLoop;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Erreur dans la deuxième ligne du fichier.");

                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Import annulée");
                                logFileWriter_general.WriteLine(DateTime.Now + " : A voir dans le fichier : " + logFileName_import);
                                //logFileWriter_general.Close();

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : rreur dans la deuxième ligne du fichier.");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                //logFileWriter_import.Close();
                                //return;
                                tabCommandeError.Add(filename.ToString());
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
                                string reference_me_doc = lastNumberReference("ME", logFileWriter_import);    //"ME00004";//get last reference number for entry STOCK document MEXXXXX and increment it
                                string reference_ms_doc = lastNumberReference("MS", logFileWriter_import);    //"MS00007";//get last reference number for removal STOCK document MSXXXXX and increment it

                                int i = 0;
                                string totallines = "";
                                List<Stock> s = new List<Stock>();

                                /*
                                List<String> product_added = new List<string>();    //Temporaly list while in execution

                                string[,] product_added_2d = new string[(lines.Count() - 1), 2];
                                int executionArrayIndex = 0;

                                for (int x = 0; x < (lines.Count()-1); x++)
                                {
                                    string[] tab = lines[x].Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (!product_added_2d[x, 0].Equals(tab[2])) // if product is not inside the array
                                    {
                                        //then add the product inside the array
                                        product_added_2d[x, 0] = tab[2]; //0 : "BAAR01"
                                        product_added_2d[x, 1] = "1";    //1 : 1   
                                    }
                                    else
                                    {
                                        //do not add the product but ! increment its index:2  where the item is inserted
                                        for(int y =0; y < (lines.Count()-1); y++)
                                        {
                                            string[] tabY = lines[y].Split(';'); //split the line by its delimiter ; - creating an array tab
                                            int increment = 0;

                                            if(tab[2].Equals(tabY[2]))
                                            {
                                                increment = Convert.ToInt16(product_added_2d[y, 1]);
                                                increment++;
                                                product_added_2d[x, 1] = increment + "";
                                                break;
                                            }
                                        }
                                    }
                                }

                                logFileWriter_general.WriteLine("");
                                logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                logFileWriter_general.WriteLine(DateTime.Now + " : La reference d'article " + tab[2] + " est trouve plusieurs fois dans le fichier");
                                logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                logFileWriter_general.WriteLine("");

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                logFileWriter_import.WriteLine(DateTime.Now + " : La reference d'article " + tab[2] + " est trouve plusieurs fois dans le fichier");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                logFileWriter_import.WriteLine("");

                                    foreach (string ligneDuFichier in lines) //read lines by line
                                    {
                                        //MessageBox.Show("READING IMPORTED FILE");



                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Lecture du fichier d'importation.");

                                        string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                        if (product_added.Contains(tab[2])) //verify if product in the list already exists from the .csv file; T: do nothing , F: continue adding
                                        {
                                            //increment counter of the corresponding product

                                            //logFileWriter_import.WriteLine(""); 
                                            //logFileWriter_import.WriteLine(DateTime.Now + " : Lecture du fichier d'importation.");
                                            logFileWriter_general.WriteLine("");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : La reference d'article " + tab[2] + " est trouve plusieurs fois dans le fichier");
                                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                            logFileWriter_general.WriteLine("");

                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Warning *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : La reference d'article " + tab[2] + " est trouve plusieurs fois dans le fichier");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                                            logFileWriter_import.WriteLine("");
                                        }
                                        else
                                        {
                                            product_added.Add(tab[2]); //add product in a temporaly list

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
                                        } //product_added.Contains(tab[2]) END.
                                        executionArrayIndex++;
                                    } // End Foreach
                                 * 
                                 * */

                                // *once list is filled with values, start executing queries for each line - one by one.*


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

                                    //deplacer les fichiers csv
                                    File.Move(filename.Name, directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + filename.Name + "' est déplacé dans ===> " + directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));

                                    logFileWriter_general.Close();

                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** erreur *********************");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le pied du page n'est pas en forme correcte.\r\nLa valeur 'nombre d'articles' n'est pas égale à nombre des lignes totale indiqué dans le pied du page.");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");
                                    //logFileWriter_import.Close();

                                    tabCommandeError.Add(filename.ToString());
                                    goto goErrorLoop;
                                }
                                else
                                {
                                    //MessageBox.Show("INSERTSTOCK BEING CALLED");
                                    //insert or update the database with the values obtained from the document
                                    if (insertStock(s, reference_ms_doc, reference_me_doc, logFileWriter_import) != null)
                                    {
                                        Console.WriteLine("Le stock est importe avec succès");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information Fatale *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Le stock est importe avec succès");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Import succès");
                                        logFileWriter_general.WriteLine("");
                                        logFileWriter_general.WriteLine("");

                                        //deplacer les fichiers csv
                                        //File.Move(filename.Name, directoryName_SuccessFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));
                                        //logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + filename.Name + "' est déplacé dans ===> " + directoryName_SuccessFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));

                                        logFileWriter_general.Close();
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

                                        //deplacer les fichiers csv
                                        //File.Move(filename.Name, directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));
                                        //logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + filename.Name + "' est déplacé dans ===> " + directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));

                                        //logFileWriter_general.Close();
                                        tabCommandeError.Add(filename.ToString());
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

                                //deplacer les fichiers cs
                                /*
                                File.Move(filename.Name, directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + filename.Name + "' est déplacé dans ===> " + directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));

                                logFileWriter_import.Close();
                                */
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;

                            }
                        }
                        else if (lines[0].Split(';')[0] == "L") //Import Veolog Stock doc
                        {
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
                                if (lines[lineIndex].Split(';').Length == 8)
                                {
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Verification Des Lignes Du Documment *********************");

                                    //  info_stock[0] == Type Ligne
                                    //  info_stock[1] == Code Article (reference)
                                    //  info_stock[2] == EAN (barcode)
                                    //  info_stock[3] == Stock

                                    string[] info_stock = lines[lineIndex].Split(';');

                                    Console.WriteLine("0: " + info_stock[0] + " | 1: " + info_stock[1] + " | 2: " + info_stock[2] + " | 3: " + info_stock[3] + " | 4: " + info_stock[4] + " | 5: " + info_stock[5] + " | 6: " + info_stock[6] + " | 7: " + info_stock[7]);

                                    if (info_stock[0] == "L")
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Type ligne => VALIDE ");


                                        try
                                        {
                                            if (info_stock[1] != "")
                                            {
                                                logFileWriter_import.WriteLine(DateTime.Now + " :  Code acticle => TROUVE ");
                                                valid_info_stock_line[lineIndex, 0] = info_stock[1];
                                            }
                                            else
                                            {
                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Code acticle => PAS TROUVE ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                                goto skipLine;
                                            }

                                            if (info_stock[2] != "")
                                            {
                                                logFileWriter_import.WriteLine(DateTime.Now + " : EAN (Code Barre) => TROUVE ");
                                                valid_info_stock_line[lineIndex, 1] = info_stock[2];
                                            }
                                            else
                                            {
                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | EAN (Code Barre) ===> PAS TROUVE ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                                goto skipLine;
                                            }

                                            if (info_stock[3] != "")
                                            {
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Quantités disponibles (Stock actuel) => TROUVE ");
                                                valid_info_stock_line[lineIndex, 2] = info_stock[3];
                                            }
                                            else
                                            {
                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Quantités disponibles (Stock actuel) => PAS TROUVE ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                                goto skipLine;
                                            }

                                            if (info_stock[4] != "")
                                            {
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Numéro de lot  => TROUVE ");
                                                valid_info_stock_line[lineIndex, 3] = info_stock[4];
                                            }
                                            else
                                            {
                                                logFileWriter_import.WriteLine("");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Numéro de lot  => PAS TROUVE ");
                                                logFileWriter_import.WriteLine(DateTime.Now + " : Le champ est vide!!! ");
                                                valid_info_stock_line[lineIndex, 3] = info_stock[4];
                                            }

                                            lineCount++;
                                        }
                                        catch (Exception ex)
                                        {
                                            logFileWriter_import.WriteLine("");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Dans le Documment *********************");
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1));
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Message |\n" + ex.Message);
                                            logFileWriter_import.WriteLine(DateTime.Now + " : Stack |\n" + ex.StackTrace);

                                            /*
                                            logFileWriter_general.Flush();
                                            logFileWriter_import.Flush();
                                            logFileWriter_import.Close();
                                            logFileWriter_general.Close();
                                            */
                                        }
                                    }
                                    else
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** ERREUR Du Documment *********************");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Ligne " + (lineIndex + 1) + " | Type ligne ===> PAS RECONNU / PAS TROUVE");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le champ n'est pas correct ou vide!!! ");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Cet article ne sera pas mis à jour dans la base de données");
                                    }

                                skipLine:;
                                }
                                else
                                {
                                    if (lines[lineIndex].Split(';')[0] == "F" && lines[lineIndex].Split(';').Length == 2)
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Fin de la lecture du documment Veolog Stock.");
                                        totallines = lines[lineIndex].Split(';')[1];
                                        stockVeologCheck = true;
                                        break;
                                    }
                                    logFileWriter_import.WriteLine("");
                                    logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                    tabCommandeError.Add(filename.ToString());
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
                                    logFileWriter_general.Close();
                                }
                                else
                                {
                                    if (insertStockVeolog(s, logFileWriter_import) != null)
                                    {
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe avec succès");

                                        string file_name_str = fileListing.GetFiles("*.csv")[index].Name;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + GetTimestamp(new DateTime()) + "_" + filename.Name;
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nom du fichier ===> " + filename.FullName);
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nouveau fichier ===> " + newFileLocation);

                                        //deplacer les fichiers csv
                                        File.Move(filename.FullName, newFileLocation);
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + file_name_str + "' est déplacé dans ===> " + newFileLocation);

                                        //logFileWriter_general.Close();
                                    }
                                    else
                                    {
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le stock");

                                        //deplacer les fichiers csv
                                        File.Move(filename.Name, directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));
                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + filename + "' est déplacé dans ===> " + directoryName_ErrorFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name));

                                        logFileWriter_general.Flush();
                                        logFileWriter_general.Close();
                                    }
                                }
                            }
                            else
                            {
                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Fin de la lecture du documment Veolog Stock, non valide.");
                            }
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
                                    tabCommandeError.Add(filename.ToString());
                                    goto goErrorLoop;
                                }
                                else
                                {
                                    //insertDesadv(d, dl);//insert or update the database with the values obtained from the document
                                }

                            }
                            else
                            {
                                Console.WriteLine("Le fichier n'est pas en bonne forme, merci de regarder son contenu."); //show error : content issue

                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }
                        }
                        else if (lines[0].Split(';')[0] == "E") //Import Veolog DESADV doc
                        {
                            logFileWriter_general.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Fichier Veolog DESADV Trouvé");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Plus information sur l'import se trouve dans le log : " + logFileName_import);
                            logFileWriter_general.WriteLine("");

                            logFileWriter_import.WriteLine(DateTime.Now + " : Import Veolog DESADV Inventaire.");

                            if (lines[0].Split(';').Length == 6)
                            {
                                string reference_DESADV_doc = lastNumberReference("BL", logFileWriter_import);    //get last reference number for desadv STOCK document MEXXXXX and increment it

                                int i = 0;
                                string totallines = "";
                                Veolog_DESADV dh = new Veolog_DESADV();
                                //Veolog_DESADV_Colis dc = new Veolog_DESADV_Colis();
                                Veolog_DESADV_Lines dll = new Veolog_DESADV_Lines();

                                List<Veolog_DESADV_Lines> dl = new List<Veolog_DESADV_Lines>(); //creating new object type desadvline and storing item values

                                foreach (string ligneDuFichier in lines) //read lines by line
                                {
                                    string[] tab = ligneDuFichier.Split(';'); //split the line by its delimiter ; - creating an array tab

                                    if (tab[0] == "E") //checking if its header of file for control
                                    {
                                        Veolog_DESADV desadv_info = new Veolog_DESADV();
                                        desadv_info.Commande_Donneur_Ordre = tab[1];
                                        desadv_info.Commande_Client_Livre = tab[2];
                                        desadv_info.Date_De_Expedition = tab[3];
                                        desadv_info.Heure_De_Expedition = tab[4];
                                        desadv_info.Etat = tab[5];

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
                                        Veolog_DESADV_Lines desadvLine_info = new Veolog_DESADV_Lines();

                                        desadvLine_info.Numero_Ligne_Order = tab[1];
                                        desadvLine_info.Code_Article = tab[2];
                                        desadvLine_info.Quantite_Colis = tab[3];
                                        desadvLine_info.Numero_Lot = tab[4];

                                        dl.Add(desadvLine_info);

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
                                    if (insertDesadv_Veolog(reference_DESADV_doc, dh, dl, logFileWriter_import) != null) //insert or update the database with the values obtained from the document
                                    {
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : importe du DESADV avec succès");

                                        //deplacer les fichiers csv
                                        string theFileName = filename.Name;
                                        string newFileLocation = directoryName_SuccessFile + @"\" + GetTimestamp(new DateTime()) + "_" + System.IO.Path.GetFileName(filename.Name);
                                        File.Move(theFileName, newFileLocation);

                                        logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier '" + theFileName + "' est déplacé dans ===> " + newFileLocation);
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_import.WriteLine("");

                                    }
                                    else
                                    {
                                        logFileWriter_import.WriteLine("");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : ********************** Information *********************");
                                        logFileWriter_general.WriteLine(DateTime.Now + " : Nous n'avons pas pu importer le DESADV");
                                        tabCommandeError.Add(filename.ToString());
                                        goto goErrorLoop;
                                    }
                                }
                            }
                            else
                            {
                                logFileWriter_import.WriteLine("");
                                logFileWriter_import.WriteLine(DateTime.Now + " : Le fichier n'est pas en bonne forme, merci de regarder son contenu.");
                                tabCommandeError.Add(filename.ToString());
                                goto goErrorLoop;
                            }
                        }
                        else
                        {

                            //Console.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                            logFileWriter_import.WriteLine("");
                            logFileWriter_general.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                            tabCommandeError.Add(filename.ToString());
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

                        tabCommandeError.Add(filename.ToString());
                    }

                goErrorLoop:;

                    //Deplaçer les fichier dans le dossier : Error File SI IL Y A DES ERREUR .....
                    if (File.Exists(dir + @"\" + filename) && tabCommandeError.Count > 0)
                    {
                        //var errorfilename = string.Format("{0:ddMMyyyy_HHmmss}_" + filename, DateTime.Now);
                        //System.IO.File.Move(dir + @"\" + filename, outputFileError + @"\" + errorfilename);

                        logFileWriter_general.Flush();
                        logFileWriter_import.Flush();
                        logFileWriter_import.WriteLine("");
                        logFileWriter_import.WriteLine(DateTime.Now + " : ********************** Fichier *********************");
                        logFileWriter_import.WriteLine(DateTime.Now + " : Import annulée");

                        //deplacer les fichiers csv
                        string theFileName = filename.Name;
                        string newFileLocation = directoryName_ErrorFile + @"\" + string.Format("{0:dd-MM-yyyy_HH.mm.ss}.log", DateTime.Now) + "_" + System.IO.Path.GetFileName(filename.Name);
                        File.Move(filename.Name, newFileLocation);
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

            /*
            if (SaveSuccess == 0)
            {
                System.IO.Directory.Delete(outputFile);
            }
            */

            Boolean envoiMail = false;

        // Envoi de mail
        if (tabCommandeError.Count != 0)
        {
            ConfSendMail cMail = getInfoMail(logFileWriter_import);
            if (cMail != null)
            {
                if (cMail.active)
                {
                    if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                    {
                        logFileWriter_general.WriteLine(DateTime.Now + " : Send Mail..Erreur Adresse de distinataire");
                    }
                    string commande = "";
                    for (int i = 0; i < tabCommandeError.Count; i++)
                    {
                        commande = commande + (i + 1) + " - " + tabCommandeError[i] + "\n";
                    }
                    Console.WriteLine(DateTime.Now + " : Envoi de mail en cours..");
                    logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
                    logFileWriter_general.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                    logFileWriter_general.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                    logFileWriter_general.Close();

                    //Envoi
                    EnvoiMail(cMail, "Erreur d'import de documents commerciaux", "Bonjour,\n\nL'import d'un ou plusieurs documents commerciaux a echoué :\n" + commande + "\nVeuillez vérifier dans le fichier Log ci-joint, les problèmes qui sont survenus au moment de l'importation.\n\nNB: Les fichiers sont déplacé dans un dossier nommé : \"Error file\".\n\nCordialement,\n\nConnecteur SAGE.", logFileName_import);   //cheminLogFile
                    envoiMail = true;
                }
                else
                {
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
                    logFileWriter_general.WriteLine("");
                    logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
                    logFileWriter_general.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                    logFileWriter_general.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                    logFileWriter_general.Close();
                }
            }
            else
            {
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
                logFileWriter_general.WriteLine("");
                logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
                logFileWriter_general.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                logFileWriter_general.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                logFileWriter_general.Close();
            }
        }

        if (!FileExiste && !envoiMail)
        {
            //Console.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
            logFileWriter_general.WriteLine(DateTime.Now + " : Fin de l'execution");
            logFileWriter_general.WriteLine("");
            logFileWriter_general.WriteLine("Nombre de fichier scanner : " + nbr);
            //var newlog = string.Format("logFile(0) {0:dd-MM-yyyy HH.mm.ss}.log", DateTime.Now);
            //System.IO.File.Move(outputFile + @"\logFile.log", dir + @"\" + newlog);
            //System.IO.Directory.Delete(outputFile);
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
            //Console.WriteLine(DateTime.Now + " : Fin de l'execution");
            //Console.WriteLine("Nombre de fichier scanner : " + nbr);


            //Console.WriteLine("");
            //Console.WriteLine("Cliquez Entrer pour sortir ..");

            //Console.Read();

            logFileWriter_general.Flush();
            logFileWriter_general.Close();
        }

        public static Boolean insertCommande(Client client, Order order)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insertCommande(false, client, order), connection);
                    //MessageBox.Show(command.CommandText);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(" ERREUR[4]" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return false;
                }
            }

        }

        public static Boolean insertCommandeLine(Client client, Order order, OrderLine orderLine)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertLigneCommande(false, client, order, orderLine), connection);
                    //MessageBox.Show(command.CommandText);
                    //Console.Read();
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageErreur.Add("Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + "." + "\n" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return false;
                }
            }

        }

        public static string[,] insertStock(List<Stock> s, string reference_MS_doc, string reference_ME_doc, StreamWriter logFileWriter)
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
                                    list_of_products[counter, 12] = name_article; // DL_Design
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
                                    logFileWriter.Close();
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
                                    logFileWriter.Close();
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
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : La Référence du produit dans le fichier n'existe pas dans la BDD.");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                            logFileWriter.Close();
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
                    connection.Close(); //disconnect from database
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
                                logFileWriter.Close();
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
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ******************** OdbcException ********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                        logFileWriter.Close();
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
                                //logFileWriter.Close();
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
                                        logFileWriter.WriteLine("");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ********************** OdbcException *********************");
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Message :" + ex.Message);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : StackTrace :" + ex.StackTrace);
                                        logFileWriter.WriteLine(DateTime.Now + " | insertStock() : Import annulée");
                                        //logFileWriter.Close();
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
                        return null;
                    }

                    connectionSQL.Close(); //disconnect from database
                    logFileWriter.WriteLine(DateTime.Now + " | insertStock() : ConnexionSQL fermée.");
                }
            }

            logFileWriter.WriteLine("");
            logFileWriter.Close();

            return list_of_products;
        }

        public static string[,] insertStockVeolog(List<Stock> s, StreamWriter logFileWriter)
        {
            string[,] list_of_products = new string[(s.Count-1), 27];
            int positive_item = 0;
            int negative_item = 0;
            DateTime d = DateTime.Now;
            string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            string reference_ME_doc = "";  //Doc ME
            string reference_MS_doc = "";   //Doc MS


            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connexion.Open();
                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Connexion ouverte.");

                    int counter = 0;

                    foreach (Stock line in s)
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Lire la ligne de l'article.");

                        int total_negative = 0;
                        int total_positive = 0;
                        string name_article = "";
                        string DL_PoidsNet = "0";
                        string DL_PoidsBrut = "0";
                        string DL_PrixUnitaire = "0";

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
                                }
                                else // If no rows returned
                                {
                                    //do nothing.
                                }
                            }
                        }

                        if(name_article != "")
                        {
                            bool isSameStock = true;
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Article trouvé.");
                            logFileWriter.WriteLine("");

                            reference_ME_doc = lastNumberReference("ME", logFileWriter);   //Doc ME
                            reference_MS_doc = lastNumberReference("MS", logFileWriter);   //Doc MS

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


                            int current_stock = (total_positive - total_negative); // substract negative stock from the positive one = to obtain the initial current stock.
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Reference: " + line.reference + " total_positive: " + total_positive + " - total_negative: " + total_negative + " = current_stock : " + current_stock + ".");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current stock BDD:" + current_stock + " || current stock Veolog: " + line.stock.Replace(",",".") + " .");

                            //transform line.stock to int format
                            line.stock = line.stock.Split(',')[0]; ;    //line.stock.Replace(",", ".");

                            if (current_stock < Convert.ToInt16(line.stock)) // if current stock in database is inferior from the one received in file : means add stock
                            {
                                //MessageBox.Show("current_stock : " + current_stock + " < " + "Line.stock : " + Convert.ToInt16(line.stock));
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " < " + "current_stock_Veolog : " + Convert.ToInt32(line.stock) + ".");

                                try
                                {
                                    positive_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : negativ_item: " + negative_item + ".");


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
                                    list_of_products[counter, 13] = (Convert.ToInt32(current_stock) - Convert.ToInt32(line.stock)).ToString().Replace(",", ".").Replace("-", ""); //line.stock; // DL_Qte
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
                                    list_of_products[counter, 20] = (Convert.ToInt32(current_stock) - Convert.ToInt32(line.stock)).ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
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
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** Exception Tableau ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Le tableau 'ME' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    logFileWriter.Close();
                                    return null;
                                }

                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Produit '" + name_article + "' est ajouté à la table list_of_products en tant qu'index ME.");
                            }

                            if (current_stock > Convert.ToInt16(line.stock)) // if current stock in database is superior from the one received in file : means remove stock
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " > " + "current_stock_Veolog : " + line.stock + ".");

                                try
                                {
                                    negative_item += 1000; //increment line by 1000 for format 1000,2000,etc
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : negativ_item: " + negative_item + ".");


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
                                    list_of_products[counter, 12] = name_article; // DL_Design
                                    list_of_products[counter, 13] = (Convert.ToInt32(current_stock) - Convert.ToInt32(line.stock)).ToString().Replace(",", ".").Replace("-", "");  //line.stock; // DL_Qte
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
                                    list_of_products[counter, 20] = (Convert.ToInt32(current_stock) - Convert.ToInt32(line.stock)).ToString().Replace(",", ".").Replace("-", ""); // EU_Qte; // EU_Qte
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
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** Exception Tableau ********************");
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Le tableau 'MS' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : StackTrace :" + ex.StackTrace);
                                    logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                                    logFileWriter.Flush();
                                    logFileWriter.Close();
                                    return null;
                                }

                                //MessageBox.Show("Product added into the table list_of_products as MS index ");

                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Produit '" + name_article + "' est ajouté à la table list_of_products en tant qu'index MS.");
                            }

                            if (current_stock == Convert.ToInt16(line.stock))
                            {
                                isSameStock = false;
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** Information ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : current_stock_BDD : " + current_stock + " == current_stock_Veolog : " + line.stock);
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import non effectué");
                                logFileWriter.WriteLine("");
                            }


                            if (positive_item > 0 && isSameStock) //check if any product for 20::addstock exists : this case > 0 ; if 1000+ then OK generate document ME_____.
                            {
                                //generate document ME_____ in database.
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Vérifier si un produit pour 20 = ME");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, curr_date_time));

                                try
                                {
                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocument(true, "20", reference_ME_doc, curr_date, curr_date_seconds, curr_date_time), connexion); //calling the query and parsing the parameters into it
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
                                    logFileWriter.Close();
                                    return null;
                                }
                                string[,] products_ME = new string[(s.Count - 1), 27]; // create array with enough space

                                //insert documentline into the database with articles having 20 as value @index 2
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert documentline into the database with articles having 20 as value @index 2");

                                for (int x = 0; x < list_of_products.GetLength(0); x++)
                                {
                                    //logFileWriter.WriteLine("x: " + x + " || list_of_products.GetLength(0) : " + list_of_products.GetLength(0));
                                    if (list_of_products[x, 1] == "20")
                                    {
                                        for (int y = 0; y < list_of_products.GetLength(1); y++)
                                        {
                                            //logFileWriter.WriteLine("x: " + x + " ; y: " + y + " || list_of_products.GetLength(1) : " + list_of_products.GetLength(1));

                                            products_ME[x, y] = list_of_products[x, y];
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : products_ME[" + x + "," + y + "] = " + products_ME[x, y]);
                                        }

                                        //insert the article to documentline in the database
                                        try
                                        {
                                            logFileWriter.WriteLine("");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert the article " + products_ME[x, 12] + " (Ref:" + products_ME[x, 9] + ") to documentline in the database");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : requette sql ===> " + QueryHelper.insertStockDocumentLine(true, products_ME, x));

                                            OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocumentLine(true, products_ME, x), connexion);
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
                                            logFileWriter.Close();
                                            return null;
                                        }
                                    }
                                }

                            }

                            if (negative_item > 0 && isSameStock) //check if any product for 21::stockremoval exists : this case > 0 ; if 1000+ then OK generate document MS_____.
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Vérifier si un produit pour 21 = MS");
                                logFileWriter.Write(DateTime.Now + " | insertStockVeolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, curr_date_time));

                                //generate document MS_____. in database.
                                try
                                {
                                    OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocument(true, "21", reference_MS_doc, curr_date, curr_date_seconds, curr_date_time), connexion); //calling the query and parsing the parameters into it
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
                                    logFileWriter.Close();
                                    return null;
                                }

                                string[,] products_MS = new string[(s.Count - 1), 27]; // create array with enough space

                                //insert documentline into the database with articles having 20 as value @index 2
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert documentline into the database with articles having 20 as value @index 2");

                                for (int x = 0; x < list_of_products.GetLength(0); x++)
                                {
                                    if (list_of_products[x, 1] == "21")
                                    {
                                        for (int y = 0; y < list_of_products.GetLength(1); y++)
                                        {
                                            products_MS[x, y] = list_of_products[x, y];
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : products_MS[" + x + "," + y + "] = " + products_MS[x, y]);
                                        }

                                        //insert the article to documentline in the database
                                        try
                                        {
                                            logFileWriter.WriteLine("");
                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : insert the article " + products_MS[x, 12] + " (Ref:" + products_MS[x, 9] + ") to documentline in the database");

                                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : requette sql ===> " + QueryHelper.insertStockDocumentLine(true, products_MS, x));

                                            OdbcCommand command = new OdbcCommand(QueryHelper.insertStockDocumentLine(true, products_MS, x), connexion);
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
                                            logFileWriter.Close();
                                            return null;
                                        }
                                    }
                                }
                            }


                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Compteur Produit ===> " + counter);
                            logFileWriter.WriteLine("");
                            counter++; // increment by 1 per product [multi-dimensional array]

                        }
                        else
                        {
                            logFileWriter.WriteLine("");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : ******************** Erreur Référence ********************");
                            logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : La Référence (Ref: " + line.reference + ") du produit dans le fichier n'existe pas dans la BDD.");

                            //logFileWriter.WriteLine(DateTime.Now + " | insertStockVeolog() : Import annulée");
                            //logFileWriter.Close();
                            //return null;
                        }

                    }   // end foreach



                    connexion.Close(); //disconnect from database
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
                    logFileWriter.Close();
                    connexion.Close();  //disconnect from database
                    return null;
                }
            }

            logFileWriter.WriteLine("");

            return list_of_products;
        }

        private static string[,] insertDesadv_Veolog(string reference_DESADV_doc, Veolog_DESADV dh, List<Veolog_DESADV_Lines> dl, StreamWriter logFileWriter)
        {
            string[,] list_of_cmd_lines = new string[dl.Count, 27];    // new string [x,y]
            string[] list_of_client_info = null;

            int position_item = 0;
            DateTime d = DateTime.Now;
            string curr_date_time = d.ToString("yyyy-MM-dd hh:mm:ss");
            string curr_date = d.ToString("yyyy-MM-dd");
            //string curr_time = "000" + d.ToString("hhmmss");
            string curr_date_seconds = d.Year + "" + d.Month + "" + d.Day + "" + d.Hour + "" + d.Minute + "" + d.Second;

            string ref_client = "";
            string ref_article = "";
            string name_article = "";
            string DL_PoidsNet = "0";
            string DL_PoidsBrut = "0";
            string DL_PrixUnitaire = "0";

            // AR_Design, AR_PoidsNet, AR_PoidsBrut, AR_PrixAch

            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL()) //connecting to database as handler
            {
                try
                {
                    connection.Open(); //opening the connection
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Connexion ouverte.");

                    int counter = 0;

                    foreach (Veolog_DESADV_Lines line in dl) //read item by item
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Lire la ligne de l'article.");

                        //get Product Name By Reference
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getProductNameByReference_DESADV(true, line.Code_Article));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getProductNameByReference_DESADV(true, line.Code_Article), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    ref_article = (reader[0].ToString());         // get product ref
                                    name_article = (reader[1].ToString());        // sum up the total_negative variable. - check query
                                    DL_PoidsNet = (reader[2].ToString());         // get unit weight NET - check query
                                    DL_PoidsBrut = (reader[3].ToString());        // get unit weight BRUT - check query  
                                    DL_PrixUnitaire = (reader[4].ToString());     // get unit price  - check query 
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. ");
                                }
                            }
                        }

                        //get Client Reference From CMD Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Commande_Donneur_Ordre));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceFromCMD_DESADV(true, dh.Commande_Donneur_Ordre), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
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
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. Ref Client ===> " + ref_client);

                                }
                            }
                        }

                        //get Client Reference by Ref
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : SQL ===> " + QueryHelper.getClientReferenceById_DESADV(true, ref_client));
                        using (OdbcCommand command = new OdbcCommand(QueryHelper.getClientReferenceById_DESADV(true, ref_client), connection)) //execute the function within this statement : getNegativeStockOfAProduct()
                        {
                            using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
                            {
                                if (reader.Read()) // If any rows returned
                                {
                                    list_of_client_info = new string[13];
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
                                }
                                else// If no rows returned
                                {
                                    //do nothing.
                                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Aucune reponse. list_of_client_info est null");
                                }
                            }
                        }

                        if (ref_article != "" && name_article != "" && list_of_client_info != null)
                        {
                            logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Article trouvé.");
                            logFileWriter.WriteLine("");

                            try
                            {
                                position_item += 1000;

                                // DESADV prefix will be used to create document
                                list_of_cmd_lines[counter, 0] = "0"; // DO_Domaine
                                list_of_cmd_lines[counter, 1] = "3"; //DO_Type
                                list_of_cmd_lines[counter, 2] = "3"; //DO_DocType
                                list_of_cmd_lines[counter, 3] = list_of_client_info[0]; //CT_NUM
                                list_of_cmd_lines[counter, 4] = reference_DESADV_doc; //DO_Piece
                                list_of_cmd_lines[counter, 5] = curr_date; //DO_Date
                                list_of_cmd_lines[counter, 6] = curr_date; //DL_DateBC
                                list_of_cmd_lines[counter, 7] = (position_item).ToString(); // DL_Ligne line number 1000,2000
                                list_of_cmd_lines[counter, 8] = curr_date_seconds; // DO_Ref
                                list_of_cmd_lines[counter, 9] = ref_article; // AR_Ref
                                list_of_cmd_lines[counter, 10] = "1"; //DL_Valorise
                                list_of_cmd_lines[counter, 11] = "1"; //DE_NO
                                list_of_cmd_lines[counter, 12] = name_article; // DL_Design
                                list_of_cmd_lines[counter, 13] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", ".");  //line.Quantite_Colis; // DL_Qte
                                list_of_cmd_lines[counter, 14] = Convert.ToDouble(DL_PoidsNet).ToString().Replace(",", "."); // DL_PoidsNet
                                if (list_of_cmd_lines[counter, 14].Equals("0")) { list_of_cmd_lines[counter, 14] = "0.000000"; } else if (!list_of_cmd_lines[counter, 14].Contains(".")) { list_of_cmd_lines[counter, 14] = list_of_cmd_lines[counter, 14] + ".000000"; }

                                list_of_cmd_lines[counter, 15] = Convert.ToDouble(DL_PoidsBrut).ToString().Replace(",", "."); // DL_PoidsBrut
                                if (list_of_cmd_lines[counter, 15].Equals("0")) { list_of_cmd_lines[counter, 15] = "0.000000"; } else if (!list_of_cmd_lines[counter, 15].Contains(".")) { list_of_cmd_lines[counter, 15] = list_of_cmd_lines[counter, 15] + ".000000"; }

                                list_of_cmd_lines[counter, 16] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixUnitaire
                                if (list_of_cmd_lines[counter, 16].Equals("0")) { list_of_cmd_lines[counter, 16] = "0.000000"; } else if (!list_of_cmd_lines[counter, 16].Contains(".")) { list_of_cmd_lines[counter, 16] = list_of_cmd_lines[counter, 16] + ".000000"; }

                                list_of_cmd_lines[counter, 17] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_PrixRU
                                if (list_of_cmd_lines[counter, 17].Equals("0")) { list_of_cmd_lines[counter, 17] = "0.000000"; } else if (!list_of_cmd_lines[counter, 17].Contains(".")) { list_of_cmd_lines[counter, 17] = list_of_cmd_lines[counter, 17] + ".000000"; }

                                list_of_cmd_lines[counter, 18] = DL_PrixUnitaire.ToString().Replace(",", "."); // DL_CMUP
                                list_of_cmd_lines[counter, 19] = DL_PrixUnitaire.ToString().Replace(",", "."); // EU_Enumere
                                list_of_cmd_lines[counter, 20] = Convert.ToInt16(line.Quantite_Colis).ToString().Replace(",", "."); // EU_Qte; // EU_Qte
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }

                                list_of_cmd_lines[counter, 21] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantHT
                                list_of_cmd_lines[counter, 22] = (Convert.ToDouble(line.Quantite_Colis) * Convert.ToDouble(DL_PrixUnitaire)).ToString().Replace(",", "."); //DL_MontantTTC
                                if (list_of_cmd_lines[counter, 20].Equals("0")) { list_of_cmd_lines[counter, 20] = "0.000000"; } else if (!list_of_cmd_lines[counter, 20].Contains(".")) { list_of_cmd_lines[counter, 20] = list_of_cmd_lines[counter, 20] + ".000000"; }
                                if (list_of_cmd_lines[counter, 21].Equals("0")) { list_of_cmd_lines[counter, 21] = "0.000000"; } else if (!list_of_cmd_lines[counter, 21].Contains(".")) { list_of_cmd_lines[counter, 21] = list_of_cmd_lines[counter, 21] + ".0"; }
                                if (list_of_cmd_lines[counter, 22].Equals("0")) { list_of_cmd_lines[counter, 22] = "0.000000"; } else if (!list_of_cmd_lines[counter, 22].Contains(".")) { list_of_cmd_lines[counter, 22] = list_of_cmd_lines[counter, 22] + ".000000"; }

                                list_of_cmd_lines[counter, 23] = ""; //PF_Num
                                list_of_cmd_lines[counter, 24] = "0"; //DL_No
                                list_of_cmd_lines[counter, 25] = "0"; //DL_FactPoids
                                list_of_cmd_lines[counter, 26] = "0"; //DL_Escompte

                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show("Exception : 2D table not working properly.\r\n" + ex.Message);
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ******************** Exception ********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Le tableau 'BL' à 2 dimensions ne fonctionne pas correctement, message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                logFileWriter.Close();
                                return null;
                            }
                        }

                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Compter => " + counter);
                        counter++;
                    }
                    // ===== End Foreach =====


                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Vérifier si un produit pour 0 = BL");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Requête en cours d'exécution ===>\r\n" + QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, dh.Commande_Donneur_Ordre, list_of_client_info, dh.Etat));

                    //generate document BLF_____. in database.
                    try
                    {
                        OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocument_Veolog(true, "3", reference_DESADV_doc, curr_date, dh.Commande_Donneur_Ordre, list_of_client_info, dh.Etat), connection); //calling the query and parsing the parameters into it
                        command.ExecuteReader(); // executing the query
                    }
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                        logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                        //logFileWriter.Close();
                        return null;
                    }


                    string[,] products_MS = new string[position_item / 1000, 27]; // create array with enough space

                    //insert documentline into the database with articles having 20 as value @index 2
                    logFileWriter.WriteLine("");
                    logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert documentline into the database with articles having 3 as value @index 2");

                    for (int x = 0; x < list_of_cmd_lines.GetLength(0); x++)
                    {
                        if (list_of_cmd_lines[x, 1] == "3")
                        {
                            for (int y = 0; y < list_of_cmd_lines.GetLength(1); y++)
                            {
                                products_MS[x, y] = list_of_cmd_lines[x, y];
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : products_BL_L[" + x + "," + y + "] = " + products_MS[x, y]);
                            }

                            //insert the article to documentline in the database
                            try
                            {
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : insert the article " + products_MS[x, 12] + " (Ref:" + products_MS[x, 9] + ") to documentline in the database");

                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : requette sql ===> " + QueryHelper.insertDesadvDocumentLine_Veolog(true, products_MS, x));

                                OdbcCommand command = new OdbcCommand(QueryHelper.insertDesadvDocumentLine_Veolog(true, products_MS, x), connection);
                                command.ExecuteReader();
                            }
                            catch (OdbcException ex)
                            {
                                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                                logFileWriter.WriteLine("");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : ********************** OdbcException *********************");
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Message :" + ex.Message);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : StackTrace :" + ex.StackTrace);
                                logFileWriter.WriteLine(DateTime.Now + " | insertDesadv_Veolog() : Import annulée");
                                //logFileWriter.Close();
                                return null;
                            }
                        }
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
                    return null;
                }
            }

            return list_of_cmd_lines;
        }

        public static string lastNumberReference(string mask, StreamWriter logFileWriter)
        {

            string db_result = "";
            string result = "";

            if (mask == "ME")
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask ME");


                using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connection.Open();

                        OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connection); //execute the function within this statement : getNegativeStockOfAProduct()

                        using (IDataReader reader = command.ExecuteReader()) // read rows of the executed query
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
                    catch (OdbcException ex)
                    {


                        Console.WriteLine("Message : " + ex.Message + ".");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  SQL ===> " + QueryHelper.getLastPieceNumberReference(false, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() |  Import annulée");
                        logFileWriter.Close();
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
                        logFileWriter.Close();
                    }

                    
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "MS")
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask MS");

                using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connection.Open();

                        OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connection); //execute the function within this statement : getNegativeStockOfAProduct()

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
                    catch (OdbcException ex)
                    {
                        Console.WriteLine("Message : " + ex.Message + ".");
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(false, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        logFileWriter.Close();
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
                        logFileWriter.Close();
                    }
                }
                logFileWriter.WriteLine("");
                return result;
            }
            else if (mask == "BL")
            {
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | Recuperer le dernier mask BL");
                logFileWriter.WriteLine(DateTime.Now + " : lastNumberReference() | SQL => " + QueryHelper.getLastPieceNumberReference(true, mask));

                using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                {
                    try
                    {
                        connection.Open();

                        OdbcCommand command = new OdbcCommand(QueryHelper.getLastPieceNumberReference(true, mask), connection); //execute the function within this statement : getNegativeStockOfAProduct()

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
                    catch (OdbcException ex)
                    {
                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " : ********************** OdbcException *********************");
                        logFileWriter.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getLastPieceNumberReference(true, mask));
                        logFileWriter.WriteLine(DateTime.Now + " : Message : " + ex.Message + ".");
                        logFileWriter.WriteLine(DateTime.Now + " : Import annulée");
                        logFileWriter.Close();
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
            return null;
        }


        /*
        public void SendToVeolog()
        {
            string outputFile = "";
            string outputFileError = "";
            string outputFileLog = "";
            //Boolean ThereIsError = false;


            Boolean FileExiste = false;
            int SaveSuccess = 0;

            List<string> tabCommande = new List<string>();
            List<string> tabCommandeError = new List<string>();
            List<Order> ordersList = new List<Order>();

            //Classes.Path path = getPath();
            //dir = path.path;

            Console.WriteLine("##################################################################################");
            Console.WriteLine("######################## Envoie Bon de Commande à Velog ##########################");
            Console.WriteLine("##################################################################################");
            Console.WriteLine("");
            Console.WriteLine("Execution en cours..");
            Console.WriteLine("");

            // Creer dossier sortie "LOG Directory" --------------------------
            var dirName = string.Format(@"{0:MM-yyyy}\LogSage_Veolog(planifiée){0:dd-MM-yyyy HH.mm.ss}", DateTime.Now);
            var logName = string.Format("Log_Veolog{0:dd-MM-yyyy}", DateTime.Now);
            outputFile = Directory.GetCurrentDirectory() + @"\LOG\Success_File\" + dirName;
            outputFileError = Directory.GetCurrentDirectory() + @"\LOG\Error_File\";
            outputFileLog = Directory.GetCurrentDirectory() + @"\LOG\" + logName;

            if (!Directory.Exists(outputFile))
            {
                System.IO.Directory.CreateDirectory(outputFile);
            }
            if (!Directory.Exists(outputFileError))
            {
                System.IO.Directory.CreateDirectory(outputFileError);
            }
            if (!Directory.Exists(outputFileLog))
            {
                System.IO.Directory.CreateDirectory(outputFileLog);
            }

            // Creer fichier de sortie "LOG File" ------------------------
            var nameLogfile = string.Format("logFile {0:dd-MM-yyyy HH.mm.ss}.log", DateTime.Now);
            LogFile = new StreamWriter(outputFileLog + @"\" + nameLogfile);
            cheminLogFile = outputFileLog + @"\" + nameLogfile;

            LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : ##################################################################################");
            LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : ######################## Envoie Bon de Commande à Velog ##########################");
            LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : ##################################################################################");
            LogFile.WriteLine("");
            LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : Scan dans la Base de données les statuts de \"Bon Commande\" ...");
            LogFile.WriteLine("");

            //Get Doc Entette DO_Statut
            //Get a list of 100 orders for Veolog with a DO_Statut == 1 
            //Export the 'Bon de Livraison' BC as .csv file
            //send the csv file to Velog
            string[,] lits_of_stock = new string[100, 2];

            using (OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connexion.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getCommandeStatut(true), connexion);

                    LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : SQL ===> " + QueryHelper.getCommandeStatut(true));

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        int x = 0;
                        while (reader.Read()) // reads lines/rows from the query
                        {
                            if (reader[101].ToString().Equals("1"))
                            {
                                lits_of_stock[x, 0] = reader[101].ToString(); // cbMarq
                                lits_of_stock[x, 1] = reader[49].ToString(); // DO_Statut
                                LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : cbMarq = " + reader[101].ToString() + " DO_Statut = " + reader[49].ToString());
                                x++;
                            }
                        }
                    }
                    LogFile.WriteLine(DateTime.Now + " | SendToVeolog() : Connexion close.");
                    connexion.Close();

                }
                catch (OdbcException ex)
                {
                    LogFile.WriteLine("");
                    LogFile.WriteLine(DateTime.Now + " : SendToVeolog() |  ********************** OdbcException *********************");
                    LogFile.WriteLine(DateTime.Now + " : SendToVeolog() |  SQL ===> " + QueryHelper.getCommandeStatut(true));
                    LogFile.WriteLine(DateTime.Now + " : SendToVeolog() |  Message : " + ex.Message + ".");
                    LogFile.WriteLine(DateTime.Now + " : SendToVeolog() |  Scan annulée");
                    LogFile.Close();
                    return;
                }
            }


            LogFile.Close();
        }

    */
        // #####################################################################################################
        //##################################################################################################

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
                                Client cli = new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString());
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
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getStockId(false), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
                                string id = reader[0].ToString();
                                connection.Close();
                                return id;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");
                                writer.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return null;
                }
            }

        }

        public static string getNumLivraison(string client_num, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getNumLivraison(false, client_num), connection))
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
                                //Console.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");
                                writer.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    //Console.Read();
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

        public static Article getArticle(string code_article, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticle(false, code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Article article = new Article(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString());
                                //Console.WriteLine(reader[0].ToString() + "-" + reader[1].ToString() + "-" + reader[2].ToString()+"-"+ reader[3].ToString()+"-"+ reader[4].ToString()+"-"+ reader[5].ToString()+"-"+ reader[6].ToString()+"-"+ reader[7].ToString()+"-"+ reader[8].ToString()+"-"+ reader[9].ToString());
                                //Console.WriteLine(reader[0].ToString());
                                //Console.WriteLine(article.AR_REF+" gamme1:"+ article.gamme1+" gamme2:"+article.gamme2 );
                                connection.Close();
                                return article;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - code article " + code_article + " n'existe pas dans la base.");
                                writer.WriteLine(DateTime.Now + " : Erreur - code article " + code_article + " n'existe pas dans la base.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[2]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[2]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
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


        public string InfoTachePlanifier()
        {
            try
            {
                string taskName = "importCommandeSage";
                TaskService ts = new TaskService();
                if (ts.FindTask(taskName, true) != null)
                {
                    Task t = ts.GetTask(taskName);
                    TaskDefinition td = t.Definition;
                    //Console.WriteLine("L'import des commandes Planifiée :");
                    //Console.WriteLine("" + td.Triggers[0]);
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
                //writer.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                return null;
            }
        }

        public static Boolean deleteCommande(string NumCommande)
        {
            // Insertion dans la base sage : cbase
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


        public static string testGamme(int type, string code_article, string gamme, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getGAMME(false, type, code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            List<string> list = new List<string>();
                            while (reader.Read())
                            {
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
                                return list[0];
                            }

                            return gamme;

                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

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

        public static string get_next_num_piece_commande(StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
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
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_condition_livraison_indice(false, c_mode), connection))
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
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[29] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[29] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }

        }

        public static List<AdresseLivraison> get_adresse_livraison(AdresseLivraison adresse, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<AdresseLivraison> list = new List<AdresseLivraison>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_adresse_livraison(false, adresse), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Console.WriteLine(reader[0].ToString());
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
                    writer.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.fournisseurExiste(false, num), connection))
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
                                //Console.WriteLine(DateTime.Now + " : Erreur[35] - Code GLN fournisseur " + num + " n'existe pas.");
                                writer.WriteLine(DateTime.Now + " : Erreur[35] - Code GLN fournisseur " + num + " n'existe pas.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(DateTime.Now + " : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(false, id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString());
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                if (flag == 1)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                }
                                if (flag == 2)
                                {
                                    // Console.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                    writer.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
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
                    writer.WriteLine(DateTime.Now + " : SQL ===> " + QueryHelper.getClient(false, id));
                    writer.WriteLine(DateTime.Now + " : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static string getDevise(string codeIso, StreamWriter writer)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getDevise(false, codeIso), connection))
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
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " : Erreur[44] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static ConfSendMail getInfoMail(StreamWriter writer)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Sage\\Connecteur sage");
                if (key != null)
                {
                    ConfSendMail confMail = new ConfSendMail();
                    confMail.active = key.GetValue("active").ToString() == "" ? false : (key.GetValue("active").ToString() == "True" ? true : false);
                    confMail.smtp = key.GetValue("smtp").ToString();
                    confMail.port = key.GetValue("port").ToString() == "" ? 0 : int.Parse(key.GetValue("port").ToString());
                    confMail.password = Utils.Decrypt(key.GetValue("password").ToString());
                    confMail.login = key.GetValue("login").ToString();
                    confMail.dest1 = key.GetValue("dest1").ToString();
                    confMail.dest2 = key.GetValue("dest2").ToString();
                    confMail.dest3 = key.GetValue("dest3").ToString();
                    return confMail;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now + " : Erreur[43] - " + ex.Message);

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
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getConditionnementArticle(false, code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Conditionnement Conditionnement = new Conditionnement(reader[0].ToString(), reader[1].ToString());
                                connection.Close();
                                return Conditionnement;

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
                    writer.WriteLine(DateTime.Now + " : Erreur[47] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }

        }

        public void LancerPlanification()
        {
            this.ImportPlanifier();

            //this.SendToVeolog();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("getting path : ");
            Classes.Path path = new Path(); 
            path.Load();

            ConfigurationExport export = new ConfigurationExport();
            export.Load();

            Console.WriteLine(DateTime.Now + " : Path => " + path.path);
            Console.WriteLine("");
            Console.WriteLine(DateTime.Now + " : exportFactures => " + export.exportFactures);
            Console.WriteLine(DateTime.Now + " : export.exportBonsLivraisons => " + export.exportBonsLivraisons);
            Console.WriteLine(DateTime.Now + " : export.exportBonsCommandes => " + export.exportBonsCommandes);
            Console.WriteLine(DateTime.Now + " : export.exportStock => " + export.exportStock);
            Console.WriteLine("");

            if (((export.exportFactures == "True") ? true : false))
            {
                Console.WriteLine(DateTime.Now + " : exportFactures");
                Classes.ExportFactures a = new Classes.ExportFactures(path.path);
                a.ExportFacture();
            }
            Console.WriteLine("");
            if (((export.exportBonsLivraisons == "True") ? true : false))
            {
                Console.WriteLine(DateTime.Now + " : exportBonsLivraisons");
                Classes.ExportBonLivraison b = new Classes.ExportBonLivraison(path.path);
                b.ExportBonLivraisonAction();
            }
            Console.WriteLine("");
            if (((export.exportBonsCommandes == "True") ? true : false))
            {
                Console.WriteLine(DateTime.Now + " : exportBonsCommandes");
                Classes.ExportCommandes c = new Classes.ExportCommandes(path.path);
                c.ExportCommande();
            }
            Console.WriteLine("");
            if (((export.exportStock == "True") ? true : false))
            {
                Console.WriteLine(DateTime.Now + " : exportStock");
                Classes.ExportStocks s = new Classes.ExportStocks(path.path);
                s.ExportStock();
            }
            Console.WriteLine("");
            Console.WriteLine(DateTime.Now + " : Done Export");

        }

        public static int Calcule_conditionnement(decimal quantite, string quantite_conditionnement, StreamWriter writer)
        {
            try
            {
                int qc = int.Parse(quantite_conditionnement);
                int q = (int)quantite;

                int valeur = q / qc;
                int reste = q % qc;
                if (reste == 0)
                    return valeur;
                else
                    return valeur + 1;
            }
            catch (Exception e)
            {
                writer.WriteLine(DateTime.Now + " : Erreur[46] - " + "Erreur Calcule de conditionnement :" + e.Message);
                return 0;
            }
        }

        // Get the current time in milliseconds
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
