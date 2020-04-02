using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Classes;
using System.IO;
using System.Globalization;
using System.Data.Odbc;
using importPlanifier.Helpers;
using System.Data;
using Microsoft.Win32.TaskScheduler;

namespace importPlanifier.Classes
{
    class Action
    {
        
        //private static string filename = "";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string dir;
        private static int nbr = 0;
        private static StreamWriter LogFile;


        public static string ConvertDate(string date)
        {
            if(date.Length == 8) {
            return date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
            }
            return "";
        }

        public void ImportPlanifier()
        {
            string outputFile = "";
            List<string> tabCommandeError = new List<string>();

                //tester s'il existe des fichies .csv
                Boolean FileExiste = false;
                //Boolean insertAdressLivr = false;
                int count = 0;
                string[] tabCommande = new string[200];
                List<Order> ordersList = new List<Order>();
                dir = getPath();
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
                Console.WriteLine("Dossier : " + fileListing);
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Scan du dossier ...");

                // Creer dossier sortie "LOG Directory" --------------------------
                var dirName = string.Format("LogSage(planifiée) {0:dd-MM-yyyy hh.mm.ss}", DateTime.Now);
                outputFile = dir + @"\" + dirName;
                System.IO.Directory.CreateDirectory(outputFile);
                // Creer fichier de sortie "LOG File" ------------------------
                LogFile = new StreamWriter(outputFile + @"\logFile.log");
                LogFile.WriteLine("##############################################");
                LogFile.WriteLine("############ L'import planifier ##############");
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
                //    Console.WriteLine(DateTime.Now + " : Erreur[22] numéro de piece non valide.");
                //    goto goError;
                //}

                //int nextId = int.Parse(nextIdString.Replace("BC",""));

                
                                // Recherche des fichiers .csv
                foreach (FileInfo filename in fileListing.GetFiles("*.csv"))
                {
                    try
                    {
                        Order order = new Order();
                        order.Lines = new List<OrderLine>();

                        order.Id = get_next_num_piece_commande();

                        //order.Id = nextId.ToString();

                        //while (order.Id.Length < 5)
                        //{
                        //    order.Id = "0" + order.Id;
                        //}

                        //order.Id = "BC" + order.Id;

                        Boolean prixDef = false;
                        nbr++;
                        FileExiste = true;
                        Console.WriteLine(DateTime.Now + " : un fichier \".cvs\" trouvé :");
                        Console.WriteLine(DateTime.Now + " : -----> " + nbr + "- " + filename);
                        Console.WriteLine(DateTime.Now + " : Scan fichier...");
                        LogFile.WriteLine(DateTime.Now + " : un fichier \".cvs\" trouvé :");
                        LogFile.WriteLine(DateTime.Now + " : -----> " + nbr + "- " + filename);
                        LogFile.WriteLine(DateTime.Now + " : Scan fichier...");

                        
                        long pos = 1;
                        string[] lines = System.IO.File.ReadAllLines(fileListing + @"\" + filename);

                        if (lines.Length < 4)
                        {
                            LogFile.WriteLine(DateTime.Now + " : Erreur[17] Le fichier contient des erreurs.");
                            Console.WriteLine(DateTime.Now + " : Erreur[17] Le fichier contient des erreurs.");
                            goto goOut;
                        }

                        if (lines[0].Split(';')[0] == "ORDERS" && lines[0].Split(';').Length == 11)
                        {
                            order.NumCommande = lines[0].Split(';')[1];
                            for (int i = 0; i < tabCommande.Length; i++)
                            {
                                if (order.NumCommande == tabCommande[i])
                                {
                                    Console.WriteLine(DateTime.Now + " : Erreur[7] - Le fichier contient un numero de commande déja scanner : " + order.NumCommande);
                                    LogFile.WriteLine(DateTime.Now + " : Erreur[7] - Le fichier contient un numero de commande déja scanner : " + order.NumCommande);

                                    goto goOut;
                                }
                            }

                            if (order.NumCommande.Length > 10)
                            {
                                Console.WriteLine(DateTime.Now + " : Erreur[21] - Numéro de commande doit être < 10");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[21] - Numéro de commande doit être < 10");

                                goto goOut;
                            }

                            string existe = existeCommande(order.NumCommande);

                            if (existe != null && existe != "erreur")
                            {
                                Console.WriteLine(DateTime.Now + " : Erreur[26] - La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + "");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[26] - La commande N° " + order.NumCommande + " existe deja dans la base. N° de pièce : " + existe + "");

                                goto goOut;
                            }

                            if (existe == "erreur")
                            {
                                goto goOut;
                            }

                            order.codeClient = lines[0].Split(';')[2];
                            order.adresseLivraison = lines[0].Split(';')[7];

                            // Ajouter ville dans la réference
                            string[] part = order.adresseLivraison.Split('.');
                            if (part.Length >= 2)
                            {
                                order.Reference = part[part.Length - 2];
                            }

                            //string Ref = order.Reference + "/" + order.NumCommande;

                            //if (Ref.Length <= 17)
                            //{
                            //    order.Reference = Ref;
                            //}
                            
                            order.deviseCommande = lines[0].Split(';')[8];

                            if (lines[1].Split(';')[0] == "ORDHD1" && lines[1].Split(';').Length == 5)
                            {

                                if (lines[1].Split(';')[1].Length == 8)
                                {
                                    order.DateCommande = ConvertDate(lines[1].Split(';')[1]);

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
                                                        line.article = getArticle(tab[2], "");
                                                        if (line.article == null)
                                                        {
                                                            goto goOut;
                                                        }

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
                                                        line.PrixNetHT = tab[14].Replace(",", ".");
                                                        line.MontantLigne = tab[11];
                                                        if (tab[21].Length != 8)
                                                        {

                                                            Console.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.\nDate de livraison non valide");
                                                            LogFile.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.\nDate de livraison non valide");

                                                            goto goOut;
                                                        }
                                                        line.DateLivraison = "'{d " + ConvertDate(tab[21]) + "}'";
                                                        if (line.DateLivraison.Length == 6)
                                                        {
                                                            line.DateLivraison = "Null";
                                                        }

                                                        line.codeAcheteur = tab[4];
                                                        line.descriptionArticle = tab[8];
                                                        total = total + Decimal.Parse(tab[11], NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                        
                                                        decimal prix = Decimal.Parse(line.PrixNetHT, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                                                        decimal prixSage = Decimal.Parse(line.article.AR_PRIXVEN.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                                                        if (prix != prixSage)
                                                        {
                                                            Console.WriteLine(DateTime.Now + " : Erreur[20] - Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + " Il est différent du prix envoyer par le client : " + prix + ".");
                                                            LogFile.WriteLine(DateTime.Now + " : Erreur[20] - Prix de l'article " + line.article.AR_REF + "(" + tab[2] + ") dans la base est : " + prixSage + " Il est différent du prix envoyer par le client : " + prix + ".");

                                                            prixDef = true;
                                                            
                                                        }

                                                        order.Lines.Add(line);
                                                    }
                                                    else
                                                    {

                                                        Console.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.");
                                                        LogFile.WriteLine(DateTime.Now + " : Erreur[8] - Erreur dans la ligne " + pos + " du fichier.");

                                                        goto goOut;
                                                    }
                                                    break;

                                            }

                                            pos++;

                                        }

                                        order.MontantTotal = total;

                                        if (order.NumCommande == "")
                                        {
                                            Console.WriteLine(DateTime.Now + " : Erreur[9] - Le champ numéro de commande est vide.");
                                            LogFile.WriteLine(DateTime.Now + " : Erreur[9] - Le champ numéro de commande est vide.");

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

                                            Client client = getClient(order.codeClient);
                                            if (client == null)
                                            {
                                                goto goOut;
                                            }

                                            order.StockId = getStockId();
                                            if (order.StockId == null)
                                            {
                                                goto goOut;
                                            }

                                            order.adresseLivraison = getNumLivraison(client.CT_Num);
                                            if (order.adresseLivraison == null)
                                            {
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
                                                Console.WriteLine(DateTime.Now + " : Erreur[10] - Aucun ligne de commande validé.");
                                                LogFile.WriteLine(DateTime.Now + " : Erreur[10] - Aucun ligne de commande validé.");

                                                goto goOut;
                                            }

                                            if (insertCommande(client, order))
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
                                                }

                                                if (nbre != 0)
                                                {
                                                    Console.WriteLine(DateTime.Now + " : Bon de Commande enregistrée : " + order.Id);
                                                    LogFile.WriteLine(DateTime.Now + " : Bon de Commande enregistrée : " + order.Id);
                                                }

                                                Console.WriteLine(DateTime.Now + " : "+nbre+"/"+order.Lines.Count+" ligne(s) enregistrée(s)");
                                                LogFile.WriteLine(DateTime.Now + " : " + nbre + "/" + order.Lines.Count + " ligne(s) enregistrée(s)");

                                                Console.WriteLine(DateTime.Now + " : Scan du fichier " + filename + " Terminé.");
                                                LogFile.WriteLine(DateTime.Now + " : Scan du fichier " + filename + " Terminé.");

                                                if (nbre != 0)
                                                {
                                                //deplacer les fichiers csv
                                                Console.WriteLine(DateTime.Now + " : Déplacer le fichier vers :");
                                                Console.WriteLine("                      " + outputFile);
                                                LogFile.WriteLine(DateTime.Now + " : Déplacer le fichier vers :");
                                                LogFile.WriteLine("                      " + outputFile);
                                                System.IO.File.Move(dir + @"\" + filename, outputFile + @"\" + filename);

                                                }

                                                //if (nbre != 0)
                                                //{
                                                //    nextId++;
                                                //}


                                                tabCommande[count] = order.NumCommande;
                                                count++;

                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine(DateTime.Now + " : Erreur[11] - Il faut mentionner le code client.");
                                            LogFile.WriteLine(DateTime.Now + " : Erreur[11] - Il faut mentionner le code client.");

                                            goto goOut;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(DateTime.Now + " : Erreur[12].. Erreur dans la troisième ligne du fichier.");
                                        LogFile.WriteLine(DateTime.Now + " : Erreur[12].. Erreur dans la troisième ligne du fichier.");

                                        goto goOut;
                                    }
                                }
                                else
                                {

                                    Console.WriteLine(DateTime.Now + " : Erreur[13] - Date de la commande est incorrecte.");
                                    LogFile.WriteLine(DateTime.Now + " : Erreur[13] - Date de la commande est incorrecte.");

                                    goto goOut;
                                }
                            }
                            else
                            {
                                Console.WriteLine(DateTime.Now + " : Erreur[14] - Erreur dans la deuxième ligne du fichier.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur[14] - Erreur dans la deuxième ligne du fichier.");

                                goto goOut;
                            }

                        }
                        else
                        {

                            Console.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");
                            LogFile.WriteLine(DateTime.Now + " : Erreur[15] - Erreur dans la première ligne du fichier.");

                            goto goOut;
                        }
                    }catch (Exception e)
                    {
                        Console.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                        LogFile.WriteLine(DateTime.Now + " : Erreur[16]" + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                        goto goOut;
                    }

            goOut: ;   
            }
                if (!FileExiste)
                {
                    Console.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                    LogFile.WriteLine(DateTime.Now + " : Il y a pas de fichier .csv dans le dossier.");
                    LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                    LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                    LogFile.Close();
                    var newlog = string.Format("logFile(0) {0:dd-MM-yyyy hh.mm.ss}.log", DateTime.Now);
                    System.IO.File.Move(outputFile + @"\logFile.log", dir + @"\" + newlog);
                    System.IO.Directory.Delete(outputFile);
                    goto goError;

                }
                if (FileExiste)
                {
                    LogFile.WriteLine(DateTime.Now + " : Fin de l'execution");
                    LogFile.WriteLine("Nombre de fichier scanner : " + nbr);
                    LogFile.Close();
                }
            goError:
                Console.WriteLine(DateTime.Now + " : Fin de l'execution");
                Console.WriteLine("Nombre de fichier scanner : " + nbr);


            Console.WriteLine("");
            Console.WriteLine("Cliquez Entrer pour sortir ..");

            Console.Read();

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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getClient(false, id), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client cli = new Client(reader[0].ToString(),reader[1].ToString(),reader[2].ToString(),reader[3].ToString(),reader[4].ToString(),reader[5].ToString(),reader[6].ToString(),reader[7].ToString(),reader[8].ToString(),reader[9].ToString(),reader[10].ToString(),reader[11].ToString(), reader[12].ToString());
                                connection.Close();
                                return cli;
                            }
                            else
                            {
                                Console.WriteLine(DateTime.Now + " : Erreur - Code Client " + id + " n'existe pas dans la base sage.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Code Client " + id + " n'existe pas dans la base sage.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[6]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    Console.Read();
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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getStockId(false), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //MessageBox.Show(reader[0].ToString());
                                string id=reader[0].ToString();
                                connection.Close();
                                return id;

                            }
                            else
                            {
                                Console.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Il n'y a pas de stock enregistré.");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(DateTime.Now + " : Erreur[5]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getNumLivraison(false, client_num), connection))
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
                                Console.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");
                                LogFile.WriteLine(DateTime.Now + " : Erreur - Numero de livraison n'existe pas pour le client " + client_num + ".");

                                return null;
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[4]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    Console.Read();
                    return null;
                }
            }

        }

        public static Boolean insertCommande(Client client,Order order)
        {
            // Insertion dans la base sage : cbase
            using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
                try
                {
                    connection.Open();

                    OdbcCommand command = new OdbcCommand(QueryHelper.insertCommande(false, client, order), connection);
                    command.ExecuteReader();
                    connection.Close();
                    return true;
                    

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    return false;
                }
            }

        }

        public static Boolean insertLigneCommande(Client client, Order order,OrderLine orderLine)
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
                            Console.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");
                            LogFile.WriteLine(DateTime.Now + " : Echec d'insertion de la ligne " + orderLine.NumLigne + " de la commande " + order.NumCommande + ".");
  
                            Console.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                            LogFile.WriteLine(DateTime.Now + " : Erreur[3]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                            return false;
                        }
                    

                    connection.Close();
                    return true;


            
            }

        }

