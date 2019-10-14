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


            List<string> list1 = EnumDsn();
            for (int i = 0; i < list1.Count; i++)
            {
                comboBox1.Items.Add(list1[i]);
            }

            if (File.Exists(pathModule+@"\Setting.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\Setting.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                comboBox1.Text = setting.DNS_1;
                textBox2.Text = setting.Nom_1;
                textBox3.Text = Utils.Decrypt(setting.Password_1);
                file.Close();
            }

            List<string> list2 = EnumDsn();
            for (int i = 0; i < list2.Count; i++)
            {
                comboBox2.Items.Add(list2[i]);
            }

            if (File.Exists(pathModule + @"\SettingSQL.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingSQL.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                comboBox2.Text = setting.DNS_2;
                textBox4.Text = setting.Nom_2;
                textBox1.Text = Utils.Decrypt(setting.Password_2);
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
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("DSN ODBC est obligatoire !!");
                return;
            }
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                MessageBox.Show("DSN SQL est obligatoire !!");
                return;
            }


            ConfigurationDNS configurationDNS1 = new ConfigurationDNS()
            {
                Prefix = comboBox1.Text.Split('_')[0],
                DNS_1 = "" + comboBox1.Text,
                Nom_1 = "" + textBox2.Text,
                Password_1 = ""+ textBox2.Text,//+ Utils.Encrypt(textBox3.Text),

            };

            try
            {
                var myfile = File.Create(pathModule + @"\Setting.xml");
                XmlSerializer xml = new XmlSerializer(typeof(ConfigurationDNS));
                xml.Serialize(myfile, configurationDNS1);
                myfile.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(""+ex.Message);
            }


            //DSN II            
            ConfigurationDNS configurationDNS2 = new ConfigurationDNS()
            {
                Prefix = comboBox2.Text.Split('_')[0],
                DNS_2 = "" + comboBox2.Text,
                Nom_2 = "" + textBox4.Text,
                Password_2 = ""+ textBox2.Text, //+ Utils.Encrypt(textBox1.Text),

            };

            try
            {

                var myfile = File.Create(pathModule + @"\SettingSQL.xml");
                XmlSerializer xml = new XmlSerializer(typeof(ConfigurationDNS));
                xml.Serialize(myfile, configurationDNS2);
                myfile.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
            }

            //Update Main UI
            Main.ModifierButtonDNS(comboBox1.Text, textBox2.Text, comboBox2.Text, textBox4.Text);
            Close();
        }


    }
}
