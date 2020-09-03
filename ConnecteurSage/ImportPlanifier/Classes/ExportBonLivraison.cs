using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using importPlanifier.Utilities;
using System.Data;
using System.IO;
using Connexion;

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

        private List<DocumentVente> GetBonLivraisonFromDataBase(StreamWriter writer, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new, string statut)
        {
            writer.Flush();
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : Called!");
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();

                

                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    DocumentVente documentVente;
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    writer.Flush();
                    writer.WriteLine(DateTime.Now + " | GetBonLivraisonFromDataBase() : SQL ===> " + QueryHelper.getListDocumentVente_v2(false, 3, statut));
                    writer.Flush();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente_v2(false, 3, statut), connection);
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
                writer.Flush();
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
                return null;
            }
        }

        private List<DocumentVenteLine> getDocumentLine(StreamWriter writer, string codeDocument, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " | getDocumentLine() : récupérer les lignes du documment => " + codeDocument);
                List<DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    writer.WriteLine(DateTime.Now + " | getDocumentLine() : SQL ===> " + QueryHelper.getListDocumentVenteLine(true, codeDocument));
                    writer.Flush();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(true, codeDocument), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                writer.WriteLine(DateTime.Now + " | getDocumentLine() : Ref => " + reader[3].ToString() + " ; Name => " + reader[4].ToString());
                                writer.Flush();
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
                    writer.WriteLine(DateTime.Now + " | getDocumentLine() : ligne total => " + lignesDocumentVente.Count);
                    writer.Flush();
                    return lignesDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.WriteLine(DateTime.Now + " | getDocumentLine() : Erreur \n " + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                writer.Flush();
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
                return null;
            }
        }

        private Customer GetClient(StreamWriter writer, string do_tiers)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " | GetClient() : ref => "+ do_tiers);
                //List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    writer.WriteLine(DateTime.Now + " | GetClient() : SQL => " + QueryHelper.getCustomer(false, do_tiers));
                    OdbcCommand command = new OdbcCommand(QueryHelper.getCustomer(false, do_tiers), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                writer.WriteLine(DateTime.Now + " | GetClient() : Client trouvé!");
                                writer.Flush();
                                return new Customer(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(), reader[16].ToString());
                            }
                            else
                            {
                                writer.WriteLine(DateTime.Now + " | GetClient() : Aucun réponse.");
                                writer.Flush();
                                return null;
                            }
                        }
                    }

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                writer.WriteLine(DateTime.Now + " | GetClient() : ref => " + do_tiers);
                writer.Flush();
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
        public List<Alert_Mail.Classes.Custom.CustomMailRecapLines> ExportBonLivraisonAction(List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
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

                Config_Export.ConfigurationSaveLoad settings = new Config_Export.ConfigurationSaveLoad();
                try
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Répurère le statut dans la config export.");
                    settings.Load();
                }
                catch (Exception ex)
                {
                    logFileWriter_export.WriteLine(DateTime.Now + "******************** Erreur Chargement ********************");
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Erreur de chargement du fichier " + settings.getFilePath());
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Message => "+ex.Message);
                    logFileWriter_export.WriteLine("");
                    logFileWriter_export.Flush();
                }
                

                if(settings.configurationExport == null)
                {
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Erreur dans la récupération des configurations.");
                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Veuillez vérifier le fichier de configuration \""+settings.getFilePath()+"\".");
                    logFileWriter_export.Flush();
                    logFileWriter_export.Close();
                    return recapLinesList_new;
                }
                logFileWriter_export.Flush();

                List<DocumentVente> BonLivrasonAExporter = GetBonLivraisonFromDataBase(logFileWriter_export, recapLinesList_new, settings.configurationExport.DSADV.Status);

                logFileWriter_export.Flush();
                if (BonLivrasonAExporter != null && BonLivrasonAExporter.Count > 0)
                {
                     string outputFile = this.pathExport + @"\Fichier Exporter\Bons de Livraisons\Vente";

                     if (!Directory.Exists(outputFile))
                     {
                         System.IO.Directory.CreateDirectory(outputFile);
                     }

                    logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Répurère le format du fichier dans la config export.");

                    if (BonLivrasonAExporter.Count > 0 && settings.configurationExport.DSADV.Format == "Plat")
                    {
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Le format du fichier d'export => " + settings.configurationExport.DSADV.Format);
                        

                        for (int i = 0; i < BonLivrasonAExporter.Count; i++)
                        {
                            logFileWriter_export.Flush();
                            exportTo = @"Export\Plat_BonLivraison\Vente";
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Nombre de DESADV à exporter ===> " + (i+1) + "/" + BonLivrasonAExporter.Count);

                            Customer customer = GetClient(logFileWriter_export, BonLivrasonAExporter[i].DO_TIERS);

                            // Init.Init init = new Init.Init();
                            // logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Json BonLivrasonAExporter :\n " + init.FormatJson(BonLivrasonAExporter[i]));

                            var fileName = string.Format("BonLivraison{0:dd-MM-yyyy_HH.mm.ss}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

                            using (StreamWriter writer = new StreamWriter(outputFile + @"\" + fileName, false, Encoding.UTF8))
                            {
                                //writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                                //writer.WriteLine("");
                                //writer.WriteLine("");

                                writer.WriteLine("DESHDR;v01.0;;" + BonLivrasonAExporter[i].DO_Piece.Replace("BL", "") + ";" + customer.CT_EDI1 + ";9;;9;" + customer.CT_EDI1 + ";9;" + customer.CT_EDI1 + ";9;;9;;9;;9;" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";;;;;" + BonLivrasonAExporter[i].LI_ADRESSE + ";;;;;;;;;;;;;;9;");
                                writer.WriteLine("");

                                writer.WriteLine("DESHD2;;;;" + customer.CT_Adresse + ";;" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + customer.CT_Intitule + ";" + customer.CT_Telephone + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                                writer.WriteLine("");

                                //if (BonLivrasonAExporter[i].IntrastatTransportMode != "")
                                //{ // Return mode de transport                           
                                //    BonLivrasonAExporter[i].IntrastatTransportMode = GetModeTransport(BonLivrasonAExporter[i].IntrastatTransportMode);
                                //}

                                writer.WriteLine("DESTRP;;;;;;;;;;");
                                writer.WriteLine("");

                                writer.WriteLine("DESREF;;;;" + BonLivrasonAExporter[i].DO_COORD01 + ";;;;;");
                                writer.WriteLine("");
                                writer.Flush();

                                BonLivrasonAExporter[i].lines = getDocumentLine(logFileWriter_export, BonLivrasonAExporter[i].DO_Piece, recapLinesList_new);


                                writer.WriteLine("DESLOG;" + BonLivrasonAExporter[i].lines.Count + ";" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].FNT_PoidsNet.Replace(",", ".") + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                                writer.WriteLine("");
                                writer.Flush();

                                for (int j = 0; j < BonLivrasonAExporter[i].lines.Count; j++)
                                {
                                    writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[j].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[j].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[j].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[j].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[j].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[j].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;");
                                    writer.WriteLine("");
                                    writer.Flush();
                                }

                                writer.WriteLine("DESEND;" + BonLivrasonAExporter[i].lines.Count + ";;;" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;;;;");
                                writer.WriteLine("");
                                writer.WriteLine("");
                                writer.Flush();

                            }
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Fichier d'export DESADV généré dans : " + outputFile + @"\" + fileName.Replace("..", "."));
                            logFileWriter_export.Flush();

                            //add to backup folder
                            addFileToBackUp(pathExport + @"\BackUp\" + exportTo, outputFile + @"\" + fileName, fileName, logFileWriter_export);
                            logFileWriter_export.Flush();

                            //UpdateDocumentVente(BonLivrasonAExporter[i].DO_Piece, recapLinesList_new);

                            recapLinesList_new.AddRange(UpdateDocumentVente(logFileWriter_export, BonLivrasonAExporter[i].DO_Piece, outputFile + @"\" + fileName));

                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Mettre à jour le Document de Vente");
                            logFileWriter_export.WriteLine("");
                            logFileWriter_export.WriteLine("");
                            logFileWriter_export.Flush();


                        }  //END FOR export Bon de livraison


                        Console.WriteLine(DateTime.Now + " : Nombre bon de livraison : " + BonLivrasonAExporter.Count);
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Nombre bon de livraison : " + BonLivrasonAExporter.Count);

                    }
                    else if (BonLivrasonAExporter.Count > 0 && settings.configurationExport.DSADV.Format == "Véolog")
                    {
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Le format du fichier d'export => " + settings.configurationExport.DSADV.Format);
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Aucun format développé.");

                    }
                    else
                    {
                        logFileWriter_export.Flush();

                        if (BonLivrasonAExporter.Count == 0)
                        {
                            logFileWriter_export.WriteLine(DateTime.Now + "******************** Attention BL ********************");
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Aucun BL a exporter.");
                            logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : BonLivrasonAExporter.Count => 0");
                            logFileWriter_export.WriteLine("");
                            logFileWriter_export.Flush();
                        }
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Le format du fichier d'export => " + settings.configurationExport.DSADV.Format);
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Pas de format disponible. Veuillez vérifier le format dans le fichier de configuration \"" + settings.getFilePath());
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Sous la section \"DSADV\" => Format");
                        logFileWriter_export.Flush();
                    }

                    
                }
                else
                {
                    if (BonLivrasonAExporter == null)
                    {
                        logFileWriter_export.WriteLine(DateTime.Now + "******************** Erreur Export BL ********************");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Erreur dans la récupération des BL");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Export Annulé.");
                        logFileWriter_export.Flush();
                    }
                    else if (BonLivrasonAExporter.Count == 0)
                    {
                        logFileWriter_export.WriteLine(DateTime.Now + "******************** Attention BL ********************");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Aucun BL a exporter.");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : BonLivrasonAExporter.Count => 0");
                        logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Export Annulé.");
                        logFileWriter_export.Flush();
                    }

                }


            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                Console.WriteLine("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));

                logFileWriter_export.WriteLine(DateTime.Now + "********************************* Exception *********************************");
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Message :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportBonLivraisonAction() : Export annullé");
                logFileWriter_export.Close();
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du bon de livraison est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
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
                try
                {
                    writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Copy file \"" + sourceFilePath + "\" to \"" + backUpFolderPath + @"\" + filename + "\"");
                    File.Copy(sourceFilePath, backUpFolderPath + @"\" + filename);
                }
                catch(Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " | addFileToBackUp() : ########### ERROR FILE Copy ###########");
                    writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Message => "+ex.Message);
                }
            }

            writer.WriteLine("");
            writer.Flush();
        }

        private void UpdateDocumentVente(string do_piece, List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
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
                recapLinesList_new.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export du bon de livraison est annulée.", e.Message, e.StackTrace, "", logFileName_export));
            }
        }

        private List<Alert_Mail.Classes.Custom.CustomMailRecapLines> UpdateDocumentVente(StreamWriter writer, string do_piece, string fileLocation)
        {
            using (OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL())
            {
                List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_ = new List<Alert_Mail.Classes.Custom.CustomMailRecapLines>();
                connexion.Open();

                //update order statut
                try
                {
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " | ExportCommande() : Changer le statut de la commande \"" + do_piece + "\".");

                    writer.WriteLine(DateTime.Now + " | ExportCommande() : SQL ===> " + QueryHelper.changeBLStatut(true, do_piece));
                    OdbcCommand command1 = new OdbcCommand(QueryHelper.changeBLStatut(true, do_piece), connexion);
                    {
                        using (IDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Statut Update
                            }
                        }
                    }
                    writer.WriteLine(DateTime.Now + " Le statut du documment \" " + fileLocation + " \" est à jour.");
                    writer.Flush();

                }
                catch (Exception ex)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " ********** Erreur ********** ");

                    if (File.Exists(fileLocation))
                    {
                        try
                        {
                            File.Delete(fileLocation);
                            writer.WriteLine(DateTime.Now + " Le fichier \" " + fileLocation + " \" est supprimer !.");
                        }
                        catch (Exception exf)
                        {
                            writer.WriteLine(DateTime.Now + " ********** Erreur Delete File ********** ");
                            writer.WriteLine(DateTime.Now + " Impossible de supprimer le fichier \" " + fileLocation + " \".");
                            writer.WriteLine(DateTime.Now + " Message: " + exf.Message);
                        }
                    }
                    else
                    {
                        writer.WriteLine(DateTime.Now + " Le fichier \" " + do_piece + " \" n'existe plus (peut déjà être envoyé en EDI).");
                    }
                    writer.Flush();

                    writer.WriteLine(DateTime.Now + " Message: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                    writer.WriteLine(DateTime.Now + " Export Annuler.");
                    writer.Flush();

                    if (ex.Message.Contains("Cet élément est en cours d'utilisation !"))
                    {
                        writer.WriteLine("");
                        writer.WriteLine(DateTime.Now + " | ExportCommande() : Cet élément est en cours d'utilisation ! Impossible de changer le statut de la commande \"" + do_piece + "\".");
                        recapLinesList_.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export de la commande est annulée. Cet élément est en cours d'utilisation ! Veuillez fermer la fenêtre de commande dans Sage afin que la commande puisse être exportée.", "Cet élément est en cours d'utilisation ! Impossible de changer le statut de la commande \"" + do_piece + "\".", ex.StackTrace, "", logFileName_export));
                    }
                    else
                    {
                        writer.WriteLine("");
                        writer.WriteLine(DateTime.Now + " | ExportCommande() : " + ex.Message);
                        recapLinesList_.Add(new Alert_Mail.Classes.Custom.CustomMailRecapLines(docRefMail, "", "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                    }
                    writer.Flush();
                    //recapLinesList_new.Add(new CustomMailRecapLines(docRefMail, "L'export de la commande est annulée.", ex.Message, ex.StackTrace, "", logFileName_export));
                }
                writer.WriteLine("");
                writer.Flush();
                return recapLinesList_;
            }
            
        }
    }
}
