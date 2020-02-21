using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using importPlanifier.Helpers;
using System.Data;
using System.IO;
using ImportPlanifier.Classes;

namespace importPlanifier.Classes
{
    class ExportBonLivraison
    {
        //private List<DocumentVente> BonLivrasonAExporter;
        //private Customer customer = new Customer();
        private string pathExport;
        private string docRefMail = "";
        private string logFileName_export;
        public string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "BON_LIVRAISON";
        private StreamWriter logFileWriter_export = null;

        public ExportBonLivraison(string path)
        {
            this.pathExport = path;
        }

        private List<DocumentVente> GetBonLivraisonFromDataBase(StreamWriter writer, List<CustomMailRecapLines> recapLinesList_new)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : Called!");
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();

                ConfigurationExport export = new ConfigurationExport();
                writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : Répurère le statut dans la config export.");
                export.Load();

                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    DocumentVente documentVente;
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : SQL ===> " + QueryHelper.getListDocumentVente(false, 3, export.exportBonsLivraisons_Statut));
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(false, 3, export.exportBonsLivraisons_Statut), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                documentVente = new DocumentVente(reader[0].ToString(), reader[1].ToString(),
                                    reader[2].ToString().Replace("00:00:00", ""), reader[3].ToString().Replace("00:00:00", ""), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(),
                                     reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString(),
                                     reader[32].ToString(), reader[33].ToString()
                                    );
                                if (documentVente.DO_Statut == "0")
                                {
                                    documentVente.DO_Statut = "Saisie";
                                }
                                if (documentVente.DO_Statut == "1")
                                {
                                    documentVente.DO_Statut = "Confirmé";
                                }
                                if (documentVente.DO_Statut == "2")
                                {
                                    documentVente.DO_Statut = "Accepté";
                                }
                                listDocumentVente.Add(documentVente);
                            }
                        }
                    }
                    writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : Numéro de facture trouvé : " + listDocumentVente.Count());
                    return listDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : " + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
                return null;
            }
        }

        private List<DocumentVenteLine> getDocumentLine(string codeDocument, List<CustomMailRecapLines> recapLinesList_new)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(false, codeDocument), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocumentVenteLine ligne = new DocumentVenteLine(reader[0].ToString().Replace("00:00:00", ""), reader[1].ToString().Replace("00:00:00", ""),
                                    reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                    reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(),
                                    reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString(),
                                    reader[32].ToString(), reader[33].ToString(), reader[34].ToString()
                                    );
                                lignesDocumentVente.Add(ligne);
                            }
                        }
                    }
                    return lignesDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
                return null;
            }
        }

        private Customer GetClient(string do_tiers)
        {
            try
            {
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getCustomer(false, do_tiers), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(), reader[16].ToString());
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                return null;
            }
        }

       

      
        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }



        public static string addZero(string date)
        {
            if (date.Length == 1)
            {
                return "0" + date;
            }
            return date;
        }

        /// <summary>
        /// Génération du fichier d'export, lancement de l'application et exporter les factures
        /// </summary>
        public List<CustomMailRecapLines> ExportBonLivraisonAction(List<CustomMailRecapLines> recapLinesList_new)
        {
            try
            {
                string exportTo = "";

                if (!Directory.Exists(logDirectoryName_export))
                {
                    Directory.CreateDirectory(logDirectoryName_export);
                }

                logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_BonLivraison_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
                var logFile_export = File.Create(logFileName_export);
                logFileWriter_export = new StreamWriter(logFile_export);

                logFileWriter_export.WriteLine("#####################################################################################");
                logFileWriter_export.WriteLine("################################# Import Planifier ##################################");
                logFileWriter_export.WriteLine("#####################################################################################");
                logFileWriter_export.WriteLine("");

                List<DocumentVente> BonLivrasonAExporter = GetBonLivraisonFromDataBase(logFileWriter_export, recapLinesList_new);

                 if (BonLivrasonAExporter != null)
                 {
                     string outputFile = this.pathExport + @"\Fichier Exporter\Bons de Livraisons\";

                     if (!Directory.Exists(outputFile))
                     {
                         System.IO.Directory.CreateDirectory(outputFile);
                     }

                    ConfigurationExport export = new ConfigurationExport();
                    logFileWriter_export.WriteLine(DateTime.Now + " | GetFacturesFromDataBase() : Répurère le format du fichier dans la config export.");
                    export.Load();

                    for (int i = 0; i < BonLivrasonAExporter.Count; i++)
                    {
                        if (export.exportFactures_Format == "Plat")
                        {
                            exportTo = @"Export\Plat_BonLivraison";
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Nombre de DESADV à exporter ===> " + i + "/" + BonLivrasonAExporter.Count);

                            Customer customer = GetClient(BonLivrasonAExporter[i].DO_TIERS);

                            var fileName = string.Format("BonLivraison{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

                            using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName, false, Encoding.UTF8))
                            {
                                //writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                                //writer.WriteLine("");
                                //writer.WriteLine("");

                                writer.WriteLine("DESHDR;v01.0;;" + BonLivrasonAExporter[i].DO_Piece.Replace("BL", "") + ";" + customer.CT_EDI1 + ";9;;9;" + customer.CT_EDI1 + ";9;" + customer.CT_EDI1 + ";9;;9;;9;;9;;" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";;;;;" + BonLivrasonAExporter[i].LI_ADRESSE + ";;;;;;;;;;;;;;9;");
                                writer.WriteLine("");

                                writer.WriteLine("DESHD2;;;;" + customer.CT_Adresse + ";;" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + customer.CT_Intitule + ";" + customer.CT_Telephone + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                                writer.WriteLine("");

                                //if (BonLivrasonAExporter[i].IntrastatTransportMode != "")
                                //{ // Return mode de transport                           
                                //    BonLivrasonAExporter[i].IntrastatTransportMode = GetModeTransport(BonLivrasonAExporter[i].IntrastatTransportMode);
                                //}

                                writer.WriteLine("DESTRP;;;;;;;;;;");
                                writer.WriteLine("");

                                writer.WriteLine("DESLOG;;;;" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].FNT_PoidsNet.Replace(",", ".") + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                                writer.WriteLine("");


                                BonLivrasonAExporter[i].lines = getDocumentLine(BonLivrasonAExporter[i].DO_Piece, recapLinesList_new);

                                for (int j = 0; j < BonLivrasonAExporter[i].lines.Count; j++)
                                {
                                    writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[j].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[j].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[j].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[j].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[j].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[j].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;");
                                    writer.WriteLine("");
                                }

                                writer.WriteLine("DESEND;" + BonLivrasonAExporter[i].lines.Count + ";;;" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;;;;");
                                writer.WriteLine("");
                                writer.WriteLine("");

                            }
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Fichier d'export DESADV généré dans : " + outputFile + @"\" + fileName.Replace("..", "."));

                            //add to backup folder
                            addFileToBackUp(pathExport + @"\BackUp\" + exportTo, pathExport + @"\" + fileName, fileName, logFileWriter_export);

                            UpdateDocumentVente(BonLivrasonAExporter[i].DO_Piece, recapLinesList_new);

                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Mettre à jour le Document de Vente");

                        }
                        else
                        {
                            logFileWriter_export.WriteLine(DateTime.Now + "******************** Erreur Format Fichier ********************");
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Le format \"" + export.exportFactures_Format + "\" n'existe pas dans le connecteur!");
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Vérifi le fichier de configuration \"" + Directory.GetCurrentDirectory() + @"\SettingExport.xml" + "\" à l'argument exportFactures_Format.");
                            logFileWriter_export.Flush();
                            recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export du bon de livraison est annulée.", "Le format \"" + export.exportFactures_Format + "\" n'existe pas dans le connecteur!", "", "", logFileName_export));
                            return recapLinesList_new;
                        }


                    }  //END FOR export Bon de livraison

                     Console.WriteLine(DateTime.Now + " : Nombre bon de livraison : " + BonLivrasonAExporter.Count);
                     logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Nombre bon de livraison : " + BonLivrasonAExporter.Count);
                 }


            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                logFileWriter_export.WriteLine(DateTime.Now + "********************************* Exception *********************************");
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Message :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraison() : Export annullé");
                logFileWriter_export.Close();
                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export du bon de livraison est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
            }

            logFileWriter_export.Close();
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

        private void UpdateDocumentVente(string do_piece, List<CustomMailRecapLines> recapLinesList_new)
        {
            try
            {
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
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
                recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
            }
        }
    }
}
