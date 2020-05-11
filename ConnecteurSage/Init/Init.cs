using Init.Classes;
using Init.Classes.Configuration;
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
        private ConfigurationGeneral mConfigurationGeneral;
        private string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public Init() { }
        public Init(ConfigurationGeneral configurationGeneral)
        {
            this.mConfigurationGeneral = configurationGeneral;
        }

        public void setDisplay(StreamWriter writer, Boolean show)
        {
            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " : Init.dll => setDisplay(show : "+show+")");
            SaveLoadInit settings = new SaveLoadInit();

            if (settings.isSettings())
            {
                // Update ini file
                General mGeneral = this.mConfigurationGeneral.general;
                General newGeneral = new General(mGeneral.showWindow, show, mGeneral.isACP_ComptaCPT_CompteG, mGeneral.ACP_ComptaCPT_CompteG);

                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " : Init.dll => setDisplay() | showWindow : " + newGeneral.showWindow);
                writer.WriteLine(DateTime.Now + " : Init.dll => setDisplay() | isAppOpen : " + newGeneral.isAppOpen);
                writer.WriteLine(DateTime.Now + " : Init.dll => setDisplay() | ACP_ComptaCPT_CompteG : " + newGeneral.ACP_ComptaCPT_CompteG);

                this.mConfigurationGeneral.general = newGeneral;
                settings.configurationGeneral = this.mConfigurationGeneral;
                settings.saveInfo();
            }
            writer.WriteLine("");
            writer.Flush();
        }


        public int checkRunningApp()
        {
            SaveLoadInit settings = new SaveLoadInit();

            if (settings.isSettings())
            {
                try
                {
                    General mGeneral = this.mConfigurationGeneral.general;

                    if (mGeneral.isAppOpen)
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
                        General newGeneral = new General(mGeneral.showWindow, true, mGeneral.isACP_ComptaCPT_CompteG, mGeneral.ACP_ComptaCPT_CompteG);
                        this.mConfigurationGeneral.general = newGeneral;
                        settings.configurationGeneral = this.mConfigurationGeneral;
                        settings.saveInfo();
                        return mGeneral.showWindow;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mode débogage 2 : " + ex.Message);
                    return 99;
                }
            }
            else
            {
                return 99;
            }
        }
    }
}
