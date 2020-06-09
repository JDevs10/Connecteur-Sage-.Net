using Reprocess_Error_Files.Classes.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reprocess
{
    public class ReprocessErrorFiles
    {
        private string directoryName_EDI = null;
        private string directoryName_tmp = Directory.GetCurrentDirectory() + @"\" + "tmp";
        private string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        private double inputHour = 1.0; // 1 hours

        private bool getEDI_CSV_Folder(StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => getEDI_CSV_Folder()");

            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
            if (settings.isSettings())
            {
                settings.Load();
                directoryName_EDI = settings.configurationGeneral.paths.EDI_Folder;
                
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => getEDI_CSV_Folder() | dossier EDI CSV trouvé.");
                return true;
            }
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => getEDI_CSV_Folder() | dossier EDI CSV non trouvé!");
            return false;
        }

        // Check if EDI files are in error file dossier
        private bool checkFiles(StreamWriter writer)
        {
            bool result;

            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles()");
            if (!Directory.Exists(directoryName_EDI))
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Le dossier  \"" + directoryName_EDI + "\" n'existe pas!");
                writer.WriteLine("");
                writer.Flush();
                return false;
            }
            if (!Directory.Exists(directoryName_ErrorFile))
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Le dossier  \"" + directoryName_ErrorFile + "\" n'existe pas!");
                writer.WriteLine("");
                writer.Flush();
                return false;
            }
            if (!Directory.Exists(directoryName_tmp))
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Le dossier  \"" + directoryName_tmp + "\" n'existe pas! Alors créé le répertoire:");
                Directory.CreateDirectory(directoryName_tmp);
                writer.WriteLine("");
                writer.Flush();
            }


            DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
            FileInfo[] allFiles = fileListing.GetFiles("*.csv");

            if(allFiles.Length > 0)
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Il y a " + allFiles.Length + " fichier EDI trouvé.");
                result = true;
            }
            else
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Aucun fichier EDI dans Error File.");
                result = false;
            }

            writer.WriteLine("");
            writer.Flush();
            return result;
        }

        // Reprocess all file after 1h
        public void reprocess(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess()");
            Console.WriteLine("");
            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess()");

            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
            if (settings.isSettings())
            {
                settings.Load();

                if (settings.configurationGeneral.reprocess.activate)
                {
                    if (!getEDI_CSV_Folder(writer))
                    {
                        writer.WriteLine("");
                        writer.Flush();
                        return;
                    }

                    if (checkFiles(writer))
                    {
                        DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
                        FileInfo[] allFiles = fileListing.GetFiles("*.csv");

                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Les fichier EDI dans " + directoryName_ErrorFile);
                        writer.Flush();
                        Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Les fichier EDI dans " + directoryName_ErrorFile);

                        if (settings.configurationGeneral.reprocess.hour == 0) 
                        {
                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Aucunne heure de retraitement saisi, HR = 0");
                            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Aucunne heure de retraitement saisi");
                            writer.Flush();
                        }
                        else
                        {
                            List<ReprocessFiles> reprocessFilesList = new List<ReprocessFiles>();
                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Heure de retraitement trouve.");
                            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Heure de retraitement trouve.");

                            for (int x = 0; x < allFiles.Length; x++)
                            {
                                FileInfo file = allFiles[x];
                                DateTime today = DateTime.Now;
                                DateTime fileDateTime = File.GetCreationTime(file.FullName);
                                TimeSpan ts = today - fileDateTime;

                                Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Index : " + (x + 1));

                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Index : " + (x + 1));
                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Date aujourd'hui : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", Date de création : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", TimeSpan : " + string.Format("{0}", ts.TotalHours));
                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Temps d'existence du fichier : "+ ts.TotalHours + " hrs >= Heure de retraitement : " + settings.configurationGeneral.reprocess.hour + " hrs");
                                writer.Flush();

                                // if file hour > X hours
                                if (ts.TotalHours >= Convert.ToDouble(settings.configurationGeneral.reprocess.hour))
                                {
                                    string[] fileNamePieces = file.Name.Split(new String[] { "__" }, StringSplitOptions.None);
                                    string newFileName = fileNamePieces[(fileNamePieces.Length - 1)];

                                    Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Ancien nom du fichier : " + file.Name + ", Nouveau nom : " + newFileName);
                                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Ancien nom du fichier : " + file.Name + ", Nouveau nom : " + newFileName);
                                    writer.Flush();

                                    try
                                    {
                                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        /// new feature
                                        /// 
                                        if (settings.configurationGeneral.reprocess.countDown == 0)
                                        {
                                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | La suppression des fichiers de retraitement X fois est désactivée");
                                            File.Copy(file.FullName, directoryName_EDI + @"\" + newFileName);
                                            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_EDI + @"\" + newFileName);
                                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_EDI + @"\" + newFileName);
                                            writer.Flush();

                                            if (File.Exists(directoryName_EDI + @"\" + newFileName))
                                            {
                                                File.Delete(file.FullName);
                                                Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Le fichier EDI existe dans le dossier \""+ directoryName_EDI + "\".\nSupprimer le fichier : " + file.FullName);
                                                writer.Flush();
                                            }
                                        }
                                        else
                                        {
                                            Reprocess_Error_Files.Classes.SaveLoadReprocess reprocess_setting = new Reprocess_Error_Files.Classes.SaveLoadReprocess();
                                            if (reprocess_setting.isFile())
                                            {
                                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Le fichier " + reprocess_setting.fileName + " existe, alors chargez-le.");
                                                reprocess_setting.Load();
                                                List<ReprocessFiles> reprocessFiles = reprocess_setting.reprocessFilesList;

                                                // check a specific string in a list of objects
                                                if (reprocessFiles.Any(item => Convert.ToInt32(newFileName.Split('.')[1]) == item.ediFileId) || reprocessFiles.Any(item => Convert.ToInt32(((newFileName.Contains("CFP41") || newFileName.Contains("CFP51") || newFileName.Contains("TWP41") || newFileName.Contains("TWP51")) ? newFileName.Split('_')[1].Replace(".csv", "") : "99999999")) == item.ediFileId))
                                                {
                                                    // the file in the obj list was found
                                                    // check count down
                                                    ReprocessFiles reprocessFiles_new = reprocessFiles.FindAll(item => Convert.ToInt32(newFileName.Split('.')[1]) == item.ediFileId)[0];

                                                    if (reprocessFiles_new.fileReprocessCount >= settings.configurationGeneral.reprocess.countDown)
                                                    {
                                                        // move the file to tmp
                                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Le Connecteur a tenté d'importer ce document "+ reprocessFiles_new.fileReprocessCount + " fois de suite sans succès, il sera déplacé sur dans le dossier => "+ directoryName_tmp);
                                                        File.Copy(file.FullName, directoryName_tmp + @"\" + file.Name);
                                                        Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_tmp + @"\" + file.Name);
                                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_tmp + @"\" + file.Name);
                                                        writer.Flush();

                                                        if (File.Exists(directoryName_tmp + @"\" + file.Name))
                                                        {
                                                            File.Delete(file.FullName);
                                                            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                                                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                                                            writer.Flush();
                                                        }

                                                        reprocessFiles.Remove(reprocessFiles_new);
                                                    }
                                                    else
                                                    {
                                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Aucun retraitement. Alors gardez le fichier "+ reprocessFiles_new.fileName + " dans les fichiers de retraitement.");
                                                        reprocessFiles.Remove(reprocessFiles_new);
                                                        reprocessFiles_new.fileReprocessCount++;
                                                        reprocessFiles.Add(reprocessFiles_new);
                                                    }

                                                }
                                                else
                                                {
                                                    // if the specific string does not contains in the list/file then add it
                                                    ReprocessFiles reprocessFiles_new = new ReprocessFiles();
                                                    reprocessFiles_new.fileReprocessCount = 1;
                                                    reprocessFiles_new.fileName = newFileName;
                                                    reprocessFiles_new.filePath = file.FullName;
                                                    reprocessFiles_new.ediFileId = Convert.ToInt32(newFileName.Split('.')[1]);
                                                    reprocessFiles.Add(reprocessFiles_new);
                                                }

                                                //save the updated list in the file
                                                reprocess_setting.reprocessFilesList = reprocessFiles;
                                                reprocess_setting.saveInfo();
                                            }
                                            else
                                            {
                                                // if no file is found then create one with one obj
                                                ReprocessFiles reprocessFiles = new ReprocessFiles();
                                                reprocessFiles.fileName = newFileName;
                                                reprocessFiles.filePath = file.FullName;
                                                reprocessFiles.fileReprocessCount = 1;

                                                string xxx = (newFileName.Contains("EDI_ORDERS") ? newFileName.Split('.')[1] : ((newFileName.Contains("CFP41") || newFileName.Contains("CFP51") || newFileName.Contains("TWP41") || newFileName.Contains("TWP51")) ? newFileName.Split('_')[1].Replace(".csv", "")  : "999") );

                                                reprocessFiles.ediFileId = Convert.ToInt32(xxx);
                                                reprocessFilesList.Add(reprocessFiles);

                                                reprocess_setting.reprocessFilesList = reprocessFilesList;
                                                reprocess_setting.saveInfo();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        writer.WriteLine(DateTime.Now + " :: ############################### Erreur Retraitement ###############################");
                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Message : " + ex.Message);
                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | StackTrace :\n" + ex.StackTrace);
                                        writer.WriteLine("");
                                    }
                                }
                                writer.Flush();
                            }

                        }
                        
                        writer.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                }
                else
                {
                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Le retraitement est désactivé!");
                    writer.Flush();
                }

            }
            else
            {
                Console.WriteLine(DateTime.Now + " :: Pas de config (init.json) trouve...");
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        private bool getEDI_CSV_Folder()
        {
            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
            if (settings.isSettings())
            {
                settings.Load();
                directoryName_EDI = settings.configurationGeneral.paths.EDI_Folder;
                return true;
            }
            return false;
        }

        // Check if EDI files are in error file dossier
        private bool checkFiles()
        {
            bool result;
            if (!Directory.Exists(directoryName_EDI))
            {
                return false;
            }
            if (!Directory.Exists(directoryName_ErrorFile))
            {
                return false;
            }
            if (!Directory.Exists(directoryName_tmp))
            {
                Directory.CreateDirectory(directoryName_tmp);
            }


            DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
            FileInfo[] allFiles = fileListing.GetFiles("*.csv");

            if (allFiles.Length > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        // Reprocess all file from Error File 
        public void reprocessManually()
        {
            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
            if (settings.isSettings())
            {
                settings.Load();

                if (settings.configurationGeneral.reprocess.activate)
                {
                    if (!getEDI_CSV_Folder())
                    {
                        return;
                    }

                    if (checkFiles())
                    {
                        DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
                        FileInfo[] allFiles = fileListing.GetFiles("*.csv");

                        if (settings.configurationGeneral.reprocess.hour == 0)
                        {
                            Console.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Aucunne heure de retraitement saisi");
                        }
                        else
                        {
                            for (int x = 0; x < allFiles.Length; x++)
                            {
                                FileInfo file = allFiles[x];
                                DateTime today = DateTime.Now;
                                DateTime fileDateTime = File.GetCreationTime(file.FullName);
                                TimeSpan ts = today - fileDateTime;

                                string[] fileNamePieces = file.Name.Split(new String[] { "__" }, StringSplitOptions.None);
                                string newFileName = fileNamePieces[(fileNamePieces.Length - 1)];

                                File.Copy(file.FullName, directoryName_EDI + @"\" + newFileName);

                                if (File.Exists(directoryName_EDI + @"\" + newFileName))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                    }
                }
            }
        }
    
        public void reprocessTmpManually()
        {
            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
            if (settings.isSettings())
            {
                settings.Load();

                // check if tmp and EDI/CSV folders existe
                getEDI_CSV_Folder();
                if (Directory.Exists(directoryName_tmp) && Directory.Exists(directoryName_EDI))
                {
                    if (settings.configurationGeneral.reprocess.activate)
                    {
                        // tmp dossier
                        DirectoryInfo fileListing = new DirectoryInfo(directoryName_tmp);
                        FileInfo[] allFiles = fileListing.GetFiles("*.csv");

                        if (allFiles.Length == 0)
                        {
                            return;
                        }

                        for (int x = 0; x < allFiles.Length; x++)
                        {
                            FileInfo file = allFiles[x];
                            string[] fileNamePieces = file.Name.Split(new String[] { "__" }, StringSplitOptions.None);
                            string newFileName = fileNamePieces[(fileNamePieces.Length - 1)];

                            File.Copy(file.FullName, directoryName_EDI + @"\" + newFileName);

                            if (File.Exists(directoryName_EDI + @"\" + newFileName))
                            {
                                File.Delete(file.FullName);
                            }
                        }
                    }
                }
            }
        }
    }
}
