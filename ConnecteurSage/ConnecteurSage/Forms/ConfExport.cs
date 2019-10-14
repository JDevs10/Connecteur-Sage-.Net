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
            list2[2] = "Velog";

            string[] list3 = new string[4];     //List des statuts
            list3[0] = "";
            list3[1] = "La Saisie";
            list3[2] = "Confirmé";
            list3[3] = "Accepté";


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
                comboBox9.Items.Add(list3[i]);
                comboBox10.Items.Add(list3[i]);
                comboBox11.Items.Add(list3[i]);
            }


            //Recuperer les donnees sauvegarde dans le fichier
            if (File.Exists(pathModule + @"\SettingExport.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationExport));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingExport.xml");
                ConfigurationExport setting = new ConfigurationExport();
                setting = (ConfigurationExport)reader.Deserialize(file);

                comboBox1.Text = ((setting.exportBonsCommandes == "True") ? "Activer" : "Désactiver");
                comboBox2.Text = ((setting.exportBonsLivraisons == "True") ? "Activer" : "Désactiver");
                comboBox3.Text = ((setting.exportFactures == "True") ? "Activer" : "Désactiver");
                comboBox4.Text = ((setting.exportStock == "True") ? "Activer" : "Désactiver");

                comboBox5.Text = setting.exportBonsCommandes_Format;
                comboBox8.Text = setting.exportBonsLivraisons_Format;
                comboBox7.Text = setting.exportFactures_Format;
                comboBox6.Text = setting.exportStock_Format;

                comboBox12.Text = getStatutName(setting.exportBonsCommandes_Statut);
                comboBox9.Text = getStatutName(setting.exportBonsLivraisons_Statut);
                comboBox10.Text = getStatutName(setting.exportFactures_Statut);
                comboBox11.Text = getStatutName(setting.exportStock_Statut);
                file.Close();
            }

            /*
            List<string> list2 = EnumDsn();
            for (int i = 0; i < list2.Count; i++)
            {
                comboBox2.Items.Add(list2[i]);
            }

            if (File.Exists(pathModule + @"\SettingStatut.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationStatuts));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingStatut.xml");
                ConfigurationStatuts setting = new ConfigurationStatuts();
                setting = (ConfigurationStatuts)reader.Deserialize(file);

                comboBox2.Text = setting.DNS_2;
                textBox2.Text = setting.Nom_2;
                textBox3.Text = Utils.Decrypt(setting.Password_2);
                file.Close();
            }*/

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
            list[2] = "Velog";
            return list;
        }
        public static string[,] getStatutExport()
        {
            string[,] list = new string[4,2];
            list[0, 0] = "";
            list[1, 0] = "La Saisie";
            list[2, 0] = "Confirmé";
            list[3, 0] = "Accepté";

            list[0, 1] = "";
            list[1, 1] = "0";
            list[2, 1] = "1";
            list[3, 1] = "2";
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

        /*
        //Requette SQL pour trouver dans la BDD les statuts
        public static int getListeStatutValue(bool statut)
        {
            Boolean[] list = getValuesExport();

            for (int i = 0; i < list.Count(); i++)
            {
                if(list[i].Nom.Equals(name))
                {
                    value = list[i].Value;
                    break;
                }
            }
            return value;
        }

        public static string getListeStatutName(int value)
        {
            string name = "";
            Boolean[] list = getValuesExport();

            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i])
                {
                    name = "True";
                    break;
                }
                else
                {
                    name = "False";
                    break;
                }
            }
            return name;
        }

        public static IEnumerable<string> EnumDsn(RegistryKey rootKey)
        {
            RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    string value = regKey.GetValue(name, "").ToString();
                    yield return name;
                }
            }
        }
        */

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) || !string.IsNullOrEmpty(comboBox2.Text) || !string.IsNullOrEmpty(comboBox3.Text) || !string.IsNullOrEmpty(comboBox4.Text) ||
                !string.IsNullOrEmpty(comboBox5.Text) || !string.IsNullOrEmpty(comboBox8.Text) || !string.IsNullOrEmpty(comboBox7.Text) || !string.IsNullOrEmpty(comboBox6.Text) ||
                !string.IsNullOrEmpty(comboBox12.Text) || !string.IsNullOrEmpty(comboBox9.Text) || !string.IsNullOrEmpty(comboBox10.Text) || !string.IsNullOrEmpty(comboBox11.Text))
            {
                ConfigurationExport configurationStatut = new ConfigurationExport()
                {
                    exportBonsCommandes = ((comboBox1.Text == "Activer") ? "True" : "False"),
                    exportBonsLivraisons = ((comboBox2.Text == "Activer") ? "True" : "False"),
                    exportFactures = ((comboBox3.Text == "Activer") ? "True" : "False"),
                    exportStock = ((comboBox4.Text == "Activer") ? "True" : "False"),

                    exportBonsCommandes_Format = comboBox5.Text,
                    exportBonsLivraisons_Format = comboBox8.Text,
                    exportFactures_Format = comboBox7.Text,
                    exportStock_Format = comboBox6.Text,

                    exportBonsCommandes_Statut = getStatutValue(comboBox12.Text),
                    exportBonsLivraisons_Statut = getStatutValue(comboBox9.Text),
                    exportFactures_Statut = getStatutValue(comboBox10.Text),
                    exportStock_Statut = getStatutValue(comboBox11.Text)
                };

                try
                {
                    var myfile = File.Create(pathModule + @"\SettingExport.xml");
                    XmlSerializer xml = new XmlSerializer(typeof(ConfigurationExport));
                    xml.Serialize(myfile, configurationStatut);
                    myfile.Close();

                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(""+ex.Message);
                }

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
                if (!string.IsNullOrEmpty(comboBox11.Text))
                {
                    MessageBox.Show("Le Statut d'Export Stock est obligatoire !!");
                }
            }
        }
    }
}
