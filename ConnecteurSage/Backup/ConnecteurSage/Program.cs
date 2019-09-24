using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using ConnecteurSage.Forms;
using ConnecteurSage.Helpers;

namespace ConnecteurSage
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\TrackBack\\Values");
                if (key != null)
                {
                    string thekey = Utils.Decrypt(key.GetValue("Key").ToString());
                    string value0 = Utils.Decrypt(key.GetValue("Value0").ToString());
                    string value1 = key.GetValue("Value1").ToString();
                    int num = 2 * (9 * 9 + 4) * 10 + 9;

                    int isCompatible = thekey.IndexOf(value0);
                    //MessageBox.Show(""+isCompatible+" - "+thekey+" - "+value0);
                    if (isCompatible != -1 && value0 != "" && thekey != "" && value1 == num.ToString())
                    {
                        //MessageBox.Show("ici");
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Main());
                    }
                    else
                    {
                        //MessageBox.Show("Votre licence n'est pas valide", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Validation());
                    }
                }
                else
                {
                    //MessageBox.Show("Votre licence n'est pas valide", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    //Application.Run(new Validation());
                    using (Forms.Validation form = new Forms.Validation())
                    {
                        form.ShowDialog();
                    }

                    if(Validation.isValide)
                    {
                         Application.Run(new Main());
                    }


                }
            }
            catch
            {
                //MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //MessageBox.Show("Votre licence n'est pas valide", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Validation());

            }


      
            //RegistryKey Nkey = Registry.CurrentUser;
            //try
            //{
            //    RegistryKey valKey = Nkey.OpenSubKey("Software\\TrackBack\\Values", true);
            //    if (valKey == null)
            //    {
            //        Nkey.CreateSubKey("Software\\TrackBack\\Values");
            //    }
            //    valKey.SetValue("Key", "{465456451518748744445}");
            //    valKey.SetValue("Value", "4556");
            //}
            //catch (Exception er)
            //{
            //    MessageBox.Show(er.Message, "MyApp", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //}
            //finally
            //{
            //    Nkey.Close();
            //} 

   
        }
    }
}
