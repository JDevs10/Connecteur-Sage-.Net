using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Xml;
using ConnecteurSage.Helpers;
using System.Data.SqlClient;
using System.Xml.Serialization;
using ConnecteurSage.Classes;

namespace ConnecteurSage.Forms
{
    public partial class Planifier : Form
    {
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string taskName;

        public Planifier()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            string path = ReturnPath();
            taskName = ReturnTaskName();

            if(taskName == null)
            {
                DialogResult resultDialog = MessageBox.Show("La configuration de la connexion ODBC est manquante...\nVoulez-vous le remplir maintenant.",
                                                "Information !",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question,
                                                MessageBoxDefaultButton.Button2);
                if (resultDialog == DialogResult.No)
                {
                    Close();
                }

                if (resultDialog == DialogResult.Yes)
                {
                    try
                    {
                        using (Forms.ConfigConnexion form = new Forms.ConfigConnexion())
                        {
                            form.ShowDialog();
                        }
                        Close();
                    }
                    catch (SDKException ex)
                    {
                        MessageBox.Show(ex.Message);
                        Close();
                    }
                }
                
            }

            TaskService ts = new TaskService();
            if (ts.FindTask(taskName, true) != null && path != null)
            {
                checkBox1.Checked = true;
                Task t = ts.GetTask(taskName);
                TaskDefinition td = t.Definition;
                label6.Text = "Tâche Planifiée :";
                string info = td.Triggers[0].ToString();
                if (info.Length > 50)
                {
                    label7.Text = "" + info.Insert(56,"\n") ;
                }
                else
                {
                    label7.Text = "" + info;
                }
                dateTimePicker2.Text = "" + td.Triggers[0].StartBoundary.ToString().Substring(0, 10);
                dateTimePicker1.Text = "" + td.Triggers[0].StartBoundary.ToString().Substring(11, 8);

                int interval = int.Parse(td.Triggers[0].Repetition.Interval.ToString().Substring(0, 2));
                
                if(interval != 0)
                {

                comboBox2.Text = "" + interval;
                checkBox2.Checked = true;

                }
                else
                {
                checkBox2.Checked = false;
                }

            }

            if ((ts.FindTask(taskName, true) == null && path == null) || (ts.FindTask(taskName, true) != null && path == null) || (ts.FindTask(taskName, true) == null && path != null))
            {

                groupBox1.Enabled = false;
                enregistrerButton.Enabled = false;
                checkBox1.Checked = false;
                checkBox2.Checked = false;

                label6.Text = "Tâche Planifiée :";
                label7.Text = "cochez la case ci-dessous";

            }

            /*
            if (ts.FindTask(taskName, true) != null && path == null) 
            {
                //Task t = ts.GetTask(taskName);
                //TaskDefinition td = t.Definition;
                //t.Enabled = false;
                ts.RootFolder.DeleteTask(taskName);
            }
            */
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                groupBox1.Enabled = false;
                groupBox3.Enabled = false;
                checkBox2.Enabled = false;
                enregistrerButton.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
                checkBox2.Enabled = true;
                groupBox3.Enabled = true;
                enregistrerButton.Enabled = true;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    //Enregistrer la tache planifiée
                    EnregistrerLaTache(dateTimePicker2.Text, dateTimePicker1.Text);

