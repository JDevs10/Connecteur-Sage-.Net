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
                numericUpDown_backupFiles.Value = backup.backup_files;

                if (!checkBox_activateBackup.Checked)
                {
                    numericUpDown_generalLog.Enabled = false;
                    numericUpDown_importLog.Enabled = false;
                    numericUpDown_exportLog.Enabled = false;
                    numericUpDown_importSuccess.Enabled = false;
                    numericUpDown_importErreur.Enabled = false;
                    numericUpDown_backupFiles.Enabled = false;
                }
            }
            else
            {
                numericUpDown_generalLog.Enabled = false;
                numericUpDown_importLog.Enabled = false;
                numericUpDown_exportLog.Enabled = false;
                numericUpDown_importSuccess.Enabled = false;
                numericUpDown_importErreur.Enabled = false;
                numericUpDown_backupFiles.Enabled = false;
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
                    Convert.ToInt32(numericUpDown_backupFiles.Value)
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
                numericUpDown_backupFiles.Enabled = false;

                ConfigurationBackup backup = new ConfigurationBackup(
                    false,
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0),
                    Convert.ToInt32(0)
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
                numericUpDown_backupFiles.Enabled = true;
            }
            else
            {
                numericUpDown_generalLog.Enabled = false;
                numericUpDown_importLog.Enabled = false;
                numericUpDown_exportLog.Enabled = false;
                numericUpDown_importSuccess.Enabled = false;
                numericUpDown_importErreur.Enabled = false;
                numericUpDown_backupFiles.Enabled = false;
            }
        }

    }
}
