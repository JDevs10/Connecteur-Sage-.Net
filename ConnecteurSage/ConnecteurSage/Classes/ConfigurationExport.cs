using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ConnecteurSage.Helpers;

namespace ConnecteurSage.Classes
{
    public class ConfigurationExport
    {
        [XmlElement]
        public string exportFactures;
        [XmlElement]
        public string exportBonsLivraisons;
        [XmlElement]
        public string exportBonsCommandes;
        [XmlElement]
        public string exportStock;

        public ConfigurationExport()
        {
            //Load();
        }

        public ConfigurationExport(string exportFactures, string exportBonsLivraisons, string exportBonsCommandes, string exportStock)
        {
            this.exportFactures = exportFactures;
            this.exportBonsLivraisons = exportBonsLivraisons;
            this.exportBonsCommandes = exportBonsCommandes;
            this.exportStock = exportStock;
        }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public void Load() 
        {
            if (File.Exists("SettingExport.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationExport));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingExport.xml");
                ConfigurationExport setting = new ConfigurationExport();
                setting = (ConfigurationExport)reader.Deserialize(file);

                this.exportFactures = setting.exportFactures;
                this.exportBonsLivraisons = setting.exportBonsLivraisons;
                this.exportBonsCommandes = setting.exportBonsCommandes;
                this.exportStock = setting.exportStock;
                file.Close();
            }
        }
    }
}