                    //Enregistrer l'emplacement dans Path.xml
                    /*
                    Classes.Path path = new Classes.Path(textBox1.Text);

                    var myfile = File.Create(pathModule+@"\Path.xml");
                    XmlSerializer xml = new XmlSerializer(typeof(Classes.Path));
                    xml.Serialize(myfile, path);
                    myfile.Close();


                    //Enregistrement du statu export
                    Classes.ConfigurationExport export = new Classes.ConfigurationExport(
                        ((checkBox5.Checked == true) ? "True" : "False"), ((checkBox4.Checked == true) ? "True" : "False"), ((checkBox3.Checked == true) ? "True" : "False"), ((checkBox6.Checked == true) ? "True" : "False")
                        );

                    var myfile1 = File.Create(pathModule + @"\SettingExport.xml");
                    XmlSerializer xml1 = new XmlSerializer(typeof(Classes.ConfigurationExport));
                    xml1.Serialize(myfile1, export);
                    myfile1.Close();
                    */
                    // ***********************************************************************
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
                }
            }
            else
            {
                TaskService ts = new TaskService();
                if (ts.FindTask(taskName, true) != null)
                {
                    //ts.RootFolder.
                    ts.RootFolder.DeleteTask(taskName);
                }

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Planifier_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog.

            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {

                textBox1.Text = folderDlg.SelectedPath;

                //Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }
        */

        public void EnregistrerLaTache(string date,string time)
        {
            string taskName = ReturnTaskName();
            if(taskName == null)
            {
                MessageBox.Show("La tache " + taskName + " n'est pas enregistré !", "Tache Planifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DailyTrigger dt = null;
            try
            {
                TaskService ts = new TaskService();
                TaskDefinition td = ts.NewTask();
                td.Principal.UserId = "SYSTEM";
                td.Principal.LogonType = TaskLogonType.S4U;
                td.Settings.AllowDemandStart = true;
                td.Principal.RunLevel = TaskRunLevel.Highest;
                dt = (DailyTrigger)td.Triggers.Add(new DailyTrigger(1));
                dt.StartBoundary = DateTime.Parse(date) + TimeSpan.FromHours(Convert.ToDouble(time.Substring(0, 2))) + TimeSpan.FromMinutes(Convert.ToDouble(time.Substring(3, 2)));
                if (checkBox2.Checked)
                {
                    dt.Repetition.Duration = TimeSpan.FromDays(1);
                    dt.Repetition.Interval = TimeSpan.FromHours(int.Parse(comboBox2.Text));
                }
                td.Actions.Add(new ExecAction(pathModule + @"\importPlanifier.exe", null, null));
                ts.RootFolder.RegisterTaskDefinition(taskName, td);
                td.Settings.Enabled = true;

                Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
                if (settings.isSettings())
                {
                    settings.Load();
                    settings.configurationGeneral.plannerTask = new Init.Classes.Configuration.PlannerTask(taskName, td.Principal.UserId, td.Settings.Enabled);
                    settings.saveInfo();
                }
                else
                {
                    settings.configurationGeneral = new Init.Classes.ConfigurationGeneral();
                    settings.configurationGeneral.general = new Init.Classes.Configuration.General();
                    settings.configurationGeneral.paths = new Init.Classes.Configuration.Paths();
                    settings.configurationGeneral.plannerTask = new Init.Classes.Configuration.PlannerTask(taskName, td.Principal.UserId, td.Settings.Enabled);
                    settings.configurationGeneral.priceType = new Init.Classes.Configuration.PriceType();
                    settings.configurationGeneral.reprocess = new Init.Classes.Configuration.Reprocess();
                    settings.saveInfo();
                }

                MessageBox.Show("La tache " + taskName + " est enregistré !", "Tache Planifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message : " + ex.Message, "Erreur de création", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        public string ReturnTaskName()
        {
            string taskName = "ConnecteurSage";
            try
            {
                Connexion.ConnexionSaveLoad settings = new Connexion.ConnexionSaveLoad();
                if (!settings.isSettings())
                {
                    MessageBox.Show("Veuillez renseigner la configuration de connexion ODBC d'abord.", "Config Général", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
                settings.Load();
                taskName += "_" + settings.configurationConnexion.SQL.PREFIX;
                return taskName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string ReturnPath()
        {

            try
            {
                Init.Classes.SaveLoadInit settings = new Init.Classes.SaveLoadInit();
                if (settings.isSettings())
                {
                    settings.Load();
                    return settings.configurationGeneral.paths.EDI_Folder;
                }
                else
                {
                    return null;
                }
            }
            catch 
            {
                //Exception pouvant survenir si l'objet SqlConnection est dans l'état 'Fermé'
                MessageBox.Show("Erreur[P1] : path file.");
                return null;
            }
           
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                comboBox2.Enabled = false;

            }
            else
            {
                comboBox2.Enabled = true;

            }


        }

    }
}
