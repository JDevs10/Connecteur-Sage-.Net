using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using ConnecteurSage.Classes;
using System.Xml.Serialization;
using System.IO;
using ConnecteurSage.Helpers;

namespace ConnecteurSage.Forms
{
    public partial class ConfExport : Form
    {
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
 
        public ConfExport()
        {
            InitializeComponent();


            Boolean[] list1 = new Boolean[2];   //List pour activer ou desactiver l'export
            list1[0] = true;
            list1[1] = false;

            string[] list2 = new string[3];     //List des formats d'export
            list2[0] = "";
            list2[1] = "Plat";
            list2[2] = "Véolog";

            string[] list3 = new string[4];     //List des statuts commande
            list3[0] = "";
            list3[1] = "Saisie";
            list3[2] = "Confirmé";
            list3[3] = "A Préparé";

            string[] list4 = new string[4];     //List des statuts DSADV
            list4[0] = "";
            list4[1] = "Saisie";
            list4[2] = "Confirmé";
            list4[3] = "A Facturer";

            string[] list5 = new string[4];     //List des statuts Facture
            list5[0] = "";
            list5[1] = "Saisie";
            list5[2] = "Confirmé";
            list5[3] = "A comptabiliser";

            string[] list6 = new string[1];     //List des statuts stock
            list6[0] = "";


            // Init les comboBoxs
            for (int i = 0; i < getValuesExport().Length; i++)  
            {
                comboBox1.Items.Add( ((list1[i] == true) ? "Activer" : "Désactiver") );
                comboBox2.Items.Add( ((list1[i] == true) ? "Activer" : "Désactiver") );
                comboBox3.Items.Add( ((list1[i] == true) ? "Activer" : "Désactiver") );
                comboBox4.Items.Add( ((list1[i] == true) ? "Activer" : "Désactiver") );
            }
            for (int i = 0; i < getFormatExport().Length; i++)
            {
                comboBox5.Items.Add(list2[i]);
                comboBox8.Items.Add(list2[i]);
                comboBox7.Items.Add(list2[i]);
                comboBox6.Items.Add(list2[i]);
            }
            for (int i = 0; i < getStatutExport().GetLength(0); i++)
            {
                comboBox12.Items.Add(list3[i]);
            }
            for (int i = 0; i < getStatutExport_DESADV().GetLength(0); i++)
            {
                comboBox9.Items.Add(list4[i]);
            }
            for (int i = 0; i < getStatutExport_FACTURE().GetLength(0); i++)
            {
                comboBox10.Items.Add(list5[i]);
            }

            Config_Export.ConfigurationSaveLoad settings = new Config_Export.ConfigurationSaveLoad();
            if (settings.isSettings())
            {
                settings.Load();
                Config_Export.Classes.ConfigurationExport configurationExport = settings.configurationExport;

                comboBox1.Text = ((configurationExport.Commande.Activate) ? "Activer" : "Désactiver");
                comboBox2.Text = ((configurationExport.DSADV.Activate) ? "Activer" : "Désactiver");
                comboBox3.Text = ((configurationExport.Facture.Activate) ? "Activer" : "Désactiver");
                comboBox4.Text = ((configurationExport.Stock.Activate) ? "Activer" : "Désactiver");

                comboBox5.Text = configurationExport.Commande.Format;
                comboBox8.Text = configurationExport.DSADV.Format;
                comboBox7.Text = configurationExport.Facture.Format;
                comboBox6.Text = configurationExport.Stock.Format;

                comboBox12.Text = getStatutName(configurationExport.Commande.Status);
                comboBox9.Text = getStatutName_DESADV(configurationExport.DSADV.Status);
                comboBox10.Text = getStatutName_FACTURE(configurationExport.Facture.Status);
            }

        }

