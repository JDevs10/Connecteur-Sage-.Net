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
        private Database.Database db = null;
        private string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private string taskName;
        BindingList<string> repeatList = new BindingList<string>();

        public Planifier()
        {
            InitializeComponent();

            this.repeatList.Clear();
            int i = 5;
            while (i < 60)
            {
                this.repeatList.Add("" + i);
                i = i + 5;
            }
            this.repeatList.Add("Indefinitely");
            comboBox2.DataSource = repeatList;

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Init database && tables
            db = new Database.Database();
            db.initTables();
            string path = db.settingsManager.get(db.connectionString, 1).EDI_Folder;
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

                //MessageBox.Show("Obj :\n"+new Init.Init().FormatJson(td.Triggers[0].Repetition));
                
                int interval = int.Parse(td.Triggers[0].Repetition.Interval.ToString().Split(':')[1]);

                //MessageBox.Show("Obj :\nInterval :" + td.Triggers[0].Repetition.Interval.ToString() + " || Sub: " + td.Triggers[0].Repetition.Interval.ToString().Split(':')[1] + "\nint: " + interval);

                if (interval != 0)
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
            }
            else
            {
                groupBox1.Enabled = true;
                checkBox2.Enabled = true;
                groupBox3.Enabled = true;
            }
            enregistrerButton.Enabled = true;
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


        public void EnregistrerLaTache(string date,string time)
        {
            string taskName = ReturnTaskName();
            if(taskName == null)
            {
                MessageBox.Show("La tache " + taskName + " n'est pas enregistré !", "Tache Planifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DailyTrigger dt = null;
            if (checkBox1.Checked)
            {
                Classes.Credentials admin = new Classes.Credentials(string.Concat(Environment.UserDomainName, "\\", Environment.UserName), "");

                try
                {
                    Forms.AdminCredentials adminCredentials = new Forms.AdminCredentials(admin);

                    if (adminCredentials.ShowDialog() == DialogResult.OK)
                    {
                        admin = adminCredentials.admin;
                        //MessageBox.Show("Admin : \n"+new Init.Init().FormatJson(admin), "Info");
                    }
                    else if (adminCredentials.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message, "Erreur[100]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    TaskService ts = new TaskService();
                    TaskDefinition td = ts.NewTask();
                    td.Principal.UserId = string.Concat(Environment.UserDomainName, "\\", Environment.UserName); // "SYSTEM";
                    td.RegistrationInfo.Author = string.Concat(Environment.UserDomainName, "\\", Environment.UserName); // "SYSTEM";
                    td.Principal.LogonType = TaskLogonType.S4U;
                    td.Settings.AllowDemandStart = true;
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    dt = (DailyTrigger)td.Triggers.Add(new DailyTrigger(1));
                    string[] times = time.Split(':');
                    try
                    {
                        //MessageBox.Show("Date: " + date + " | time: " + time + "\nTime 1: " + times[0] + " | Time 2: " + times[1] + " | Time 3: " + times[2].Split(' ')[0], "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dt.StartBoundary = DateTime.Parse(date) + TimeSpan.FromHours(Convert.ToDouble(times[0])) + TimeSpan.FromMinutes(Convert.ToDouble(times[1]));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message : " + ex.Message+ "\nDate: " + date + " | time: " + time + "\nTime 1: " + times[0] + " | Time 2: " + times[1] + " | Time 3: " + times[2].Split(' ')[0], "Erreur[101]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (checkBox2.Checked)
                    {
                        //dt.Repetition.Duration = TimeSpan.FromDays(1);
                        dt.Repetition.Interval = TimeSpan.FromMinutes(int.Parse(comboBox2.Text));
                        dt.Repetition.StopAtDurationEnd = true;
                    }
                    td.Actions.Add(new ExecAction(pathModule + @"\ConnecteurAuto.exe", null, null));
                    //ts.RootFolder.RegisterTaskDefinition(taskName, td);
                    ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, admin.User, admin.Password, TaskLogonType.Password, null);
                    td.Settings.Enabled = true;

                    int result = -99;

                    Database.Model.Settings settings = db.settingsManager.get(db.connectionString, 1);
                    if(settings != null)
                    {
                        settings.plannerTask_active = td.Settings.Enabled ? 1 : 0;
                        settings.plannerTask_UserId = td.Principal.UserId;
                        settings.plannerTask_name = taskName;

                        result = db.settingsManager.update(db.connectionString, settings);
                    }
                    else
                    {
                        MessageBox.Show("Veuillez d'abord enregistrer les paramètres généraux", "Erreur de création", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if(result > 0)
                    {
                        MessageBox.Show("La tache " + taskName + " est enregistré !", "Tache Planifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message, "Erreur de création", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                try
                {
                    TaskService ts = new TaskService();
                    ts.RootFolder.DeleteTask(taskName);

                    int result = -99;
                    Database.Model.Settings settings = db.settingsManager.get(db.connectionString, 1);
                    if (settings != null)
                    {
                        settings.plannerTask_active = 0; // is false | desable
                        settings.plannerTask_UserId = "";
                        settings.plannerTask_name = "";
                        result = db.settingsManager.update(db.connectionString, settings);
                    }
                    else
                    {
                        MessageBox.Show("Veuillez d'abord enregistrer les paramètres généraux", "Erreur de création", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (result > 0)
                    {
                        MessageBox.Show("La tache " + taskName + " est supprimé !", "Tache Planifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message, "Erreur de suppréssion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            Close();
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
