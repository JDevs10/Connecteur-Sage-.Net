using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Init
{
    public class Init
    {
        public Connecteur_Info.ConnecteurInfo connecteurInfo { get; set; }
        private string _FILENAME_ = "init.ini";
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public Init() { }
        public Init(Connecteur_Info.ConnecteurInfo connecteurInfo)
        {
            this.connecteurInfo = connecteurInfo;
        }

        public Boolean isSettings()
        {
            if (File.Exists(pathModule + @"\"+ _FILENAME_))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean isSettings_w_logs(StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " : Init.dll => Init => isSettings_w_logs() | checking if file => \""+ pathModule + @"\" + _FILENAME_ + "\" existe...");
            if (File.Exists(pathModule + @"\" + _FILENAME_))
            {
                writer.WriteLine(DateTime.Now + " : Init.dll => Init => isSettings_w_logs() | la config existe.");
                writer.WriteLine("");
                writer.Flush();
                return true;
            }
            else
            {
                writer.WriteLine(DateTime.Now + " : Init.dll => Init => isSettings_w_logs() | La config n'existe pes");
                writer.WriteLine("");
                writer.Flush();
                return false;
            }
        }

        public void Load()
        {
            if (isSettings())
            {
                FileStream fs = new FileStream(pathModule + @"\" + _FILENAME_, FileMode.Open, FileAccess.Read);
                StreamReader file = new System.IO.StreamReader(fs);
                Connecteur_Info.ConnecteurInfo deserializedConnecteurInfo = JsonConvert.DeserializeObject<Connecteur_Info.ConnecteurInfo>(file.ReadToEnd());
                this.connecteurInfo = deserializedConnecteurInfo;
                file.Close();
            }
        }

        public void Load_w_logs(StreamWriter writer)
        {
            writer.WriteLine("");
            if (isSettings_w_logs(writer))
            {
                FileStream fs = new FileStream(pathModule + @"\" + _FILENAME_, FileMode.Open, FileAccess.Read);
                StreamReader file = new System.IO.StreamReader(fs);
                Connecteur_Info.ConnecteurInfo deserializedConnecteurInfo = JsonConvert.DeserializeObject<Connecteur_Info.ConnecteurInfo>(file.ReadToEnd());
                this.connecteurInfo = deserializedConnecteurInfo;
                file.Close();

                writer.WriteLine(DateTime.Now + " : Init.dll => Init => Load_w_logs() | Loading...");
                writer.WriteLine(FormatJson_ConnecteurInfo());
            }
            else
            {
                writer.WriteLine(DateTime.Now + " : Init.dll => Init => Load_w_logs() | No file \"" + pathModule + @"\" + _FILENAME_ + "\" found!");
            }
            writer.WriteLine("");
            writer.Flush();
        }

        public void saveInfo()
        {
            try
            {
                //var myfile = File.Create(pathModule + @"\" + fileName);
                FileStream fs = new FileStream(pathModule + @"\" + _FILENAME_, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                string json = JsonConvert.SerializeObject(this.connecteurInfo, Newtonsoft.Json.Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }

        public void saveInfo_w_logs(StreamWriter writer_)
        {
            try
            {
                writer_.WriteLine("");
                //var myfile = File.Create(pathModule + @"\" + fileName);
                FileStream fs = new FileStream(pathModule + @"\" + _FILENAME_, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                string json = JsonConvert.SerializeObject(this.connecteurInfo);

                writer_.WriteLine(DateTime.Now + " : Init.dll => Init => saveInfo_w_logs() | Saving....");
                writer_.WriteLine(FormatJson_ConnecteurInfo());

                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
                writer_.WriteLine(DateTime.Now + " : Init.dll => Init => saveInfo_w_logs() | ************** Exception **************");
                writer_.WriteLine(DateTime.Now + " : Init.dll => Init => saveInfo_w_logs() | Message : " + ex.Message);
                writer_.WriteLine(DateTime.Now + " : Init.dll => Init => saveInfo_w_logs() | StackTrace : " + ex.StackTrace);
            }
            writer_.WriteLine("");
            writer_.Flush();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:
        /// Pretty Print Json
        public string FormatJson_ConnecteurInfo()
        {
            if (isSettings())
            {
                Load();
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(this.connecteurInfo, Newtonsoft.Json.Formatting.Indented);
                return f;
            }
            else
            {
                return "No file \"" + pathModule + @"\" + _FILENAME_ + "\" found!";
            }
        }

    }
}
