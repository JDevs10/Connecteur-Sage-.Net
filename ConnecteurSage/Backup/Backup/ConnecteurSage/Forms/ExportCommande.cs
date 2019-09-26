﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnecteurSage.Classes;
using System.Data.Odbc;
using ConnecteurSage.Helpers;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace ConnecteurSage.Forms
{
    public partial class ExportCommande : Form
    {
        #region Champs privés
        /// <summary>
        /// commande à exporter
        /// </summary>
        private Order CommandeAExporter;

        #endregion

        #region Constructeurs
        /// <summary>
        /// Création d'une instance de ImportOrdersForm
        /// </summary>
        public ExportCommande()
        {
            InitializeComponent();
        }
        #endregion

        #region Intéractions avec l'application

        private List<Order> GetCommandesFromDataBase()
        {
            try
            {
            //DocumentVente Facture = new DocumentVente();
            List<Order> listCommande = new List<Order>();
             using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
            {
               
                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getListCommandes(), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                           while(reader.Read())
                            {
                                Order order = new Order(reader[0].ToString(), reader[1].ToString(), 
                                    reader[2].ToString().Replace(", ",",")+"."+reader[3].ToString()+"."+reader[6].ToString()+"."+reader[7].ToString(),
                                    reader[8].ToString(), reader[9].ToString().Replace("00:00:00",""),
                                    reader[10].ToString(), reader[11].ToString(),
                                    reader[12].ToString(), reader[13].ToString(), reader[15].ToString(),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[0] : null),
                                    (reader[14].ToString().Split(';').Length == 2 ? reader[14].ToString().Split(';')[1] : null),
                                    reader[16].ToString()
                                    );
                                listCommande.Add(order);
                            }
                        }
                    }
                    return listCommande;

            }

                                 }

                catch (Exception e)
                {
                    //Exceptions pouvant survenir durant l'exécution de la requête SQL
                    MessageBox.Show(e.Message);
                    return null;
                }
        }

       




        /// <summary>
        /// Génération du fichier d'import, lancement de l'application et import des commandes
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

                if (CommandeAExporter.deviseCommande == "0")
                {
                    CommandeAExporter.deviseCommande = "1";
                }

                if (CommandeAExporter.deviseCommande != "")
                {
                    CommandeAExporter.deviseCommande = getDeviseIso(CommandeAExporter.deviseCommande);
                }

                if (CommandeAExporter.DO_MOTIF == "")
                {
                    CommandeAExporter.DO_MOTIF = CommandeAExporter.NumCommande;
                }

                if (CommandeAExporter.DO_MOTIF == "")
                {
                    DialogResult resultDialog = MessageBox.Show("N° de commande non enregistrer.\nVoulez vous Continuer ?",
                                                     "Worning Message !!",
                                                     MessageBoxButtons.OKCancel,
                                                     MessageBoxIcon.Warning,
                                                     MessageBoxDefaultButton.Button2);

                    if (resultDialog == DialogResult.Cancel)
                    {
                        goto jamp;
                    }

                    if (resultDialog == DialogResult.OK)
                    {
                        CommandeAExporter.DO_MOTIF = "";
                    }

                }

                if (CommandeAExporter.codeClient == "")
                {
                    DialogResult resultDialog = MessageBox.Show("Code GNL client n'est pas enregistrer.\nVoulez vous continuer ?",
                                                     "Worning Message !!",
                                                     MessageBoxButtons.OKCancel,
                                                     MessageBoxIcon.Warning,
                                                     MessageBoxDefaultButton.Button2);

                    if (resultDialog == DialogResult.Cancel)
                    {
                        goto jamp;
                    }

                    if (resultDialog == DialogResult.OK)
                    {
                        CommandeAExporter.codeClient = "";
                    }

                }

                if (!IsNumeric(CommandeAExporter.DO_MOTIF) && CommandeAExporter.DO_MOTIF != "")
                {
                    DialogResult resultDialog = MessageBox.Show("N° de commande est mal enregistrer.\nVoulez vous Continuer ?",
                                                     "Worning Message !!",
                                                     MessageBoxButtons.OKCancel,
                                                     MessageBoxIcon.Warning,
                                                     MessageBoxDefaultButton.Button2);

                    if (resultDialog == DialogResult.Cancel)
                    {
                        goto jamp;
                    }

                    if (resultDialog == DialogResult.OK)
                    {
                        CommandeAExporter.DO_MOTIF = "";
                    }

                }

                if (!IsNumeric(CommandeAExporter.codeClient) && CommandeAExporter.codeClient != "")
                {
                    DialogResult resultDialog = MessageBox.Show("Code GNL client est mal enregistrer.\nVoulez vous continuer ?",
                                                     "Worning Message !!",
                                                     MessageBoxButtons.OKCancel,
                                                     MessageBoxIcon.Warning,
                                                     MessageBoxDefaultButton.Button2);

                    if (resultDialog == DialogResult.Cancel)
                    {
                        goto jamp;
                    }

                    if (resultDialog == DialogResult.OK)
                    {
                        CommandeAExporter.DO_MOTIF = "";
                    }

                }


                var fileName = string.Format("EDI_ORDERS." + CommandeAExporter.codeClient + "." + CommandeAExporter.NumCommande + "." + ConvertDate(CommandeAExporter.DateCommande) + "."+ CommandeAExporter.adresseLivraison + ".{0:yyyyMMddhhmmss}.csv", DateTime.Now);

                fileName = fileName.Replace("...", ".");

                using (StreamWriter writer = new StreamWriter(textBox1.Text+@"\"+fileName.Replace("..","."), false, Encoding.UTF8))
                {


                    writer.WriteLine("ORDERS;" + CommandeAExporter.DO_MOTIF + ";" + CommandeAExporter.codeClient + ";" + CommandeAExporter.codeAcheteur + ";" + CommandeAExporter.codeFournisseur + ";;;" + CommandeAExporter.nom_contact + "." + CommandeAExporter.adresseLivraison.Replace("..", ".").Replace("...", ".") + ";" + CommandeAExporter.deviseCommande + ";;");
                    
                    if (CommandeAExporter.DateCommande != "")
                    {
                        CommandeAExporter.DateCommande = ConvertDate(CommandeAExporter.DateCommande);
                    }

                    //if (CommandeAExporter.DateCommande != " ")
                    //{
                    //    CommandeAExporter.conditionLivraison = "";
                    //}

                    writer.WriteLine("ORDHD1;" + CommandeAExporter.DateCommande + ";" + CommandeAExporter.conditionLivraison + ";;");

                    CommandeAExporter.Lines = getLigneCommande(CommandeAExporter.NumCommande);

                    for (int i = 0; i < CommandeAExporter.Lines.Count;i++ )
                    {
                        if (!IsNumeric(CommandeAExporter.Lines[i].codeAcheteur))
                        {
                            CommandeAExporter.Lines[i].codeAcheteur = "";
                        }

                        if (!IsNumeric(CommandeAExporter.Lines[i].codeFournis))
                        {
                            CommandeAExporter.Lines[i].codeFournis = "";
                        }

                        writer.WriteLine("ORDLIN;" + CommandeAExporter.Lines[i].NumLigne + ";" + CommandeAExporter.Lines[i].codeArticle + ";GS1;" + CommandeAExporter.Lines[i].codeAcheteur + ";" + CommandeAExporter.Lines[i].codeFournis + ";;A;" + CommandeAExporter.Lines[i].descriptionArticle + ";" + CommandeAExporter.Lines[i].Quantite.Replace(",", ".") + ";LM;" + CommandeAExporter.Lines[i].MontantLigne.Replace(",", ".") + ";;;" + CommandeAExporter.Lines[i].PrixNetHT.Replace(",", ".") + ";;;LM;;;;" + ConvertDate(CommandeAExporter.Lines[i].DateLivraison) + ";");
                    }
                    writer.WriteLine("ORDEND;" + CommandeAExporter.MontantTotal.Replace(",", ".") + ";");


                    
                }

                MessageBox.Show("Commande exportée avec succés" , "Information !!",
                                             MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                jamp :
                Close();

                
                
            }
            catch (Exception ex)
            {
                //Exception pouvant survenir si lorsque l'accès au disque dur est refusé
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Méthodes diverses
        /// <summary>
        /// Chargement de la fenêtre
        /// </summary>
        /// <param name="e">paramètres de l'évènement</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {

                textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Initialize the dialog that will contain the progress bar
                ProgressDialog progressDialog = new ProgressDialog();

                // Initialize the thread that will handle the background process
                Thread backgroundThread = new Thread(
                    new ThreadStart(() =>
                    {
                        // Set the flag that indicates if a process is currently running
                        //isProcessRunning = true;
                        for (int n = 0; n < 25; n++)
                        {
                            Thread.Sleep(1);
                            progressDialog.UpdateProgress(n);
                        }

                        //Affichage des clients du dossier
                        if (customersDataGridView.InvokeRequired)
                        {
                            customersDataGridView.Invoke(new MethodInvoker(delegate
                            {
                                customersDataGridView.DataSource = GetCommandesFromDataBase();
                                for (int n = 26; n < 45; n++)
                                {
                                    Thread.Sleep(1);
                                    progressDialog.UpdateProgress(n);
                                }
                                importButton.Enabled = customersDataGridView.Rows.Count > 0;

                                if (customersDataGridView.Columns["Id"] != null)
                                    customersDataGridView.Columns["Id"].Visible = false;
                                if (customersDataGridView.Columns["NumCommande"] != null)
                                    customersDataGridView.Columns["NumCommande"].HeaderText = " Id Commande";
                                if (customersDataGridView.Columns["NomClient"] != null)
                                    customersDataGridView.Columns["NomClient"].HeaderText = "Client";
                                if (customersDataGridView.Columns["codeFournisseur"] != null)
                                    customersDataGridView.Columns["codeFournisseur"].Visible = false;
                                if (customersDataGridView.Columns["adresseLivraison"] != null)
                                    customersDataGridView.Columns["adresseLivraison"].HeaderText = "Adresse livraison";
                                if (customersDataGridView.Columns["DateCommande"] != null)
                                    customersDataGridView.Columns["DateCommande"].HeaderText = "Date";
                                if (customersDataGridView.Columns["MontantTotal"] != null)
                                    customersDataGridView.Columns["MontantTotal"].HeaderText = "Montant Total";
                                if (customersDataGridView.Columns["deviseCommande"] != null)
                                    customersDataGridView.Columns["deviseCommande"].Visible = false;
                                if (customersDataGridView.Columns["StockId"] != null)
                                    customersDataGridView.Columns["StockId"].Visible = false;
                                if (customersDataGridView.Columns["DateLivraison"] != null)
                                    customersDataGridView.Columns["DateLivraison"].Visible = false;
                                if (customersDataGridView.Columns["conditionLivraison"] != null)
                                    customersDataGridView.Columns["conditionLivraison"].Visible = false;
                                if (customersDataGridView.Columns["codeClient"] != null)
                                    customersDataGridView.Columns["codeClient"].Visible = false;
                                if (customersDataGridView.Columns["Reference"] != null)
                                    customersDataGridView.Columns["Reference"].Visible = false;
                                if (customersDataGridView.Columns["Commentaires"] != null)
                                    customersDataGridView.Columns["Commentaires"].Visible = false;
                                if (customersDataGridView.Columns["villeReference"] != null)
                                    customersDataGridView.Columns["villeReference"].Visible = false;
                                if (customersDataGridView.Columns["DO_MOTIF"] != null)
                                    customersDataGridView.Columns["DO_MOTIF"].HeaderText = "N° commande";
                                if (customersDataGridView.Columns["codeAcheteur"] != null)
                                    customersDataGridView.Columns["codeAcheteur"].Visible = false;

                                if (customersDataGridView.Columns["adresse"] != null)
                                    customersDataGridView.Columns["adresse"].Visible = false;
                                if (customersDataGridView.Columns["codepostale"] != null)
                                    customersDataGridView.Columns["codepostale"].Visible = false;
                                if (customersDataGridView.Columns["ville"] != null)
                                    customersDataGridView.Columns["ville"].Visible = false;
                                if (customersDataGridView.Columns["pays"] != null)
                                    customersDataGridView.Columns["pays"].Visible = false;
                                if (customersDataGridView.Columns["do_coord01"] != null)
                                    customersDataGridView.Columns["do_coord01"].Visible = false;
                                if (customersDataGridView.Columns["nom_contact"] != null)
                                    customersDataGridView.Columns["nom_contact"].Visible = false;

                                if (customersDataGridView.Columns["FNT_MONTANTTOTALTAXES"] != null)
                                    customersDataGridView.Columns["FNT_MONTANTTOTALTAXES"].Visible = false;
                                if (customersDataGridView.Columns["DL_MONTANTHT"] != null)
                                    customersDataGridView.Columns["DL_MONTANTHT"].Visible = false;
                                if (customersDataGridView.Columns["DL_MONTANTTTC"] != null)
                                    customersDataGridView.Columns["DL_MONTANTTTC"].Visible = false;
                               
                                        //Récupération du prochain identifiant de commande à utiliser
                                //string nextOrderId = GetNextOrderId();
                            }));
                        }

                        for (int n = 46; n < 100; n++)
                        {
                            Thread.Sleep(1);
                            progressDialog.UpdateProgress(n);
                        }

                        // Close the dialog if it hasn't been already
                        if (progressDialog.InvokeRequired)
                            progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));

                        // Reset the flag that indicates if a process is currently running
                        //isProcessRunning = false;
                    }
                ));

                // Start the background process thread
                backgroundThread.Start();

                // Open the dialog
                progressDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message.Replace("[CBase]", "").Replace("[Simba]", " ").Replace("[Simba ODBC Driver]", "").Replace("[Microsoft]", " ").Replace("[Gestionnaire de pilotes ODBC]", "").Replace("[SimbaEngine ODBC Driver]", " ").Replace("[DRM File Library]", ""), "Erreur!!",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        public static string ConvertDate(string date)
        {
            if (date.Length == 11 || date.Length == 19)
            {
                return date.Substring(6, 4) + date.Substring(3, 2) + date.Substring(0, 2);
            }
            return date;
        }

        /// <summary>
        /// Fermeture de la fenêtre
        /// </summary>
        /// <param name="sender">objet déclenchant l'évènement</param>
        /// <param name="e">paramètres de l'évènement</param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Remplissage des infos d'adresse du client sélectionné
        /// </summary>
        /// <param name="sender">objet déclenchant l'évènement</param>
        /// <param name="e">paramètres de l'évènement</param>
        private void customersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (customersDataGridView.SelectedRows.Count == 0)
            {
                importButton.Enabled = false;
                return;
            }
            Order order = customersDataGridView.SelectedRows[0].DataBoundItem as Order;
            if (order == null)
                throw new NullReferenceException("order");

            CommandeAExporter = order;

        }

        /// <summary>
        /// Lancement de l'import
        /// </summary>
        /// <param name="sender">objet déclenchant l'évènement</param>
        /// <param name="e">paramètres de l'évènement</param>
        private void importButton_Click(object sender, EventArgs e)
        {                        
            importButton.Enabled = false;

            ExportFacture();
        }
        #endregion

        private void itemsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void customersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ImportOrdersForm_Load(object sender, EventArgs e)
        {

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

        private string getDeviseIso(string code)
        {
            try
            {
                using (OdbcConnection connection = Connexion.CreateOdbcConnextion())
                {

                    connection.Open();
                    //Exécution de la requête permettant de récupérer les articles du dossier
                    OdbcCommand command = new OdbcCommand(QueryHelper.getDeviseIso(code), connection);
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

        private List<OrderLine> getLigneCommande(string code)
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

        public static Boolean IsNumeric(string Nombre)
        {
            try
            {
                int.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
