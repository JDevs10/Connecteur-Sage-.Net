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
    public partial class Configuration : Form
    {
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
 
        public Configuration()
        {
            InitializeComponent();


            List<string> list = EnumDsn();
            for (int i = 0; i < list.Count; i++)
            {
                comboBox1.Items.Add(list[i]);
            }

            if (File.Exists(pathModule+@"\Setting.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\Setting.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                comboBox1.Text = setting.DNS;
                textBox2.Text = setting.Nom;
                textBox3.Text = Utils.Decrypt(setting.Password);
                file.Close();
            }

            
        }


        public static List<string> EnumDsn()
        {
            List<string> list = new List<string>();
            list.AddRange(EnumDsn(Registry.CurrentUser));
            list.AddRange(EnumDsn(Registry.LocalMachine));
            return list;
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
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
            ConfigurationDNS configurationDNS = new ConfigurationDNS()
            {
                DNS = "" + comboBox1.Text,
                Nom = "" + textBox2.Text,
                Password = "" + Utils.Encrypt(textBox3.Text),

            };

            try
            {

                var myfile = File.Create(pathModule + @"\Setting.xml");
                XmlSerializer xml = new XmlSerializer(typeof(ConfigurationDNS));
                xml.Serialize(myfile, configurationDNS);
                myfile.Close();



                Main.ModifierButtonDNS(comboBox1.Text);
                Main.ModifierButtonNom(textBox2.Text);

                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(""+ex.Message);
            }
            
            }
            else {
            MessageBox.Show("DSN est obligatoire !!");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