        public static Article getArticle(string code_article, string CT_Num)
        {
            using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
            {
                try
                {
                    connection.Open();
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.getArticle(true, code_article, CT_Num), connection))
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Article article = new Article(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString());
                                connection.Close();
                                return article;

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
                    return null;
                }
            }

        }

        public static string getPath()
        {
            try
            {
                Classes.Path path = new Classes.Path();
                path.Load();
                return path.path;
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
                        Console.WriteLine("L'import des commandes Planifiée :");
                        Console.WriteLine("" + td.Triggers[0]);
                        return "" + td.Triggers[0];
                    }
                    else
                    {
                        Console.WriteLine("Il n'y a pas d'import planifiée enregistré.");
                        return null;
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " : Erreur[1] - " + e.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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


        public static string testGamme(int type, string code_article, string gamme)
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
                    Console.WriteLine(DateTime.Now + " : Erreur[18] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_NumPiece_Motif(false, num), connection))
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
                    Console.WriteLine(DateTime.Now + " : Erreur[23] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.MaxNumPiece(false), connection))
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
                    Console.WriteLine(DateTime.Now + " : Erreur[24] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
                Console.WriteLine(DateTime.Now + " : Erreur[25] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
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
                    using (OdbcCommand command = new OdbcCommand(QueryHelper.get_Next_NumPiece_BonCommande(false), connection))
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
                    Console.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    LogFile.WriteLine(DateTime.Now + " : Erreur[28] - " + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    return "erreur";
                }
            }

        }



        public static Boolean IsNumeric(string Nombre)
        {
            try
            {
                int.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
