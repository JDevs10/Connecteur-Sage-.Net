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
        [XmlElement]
        public string ACP_ComptaCPT_CompteG;


        private static string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string fileName = localPath + @"\init.ini";

        public InitConfig() { }

        public InitConfig(int showWindow, bool isOpen)
        {
            this.showWindow = showWindow;
            this.isOpen = isOpen;
            this.ACP_ComptaCPT_CompteG = "";
        }

        public InitConfig(int showWindow, bool isOpen, string ACP_ComptaCPT_CompteG)
        {
            this.showWindow = showWindow;
            this.isOpen = isOpen;
            this.ACP_ComptaCPT_CompteG = ACP_ComptaCPT_CompteG;
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
                writer.WriteLine(DateTime.Now + " : Dlls.dll => saveInfo() | ACP_ComptaCPT_CompteG : " + mInitConfig.ACP_ComptaCPT_CompteG); 
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
                    this.ACP_ComptaCPT_CompteG = mInitConfig.ACP_ComptaCPT_CompteG;
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
                    this.ACP_ComptaCPT_CompteG = mInitConfig.ACP_ComptaCPT_CompteG;

                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | showWindow : " + this.showWindow);
                    writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | isOpen : " + this.isOpen);
                    writer.WriteLine(DateTime.Now + " : Dlls.dll => Load() | ACP_ComptaCPT_CompteG : " + this.ACP_ComptaCPT_CompteG);
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
                InitConfig newIni = new InitConfig(ini.showWindow, false, ini.ACP_ComptaCPT_CompteG);
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
                InitConfig newIni = new InitConfig(ini.showWindow, false, ini.ACP_ComptaCPT_CompteG);
                InitConfig x = new InitConfig();
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | showWindow : " + ini.showWindow);
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | isOpen : false"); 
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | ACP_ComptaCPT_CompteG : " + ACP_ComptaCPT_CompteG); 
                x.saveInfo(newIni, writer);
            }
            else
            {
                InitConfig newIni = new InitConfig(5, false);
                InitConfig x = new InitConfig();

                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | showWindow : 5");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | isOpen : false");
                writer.WriteLine(DateTime.Now + " : Dlls.dll => resetWindowDisplay() | ACP_ComptaCPT_CompteG : ");
                x.saveInfo(newIni, writer);
            }
            writer.WriteLine("");
        }

        public int checkRunningApp()
        {
            Dlls.InitConfig ini = new Dlls.InitConfig();
            try
            {
                int SW;
                bool isOpen = false;

                if (ini.checkFileExistance())
                {
                    ini.Load();
                    SW = ini.showWindow;
                    isOpen = ini.isOpen;
                }
                else
                {
                    Dlls.InitConfig newIni = new Dlls.InitConfig(5, false);
                    Dlls.InitConfig x = new Dlls.InitConfig();
                    x.saveInfo(newIni);
                    SW = 5;
                    isOpen = false;
                }

                //check if the software is already running ?
                if (isOpen)
                {
                    Console.WriteLine("Le Planificateur est déja en cour");
                    for (int z = 5; z > 0; z--)
                    {
                        Console.WriteLine(DateTime.Now + " Fermeture dans " + z + " seconds....");
                        System.Threading.Thread.Sleep((z * 500));
                    }

                    return 99;
                }
                else
                {
                    Dlls.InitConfig ini_ = new Dlls.InitConfig();
                    ini_.Load();
                    ini_.isOpen = true;
                    Dlls.InitConfig x = new Dlls.InitConfig();
                    x.saveInfo(ini_);
                }

                return SW;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mode débogage 2 : " + ex.Message);
                return 99;
            }
        }

    }
}
