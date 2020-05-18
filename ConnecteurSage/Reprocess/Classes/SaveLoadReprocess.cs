using Newtonsoft.Json;
using Reprocess_Error_Files.Classes.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reprocess_Error_Files.Classes
{
    public class SaveLoadReprocess
    {
        public List<ReprocessFiles> reprocessFilesList;
        private string fileName = "reprocessFiles.json";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public SaveLoadReprocess() { }
        public SaveLoadReprocess(List<ReprocessFiles> reprocessFilesList)
        {
            this.reprocessFilesList = reprocessFilesList;
        }

        public Boolean isFile()
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
            if (isFile())
            {
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                List<ReprocessFiles> deserializedProduct = JsonConvert.DeserializeObject<List<ReprocessFiles>>(file.ReadToEnd());
                this.reprocessFilesList = deserializedProduct;
                file.Close();
            }
        }

        public void Load(StreamWriter writer)
        {
            writer.WriteLine("");
            if (isFile())
            {
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                List<ReprocessFiles> deserializedProduct = JsonConvert.DeserializeObject<List<ReprocessFiles>>(file.ReadToEnd());
                this.reprocessFilesList = deserializedProduct;
                file.Close();

                writer.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => Load() | Loading...");
                writer.WriteLine(FormatJson(this.reprocessFilesList));
            }
            else
            {
                writer.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => Load() | No file \"" + fileName + "\" found!");
            }
            writer.WriteLine("");
            writer.Flush();
        }

        public void saveInfo()
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(this.reprocessFilesList, Newtonsoft.Json.Formatting.Indented);

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
                string json = JsonConvert.SerializeObject(this.reprocessFilesList);

                writer_.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => saveInfo() | Saving....");
                writer_.WriteLine(FormatJson(this.reprocessFilesList));

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
                writer_.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => saveInfo() | ************** Exception **************");
                writer_.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => saveInfo() | Message : " + ex.Message);
                writer_.WriteLine(DateTime.Now + " : Init.dll => ReprocessFiles => saveInfo() | StackTrace : " + ex.StackTrace);
            }
            writer_.WriteLine("");
            writer_.Flush();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// Pretty Print Json
        public string FormatJson()
        {
            if (isFile())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.reprocessFilesList, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }

        public string FormatJson(List<ReprocessFiles> configurationGeneral)
        {
            if (isFile())
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
