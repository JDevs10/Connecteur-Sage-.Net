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
        private bool checkConfig()
        {
            if(File.Exists(Directory.GetCurrentDirectory() + @"\SettingBackup.xml"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void startClean(StreamWriter writer, string[,] paths)
        {
            writer.WriteLine("");
            writer.WriteLine("################################ Nettoyage De Fichier ###############################");
            writer.WriteLine("");

            try
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Cleaning Files");
                writer.Flush();

                cleanFiles(writer, paths);

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

            if (checkConfig())
            {
                ConfigurationBackup configBackup = new ConfigurationBackup();
                configBackup.Load();
                writer.WriteLine(DateTime.Now + " : cleanFiles() | Paramètres de nettoyage trouvés et chargés.");

                if (configBackup.activate)
                {
                    Console.WriteLine("Cleaning settings are activated !");
                    writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Paramètres de nettoyage activé.");
                    writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + directoriesList.GetLength(0) + " dossiers à nettoyer.");
                    writer.Flush();

                    List<String> doneDirectories = new List<string>();

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        if (configBackup.general_Log != 0 && directoriesList[x, 0] == "general_logs" && !doneDirectories.Contains("general_logs"))
                        {
                            doneDirectories.Add("general_logs");

                            Console.WriteLine("");
                            Console.WriteLine("Nettoyage des logs généraux ...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs généraux ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.general_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers log après " + ago);
                                Console.WriteLine("");

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des logs généraux => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_Log != 0 && directoriesList[x, 0] == "import_logs" && !doneDirectories.Contains("import_logs"))
                        {
                            doneDirectories.Add("import_logs");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Import logs...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs import ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " : cleanFiles() | Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des logs import => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + "  :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_Log != 0 && directoriesList[x, 0] == "export_Logs" && !doneDirectories.Contains("export_Logs"))
                        {
                            doneDirectories.Add("export_Logs");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export logs...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + "  :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des logs d'export ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);
                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des logs d'export => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers log après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + "  :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_files_success != 0 && directoriesList[x, 0] == "import_files_success" && !doneDirectories.Contains("import_files_success"))
                        {
                            doneDirectories.Add("import_files_success");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import réussi...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + "  :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers d'import réussi ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_success);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago + "\n");
                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des fichiers d'import => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers d'import après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.import_files_error != 0 && directoriesList[x, 0] == "import_files_error" && !doneDirectories.Contains("import_files_error"))
                        {
                            doneDirectories.Add("import_files_error");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import en erreur...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers d'import en erreur...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_error);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des fichiers d'import en erreur => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers d'import en erreur après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_BC != 0 && directoriesList[x, 0] == "export_files_BC" && !doneDirectories.Contains("export_files_BC"))
                        {
                            doneDirectories.Add("export_files_BC");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BC...");
                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers EDI BC...");

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BC);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des fichiers EDI BC => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers EDI BC après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_BL != 0 && directoriesList[x, 0] == "export_files_BL" && !doneDirectories.Contains("export_files_BL"))
                        {
                            doneDirectories.Add("export_files_BL");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BL...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + "  :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers EDI BL...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_BL_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_BonLivraison";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BL);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Delete all old file after " + ago);

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_FA != 0 && directoriesList[x, 0] == "export_files_FA" && !doneDirectories.Contains("export_files_FA"))
                        {
                            doneDirectories.Add("export_files_FA");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files FA...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers EDI FA...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_FA_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_Facture";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_FA);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des fichiers EDI FA => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers EDI FA après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                writer.WriteLine("");
                                writer.Flush();
                            }
                        }
                        if (configBackup.export_files_ME_MS != 0 && directoriesList[x, 0] == "export_files_ME_MS" && !doneDirectories.Contains("export_files_ME_MS"))
                        {
                            doneDirectories.Add("export_files_ME_MS");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files ME/MS...");

                            writer.WriteLine("");
                            writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Nettoyage des fichiers EDI ME/MS...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_ME_MS_type == "Veolog_Stock")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Veolog_Stock";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Aucun format selectionné !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_ME_MS);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Chemin des fichiers EDI ME/MS => " + backUpFolderPath);
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Supprimer tous les anciens fichiers EDI ME/MS après " + string.Format("{0:dd-MM-yyyy HH.mm.ss}", ago));

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
                                writer.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
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
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// No StreamWriter
        /// 

        public void startClean(string[,] paths)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("################################ Nettoyage De Fichier ###############################");
            Console.WriteLine("");

            try
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now + " : Cleaning Files");

                cleanFiles(paths);
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
            if (checkConfig())
            {
                ConfigurationBackup configBackup = new ConfigurationBackup();
                configBackup.Load();

                if (configBackup.activate)
                {
                    Console.WriteLine("Cleaning settings are activated !");

                    List<String> doneDirectories = new List<string>();

                    for (int x = 0; x < directoriesList.GetLength(0); x++)
                    {
                        if (configBackup.general_Log != 0 && directoriesList[x, 0] == "general_logs" && !doneDirectories.Contains("general_logs"))
                        {
                            doneDirectories.Add("general_logs");

                            Console.WriteLine("");
                            Console.WriteLine("Nettoyage des logs généraux ...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.general_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Supprimer tous les anciens fichiers log après " + ago);
                                Console.WriteLine("");

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                                int filesDeleted = 0;
                                int allFiles = fileListing.GetFiles("*.txt").Length;

                                for (int y = 0; y < allFiles; y++)
                                {
                                    FileInfo filename = fileListing.GetFiles("*.txt")[y];
                                    DateTime fileDateTime = File.GetCreationTime(filename.FullName);
                                    DateTime fileDateTimeModif = File.GetLastWriteTime(filename.FullName);

                                    if (fileDateTime.ToString("dd/MM/yyyy hh:mm:ss") == fileDateTimeModif.ToString("dd/MM/yyyy hh:mm:ss"))
                                    {
                                        //file was never modified
                                        if (fileDateTime < ago)
                                        {
                                            Console.WriteLine(DateTime.Now + " | cleanFiles() : File creation date: " + fileDateTime + " < " + ago);
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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " fichiers ont été supprimés.");
                                Console.WriteLine("");
                            }
                        }
                        if (configBackup.import_Log != 0 && directoriesList[x, 0] == "import_logs" && !doneDirectories.Contains("import_logs"))
                        {
                            doneDirectories.Add("import_logs");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Import logs...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " : cleanFiles() | Delete all old file after " + ago);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.export_Log != 0 && directoriesList[x, 0] == "export_Logs" && !doneDirectories.Contains("export_Logs"))
                        {
                            doneDirectories.Add("export_Logs");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export logs...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_Log);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);
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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.import_files_success != 0 && directoriesList[x, 0] == "import_files_success" && !doneDirectories.Contains("import_files_success"))
                        {
                            doneDirectories.Add("import_files_success");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import réussi...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_success);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago + "\n");
                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.import_files_error != 0 && directoriesList[x, 0] == "import_files_error" && !doneDirectories.Contains("import_files_error"))
                        {
                            doneDirectories.Add("import_files_error");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Nettoyage des fichiers d'import en erreur...");

                            string backUpFolderPath = directoriesList[x, 1];
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.import_files_error);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.export_files_BC != 0 && directoriesList[x, 0] == "export_files_BC" && !doneDirectories.Contains("export_files_BC"))
                        {
                            doneDirectories.Add("export_files_BC");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BC...");

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
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BC);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.export_files_BL != 0 && directoriesList[x, 0] == "export_files_BL" && !doneDirectories.Contains("export_files_BL"))
                        {
                            doneDirectories.Add("export_files_BL");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files BL...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_BL_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_BonLivraison";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_BL);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " :: Fichier-De-Nettoyage.dll => cleanFiles() | Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.export_files_FA != 0 && directoriesList[x, 0] == "export_files_FA" && !doneDirectories.Contains("export_files_FA"))
                        {
                            doneDirectories.Add("export_files_FA");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files FA...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_FA_type == "Export_Plat")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Plat_Facture";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_FA);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                            }
                        }
                        if (configBackup.export_files_ME_MS != 0 && directoriesList[x, 0] == "export_files_ME_MS" && !doneDirectories.Contains("export_files_ME_MS"))
                        {
                            doneDirectories.Add("export_files_ME_MS");

                            Console.WriteLine("");
                            Console.WriteLine(DateTime.Now + " : Cleaning Export files ME/MS...");

                            string backUpFolderPath = null;
                            if (configBackup.export_files_ME_MS_type == "Veolog_Stock")
                            {
                                backUpFolderPath = directoriesList[x, 1] + @"\BackUp\Export\Veolog_Stock";
                            }
                            else
                            {
                                Console.WriteLine("No format was selected !");
                                break;
                            }
                            if (Directory.Exists(backUpFolderPath))
                            {
                                //delete all files the backup folder after x days
                                DateTime today = DateTime.Now;
                                DateTime ago = today.AddDays(-configBackup.export_files_ME_MS);  //DateTime of x days ago

                                Console.WriteLine(DateTime.Now + " | cleanFiles() : Delete all old file after " + ago);

                                DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);

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
                                            File.Delete(filename.FullName);
                                            filesDeleted++;
                                        }
                                    }
                                    //writer.Flush();
                                }
                                Console.WriteLine(DateTime.Now + " | cleanFiles() : " + filesDeleted + "/" + allFiles + " files were deleted.");
                                Console.WriteLine("");
                            }
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

    }
}
