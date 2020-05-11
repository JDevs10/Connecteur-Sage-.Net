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
using System.Globalization;
using Connexion;

namespace ConnecteurSage.Forms
{
    public partial class ExportFactures : Form
    {
        private List<DocumentVente> FacturesAExporter;
        private Customer customer = new Customer();
        private Societe societe = null;

        private string logDirectoryName_export = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Export" + @"\" + "FACTURE";
        private StreamWriter logFileWriter_export = null;

        public ExportFactures()
        {
            InitializeComponent();
        }

        private List<DocumentVente> GetFacturesFromDataBase(string client)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(false, client, 67), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocumentVente documentVente = new DocumentVente(reader[0].ToString(), reader[1].ToString().Replace("00:00:00", ""),
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

        private string GetModeReglement(string do_piece)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                //List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getModeReglement(false, do_piece), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString()+";"+reader[1].ToString()+";"+reader[2].ToString()+";"+reader[3].ToString()+";"+reader[4].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

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

        private List<DocumentVenteLine> getDocumentLine(string codeDocument)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(false, codeDocument), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DocumentVenteLine ligne = new DocumentVenteLine(reader[0].ToString().Replace("00:00:00", ""), reader[1].ToString().Replace("00:00:00", ""),
                                    reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString(),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[14].ToString(), reader[15].ToString(),
                                    reader[16].ToString(), reader[17].ToString(), reader[18].ToString(), reader[19].ToString(),
                                    reader[20].ToString(), reader[21].ToString(), reader[22].ToString(), reader[23].ToString(),
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(),
                                        reader[28].ToString(), reader[29].ToString(), reader[30].ToString(), reader[31].ToString()
                                    );
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
                                if (customersDataGridView.Columns["CT_SvFormeJuri"] != null)
                                    customersDataGridView.Columns["CT_SvFormeJuri"].Visible = false;
                        


                                //test si un champ selectionner
                                if (customersDataGridView.SelectedRows.Count == 0)
                                {
                                    return;
                                }

                                Client customer = customersDataGridView.SelectedRows[0].DataBoundItem as Client;
                                if (FacturesDataGridView.InvokeRequired)
                                {
                                    FacturesDataGridView.Invoke(new MethodInvoker(delegate
                                    {
                                        FacturesDataGridView.DataSource = GetFacturesFromDataBase(customer.CT_Num);

                                        for (int n = 76; n < 90; n++)
                                        {
                                            Thread.Sleep(1);
                                            ProgressDialog.UpdateProgress(n);
                                        }

                                        importButton.Enabled = FacturesDataGridView.Rows.Count > 0;

                                        if (FacturesDataGridView.Columns["DO_Piece"] != null)
                                            FacturesDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
                                        if (FacturesDataGridView.Columns["DO_date"] != null)
                                            FacturesDataGridView.Columns["DO_date"].HeaderText = "Date";
                                        if (FacturesDataGridView.Columns["DO_dateLivr"] != null)
                                            FacturesDataGridView.Columns["DO_dateLivr"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_devise"] != null)
                                            FacturesDataGridView.Columns["DO_devise"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_No"] != null)
                                            FacturesDataGridView.Columns["LI_No"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_Statut"] != null)
                                            FacturesDataGridView.Columns["DO_Statut"].HeaderText = "Status";
                                        if (FacturesDataGridView.Columns["DO_taxe1"] != null)
                                            FacturesDataGridView.Columns["DO_taxe1"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_taxe2"] != null)
                                            FacturesDataGridView.Columns["DO_taxe2"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_taxe3"] != null)
                                            FacturesDataGridView.Columns["DO_taxe3"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_TypeTaxe1"] != null)
                                            FacturesDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_TypeTaxe2"] != null)
                                            FacturesDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_TypeTaxe3"] != null)
                                            FacturesDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_MontantEcheance"] != null)
                                            FacturesDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                                            FacturesDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_NetAPayer"] != null)
                                            FacturesDataGridView.Columns["FNT_NetAPayer"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_PoidsBrut"] != null)
                                            FacturesDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_PoidsNet"] != null)
                                            FacturesDataGridView.Columns["FNT_PoidsNet"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_Escompte"] != null)
                                            FacturesDataGridView.Columns["FNT_Escompte"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_TotalHT"] != null)
                                            FacturesDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
                                        if (FacturesDataGridView.Columns["FNT_TotalHTNet"] != null)
                                            FacturesDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
                                        if (FacturesDataGridView.Columns["FNT_TotalTTC"] != null)
                                            FacturesDataGridView.Columns["FNT_TotalTTC"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_ADRESSE"] != null)
                                            FacturesDataGridView.Columns["LI_ADRESSE"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_CODEPOSTAL"] != null)
                                            FacturesDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_CODEREGION"] != null)
                                            FacturesDataGridView.Columns["LI_CODEREGION"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_COMPLEMENT"] != null)
                                            FacturesDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_VILLE"] != null)
                                            FacturesDataGridView.Columns["LI_VILLE"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_PAYS"] != null)
                                            FacturesDataGridView.Columns["LI_PAYS"].Visible = false;
                                        if (FacturesDataGridView.Columns["C_MODE"] != null)
                                            FacturesDataGridView.Columns["C_MODE"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_MOTIF"] != null)
                                            FacturesDataGridView.Columns["DO_MOTIF"].Visible = false;
                                        if (FacturesDataGridView.Columns["DO_COORD01"] != null)
                                            FacturesDataGridView.Columns["DO_COORD01"].Visible = false;
                                        if (FacturesDataGridView.Columns["LI_Intitule"] != null)
                                            FacturesDataGridView.Columns["LI_Intitule"].Visible = false;
                                        if (FacturesDataGridView.Columns["ca_num"] != null)
                                            FacturesDataGridView.Columns["ca_num"].Visible = false;

                                    }));
                                }

                                //Récupération du prochain identifiant de commande à utiliser
                                //string nextOrderId = GetNextOrderId();
                            }));
                        }

                        for (int n = 46; n < 80; n++)
                        {
                            Thread.Sleep(1);
                            ProgressDialog.UpdateProgress(n);
                        }

                        societe = getInfoSociete();

                        for (int n = 81; n < 100; n++)
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

        private Societe getInfoSociete()
        {
            try
            {
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getInfoSociete(false), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Societe(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString());
                            }
                        }
                    }
                    return null;

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

        private string getGNLClientLivraison(string intitule)
        {
            try
            {
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getGNLClientLivraison(false, intitule), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                        }
                    }
                    return null;

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

                FacturesDataGridView.DataSource = GetFacturesFromDataBase(customer.CT_Num);
                importButton.Enabled = FacturesDataGridView.Rows.Count > 0;
                if (FacturesDataGridView.Columns["DO_Piece"] != null)
                    FacturesDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
                if (FacturesDataGridView.Columns["DO_date"] != null)
                    FacturesDataGridView.Columns["DO_date"].HeaderText = "Date";
                if (FacturesDataGridView.Columns["DO_dateLivr"] != null)
                    FacturesDataGridView.Columns["DO_dateLivr"].Visible = false;
                if (FacturesDataGridView.Columns["DO_devise"] != null)
                    FacturesDataGridView.Columns["DO_devise"].Visible = false;
                if (FacturesDataGridView.Columns["LI_No"] != null)
                    FacturesDataGridView.Columns["LI_No"].Visible = false;
                if (FacturesDataGridView.Columns["DO_Statut"] != null)
                    FacturesDataGridView.Columns["DO_Statut"].HeaderText = "Status";
                if (FacturesDataGridView.Columns["DO_taxe1"] != null)
                    FacturesDataGridView.Columns["DO_taxe1"].Visible = false;
                if (FacturesDataGridView.Columns["DO_taxe2"] != null)
                    FacturesDataGridView.Columns["DO_taxe2"].Visible = false;
                if (FacturesDataGridView.Columns["DO_taxe3"] != null)
                    FacturesDataGridView.Columns["DO_taxe3"].Visible = false;
                if (FacturesDataGridView.Columns["DO_TypeTaxe1"] != null)
                    FacturesDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
                if (FacturesDataGridView.Columns["DO_TypeTaxe2"] != null)
                    FacturesDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
                if (FacturesDataGridView.Columns["DO_TypeTaxe3"] != null)
                    FacturesDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_MontantEcheance"] != null)
                    FacturesDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                    FacturesDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_NetAPayer"] != null)
                    FacturesDataGridView.Columns["FNT_NetAPayer"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_PoidsBrut"] != null)
                    FacturesDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_PoidsNet"] != null)
                    FacturesDataGridView.Columns["FNT_PoidsNet"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_Escompte"] != null)
                    FacturesDataGridView.Columns["FNT_Escompte"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_TotalHT"] != null)
                    FacturesDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
                if (FacturesDataGridView.Columns["FNT_TotalHTNet"] != null)
                    FacturesDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
                if (FacturesDataGridView.Columns["FNT_TotalTTC"] != null)
                    FacturesDataGridView.Columns["FNT_TotalTTC"].Visible = false;
                if (FacturesDataGridView.Columns["LI_ADRESSE"] != null)
                    FacturesDataGridView.Columns["LI_ADRESSE"].Visible = false;
                if (FacturesDataGridView.Columns["LI_CODEPOSTAL"] != null)
                    FacturesDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
                if (FacturesDataGridView.Columns["LI_CODEREGION"] != null)
                    FacturesDataGridView.Columns["LI_CODEREGION"].Visible = false;
                if (FacturesDataGridView.Columns["LI_COMPLEMENT"] != null)
                    FacturesDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
                if (FacturesDataGridView.Columns["LI_VILLE"] != null)
                    FacturesDataGridView.Columns["LI_VILLE"].Visible = false;
                if (FacturesDataGridView.Columns["LI_PAYS"] != null)
                    FacturesDataGridView.Columns["LI_PAYS"].Visible = false;
                if (FacturesDataGridView.Columns["C_MODE"] != null)
                    FacturesDataGridView.Columns["C_MODE"].Visible = false;
                if (FacturesDataGridView.Columns["DO_MOTIF"] != null)
                    FacturesDataGridView.Columns["DO_MOTIF"].Visible = false;
                if (FacturesDataGridView.Columns["DO_COORD01"] != null)
                    FacturesDataGridView.Columns["DO_COORD01"].Visible = false;
                if (FacturesDataGridView.Columns["LI_Intitule"] != null)
                    FacturesDataGridView.Columns["LI_Intitule"].Visible = false;
                if (FacturesDataGridView.Columns["do_txescompte"] != null)
                    FacturesDataGridView.Columns["do_txescompte"].Visible = false;
                if (FacturesDataGridView.Columns["ca_num"] != null)
                    FacturesDataGridView.Columns["ca_num"].Visible = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FacturesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            FacturesLinesDataGridView.DataSource = null;
            FacturesAExporter = new List<DocumentVente>();
            foreach (DataGridViewRow row in FacturesDataGridView.SelectedRows)
            {
                DocumentVente item = row.DataBoundItem as DocumentVente;
                if (item == null)
                    throw new NullReferenceException("item");
                FacturesAExporter.Add(item);
            }
            FacturesLinesDataGridView.DataSource = FacturesAExporter;
            importButton.Enabled = FacturesLinesDataGridView.Rows.Count > 0;
            if (FacturesLinesDataGridView.Columns["DO_Piece"] != null)
                FacturesLinesDataGridView.Columns["DO_Piece"].HeaderText = "Numero";
            if (FacturesLinesDataGridView.Columns["DO_date"] != null)
                FacturesLinesDataGridView.Columns["DO_date"].HeaderText = "Date";
            if (FacturesLinesDataGridView.Columns["DO_dateLivr"] != null)
                FacturesLinesDataGridView.Columns["DO_dateLivr"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_devise"] != null)
                FacturesLinesDataGridView.Columns["DO_devise"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_No"] != null)
                FacturesLinesDataGridView.Columns["LI_No"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_Statut"] != null)
                FacturesLinesDataGridView.Columns["DO_Statut"].HeaderText = "Status";
            if (FacturesLinesDataGridView.Columns["DO_taxe1"] != null)
                FacturesLinesDataGridView.Columns["DO_taxe1"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_taxe2"] != null)
                FacturesLinesDataGridView.Columns["DO_taxe2"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_taxe3"] != null)
                FacturesLinesDataGridView.Columns["DO_taxe3"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_TypeTaxe1"] != null)
                FacturesLinesDataGridView.Columns["DO_TypeTaxe1"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_TypeTaxe2"] != null)
                FacturesLinesDataGridView.Columns["DO_TypeTaxe2"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_TypeTaxe3"] != null)
                FacturesLinesDataGridView.Columns["DO_TypeTaxe3"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_MontantEcheance"] != null)
                FacturesLinesDataGridView.Columns["FNT_MontantEcheance"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_MontantTotalTaxes"] != null)
                FacturesLinesDataGridView.Columns["FNT_MontantTotalTaxes"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_NetAPayer"] != null)
                FacturesLinesDataGridView.Columns["FNT_NetAPayer"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_PoidsBrut"] != null)
                FacturesLinesDataGridView.Columns["FNT_PoidsBrut"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_PoidsNet"] != null)
                FacturesLinesDataGridView.Columns["FNT_PoidsNet"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_Escompte"] != null)
                FacturesLinesDataGridView.Columns["FNT_Escompte"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_TotalHT"] != null)
                FacturesLinesDataGridView.Columns["FNT_TotalHT"].HeaderText = "Total HT";
            if (FacturesLinesDataGridView.Columns["FNT_TotalHTNet"] != null)
                FacturesLinesDataGridView.Columns["FNT_TotalHTNet"].Visible = false;
            if (FacturesLinesDataGridView.Columns["FNT_TotalTTC"] != null)
                FacturesLinesDataGridView.Columns["FNT_TotalTTC"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_ADRESSE"] != null)
                FacturesLinesDataGridView.Columns["LI_ADRESSE"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_CODEPOSTAL"] != null)
                FacturesLinesDataGridView.Columns["LI_CODEPOSTAL"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_CODEREGION"] != null)
                FacturesLinesDataGridView.Columns["LI_CODEREGION"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_COMPLEMENT"] != null)
                FacturesLinesDataGridView.Columns["LI_COMPLEMENT"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_VILLE"] != null)
                FacturesLinesDataGridView.Columns["LI_VILLE"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_PAYS"] != null)
                FacturesLinesDataGridView.Columns["LI_PAYS"].Visible = false;
            if (FacturesLinesDataGridView.Columns["C_MODE"] != null)
                FacturesLinesDataGridView.Columns["C_MODE"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_MOTIF"] != null)
                FacturesLinesDataGridView.Columns["DO_MOTIF"].Visible = false;
            if (FacturesLinesDataGridView.Columns["DO_COORD01"] != null)
                FacturesLinesDataGridView.Columns["DO_COORD01"].Visible = false;
            if (FacturesLinesDataGridView.Columns["LI_Intitule"] != null)
                FacturesLinesDataGridView.Columns["LI_Intitule"].Visible = false;
            if (FacturesLinesDataGridView.Columns["do_txescompte"] != null)
                FacturesLinesDataGridView.Columns["do_txescompte"].Visible = false;
            if (FacturesLinesDataGridView.Columns["ca_num"] != null)
                FacturesLinesDataGridView.Columns["ca_num"].Visible = false;

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
        /// Génération du fichier d'export, lancement de l'application et exporter les factures
        /// </summary>
        private void ExportFacture(StreamWriter logFileWriter)
        {
            logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Export Facture.");

            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Le chemin du fichier d'import de commande doit être renseigné");
                    logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Le chemin du fichier d'import de commande doit être renseigné.");
                    logFileWriter.Close();
                    return;
                }
                
                //FacturesAExporter.
                var fileName = string.Format("INV-{0:HHmmss}.{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EdiCode + ".csv", DateTime.Now);
                string prefix = "FA";
                string identifiant = "380";

                if (FacturesAExporter.Count > 0 && FacturesAExporter[0].DO_Piece.StartsWith("AV"))
                {
                    fileName = string.Format("AVOIR-{0:HHmmss}.{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EdiCode + ".csv", DateTime.Now);
                    prefix = "AV";
                    identifiant = "381";
                }

                using (StreamWriter writer = new StreamWriter(textBox1.Text + @"\" + fileName, false, Encoding.Default))
                {
                    logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Ecrire le fichier dans : " + textBox1.Text + @"\" + fileName);

                    writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                    writer.WriteLine("");
                    writer.WriteLine("");

                    for (int i = 0; i < FacturesAExporter.Count; i++)
                    {

                        //string[] tab = new string[] { "", "", "" };

                        logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : " + i + "/" + FacturesAExporter.Count + " a exporter.");

                        //if (FacturesAExporter[i].OriginDocumentType == "8")
                        //{
                        //    tab = GetCommandeFacture(FacturesAExporter[i].Id).Split(';');
                        //}

                        string[] docRegl = GetModeReglement(FacturesAExporter[i].DO_Piece).Split(';');

                        string modeReglement = "";
                        string DR_DATE = "";
                        string DR_TYPEREGL = "";
                        string DR_POURCENT = "";
                        string DR_MONTANT = "";

                        if (docRegl.Length != 0)
                        {
                            modeReglement = docRegl[0];
                            DR_DATE = docRegl[1];
                            DR_TYPEREGL = docRegl[2];
                            DR_POURCENT = docRegl[3];
                            DR_MONTANT = docRegl[4];
                        }

                        string devise = "";
                        if(FacturesAExporter[i].DO_devise != "0")
                        {
                            if (FacturesAExporter[i].DO_devise == "1")
                            {
                                devise = "EUR";
                            }
                            else
                            {
                                devise = getDeviseIso(FacturesAExporter[i].DO_devise);
                            }
                        }

                        writer.WriteLine("DEMAT-HD1;v01.0;;" + FacturesAExporter[i].DO_Piece.Replace(prefix, "") + ";" + identifiant + ";9;" + ConvertDate(FacturesAExporter[i].DO_date) + ";" + ConvertDate(FacturesAExporter[i].DO_dateLivr) + ";;;;;" + modeReglement + ";;" + customer.CT_SvFormeJuri + ";;0;;" + FacturesAExporter[i].DO_COORD01 + ";;" + FacturesAExporter[i].DO_COORD01 + ";;;;;;;;;;;;;;;;;;;;;;;;" + devise + ";;;" + ConvertDate(DR_DATE) + ";;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".").Replace("00000", "") + ";;;;;;;;;;;;;");
                        writer.WriteLine("");

                        // Code GNL extraie de DO_MOTIF
                        //writer.WriteLine("DEMAT-HD2;" + customer.CT_EdiCode + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";;;;;;;3700471600002;TRACE SPORT;32 RUE DE PARADIS;75010;PARIS;FR;;;;;;;;;;;;;" + (FacturesAExporter[i].DO_MOTIF.Split(';').Length == 2 ? FacturesAExporter[i].DO_MOTIF.Split(';')[0] : null) + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";XXXX;500110226;ESA28425270;FR68500110226;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                        
                        // Code GLN extraie de li_complement
                        // writer.WriteLine("DEMAT-HD2;" + customer.CT_EdiCode + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";" + societe.D_Commentaire + ";" + societe.D_RaisonSoc + ";" + societe.D_Adresse + ";" + societe.D_CodePostal + ";" + societe.D_Ville + ";" + (societe.D_Pays.ToUpper() == "FRANCE" ? "FR" : societe.D_Pays) + ";;;;;;;;;;;;;" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + FacturesAExporter[i].LI_PAYS + ";XXXX;" + societe.D_Siret + ";ESA28425270;" + societe.D_Identifiant + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;" + societe.D_Commentaire + ";;;;;;;;;;;;;;;;;;");
                        // DEMAT-HD2 est changé pour extraire les infos de la société la table p_dossier(version 5.2)
                        // D_commentaire c'est le GLN de la société

                        string glnLivraison = null;

                        if (FacturesAExporter[i].LI_Intitule == customer.CT_Intitule)
                        {
                            glnLivraison = customer.CT_EdiCode;
                        }
                        else
                        {
                            glnLivraison = getGNLClientLivraison(FacturesAExporter[i].LI_Intitule);
                        }

                        writer.WriteLine("DEMAT-HD2;" + glnLivraison + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + (FacturesAExporter[i].LI_PAYS.ToUpper() == "FRANCE" ? "FR" : FacturesAExporter[i].LI_PAYS) + ";" + customer.CT_EdiCode + ";" + customer.CT_Num + ";" + customer.CT_Adresse + ";" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + (customer.CT_Pays.ToUpper() == "FRANCE" ? "FR" : customer.CT_Pays) + ";" + societe.D_Commentaire + ";" + societe.D_RaisonSoc + ";" + societe.D_Adresse + ";" + societe.D_CodePostal + ";" + societe.D_Ville + ";" + (societe.D_Pays.ToUpper() == "FRANCE" ? "FR" : societe.D_Pays) + ";;;;;;;;;;;;;" + FacturesAExporter[i].LI_COMPLEMENT + ";" + FacturesAExporter[i].LI_Intitule + ";" + FacturesAExporter[i].LI_ADRESSE + ";" + FacturesAExporter[i].LI_CODEPOSTAL + ";" + FacturesAExporter[i].LI_VILLE + ";" + (FacturesAExporter[i].LI_PAYS.ToUpper() == "FRANCE" ? "FR" : FacturesAExporter[i].LI_PAYS) + ";XXXX;" + societe.D_Siret + ";ESA28425270;" + societe.D_Identifiant + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;" + societe.D_Commentaire + ";;;;;;;;;;;;;;;;;;");
                        writer.WriteLine("");

                        if (FacturesAExporter[i].DO_Piece.StartsWith("FA"))
                        {
                            writer.WriteLine("DEMAT-CTH;1;AAI;Type de document;Facture;;;;");
                            writer.WriteLine("");
                        }
                        else if (FacturesAExporter[i].DO_Piece.StartsWith("AV"))
                        {
                            writer.WriteLine("DEMAT-CTH;1;AAI;Type de document;Bon d’avoir;;;;");
                            writer.WriteLine("");
                        }

                        writer.WriteLine("DEMAT-CTH;2;AAI;N° d'accord;" + FacturesAExporter[i].ca_num + ";;;;");
                        writer.WriteLine("");

                        //writer.WriteLine("DEMAT-CTA;" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";" + FacturesAExporter[i].InvoicingContact_Function + ";;" + FacturesAExporter[i].InvoicingContact_Name + " " + FacturesAExporter[i].InvoicingContact_FirstName + ";" + FacturesAExporter[i].InvoicingContact_Email + ";" + FacturesAExporter[i].InvoicingContact_Fax + ";" + FacturesAExporter[i].InvoicingContact_Phone + ";;;;;;;;;;;;;;;;;;;" + FacturesAExporter[i].DeliveryContact_Function + ";;" + FacturesAExporter[i].DeliveryContact_Name + " " + FacturesAExporter[i].DeliveryContact_FirstName + ";" + FacturesAExporter[i].DeliveryContact_Email + ";" + FacturesAExporter[i].DeliveryContact_Fax + ";" + FacturesAExporter[i].DeliveryContact_Phone + ";;;;;;;");
                        //writer.WriteLine("");

                        //if (FacturesAExporter[i]. != "0,00000000" || FacturesAExporter[i].DiscountAmount != "0,00000000")
                        //{
                            writer.WriteLine("DEMAT-REM;;A;;;;;;;;" + FacturesAExporter[i].FNT_Escompte + ";" + FacturesAExporter[i].do_txescompte.Replace(",", ".").Replace("00000", "") + ";;");
                            writer.WriteLine("");
                        //}

                        FacturesAExporter[i].lines = getDocumentLine(FacturesAExporter[i].DO_Piece);

                        for (int j = 0; j < FacturesAExporter[i].lines.Count; j++)
                        {

                            writer.WriteLine("DEMAT-LIN;" + FacturesAExporter[i].lines[j].DL_Ligne + ";" + FacturesAExporter[i].lines[j].AR_CODEBARRE + ";EAN;;;" + customer.CT_EdiCode + ";;;;" + FacturesAExporter[i].lines[j].DL_Design + ";;" + FacturesAExporter[i].lines[j].DL_PoidsNet.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].DL_PoidsBrut.Replace(",", ".") + ";;" + FacturesAExporter[i].lines[j].DL_Qte + ";" + FacturesAExporter[i].lines[j].DL_QteBL + ";" + FacturesAExporter[i].lines[j].EU_Qte + ";;;;" + FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";" + FacturesAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;1;;;" + ConvertDate(FacturesAExporter[i].lines[j].DO_DateLivr.Replace("00:00:00", "")) + ";" + FacturesAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;;;" + FacturesAExporter[i].lines[j].FNT_MontantTTC.Replace(",", ".") + ";;;;;;;;");
                            writer.WriteLine("");

                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "0")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Débit";
                            }
                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "0")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Débit";
                            }
                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "0")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Débit";
                            }

                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe1 == "1")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe1 = "TVA/Encaissement";
                            }
                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe2 == "1")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe2 = "TVA/Encaissement";
                            }
                            if (FacturesAExporter[i].lines[j].DL_TypeTaxe3 == "1")
                            {
                                FacturesAExporter[i].lines[j].DL_TypeTaxe3 = "TVA/Encaissement";
                            }

                            //decimal montantTaxe = Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                            //" + (montantTaxe * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture)/100)).ToString().Replace(",",".").Substring(0,3) + "

                            // Calcule taxe
                            decimal montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                            writer.WriteLine("DEMAT-TAX;1;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe1 + ";" + FacturesAExporter[i].lines[j].DL_Taxe1.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                            writer.WriteLine("");

                            //if (FacturesAExporter[i].lines[j].DL_Taxe2 != FacturesAExporter[i].lines[j].DL_Taxe1 && FacturesAExporter[i].lines[j].DL_Taxe2 != "0")
                            if (FacturesAExporter[i].lines[j].DL_Taxe2 != "0")
                            {
                                montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe2.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                writer.WriteLine("DEMAT-TAX;2;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe2 + ";" + FacturesAExporter[i].lines[j].DL_Taxe2.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                                writer.WriteLine("");
                            }

                            //if ((FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe1) && (FacturesAExporter[i].lines[j].DL_Taxe3 != FacturesAExporter[i].lines[j].DL_Taxe2) && FacturesAExporter[i].lines[j].DL_Taxe3 != "0")
                            if (FacturesAExporter[i].lines[j].DL_Taxe3 != "0")
                            {
                                montantTaxe = (Decimal.Parse(FacturesAExporter[i].lines[j].FNT_MontantHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].lines[j].DL_Taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                                writer.WriteLine("DEMAT-TAX;3;;" + FacturesAExporter[i].lines[j].DL_TypeTaxe3 + ";" + FacturesAExporter[i].lines[j].DL_Taxe3.Replace(",", ".") + ";;" + Math.Round(montantTaxe, 2).ToString().Replace(",", ".") + ";;;");
                                writer.WriteLine("");
                            }

                            //---- Remise ----

                            string MontantRemise = "";
                            string PourcentageRemise = "";

                            if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "0")
                            {
                                MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur.Replace(",", ".");
                                PourcentageRemise = "";
                            }

                            if (FacturesAExporter[i].lines[j].DL_Remise01REM_Type == "1")
                            {
                                MontantRemise = "";
                                PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                            }

                            //if (FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur != "0")
                            //{
                                writer.WriteLine("DEMAT-DED;;A;;;;;;;" + FacturesAExporter[i].lines[j].DL_Remise01REM_Type + ";" + MontantRemise + ";" + PourcentageRemise + ";;");
                                writer.WriteLine("");
                            //}


                            if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "0")
                            {
                                MontantRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                                PourcentageRemise = "";
                            }

                            if (FacturesAExporter[i].lines[j].DL_Remise03REM_Type == "1")
                            {
                                MontantRemise = "";
                                PourcentageRemise = FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur;
                            }

                            if ((FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != "0") && (FacturesAExporter[i].lines[j].DL_Remise03REM_Valeur != FacturesAExporter[i].lines[j].DL_Remise01REM_Valeur))
                            {
                                writer.WriteLine("DEMAT-DED;;A;;;;;;;;" + MontantRemise + ";" + PourcentageRemise + ";;");
                                writer.WriteLine("");
                            }

                        }

                        //  Les lignes des taxes


                        if (FacturesAExporter[i].DO_TypeTaxe1 == "0")
                        {
                            FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Débit";
                        }
                        if (FacturesAExporter[i].DO_TypeTaxe2 == "0")
                        {
                            FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Débit";
                        }
                        if (FacturesAExporter[i].DO_TypeTaxe3 == "0")
                        {
                            FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Débit";
                        }

                        if (FacturesAExporter[i].DO_TypeTaxe1 == "1")
                        {
                            FacturesAExporter[i].DO_TypeTaxe1 = "TVA/Encaissement";
                        }
                        if (FacturesAExporter[i].DO_TypeTaxe2 == "1")
                        {
                            FacturesAExporter[i].DO_TypeTaxe2 = "TVA/Encaissement";
                        }

                        if (FacturesAExporter[i].DO_TypeTaxe3 == "1")
                        {
                            FacturesAExporter[i].DO_TypeTaxe3 = "TVA/Encaissement";
                        }

                        // Calcule taxe
                        decimal montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe1.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                        writer.WriteLine("DEMAT-TTX;1;;" + FacturesAExporter[i].DO_TypeTaxe1.Replace(",", ".") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",",".") + ";;");
                            writer.WriteLine("");
                        
                        //if (FacturesAExporter[i].DO_taxe2 != FacturesAExporter[i].DO_taxe1 && FacturesAExporter[i].DO_taxe2 != "0")
                        
                        if (FacturesAExporter[i].DO_taxe2 != "0")
                        {
                            // Calcule taxe
                            montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                            writer.WriteLine("DEMAT-TTX;2;;" + FacturesAExporter[i].DO_TypeTaxe2.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",", ".") + ";;");
                            writer.WriteLine("");
                        }

                        if (FacturesAExporter[i].DO_taxe3 != "0")
                           // if ((FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe1) && (FacturesAExporter[i].DO_taxe3 != FacturesAExporter[i].DO_taxe2) && FacturesAExporter[i].DO_taxe3 != "0")
                        {
                            // Calcule taxe
                            montantTaxe1 = (Decimal.Parse(FacturesAExporter[i].FNT_TotalHT.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) * (Decimal.Parse(FacturesAExporter[i].DO_taxe3.Replace(",", "."), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture) / 100));

                            writer.WriteLine("DEMAT-TTX;3;;" + FacturesAExporter[i].DO_TypeTaxe3.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].DO_taxe1.Replace(",", ".") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + Math.Round(montantTaxe1, 2).ToString().Replace(",", ".") + ";;");
                            writer.WriteLine("");
                        }


                        writer.WriteLine("DEMAT-END;;;" + FacturesAExporter[i].DO_Piece.Replace(prefix, "") + ";" + FacturesAExporter[i].FNT_TotalHT.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_TotalTTC.Replace(",", ".").Replace("00000", "") + ";" + FacturesAExporter[i].FNT_MontantTotalTaxes.Replace(",", ".") + ";;;;" + FacturesAExporter[i].FNT_Escompte.Replace(",", ".") + ";;" + FacturesAExporter[i].FNT_NetAPayer.Replace(",", ".") + ";;;;");
                        writer.WriteLine("");
                        writer.WriteLine("");
                    }

                    writer.WriteLine("DEMAT-ZZZ;v01.0;;;;");

                    logFileWriter.WriteLine(DateTime.Now + " | ExportFacture() : Facture Exporté.");
                }

                MessageBox.Show("Nombre de facture : " + FacturesAExporter.Count, "Information !!",
                                             MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                logFileWriter.Close();
            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                logFileWriter.WriteLine(DateTime.Now + " | ExportStock() : ERREUR :: " + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""));
                logFileWriter.Close();
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            //importButton.Enabled = true;

            if(!Directory.Exists(logDirectoryName_export))
            {
                Directory.CreateDirectory(logDirectoryName_export);
            }

            var logFileName_export = logDirectoryName_export + @"\" + string.Format("LOG_Export_Facture_{0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile_export = File.Create(logFileName_export);

            //Write in the log file 
            logFileWriter_export = new StreamWriter(logFile_export);
            //logFileWriter.Write(string.Format("{0:HH:mm:ss}", DateTime.Now) + " \r\n");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("################################ ConnecteurSage Sage ################################");
            logFileWriter_export.WriteLine("#####################################################################################");
            logFileWriter_export.WriteLine("");

            ExportFacture(logFileWriter_export);

            //logFileWriter_export.Close();
            Close();
        }

        private string getDeviseIso(string code)
        {
            try
            {
                using (OdbcConnection connection = ConnexionManager.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(false, code), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader[0].ToString();
                            }
                        }
                    }
                    return null;

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
    }
}
