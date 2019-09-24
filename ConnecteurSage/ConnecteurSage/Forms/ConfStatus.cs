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
    public partial class ConfStatus : Form
    {
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
 
        public ConfStatus()
        {
            InitializeComponent();



            List<Statut> list1 = new Statut().getListeStatut();
            for (int i = 0; i < list1.Count; i++)
            {
                comboBox1.Items.Add(list1[i].Nom);
                comboBox2.Items.Add(list1[i].Nom);
                comboBox3.Items.Add(list1[i].Nom);
            }

            if (File.Exists(pathModule + @"\SettingStatut.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationStatuts));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingStatut.xml");
                ConfigurationStatuts setting = new ConfigurationStatuts();
                setting = (ConfigurationStatuts)reader.Deserialize(file);

                comboBox1.Text = getListeStatutName(setting.Statut_Commande);
                comboBox2.Text = getListeStatutName(setting.Statut_BonLivraision);
                comboBox3.Text = getListeStatutName(setting.Statut_Facture);
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

        //Requette SQL pour trouver dans la BDD les statuts
        

        public static int getListeStatutValue(string name)
        {
            int value = -1;
            List<Statut> list = new Statut().getListeStatut();

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

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) || !string.IsNullOrEmpty(comboBox2.Text) || !string.IsNullOrEmpty(comboBox3.Text) ||
                comboBox1.Text == "Sélectionné le Statut" || comboBox2.Text == "Sélectionné le Statut" || comboBox3.Text == "Sélectionné le Statut")
            {
                ConfigurationStatuts configurationStatut = new ConfigurationStatuts()
                {
                    Statut_Commande = getListeStatutValue(comboBox1.Text),
                    Statut_BonLivraision = getListeStatutValue(comboBox2.Text),
                    Statut_Facture = getListeStatutValue(comboBox3.Text),
                };

                try
                {
                    var myfile = File.Create(pathModule + @"\SettingStatut.xml");
                    XmlSerializer xml = new XmlSerializer(typeof(ConfigurationStatuts));
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
                    MessageBox.Show("Statut Commande est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox2.Text))
                {
                    MessageBox.Show("Statut Bon de Livraision est obligatoire !!");
                }

                if (!string.IsNullOrEmpty(comboBox3.Text))
                {
                    MessageBox.Show("Statut de Facture est obligatoire !!");
                }
            }
        }

    }
}
