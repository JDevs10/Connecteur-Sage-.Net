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
                        form.ShowDialog();
                    }
                }
                // Récupération d'une possible SDKException
                catch (SDKException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (File.Exists(pathModule + @"\SettingExport.xml"))
            {
                //XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                //StreamReader file = new System.IO.StreamReader("Setting.xml");
                //ConfigurationDNS setting = new ConfigurationDNS();
                //setting = (ConfigurationDNS)reader.Deserialize(file);

                ConfigurationExport setting = new ConfigurationExport();
                setting.Load();

                label6.Text = "Statut d'Export Commande : " + ((setting.exportBonsCommandes == "True") ? "Activer":"Désactiver");
                label7.Text = "Statut d'Export Livraision : " + ((setting.exportBonsLivraisons == "True") ? "Activer":"Désactiver");
                label8.Text = "Statut d'Export Facture : " + ((setting.exportFactures == "True") ? "Activer":"Désactiver");
                label10.Text = "Statut Export Stock : "+((setting.exportStock == "True") ? "Activer":"Désactiver");
            }
            else
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
            string checkTable = null;
            try
            {
                //Verifie si la table 'Velog_Pending' avec id, cmd_id, cmd_ref, statut
                using (OdbcConnection connection = Connexion.CreateOdbcConnexionSQL())
                {
                    connection.Open();
                    OdbcCommand command = new OdbcCommand(QueryHelper.checkVelog_PendingTable(true), connection);
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader[0].ToString().Equals("0"))
                                {
                                    checkTable = reader[0].ToString();

                                } else if (reader[0].ToString().Equals("1"))
                                {
                                    checkTable = reader[0].ToString();
                                }
                            }
                        }
                    }

                    //Si lea table n'existe pas alors creer la
                    if (checkTable != null && checkTable.Equals("0"))
                    {
                        OdbcCommand command1 = new OdbcCommand(QueryHelper.createVelog_PendingTable(true), connection);
                        {
                            using (IDataReader reader = command1.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine("Table \"Velog_Pending\" est créé !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                    else if(checkTable != null && checkTable.Equals("1"))
                    {
                        Console.WriteLine("Table \"Velog_Pending\" existe !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
            catch (Exception ex)
            {
                //Exceptions pouvant survenir durant l'exécution de la requête SQL
                Console.WriteLine(ex.Message);
            }
        }
    }
}
