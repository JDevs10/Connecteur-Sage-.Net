using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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

            // Init Connecteur Info, Version and Database path in a file
            Init.Init init = new Init.Init();
            if (init.isSettings())
            {
                init.Load();
                init.connecteurInfo.installation_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                init.saveInfo();
            }
            else
            {
                init.connecteurInfo = new Connecteur_Info.ConnecteurInfo();
                init.connecteurInfo.installation_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                init.saveInfo();
            }

            // Init database
            Database.Database db = new Database.Database();
            Database.Model.Settings settings_ = db.settingsManager.get(db.connectionString ,1);

            if(settings_ != null)
            {
                if (settings_.showWindow == 5)
                {
                    // visible Software while running
                    debugMode_checkBox.Checked = true;
                }
                else if (settings_.showWindow == 0)
                {
                    // Hide Software while running
                    debugMode_checkBox.Checked = false;
                }

                /*
                if (settings_.isAppOpen)
                {
                    label3.Text = "Planificateur en Cour...";
                }
                else
                {
                    label3.Text = "Planificateur est fermet.";
                }
                */

                if (settings_.isACP_ComptaCPT_CompteG == 1 ? true : false)
                {
                    checkBox_activate_compt_g_taxe.Text = "Activer";
                    checkBox_activate_compt_g_taxe.Checked = true;
                    textBox1.Enabled = true;
                    textBox1.Text = "" + settings_.ACP_ComptaCPT_CompteG;
                }
                else
                {
                    checkBox_activate_compt_g_taxe.Text = "Désactiver";
                    checkBox_activate_compt_g_taxe.Checked = false;
                    textBox1.Enabled = false;
                    textBox1.Text = "" + settings_.ACP_ComptaCPT_CompteG;
                }

                textBox2.Text = settings_.EDI_Folder;

                ////////////////////////////////////////////////////////////////////////////////////
                /// Tarifaire
                /// 
                if (settings_.priceType_active == 1 ? true : false)
                {
                    checkBox_activer_tarif.Text = "Activer";
                    checkBox_activer_tarif.Checked = true;

                    radioButton_tarif_cmd_EDI.Enabled = true;
                    radioButton_tarif_cmd_EDI.Checked = settings_.priceType_cmdEDIPrice == 1 ? true : false;
                    radioButton_tarif_produit.Enabled = true;
                    radioButton_tarif_produit.Checked = settings_.priceType_productPrice == 1 ? true : false;
                    radioButton_tarif_categorie.Enabled = true;
                    radioButton_tarif_categorie.Checked = settings_.priceType_categoryPrice == 1 ? true : false;
                    radioButton_tarif_client.Enabled = true;
                    radioButton_tarif_client.Checked = settings_.priceType_clientPrice == 1 ? true : false;
                }
                else
                {
                    checkBox_activer_tarif.Text = "Désactiver";
                    checkBox_activer_tarif.Checked = false;

                    radioButton_tarif_cmd_EDI.Enabled = false;
                    radioButton_tarif_cmd_EDI.Checked = settings_.priceType_cmdEDIPrice == 1 ? true : false;
                    radioButton_tarif_produit.Enabled = false;
                    radioButton_tarif_produit.Checked = settings_.priceType_productPrice == 1 ? true : false;
                    radioButton_tarif_categorie.Enabled = false;
                    radioButton_tarif_categorie.Checked = settings_.priceType_categoryPrice == 1 ? true : false;
                    radioButton_tarif_client.Enabled = false;
                    radioButton_tarif_client.Checked = settings_.priceType_clientPrice == 1 ? true : false;
                }

                ////////////////////////////////////////////////////////////////////////////////////
                /// Reprocess
                /// 
                if (settings_.reprocess_active == 1 ? true : false)
                {
                    checkBox_reprocess_activate.Text = "Activer";
                    checkBox_reprocess_activate.Checked = true;
                }
                else
                {
                    checkBox_reprocess_activate.Text = "Désactiver";
                    checkBox_reprocess_activate.Checked = false;
                }

                numericUpDown_hour.Value = settings_.reprocess_hour;
                numericUpDown1_reprocess_cd.Value = settings_.reprocess_countDown;
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

            Database.Database db = new Database.Database();
            Database.Model.Settings settings_ = db.settingsManager.get(db.connectionString, 1);

            if (settings_ == null)
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

                if(textBox2.Text.Trim().Equals("") || textBox2.Text == null)
                {
                    MessageBox.Show("Veuillez ajouter le chemin du répertoire CSV.", "Erreur Répertoire", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


                try
                {
                    Database.Model.Settings settings = new Database.Model.Settings(
                    1, show, checkBox_activate_compt_g_taxe.Checked ? 1 : 0, Convert.ToInt32(compt_g_taxe.Trim()), @"" + Directory.GetCurrentDirectory(), @"" + textBox2.Text.Trim(),
                    "", "", 0,
                    checkBox_activer_tarif.Checked ? 1 : 0, radioButton_tarif_cmd_EDI.Checked ? 1 : 0, radioButton_tarif_produit.Checked ? 1 : 0, radioButton_tarif_categorie.Checked ? 1 : 0, radioButton_tarif_client.Checked ? 1 : 0,
                    checkBox_reprocess_activate.Checked ? 1 : 0, numericUpDown_hour.Value, Convert.ToInt32(numericUpDown1_reprocess_cd.Value));

                    int result = db.settingsManager.insert(db.connectionString, settings);

                    string TABLE_NAME = "Settings";
                    string COLONNE_ID = "id";
                    string COLONNE_showWindow = "showWindow";
                    string COLONNE_isACP_ComptaCPT_CompteG = "isACP_ComptaCPT_CompteG";
                    string COLONNE_ACP_ComptaCPT_CompteG = "ACP_ComptaCPT_CompteG";
                    // paths settings
                    string COLONNE_EXE_Folder = "EXE_Folder";
                    string COLONNE_EDI_Folder = "EDI_Folder";
                    // plannerTask settings
                    string COLONNE_plannerTask_name = "plannerTask_name";
                    string COLONNE_plannerTask_UserId = "plannerTask_UserId";
                    string COLONNE_plannerTask_active = "plannerTask_active";
                    //private settings
                    string COLONNE_priceType_active = "priceType_active";
                    string COLONNE_priceType_cmdEDIPrice = "priceType_cmdEDIPrice";
                    string COLONNE_priceType_productPrice = "priceType_productPrice";
                    string COLONNE_priceType_categoryPrice = "priceType_categoryPrice";
                    string COLONNE_priceType_clientPrice = "priceType_clientPrice";
                    // reprocess settings
                    string COLONNE_reprocess_active = "reprocess_active";
                    string COLONNE_reprocess_hour = "reprocess_hour";
                    string COLONNE_reprocess_countDown = "reprocess_countDown";


                    using (SQLiteConnection conn = new SQLiteConnection(db.connectionString))
                    {
                        try
                        {
                            string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_showWindow + "', '" + COLONNE_isACP_ComptaCPT_CompteG + "', '" + COLONNE_ACP_ComptaCPT_CompteG + "', '" + COLONNE_EXE_Folder + "', '" + COLONNE_EDI_Folder + "', '" + COLONNE_plannerTask_name + "', '" + COLONNE_plannerTask_UserId + "', '" + COLONNE_plannerTask_active + "', " +
                                "'" + COLONNE_priceType_active + "', '" + COLONNE_priceType_cmdEDIPrice + "', '" + COLONNE_priceType_productPrice + "', '" + COLONNE_priceType_categoryPrice + "', '" + COLONNE_priceType_clientPrice + "', " +
                                "'" + COLONNE_reprocess_active + "', '" + COLONNE_reprocess_hour + "', '" + COLONNE_reprocess_countDown + "') " +
                                "VALUES (1, " + settings.showWindow + ", " + settings.isACP_ComptaCPT_CompteG + ", " + settings.ACP_ComptaCPT_CompteG + ", '" + settings.EXE_Folder + "', '" + settings.EDI_Folder + "', '" + settings.plannerTask_name + "', '" + settings.plannerTask_UserId + "', " + settings.plannerTask_active + ", " +
                                "" + settings.priceType_active + ", " + settings.priceType_cmdEDIPrice + ", " + settings.priceType_productPrice + ", " + settings.priceType_categoryPrice + ", " + settings.priceType_clientPrice + ", " +
                                "" + settings.reprocess_active + ", " + settings.reprocess_hour.ToString().Replace(',', '.') + ", " + settings.reprocess_countDown + ")";

                            /*
                            string SQL_insert__ = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_showWindow + "', '" + COLONNE_isACP_ComptaCPT_CompteG + "', '" + COLONNE_ACP_ComptaCPT_CompteG + "', '" + COLONNE_EXE_Folder + "', '" + COLONNE_EDI_Folder + "', '" + COLONNE_plannerTask_name + "', '" + COLONNE_plannerTask_UserId + "', '" + COLONNE_plannerTask_active + "','" + COLONNE_priceType_active + "', '" + COLONNE_priceType_cmdEDIPrice + "', '" + COLONNE_priceType_productPrice + "', '" + COLONNE_priceType_categoryPrice + "', '" + COLONNE_priceType_clientPrice + "','" + COLONNE_reprocess_active + "', '" + COLONNE_reprocess_hour + "', '" + COLONNE_reprocess_countDown + "') VALUES " +
                                "(1," + settings.showWindow + ", " + settings.isACP_ComptaCPT_CompteG + ", " + settings.ACP_ComptaCPT_CompteG + ", '" + settings.EXE_Folder + "', '" + settings.EDI_Folder + "', '" + settings.plannerTask_name + "', '" + settings.plannerTask_UserId + "', " + settings.plannerTask_active + ", " +
                                "" + settings.priceType_active + ", " + settings.priceType_cmdEDIPrice + ", " + settings.priceType_productPrice + ", " + settings.priceType_categoryPrice + ", " + settings.priceType_clientPrice + ", " +
                                "" + settings.reprocess_active + ", " + settings.reprocess_hour.ToString().Replace(',','.') + ", " + settings.reprocess_countDown + ")";
                            */

                            /*
                            INSERT INTO Settings ('id', 'showWindow', 'isACP_ComptaCPT_CompteG', 'ACP_ComptaCPT_CompteG', 'EXE_Folder', 'EDI_Folder', 'plannerTask_name', 'plannerTask_UserId', 'plannerTask_active',
                            'priceType_active', 'priceType_cmdEDIPrice', 'priceType_productPrice', 'priceType_categoryPrice', 'priceType_clientPrice',
                            'reprocess_active', 'reprocess_hour', 'reprocess_countDown')
                            VALUES(1, 5, 1, 12347, 'Path EXE Folder', 'Path EDI Folder', '', '', 0,
                            1, 0, 0, 0, 1,
                            1, 0.5, 3)
                            */

                            //MessageBox.Show("SQL : " + SQL_insert + "\n\nJson: \n" + db.JsonFormat(settings), "Config Général", MessageBoxButtons.OK, MessageBoxIcon.Information);


                            int x = -9;
                            conn.Open();

                            if (conn.State == System.Data.ConnectionState.Open)
                            {
                                SQLiteCommand command = new SQLiteCommand(SQL_insert, conn);
                                x = command.ExecuteNonQuery();
                            }

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\nSettings Insert [ERROR]");
                            Console.WriteLine("Message : " + ex.Message);
                            Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                            conn.Close();
                        }
                    }


                    /*
                    if (result <= 0)
                    {
                        MessageBox.Show("result : " + result + "\n\nJson: \n" + db.JsonFormat(settings), "Config Général", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    */
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Config Général ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                // if the config exist, then only get the isAppOpen variable
                // and re-save it with the new overall config

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

                // Init database
                Database.Model.Settings db_settings = new Database.Model.Settings(
                    1, show, checkBox_activate_compt_g_taxe.Checked ? 1 : 0, Convert.ToInt32(compt_g_taxe.Trim()), Directory.GetCurrentDirectory(), textBox2.Text.Trim(),
                    "", "", 0,
                    checkBox_activer_tarif.Checked ? 1 : 0, radioButton_tarif_cmd_EDI.Checked ? 1 : 0, radioButton_tarif_produit.Checked ? 1 : 0, radioButton_tarif_categorie.Checked ? 1 : 0, radioButton_tarif_client.Checked ? 1 : 0,
                    checkBox_reprocess_activate.Checked ? 1 : 0, numericUpDown_hour.Value, Convert.ToInt32(numericUpDown1_reprocess_cd.Value));
                db.settingsManager.update(db.connectionString, db_settings);
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

        private void GeneralConfig_Load(object sender, EventArgs e)
        {

        }
    }
}
