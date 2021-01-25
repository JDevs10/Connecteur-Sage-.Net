using Config_Import.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Import
{
    public class ConfigurationSaveLoad
    {
        public ConfigurationImport configurationImport { get; set; }
        public string fileName = "";
        //private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationSaveLoad() 
        {
            Database.Database db = new Database.Database();
            fileName = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\SettingImport.json";
        }
        public ConfigurationSaveLoad(ConfigurationImport mConfigurationImport)
        {
            Database.Database db = new Database.Database();
            fileName = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\SettingImport.json";

            this.configurationImport = mConfigurationImport;
        }

        public Boolean isSettings()
        {
            if (File.Exists(fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Load()
        {
            if (isSettings())
            {
                StreamReader file = new System.IO.StreamReader(fileName);
                ConfigurationImport deserializedProduct = JsonConvert.DeserializeObject<ConfigurationImport>(file.ReadToEnd());
                this.configurationImport = deserializedProduct;
                file.Close();
            }
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(fileName);
                string json = JsonConvert.SerializeObject(this.configurationImport, Newtonsoft.Json.Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(myfile))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }

        // check import setting json key/values
        public string checkSettingsIntegrity()
        {
            string result = null;
            try
            {
                Load();
                if (this.configurationImport.GetType() == typeof(Config_Import.Classes.ConfigurationImport) &&
                this.configurationImport != null &&
                this.configurationImport.Doc_Achat.GetType() == typeof(Config_Import.Classes.Custom_Doc.Doc_Achat) &&
                this.configurationImport.Doc_Achat != null &&
                this.configurationImport.Doc_Achat.Commande.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.Commande) &&
                this.configurationImport.Doc_Achat.Commande != null &&
                this.configurationImport.Doc_Achat.Commande.Activate != null &&
                this.configurationImport.Doc_Achat.Commande.Format != null &&
                this.configurationImport.Doc_Achat.Commande.Status != null &&

                this.configurationImport.Doc_Achat.DSADV.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.DSADV) &&
                this.configurationImport.Doc_Achat.DSADV != null &&
                this.configurationImport.Doc_Achat.DSADV.Activate != null &&
                this.configurationImport.Doc_Achat.DSADV.Format != null &&
                this.configurationImport.Doc_Achat.DSADV.Status != null &&

                this.configurationImport.Doc_Achat.Facture.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.Facture) &&
                this.configurationImport.Doc_Achat.Facture != null &&
                this.configurationImport.Doc_Achat.Facture.Activate != null &&
                this.configurationImport.Doc_Achat.Facture.Format != null &&
                this.configurationImport.Doc_Achat.Facture.Status != null &&


                this.configurationImport.Doc_Ventes.GetType() == typeof(Config_Import.Classes.Custom_Doc.Doc_Ventes) &&
                this.configurationImport.Doc_Ventes != null &&
                this.configurationImport.Doc_Ventes.Commande.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.Commande) &&
                this.configurationImport.Doc_Ventes.Commande != null &&
                this.configurationImport.Doc_Ventes.Commande.Activate != null &&
                this.configurationImport.Doc_Ventes.Commande.Format != null &&
                this.configurationImport.Doc_Ventes.Commande.Status != null &&

                this.configurationImport.Doc_Ventes.DSADV.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.DSADV) &&
                this.configurationImport.Doc_Ventes.DSADV != null &&
                this.configurationImport.Doc_Ventes.DSADV.Activate != null &&
                this.configurationImport.Doc_Ventes.DSADV.Format != null &&
                this.configurationImport.Doc_Ventes.DSADV.Status != null &&

                this.configurationImport.Doc_Ventes.Facture.GetType() == typeof(Config_Import.Classes.Custom_Doc.Custom.Facture) &&
                this.configurationImport.Doc_Ventes.Facture != null &&
                this.configurationImport.Doc_Ventes.Facture.Activate != null &&
                this.configurationImport.Doc_Ventes.Facture.Format != null &&
                this.configurationImport.Doc_Ventes.Facture.Status != null &&


                this.configurationImport.Doc_Stock.GetType() == typeof(Config_Import.Classes.Custom_Doc.Doc_Stock) &&
                this.configurationImport.Doc_Stock.Stock != null &&
                this.configurationImport.Doc_Stock.Stock.Activate != null &&
                this.configurationImport.Doc_Stock.Stock.Format != null &&
                this.configurationImport.Doc_Stock.Stock.Status != null)
                {
                    result = "L'intégralité de la configuration import dans " + fileName + " est valide";
                }
                else
                {
                    result = "################################### Erreur Config Import ###################################\n\n" +
                            "L'intégralité de la configuration import dans " + fileName + " n'est pas valide, veuillez verifier son intégralité.";
                }
            }
            catch (Exception ex)
            {
                result =    "################################### Erreur Config Import ###################################\n\n" +
                            "L'intégralité de la configuration import dans " + fileName + " n'est pas valide, veuillez verifier son intégralité.\n" +
                            "Message => " + ex.Message + "\n" +
                            "Stacktrace => " + ex.StackTrace;
            }
            return result;
        }


        public string FormatJson()
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.configurationImport, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }
    }
}
