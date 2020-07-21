using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using ConnecteurSage.Classes;
using ConnecteurSage.Helpers;
using System.Threading;
using System.IO;
using Connexion;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace ConnecteurSage.Forms
{
    public partial class ExportBonLivraison : Form
    {
        public ExportBonLivraison()
        {
            InitializeComponent();
        }

        private List<DocumentVente> BonLivrasonAExporter;
        private Customer customer = new Customer();

        private string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "BON_LIVRAISON";
        private StreamWriter logFileWriter_export = null;


        private List<DocumentVente> GetBonLivraisonFromDataBase(string client, string statut)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    DocumentVente documentVente;
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(false, client, 3, statut), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                documentVente = new DocumentVente(reader[0].ToString(), reader[1].ToString().Replace("00:00:00", ""),
                                    reader[2].ToString().Replace("00:00:00", ""), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(), 
                                     reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString(),
                                     reader[32].ToString()
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
                    return listDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private List<DocumentVenteLine> getDocumentLine(StreamWriter logFileWriter, string codeDocument)
        {
            try
            {
                logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : called.");
                logFileWriter.Flush();

                //DocumentVente Facture = new DocumentVente();
                List <DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnexionSQL())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : SQL => " + QueryHelper.getListDocumentVenteLine(true, codeDocument));
                    logFileWriter.Flush(); 
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(true, codeDocument), connection);
                    {
                        int lines = 1;
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : Line troué : " + lines);
                                logFileWriter.Flush();

                                DocumentVenteLine ligne = new DocumentVenteLine(reader[0].ToString().Replace("00:00:00", ""), reader[1].ToString().Replace("00:00:00", ""),
                                    reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                    reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString()
                                    );


                                // Calculate the Net Unite Price
                                double total_remise = Convert.ToDouble(ligne.DL_Remise01REM_Valeur) + Convert.ToDouble(ligne.DL_Remise02REM_Valeur) + Convert.ToDouble(ligne.DL_Remise03REM_Valeur);
                                double total_remise_Pourcent = total_remise / 100;
                                double PV_Net = Convert.ToDouble(ligne.DL_PrixUnitaire) * total_remise_Pourcent;     //PV_Net == Prix de Vente Net
                                ligne.DL_PrixUNet = (Convert.ToDouble(ligne.DL_PrixUnitaire) - PV_Net).ToString();    //Prix Unitaire Net

                                // Calculate DL_MontantTaxes
                                ligne.DL_MontantTaxes = (Convert.ToDouble(ligne.DL_Taxe1) + Convert.ToDouble(ligne.DL_Taxe2) + Convert.ToDouble(ligne.DL_Taxe3)).ToString();

                                lignesDocumentVente.Add(ligne);
                            }
                        }
                    }
                    return lignesDocumentVente;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                logFileWriter.WriteLine("");
                logFileWriter.WriteLine("");
                logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : ############################# Erreur #############################");
                logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : Message : " + "" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter.WriteLine(DateTime.Now + " | getDocumentLine() : Stacktrace : " + e.StackTrace);
                logFileWriter.Flush();
                return null;
            }
        }

        private List<Customer> GetListClients()
        {
            try
            {
                List<Customer> listClient = new List<Customer>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListClient(false), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Customer client = new Customer(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString(), reader[10].ToString(), reader[11].ToString(), reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(), reader[16].ToString());
                                listClient.Add(client);
                            }
                        }
                    }
                    return listClient;

                }

            }

            catch (Exception e)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + e.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                Config_Export.ConfigurationSaveLoad exportSettings = new Config_Export.ConfigurationSaveLoad();
                try
                {
                    if (!exportSettings.isSettings())
                    {
                        MessageBox.Show("La configuration d'export d'un document n'est pas renseigné!\nVeuillez ajouter la configuration avant d'utiliser cette action.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                    exportSettings.Load();

                    if (!exportSettings.configurationExport.DSADV.Activate)
                    {
                        MessageBox.Show("L'export des Bons de Livraisons sont désactivé.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                    if (exportSettings.configurationExport.DSADV.Status == null || !int.TryParse(exportSettings.configurationExport.DSADV.Status, out int _))
                    {
                        MessageBox.Show("Le statut d'export des Bons de Livraisons n'est pas correcte.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message + "\nStacktrace : \n" + ex.StackTrace, " ***** Erreur Config Export *****", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Close();
                }

                textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Initialize the dialog that will contain the progress bar
                ProgressDialog ProgressDialog = new ProgressDialog();

                // Initialize the thread that will handle the background process
                Thread backgroundThread = new Thread(
                    new ThreadStart(() =>
                    {
                        // Set the flag that indicates if a process is currently running
                        //isProcessRunning = true;
                        for (int n = 0; n < 25; n++)
                        {
                            Thread.Sleep(1);
                            ProgressDialog.UpdateProgress(n);
                        }

                        //Affichage des clients du dossier
                        if (customersDataGridView.InvokeRequired)
                        {
                            customersDataGridView.Invoke(new MethodInvoker(delegate
                            {
                                customersDataGridView.DataSource = GetListClients();
                                for (int n = 26; n < 45; n++)
                                {
                                    Thread.Sleep(1);
                                    ProgressDialog.UpdateProgress(n);
                                }
                                importButton.Enabled = customersDataGridView.Rows.Count > 0;

                                if (customersDataGridView.Columns["CT_Num"] != null)
                                    customersDataGridView.Columns["CT_Num"].HeaderText = "Client";
                                if (customersDataGridView.Columns["CT_Intitule"] != null)
                                    customersDataGridView.Columns["CT_Intitule"].Visible = false;
                                if (customersDataGridView.Columns["CT_Adresse"] != null)
                                    customersDataGridView.Columns["CT_Adresse"].Visible = false;
                                if (customersDataGridView.Columns["CT_APE"] != null)
                                    customersDataGridView.Columns["CT_APE"].Visible = false;
                                if (customersDataGridView.Columns["CAPITAL_SOCIAL"] != null)
                                    customersDataGridView.Columns["CAPITAL_SOCIAL"].Visible = false;
                                if (customersDataGridView.Columns["CT_CodePostal"] != null)
                                    customersDataGridView.Columns["CT_CodePostal"].Visible = false;
                                if (customersDataGridView.Columns["CT_CodeRegion"] != null)
                                    customersDataGridView.Columns["CT_CodeRegion"].Visible = false;
                                if (customersDataGridView.Columns["CT_Complement"] != null)
                                    customersDataGridView.Columns["CT_Complement"].Visible = false;
                                if (customersDataGridView.Columns["CT_CONTACT"] != null)
                                    customersDataGridView.Columns["CT_CONTACT"].Visible = false;
                                if (customersDataGridView.Columns["CT_EdiCode"] != null)
                                    customersDataGridView.Columns["CT_EdiCode"].HeaderText = "GLN";
                                if (customersDataGridView.Columns["CT_email"] != null)
                                    customersDataGridView.Columns["CT_email"].Visible = false;
                                if (customersDataGridView.Columns["CT_Identifiant"] != null)
                                    customersDataGridView.Columns["CT_Identifiant"].Visible = false;
                                if (customersDataGridView.Columns["CT_Ville"] != null)
                                    customersDataGridView.Columns["CT_Ville"].Visible = false;
                                if (customersDataGridView.Columns["CT_Pays"] != null)
                                    customersDataGridView.Columns["CT_Pays"].Visible = false;
                                if (customersDataGridView.Columns["CT_Siret"] != null)
                                    customersDataGridView.Columns["CT_Siret"].Visible = false;
                                if (customersDataGridView.Columns["CT_Telephone"] != null)
                                    customersDataGridView.Columns["CT_Telephone"].Visible = false;
                                if (customersDataGridView.Columns["N_DEVISE"] != null)
                                    customersDataGridView.Columns["N_DEVISE"].Visible = false;


                                //test si un champ selectionner
                                if (customersDataGridView.SelectedRows.Count == 0)
                                {
                                    return;
                                }

                                Client customer = customersDataGridView.SelectedRows[0].DataBoundItem as Client;
                                if (BonLivraisonDataGridView.InvokeRequired)
                                {
                                    BonLivraisonDataGridView.Invoke(new MethodInvoker(delegate
                                    {
                                        BonLivraisonDataGridView.DataSource = GetBonLivraisonFromDataBase(customer.CT_Num, exportSettings.configurationExport.DSADV.Status);

                                        for (int n = 76; n < 90; n++)
                                        {
                                            Thread.Sleep(1);
                                            ProgressDialog.UpdateProgress(n);
                                        }

                                        importButton.Enabled = BonLivraisonDataGridView.Rows.Count > 0;

                                        if (BonLivraisonDataGridView.Columns["DO_Piece"] != null)
                                            BonLivraisonDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
                                        if (BonLivraisonDataGridView.Columns["DO_date"] != null)
                                            BonLivraisonDataGridView.Columns["DO_date"].HeaderText = "Date";
                                        if (BonLivraisonDataGridView.Columns["DO_dateLivr"] != null)
                                            BonLivraisonDataGridView.Columns["DO_dateLivr"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_devise"] != null)
                                            BonLivraisonDataGridView.Columns["DO_devise"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_No"] != null)
                                            BonLivraisonDataGridView.Columns["LI_No"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_Statut"] != null)
                                            BonLivraisonDataGridView.Columns["DO_Statut"].HeaderText = "Status";
                                        if (BonLivraisonDataGridView.Columns["DO_taxe1"] != null)
                                            BonLivraisonDataGridView.Columns["DO_taxe1"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_taxe2"] != null)
                                            BonLivraisonDataGridView.Columns["DO_taxe2"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_taxe3"] != null)
                                            BonLivraisonDataGridView.Columns["DO_taxe3"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_TypeTaxe1"] != null)
                                            BonLivraisonDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_TypeTaxe2"] != null)
                                            BonLivraisonDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_TypeTaxe3"] != null)
                                            BonLivraisonDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_MontantEcheance"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_NetAPayer"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_NetAPayer"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_PoidsBrut"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_PoidsNet"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_PoidsNet"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_Escompte"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_Escompte"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_TotalHT"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
                                        if (BonLivraisonDataGridView.Columns["FNT_TotalHTNet"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["FNT_TotalTTC"] != null)
                                            BonLivraisonDataGridView.Columns["FNT_TotalTTC"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_ADRESSE"] != null)
                                            BonLivraisonDataGridView.Columns["LI_ADRESSE"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_CODEPOSTAL"] != null)
                                            BonLivraisonDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_CODEREGION"] != null)
                                            BonLivraisonDataGridView.Columns["LI_CODEREGION"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_COMPLEMENT"] != null)
                                            BonLivraisonDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_VILLE"] != null)
                                            BonLivraisonDataGridView.Columns["LI_VILLE"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_PAYS"] != null)
                                            BonLivraisonDataGridView.Columns["LI_PAYS"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["C_MODE"] != null)
                                            BonLivraisonDataGridView.Columns["C_MODE"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_MOTIF"] != null)
                                            BonLivraisonDataGridView.Columns["DO_MOTIF"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["DO_COORD01"] != null)
                                            BonLivraisonDataGridView.Columns["DO_COORD01"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["LI_Intitule"] != null)
                                            BonLivraisonDataGridView.Columns["LI_Intitule"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["do_txescompte"] != null)
                                            BonLivraisonDataGridView.Columns["do_txescompte"].Visible = false;
                                        if (BonLivraisonDataGridView.Columns["ca_num"] != null)
                                            BonLivraisonDataGridView.Columns["ca_num"].Visible = false;
                                    }));
                                }

                                //Récupération du prochain identifiant de commande à utiliser
                                //string nextOrderId = GetNextOrderId();
                            }));
                        }

                        for (int n = 46; n < 100; n++)
                        {
                            Thread.Sleep(1);
                            ProgressDialog.UpdateProgress(n);
                        }

                        // Close the dialog if it hasn't been already
                        if (ProgressDialog.InvokeRequired)
                            ProgressDialog.BeginInvoke(new Action(() => ProgressDialog.Close()));

                        // Reset the flag that indicates if a process is currently running
                        //isProcessRunning = false;
                    }
                ));

                // Start the background process thread
                backgroundThread.Start();

                // Open the dialog
                ProgressDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog.

            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {

                textBox1.Text = folderDlg.SelectedPath;

                //Environment.SpecialFolder root = folderDlg.RootFolder;

            }
        }

        private List<OrderLine> getLigneFactures(string code)
        {
            try
            {
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    List<OrderLine> lines = new List<OrderLine>();

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListLignesCommandes(false, code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lines.Add(new OrderLine(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString()));
                            }

                            return lines;
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private void customersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                Config_Export.ConfigurationSaveLoad exportSettings = new Config_Export.ConfigurationSaveLoad();
                try
                {
                    if (!exportSettings.isSettings())
                    {
                        MessageBox.Show("La configuration d'export d'un document n'est pas renseigné!\nVeuillez ajouter la configuration avant d'utiliser cette action.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                    exportSettings.Load();

                    if (!exportSettings.configurationExport.DSADV.Activate)
                    {
                        MessageBox.Show("L'export des Bons de Livraisons sont désactivé.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                    if (exportSettings.configurationExport.DSADV.Status == null || !int.TryParse(exportSettings.configurationExport.DSADV.Status, out int _))
                    {
                        MessageBox.Show("Le statut d'export des Bons de Livraisons n'est pas correcte.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message + "\nStacktrace : \n" + ex.StackTrace, " ***** Erreur Config Export *****", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Close();
                }

                if (customersDataGridView.SelectedRows.Count == 0)
                {
                    importButton.Enabled = false;
                    return;
                }

                customer = customersDataGridView.SelectedRows[0].DataBoundItem as Customer;
                if (customer == null)
                    throw new NullReferenceException("customer");
                thirdIdTextBox.Text = customer.CT_Num;
                thirdNameTextBox.Text = customer.CT_Intitule;
                addressTextBox.Text = customer.CT_Adresse;
                zipCodeTextBox.Text = customer.CT_CodePostal;
                cityTextBox.Text = customer.CT_Ville;
                countryTextBox.Text = customer.CT_Pays;

                BonLivraisonDataGridView.DataSource = GetBonLivraisonFromDataBase(customer.CT_Num, exportSettings.configurationExport.DSADV.Status);
                importButton.Enabled = BonLivraisonDataGridView.Rows.Count > 0;
                if (BonLivraisonDataGridView.Columns["DO_Piece"] != null)
                    BonLivraisonDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
                if (BonLivraisonDataGridView.Columns["DO_date"] != null)
                    BonLivraisonDataGridView.Columns["DO_date"].HeaderText = "Date";
                if (BonLivraisonDataGridView.Columns["DO_dateLivr"] != null)
                    BonLivraisonDataGridView.Columns["DO_dateLivr"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_devise"] != null)
                    BonLivraisonDataGridView.Columns["DO_devise"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_No"] != null)
                    BonLivraisonDataGridView.Columns["LI_No"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_Statut"] != null)
                    BonLivraisonDataGridView.Columns["DO_Statut"].HeaderText = "Status";
                if (BonLivraisonDataGridView.Columns["DO_taxe1"] != null)
                    BonLivraisonDataGridView.Columns["DO_taxe1"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_taxe2"] != null)
                    BonLivraisonDataGridView.Columns["DO_taxe2"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_taxe3"] != null)
                    BonLivraisonDataGridView.Columns["DO_taxe3"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_TypeTaxe1"] != null)
                    BonLivraisonDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_TypeTaxe2"] != null)
                    BonLivraisonDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_TypeTaxe3"] != null)
                    BonLivraisonDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_MontantEcheance"] != null)
                    BonLivraisonDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                    BonLivraisonDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_NetAPayer"] != null)
                    BonLivraisonDataGridView.Columns["FNT_NetAPayer"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_PoidsBrut"] != null)
                    BonLivraisonDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_PoidsNet"] != null)
                    BonLivraisonDataGridView.Columns["FNT_PoidsNet"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_Escompte"] != null)
                    BonLivraisonDataGridView.Columns["FNT_Escompte"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_TotalHT"] != null)
                    BonLivraisonDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
                if (BonLivraisonDataGridView.Columns["FNT_TotalHTNet"] != null)
                    BonLivraisonDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
                if (BonLivraisonDataGridView.Columns["FNT_TotalTTC"] != null)
                    BonLivraisonDataGridView.Columns["FNT_TotalTTC"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_ADRESSE"] != null)
                    BonLivraisonDataGridView.Columns["LI_ADRESSE"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_CODEPOSTAL"] != null)
                    BonLivraisonDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_CODEREGION"] != null)
                    BonLivraisonDataGridView.Columns["LI_CODEREGION"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_COMPLEMENT"] != null)
                    BonLivraisonDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_VILLE"] != null)
                    BonLivraisonDataGridView.Columns["LI_VILLE"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_PAYS"] != null)
                    BonLivraisonDataGridView.Columns["LI_PAYS"].Visible = false;
                if (BonLivraisonDataGridView.Columns["C_MODE"] != null)
                    BonLivraisonDataGridView.Columns["C_MODE"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_MOTIF"] != null)
                    BonLivraisonDataGridView.Columns["DO_MOTIF"].Visible = false;
                if (BonLivraisonDataGridView.Columns["DO_COORD01"] != null)
                    BonLivraisonDataGridView.Columns["DO_COORD01"].Visible = false;
                if (BonLivraisonDataGridView.Columns["LI_Intitule"] != null)
                    BonLivraisonDataGridView.Columns["LI_Intitule"].Visible = false;
                if (BonLivraisonDataGridView.Columns["do_txescompte"] != null)
                    BonLivraisonDataGridView.Columns["do_txescompte"].Visible = false;
                if (BonLivraisonDataGridView.Columns["ca_num"] != null)
                    BonLivraisonDataGridView.Columns["ca_num"].Visible = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void BonLivraisonDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            LinesDataGridView.DataSource = null;
            BonLivrasonAExporter = new List<DocumentVente>();
            foreach (DataGridViewRow row in BonLivraisonDataGridView.SelectedRows)
            {
                DocumentVente item = row.DataBoundItem as DocumentVente;
                if (item == null)
                    throw new NullReferenceException("item");
                BonLivrasonAExporter.Add(item);
            }
            LinesDataGridView.DataSource = BonLivrasonAExporter;
            importButton.Enabled = LinesDataGridView.Rows.Count > 0;
            if (LinesDataGridView.Columns["DO_Piece"] != null)
                LinesDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
            if (LinesDataGridView.Columns["DO_date"] != null)
                LinesDataGridView.Columns["DO_date"].HeaderText = "Date";
            if (LinesDataGridView.Columns["DO_dateLivr"] != null)
                LinesDataGridView.Columns["DO_dateLivr"].Visible = false;
            if (LinesDataGridView.Columns["DO_devise"] != null)
                LinesDataGridView.Columns["DO_devise"].Visible = false;
            if (LinesDataGridView.Columns["LI_No"] != null)
                LinesDataGridView.Columns["LI_No"].Visible = false;
            if (LinesDataGridView.Columns["DO_Statut"] != null)
                LinesDataGridView.Columns["DO_Statut"].HeaderText = "Status";
            if (LinesDataGridView.Columns["DO_taxe1"] != null)
                LinesDataGridView.Columns["DO_taxe1"].Visible = false;
            if (LinesDataGridView.Columns["DO_taxe2"] != null)
                LinesDataGridView.Columns["DO_taxe2"].Visible = false;
            if (LinesDataGridView.Columns["DO_taxe3"] != null)
                LinesDataGridView.Columns["DO_taxe3"].Visible = false;
            if (LinesDataGridView.Columns["DO_TypeTaxe1"] != null)
                LinesDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
            if (LinesDataGridView.Columns["DO_TypeTaxe2"] != null)
                LinesDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
            if (LinesDataGridView.Columns["DO_TypeTaxe3"] != null)
                LinesDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
            if (LinesDataGridView.Columns["FNT_MontantEcheance"] != null)
                LinesDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
            if (LinesDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                LinesDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
            if (LinesDataGridView.Columns["FNT_NetAPayer"] != null)
                LinesDataGridView.Columns["FNT_NetAPayer"].Visible = false;
            if (LinesDataGridView.Columns["FNT_PoidsBrut"] != null)
                LinesDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
            if (LinesDataGridView.Columns["FNT_PoidsNet"] != null)
                LinesDataGridView.Columns["FNT_PoidsNet"].Visible = false;
            if (LinesDataGridView.Columns["FNT_Escompte"] != null)
                LinesDataGridView.Columns["FNT_Escompte"].Visible = false;
            if (LinesDataGridView.Columns["FNT_TotalHT"] != null)
                LinesDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
            if (LinesDataGridView.Columns["FNT_TotalHTNet"] != null)
                LinesDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
            if (LinesDataGridView.Columns["FNT_TotalTTC"] != null)
                LinesDataGridView.Columns["FNT_TotalTTC"].Visible = false;
            if (LinesDataGridView.Columns["LI_ADRESSE"] != null)
                LinesDataGridView.Columns["LI_ADRESSE"].Visible = false;
            if (LinesDataGridView.Columns["LI_CODEPOSTAL"] != null)
                LinesDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
            if (LinesDataGridView.Columns["LI_CODEREGION"] != null)
                LinesDataGridView.Columns["LI_CODEREGION"].Visible = false;
            if (LinesDataGridView.Columns["LI_COMPLEMENT"] != null)
                LinesDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
            if (LinesDataGridView.Columns["LI_VILLE"] != null)
                LinesDataGridView.Columns["LI_VILLE"].Visible = false;
            if (LinesDataGridView.Columns["LI_PAYS"] != null)
                LinesDataGridView.Columns["LI_PAYS"].Visible = false;
            if (LinesDataGridView.Columns["C_MODE"] != null)
                LinesDataGridView.Columns["C_MODE"].Visible = false;
            if (LinesDataGridView.Columns["DO_MOTIF"] != null)
                LinesDataGridView.Columns["DO_MOTIF"].Visible = false;
            if (LinesDataGridView.Columns["DO_COORD01"] != null)
                LinesDataGridView.Columns["DO_COORD01"].Visible = false;
            if (LinesDataGridView.Columns["LI_Intitule"] != null)
                LinesDataGridView.Columns["LI_Intitule"].Visible = false;
            if (LinesDataGridView.Columns["do_txescompte"] != null)
                LinesDataGridView.Columns["do_txescompte"].Visible = false;
            if (LinesDataGridView.Columns["ca_num"] != null)
                LinesDataGridView.Columns["ca_num"].Visible = false;
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
        /// Génération du fichier d'export, lancement de l'application et exporter les Bon De Livraison
        /// </summary>

        private void ExportFacture(StreamWriter logFileWriter, Config_Export.ConfigurationSaveLoad exportSettings)
        {
            logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : Export Bon Livraison.");

            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Le chemin du fichier d'import de commande doit être renseigné");

                    logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : Le chemin du fichier d'export de Bon Livraison doit être renseigné.");
                    logFileWriter.Close();
                    return;
                }

                logFileWriter.Flush();

                if (exportSettings.configurationExport.DSADV.Format.Equals("Plat"))
                {
                    var fileName = string.Format("BonLivraison{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EdiCode + ".csv", DateTime.Now);

                    using (StreamWriter writer = new StreamWriter(textBox1.Text + @"\" + fileName, false, Encoding.UTF8))
                    {
                        logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : Ecrire le fichier dans : " + textBox1.Text + @"\" + fileName);

                        //writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                        //writer.WriteLine("");
                        //writer.WriteLine("");

                        logFileWriter.Flush();
                        for (int i = 0; i < BonLivrasonAExporter.Count; i++)
                        {
                            //string EANClient = GetEANClient(BonLivrasonAExporter[i].CustomerId);

                            //string[] tab = new string[] { "", "", "" };

                            logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : " + (i + 1) + "/" + BonLivrasonAExporter.Count + " a exporter.");
                            logFileWriter.Flush();
                            //if (BonLivrasonAExporter[i].OriginDocumentType == "8")
                            //{ // Return la commande d'origin                            
                            //    tab = GetCommandeFacture(BonLivrasonAExporter[i].Id).Split(';');
                            //}

                            writer.WriteLine("DESHDR;v01.0;;" + BonLivrasonAExporter[i].DO_Piece.Replace("BL", "") + ";" + customer.CT_EdiCode + ";9;;9;" + customer.CT_EdiCode + ";9;" + customer.CT_EdiCode + ";9;;9;;9;;9;;" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";;;;;" + BonLivrasonAExporter[i].LI_ADRESSE + ";;;;;;;;;;;;;;9;");
                            writer.WriteLine("");
                            writer.WriteLine("DESHD2;;;;" + customer.CT_Adresse + ";;" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + customer.CT_Intitule + ";" + customer.CT_Telephone + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                            writer.WriteLine("");

                            //if (BonLivrasonAExporter[i].IntrastatTransportMode != "")
                            //{ // Return mode de transport                           
                            //    BonLivrasonAExporter[i].IntrastatTransportMode = GetModeTransport(BonLivrasonAExporter[i].IntrastatTransportMode);
                            //}
                            writer.WriteLine("DESTRP;;;;;;;;;;");
                            writer.WriteLine("");
                            writer.WriteLine("DESREF;;;;"+ BonLivrasonAExporter[i].DO_COORD01 + ";;;;;");
                            writer.WriteLine("");
                            writer.WriteLine("DESLOG;;;;" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].FNT_PoidsNet.Replace(",", ".") + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                            writer.WriteLine("");
                            writer.Flush();

                            BonLivrasonAExporter[i].lines = getDocumentLine(logFileWriter, BonLivrasonAExporter[i].DO_Piece);


                            // DL_PrixUNet et non DL_PrixUnitaire
                            //writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[0].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[0].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[0].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[0].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[0].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[0].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[0].DL_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[0].DL_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[0].DL_NoColis + ";;;;;;;;;;;");


                            for (int j = 0; j < BonLivrasonAExporter[i].lines.Count; j++)
                            {
                                writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[j].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[j].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[j].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[j].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[j].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[j].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[j].DL_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[j].DL_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;");
                                writer.Flush();
                                writer.WriteLine("");
                            }


                            writer.WriteLine("DESEND;" + BonLivrasonAExporter[i].lines.Count + ";;;" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;;;;");
                            writer.Flush();
                            writer.WriteLine("");
                            writer.WriteLine("");
                            writer.Flush();
                        }

                        logFileWriter.WriteLine("");
                        logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : Un export en Json du BL :");

                        Init.Init init = new Init.Init();
                        logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : JSON : \n" + init.FormatJson(BonLivrasonAExporter));
                        logFileWriter.WriteLine("");
                    }
                }
                else
                {
                    MessageBox.Show("Le format \"" + exportSettings.configurationExport.DSADV.Format + "\" d'export n'existe pas dans le connecteur!", "Erreur Format Fichier", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    logFileWriter.WriteLine(DateTime.Now + "******************** Erreur Format Fichier ********************");
                    logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Le format \"" + exportSettings.configurationExport.DSADV.Format + "\" n'existe pas dans le connecteur!");
                    logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Vérifi le fichier de configuration \"" + Directory.GetCurrentDirectory() + @"\SettingExport.xml" + "\" à l'argument DSADV => Format.");
                    logFileWriter.Flush();
                    logFileWriter.Close();
                    return;
                }

                

                MessageBox.Show("Nombre bon de livraison : " + BonLivrasonAExporter.Count, "Information !!",
                                             MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : Bon de Livraison Exporté");
                logFileWriter.Flush();
                logFileWriter.Close();

            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                logFileWriter.WriteLine(DateTime.Now + " | ExportBonLivraison() : ERREUR :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter.Flush();
                logFileWriter.Close();
            }
        }
        private void importButton_Click(object sender, EventArgs e)
        {
            //importButton.Enabled = true;

            if (!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_BonLivraison_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_export = File.Create(logFileName_export);

            //Write in the log file 
            logFileWriter_export = new StreamWriter(logFile_export);
            //logFileWriter.Write(string.Format("{0:HH:mm:ss}", DateTime.Now) + " \r\n");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("");

            Config_Export.ConfigurationSaveLoad exportSettings = new Config_Export.ConfigurationSaveLoad();
            try
            {
                if (!exportSettings.isSettings())
                {
                    MessageBox.Show("La configuration d'export d'un document n'est pas renseigné!\nVeuillez ajouter la configuration avant d'utiliser cette action.", "Config d'Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    logFileWriter_export.WriteLine(DateTime.Now + " | importButton_Click() : La configuration d'export d'un document n'est pas renseigné!\nVeuillez ajouter la configuration avant d'utiliser cette action.");
                    logFileWriter_export.Flush();
                    logFileWriter_export.Close();
                    return;
                }
                exportSettings.Load();
            }
            catch (Exception ex)
            {
                logFileWriter_export.WriteLine(DateTime.Now + "******************** Erreur Config Export ********************");
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Message : " + ex.Message);
                logFileWriter_export.WriteLine(DateTime.Now + " | ExportFacture() : Stacktrace : \n" + ex.StackTrace);
                logFileWriter_export.Flush();
                logFileWriter_export.Close();
                return;
            }

            /** ================================================ **/
            /** ============== ExportBonLivraison ============== **/
            /** ================================================ **/
            ExportFacture(logFileWriter_export, exportSettings);

            //logFileWriter_export.Close();
            Close();
        }

        private void closeButton_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void customersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
