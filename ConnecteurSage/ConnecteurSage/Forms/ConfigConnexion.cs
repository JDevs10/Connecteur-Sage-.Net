using ConnecteurSage.Classes;
using ConnecteurSage.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ConnecteurSage.Forms
{
    public partial class ConfigConnexion : Form
    {
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigConnexion()
        {
            InitializeComponent();

            List<string> list1 = EnumDsn();
            for (int i = 0; i < list1.Count; i++)
            {
                comboBox1.Items.Add(list1[i]);
            }

            List<string> list2 = EnumDsn();
            for (int i = 0; i < list2.Count; i++)
            {
                comboBox2.Items.Add(list2[i]);
            }

            Connexion.ConnexionSaveLoad settings = new Connexion.ConnexionSaveLoad();
            if (settings.isSettings())
            {
                settings.Load();

                comboBox1.Text = settings.configurationConnexion.ODBC.DNS;
                textBox2.Text = settings.configurationConnexion.ODBC.USER;
                textBox3.Text = settings.configurationConnexion.ODBC.PWD;

                comboBox2.Text = settings.configurationConnexion.SQL.DNS;
                textBox4.Text = settings.configurationConnexion.SQL.USER;
                textBox1.Text = settings.configurationConnexion.SQL.PWD;
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

            Connexion.Classes.ConfigurationConnexion configurationConnexion = new Connexion.Classes.ConfigurationConnexion(
                new Connexion.Classes.Custom.ODBC(comboBox1.Text, textBox2.Text, textBox3.Text),
                new Connexion.Classes.Custom.SQL(comboBox2.Text.Split(new String[] { "__" }, StringSplitOptions.None)[0], comboBox2.Text, textBox4.Text, textBox1.Text)
            );
            Connexion.ConnexionSaveLoad settings = new Connexion.ConnexionSaveLoad();
            settings.configurationConnexion = configurationConnexion;
            settings.saveInfo();

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
