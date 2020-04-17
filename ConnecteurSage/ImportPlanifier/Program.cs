using importPlanifier.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Dlls;

namespace ImportPlanifier
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            try
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
                        return;
                    }
                    else
                    {
                        Dlls.InitConfig ini_ = new Dlls.InitConfig();
                        ini_.Load();
                        ini_.isOpen = true;
                        Dlls.InitConfig x = new Dlls.InitConfig();
                        x.saveInfo(ini_);
                    }

                    // hide or show the running software window
                    var handle = GetConsoleWindow();
                    ShowWindow(handle, SW);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mode débogage 2 : " + ex.Message);
                }

                Action2 action = new Action2();
                action.LancerPlanification();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
