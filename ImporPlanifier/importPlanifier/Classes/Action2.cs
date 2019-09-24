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
        private static StreamWriter LogFile;
        private static string cheminLogFile;


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
            string outputFile = "";
            string outputFileError = "";
            string outputFileLog = "";
            //Boolean ThereIsError = false;


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
            //Console.WriteLine("##############################################");
            //Console.WriteLine("############ L'import planifier ##############");
            //Console.WriteLine("##############################################");
            //Console.WriteLine("");

            if (dir == null)
            {
                //Console.WriteLine(DateTime.Now + " : *********** Erreur **********");
                //Console.WriteLine(DateTime.Now + " : L'emplacement de l'import n'est pas enregistrer");
                //Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }

            if (!Directory.Exists(dir))
            {
                //Console.WriteLine(DateTime.Now + " : *********** Erreur **********");
                //Console.WriteLine(DateTime.Now + " : L'emplacement n'est pas trouvé.");
                //Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }

            DirectoryInfo fileListing = new DirectoryInfo(dir);
            string infoPlan = InfoTachePlanifier();
            if (infoPlan == null)
            {
                //Console.WriteLine(DateTime.Now + " : Aucune importation planifiée trouvé");
                //Console.WriteLine(DateTime.Now + " : Import annulée");
                goto goError;
            }
            //Console.WriteLine("Dossier : " + fileListing);
            //Console.WriteLine("");
            //Console.WriteLine(DateTime.Now + " : Scan du dossier ...");

            // Creer dossier sortie "LOG Directory" --------------------------
            var dirName = string.Format(@"Commandes {0:MM-yyyy}\LogSage(planifiée) {0:dd-MM-yyyy HH.mm.ss}", DateTime.Now);
            var logName = string.Format("Log {0:dd-MM-yyyy}", DateTime.Now);
            outputFile = dir + @"\Success File\" + dirName;
            outputFileError = dir + @"\Error File\";
            outputFileLog = dir + @"\LOG\" + logName;
            System.IO.Directory.CreateDirectory(outputFile);
            if (!Directory.Exists(outputFileError))
            {
                System.IO.Directory.CreateDirectory(outputFileError);
            }
            if (!Directory.Exists(outputFileLog))
            {
                System.IO.Directory.CreateDirectory(outputFileLog);
            }
            // Creer fichier de sortie "LOG File" ------------------------
            //LogFile = new StreamWriter(outputFile + @"\logFile.log");
            var nameLogfile = string.Format("logFile {0:dd-MM-yyyy HH.mm.ss}.log", DateTime.Now);
            LogFile = new StreamWriter(outputFileLog + @"\" + nameLogfile);
            cheminLogFile = outputFileLog + @"\" + nameLogfile;
            LogFile.WriteLine("##############################################");
            LogFile.WriteLine("############ L'import planifié ##############");
            LogFile.WriteLine("##############################################");
            LogFile.WriteLine("");
            LogFile.WriteLine(infoPlan);
            LogFile.WriteLine("Dossier : " + fileListing);
            LogFile.WriteLine("");
            LogFile.WriteLine(DateTime.Now + " : Scan du dossier ...");

            //string nextIdString = NextNumPiece();

            //if (nextIdString == "erreur")
            //{
            //    goto goError;
            //}

            //if (nextIdString == null)
            //{
            //    LogFile.WriteLine(DateTime.Now + " : Erreur[22] numéro de piece non valide.");
            //    //Console.WriteLine(DateTime.Now + " : Erreur[22] numéro de piece non valide.");
            //    goto goError;
            //}

            //int nextId = int.Parse(nextIdString.Replace("BC",""));


            // Recherche des fichiers .csv
            foreach (FileInfo filename in fileListing.GetFiles("*.csv"))
            {
                Order order = new Order();

                try
                {
                    
                    order.Lines = new List<OrderLine>();

                    order.Id = get_next_num_piece_commande();

                    //if (TestSiNumPieceExisteDeja(order.Id))
                    //{
                    //    order.Id = NextNumPiece();
                    //}
                    if (order.Id == null || order.Id == "erreur")
                    {
                        tabCommandeError.Add(filename.ToString());
                        goto goOut; 
                    }
                    //order.Id = nextId.ToString();

                    //while (order.Id.Length < 5)
                    //{
                    //    order.Id = "0" + order.Id;
                    //}

                    //order.Id = "BC" + order.Id;

                    Boolean prixDef = false;
                    nbr++;
                    FileExiste = true;
                    //Console.WriteLine(DateTime.Now + " : un fichier \".csv\" trouvé :");
                    //Console.WriteLine(DateTime.Now + " : -----> " + nbr + "- " + filename);
                    //Console.WriteLine(DateTime.Now + " : Scan fichier...");
                    LogFile.WriteLine(DateTime.Now + " : un fichier \".csv\" trouvé :");
                    LogFile.WriteLine(DateTime.Now + " : -----> Fichier " + nbr + " : " + filename);
                    LogFile.WriteLine(DateTime.Now + " : Scan fichier...");
                    Console.WriteLine(DateTime.Now + " : Ficher " + nbr + " : " + filename);

                    long pos = 1;
                    string[] lines = System.IO.File.ReadAllLines(fileListing + @"\" + filename, Encoding.Default);

                    if (lines.Length < 4)
                    {
                        LogFile.WriteLine(DateTime.Now + " : Erreur[17] Le fichier contient des erreurs.");
                        //Console.WriteLine(DateTime.Now + " : Erreur[17] Le fichier contient des erreurs.");
                        tabCommandeError.Add(filename.ToString());
                        goto goOut;
                    }

                    if (lines[0].Split(';')[0] == "ORDERS" && lines[0].Split(';').Length == 11)
                    {
                        order.NumCommande = lines[0].Split(';')[1];

                        if (order.NumCommande.Length > 10)
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[21] - Numéro de commande doit être < 10");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[21] - Numéro de commande doit être < 10");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        if (order.NumCommande == "")
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[33] - Le champ numéro de commande est vide.");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[33] - Le champ numéro de commande est vide.");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        if (!IsNumeric(order.NumCommande))
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[34] - Le champ numéro de commande est invalide.");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[34] - Le champ numéro de commande est invalide.");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        for (int i = 0; i < tabCommande.Count; i++)
                        {
                            if (order.NumCommande == tabCommande[i])
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur[7] - Le fichier contient un numero de commande déja scanner : " + order.NumCommande);
                                LogFile.WriteLine(DateTime.Now + " : Erreur[7] - Le fichier contient un numero de commande déja scanner : " + order.NumCommande);
                                tabCommandeError.Add(filename.ToString());
                                goto goOut;
                            }
                        }


                        string existe = existeCommande(order.NumCommande);

                        if (existe != null && existe != "erreur")
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[26] - La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + "");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[26] - La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + "");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        if (existe == "erreur")
                        {
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        order.codeClient = lines[0].Split(';')[2];
                        order.codeAcheteur = lines[0].Split(';')[3];
                        order.codeFournisseur = lines[0].Split(';')[4];
                        order.adresseLivraison = lines[0].Split(';')[7];

                        Client client = getClient(order.codeClient, 1);
                        if (client == null)
                        {
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        Client client2 = getClient(order.codeAcheteur, 2);
                        if (client2 == null)
                        {
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        if (existeFourniseur(order.codeFournisseur) == null)
                        {
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        order.adresseLivraison = lines[0].Split(';')[7];
                        string[] tab_adress = order.adresseLivraison.Split('.');
                        if (tab_adress.Length != 5)
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[38] - La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante : Nom.Adresse.CodePostal.Ville.Pays");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[38] - La forme de l'adresse de livraison est incorrecte, Veuillez respecter la forme suivante : Nom.Adresse.CodePostal.Ville.Pays");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }
                        order.nom_contact = tab_adress[0];
                        order.adresse = tab_adress[1].Replace("'", "''");
                        order.codepostale = tab_adress[2];
                        order.ville = tab_adress[3].Replace("'", "''");
                        order.pays = tab_adress[4];

                        //return;

                        List<AdresseLivraison> listAdress = get_adresse_livraison(new AdresseLivraison(1,client.CT_Num, order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays));

                    

                        // Ajouter ville dans la réference
                        //string[] part = order.adresseLivraison.Split('.');
                        //if (part.Length >= 2)
                        //{
                        order.Reference = order.ville;
                        //}

                        //string Ref = order.Reference + "/" + order.NumCommande;

                        //if (Ref.Length <= 17)
                        //{
                        //    order.Reference = Ref;
                        //}

                        order.deviseCommande = lines[0].Split(';')[8];

                        if (order.deviseCommande != "")
                        {
                            order.deviseCommande = getDevise(order.deviseCommande);
                        }

                        if (order.deviseCommande == "erreur")
                        {
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                        if (lines[1].Split(';')[0] == "ORDHD1" && lines[1].Split(';').Length == 5)
                        {

                            if (lines[1].Split(';')[1].Length == 8)
                            {
                                order.DateCommande = ConvertDate(lines[1].Split(';')[1]);
                                order.conditionLivraison = lines[1].Split(';')[2];


                                if (order.conditionLivraison != "")
                                {
                                    order.conditionLivraison = get_condition_livraison(order.conditionLivraison);
                                }

                                if (string.IsNullOrEmpty(order.conditionLivraison))
                                {
                                    order.conditionLivraison = "1";
                                }

                                if (lines[2].Split(';')[0] == "ORDLIN" && lines[2].Split(';').Length == 23)
                                {
                                    decimal total = 0m;
                                    foreach (string ligneDuFichier in lines)
                                    {

                                        string[] tab = ligneDuFichier.Split(';');

                                        switch (tab[0])
                                        {
                                            case "ORDLIN":
                                                if (tab.Length == 23)
                                                {
                                                    OrderLine line = new OrderLine();
                                                    line.NumLigne = tab[1];
                                                    line.article = getArticle(tab[2]);
                                                    if (line.article == null)
                                                    {
                                                        tabCommandeError.Add(filename.ToString());
                                                        goto goOut;
                                                    }

                                                    line.article.Conditionnement = getConditionnementArticle(line.article.AR_REF);

                                                    if (line.article.AR_Nomencl == "2" || line.article.AR_Nomencl == "3")
                                                    {
                                                        line.article.AR_REFCompose = line.article.AR_REF;
                                                    }

                                                    if (line.article.gamme1 != "0")
                                                    {
                                                        line.article.gamme1 = testGamme(0, line.article.AR_REF, line.article.gamme1);
                                                    }

                                                    if (line.article.gamme2 != "0")
                                                    {
                                                        line.article.gamme2 = testGamme(1, line.article.AR_REF, line.article.gamme2);
                                                    }

                                                    line.Quantite = tab[9].Replace(",", ".");
                                                    decimal d = Decimal.Parse(line.Quantite, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                    if (d == 0)
                                                    {

                                                        line.Quantite = "1";

                                                    }

                                                    if (line.article.Conditionnement != null)
                                                    {
                                                        int quantite_Conditionnement = Calcule_conditionnement(d, line.article.Conditionnement.EC_QUANTITE);
                                                        line.Calcule_conditionnement = quantite_Conditionnement.ToString();
                                                    } 

                                                    line.PrixNetHT = tab[14].Replace(",", ".");
                                                    line.MontantLigne = tab[11];
                                                    if (tab[21].Length != 8)
                                                    {

                                                        //Console.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.\nDate de livraison non valide");
                                                        LogFile.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.\nDate de livraison non valide");
                                                        tabCommandeError.Add(filename.ToString());
                                                        goto goOut;
                                                    }
                                                    line.DateLivraison = "'{d " + ConvertDate(tab[21]) + "}'";
                                                    if (line.DateLivraison.Length == 6)
                                                    {
                                                        line.DateLivraison = "Null";
                                                    }

                                                    //if (line.article.AR_UnitePoids == "2")
                                                    //{
                                                    //     if(!string.IsNullOrEmpty(line.article.AR_POIDSBRUT))
                                                    //    {
                                                    //    line.article.AR_POIDSBRUT = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                    //     }
                                                    //     if (!string.IsNullOrEmpty(line.article.AR_POIDSNET))
                                                    //     {
                                                    //         line.article.AR_POIDSNET = Convert.ToString(1000 * d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                    //     }
                                                    //}

                                                    //line.article.AR_POIDSNET = line.article.AR_POIDSNET;

                                                    //if (line.article.AR_UnitePoids == "3")
                                                    //{
                                                        if(!string.IsNullOrEmpty(line.article.AR_POIDSBRUT))
                                                        {
                                                        line.article.AR_POIDSBRUT = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSBRUT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                        }
                                                        if (!string.IsNullOrEmpty(line.article.AR_POIDSNET))
                                                        {
                                                            line.article.AR_POIDSNET = Convert.ToString(d * Decimal.Parse(line.article.AR_POIDSNET, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)).Replace(",", ".");
                                                        }
                                                     //}

                                                    line.codeAcheteur = tab[4].Replace(" ", "");
                                                    line.codeFournis = tab[5].Replace(" ", "");

                                                    line.descriptionArticle = tab[8].Replace("'","''");
                                                    if (string.IsNullOrEmpty(line.descriptionArticle))
                                                    {
                                                        line.descriptionArticle = line.article.AR_DESIGN;
                                                    }
                                                    total = total + Decimal.Parse(tab[11], NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                    decimal prix = Decimal.Parse(line.PrixNetHT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                    decimal prixSage = Decimal.Parse(line.article.AR_PRIXVEN.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                    if (prix != prixSage)
                                                    {
                                                        //Console.WriteLine(DateTime.Now + " : Erreur[20] - Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + " Il est différent du prix envoyer par le client : " + prix + ".");
                                                        LogFile.WriteLine(DateTime.Now + " : Erreur[20] - Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + " Il est différent du prix envoyer par le client : " + prix + ".");

                                                        prixDef = true;

                                                    }

                                                    order.Lines.Add(line);
                                                }
                                                else
                                                {

                                                    //Console.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.");
                                                    LogFile.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.");
                                                    tabCommandeError.Add(filename.ToString());
                                                    goto goOut;
                                                }
                                                break;

                                        }

                                        pos++;

                                    }

                                    order.MontantTotal = total;

                                    if (order.NumCommande == "")
                                    {
                                        //Console.WriteLine(DateTime.Now + " : Erreur[9] - Le champ numéro de commande est vide.");
                                        LogFile.WriteLine(DateTime.Now + " : Erreur[9] - Le champ numéro de commande est vide.");
                                        tabCommandeError.Add(filename.ToString());
                                        goto goOut;
                                    }

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

                                        order.StockId = getStockId();
                                        if (order.StockId == null)
                                        {
                                            tabCommandeError.Add(filename.ToString());
                                            goto goOut;
                                        }

                                        order.adresseLivraison = getNumLivraison(client.CT_Num);
                                        if (order.adresseLivraison == null)
                                        {
                                            tabCommandeError.Add(filename.ToString());
                                            goto goOut;
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
                                            //Console.WriteLine(DateTime.Now + " : Erreur[10] - Aucun ligne de commande validé.");
                                            LogFile.WriteLine(DateTime.Now + " : Erreur[10] - Aucun ligne de commande validé.");
                                            tabCommandeError.Add(filename.ToString());
                                            goto goOut;
                                        }

                                        //if (listAdress.Count != 0)
                                        //{
                                        //    order.adresseLivraison = listAdress[0].Li_no;
                                        //    if (string.IsNullOrEmpty(order.adresseLivraison))
                                        //    {
                                        //        LogFile.WriteLine(DateTime.Now + " : Adresse de livraison invalide");
                                        //        tabCommandeError.Add(filename.ToString());
                                        //        goto goOut;
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

                                        //incIntitule :

                                        //    if (listIntitules.Count != 0)
                                        //    {
                                        //        for(int i=0;i<listIntitules.Count;i++)
                                        //        {
                                        //            if (intitule == listIntitules[i])
                                        //                {
                                        //                    if(inc != 1)
                                        //                    {
                                        //                        intitule = intitule.Substring(0, (intitule.Length - 4)); 
                                        //                    }
                                        //                    intitule = intitule + " N°" + inc;
                                        //                    inc++;
                                        //                    goto incIntitule;
                                        //                }
                                        //        }
                                        //    }

                                        //    if (insert_adresse_livraison(client.CT_Num, new AdresseLivraison("", order.nom_contact, order.adresse, order.codepostale, order.ville, order.pays, order.conditionLivraison, intitule)))
                                        //    {
                                        //        order.adresseLivraison = get_Last_insert_livraison(client.CT_Num);
                                        //        if (string.IsNullOrEmpty(order.adresseLivraison))
                                        //        {
                                        //            tabCommandeError.Add(filename.ToString());
                                        //            goto goOut;
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        tabCommandeError.Add(filename.ToString());
                                        //        goto goOut;
                                        //    }
                                        //}

                                        order.adresseLivraison = getNumLivraison(client.CT_Num);
                                        if (order.adresseLivraison == null)
                                        {
                                            tabCommandeError.Add(filename.ToString());
                                            goto goOut;
                                        }

                                        // -----------------------------------------------------------
                                        Boolean RefaitInsert = false;
                                        int comptTestInsertion = 1;

                                    RefaitInsertCommande:

                                        if (RefaitInsert)
                                        {                                    
                                            order.Id = NextNumPiece();
                                            LogFile.WriteLine(DateTime.Now + " : Correction de l'erreur d'insertion.. Nouveau Numéro de pièce " + order.Id);
                                            LogFile.WriteLine(DateTime.Now + " : Refaire l'insertion de commande : INSERT N°" + comptTestInsertion);
                                        }

                                        //Insertion de commande
                                        string result = insertCommande(client, order);

                                        if (result == "OK")
                                        {

                                            int nbre = 0;

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

                                                if (insertLigneCommande(client, order, order.Lines[i]))
                                                {
                                                    nbre++;
                                                }
                                            }

                                            if (nbre == 0)
                                            {
                                                deleteCommande(order.NumCommande);
                                                tabCommandeError.Add(filename.ToString());
                                            }

                                            if (nbre != 0)
                                            {
                                                //Console.WriteLine(DateTime.Now + " : Bon de Commande enregistrée : " + order.Id);
                                                LogFile.WriteLine(DateTime.Now + " : Bon de Commande enregistrée : " + order.Id);
                                            }

                                            //Console.WriteLine(DateTime.Now + " : " + nbre + "/" + order.Lines.Count + " ligne(s) enregistrée(s)");
                                            LogFile.WriteLine(DateTime.Now + " : " + nbre + "/" + order.Lines.Count + " ligne(s) enregistrée(s)");

                                            //Console.WriteLine(DateTime.Now + " : Scan du fichier " + filename + " Terminé.");
                                            LogFile.WriteLine(DateTime.Now + " : Scan du fichier " + filename + " Terminé.");

                                            if (nbre != 0)
                                            {
                                                //deplacer les fichiers csv
                                                //Console.WriteLine(DateTime.Now + " : Déplacer le fichier vers :");
                                                //Console.WriteLine("                      " + outputFile);
                                                LogFile.WriteLine(DateTime.Now + " : Déplacer le fichier vers :");
                                                LogFile.WriteLine("                      " + outputFile);
                                                System.IO.File.Move(dir + @"\" + filename, outputFile + @"\" + filename);
                                                SaveSuccess++;
                                            }

                                            //if (nbre != 0)
                                            //{
                                            //    nextId++;
                                            //}


                                            //tabCommande[count] = order.NumCommande;
                                            //count++;
                                            tabCommande.Add(order.NumCommande);

                                        }
                                        if (result == "ERROR")
                                        {
                                            tabCommandeError.Add(filename.ToString());
                                            goto goOut;
                                        }
                                        if (result == "ERROR_PIECE")
                                        {
                                            if (comptTestInsertion < 2)
                                            {
                                                RefaitInsert = true;
                                                comptTestInsertion++;
                                                goto RefaitInsertCommande;
                                            }
                                            else
                                            {
                                                tabCommande.Add(order.NumCommande);
                                                goto goOut;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //Console.WriteLine(DateTime.Now + " : Erreur[11] - Il faut mentionner le code client.");
                                        LogFile.WriteLine(DateTime.Now + " : Erreur[11] - Il faut mentionner le code client.");
                                        tabCommandeError.Add(filename.ToString());
                                        goto goOut;
                                    }
                                }
                                else
                                {
                                    //Console.WriteLine(DateTime.Now + " : Erreur[12].. Erreur dans la troisième ligne du fichier.");
                                    LogFile.WriteLine(DateTime.Now + " : Erreur[12].. Erreur dans la troisième ligne du fichier.");
                                    tabCommandeError.Add(filename.ToString());
                                    goto goOut;
                                }
                            }
                            else
                            {

                                //Console.WriteLine(DateTime.Now + " : Erreur[13] - Date de la commande est incorrecte.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[13] - Date de la commande est incorrecte.");
                                tabCommandeError.Add(filename.ToString());
                                goto goOut;
                            }
                        }
                        else
                        {
                            //Console.WriteLine(DateTime.Now + " : Erreur[14] - Erreur dans la deuxième ligne du fichier.");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[14] - Erreur dans la deuxième ligne du fichier.");
                            tabCommandeError.Add(filename.ToString());
                            goto goOut;
                        }

                    }
                    else
                    {

                        //Console.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                        LogFile.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                        tabCommandeError.Add(filename.ToString());
                        goto goOut;
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    tabCommandeError.Add(filename.ToString());
                    goto goOut;
                }

            goOut: ;
                //Deplaçer les fichier dans le dossier : Error File
                if (File.Exists(dir + @"\" + filename))
                {
                    var errorfilename = string.Format("{0:ddMMyyyy_HHmmss}_" + filename, DateTime.Now);
                    System.IO.File.Move(dir + @"\" + filename, outputFileError + @"\" + errorfilename);
                }

            }

            if(SaveSuccess == 0)
            {
                System.IO.Directory.Delete(outputFile);
            }

            Boolean envoiMail = false;

            // Envoi de mail
            if(tabCommandeError.Count != 0)
            {
                ConfSendMail cMail = getInfoMail();
                if (cMail != null)
                {
                    if (cMail.active)
                    {
                        if (cMail.dest1 == "" && cMail.dest2 == "" && cMail.dest3 == "")
                        {
                            LogFile.WriteLine(DateTime.Now + " : Send Mail..Erreur Adresse de distinataire");
                        }
                        string commande = "";
                        for (int i = 0; i < tabCommandeError.Count; i++)
                        {
                            commande = commande + (i+1) + " - " + tabCommandeError[i] + "\n";
                        }
                        Console.WriteLine(DateTime.Now + " : Envoi de mail en cours..");
                        LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                        LogFile.WriteLine("");
                        LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                        LogFile.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                        LogFile.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                        LogFile.Close();

                        //Envoi
                        EnvoiMail(cMail, "Erreur d'import de documents commerciaux", "Bonjour,\n\nL'import d'un ou plusieurs documents commerciaux a echoué :\n" + commande + "\nVeuillez vérifier dans le fichier Log ci-joint, les problèmes qui sont survenus au moment de l'importation.\n\nNB: Les fichiers sont déplacé dans un dossier nommé : \"Error file\".\n\nCordialement,\n\nConnecteur SAGE.", cheminLogFile);

                        envoiMail = true;
                    }
                    else
                    {
                        LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                        LogFile.WriteLine("");
                        LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                        LogFile.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                        LogFile.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                        LogFile.Close();
                    }
                }
                else
                {
                    LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                    LogFile.WriteLine("");
                    LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                    LogFile.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                    LogFile.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                    LogFile.Close();
                }
            }

            if (!FileExiste && !envoiMail)
            {
                //Console.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                LogFile.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                LogFile.WriteLine("");
                LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                LogFile.Close();
                //var newlog = string.Format("logFile(0) {0:dd-MM-yyyy HH.mm.ss}.log", DateTime.Now);
                //System.IO.File.Move(outputFile + @"\logFile.log", dir + @"\" + newlog);
                //System.IO.Directory.Delete(outputFile);
                goto goError;

            }
            if (FileExiste && tabCommandeError.Count == 0)
            {
                LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                LogFile.WriteLine("");
                LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                LogFile.WriteLine("Nombre de commandes validées : " + SaveSuccess);
                LogFile.WriteLine("Nombre de commandes echouées : " + (nbr - SaveSuccess));
                LogFile.Close();
            }

          
        goError: ;
            //Console.WriteLine(DateTime.Now + " : Fin de l'execution");
            //Console.WriteLine("Nombre de fichier scanner : " + nbr);


            //Console.WriteLine("");
            //Console.WriteLine("Cliquez Entrer pour sortir ..");

            //Console.Read();

        }




        // #####################################################################################################
        //##################################################################################################

        public static Client getClient(string id)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(id), connection))
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
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Code Client " + id + " n'existe pas dans la base sage.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    //Console.Read();
                    return null;
                }
            }

        }

        public static string getStockId()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getStockId(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string id = reader[0].ToString();
                                connection.Close();
                                return id;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return null;
                }
            }

        }

        public static string getNumLivraison(string client_num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getNumLivraison(client_num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    //Console.Read();
                    return null;
                }
            }

        }

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

        public static Boolean insertLigneCommande(Client client, Order order, OrderLine orderLine)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {

                try
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.insertLigneCommande(client, order, orderLine), connection);
                    command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");
                    LogFile.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");

                    //Console.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return false;
                }


                connection.Close();
                return true;



            }

        }

        public static Article getArticle(string code_article)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticle(code_article), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Article article = new Article(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString());
                                //Console.WriteLine(reader[0].ToString() + "-" + reader[1].ToString() + "-" + reader[2].ToString()+"-"+ reader[3].ToString()+"-"+ reader[4].ToString()+"-"+ reader[5].ToString()+"-"+ reader[6].ToString()+"-"+ reader[7].ToString()+"-"+ reader[8].ToString()+"-"+ reader[9].ToString());
                                //MessageBox.Show(reader[0].ToString());
                                //MessageBox.Show(article.AR_REF+" gamme1:"+ article.gamme1+" gamme2:"+article.gamme2 );
                                connection.Close();
                                return article;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur - code article " + code_article + " n'existe pas dans la base.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - code article " + code_article + " n'existe pas dans la base.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[2]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[2]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
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
            catch
            {
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
                //Console.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                LogFile.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

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
                    OdbcCommand command = new OdbcCommand(QueryHelper.deleteCommande(NumCommande), connection);
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


        public static string testGamme(int type, string code_article, string gamme)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getGAMME(type, code_article), connection))
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return gamme;
                }
            }

        }


        public static string existeCommande(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_NumPiece_Motif(num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static string MaxNumPiece()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.MaxNumPiece(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static string NextNumPiece()
        {
            try
            {
                string NumCommande = MaxNumPiece();

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
                LogFile.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return "erreur";
            }



        }

        public static string get_next_num_piece_commande()
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_Next_NumPiece_BonCommande(), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {
                                return NextNumPiece();
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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


        public static string get_condition_livraison(string c_mode)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_condition_livraison_indice(c_mode), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[29] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }

        }

        public static List<AdresseLivraison> get_adresse_livraison(AdresseLivraison adresse)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<AdresseLivraison> list = new List<AdresseLivraison>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_adresse_livraison(adresse), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                list.Add(new AdresseLivraison(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), "",""));
                            }

                            return list;
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }

        }

        public static Boolean insert_adresse_livraison(string client, AdresseLivraison adresse)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insert_adresse_livraison(client, adresse), connection);
                    command.ExecuteReader();

                    connection.Close();
                    return true;


                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[30] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return false;
                }
            }

        }

        public static string existeFourniseur(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.fournisseurExiste(num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string numero = reader[0].ToString();
                                connection.Close();
                                return numero;

                            }
                            else
                            {
                                //Console.WriteLine(DateTime.Now + " : Erreur[35] - Code GLN fournisseur " + num + " n'existe pas.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[35] - Code GLN fournisseur " + num + " n'existe pas.");
  
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(DateTime.Now + " : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[36] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
   
                    return "erreur";
                }
            }

        }

        public static string get_Last_insert_livraison(string client)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_last_Num_Livraison(client), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string num = reader[0].ToString();
                                connection.Close();
                                return num;

                            }
                            else
                            {

                                //Console.WriteLine(DateTime.Now + " : Erreur[39] - Numero de livraison n'existe pas");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[39] - Numero de livraison n'existe pas");


                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[37] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[37] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static Client getClient(string id, int flag)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(id), connection))
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
                                    //Console.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");
                                    LogFile.WriteLine(DateTime.Now + " : Erreur[41] - GLN émetteur  " + id + " n'existe pas dans la base sage.");


                                }
                                if (flag == 2)
                                {

                                    //Console.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");
                                    LogFile.WriteLine(DateTime.Now + " : Erreur[40] - GLN destinataire  " + id + " n'existe pas dans la base sage.");


                                }
                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    //Console.WriteLine(DateTime.Now + " : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[38] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static string getDevise(string codeIso)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getDevise(codeIso), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[44] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return "erreur";
                }
            }

        }

        public static ConfSendMail getInfoMail()
        {
                try
                {
                     Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Sage\\Connecteur sage");
                     if (key != null)
                     {
                         ConfSendMail confMail = new ConfSendMail();
                         confMail.active = key.GetValue("active").ToString() == "" ? false : (key.GetValue("active").ToString() == "True" ? true : false);
                         confMail.smtp = key.GetValue("smtp").ToString();
                         confMail.port = key.GetValue("port").ToString() == "" ? 0:int.Parse(key.GetValue("port").ToString());
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[43] - "+ex.Message);

                    return null;
                }
        }

        public static void EnvoiMail(ConfSendMail confMail,string subject,string body,string attachement)
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
                if(!string.IsNullOrEmpty(confMail.dest2))
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
                Console.WriteLine(DateTime.Now + " : "+e.Message);
            }
        }

        public static Boolean TestSiNumPieceExisteDeja(string num)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestSiNumPieceExisteDeja(num), connection))
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[42] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return false;
                }
            }

        }

        public static List<string> TestIntituleLivraison(string Intitule)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    List<string> adresses = new List<string>();
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.TestIntituleLivraison(Intitule), connection))
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[45] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                    return null;
                }
            }

        }

        public static Conditionnement getConditionnementArticle(string code_article)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getConditionnementArticle(code_article), connection))
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
                    LogFile.WriteLine(DateTime.Now + " : Erreur[47] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return null;
                }
            }

        }

        public void LancerPlanification()
        {
            this.ImportPlanifier();

            Classes.Path path = getPath();

            if (path.exportFactures)
            {
                Classes.ExportFactures a = new Classes.ExportFactures(path.path);
                a.ExportFacture();
            }

            if (path.exportBonsLivraisons)
            {
            Classes.ExportBonLivraison b = new Classes.ExportBonLivraison(path.path);
            b.ExportBonLivraisonAction();
            }

            if (path.exportBonsCommandes)
            {
                Classes.ExportCommandes c = new Classes.ExportCommandes(path.path);
                c.ExportCommande();
            }

        }

        public static int Calcule_conditionnement(decimal quantite, string quantite_conditionnement)
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
                LogFile.WriteLine(DateTime.Now + " : Erreur[46] - " + "Erreur Calcule de conditionnement :" + e.Message);
                return 0;
            }
        }
    }
}
