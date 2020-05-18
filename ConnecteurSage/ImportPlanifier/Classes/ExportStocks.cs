using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using importPlanifier.Utilities;
using System.Data.Odbc;
using System.Data;
using System.IO;
using Connexion;

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
        private string docRefMail = "";
        private string logFileName_export;
        public string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "STOCK";
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
            using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
            {
                connection.Open();
                //Exécution de la requête permettant de récupérer les articles du dossier
                OdbcCommand command = new OdbcCommand(QueryHelper.getListCommandes(false), connection);
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
        public List<Alert_Mail.Classes.Custom.CustomMailRecapLines> ExportStock(List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            if (!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Stock_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_export = File.Create(logFileName_export);

            //Write in the log file 
            logFileWriter_export = new StreamWriter(logFile_export);
            //logFileWriter.Write(string.Format("{0:HH:mm:ss}", DateTime.Now) + " \r\n");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("");

            string exportTo = "";
            string exportStockPath = pathExport + @"\Export_Veolog";

            if (!Directory.Exists(exportStockPath))
            {
                Directory.CreateDirectory(exportStockPath);
            }

            logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Path Export ==> "+ exportStockPath);

            try
            {
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Export Stock.");

                if (string.IsNullOrEmpty(exportStockPath)) //check if the seleted path is empty
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Le chemin pour l'export du fichier stock liste doit être renseigné !");
                    logFileWriter_export.Flush();
                    logFileWriter_export.Close();
                    recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du stock est annulée.", "Le chemin pour l'export du fichier stock liste doit être renseigné !", "", "", logFileName_export));
                    return recapLinesList_new;
                }

                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Récupérer le stock de tous les produits et leur stock.");

                List<Stock> s = new List<Stock>(); //creating list type stock
                s = GetStockArticle(logFileWriter_export, recapLinesList_new); //call function GetStockArticle to get all the products and their stock

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

                    string fileName_ = string.Format("stock_{0:yyMMddHHmmss}.csv", DateTime.Now); //file.
                    string fileName = exportStockPath + @"\" + fileName_; //creating the file.

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

                        //export veolog
                        exportTo = @"Export\Veolog_Stock";
                    }

                    // *file has been generated at the end of the method using @fileName*

                    /*string myFileData = File.ReadAllText(fileName); //get all content of the created file (need to fix)
                    if (myFileData.EndsWith(Environment.NewLine)) //check if at the end of the has empty return/jump character
                    {
                        File.WriteAllText(@"D:\test_backup.csv", myFileData.TrimEnd(Environment.NewLine.ToCharArray()) ); //remove jump at the end of the file
                    }*/

                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : Le fichier a été généré à : " + fileName);

                    //add to backup folder
                    addFileToBackUp(pathExport + @"\BackUp\" + exportTo, pathExport + @"\" + fileName, fileName_, logFileWriter_export);
                }

            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportStock() : ERREUR :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter_export.Flush();
                logFileWriter_export.Close();
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du stock est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
            }

            logFileWriter_export.Flush();
            logFileWriter_export.Close();
            Console.WriteLine(DateTime.Now + " | ExportCommande() : Close.");
            return recapLinesList_new;
        }

        public static void addFileToBackUp(string backUpFolderPath, string sourceFilePath, string filename, StreamWriter writer)
        {
            writer.WriteLine("");
            //check if the backup folder exist
            if (!Directory.Exists(backUpFolderPath))
            {
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Create BackUp folder at \"" + backUpFolderPath + "\"");
                Directory.CreateDirectory(backUpFolderPath);
            }

            //copy the file to the backup folder
            if (File.Exists(backUpFolderPath + @"\" + filename))
            {
                int version = 0;
                //Get all .csv files in the folder
                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : File \"" + backUpFolderPath + @"\" + filename + "\" exist so add version it");

                for (int x = 0; x < fileListing.GetFiles("*.csv").Length; x++)
                {
                    string[] cutFileName = filename.Split('_');
                    string withouExtension = cutFileName[3].Split('.')[0];
                    string newFileName = cutFileName[0] + "_" + cutFileName[1] + "_" + cutFileName[2] + "_" + withouExtension;
                    FileInfo Filename = fileListing.GetFiles("*.csv")[x];

                    if ((Filename.Name).Contains(newFileName))
                    {
                        version++;
                        writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Version: " + version + " || (" + Filename.Name + ").Contains(" + newFileName + ")");
                    }
                }
                //File.Delete(destFilePath);
                string[] cutFileName_1 = filename.Split('.');
                string newFileName_1 = cutFileName_1[0] + "_v" + version + "." + cutFileName_1[1];
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Copy file \"" + sourceFilePath + "\" to \"" + backUpFolderPath + @"\" + newFileName_1 + "\"");
                File.Copy(sourceFilePath, backUpFolderPath + @"\" + newFileName_1);
            }
            else
            {
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Copy file \"" + sourceFilePath + "\" to \"" + backUpFolderPath + @"\" + filename + "\"");
                File.Copy(sourceFilePath, backUpFolderPath + @"\" + filename);
            }

            writer.WriteLine("");
            writer.Flush();
        }

        private List<Stock> GetStockArticle(StreamWriter logFileWriter, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            try
            {
                List<Stock> stock_info = new List<Stock>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL())
                {
                    connection.Open();//connecting as handler with database

                    logFileWriter.WriteLine(DateTime.Now + " : GetStockArticle | SQL: " + QueryHelper.getStockInfo(true));

                    OdbcCommand command = new OdbcCommand(QueryHelper.getStockInfo(true), connection);//Exécution de la requête permettant de récupérer les articles du dossier
                    {
                        using (IDataReader reader = command.ExecuteReader()) //reading lines fetched.
                        {
                            while (reader.Read())
                            {
                                Stock stock = new Stock(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), "", "", "");
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
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du stock est annulée.", e.Message, e.StackTrace, "", logFileName_export));
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
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(false, code), connection);
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
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    List<OrderLine> lines = new List<OrderLine>();

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListLignesCommandes(false, code), connection);
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
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.updateDocumentdeVente(false, do_piece), connection);
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
