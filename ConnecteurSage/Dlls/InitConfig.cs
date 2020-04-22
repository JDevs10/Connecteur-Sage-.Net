using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Dlls
{
    public class InitConfig
    {
        [XmlElement]
        public int showWindow;
        [XmlElement]
        public bool isOpen;


        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string fileName = localPath + @"\init.ini";

        public InitConfig() { }

        public InitConfig(int showWindow, bool isOpen)
        {
            this.showWindow = showWindow;
            this.isOpen = isOpen;
        }

        public bool checkFileExistance()
        {
            return File.Exists(fileName);
        }

        public bool checkFileExistance(StreamWriter writer)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " : Dlls.dll => checkFileExistance() | fileName : " + fileName);
            return File.Exists(fileName);
        }

        public void saveInfo(InitConfig mInitConfig)
        {
            try
            {
                var myfile = File.Create(fileName);
                XmlSerializer xml = new XmlSerializer(typeof(InitConfig));
                xml.Serialize(myfile, mInitConfig);
                myfile.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("saveInfo() .ini :: " + ex.Message);
            }
        }

        public void saveInfo(InitConfig mInitConfig, StreamWriter writer)
        {
            try
            {
                var myfile = File.Create(fileName);
                XmlSerializer xml = new XmlSerializer(typeof(InitConfig));
                xml.Serialize(myfile, mInitConfig);
                myfile.Close();

                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => saveInfo() | showWindow : " + mInitConfig.showWindow);
                writer.WriteLine(DateTime.Now + " : Dlls.dll => saveInfo() | isOpen : " + mInitConfig.isOpen);
            }
            catch (Exception ex)
            {
                Console.WriteLine("saveInfo() .ini :: " + ex.Message);
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => saveInfo() | ************** Exception **************");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => saveInfo() | " + ex.Message);
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(fileName))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(InitConfig));
                    StreamReader file = new StreamReader(fileName);
                    InitConfig mInitConfig = (InitConfig)reader.Deserialize(file);

                    this.showWindow = mInitConfig.showWindow;
                    this.isOpen = mInitConfig.isOpen;

                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Load() .ini :: " + ex.Message);
            }
        }
        public void Load(StreamWriter writer)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(InitConfig));
                    StreamReader file = new StreamReader(fileName);
                    InitConfig mInitConfig = (InitConfig)reader.Deserialize(file);

                    this.showWindow = mInitConfig.showWindow;
                    this.isOpen = mInitConfig.isOpen;

                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | showWindow : " + this.showWindow);
                    writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | isOpen : " + this.isOpen);

                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Load() .ini :: " + ex.Message);
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | ************** Exception **************");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | " + ex.Message);
            }
        }

        public void resetWindowDisplay()
        {
            // Update ini file
            InitConfig ini = new InitConfig();
            if (ini.checkFileExistance())
            {
                ini.Load();
                InitConfig newIni = new InitConfig(ini.showWindow, false);
                InitConfig x = new InitConfig();
                x.saveInfo(newIni);
            }
            else
            {
                InitConfig newIni = new InitConfig(5, false);
                InitConfig x = new InitConfig();
                x.saveInfo(newIni);
            }
        }

        public void resetWindowDisplay(StreamWriter writer)
        {
            // Update ini file
            InitConfig ini = new InitConfig();
            if (ini.checkFileExistance(writer))
            {
                ini.Load(writer);
                InitConfig newIni = new InitConfig(ini.showWindow, false);
                InitConfig x = new InitConfig();
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | showWindow : " + ini.showWindow);
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | isOpen : false");

                x.saveInfo(newIni, writer);
            }
            else
            {
                InitConfig newIni = new InitConfig(5, false);
                InitConfig x = new InitConfig();

                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | showWindow : 5");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | isOpen : false");

                x.saveInfo(newIni, writer);
            }
            writer.WriteLine("");
        }
    }
}
