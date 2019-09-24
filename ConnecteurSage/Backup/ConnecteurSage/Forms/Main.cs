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

            if (File.Exists(pathModule+@"\Setting.xml"))
            {
                //XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                //StreamReader file = new System.IO.StreamReader("Setting.xml");
                //ConfigurationDNS setting = new ConfigurationDNS();
                //setting = (ConfigurationDNS)reader.Deserialize(file);

                ConfigurationDNS setting = new ConfigurationDNS();
                setting.Load();

                label1.Text = "DSN  : " + setting.DNS;
                label2.Text = "Nom : " + setting.Nom;
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


    }
}
