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
        public string exportFactures_Format;
        [XmlElement]
        public string exportFactures_Statut;

        [XmlElement]
        public string exportBonsLivraisons;
        [XmlElement]
        public string exportBonsLivraisons_Format;
        [XmlElement]
        public string exportBonsLivraisons_Statut;

        [XmlElement]
        public string exportBonsCommandes;
        [XmlElement]
        public string exportBonsCommandes_Format;
        [XmlElement]
        public string exportBonsCommandes_Statut;

        [XmlElement]
        public string exportStock;
        [XmlElement]
        public string exportStock_Format;
        [XmlElement]
        public string exportStock_Statut;

        public ConfigurationExport()
        {
            //Load();
        }

        public ConfigurationExport(string exportFactures, string exportBonsLivraisons, string exportBonsCommandes, string exportStock,
                                   string exportFactures_Format, string exportBonsLivraisons_Format, string exportBonsCommandes_Format, string exportStock_Format,
                                   string exportFactures_Statut, string exportBonsLivraisons_Statut, string exportBonsCommandes_Statut, string exportStock_Statut)
        {
            this.exportFactures = exportFactures;
            this.exportFactures_Format = exportFactures_Format;
            this.exportFactures_Statut = exportFactures_Statut;

            this.exportBonsLivraisons = exportBonsLivraisons;
            this.exportBonsLivraisons_Format = exportBonsLivraisons_Format;
            this.exportBonsLivraisons_Statut = exportBonsLivraisons_Statut;

            this.exportBonsCommandes = exportBonsCommandes;
            this.exportBonsCommandes_Format = exportBonsCommandes_Format;
            this.exportBonsCommandes_Statut = exportBonsCommandes_Statut;

            this.exportStock = exportStock;
            this.exportStock_Format = exportStock_Format;
            this.exportStock_Statut = exportStock_Statut;
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
                this.exportFactures_Format = setting.exportFactures_Format;
                this.exportFactures_Statut = setting.exportFactures_Statut;

                this.exportBonsLivraisons = setting.exportBonsLivraisons;
                this.exportBonsLivraisons_Format = setting.exportBonsLivraisons_Format;
                this.exportBonsLivraisons_Statut = setting.exportBonsLivraisons_Statut;

                this.exportBonsCommandes = setting.exportBonsCommandes;
                this.exportBonsCommandes_Format = setting.exportBonsCommandes_Format;
                this.exportBonsCommandes_Statut = setting.exportBonsCommandes_Statut;

                this.exportStock = setting.exportStock;
                this.exportStock_Format = setting.exportStock_Format;
                this.exportStock_Statut = setting.exportStock_Statut;
                file.Close();
            }
        }
    }
}
