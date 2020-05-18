using Alert_Mail.Classes.Custom;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes
{
    public class ConfigurationCustomMailSaveLoad
    {
        public List<CustomMailSuccess> customMailSuccessList { get; set; }
        public CustomMailRecap customMailRecap { get; set; }
        public string fileName_SUC_Imp = "Mail_SUC_IMP.ml";
        public string fileName_SUC_Exp = "Mail_SUC_EXP.ml";
        public string fileName_ERR_Imp = "Mail_ERR_IMP.ml";
        public string fileName_ERR_Exp = "Mail_ERR_EXP.ml";
        public string fileName_ERR_recap = "Mail_ERR_Recap.ml";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public ConfigurationCustomMailSaveLoad() { }
        public ConfigurationCustomMailSaveLoad(CustomMailRecap mCustomMailRecap)
        {
            this.customMailRecap = mCustomMailRecap;
        }

        public Boolean isSettings(string fileName)
        {
            if (File.Exists(pathModule + @"\" +fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Load(string fileName)
        {
            if (isSettings(fileName))
            {
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                CustomMailRecap deserialized = JsonConvert.DeserializeObject<CustomMailRecap>(file.ReadToEnd());
                this.customMailRecap = deserialized;
                file.Close();
            }
        }

        public void Load(string fileName, List<CustomMailSuccess> customMailSuccesses)
        {
            if (isSettings(fileName))
            {
                StreamReader file = new System.IO.StreamReader(pathModule + @"\" + fileName);
                List<CustomMailSuccess> deserialized = JsonConvert.DeserializeObject<List<CustomMailSuccess>>(file.ReadToEnd());
                this.customMailSuccessList = deserialized;
                file.Close();
            }
        }

        public void saveInfo(string fileName, CustomMailRecap customMailRecap)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(customMailRecap, Formatting.Indented);

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

        public void saveInfo(string fileName, List<CustomMailSuccess> customMailSuccesses)
        {
            try
            {
                var myfile = File.Create(pathModule + @"\" + fileName);
                string json = JsonConvert.SerializeObject(customMailSuccesses, Formatting.Indented);

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

        public string FormatJson(string fileName)
        {
            if (isSettings(fileName))
            {
                Load(fileName);
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.customMailRecap, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + fileName + "\" found!";
            }
        }


    }
}
