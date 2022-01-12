using importPlanifier.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fichier_De_Nettoyage
{
    public class FichierDeNettoyage
    {
        private string fileName = null;
        string[,] paths = null;

        public FichierDeNettoyage()
        {
            Database.Database db = new Database.Database();
            this.fileName = db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\SettingBackup.xml";

            Database.Model.Settings db_settings_ = db.settingsManager.get(db.connectionString, 1);
            this.paths = new string[12, 2] {
                { "general_logs", db_settings_.EXE_Folder + @"\LOG"}, //log files
                { "general_logs", db_settings_.EXE_Folder + @"\LOG\LOG_AlertMail"}, //log files
                { "import_logs", db_settings_.EXE_Folder + @"\LOG\LOG_Import" }, //log files
                { "export_Logs", db_settings_.EXE_Folder + @"\LOG\LOG_Export\FACTURE" }, //log files
                { "export_Logs", db_settings_.EXE_Folder + @"\LOG\LOG_Export\BON_LIVRAISON" }, //log files
                { "export_Logs", db_settings_.EXE_Folder + @"\LOG\LOG_Export\COMMANDE" }, //log files
                { "export_Logs", db_settings_.EXE_Folder + @"\LOG\LOG_Export\STOCK" }, //log files
                { "import_files_success", db_settings_.EXE_Folder + @"\Success File" }, //fichier import success
                { "import_files_error", db_settings_.EXE_Folder + @"\Error File" }, //fichier import erreur
                { "backup_files", db_settings_.EDI_Folder + @"\BackUp\Export\Orders" }, //backup files
                { "backup_files", db_settings_.EDI_Folder + @"\BackUp\Export\Article" }, //backup files
                { "backup_files", db_settings_.EDI_Folder + @"\BackUp\Export\Client" }, //backup files
            };
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// With StreamWriter
        /// 
        #region With StreamWriter
        private bool checkConfig(StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles()");

            if (File.Exists(this.fileName))
            {
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres trouvé => " + this.fileName);
                return true;
            }
            else
            {
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres pas trouvé => " + this.fileName);
                return false;
            }
        }

        public void startClean(StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine("################################ Nettoyage De Fichier ###############################");
            writer.WriteLine("");

            try
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Cleaning Files");
                writer.Flush();

                // cleanFiles(writer, paths);
                cleanFiles(writer, this.paths);

                writer.WriteLine("");
                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("###### cleanFiles ######");
                Console.WriteLine(ex.Message);
                writer.WriteLine("********************* Fichier-De-Nettoyage.dll Erreur ********************");
                writer.WriteLine("" + ex.Message);
                writer.WriteLine("" + ex.StackTrace);
                writer.Flush();
            }
        }

        private void cleanFiles(StreamWriter writer, string[,] directoriesList)
        {
            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles()");

            if (this.checkConfig(writer))
            {
                ConfigurationBackup configBackup = new ConfigurationBackup();
                configBackup.Load();
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres de nettoyage trouvés et chargés.");

                if (configBackup.activate)
                {
                    Console.WriteLine("Cleaning settings are activated !");
                    writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Paramètres de nettoyage activé.");
                    writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + directoriesList.GetLength(0) + " dossiers à nettoyer :");
                    writer.Flush();

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Type : " + directoriesList[x, 0] + " Répertoire : " + directoriesList[x, 1]);
                        writer.Flush();
                    }

                    writer.WriteLine("");
                    writer.Flush();

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        switch(directoriesList[x, 0])
                        {
                            case "general_logs":
                                if (configBackup.general_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.general_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux est désactivé!");
                                }                         
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            case "import_logs":
                                if (configBackup.import_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.import_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import est désactivé!");
                                }
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            case "export_Logs":
                                if (configBackup.export_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.export_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export est désactivé!");
                                }
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            case "import_files_success":
                                if (configBackup.import_files_success > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.import_files_success, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK est désactivé!");
                                }
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            case "import_files_error":
                                if (configBackup.import_files_error > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.import_files_error, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur est désactivé!");
                                }                                
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            case "backup_files":
                                if (configBackup.backup_files > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(writer, configBackup.backup_files, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup est désactivé!");
                                    writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup est désactivé!");
                                }
                                writer.WriteLine("");
                                writer.Flush();
                                break;

                            default:
                                Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Paramètre inconnu \"{directoriesList[x, 0]}\" du répertoir \"{directoriesList[x, 1]}\".");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Paramètre inconnu \"{directoriesList[x, 0]}\" du répertoir \"{directoriesList[x, 1]}\".");
                                writer.Flush();
                                break;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Les paramètres de nettoyage sont désactivés !");
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Les paramètres de nettoyage sont désactivés !");
                }
            }
            else
            {
                Console.WriteLine("Les paramètres de nettoyage sont manquants !!!");
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Les paramètres de nettoyage sont manquants !!!");
            }
            writer.WriteLine("");
            writer.Flush();
        }

        private void deleteOldFiles(StreamWriter writer, int configBackup_daysToKeep, string backUpFolderPath)
        {
            if (Directory.Exists(backUpFolderPath))
            {
                //delete all files the backup folder after x days
                DateTime today = DateTime.Now;
                DateTime ago = today.AddDays(-configBackup_daysToKeep);  //DateTime of x days ago

                Console.WriteLine(DateTime.Now + " | deleteOldFiles() : Supprimer tous les anciens fichiers log après " + ago);
                Console.WriteLine("");

                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                int filesDeleted = 0;
                FileInfo[] files = fileListing.GetFiles("*.txt");
                int allFiles = files.Length;

                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => deleteOldFiles() | Chemin des logs généraux => " + backUpFolderPath);
                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => deleteOldFiles() | Il y a " + allFiles + " fichiers");
                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => deleteOldFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));
                writer.Flush();

                for (int y = 0; y < allFiles; y++)
                {
                    FileInfo filename = files[y];
                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                    //writer.WriteLine(DateTime.Now + " | deleteOldFiles() : File: " + filename.Name + " | Creation Date: " + fileDateTime + " | Modify Date: " + fileDateTimeModif);
                    writer.Flush();

                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                    {
                        //file was never modified
                        if (fileDateTime < ago)
                        {
                            Console.WriteLine(DateTime.Now + " | deleteOldFiles() : File creation date: " + fileDateTime + " < " + ago);
                            File.Delete(filename.FullName);
                            //writer.WriteLine(DateTime.Now + " | deleteOldFiles() :Created: Delete File => " + filename.FullName);
                            writer.Flush();

                            filesDeleted++;
                        }
                    }
                    else
                    {
                        //file was modified
                        if (fileDateTimeModif < ago)
                        {
                            Console.WriteLine(DateTime.Now + " | deleteOldFiles() : File modify date: " + fileDateTimeModif + " < " + ago);
                            File.Delete(filename.FullName);
                            //writer.WriteLine(DateTime.Now + " | deleteOldFiles() :Modify: Delete File => " + filename.FullName);
                            writer.Flush();

                            filesDeleted++;
                        }
                    }
                    //writer.Flush();
                }
                Console.WriteLine(DateTime.Now + " | deleteOldFiles() : " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                Console.WriteLine("");
                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => deleteOldFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                writer.WriteLine("");
                writer.Flush();
            }
        }
        
        
        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// No StreamWriter
        /// 
        #region No StreamWriter
        private bool checkConfig()
        {
            if (File.Exists(this.fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void startClean()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("################################ Nettoyage De Fichier ###############################");
            Console.WriteLine("");

            try
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Cleaning Files");

                cleanFiles(this.paths);
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("********************* Fichier-De-Nettoyage.dll Erreur ********************");
                Console.WriteLine("" + ex.Message);
                Console.WriteLine("" + ex.StackTrace);
            }
        }

        private void cleanFiles(string[,] directoriesList)
        {
            if (this.checkConfig())
            {
                ConfigurationBackup configBackup = new ConfigurationBackup();
                configBackup.Load();
                if (configBackup.activate)
                {
                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        Console.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Type : " + directoriesList[x, 0] + " Répertoire : " + directoriesList[x, 1]);
                    }

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        switch (directoriesList[x, 0])
                        {
                            case "general_logs":
                                if (configBackup.general_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.general_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux est désactivé!");
                                }
                                break;

                            case "import_logs":
                                if (configBackup.import_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.import_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'import est désactivé!");
                                }
                                break;

                            case "export_Logs":
                                if (configBackup.export_Log > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.export_Log, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export est désactivé!");
                                }
                                break;

                            case "import_files_success":
                                if (configBackup.import_files_success > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.import_files_success, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import OK est désactivé!");
                                }
                                break;

                            case "import_files_error":
                                if (configBackup.import_files_error > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.import_files_error, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers import en erreur est désactivé!");
                                }
                                break;

                            case "backup_files":
                                if (configBackup.backup_files > 0)
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup dans le répertoir \"{directoriesList[x, 1]}\" ...");
                                    this.deleteOldFiles(configBackup.backup_files, directoriesList[x, 1]);
                                }
                                else
                                {
                                    Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers backup est désactivé!");
                                }
                                break;

                            default:
                                Console.WriteLine(DateTime.Now + $" :: Fichier-De-Nettoyage.dll => cleanFiles() | Paramètre inconnu \"{directoriesList[x, 0]}\" du répertoir \"{directoriesList[x, 1]}\".");
                                break;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Les paramètres de nettoyage sont désactivés !");
                }
            }
            else
            {
                Console.WriteLine("Les paramètres de nettoyage sont manquants !!!");
            }
        }

        private void deleteOldFiles(int configBackup_daysToKeep, string backUpFolderPath)
        {
            if (Directory.Exists(backUpFolderPath))
            {
                //delete all files the backup folder after x days
                DateTime today = DateTime.Now;
                DateTime ago = today.AddDays(-configBackup_daysToKeep);  //DateTime of x days ago

                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                int filesDeleted = 0;
                FileInfo[] files = fileListing.GetFiles("*.txt");
                int allFiles = files.Length;

                for (int y = 0; y < allFiles; y++)
                {
                    FileInfo filename = files[y];
                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                    {
                        //file was never modified
                        if (fileDateTime < ago)
                        {
                            File.Delete(filename.FullName);
                            filesDeleted++;
                        }
                    }
                    else
                    {
                        //file was modified
                        if (fileDateTimeModif < ago)
                        {
                            File.Delete(filename.FullName);
                            filesDeleted++;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
