using ConnecteurSage.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConnecteurSage.Forms
{
    public partial class ConfigBackup : Form
    {
        public ConfigBackup()
        {
            InitializeComponent();

            string[] formatList = new string[3];     //List des formats d'export
            formatList[0] = "veuillez choisir un format";
            formatList[1] = "Plat";
            formatList[2] = "Véolog";

            for (int i = 0; i < formatList.Length; i++)
            {
                comboBox1.Items.Add(formatList[i]);
                comboBox2.Items.Add(formatList[i]);
                comboBox3.Items.Add(formatList[i]);
                comboBox4.Items.Add(formatList[i]);
            }

            if (File.Exists(Directory.GetCurrentDirectory() + @"\SettingBackup.xml"))
            {
                ConfigurationBackup backup = new ConfigurationBackup();
                backup.Load();
                checkBox_activateBackup.Checked = backup.activate;
                numericUpDown_generalLog.Value = backup.general_Log;
                numericUpDown_importLog.Value = backup.import_Log;
                numericUpDown_exportLog.Value = backup.export_Log;
                numericUpDown_importSuccess.Value = backup.import_files_success;
                numericUpDown_importErreur.Value = backup.import_files_error;
                numericUpDown_exportBC.Value = backup.export_files_BC;
                numericUpDown_exportBL.Value = backup.export_files_BL;
                numericUpDown_exportFA.Value = backup.export_files_FA;
                numericUpDown_exportME_MS.Value = backup.export_files_ME_MS;

                if (!checkBox_activateBackup.Checked)
                {
                    numericUpDown_generalLog.Enabled = false;
                    numericUpDown_importLog.Enabled = false;
                    numericUpDown_exportLog.Enabled = false;
                    numericUpDown_importSuccess.Enabled = false;
                    numericUpDown_importErreur.Enabled = false;
                    numericUpDown_exportBC.Enabled = false;
                    numericUpDown_exportBL.Enabled = false;
                    numericUpDown_exportFA.Enabled = false;
                    numericUpDown_exportME_MS.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                }
            }
            else
            {
                numericUpDown_generalLog.Enabled = false;
                numericUpDown_importLog.Enabled = false;
                numericUpDown_exportLog.Enabled = false;
                numericUpDown_importSuccess.Enabled = false;
                numericUpDown_importErreur.Enabled = false;
                numericUpDown_exportBC.Enabled = false;
                numericUpDown_exportBL.Enabled = false;
                numericUpDown_exportFA.Enabled = false;
                numericUpDown_exportME_MS.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
            }
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (checkBox_activateBackup.Checked)
            {
                string BC_Type = "";
                string BL_Type = "";
                string FA_Type = "";
                string ME_MS_Type = "";

                if (Convert.ToInt32(numericUpDown_exportBC.Value) != 0)
                {
                    if (comboBox1.Text != "")
                    {
                        BC_Type = comboBox1.Text;
                    }
                    else
                    {
                        MessageBox.Show("Veuillez choisir un format!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                if (Convert.ToInt32(numericUpDown_exportBL.Value) != 0)
                {
                    if (comboBox2.Text != "")
                    {
                        BL_Type = comboBox2.Text;
                    }
                    else
                    {
                        MessageBox.Show("Veuillez choisir un format!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                if (Convert.ToInt32(numericUpDown_exportFA.Value) != 0)
                {
                    if (comboBox3.Text != "")
                    {
                        FA_Type = comboBox3.Text;
                    }
                    else
                    {
                        MessageBox.Show("Veuillez choisir un format!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                if (Convert.ToInt32(numericUpDown_exportME_MS.Value) != 0)
                {
                    if (comboBox4.Text != "")
                    {
                        ME_MS_Type = comboBox4.Text;
                    }
                    else
                    {
                        MessageBox.Show("Veuillez choisir un format!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                ConfigurationBackup backup = new ConfigurationBackup(
                    checkBox_activateBackup.Checked,
                    Convert.ToInt32(numericUpDown_generalLog.Value),
                    Convert.ToInt32(numericUpDown_importLog.Value),
                    Convert.ToInt32(numericUpDown_exportLog.Value),
                    Convert.ToInt32(numericUpDown_importSuccess.Value),
                    Convert.ToInt32(numericUpDown_importErreur.Value),
                    Convert.ToInt32(numericUpDown_exportBC.Value),
                    BC_Type,
                    Convert.ToInt32(numericUpDown_exportBL.Value),
                    BL_Type,
                    Convert.ToInt32(numericUpDown_exportFA.Value),
                    FA_Type,
                    Convert.ToInt32(numericUpDown_exportME_MS.Value),
                    ME_MS_Type
                );
                backup.saveInfo(backup);
                Close();
            }
            else
            {
                numericUpDown_generalLog.Enabled = false;
                numericUpDown_importLog.Enabled = false;
                numericUpDown_exportLog.Enabled = false;
                numericUpDown_importSuccess.Enabled = false;
                numericUpDown_importErreur.Enabled = false;
                numericUpDown_exportBC.Enabled = false;
                numericUpDown_exportBL.Enabled = false;
                numericUpDown_exportFA.Enabled = false;
                numericUpDown_exportME_MS.Enabled = false;

                ConfigurationBackup backup = new ConfigurationBackup(
                    false,
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    "",
                    Convert.ToInt32(0),
                    "",
                    Convert.ToInt32(0),
                    "",
                    Convert.ToInt32(0),
                    ""
                );
                backup.saveInfo(backup);
                Close();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox_activateBackup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_activateBackup.Checked)
            {
                numericUpDown_generalLog.Enabled = true;
                numericUpDown_importLog.Enabled = true;
                numericUpDown_exportLog.Enabled = true;
                numericUpDown_importSuccess.Enabled = true;
                numericUpDown_importErreur.Enabled = true;
                numericUpDown_exportBC.Enabled = true;
                numericUpDown_exportBL.Enabled = true;
                numericUpDown_exportFA.Enabled = true;
                numericUpDown_exportME_MS.Enabled = true;
                if (numericUpDown_exportBC.Value > 0)
                {
                    comboBox1.Enabled = true;
                }
                else
                {
                    comboBox1.Enabled = false;
                }
                if (numericUpDown_exportBL.Value > 0)
                {
                    comboBox2.Enabled = true;
                }
                else
                {
                    comboBox2.Enabled = false;
                }
                if (numericUpDown_exportFA.Value > 0)
                {
                    comboBox3.Enabled = true;
                }
                else
                {
                    comboBox3.Enabled = false;
                }
                if (numericUpDown_exportME_MS.Value > 0)
                {
                    comboBox4.Enabled = true;
                }
                else
                {
                    comboBox4.Enabled = false;
                }
            }
            else
            {
                numericUpDown_generalLog.Enabled = false;
                numericUpDown_importLog.Enabled = false;
                numericUpDown_exportLog.Enabled = false;
                numericUpDown_importSuccess.Enabled = false;
                numericUpDown_importErreur.Enabled = false;
                numericUpDown_exportBC.Enabled = false;
                numericUpDown_exportBL.Enabled = false;
                numericUpDown_exportFA.Enabled = false;
                numericUpDown_exportME_MS.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
            }
        }

        private void numericUpDown_exportBC_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_exportBC.Value > 0)
            {
                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
            }
        }

        private void numericUpDown_exportBL_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_exportBL.Value > 0)
            {
                comboBox2.Enabled = true;
            }
            else
            {
                comboBox2.Enabled = false;
            }
        }

        private void numericUpDown_exportFA_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_exportFA.Value > 0)
            {
                comboBox3.Enabled = true;
            }
            else
            {
                comboBox3.Enabled = false;
            }
        }

        private void numericUpDown_exportME_MS_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_exportME_MS.Value > 0)
            {
                comboBox4.Enabled = true;
            }
            else
            {
                comboBox4.Enabled = false;
            }
        }
    }
}
