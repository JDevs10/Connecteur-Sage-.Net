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
using System.Threading;
using ConnecteurSage.Helpers;
using ConnecteurSage.Forms;
using System.Diagnostics;
using Connexion;

namespace ConnecteurSage
{
    public partial class Main : Form
    {
        private bool isDocAchatCommande = false;
        private bool isDocAchatDESADV = false;
        private bool isDocAchatFacture = false;
        private bool isDocVenteCommande = false;
        private bool isDocVenteDESADV = false;
        private bool isDocVenteFacture = false;
        private bool isDocStock = false;
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
            string version = new Connecteur_Info.ConnecteurInfo().Version;
            label3.Text = "Connecteur Sage v" + version + " \nImport et export de documents commerciaux.";
            labelVersion.Text = "Version : " + version;
            labelCopyright.Text = "Copyright © 2013 - 2020";

            Forms.ProgressDialog progressDialog = new Forms.ProgressDialog();

            // Initialize the thread that will handle the background process
            Thread backgroundThread = new Thread(
            new ThreadStart(() =>
            {
                //Loading General Settings
                if (progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Text = "Loading General Settings...."));
                for (int n = 0; n < 10; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
                if (settings.isSettings())
                {
                    settings.Load();

                    //settings.configurationGeneral.general.showWindow == 5  ---> show software
                    label_debugMode.Text = ((settings.configurationGeneral.general.showWindow == 5) ? "Mode Débugage : Activé" : "Mode Débugage : Désactivé");
                    label_tarifaire.Text = ((settings.configurationGeneral.priceType.activate) ? "Configuration Tarifaire : Activé" : "Config Tarifaire : Désactivé");
                    label_retraitement.Text = ((settings.configurationGeneral.reprocess.activate) ? "Retraitement : Activé" : "Retraitement : Désactivé");
                }
                else
                {
                    try
                    {
                        using (Forms.GeneralConfig form = new Forms.GeneralConfig())
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


                //Loading Connexion Settings
                if (progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Text = "Loading Connexion Settings...."));
                for (int n = 10; n < 20; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                Connexion.ConnexionSaveLoad conn_Settings = new ConnexionSaveLoad();

                if (conn_Settings.isSettings())
                {
                    conn_Settings.Load();

                    label1.Text = "DSN ODBC : " + conn_Settings.configurationConnexion.ODBC.DNS;
                    label2.Text = "DSN ODBC Nom : " + conn_Settings.configurationConnexion.ODBC.USER;
                    label5.Text = "DSN SQL : " + conn_Settings.configurationConnexion.SQL.DNS;
                    label9.Text = "DSN SQL Nom : " + conn_Settings.configurationConnexion.SQL.USER;
                }
                else
                {
                    try
                    {
                        using (Forms.ConfigConnexion form = new Forms.ConfigConnexion())
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

                //Loading Import Settings
                if (progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Text = "Loading Import Settings...."));
                for (int n = 20; n < 40; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }
                Config_Import.ConfigurationSaveLoad import_Settings = new Config_Import.ConfigurationSaveLoad();

                if (import_Settings.isSettings())
                {
                    import_Settings.Load();

                    int cpt = 0;
                    int count = -1;

                    //Doc Export Achat 
                    //no config yet, so show désactivated only
                    cpt = 0;
                    count = 3;
                    cpt += ((import_Settings.configurationImport.Doc_Achat.Commande.Activate == "true") ? 1 : 0);
                    isDocAchatCommande = ((import_Settings.configurationImport.Doc_Achat.Commande.Activate == "true") ? true : false);
                    cpt += ((import_Settings.configurationImport.Doc_Achat.DSADV.Activate == "true") ? 1 : 0);
                    isDocAchatDESADV = ((import_Settings.configurationImport.Doc_Achat.DSADV.Activate == "true") ? true : false);
                    cpt += ((import_Settings.configurationImport.Doc_Achat.Facture.Activate == "true") ? 1 : 0);
                    isDocAchatFacture = ((import_Settings.configurationImport.Doc_Achat.Facture.Activate == "true") ? true : false);
                    label2.Text = ((cpt == 0) ? "Document Achat d'import : désactivé" : "Document Achat d'import : " + cpt + "/" + count + " activé");

                    //Doc Export Vente
                    cpt = 0;
                    count = 3;
                    cpt += ((import_Settings.configurationImport.Doc_Ventes.Commande.Activate == "true") ? 1 : 0);
                    isDocVenteCommande = ((import_Settings.configurationImport.Doc_Ventes.Commande.Activate == "true") ? true : false);
                    cpt += ((import_Settings.configurationImport.Doc_Ventes.DSADV.Activate == "true") ? 1 : 0);
                    isDocVenteDESADV = ((import_Settings.configurationImport.Doc_Ventes.DSADV.Activate == "true") ? true : false);
                    cpt += ((import_Settings.configurationImport.Doc_Ventes.Facture.Activate == "true") ? 1 : 0);
                    isDocVenteFacture = ((import_Settings.configurationImport.Doc_Ventes.Facture.Activate == "true") ? true : false);
                    label9.Text = ((cpt == 0) ? "Document Vente d'import : désactivé" : "Document Vente d'import : " + cpt + "/" + count + " activé");

                    // Stock
                    cpt = 0;
                    count = 1;
                    cpt += ((import_Settings.configurationImport.Doc_Stock.Stock.Activate == "true") ? 1 : 0);
                    isDocStock = ((import_Settings.configurationImport.Doc_Stock.Stock.Activate == "true") ? true : false);
                    label11.Text = ((cpt == 0) ? "Document de stock import : désactivé" : "Document de stock import : " + cpt + "/" + count + " activé");

                }
                else
                {
                    try
                    {
                        using (ConfigImport form = new ConfigImport())
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


                // Loading Export Settings
                if (progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Text = "Loading Export Settings...."));
                for (int n = 40; n < 60; n++)
                {
                    Thread.Sleep(1);
                    progressDialog.UpdateProgress(n);
                }

                Config_Export.ConfigurationSaveLoad export_Settings = new Config_Export.ConfigurationSaveLoad();

                if (export_Settings.isSettings())
                {
                    export_Settings.Load();

                    int cpt = 0;
                    int count = -1;

                    //Doc Export Achat 
                    //no config yet, so show désactivated only
                    cpt = 0;
                    count = 3;
                    label7.Text = ((cpt == 0) ? "Document Achat d'Export : désactivé" : "Document Achat d'Export : " + cpt + "/" + count + " activé");
                    groupBox_export_doc_achat.Enabled = ((cpt == 0) ? false : true);
                    label_export_doc_achat.Text = ((cpt == 0) ? "Ces fonctionnalité ne sont pas accessible..." : ".....");

                    //Doc Export Vente   
                    cpt = 0;
                    count = 3;
                    cpt += ((export_Settings.configurationExport.Commande.Activate) ? 1 : 0);
                    cpt += ((export_Settings.configurationExport.DSADV.Activate) ? 1 : 0);
                    cpt += ((export_Settings.configurationExport.Facture.Activate) ? 1 : 0);
                    label8.Text = ((cpt == 0) ? "Document Vente d'Export : désactivé" : "Document Vente d'Export : " + cpt + "/" + count + " activé");
                    groupBox_export_doc_vente.Enabled = ((cpt == 0) ? false : true);
                    label_groupBox_export_doc_vente.Text = ((cpt == 0) ? "la configuration est nécessaire. Veuillez vous rendre dans la configuration d'export et la remplir, merci..." : "");

                    // Stock
                    cpt = 0;
                    count = 1;
                    cpt += ((export_Settings.configurationExport.Stock.Activate) ? 1 : 0);
                    label10.Text = ((cpt == 0) ? "Document stock d'Export : désactivé" : "Document stock d'Export : " + cpt+"/"+count+" activé");
                    groupBox_export_doc_stock.Enabled = ((cpt == 0) ? false : true);
                    label_export_doc_stock.Text = ((cpt == 0) ? "Veuillez vous rendre dans la configuration d'export et la remplir, merci..." : "");
                }
                else
                {
                    try
                    {
                        using (ConfExport form = new ConfExport())
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

                //Loading Backup Settings
                if (progressDialog.InvokeRequired)
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Text = "Loading Backup Settings...."));
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
                            form.ShowDialog();
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
                {
                    progressDialog.BeginInvoke(new System.Action(() => progressDialog.Close()));
                    string msg = "Vous pouvez réaliser l'import des documents commerciaux suivantes :\n\n";
                    msg += "Import des bons de commandes d'achat : " + ((isDocAchatCommande) ? "activé" : "désactivé") + "\n";
                    msg += "Import des bons de livraison d'achat : " + ((isDocAchatDESADV) ? "activé" : "désactivé") + "\n";
                    msg += "Import des factures d'achat : " + ((isDocAchatFacture) ? "activé" : "désactivé") + "\n\n";
                    msg += "Import des bons de commandes vente : " + ((isDocVenteCommande) ? "activé" : "désactivé") + "\n";
                    msg += "Import des bons de livraison vente : " + ((isDocVenteDESADV) ? "activé" : "désactivé") + "\n";
                    msg += "Import des factures vente : " + ((isDocVenteFacture) ? "activé" : "désactivé") + "\n\n";
                    msg += "Import des stock : " + ((isDocStock) ? "activé" : "désactivé") + "\n";

                    label4.Text = msg;
                }
                    
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
                using (Forms.ConfigConnexion form = new Forms.ConfigConnexion())
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

                //Open Z-Cron
                /*
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Z-Cron\z-cron.exe"))
                {
                    MessageBox.Show("Ne peut pas ouvrir Z-Cron à : " + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Z-Cron\z-cron.exe", "Z-Cron", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Process emailExe = Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Z-Cron\z-cron.exe");
                emailExe.WaitForExit();
                */

            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {

            label4.Text = "Vous pouvez planifier l'import \ndes documents pour une heure \nqui vous convient.";

        }

        private void button1_MouseHover(object sender, EventArgs e)
        {

            label4.Text = "Vous pouvez réaliser l'import des \ndocuments d'une façon manuelle.";
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            string msg = "Vous pouvez réaliser l'import des documents commerciaux suivantes :\n\n";
            msg += "Import des bons de commandes d'achat : " + ((isDocAchatCommande) ? "activé" : "désactivé") + "\n";
            msg += "Import des bons de livraison d'achat : " + ((isDocAchatDESADV) ? "activé" : "désactivé") + "\n";
            msg += "Import des factures d'achat : " + ((isDocAchatFacture) ? "activé" : "désactivé") + "\n\n";
            msg += "Import des bons de commandes vente : " + ((isDocVenteCommande) ? "activé" : "désactivé") + "\n";
            msg += "Import des bons de livraison vente : " + ((isDocVenteDESADV) ? "activé" : "désactivé") + "\n";
            msg += "Import des factures vente : " + ((isDocVenteFacture) ? "activé" : "désactivé") + "\n\n";
            msg += "Import des stock : " + ((isDocStock) ? "activé" : "désactivé") + "\n";

            label4.Text = msg;
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
                using (Forms.ConfigEmail form = new Forms.ConfigEmail())
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

            /*
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
            */
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
                OdbcConnection connexion = ConnexionManager.CreateOdbcConnexionSQL();
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

        private void button10_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (Forms.GeneralConfig form = new Forms.GeneralConfig())
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

        private void button_config_import_Click(object sender, EventArgs e)
        {
            try
            {
                using (Forms.ConfigImport form = new Forms.ConfigImport())
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

        private void button14_Click(object sender, EventArgs e)
        {

        }
    }
}
