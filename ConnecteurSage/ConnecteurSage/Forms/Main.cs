using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnecteurSage.Classes;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32.TaskScheduler;
using System.Data.Odbc;
using ConnecteurSage.Helpers;
using System.Threading;

namespace ConnecteurSage
{
    public partial class Main : Form
    {
        private const string taskName = "importCommandeSage";

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //DateTime value = new DateTime(2013, 5, 1);
            //if (value <= DateTime.Today)
            //{
            //    button1.Enabled = false;
            //    button2.Enabled = false;
            //    TaskService ts = new TaskService();
            //    if (ts.FindTask(taskName, true) != null)
            //    {
            //        ts.RootFolder.DeleteTask(taskName);
            //    }
            //    MessageBox.Show("Une erreur est survenu lors du démarrage du connecteur.", "Fichier de configuration !!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //label2.Text = "La version d'essaie est terminé !!";
            //    Close();
            //}

        }
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public Main()
        {
            InitializeComponent();

            Forms.ProgressDialog progressDialog = new Forms.ProgressDialog();

            // Initialize the thread that will handle the background process
            Thread backgroundThread = new Thread(
            new ThreadStart(() =>
                {
                //Loading Connexion Settings
                progressDialog.Text = "Loading Connexion Settings....";
                for (int n = 0; n < 20; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                if (File.Exists(pathModule + @"\Setting.xml") && File.Exists(pathModule + @"\SettingSQL.xml"))
                {
                    //XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                    //StreamReader file = new System.IO.StreamReader("Setting.xml");
                    //ConfigurationDNS setting = new ConfigurationDNS();
                    //setting = (ConfigurationDNS)reader.Deserialize(file);

                    ConfigurationDNS setting1 = new ConfigurationDNS();
                    setting1.Load();
                    ConfigurationDNS setting2 = new ConfigurationDNS();
                    setting2.LoadSQL();

                    label1.Text = "DSN ODBC : " + setting1.DNS_1;
                    label2.Text = "DSN ODBC Nom : " + setting1.Nom_1;
                    label5.Text = "DSN SQL : " + setting2.DNS_2;
                    label9.Text = "DSN SQL Nom : " + setting1.Nom_2;
                    //file.Close();

                }
                else
                {
                    try
                    {
                        using (Forms.Configuration form = new Forms.Configuration())
                        {
                            form.ShowDialog(progressDialog);
                        }
                    }
                    // Récupération d'une possible SDKException
                    catch (SDKException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                //Loading Export Settings
                progressDialog.Text = "Loading Export Settings....";
                for (int n = 20; n < 40; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                if (File.Exists(pathModule + @"\SettingExport.xml"))
                {
                    //XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                    //StreamReader file = new System.IO.StreamReader("Setting.xml");
                    //ConfigurationDNS setting = new ConfigurationDNS();
                    //setting = (ConfigurationDNS)reader.Deserialize(file);

                    ConfigurationExport setting = new ConfigurationExport();
                    setting.Load();

                    label6.Text = "Statut d'Export Commande : " + ((setting.exportBonsCommandes == "True") ? "Activer" : "Désactiver");
                    label7.Text = "Statut d'Export Livraision : " + ((setting.exportBonsLivraisons == "True") ? "Activer" : "Désactiver");
                    label8.Text = "Statut d'Export Facture : " + ((setting.exportFactures == "True") ? "Activer" : "Désactiver");
                    label10.Text = "Statut Export Stock : " + ((setting.exportStock == "True") ? "Activer" : "Désactiver");
                }
                else
                {
                    try
                    {
                        using (Forms.ConfExport form = new Forms.ConfExport())
                        {
                            form.ShowDialog(progressDialog);
                        }
                    }
                    // Récupération d'une possible SDKException
                    catch (SDKException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                for (int n = 40; n < 60; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                //Loading Backup Settings
                progressDialog.Text = "Loading Backup Settings....";
                for (int n = 60; n < 80; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                if (File.Exists(pathModule + @"\SettingBackup.xml"))
                {
                    ConfigurationBackup backup = new ConfigurationBackup();
                    backup.Load();
                    label_backup_activation.Text = ((backup.activate) ? "Activation : Oui" : "Activation : Non");
                    label_backup_generalLog.Text = ((backup.general_Log != 0) ? "Log général : " + backup.general_Log + " jours" : "Log général : désactiver");
                    label_backup_importLog.Text = ((backup.import_Log != 0) ? "Log d'import : " + backup.import_Log + " jours" : "Log d'import : désactiver");
                    label_backup_exportLog.Text = ((backup.export_Log != 0) ? "Log d'export : " + backup.export_Log + " jours" : "Log d'export : désactiver");
                    label_backup_import_success.Text = ((backup.import_files_success != 0) ? "Fichier EDI import (Success) : " + backup.import_files_success + " jours" : "Fichier EDI import (Success) : désactiver");
                    label_backup_import_error.Text = ((backup.import_files_error != 0) ? "Fichier EDI import (Erreur) : " + backup.import_files_error + " jours" : "Fichier EDI import (Erreur) : désactiver");
                    label_backup_export_BC.Text = ((backup.export_files_BC != 0) ? "Fichier EDI Export (BC) : " + backup.export_files_BC + " jours" : "Fichier EDI Export (BC) : désactiver");
                    label_backup_export_BL.Text = ((backup.export_files_BL != 0) ? "Fichier EDI Export (BL) : " + backup.export_files_BL + " jours" : "Fichier EDI Export (BL) : désactiver");
                    label_backup_export_FA.Text = ((backup.export_files_FA != 0) ? "Fichier EDI Export (FA) : " + backup.export_files_FA + " jours" : "Fichier EDI Export (FA) : désactiver");
                    label_backup_export_ME_MS.Text = ((backup.export_files_ME_MS != 0) ? "Fichier EDI Export (ME/MS) : " + backup.export_files_ME_MS + " jours" : "Fichier EDI Export (ME/MS) : désactiver");
                }
                else
                {
                    try
                    {
                        using (Forms.ConfigBackup form = new Forms.ConfigBackup())
                        {
                            form.ShowDialog(progressDialog);
                        }
                    }
                    // Récupération d'une possible SDKException
                    catch (SDKException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                bool ok = false;
                for (int n = 80; n < 100; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                    if (n == 99) { ok = true; };
                }

                // Close the dialog if it hasn't been already
                if (ok && progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Close()));
                }
            ));

            // Start the background process thread
            backgroundThread.Start();

            // Open the dialog
            progressDialog.ShowDialog();
        }

        public static void ModifierButtonDNS(string text1, string text2, string text3, string text4)
        {
            Main main = new Main();
            main.label1.Text = "DSN I : " + text1;
            main.label2.Text = "Nom I : " + text2;
            main.label5.Text = "DSN II : " + text3;
            main.label9.Text = "Nom II : " + text4;
        }

        public static string getListeStatutName(int value)
        {
            string name = "";
            List<Statut> list = new Statut().getListeStatut();

            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Value == value)
                {
                    name = list[i].Nom;
                    break;
                }
            }
            return name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.Configuration form = new Forms.Configuration())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (Forms.importManuel form = new Forms.importManuel())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                using (Forms.importManuel form = new Forms.importManuel())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.Planifier form = new Forms.Planifier())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {

            label4.Text = "Vous pouvez planifier l'import \nde commandes pour une heure \nqui vous convient.";

        }

        private void button1_MouseHover(object sender, EventArgs e)
        {

            label4.Text = "Vous pouvez réaliser l'import de \ncommandes d'une façon manuelle.";
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {

            label4.Text = "Vous pouvez réaliser l'import et\n" +
                    "l'export de documents commerciaux. \n" +
                    "Aussi bien en manuel qu'en \n" +
                    "automatique.";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //EXPORT COMMANDES
            try
            {
                using (Forms.ExportCommande form = new Forms.ExportCommande())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //EXPORT FACTURES
            try
            {
                using (Forms.ExportFactures form = new Forms.ExportFactures())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //EXPORT BONLIVRAISONS
            try
            {
                using (Forms.ExportBonLivraison form = new Forms.ExportBonLivraison())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.ConfMail form = new Forms.ConfMail())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //EXPORT STOCKS
            try
            {
                using (Forms.ExportStocks form = new Forms.ExportStocks())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.ConfExport form = new Forms.ConfExport())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (initDOC_Numerotation())
            {
                MessageBox.Show("La table \"DOC_NumerotationTable\" est créé/existe avec des données!",
                    "Information !",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("La table \"DOC_NumerotationTable\" n'est ps créé!",
                    "Erreur !",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        public static int checkDOC_Numerotation(OdbcConnection connexion)
        {
            int result = -1;
            //writer.WriteLine("");
            //writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : Vérifier si la table de numérotation existe");

            try
            {
                OdbcCommand command = new OdbcCommand(QueryHelper.checkDOC_NumerotationTable(true), connexion);
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader[0].ToString() == "1")
                        {
                            result = 1;
                            //writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : La table de numérotation existe avec des données");
                        }
                        else
                        {
                            result = 0;
                            //writer.WriteLine(DateTime.Now + " | checkDOC_Numerotation() : La table de numérotation existe sans des données");
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur[200]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""),
                            "Erreur !",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error); 
                result = -1;
            }
            //writer.WriteLine("");
            return result;
        }

        public static bool initDOC_Numerotation()
        {
            //writer.WriteLine("");
            //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Init");
            bool result = false;
            try
            {
                OdbcConnection connexion = Connexion.CreateOdbcConnexionSQL();
                connexion.Open();
                int check = checkDOC_Numerotation(connexion);
                OdbcCommand command = null;

                if (check == 0 || check == 1)   //if the DOC_Numerotation do nothing
                {
                    result = true;
                    //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation existe!");
                    DialogResult resultDialog5 = MessageBox.Show("La table \"DOC_NumerotationTable\" existe !\n\nVoulez-vous vider et réinitialiser tous les données dans cette table ?",
                                                    "Information !",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question,
                                                    MessageBoxDefaultButton.Button2);
                    if (resultDialog5 == DialogResult.Yes)
                    {
                        try
                        {
                            command = new OdbcCommand(QueryHelper.deleteDOC_NumerotationTable(true), connexion);
                            command.ExecuteReader();
                            command = new OdbcCommand(QueryHelper.insertDOC_NumerotationTable(true, "BC200000", "BCF200000", "CF200000", "BL200000", "ME200000", "MS200000"), connexion);
                            command.ExecuteReader();
                        }
                        // Récupération d'une possible SDKException
                        catch (SDKException ex)
                        {
                            MessageBox.Show(" ERROR[Init 3] : " + ex.Message, "Erreur!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            result = false;
                        }
                    }

                    if (resultDialog5 == DialogResult.No)
                    {
                        result = true;
                    }
                }
                else if (check == -1)      //if the tDOC_Numerotation doesn't exist then create it 
                {
                    //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation does not existe, so create the table!");
                    
                    try
                    {
                        //Create DOC_Numerotation Table
                        command = new OdbcCommand(QueryHelper.createDOC_NumerotationTable(true), connexion);
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation created!");
                        }

                        //Set up the first init Numérotation
                        command = new OdbcCommand(QueryHelper.insertDOC_NumerotationTable(true, "BC200000", "BCF200000", "CF200000", "BL200000", "ME200000", "MS200000"), connexion);
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            result = true;
                            //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : Table DOC_Numerotation created!");
                        }
                        
                        connexion.Close();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        MessageBox.Show("Erreur[200]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""),
                            "Erreur !",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        //writer.WriteLine(DateTime.Now + " | initDOC_Numerotation() : ******************** Erreur ********************");
                        //writer.WriteLine(DateTime.Now + " : Erreur[200]" + ex.Message.Replace("[CBase]", "").Replace("[Microsoft]", "").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", "").Replace("ERROR", ""));
                    }
                }
                connexion.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR[Init 4] : " + ex.Message, "Erreur!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = false;
            }
            //writer.WriteLine("");
            return result;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.ConfigBackup form = new Forms.ConfigBackup())
                {
                    form.ShowDialog();
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
