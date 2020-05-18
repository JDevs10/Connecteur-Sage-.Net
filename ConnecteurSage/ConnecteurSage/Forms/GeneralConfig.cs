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

                ////////////////////////////////////////////////////////////////////////////////////
                /// Reprocess
                /// 
                if (configurationGeneral.reprocess.activate)
                {
                    checkBox_reprocess_activate.Text = "Activer";
                    checkBox_reprocess_activate.Checked = true;
                }
                else
                {
                    checkBox_reprocess_activate.Text = "Désactiver";
                    checkBox_reprocess_activate.Checked = false;
                }

                numericUpDown_hour.Value = configurationGeneral.reprocess.hour;
                numericUpDown1_reprocess_cd.Value = configurationGeneral.reprocess.countDown;
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

            if(checkBox_reprocess_activate.Checked == true && Convert.ToInt32(numericUpDown_hour.Value) == 0)
            {
                MessageBox.Show("Heure de retraitement n'est pas configuré !", "Retraitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            configurationGeneral.general = new Init.Classes.Configuration.General(show, false, checkBox_activate_compt_g_taxe.Checked, Convert.ToInt32(textBox1.Text.Trim()));
            configurationGeneral.paths = new Init.Classes.Configuration.Paths(textBox2.Text.Trim());
            configurationGeneral.reprocess = new Init.Classes.Configuration.Reprocess(checkBox_reprocess_activate.Checked, Convert.ToInt32(numericUpDown_hour.Value), Convert.ToInt32(numericUpDown1_reprocess_cd.Value));
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

        private void checkBox_reprocess_activate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_reprocess_activate.Checked)
            {
                numericUpDown_hour.Enabled = true;
                label7_reprocess_hour.Enabled = true;
                numericUpDown1_reprocess_cd.Enabled = true;
                label7_reprocess.Enabled = true;
            }
            else
            {
                numericUpDown_hour.Enabled = false;
                label7_reprocess_hour.Enabled = false;
                numericUpDown1_reprocess_cd.Enabled = false;
                label7_reprocess.Enabled = false;
            }
        }

        private void numericUpDown1_reprocess_cd_ValueChanged(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(numericUpDown1_reprocess_cd.Value);
            if (count == 0)
            {
                label7_reprocess.Text = "Cette fonctionnalité est désactiver!\n" +
                                        "Aucun fichier ne sera supprimé pendant / après le retraitement.";
            }
            else
            {
                label7_reprocess.Text = "Si le fichier a été retraité " + count + " fois, \n" +
                                        "il sera supprimé de la file d'attente.";
            }
        }

        private void numericUpDown_hour_ValueChanged(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(numericUpDown_hour.Value);
            if (count == 0)
            {
                label7_reprocess_hour.Text = "Ce paramètre est obligatoire\nSi activé !!!.";
            }
            else
            {
                label7_reprocess_hour.Text = "Retraitement tous les " + count + " heure(s).";
            }
        }
    }
}
