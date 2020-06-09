using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace ImportPlanifier
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        static bool exitSystem = false;
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        private static Thread newThread = null;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }


        private static bool Handler(CtrlType sig)
        {
            newThread.Abort();

            Connecteur_Info.Custom.Batch_Interrup interrup = new Connecteur_Info.Custom.Batch_Interrup();
            interrup.interruption();

            //do your cleanup here
            Thread.Sleep(5000); //simulate some cleanup delay
            Console.WriteLine("");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////// Fichier_De_Nettoyage.dll ///////////////
            ///
            string logDirectoryName_general = Directory.GetCurrentDirectory() + @"\" + "LOG";
            string logDirectoryName_import = Directory.GetCurrentDirectory() + @"\" + "LOG" + @"\" + "LOG_Import";

            Fichier_De_Nettoyage.FichierDeNettoyage clean = new Fichier_De_Nettoyage.FichierDeNettoyage();
            string[,] paths = new string[4, 2] {
                { "general_logs", logDirectoryName_general}, //log files
                { "import_logs", logDirectoryName_import }, //log files
                { "import_files_success", Directory.GetCurrentDirectory() + @"\Success File" }, //fichier import success
                { "import_files_error", Directory.GetCurrentDirectory() + @"\Error File" } //fichier import erreur
            };

            clean.startClean(paths);
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            Console.WriteLine("Cleanup complete !");

            Init.Init init = new Init.Init();
            init.setDisplay(false);

            Console.WriteLine("General Config reseted!");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }

        public void Start()
        {
            // start a thread and start doing some processing
            //Console.WriteLine("Thread started, processing...\n\n");

            Launch mLaunch = new Launch();
            newThread = new Thread(new ThreadStart(mLaunch.go));
            newThread.Start();
        }
        #endregion

        static void Main(string[] args)
        {
            int Height = Console.LargestWindowHeight - 10;
            int Width = Console.LargestWindowWidth - 10;
            Console.SetWindowSize(Width, Height);
            Console.SetWindowPosition(0, 0);

            //////////////////////////////////////////////////////////////////////////////////////
            ///// Check if the app is running
            ///

            Init.Init init = new Init.Init();
            int result =  init.checkRunningApp();

            if (result != 99)
            {
                // hide or show the running software window
                var handle = GetConsoleWindow();
                ShowWindow(handle, result);
            }
            else
            {
                return;
            }
            ///////////////////////////////////////////


            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //start your multi threaded program here
            Program p = new Program();
            p.Start();

            //hold the console so it doesn’t run off the end
            /*
            while (!exitSystem)
            {
                Thread.Sleep(500);
            }
            */
        }
    }
}
