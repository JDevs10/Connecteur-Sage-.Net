using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace importPlanifier.Classes
{
    class ConfigurationExport
    {
        [XmlElement]
        public Boolean exportFactures { get; set; }
        [XmlElement]
        public Boolean exportBonsLivraisons { get; set; }
        [XmlElement]
        public Boolean exportBonsCommandes { get; set; }
        [XmlElement]
        public Boolean exportStock { get; set; }

        public ConfigurationExport()
        {
            //Load();
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
