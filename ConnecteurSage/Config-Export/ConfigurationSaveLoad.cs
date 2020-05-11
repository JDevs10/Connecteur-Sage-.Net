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
        private string fileName = "SettingExport.json";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationSaveLoad() { }
        public ConfigurationSaveLoad(ConfigurationExport mConfigurationExport)
        {
            this.configurationExport = mConfigurationExport;
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
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                ConfigurationExport deserializedProduct = JsonConvert.DeserializeObject<ConfigurationExport>(file.ReadToEnd());
                this.configurationExport = deserializedProduct;
                file.Close();
            }
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(this.configurationExport);

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
