using AlertMail.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Alert_Mail.Classes
{
    public class ConfigurationSaveLoad
    {
        public ConfigurationEmail configurationEmail { get; set; }
        private string fileName = "SettingMail.json";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationSaveLoad()
        {

        }

        public ConfigurationSaveLoad(ConfigurationEmail mConfigurationEmail)
        {
            this.configurationEmail = mConfigurationEmail;
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
                ConfigurationEmail deserializedProduct = JsonConvert.DeserializeObject<ConfigurationEmail>(file.ReadToEnd());
                this.configurationEmail = deserializedProduct;
                file.Close();
            }
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(this.configurationEmail);

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

    }
}
