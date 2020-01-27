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
                numericUpDown_exportBLF.Value = backup.export_files_BLF;

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
                    numericUpDown_exportBLF.Enabled = false;
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
                numericUpDown_exportBLF.Enabled = false;
            }
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            if (checkBox_activateBackup.Checked)
            {
                ConfigurationBackup backup = new ConfigurationBackup(
                    checkBox_activateBackup.Checked,
                    Convert.ToInt32(numericUpDown_generalLog.Value),
                    Convert.ToInt32(numericUpDown_importLog.Value),
                    Convert.ToInt32(numericUpDown_exportLog.Value),
                    Convert.ToInt32(numericUpDown_importSuccess.Value),
                    Convert.ToInt32(numericUpDown_importErreur.Value),
                    Convert.ToInt32(numericUpDown_exportBC.Value),
                    Convert.ToInt32(numericUpDown_exportBL.Value),
                    Convert.ToInt32(numericUpDown_exportFA.Value),
                    Convert.ToInt32(numericUpDown_exportME_MS.Value),
                    Convert.ToInt32(numericUpDown_exportBLF.Value)
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
                numericUpDown_exportBLF.Enabled = false;
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
                numericUpDown_exportBLF.Enabled = true;
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
                numericUpDown_exportBLF.Enabled = false;
            }
        }
    }
}
