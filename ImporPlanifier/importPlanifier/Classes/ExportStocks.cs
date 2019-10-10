﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Helpers;
using System.Data.Odbc;
using System.Data;
using System.IO;

namespace importPlanifier.Classes
{
    class ExportStocks
    {
        #region Champs privés
        /// <summary>
        /// commande à exporter
        /// </summary>
        //private Order CommandeAExporter;

        private string pathExport;

        private string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "STOCK";
        private StreamWriter logFileWriter_export = null;

        #endregion


        public ExportStocks(string path)
        {
            this.pathExport = path;
        }

        #region Intéractions avec l'application

        private List<Order> GetCommandesFromDataBase()
        {
            try
            {
            //DocumentVente Facture = new DocumentVente();
            List<Order> listCommande = new List<Order>();
             using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
               
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListCommandes(), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                           while(reader.Read())
                            {
                                Order order = new Order(reader[0].ToString(), reader[1].ToString(), 
                                    reader[2].ToString().Replace(", ",",")+"."+reader[3].ToString()+"."+reader[6].ToString()+"."+reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString().Replace("00:00:00",""),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[15].ToString(),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[0] : null),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[1] : null),
                                    reader[16].ToString()
                                    );
                                listCommande.Add(order);
                            }
                        }
                    }
                    return listCommande;

            }

                                 }

                catch (Exception e)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    Console.WriteLine(e.Message);
                    return null;
                }
        }






        /// <summary>
        /// Génération du fichier d'import, lancement de l'application et import des commandes
        /// </summary>
        /// 
        // Need to finish 
        public void ExportStock()
        {
            if (!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Stock_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_export = File.Create(logFileName_export);

            //Write in the log file 
            logFileWriter_export = new StreamWriter(logFile_export);
            //logFileWriter.Write(string.Format("{0:HH:mm:ss}", DateTime.Now) + " \r\n");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("");

            string exportStockPath = pathExport;

            logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Path Export ==> "+ exportStockPath);

            try
            {
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Export Stock.");

                if (string.IsNullOrEmpty(exportStockPath)) //check if the seleted path is empty
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Le chemin pour l'export du fichier stock liste doit être renseigné !");
                    logFileWriter_export.Close();
                    return;
                }

                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Récupérer le stock de tous les produits et leur stock.");

                List<Stock> s = new List<Stock>(); //creating list type stock
                s = GetStockArticle(logFileWriter_export); //call function GetStockArticle to get all the products and their stock

                //testing purpose only :begin
                /*Stock s1 = new Stock("product 1", "PROD1", "1234567891234", "59", "LOT-BDF9411123", "5.00000", "0");
                Stock s2 = new Stock("product 2", "PROD2", "4321987654321", "15", "MV32", "1.0000", "1");
                s.Add(s1);
                s.Add(s2);*/
                //testing purpose only :end

                if (s == null) //check if the list is empty or not
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Failed to obtain value from database : (Maybe failed to connect with database) ");
                }
                else
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Nombre de Stock récupéré : " + s.Count);

                    string[] stocklines = new string[s.Count]; //creating array to add output lines for file
                    int i = 0;
                    foreach (Stock stockline in s) //reading line per line from the list
                    {
                        stocklines[i] = "L;" + stockline.reference + ";" + stockline.codebarre + ";" + stockline.stock + ";" + stockline.numerolot + ";" + stockline.lotqty + ";" + stockline.lotepuise + ";" + (i + 1); //adding lines into array for file
                        i++; //increment for further adding/reading into the array
                    }

                    string fileName = string.Format(exportStockPath + @"\" + "stock_{0:yyMMddHHmmss}.csv", DateTime.Now); //creating the file.

                    if (File.Exists(fileName)) //verifying if the file exists else delete and recreate
                    {
                        File.Delete(fileName); //delete file.
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName)) // streaming the file 
                    {
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Écrire dans le fichier à : " + fileName);

                        foreach (string line in stocklines) //reading line per line from array
                        {
                            file.WriteLine(line); //writing inside the file
                        }
                        file.WriteLine("F" + ";" + i); //writing at the end of file
                    }

                    // *file has been generated at the end of the method using @fileName*

                    /*string myFileData = File.ReadAllText(fileName); //get all content of the created file (need to fix)
                    if (myFileData.EndsWith(Environment.NewLine)) //check if at the end of the has empty return/jump character
                    {
                        File.WriteAllText(@"D:\test_backup.csv", myFileData.TrimEnd(Environment.NewLine.ToCharArray()) ); //remove jump at the end of the file
                    }*/

                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Le fichier a été généré à : " + fileName);
                }

            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : ERREUR :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
            }

            logFileWriter_export.Close();

            /*
            try
            {
                if (!Directory.Exists(logDirectoryName_export))
                {
                    Directory.CreateDirectory(logDirectoryName_export);
                }

                var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Commande_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
                var logFile_export = File.Create(logFileName_export);
                logFileWriter_export = new StreamWriter(logFile_export);

                logFileWriter_export.WriteLine("#####################################################################################");
                logFileWriter_export.WriteLine("################################# Import Planifier ##################################");
                logFileWriter_export.WriteLine("#####################################################################################");
                logFileWriter_export.WriteLine("");

                List<Order> CommandeAExporter = GetCommandesFromDataBase();

                string outputFile = this.pathExport + @"\Fichier Exporter\Export Bons de commandes\";

                if (!Directory.Exists(outputFile))
                {
                    System.IO.Directory.CreateDirectory(outputFile);
                }

                for(int i=0;i<CommandeAExporter.Count;i++)
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Nombre de commande à exporter ===> " + i + "/" + CommandeAExporter.Count);

                    if (CommandeAExporter[i].deviseCommande == "0")
                    {
                        CommandeAExporter[i].deviseCommande = "1";
                    }

                    if (CommandeAExporter[i].deviseCommande != "")
                    {
                        CommandeAExporter[i].deviseCommande = getDeviseIso(CommandeAExporter[i].deviseCommande);
                    }

                    //if (CommandeAExporter[i].DO_MOTIF == "")
                    //{
                    //    CommandeAExporter[i].DO_MOTIF = CommandeAExporter[i].NumCommande;
                    //}

                    //if (CommandeAExporter[i].DO_MOTIF == "")
                    //{
                    //    DialogResult resultDialog = Console.WriteLine("N° de commande non enregistrer.\nVoulez vous Continuer ?");

                    //    if (resultDialog == DialogResult.Cancel)
                    //    {
                    //        goto jamp;
                    //    }

                    //    if (resultDialog == DialogResult.OK)
                    //    {
                    //        CommandeAExporter[i].DO_MOTIF = "";
                    //    }

                    //}

                    //if (CommandeAExporter[i].codeClient == "")
                    //{
                    //    DialogResult resultDialog = Console.WriteLine("Code GNL client n'est pas enregistrer.\nVoulez vous continuer ?");

                    //    if (resultDialog == DialogResult.Cancel)
                    //    {
                    //        goto jamp;
                    //    }

                    //    if (resultDialog == DialogResult.OK)
                    //    {
                    //        CommandeAExporter.codeClient = "";
                    //    }

                    //}

                    if (!IsNumeric(CommandeAExporter[i].DO_MOTIF) && CommandeAExporter[i].DO_MOTIF != "")
                    {
                        //DialogResult resultDialog = Console.WriteLine("N° de commande est mal enregistrer.\nVoulez vous Continuer ?");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : N° de commande est mal enregistrer");

                        //if (resultDialog == DialogResult.Cancel)
                        //{
                        //    goto jamp;
                        //}

                        //if (resultDialog == DialogResult.OK)
                        //{
                        CommandeAExporter[i].DO_MOTIF = "";
                        //}

                    }

                    //if (!IsNumeric(CommandeAExporter[i].codeClient) && CommandeAExporter[i].codeClient != "")
                    //{
                    //    DialogResult resultDialog = Console.WriteLine("Code GNL client est mal enregistrer.\nVoulez vous continuer ?");

                    //    if (resultDialog == DialogResult.Cancel)
                    //    {
                    //        goto jamp;
                    //    }

                    //    if (resultDialog == DialogResult.OK)
                    //    {
                    //        CommandeAExporter[i].DO_MOTIF = "";
                    //    }

                    //}


                    var fileName = string.Format("EDI_ORDERS." + CommandeAExporter[i].codeClient + "." + CommandeAExporter[i].NumCommande + "." + ConvertDate(CommandeAExporter[i].DateCommande) + "."+ CommandeAExporter[i].adresseLivraison + ".{0:yyyyMMddhhmmss}.csv", DateTime.Now);

                    fileName = fileName.Replace("...", ".");

                    using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName.Replace("..", "."), false, Encoding.UTF8))
                    {

                        writer.WriteLine("ORDERS;" + CommandeAExporter[i].DO_MOTIF + ";" + CommandeAExporter[i].codeClient + ";" + CommandeAExporter[i].codeAcheteur + ";" + CommandeAExporter[i].codeFournisseur + ";;;" + CommandeAExporter[i].nom_contact + "." + CommandeAExporter[i].adresseLivraison.Replace("..", ".").Replace("...", ".") + ";" + CommandeAExporter[i].deviseCommande + ";;");
                    
                        if (CommandeAExporter[i].DateCommande != "")
                        {
                            CommandeAExporter[i].DateCommande = ConvertDate(CommandeAExporter[i].DateCommande);
                        }

                        //if (CommandeAExporter[i].DateCommande != " ")
                        //{
                        //    CommandeAExporter[i].conditionLivraison = "";
                        //}

                        writer.WriteLine("ORDHD1;" + CommandeAExporter[i].DateCommande + ";" + CommandeAExporter[i].conditionLivraison + ";;");

                        CommandeAExporter[i].Lines = getLigneCommande(CommandeAExporter[i].NumCommande);

                        for (int j = 0; j < CommandeAExporter[i].Lines.Count;j++ )
                        {
                            if (!IsNumeric(CommandeAExporter[i].Lines[j].codeAcheteur))
                            {
                                CommandeAExporter[i].Lines[j].codeAcheteur = "";
                            }

                            if (!IsNumeric(CommandeAExporter[i].Lines[j].codeFournis))
                            {
                                CommandeAExporter[i].Lines[j].codeFournis = "";
                            }

                            writer.WriteLine("ORDLIN;" + CommandeAExporter[i].Lines[j].NumLigne + ";" + CommandeAExporter[i].Lines[j].codeArticle + ";GS1;" + CommandeAExporter[i].Lines[j].codeAcheteur + ";" + CommandeAExporter[i].Lines[j].codeFournis + ";;A;" + CommandeAExporter[i].Lines[j].descriptionArticle + ";" + CommandeAExporter[i].Lines[j].Quantite.Replace(",", ".") + ";LM;" + CommandeAExporter[i].Lines[j].MontantLigne.Replace(",", ".") + ";;;" + CommandeAExporter[i].Lines[j].PrixNetHT.Replace(",", ".") + ";;;LM;;;;" + ConvertDate(CommandeAExporter[i].Lines[j].DateLivraison) + ";");
                        }
                        writer.WriteLine("ORDEND;" + CommandeAExporter[i].MontantTotal.ToString().Replace(",", ".") + ";");

                    }
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Fichier d'export commande généré dans : " + outputFile + @"\" + fileName.Replace("..", "."));

                    UpdateDocumentVente(CommandeAExporter[i].NumCommande);
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Mettre à jour le Document de Vente");
                }

                Console.WriteLine(DateTime.Now + " : Nombre de commande exporté : " + CommandeAExporter.Count);
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Nombre de commande exporté : " + CommandeAExporter.Count);

            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                Console.WriteLine(ex.Message);

                logFileWriter_export.WriteLine(DateTime.Now + "********************************* Exception *********************************");
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Message :: " + ex.Message);
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportCommande() : Export annullé");
                logFileWriter_export.Close();
            }
            */
        }

        private List<Stock> GetStockArticle(StreamWriter logFileWriter)
        {
            try
            {
                List<Stock> stock_info = new List<Stock>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    connection.Open();//connecting as handler with database

                    logFileWriter.WriteLine(DateTime.Now + " : GetStockArticle | SQL: " + QueryHelper.getStockInfo());

                    OdbcCommand command = new OdbcCommand(QueryHelper.getStockInfo(), connection);//Exécution de la requête permettant de récupérer les articles du dossier
                    {
                        using (IDataReader reader = command.ExecuteReader()) //reading lines fetched.
                        {
                            while (reader.Read())
                            {
                                Stock stock = new Stock(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
                                stock_info.Add(stock);
                            }
                        }
                    }
                    return stock_info;
                }
            }
            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                logFileWriter.WriteLine(DateTime.Now + " : GetStockArticle | " + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }
        #endregion

        #region Méthodes diverses
        /// <summary>
        /// Chargement de la fenêtre
        /// </summary>
        /// <param name="e">paramètres de l'évènement</param>


        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }


        #endregion


        private string getDeviseIso(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                        }
                    }
                    return null;

                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

        private List<OrderLine> getLigneCommande(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    List<OrderLine> lines = new List<OrderLine>();

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListLignesCommandes(code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lines.Add(new OrderLine(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                            }

                            return lines;
                        }
                    }
                    return null;

                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
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

        private void UpdateDocumentVente(string do_piece)
        {
            try
            {
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.updateDocumentdeVente(do_piece), connection);
                    command.ExecuteNonQuery();
                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
            }
        }
    }
}
