using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init.Classes
{
    public class SaveLoadInit
    {
        public ConfigurationGeneral configurationGeneral { get; set; }
        private string fileName = "init.json";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public SaveLoadInit(){ }
        public SaveLoadInit(ConfigurationGeneral mConfigurationGeneral)
        {
            this.configurationGeneral = mConfigurationGeneral;
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
                ConfigurationGeneral deserializedProduct = JsonConvert.DeserializeObject<ConfigurationGeneral>(file.ReadToEnd());
                this.configurationGeneral = deserializedProduct;
                file.Close();
            }
        }

        public void Load(StreamWriter writer)
        {
            writer.WriteLine("");
            if (isSettings())
            {
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                ConfigurationGeneral deserializedProduct = JsonConvert.DeserializeObject<ConfigurationGeneral>(file.ReadToEnd());
                this.configurationGeneral = deserializedProduct;
                file.Close();

                writer.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => Load() | Loading...");
                writer.WriteLine(FormatJson(this.configurationGeneral));
            }
            else
            {
                writer.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => Load() | No file \"" + fileName + "\" found!");
            }
            writer.WriteLine("");
            writer.Flush();
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(this.configurationGeneral, Newtonsoft.Json.Formatting.Indented);

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

        public void saveInfo(StreamWriter writer_)
        {
            try
            {
                writer_.WriteLine("");
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(this.configurationGeneral);

                writer_.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => saveInfo() | Saving....");
                writer_.WriteLine(FormatJson(this.configurationGeneral));

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
                writer_.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => saveInfo() | ************** Exception **************");
                writer_.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => saveInfo() | Message : " + ex.Message);
                writer_.WriteLine(DateTime.Now + " : Reprocess.dll => SaveLoadInit => saveInfo() | StackTrace : " + ex.StackTrace);
            }
            writer_.WriteLine("");
            writer_.Flush();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// Pretty Print Json
        public string FormatJson()
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.configurationGeneral, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }

        public string FormatJson(ConfigurationGeneral configurationGeneral)
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(configurationGeneral, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }
    }
}
