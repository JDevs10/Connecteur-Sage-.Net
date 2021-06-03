using Connexion;
using Export.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Export
{
    public class DocummentCommande
    {
        Database.Database db = null;
        private string fileName_ = string.Format("Client__{0:ddMMyyyy-HH.mm.ss}_.csv", DateTime.Now);
        private string fileName = null;
        private string export_folder = null;
        private string export_backup_folder = null;
        private string logDirectoryName_export = null;
        public List<Order> commandetList = null;

        public DocummentCommande()
        {
            // Init database && tables
            this.db = new Database.Database();

            Database.Model.Settings settings = db.settingsManager.get(db.connectionString, 1);
            this.logDirectoryName_export = settings.EXE_Folder + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "COMMANDE";

            export_folder = db.settingsManager.get(db.connectionString, 1).EDI_Folder + @"\Export_Client";
            export_backup_folder = db.settingsManager.get(db.connectionString, 1).EDI_Folder + @"\BackUp\Export\Client";
            fileName = export_folder + @"\" + fileName_;
            commandetList = new List<Order>();
        }


        //######################################
        // Export Code For Auto/Console app
        //######################################

        public void export_console(StreamWriter writer)
        {
            if (!Directory.Exists(export_folder))
            {
                Directory.CreateDirectory(export_folder);
            }

            //getClientData_c(writer);

            //Vérifier si le fichier a bien été créé et écrit
            if (File.Exists(fileName))
            {
                if (new FileInfo(fileName).Length > 0)
                {
                    //add to backup folder
                    addFileToBackUp_c(export_backup_folder, fileName, fileName_, writer);
                }
            }
        }

        public List<Alert_Mail.Classes.Custom.CustomMailRecapLines> exportCommande(List<Alert_Mail.Classes.Custom.CustomMailRecapLines> recapLinesList_new)
        {
            return null;
        }


        public void export_window()
        {
            /*
            if (!Directory.Exists(export_folder))
            {
                Directory.CreateDirectory(export_folder);
            }

            getClientData_w();

            //Vérifier si le fichier a bien été créé et écrit
            if (File.Exists(fileName))
            {
                if (new FileInfo(fileName).Length > 0)
                {
                    //add to backup folder
                    addFileToBackUp_w(export_backup_folder, fileName, fileName_);
                }
            }*/
        }


        private static void addFileToBackUp_w(string backUpFolderPath, string sourceFilePath, string filename)
        {
            //check if the backup folder exist
            if (!Directory.Exists(backUpFolderPath))
            {
                Directory.CreateDirectory(backUpFolderPath);
            }

            try
            {
                //copy the file to the backup folder
                if (File.Exists(backUpFolderPath + @"\" + filename))
                {
                    int version = 0;
                    //Get all .csv files in the folder
                    DirectoryInfo fileListing = new DirectoryInfo(backUpFolderPath);
                    
                    for (int x = 0; x < fileListing.GetFiles("*.csv").Length; x++)
                    {
                        string[] cutFileName = filename.Split('_');
                        string withouExtension = cutFileName[3].Split('.')[0];
                        string newFileName = cutFileName[0] + "_" + cutFileName[1] + "_" + cutFileName[2] + "_" + withouExtension;
                        FileInfo Filename = fileListing.GetFiles("*.csv")[x];

                        if ((Filename.Name).Contains(newFileName))
                        {
                            version++;
                        }
                    }

                    string[] cutFileName_1 = filename.Split('.');
                    string newFileName_1 = cutFileName_1[0] + "_v" + version + "." + cutFileName_1[1];
                    File.Copy(sourceFilePath, backUpFolderPath + @"\" + newFileName_1);
                }
                else
                {
                    File.Copy(sourceFilePath, backUpFolderPath + @"\" + filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fichier BackUp Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private static void addFileToBackUp_c(string backUpFolderPath, string sourceFilePath, string filename, StreamWriter writer)
        {
            writer.WriteLine("");
            //check if the backup folder exist
            if (!Directory.Exists(backUpFolderPath))
            {
                writer.WriteLine(DateTime.Now + " | addFileToBackUp() : Create BackUp folder at \"" + backUpFolderPath + "\"");
                Directory.CreateDirectory(backUpFolderPath);
            }

            try
            {
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
            }
            catch (Exception ex)
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : addFileToBackUp() |  ********************** Copy File *********************");
                writer.WriteLine(DateTime.Now + " : addFileToBackUp() |  Message Dev : N'arrive pas a Archiver ce fichier " + filename + ". Peut-être le fichier est déja pris par TDX.");
                writer.WriteLine(DateTime.Now + " : addFileToBackUp() |  Message : " + ex.Message);
                writer.WriteLine(DateTime.Now + " : addFileToBackUp() |  StackTrace : " + ex.StackTrace);
                writer.Flush();
            }

            writer.WriteLine("");
            writer.Flush();
        }

    }
}