        public static Boolean[] getValuesExport()
        {
            Boolean[] list1 = new Boolean[2];
            list1[0] = true;
            list1[1] = false;

            return list1;
        }
        public static string[] getFormatExport()
        {
            string[] list = new string[3];
            list[0] = "";
            list[1] = "Plat";
            list[2] = "Véolog";
            return list;
        }
        public static string[,] getStatutExport()
        {
            string[,] list = new string[4,2];
            list[0, 0] = "";            list[0, 1] = "";
            list[1, 0] = "Saisie";      list[1, 1] = "0";   // Order created
            list[2, 0] = "Confirmé";    list[2, 1] = "1";   // Order ready to be send in EDI to Veolog
            list[3, 0] = "A Préparé";   list[3, 1] = "2";   // Order send to Veolog
            return list;
        }
        public static string[,] getStatutExport_DESADV()
        {
            string[,] list = new string[4, 2];
            list[0, 0] = "";            list[0, 1] = "";
            list[1, 0] = "Saisie";      list[1, 1] = "0";   // Order created
            list[2, 0] = "Confirmé";    list[2, 1] = "1";   // Order ready to be send in EDI to Veolog
            list[3, 0] = "A Facturer";  list[3, 1] = "2";   // Order send to Veolog
            return list;
        }
        public static string[,] getStatutExport_FACTURE()
        {
            string[,] list = new string[4, 2];
            list[0, 0] = "";                list[0, 1] = "";
            list[1, 0] = "Saisie";          list[1, 1] = "0";   // Order created
            list[2, 0] = "Confirmé";        list[2, 1] = "1";   // Order ready to be send in EDI to Veolog
            list[3, 0] = "A comptabiliser"; list[3, 1] = "2";   // Order send to Veolog
            return list;
        }
        public static string getStatutValue(string value)
        {
            string result = "";
            for(int x=0; x<getStatutExport().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport()[x, 0]))
                {
                    result = getStatutExport()[x, 1];
                    break;
                }
            }
            return result;
        }
        public static string getStatutValue_DESADV(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport_DESADV().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport_DESADV()[x, 0]))
                {
                    result = getStatutExport_DESADV()[x, 1];
                    break;
                }
            }
            return result;
        }
        public static string getStatutValue_FACTURE(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport_FACTURE().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport_FACTURE()[x, 0]))
                {
                    result = getStatutExport_FACTURE()[x, 1];
                    break;
                }
            }
            return result;
        }
        public static string getStatutName(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport()[x, 1]))
                {
                    result = getStatutExport()[x, 0];
                    break;
                }
            }
            return result;
        }
        public static string getStatutName_DESADV(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport_DESADV().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport_DESADV()[x, 1]))
                {
                    result = getStatutExport_DESADV()[x, 0];
                    break;
                }
            }
            return result;
        }
        public static string getStatutName_FACTURE(string value)
        {
            string result = "";
            for (int x = 0; x < getStatutExport_FACTURE().GetLength(0); x++)
            {
                if (value.Equals(getStatutExport_FACTURE()[x, 1]))
                {
                    result = getStatutExport_FACTURE()[x, 0];
                    break;
                }
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) || !string.IsNullOrEmpty(comboBox2.Text) || !string.IsNullOrEmpty(comboBox3.Text) || !string.IsNullOrEmpty(comboBox4.Text) ||
                !string.IsNullOrEmpty(comboBox5.Text) || !string.IsNullOrEmpty(comboBox8.Text) || !string.IsNullOrEmpty(comboBox7.Text) || !string.IsNullOrEmpty(comboBox6.Text) ||
                !string.IsNullOrEmpty(comboBox12.Text) || !string.IsNullOrEmpty(comboBox9.Text) || !string.IsNullOrEmpty(comboBox10.Text))
            {
                Config_Export.ConfigurationSaveLoad settings = new Config_Export.ConfigurationSaveLoad();
                Config_Export.Classes.ConfigurationExport configurationExport = new Config_Export.Classes.ConfigurationExport(
                    new Config_Export.Classes.Custom.Commande(((comboBox1.Text == "Activer") ? true : false), comboBox5.Text, getStatutValue(comboBox12.Text)),
                    new Config_Export.Classes.Custom.DSADV(((comboBox2.Text == "Activer") ? true : false), comboBox8.Text, getStatutValue_DESADV(comboBox9.Text)),
                    new Config_Export.Classes.Custom.Facture(((comboBox3.Text == "Activer") ? true : false), comboBox7.Text, getStatutValue_FACTURE(comboBox10.Text)),
                    new Config_Export.Classes.Custom.Stock(((comboBox4.Text == "Activer") ? true : false), comboBox6.Text)
                );

                settings.configurationExport = configurationExport;
                settings.saveInfo();

                Close();
            }
            else {
                if (!string.IsNullOrEmpty(comboBox1.Text))
                {
                    MessageBox.Show("Export Commande est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox2.Text))
                {
                    MessageBox.Show("Export Bon de Livraision est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox3.Text))
                {
                    MessageBox.Show("D'Export Facture est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox4.Text))
                {
                    MessageBox.Show("D'Export Stock est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox5.Text))
                {
                    MessageBox.Show("Le Format d'Export Commande est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox8.Text))
                {
                    MessageBox.Show("Le Format d'Export Bon de Livraision est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox7.Text))
                {
                    MessageBox.Show("Le Format d'Export Facture est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox6.Text))
                {
                    MessageBox.Show("Le Format d'Export Stock est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox12.Text))
                {
                    MessageBox.Show("Le Statut d'Export Commande est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox9.Text))
                {
                    MessageBox.Show("Le Statut d'Export Bon de Livraision est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox10.Text))
                {
                    MessageBox.Show("Le Statut d'Export Facture est obligatoire !!");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Activer")
            {
                comboBox5.Enabled = true;
                comboBox12.Enabled = true;
            }
            else
            {
                comboBox5.Enabled = false;
                comboBox12.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Activer")
            {
                comboBox8.Enabled = true;
                comboBox9.Enabled = true;
            }
            else
            {
                comboBox8.Enabled = false;
                comboBox9.Enabled = false;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "Activer")
            {
                comboBox7.Enabled = true;
                comboBox10.Enabled = true;
            }
            else
            {
                comboBox7.Enabled = false;
                comboBox10.Enabled = false;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.Text == "Activer")
            {
                comboBox6.Enabled = true;
            }
            else
            {
                comboBox6.Enabled = false;
            }
        }
    }
}
