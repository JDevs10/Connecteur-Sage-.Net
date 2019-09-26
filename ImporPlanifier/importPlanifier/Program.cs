using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using importPlanifier.Helpers;
using System.Windows.Forms;

namespace importPlanifier
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                /*
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\TrackBack\\Values");
                if (key != null)
                {
                    string thekey = Utils.Decrypt(key.GetValue("Key").ToString());
                    string value0 = Utils.Decrypt(key.GetValue("Value0").ToString());
                    string value1 = key.GetValue("Value1").ToString();

                    int isCompatible = thekey.IndexOf(value0);
                    //MessageBox.Show(""+isCompatible+" - "+thekey+" - "+value0);
                    if (isCompatible != -1 && value0 != "" && thekey != "" && value1 == "1709")
                    {

                        Classes.Action2 action = new Classes.Action2();
                        action.LancerPlanification();
                        //Console.Read();
                        //Classes.ExportFactures a = new Classes.ExportFactures();
                        //a.ExportFacture();
                        //Console.Read();

                    }
                    else
                    {
                        //MessageBox.Show("Votre licence n'est pas valide", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //Application.EnableVisualStyles();
                        //Application.SetCompatibleTextRenderingDefault(false);
                        //Application.Run(new Validation());
                        Console.WriteLine("Votre licence n'est pas valide");
                        Console.Read();
                    }
                }
                 * */

                Classes.Action2 action = new Classes.Action2();
                action.LancerPlanification();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine("Votre licence n'est pas valide");
                Console.Read();

            }
        }
    }
}
