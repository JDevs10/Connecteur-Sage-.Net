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


            Boolean[] list1 = new Boolean[2];
            list1[0] = true;
            list1[1] = false;


            for (int i = 0; i < getValuesExport().Length; i++)
            {
                comboBox1.Items.Add(list1[i]);
                comboBox2.Items.Add(list1[i]);
                comboBox3.Items.Add(list1[i]);
                comboBox4.Items.Add(list1[i]);
            }

            if (File.Exists(pathModule + @"\SettingExport.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationExport));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingExport.xml");
                ConfigurationExport setting = new ConfigurationExport();
                setting = (ConfigurationExport)reader.Deserialize(file);

                comboBox1.Text = setting.exportBonsCommandes;
                comboBox2.Text = setting.exportBonsLivraisons;
                comboBox3.Text = setting.exportFactures;
                comboBox4.Text = setting.exportStock;
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
            if (!string.IsNullOrEmpty(comboBox1.Text) || !string.IsNullOrEmpty(comboBox2.Text) || !string.IsNullOrEmpty(comboBox3.Text) ||
                comboBox1.Text == "Sélectionné le Statut" || comboBox2.Text == "Sélectionné le Statut" || comboBox3.Text == "Sélectionné le Statut")
            {
                ConfigurationExport configurationStatut = new ConfigurationExport()
                {
                    exportBonsCommandes = comboBox1.Text,
                    exportBonsLivraisons = comboBox2.Text,
                    exportFactures = comboBox3.Text,
                    exportStock = comboBox4.Text
                };

                try
                {
                    var myfile = File.Create(pathModule + @"\SettingExport.xml");
                    XmlSerializer xml = new XmlSerializer(typeof(ConfigurationExport));
                    xml.Serialize(myfile, configurationStatut);
                    myfile.Close();

                    //Update labels
                    /*
                    ConfigurationExport settings = new ConfigurationExport();
                    Main main = new Main();
                    main.setExportValues(settings.exportBonsCommandes, settings.exportBonsLivraisons, settings.exportFactures, settings.exportStock);
                    */
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
                    MessageBox.Show("Statut Export Commande est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox2.Text))
                {
                    MessageBox.Show("Statut Export Bon de Livraision est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox3.Text))
                {
                    MessageBox.Show("Statut d'Export Facture est obligatoire !!");
                }
                if (!string.IsNullOrEmpty(comboBox4.Text))
                {
                    MessageBox.Show("Statut d'Export Stock est obligatoire !!");
                }
            }
        }

    }
}
