using Reprocess_Error_Files.Classes.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reprocess
{
    public class ReprocessErrorFiles
    {
        private string directoryName_EDI = null;
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

                        if(settings.configurationGeneral.reprocess.hour == 0) 
                        {
                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Aucunne heure de retraitement saisi, HR = 0");
                            writer.Flush();
                        }
                        else
                        {
                            for (int x = 0; x < allFiles.Length; x++)
                            {
                                FileInfo file = allFiles[x];
                                DateTime today = DateTime.Now;
                                DateTime fileDateTime = File.GetCreationTime(file.FullName);

                                TimeSpan ts = today - fileDateTime;

                                writer.WriteLine("");
                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Index : " + (x + 1));
                                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Date aujourd'hui : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", Date de création : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", TimeSpan : " + string.Format("{0}", ts.TotalHours));
                                writer.Flush();

                                // if file hour > X hours
                                if (ts.TotalHours >= settings.configurationGeneral.reprocess.hour)
                                {
                                    string[] fileNamePieces = file.Name.Split(new String[] { "__" }, StringSplitOptions.None);
                                    string newFileName = fileNamePieces[(fileNamePieces.Length - 1)];

                                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Ancien nom du fichier : " + file.Name + ", Nouveau nom : " + newFileName);
                                    writer.Flush();

                                    File.Copy(file.FullName, directoryName_EDI + @"\" + newFileName);
                                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_EDI + @"\" + newFileName);
                                    writer.Flush();

                                    if (File.Exists(directoryName_EDI + @"\" + newFileName))
                                    {
                                        File.Delete(file.FullName);
                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                                        writer.Flush();
                                    }

                                    //////////////////////////////
                                    /// new feature
                                    /// 
                                    if(settings.configurationGeneral.reprocess.countDown == 0)
                                    {
                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | La suppression des fichiers de retraitement X fois est désactivée");
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        Reprocess_Error_Files.Classes.SaveLoadReprocess reprocess_setting = new Reprocess_Error_Files.Classes.SaveLoadReprocess();
                                        if (reprocess_setting.isFile())
                                        {
                                            reprocess_setting.Load();
                                            List<ReprocessFiles> reprocessFiles = reprocess_setting.reprocessFilesList;

                                            // check a specific string in a list of objects
                                            if (reprocessFiles.Any(p => file.Name.Contains(p.fileName)))
                                            {
                                                // the file in the obj list was found
                                                // check count down
                                                List<ReprocessFiles> reprocessFiles_new = reprocessFiles.FindAll(p => file.Name.Contains(p.fileName));
                                                for (int y = 0; y < reprocessFiles_new.Count; y++)
                                                {
                                                    if (reprocessFiles_new[y].fileReprocessCount >= settings.configurationGeneral.reprocess.countDown)
                                                    {
                                                        File.Delete(file.FullName);
                                                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                                                        writer.Flush();
                                                    }
                                                    else
                                                    {
                                                        reprocessFiles_new[y].fileReprocessCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                writer.Flush();
                            }

                        }
                        
                        writer.WriteLine("");
                    }
                }

            }
            
        }
    }
}
