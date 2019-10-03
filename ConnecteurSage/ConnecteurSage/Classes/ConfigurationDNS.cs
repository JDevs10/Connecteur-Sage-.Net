using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ConnecteurSage.Helpers;

namespace ConnecteurSage.Classes
{
    public class ConfigurationDNS
    {
        [XmlElement]
        public string Prefix { get; set; }
        [XmlElement]
        public string DNS_1 { get; set; }
        [XmlElement]
        public string Nom_1 { get; set; }
        [XmlElement]
        public string Password_1 { get; set; }

        [XmlElement]
        public string DNS_2 { get; set; }
        [XmlElement]
        public string Nom_2 { get; set; }
        [XmlElement]
        public string Password_2 { get; set; }

        public ConfigurationDNS()
        {
            //Load();
        }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public void Load() 
        {
            if (File.Exists("Setting.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule+@"\Setting.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                this.Prefix = setting.Prefix;
                this.DNS_1 = setting.DNS_1;
                this.Nom_1 = setting.Nom_1;
                this.Password_1 = Utils.Decrypt(setting.Password_1);
                file.Close();
            }
        }

        public void LoadSQL()
        {
            //DSN II
            if (File.Exists("SettingSQL.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingSQL.xml");
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);

                this.Prefix = setting.Prefix;
                this.DNS_2 = setting.DNS_2;
                this.Nom_2 = setting.Nom_2;
                this.Password_2 = Utils.Decrypt(setting.Password_2);
                file.Close();
            }
        }
    }
}
