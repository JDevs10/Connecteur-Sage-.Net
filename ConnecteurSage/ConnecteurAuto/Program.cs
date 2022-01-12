using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;


namespace ConnecteurAuto
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

            Fichier_De_Nettoyage.FichierDeNettoyage clean = new Fichier_De_Nettoyage.FichierDeNettoyage();
            clean.startClean();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            Console.WriteLine("Cleanup complete !");

            /*
            Init.Init init = new Init.Init();
            init.setDisplay(false);
            */

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
            const int SW_SHOWMINIMIZED = 2;
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOWMINIMIZED);

            // Hide or not the screen
            Database.Database db = new Database.Database();
            db.initTables();
            Database.Model.Settings _settings_ = db.settingsManager.get(db.connectionString, 1);
            int SW_HIDE_SHOW = _settings_.showWindow;

            // SW_HIDE = 0
            // SW_SHOW = 5
            if (SW_HIDE_SHOW == 5)
            {
                // Show
                ShowWindow(handle, 9);
                ShowWindow(handle, SW_HIDE_SHOW);
                int Height = Console.LargestWindowHeight - 20;
                int Width = Console.LargestWindowWidth - 20;
                Console.SetWindowSize(Width, Height);
                Console.SetWindowPosition(0, 0);
            }
            else if (SW_HIDE_SHOW == 0)
            {
                // Hide
                Console.SetWindowSize(1, 1);
                Console.SetWindowPosition(0, 0);
                ShowWindow(handle, SW_HIDE_SHOW);
            }

            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //start your multi threaded program here
            Program p = new Program();
            p.Start();

        }
    }
}
