using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnecteurSage.Forms
{
    public partial class GeneralConfig : Form
    {
        public GeneralConfig()
        {
            InitializeComponent();

            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();

            if (settings.isSettings())
            {
                settings.Load();
                Init.Classes.ConfigurationGeneral configurationGeneral = settings.configurationGeneral;

                if (configurationGeneral.general.showWindow == 5)
                {
                    // visible Software while running
                    debugMode_checkBox.Checked = true;
                }
                else if (configurationGeneral.general.showWindow == 0)
                {
                    // Hide Software while running
                    debugMode_checkBox.Checked = false;
                }

                if (configurationGeneral.general.isAppOpen)
                {
                    label3.Text = "Planificateur en Cour...";
                }
                else
                {
                    label3.Text = "Planificateur est fermet.";
                }

                if (configurationGeneral.general.isACP_ComptaCPT_CompteG)
                {
                    checkBox_activate_compt_g_taxe.Text = "Activer";
                    checkBox_activate_compt_g_taxe.Checked = true;
                    textBox1.Enabled = true;
                    textBox1.Text = "" + configurationGeneral.general.ACP_ComptaCPT_CompteG;
                }
                else
                {
                    checkBox_activate_compt_g_taxe.Text = "Désactiver";
                    checkBox_activate_compt_g_taxe.Checked = false;
                    textBox1.Enabled = false;
                    textBox1.Text = "" + configurationGeneral.general.ACP_ComptaCPT_CompteG;
                }

                textBox2.Text = configurationGeneral.paths.EDI_Folder;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_activate_compt_g_taxe.Checked)
            {
                checkBox_activate_compt_g_taxe.Text = "Activer";
                textBox1.Enabled = true;
            }
            else
            {
                checkBox_activate_compt_g_taxe.Text = "Désactiver";
                textBox1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int show;
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;
            Init.Classes.ConfigurationGeneral configurationGeneral = new Init.Classes.ConfigurationGeneral();
            Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();

            if (debugMode_checkBox.Checked)
            {
                show = SW_SHOW;
            }
            else
            {
                show = SW_HIDE;
            }

            int _;
            if (!textBox1.Text.Equals("") && !int.TryParse(textBox1.Text.Trim(), out _))
            {
                MessageBox.Show("Seulement les chiffres entre 0 à 9 sont valide!", "Compt. G. Taxe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            configurationGeneral.general = new Init.Classes.Configuration.General(show, false, checkBox_activate_compt_g_taxe.Checked, Convert.ToInt32(textBox1.Text.Trim()));
            configurationGeneral.paths = new Init.Classes.Configuration.Paths(textBox2.Text.Trim());
            settings.configurationGeneral = configurationGeneral;
            settings.saveInfo();

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedPath = "null";
                var t = new Thread((ThreadStart)(() => {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
                    fbd.ShowNewFolderButton = true;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        selectedPath = fbd.SelectedPath;
                    }
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();

                textBox2.Text = selectedPath;
            }
            // Récupération d'une possible SDKException
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
