using Config_Export.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_Export
{
    public class ConfigurationSaveLoad
    {
        public ConfigurationExport configurationExport { get; set; }
        private string fileName = "";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationSaveLoad() 
        {
            Database.Database db = new Database.Database();
            this.fileName = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\SettingExport.json";
        
        }
        public ConfigurationSaveLoad(ConfigurationExport mConfigurationExport)
        {
            Database.Database db = new Database.Database();
            this.fileName = @"" + db.settingsManager.get(db.connectionString, 1).EXE_Folder + @"\SettingExport.json";

            this.configurationExport = mConfigurationExport;
        }

        public string getFilePath()
        {
            return fileName;
        }

        public Boolean isSettings()
        {
            if (File.Exists(this.fileName))
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
                StreamReader file = new System.IO.StreamReader(this.fileName);
                 ConfigurationExport deserializedProduct = JsonConvert.DeserializeObject<ConfigurationExport>(file.ReadToEnd());
                this.configurationExport = deserializedProduct;
                file.Close();
            }
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(this.fileName);
                string json = JsonConvert.SerializeObject(this.configurationExport, Newtonsoft.Json.Formatting.Indented);

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


        public string FormatJson()
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.configurationExport, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }
    }
}
