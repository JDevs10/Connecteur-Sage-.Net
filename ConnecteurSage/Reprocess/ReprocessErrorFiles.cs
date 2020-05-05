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
        private string directoryName_SuccessFile = Directory.GetCurrentDirectory() + @"\" + "Success File";
        private string directoryName_ErrorFile = Directory.GetCurrentDirectory() + @"\" + "Error File";
        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        private double inputHour = 2.0; // 2 hours

        // Check if EDI files are in error file dossier
        private bool checkFiles(StreamWriter writer)
        {
            bool result;

            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles()");
            if (!Directory.Exists(directoryName_SuccessFile))
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Le dossier  \"" + directoryName_SuccessFile + "\" n'existe pas!");
                result = false;
            }
            if (!Directory.Exists(directoryName_ErrorFile))
            {
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => checkFiles() | Le dossier  \"" + directoryName_ErrorFile + "\" n'existe pas!");
                result = false;
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

        // Reprocess all file after 2h
        public void reprocess(StreamWriter writer)
        {
            writer.Flush();
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess()");

            if (checkFiles(writer))
            {
                DirectoryInfo fileListing = new DirectoryInfo(directoryName_ErrorFile);
                FileInfo[] allFiles = fileListing.GetFiles("*.csv");

                int cpt = 1;
                writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Les fichier EDI dans " + directoryName_ErrorFile);
                writer.Flush();

                for (int x = 0; x < allFiles.Length; x++)
                {
                    FileInfo file = allFiles[x];
                    DateTime today = DateTime.Now;
                    DateTime fileDateTime = File.GetCreationTime(file.FullName);

                    TimeSpan ts = today - fileDateTime;

                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Index : " + cpt);
                    writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Date aujourd'hui : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", Date de création : " + string.Format("{0:dd-MM-yyyy HH:mm}", fileDateTime) + ", TimeSpan : " + string.Format("{0:HH:mm}", ts));
                    writer.Flush();

                    // if file hour > 2hs
                    if (ts.TotalHours > inputHour)
                    {
                        string[] fileNamePieces = file.Name.Split(new String[] { "__" }, StringSplitOptions.None);
                        string newFileName = fileNamePieces[(fileNamePieces.Length - 1)];

                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Ancien nom du fichier : " + file.Name + ", Nouveau nom : " + newFileName);

                        File.Copy(file.FullName, directoryName_SuccessFile + @"\" + newFileName);
                        writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Copie : " + file.FullName + "  à :  " + directoryName_SuccessFile + @"\" + newFileName);

                        if (File.Exists(directoryName_SuccessFile + @"\" + newFileName))
                        {
                            File.Delete(file.FullName);
                            writer.WriteLine(DateTime.Now + " :: Reprocess.dll => reprocess() | Supprimer le fichier : " + file.FullName);
                        }
                    }
                    writer.WriteLine("");
                    writer.Flush();
                }
                writer.WriteLine("");
            }
        }


    }
}
