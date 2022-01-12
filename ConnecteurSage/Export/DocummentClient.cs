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
    public class DocummentClient
    {
        private string fileName_ = string.Format("Client__{0:ddMMyyyy-HH.mm.ss}_.csv", DateTime.Now);
        private string fileName = null;
        private string export_folder = null;
        private string export_backup_folder = null;
        public List<Client> clientList = null;

        public DocummentClient()
        {
            Database.Database db = new Database.Database();
            export_folder = db.settingsManager.get(db.connectionString, 1).EDI_Folder + @"\Export\Client";
            export_backup_folder = db.settingsManager.get(db.connectionString, 1).EDI_Folder + @"\BackUp\Export\Client";
            fileName = export_folder + @"\" + fileName_;
            clientList = new List<Client>();
        }

        public void export_window()
        {
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
            }
        }

        public void export_console(StreamWriter writer)
        {
            if (!Directory.Exists(export_folder))
            {
                Directory.CreateDirectory(export_folder);
            }

            getClientData_c(writer);

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

        private void getClientData_w()
        {

            using (OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL())
            {

                try
                {
                    connexion.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getAllClients(true), connexion);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // reads lines/rows from the query
                        {
                            clientList.Add(new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString()));
                        }
                    }
                    connexion.Close();

                }
                catch (OdbcException ex)
                {
                    MessageBox.Show(ex.Message, "Erreur db client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if(clientList.Count == 0)
                {
                    MessageBox.Show("Aucun client trouvé dans la base Sage", "Client", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                // create file et populate it
                //logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : CommandeAExporter JSON => " + new Database.Database().JsonFormat(CommandeAExporter));

                using (StreamWriter clientFileWriter = new StreamWriter(fileName, false, Encoding.Default))
                {
                    clientFileWriter.WriteLine("E;Client;"+ string.Format("{0:ddMMyyyy}", DateTime.Now)+";"); // E start line in doc
                    clientFileWriter.Flush();

                    for (int i = 0; i < clientList.Count; i++)
                    {
                        clientFileWriter.WriteLine("L;"+ clientList[i].CT_Num + ";" + clientList[i].CG_NumPrinc + ";" + clientList[i].CT_NumPayeur + ";" + clientList[i].N_Condition + ";" + clientList[i].N_Devise + ";" + clientList[i].N_Expedition + ";" + clientList[i].CT_Langue + ";" + clientList[i].CT_Facture + ";" + clientList[i].N_Period + ";" + clientList[i].N_CatTarif + ";" + clientList[i].CT_Taux02 + ";" + clientList[i].N_CatCompta + ";" + clientList[i].CT_NumCentrale + ";" + clientList[i].CT_Intitule + ";" + clientList[i].CO_No + ";" + clientList[i].CT_EdiCode + ";"); // L => each line in doc
                        clientFileWriter.Flush();
                    }

                    clientFileWriter.WriteLine("F;" + clientList.Count + ";"); // F => end doc line
                    clientFileWriter.Flush();
                    clientFileWriter.Close();

                }

            }

        }

        private void getClientData_c(StreamWriter writer)
        {

            using (OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL())
            {

                try
                {
                    connexion.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.getAllClients(true), connexion);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // reads lines/rows from the query
                        {
                            clientList.Add(new Client(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString()));
                        }
                    }
                    connexion.Close();

                }
                catch (OdbcException ex)
                {
                    return;
                }

                if (clientList.Count == 0)
                {
                    return;
                }


                // create file et populate it
                //logFileWriter.WriteLine(DateTime.Now + " | ExportCommande() : CommandeAExporter JSON => " + new Database.Database().JsonFormat(CommandeAExporter));

                using (StreamWriter clientFileWriter = new StreamWriter(fileName, false, Encoding.Default))
                {
                    clientFileWriter.WriteLine("E;Client;" + string.Format("{0:ddMMyyyy}", DateTime.Now) + ";"); // E start line in doc
                    clientFileWriter.Flush();

                    for (int i = 0; i < clientList.Count; i++)
                    {
                        clientFileWriter.WriteLine("L;" + clientList[i].CT_Num + ";" + clientList[i].CG_NumPrinc + ";" + clientList[i].CT_NumPayeur + ";" + clientList[i].N_Condition + ";" + clientList[i].N_Devise + ";" + clientList[i].N_Expedition + ";" + clientList[i].CT_Langue + ";" + clientList[i].CT_Facture + ";" + clientList[i].N_Period + ";" + clientList[i].N_CatTarif + ";" + clientList[i].CT_Taux02 + ";" + clientList[i].N_CatCompta + ";" + clientList[i].CT_NumCentrale + ";" + clientList[i].CT_Intitule + ";" + clientList[i].CO_No + ";" + clientList[i].CT_EdiCode + ";"); // L => each line in doc
                        clientFileWriter.Flush();
                    }

                    clientFileWriter.WriteLine("F;" + clientList.Count + ";"); // F => end doc line
                    clientFileWriter.Flush();
                    clientFileWriter.Close();

                }

            }

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
