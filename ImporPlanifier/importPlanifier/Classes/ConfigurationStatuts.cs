using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace importPlanifier.Classes
{
    class ConfigurationStatuts
    {
        [XmlElement]
        public int Statut_Commande { get; set; }
        [XmlElement]
        public int Statut_BonLivraision { get; set; }
        [XmlElement]
        public int Statut_Facture { get; set; }

        public ConfigurationStatuts()
        {
            //Load();
        }

        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public void Load()
        {
            if (File.Exists("SettingStatut.xml"))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationStatuts));
                StreamReader file = new System.IO.StreamReader(pathModule + @"\SettingStatut.xml");
                ConfigurationStatuts setting = new ConfigurationStatuts();
                setting = (ConfigurationStatuts)reader.Deserialize(file);

                this.Statut_Commande = Convert.ToInt16(setting.Statut_Commande);
                this.Statut_BonLivraision = Convert.ToInt16(setting.Statut_BonLivraision);
                this.Statut_Facture = Convert.ToInt16(setting.Statut_Facture);
                file.Close();
            }
        }
    }
}
