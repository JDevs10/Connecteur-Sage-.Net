using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
                /// Tarifaire
                /// 
                if (configurationGeneral.priceType.activate)
                {
                    checkBox_activer_tarif.Text = "Activer";
                    checkBox_activer_tarif.Checked = true;

                    radioButton_tarif_cmd_EDI.Enabled = true;
                    radioButton_tarif_cmd_EDI.Checked = configurationGeneral.priceType.cmdEDIPrice;
                    radioButton_tarif_produit.Enabled = true;
                    radioButton_tarif_produit.Checked = configurationGeneral.priceType.productPrice;
                    radioButton_tarif_categorie.Enabled = true;
                    radioButton_tarif_categorie.Checked = configurationGeneral.priceType.categoryPrice;
                    radioButton_tarif_client.Enabled = true;
                    radioButton_tarif_client.Checked = configurationGeneral.priceType.clientPrice;
                }
                else
                {
                    checkBox_activer_tarif.Text = "Désactiver";
                    checkBox_activer_tarif.Checked = false;

                    radioButton_tarif_cmd_EDI.Enabled = false;
                    radioButton_tarif_cmd_EDI.Checked = configurationGeneral.priceType.cmdEDIPrice;
                    radioButton_tarif_produit.Enabled = false;
                    radioButton_tarif_produit.Checked = configurationGeneral.priceType.productPrice;
                    radioButton_tarif_categorie.Enabled = false;
                    radioButton_tarif_categorie.Checked = configurationGeneral.priceType.categoryPrice;
                    radioButton_tarif_client.Enabled = false;
                    radioButton_tarif_client.Checked = configurationGeneral.priceType.clientPrice;
                }

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

            if (!settings.isSettings())
            {
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

                if (checkBox_reprocess_activate.Checked == true && Convert.ToDouble(numericUpDown_hour.Value) == 0.0)
                {
                    MessageBox.Show("Heure de retraitement n'est pas configuré !", "Retraitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string compt_g_taxe = "";
                if (textBox1.Enabled)
                {
                    compt_g_taxe = textBox1.Text.ToString();
                }
                else
                {
                    compt_g_taxe = "-1";
                }

                configurationGeneral.general = new Init.Classes.Configuration.General(show, false, checkBox_activate_compt_g_taxe.Checked, Convert.ToInt32(compt_g_taxe.Trim()));
                configurationGeneral.paths = new Init.Classes.Configuration.Paths(textBox2.Text.Trim());
                configurationGeneral.plannerTask = new Init.Classes.Configuration.PlannerTask();
                configurationGeneral.priceType = new Init.Classes.Configuration.PriceType(checkBox_activer_tarif.Checked, radioButton_tarif_cmd_EDI.Checked, radioButton_tarif_produit.Checked, radioButton_tarif_categorie.Checked, radioButton_tarif_client.Checked);
                configurationGeneral.reprocess = new Init.Classes.Configuration.Reprocess(checkBox_reprocess_activate.Checked, numericUpDown_hour.Value, Convert.ToInt32(numericUpDown1_reprocess_cd.Value));
                settings.configurationGeneral = configurationGeneral;
                settings.saveInfo();
            }
            else
            {
                // if the config exist, then only get the isAppOpen variable
                // and re-save it with the new overall config
                settings.Load();
                Init.Classes.ConfigurationGeneral configurationGeneral_2 = settings.configurationGeneral;

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

                if (checkBox_reprocess_activate.Checked == true && Convert.ToDouble(numericUpDown_hour.Value) == 0.0)
                {
                    MessageBox.Show("Heure de retraitement n'est pas configuré !", "Retraitement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string compt_g_taxe = "";
                if (textBox1.Enabled)
                {
                    compt_g_taxe = textBox1.Text.ToString();
                }
                else
                {
                    compt_g_taxe = "-1";
                }

                configurationGeneral.general = new Init.Classes.Configuration.General(show, configurationGeneral_2.general.isAppOpen, checkBox_activate_compt_g_taxe.Checked, Convert.ToInt32(compt_g_taxe.Trim()));
                configurationGeneral.paths = new Init.Classes.Configuration.Paths(textBox2.Text.Trim());
                configurationGeneral.plannerTask = new Init.Classes.Configuration.PlannerTask();
                configurationGeneral.priceType = new Init.Classes.Configuration.PriceType(checkBox_activer_tarif.Checked, radioButton_tarif_cmd_EDI.Checked, radioButton_tarif_produit.Checked, radioButton_tarif_categorie.Checked, radioButton_tarif_client.Checked);
                configurationGeneral.reprocess = new Init.Classes.Configuration.Reprocess(checkBox_reprocess_activate.Checked, numericUpDown_hour.Value, Convert.ToInt32(numericUpDown1_reprocess_cd.Value));
                settings.configurationGeneral = configurationGeneral;
                settings.saveInfo();
            }

            

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
                checkBox_reprocess_activate.Text = "Activer";
                numericUpDown_hour.Enabled = true;
                label7_reprocess_hour.Enabled = true;
                numericUpDown1_reprocess_cd.Enabled = true;
                label7_reprocess.Enabled = true;
                button_reprocess.Enabled = true;
                button_tmp.Enabled = true;
            }
            else
            {
                checkBox_reprocess_activate.Text = "Désactiver";
                numericUpDown_hour.Enabled = false;
                label7_reprocess_hour.Enabled = false;
                numericUpDown1_reprocess_cd.Enabled = false;
                label7_reprocess.Enabled = false;
                button_reprocess.Enabled = false;
                button_tmp.Enabled = false;
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
            double count = Convert.ToDouble(numericUpDown_hour.Value);
            if (count == 0.0)
            {
                label7_reprocess_hour.Text = "Ce paramètre est obligatoire\nSi activé !!!.";
            }
            else
            {
                label7_reprocess_hour.Text = "Retraitement tous les " + count + " heure(s).";
            }
        }

        private void checkBox_activer_tarif_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_activer_tarif.Checked)
            {
                checkBox_activer_tarif.Text = "Activer";
                radioButton_tarif_cmd_EDI.Enabled = true;
                radioButton_tarif_produit.Enabled = true;
                radioButton_tarif_categorie.Enabled = true;
                radioButton_tarif_client.Enabled = true;
            }
            else
            {
                checkBox_activer_tarif.Text = "Désactiver";
                radioButton_tarif_cmd_EDI.Enabled = false;
                radioButton_tarif_cmd_EDI.Checked = false;

                radioButton_tarif_produit.Enabled = false;
                radioButton_tarif_produit.Checked = false;

                radioButton_tarif_categorie.Enabled = false;
                radioButton_tarif_categorie.Checked = false;

                radioButton_tarif_client.Enabled = false;
                radioButton_tarif_client.Checked = false;
            }
        }

        private void button_reprocess_Click(object sender, EventArgs e)
        {
            string directory = Directory.GetCurrentDirectory() + @"\" + "Error File";
            DirectoryInfo fileListing = new DirectoryInfo(directory);

            if (!Directory.Exists(directory))
            {
                //Create log directory
                Directory.CreateDirectory(directory);
            }

            FileInfo[] allFiles = fileListing.GetFiles("*.csv");

            if(allFiles.Length > 0)
            {
                DialogResult resultDialog = MessageBox.Show("Voulez vous vraiment traiter les " + allFiles.Length + " fichier(s) EDI en erreur maitenant ?",
                                                         "Warning Retraitement Erreur !",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);

                if (resultDialog == DialogResult.Yes)
                {
                    try
                    {
                        Reprocess.ReprocessErrorFiles reprocessErrorFiles = new Reprocess.ReprocessErrorFiles();
                        reprocessErrorFiles.reprocessManually();
                        MessageBox.Show("Tous les Fichiers EDI sont transférés dans le repairtoir EDI.\nLors du prochain traitement du connecteur, ces fichiers seront traités.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message : " + ex.Message + "\n\nStackTrace : " + ex.StackTrace, "Retraitement Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (resultDialog == DialogResult.No)
                {

                }
            }
        }

        private void button_tmp_Click(object sender, EventArgs e)
        {
            string directory = Directory.GetCurrentDirectory() + @"\" + "tmp";
            DirectoryInfo fileListing = new DirectoryInfo(directory);

            if (!Directory.Exists(directory))
            {
                //Create log directory
                Directory.CreateDirectory(directory);
            }

            FileInfo[] allFiles = fileListing.GetFiles("*.csv");

            if (allFiles.Length > 0)
            {
                DialogResult resultDialog = MessageBox.Show("Voulez vous vraiment traiter les " + allFiles.Length + " fichier(s) EDI dans la poubelle maitenant ?\nVérifiez que ces fichiers ou documents sont corrects avant de les retraiter!",
                                                         "Warning Retraitement Poubelle !!",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);

                if (resultDialog == DialogResult.Yes)
                {
                    try
                    {
                        Reprocess.ReprocessErrorFiles reprocessErrorFiles = new Reprocess.ReprocessErrorFiles();
                        reprocessErrorFiles.reprocessTmpManually();
                        MessageBox.Show("Tous les Fichiers EDI dans la poubelle sont transférés dans le repairtoir EDI.\nLors du prochain traitement du connecteur, ces fichiers seront traités.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message : " + ex.Message + "\n\nStackTrace : " + ex.StackTrace, "Retraitement Poubelle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (resultDialog == DialogResult.No)
                {

                }
            }
        }
    }
}
