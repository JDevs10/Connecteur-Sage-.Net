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


        private List<DocumentVente> GetBonLivraisonFromDataBase(string client)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVente> listDocumentVente = new List<DocumentVente>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    DocumentVente documentVente;
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVente(client, 3), connection);
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
                                     reader[24].ToString(), reader[25].ToString(), reader[26].ToString(), reader[27].ToString(), reader[28].ToString(), reader[29].ToString()
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

        private List<DocumentVenteLine> getDocumentLine(string codeDocument)
        {
            try
            {
                //DocumentVente Facture = new DocumentVente();
                List<DocumentVenteLine> lignesDocumentVente = new List<DocumentVenteLine>();
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListDocumentVenteLine(codeDocument), connection);
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
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListClient(), connection);
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
                                if (customersDataGridView.Columns["CT_EDI1"] != null)
                                    customersDataGridView.Columns["CT_EDI1"].HeaderText = "GLN";
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
                                        BonLivraisonDataGridView.DataSource = GetBonLivraisonFromDataBase(customer.CT_Num);

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
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {
                    List<OrderLine> lines = new List<OrderLine>();

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListLignesCommandes(code), connection);
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

                BonLivraisonDataGridView.DataSource = GetBonLivraisonFromDataBase(customer.CT_Num);
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
        private void ExportFacture()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Le chemin du fichier d'import de commande doit être renseigné");
                    return;
                }


                var fileName = string.Format("BonLivraison{0:yyyyMMdd}." + customer.CT_Num + "." + customer.CT_EDI1 + ".csv", DateTime.Now);

                using (StreamWriter writer = new StreamWriter(textBox1.Text + @"\" + fileName, false, Encoding.UTF8))
                {
                    //writer.WriteLine("DEMAT-AAA;v01.0;;;" + DateTime.Today.Year + addZero(DateTime.Today.Month.ToString()) + addZero(DateTime.Today.Day.ToString()) + ";;");
                    //writer.WriteLine("");
                    //writer.WriteLine("");

                    for (int i = 0; i < BonLivrasonAExporter.Count; i++)
                    {
                        //string EANClient = GetEANClient(BonLivrasonAExporter[i].CustomerId);

                        //string[] tab = new string[] { "", "", "" };



                        //if (BonLivrasonAExporter[i].OriginDocumentType == "8")
                        //{ // Return la commande d'origin                            
                        //    tab = GetCommandeFacture(BonLivrasonAExporter[i].Id).Split(';');
                        //}




                        writer.WriteLine("DESHDR;v01.0;;" + BonLivrasonAExporter[i].DO_Piece.Replace("BL", "") + ";" + customer.CT_EDI1 + ";9;;9;" + customer.CT_EDI1 + ";9;" + customer.CT_EDI1 + ";9;;9;;9;;9;;" + ConvertDate(BonLivrasonAExporter[i].DO_dateLivr) + ";;;;;" + BonLivrasonAExporter[i].LI_ADRESSE + ";;;;;;;;;;;;;;9;");
                        writer.WriteLine("");

                        writer.WriteLine("DESHD2;;;;" + customer.CT_Adresse + ";;" + customer.CT_CodePostal + ";" + customer.CT_Ville + ";" + customer.CT_Pays + ";" + customer.CT_Intitule + ";" + customer.CT_Telephone + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                        writer.WriteLine("");

                        //if (BonLivrasonAExporter[i].IntrastatTransportMode != "")
                        //{ // Return mode de transport                           
                        //    BonLivrasonAExporter[i].IntrastatTransportMode = GetModeTransport(BonLivrasonAExporter[i].IntrastatTransportMode);
                        //}

                        writer.WriteLine("DESTRP;;;;;;;;;;");
                        writer.WriteLine("");

                        writer.WriteLine("DESLOG;;;;" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].FNT_PoidsNet.Replace(",", ".") + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
                        writer.WriteLine("");


                        BonLivrasonAExporter[i].lines = getDocumentLine(BonLivrasonAExporter[i].DO_Piece);

                        for (int j = 0; j < BonLivrasonAExporter[i].lines.Count; j++)
                        {

                            writer.WriteLine("DESLIN;" + BonLivrasonAExporter[i].lines[j].DL_Ligne + ";;" + BonLivrasonAExporter[i].lines[j].AR_CODEBARRE + ";;;;;;;" + BonLivrasonAExporter[i].lines[j].DL_Design + ";;;" + BonLivrasonAExporter[i].lines[j].DL_Qte + ";;" + BonLivrasonAExporter[i].lines[j].EU_Qte + ";;;;;;;;;;;" + ConvertDate(BonLivrasonAExporter[i].lines[j].DO_DateLivr) + ";;;" + BonLivrasonAExporter[i].lines[j].FNT_PrixUNet.Replace(",", ".") + ";;;;;;" + BonLivrasonAExporter[i].lines[j].FNT_MontantHT.Replace(",", ".") + ";;" + BonLivrasonAExporter[i].lines[j].DL_NoColis + ";;;;;;;;;;;");
                            writer.WriteLine("");


                        }


                        writer.WriteLine("DESEND;" + BonLivrasonAExporter[i].lines.Count + ";;;" + BonLivrasonAExporter[i].FNT_TotalHTNet.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_TotalHT.Replace(",", ".") + ";" + BonLivrasonAExporter[i].FNT_PoidsBrut.Replace(",", ".") + ";;;;;");
                        writer.WriteLine("");
                        writer.WriteLine("");
                    }



                }

                MessageBox.Show("Nombre bon de livraison : " + BonLivrasonAExporter.Count, "Information !!",
                                             MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


                Close();



            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            //importButton.Enabled = true;
            ExportFacture();
        }

        private void closeButton_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
