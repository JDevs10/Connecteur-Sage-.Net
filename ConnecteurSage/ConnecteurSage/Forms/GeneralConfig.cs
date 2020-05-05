using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnecteurSage.Forms
{
    public partial class GeneralConfig : Form
    {
        public GeneralConfig()
        {
            InitializeComponent();

            int SW = -1;
            bool isOpen = false;
            string compt_G_Taxe = "";
            Dlls.InitConfig ini = new Dlls.InitConfig();

            if (ini.checkFileExistance())
            {
                ini.Load();
                SW = ini.showWindow;
                isOpen = ini.isOpen;
                compt_G_Taxe = ini.ACP_ComptaCPT_CompteG;
            }
            else
            {
                Dlls.InitConfig newIni = new Dlls.InitConfig(5, false);
                Dlls.InitConfig x = new Dlls.InitConfig();
                x.saveInfo(newIni);
                SW = 5;
            }

            if (SW == 5)
            {
                // visible Software while running
                debugMode_checkBox.Checked = true;
            }
            else if(SW == 0)
            {
                // Hide Software while running
                debugMode_checkBox.Checked = false;
            }

            if (isOpen)
            {
                label3.Text = "Planificateur en Cour...";
            }
            else
            {
                label3.Text = "Planificateur est fermet.";
            }

            if (compt_G_Taxe != "")
            {
                checkBox1.Checked = true;
                textBox1.Text = compt_G_Taxe;
            }
            else
            {
                checkBox1.Checked = false;
                textBox1.Text = "";
            }

        }

        private void debugMode_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;

            if (debugMode_checkBox.Checked)
            {
                try
                {
                    Dlls.InitConfig x = new Dlls.InitConfig();
                    if (x.checkFileExistance())
                    {
                        x.Load();
                        Dlls.InitConfig ini = new Dlls.InitConfig(SW_SHOW, x.isOpen);
                        ini.saveInfo(ini);
                    }
                    else
                    {
                        Dlls.InitConfig ini = new Dlls.InitConfig(SW_SHOW, false);
                        x.saveInfo(ini);
                    }

                    //MessageBox.Show("Les fenêtres de planification seront visibles.", "Mode débogage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Mode débogage 1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    /*
                    Dlls.InitConfig ini = new Dlls.InitConfig(SW_HIDE, false);
                    Dlls.InitConfig x = new Dlls.InitConfig();
                    x.saveInfo(ini);
                    */

                    Dlls.InitConfig x = new Dlls.InitConfig();
                    if (x.checkFileExistance())
                    {
                        x.Load();
                        Dlls.InitConfig ini = new Dlls.InitConfig(SW_HIDE, x.isOpen);
                        ini.saveInfo(ini);
                    }
                    else
                    {
                        Dlls.InitConfig ini = new Dlls.InitConfig(SW_HIDE, false);
                        x.saveInfo(ini);
                    }

                    //MessageBox.Show("Les fenêtres de planification ne seront plus visibles.", "Mode débogage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Mode débogage 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Text = "Activer";
                textBox1.Enabled = true;
            }
            else
            {
                checkBox1.Text = "Désactiver";
                textBox1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Save Compt. G. Taxe info
            if (checkBox1.Checked)
            {
                Dlls.InitConfig x = new Dlls.InitConfig();
                if (x.checkFileExistance())
                {
                    x.Load();
                    Dlls.InitConfig ini = new Dlls.InitConfig(x.showWindow, x.isOpen, textBox1.Text.Trim());
                    ini.saveInfo(ini);
                }
                else
                {
                    Dlls.InitConfig ini = new Dlls.InitConfig(5, false);
                    x.saveInfo(ini);
                }
            }
            else
            {
                Dlls.InitConfig x = new Dlls.InitConfig();
                if (x.checkFileExistance())
                {
                    x.Load();
                    Dlls.InitConfig ini = new Dlls.InitConfig(x.showWindow, x.isOpen, "");
                    ini.saveInfo(ini);
                }
                else
                {
                    Dlls.InitConfig ini = new Dlls.InitConfig(5, false);
                    x.saveInfo(ini);
                }
            }

            Close();
        }
    }
}
