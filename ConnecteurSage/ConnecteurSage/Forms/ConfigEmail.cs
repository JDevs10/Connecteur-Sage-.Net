using Alert_Mail.Classes;
using ConnecteurSage.Classes;
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
    public partial class ConfigEmail : Form
    {
        private List<string> clientEmails = new List<string>();
        private List<string> teamEmails = new List<string>();

        public ConfigEmail()
        {
            InitializeComponent();

            loadConfiguration();
        }

        private void loadConfiguration()
        {
            Alert_Mail.Classes.ConfigurationSaveLoad settings = new Alert_Mail.Classes.ConfigurationSaveLoad();
            if (settings.isSettings())
            {
                settings.Load();

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// AlertMail Activation
                /// 
                checkBox1.Checked = settings.configurationEmail.active;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Connexion configuration
                /// 
                textBox_smtp.Text = Helpers.Utils.Decrypt(settings.configurationEmail.connexion.smtp);
                textBox_port.Text = Helpers.Utils.Decrypt(settings.configurationEmail.connexion.port);
                textBox_login.Text = Helpers.Utils.Decrypt(settings.configurationEmail.connexion.login);
                textBox_pwd.Text = Helpers.Utils.Decrypt(settings.configurationEmail.connexion.password);

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Email lists configuration
                /// 
                checkBox_activate_send_client_emails.Checked = settings.configurationEmail.emailLists.activeClient;
                this.clientEmails = settings.configurationEmail.emailLists.emailClientList;
                string msg = "";
                if (clientEmails.Count < 1)
                {
                    msg = clientEmails[0];
                }
                else
                {
                    msg += clientEmails[0];
                    for (int x = 1; x < clientEmails.Count; x++)
                    {
                        msg += "\n" + clientEmails[x];
                    }
                }
                label_client_email_list.Text = msg;

                checkBox_activate_send_team_emails.Checked = settings.configurationEmail.emailLists.activeTeam;
                this.teamEmails = settings.configurationEmail.emailLists.emailTeamList;
                msg = "";
                if (teamEmails.Count < 1)
                {
                    msg = teamEmails[0];
                }
                else
                {
                    msg += teamEmails[0];
                    for (int x = 1; x < teamEmails.Count; x++)
                    {
                        msg += "\n" + teamEmails[x];
                    }
                }
                label_team_email_list.Text = msg;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Inport mail configuration
                /// 
                checkBox4.Checked = settings.configurationEmail.emailImport.active;
                radioButton1.Checked = settings.configurationEmail.emailImport.eachDocument;
                radioButton2.Checked = settings.configurationEmail.emailImport.atTheEnd;
                checkBox5.Checked = settings.configurationEmail.emailImport.informClient;
                checkBox6.Checked = settings.configurationEmail.emailImport.informTeam;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Export mail configuration
                /// 
                checkBox9.Checked = settings.configurationEmail.emailExport.active;
                radioButton3.Checked = settings.configurationEmail.emailExport.eachDocument;
                radioButton4.Checked = settings.configurationEmail.emailExport.atTheEnd;
                checkBox8.Checked = settings.configurationEmail.emailExport.informClient;
                checkBox7.Checked = settings.configurationEmail.emailExport.informTeam;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Error mail configuration
                /// 
                checkBox12.Checked = settings.configurationEmail.emailError.active;
                checkBox11.Checked = settings.configurationEmail.emailError.informClient;
                checkBox10.Checked = settings.configurationEmail.emailError.informTeam;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Summary mail  configuration
                /// 
                checkBox15.Checked = settings.configurationEmail.emailSummary.active;
                numericUpDown1.Value = settings.configurationEmail.emailSummary.hours;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                /// Inport configuration
                /// 
                checkBox13.Checked = settings.configurationEmail.emailPendingFiles.active;
                numericUpDown2.Value = settings.configurationEmail.emailPendingFiles.hours;
            }
        }

        private void cancel_config_Click(object sender, EventArgs e)
        {
            Close();
            /*
            ConfigurationSaveLoad settings = new ConfigurationSaveLoad();
            settings.Load();
            MessageBox.Show(settings.FormatJson(), "Json");
            */
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
                groupBox5.Enabled = true;
                groupBox6.Enabled = true;
                groupBox7.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                groupBox5.Enabled = false;
                groupBox6.Enabled = false;
                groupBox7.Enabled = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
            }
            else
            {
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                checkBox8.Enabled = true;
                checkBox7.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
            }
            else
            {
                checkBox8.Enabled = false;
                checkBox7.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                checkBox11.Enabled = true;
                checkBox10.Enabled = true;
            }
            else
            {
                checkBox11.Enabled = false;
                checkBox10.Enabled = false;
            }
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked)
            {
                label12.Enabled = true;
                label13.Enabled = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                label12.Enabled = false;
                label13.Enabled = false;
                numericUpDown1.Enabled = false;
            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                label15.Enabled = true;
                label14.Enabled = true;
                numericUpDown2.Enabled = true;
            }
            else
            {
                label15.Enabled = false;
                label14.Enabled = false;
                numericUpDown2.Enabled = false;
            }
        }

        private void button_add_client_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.AddEmails addEmails = new Forms.AddEmails("client", clientEmails);

                if (addEmails.ShowDialog() == DialogResult.OK)
                {
                    clientEmails = addEmails.mEmailList;

                    string msg = "";
                    if(clientEmails.Count < 1)
                    {
                        msg = clientEmails[0];
                    }
                    else
                    {
                        msg += clientEmails[0];
                        for (int x = 1; x < clientEmails.Count; x++)
                        {
                            msg += "\n" + clientEmails[x];
                        }
                    }
                    label_client_email_list.Text = msg;
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_add_team_emails_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.AddEmails addEmails = new Forms.AddEmails("équipe", teamEmails);
                
                if (addEmails.ShowDialog() == DialogResult.OK)
                {
                    teamEmails = addEmails.mEmailList;

                    string msg = "";
                    if (teamEmails.Count < 1)
                    {
                        msg = teamEmails[0];
                    }
                    else
                    {
                        msg += teamEmails[0];
                        for (int x = 1; x < teamEmails.Count; x++)
                        {
                            msg += "\n" + teamEmails[x];
                        }
                    }
                    label_team_email_list.Text = msg;
                }
            }
            // Récupération d'une possible SDKException
            catch (SDKException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void save_config_Click(object sender, EventArgs e)
        {
            int _;
            if(!textBox_port.Text.Equals("") && !int.TryParse(textBox_port.Text, out _))
            {
                MessageBox.Show("Seulement les chiffres entre 0 à 9 sont valide!", "Port de Connexion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ConfigurationEmail mConfigurationEmail = new ConfigurationEmail(
                checkBox1.Checked,
                new Alert_Mail.Classes.Configuration.Connexion(
                    Helpers.Utils.Encrypt(textBox_smtp.Text.Trim()),
                    Helpers.Utils.Encrypt(textBox_port.Text.Trim()),
                    Helpers.Utils.Encrypt(textBox_login.Text.Trim()),
                    Helpers.Utils.Encrypt(textBox_pwd.Text.Trim())),
                new Alert_Mail.Classes.Configuration.EmailLists(
                    checkBox_activate_send_client_emails.Checked,
                    clientEmails,
                    checkBox_activate_send_team_emails.Checked,
                    teamEmails),
                new Alert_Mail.Classes.Configuration.EmailImport(
                    checkBox4.Checked,
                    radioButton1.Checked,
                    radioButton2.Checked,
                    checkBox5.Checked,
                    checkBox6.Checked),
                new Alert_Mail.Classes.Configuration.EmailExport(
                    checkBox9.Checked,
                    radioButton3.Checked,
                    radioButton4.Checked,
                    checkBox8.Checked,
                    checkBox7.Checked),
                new Alert_Mail.Classes.Configuration.EmailError(
                    checkBox12.Checked,
                    checkBox11.Checked,
                    checkBox10.Checked),
                new Alert_Mail.Classes.Configuration.EmailSummary(
                    checkBox15.Checked,
                    Convert.ToInt16(numericUpDown1.Value),
                    DateTime.Now),
                new Alert_Mail.Classes.Configuration.EmailPendingFiles(
                    checkBox13.Checked,
                    Convert.ToInt16(numericUpDown2.Value))
            );

            ConfigurationSaveLoad settings = new ConfigurationSaveLoad(mConfigurationEmail);
            settings.saveInfo();

            Close();
        }
    }
}
