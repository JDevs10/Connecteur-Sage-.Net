using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnecteurSage.Helpers;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ConnecteurSage.Classes;
using System.IO;
using System.Xml.Serialization;

namespace ConnecteurSage.Forms
{
    public partial class ConfMail : Form
    {
        private Microsoft.Win32.RegistryKey key;

        public ConfMail()
        {
            try
            {
                InitializeComponent();
                if (File.Exists(Directory.GetCurrentDirectory() + @"\SettingMail.xml"))
                {
                    ConfSendMail mailSettings = new ConfSendMail();
                    mailSettings.Load();
                    checkBox1.Checked = mailSettings.active;
                    textBox1.Text = mailSettings.smtp;
                    textBox2.Text = mailSettings.port+"";
                    textBox4.Text = Utils.Decrypt(mailSettings.password);
                    textBox3.Text = mailSettings.login;
                    textBox5.Text = mailSettings.dest1;
                    textBox6.Text = mailSettings.dest2;
                    textBox7.Text = mailSettings.dest3;

                    if (!checkBox1.Checked)
                    {
                        groupBox1.Enabled = false;
                        groupBox2.Enabled = false;
                        enregistrer_config.Enabled = false;
                    }
                }
                else
                {
                    checkBox1.Checked = false;
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    enregistrer_config.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void enregistrer_config_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox5.Text == "" && textBox6.Text == "" && textBox7.Text == "" && checkBox1.Checked)
                {
                    MessageBox.Show("Adresse Destinataire est obligatoire (il en faut au moins un).", "Erreur!!",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (textBox5.Text != "" && !IsValidEmail(textBox5.Text) && checkBox1.Checked)
                {
                    MessageBox.Show("Adresse destinataire 1 n'est pas valide.", "Erreur!!",
                                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (textBox6.Text != "" && !IsValidEmail(textBox6.Text) && checkBox1.Checked)
                {
                    MessageBox.Show("Adresse destinataire 2 n'est pas valide.", "Erreur!!",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (textBox7.Text != "" && !IsValidEmail(textBox7.Text) && checkBox1.Checked)
                {
                    MessageBox.Show("Adresse destinataire 3 n'est pas valide.", "Erreur!!",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //CreerCleRegistre(new ConfSendMail(textBox1.Text, textBox2.Text == "" ? 587:int.Parse(textBox2.Text), textBox3.Text, Utils.Encrypt(textBox4.Text),textBox5.Text,textBox6.Text,textBox7.Text,checkBox1.Checked));

                ConfSendMail mailSettings = new ConfSendMail()
                {
                    smtp = textBox1.Text,
                    port = textBox2.Text == "" ? 587 : int.Parse(textBox2.Text),
                    login = textBox3.Text,
                    password = Utils.Encrypt(textBox4.Text),
                    dest1 = textBox5.Text,
                    dest2 = textBox6.Text,
                    dest3 = textBox7.Text,
                    active = checkBox1.Checked,
                    totalTicks = (210 / 5),   //3.5h x 60mins = 210mins; 210mins/5mins = 42 ticks
                    remaningTicks = (210 / 5)
                };
                mailSettings.saveInfo(mailSettings);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static bool IsValidEmail(string email)
        {
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                //enregistrer_config.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                enregistrer_config.Enabled = true;
            }
        }

        public void CreerCleRegistre(ConfSendMail cMail)
        {
            try
            {
                key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Sage\\Connecteur sage");
                key.SetValue("Version", "0.1");
                key.SetValue("Name", "Connecteur sage");
                key.SetValue("smtp", cMail.smtp);
                key.SetValue("port", cMail.port);
                key.SetValue("login", cMail.login);
                key.SetValue("password", cMail.password);
                key.SetValue("dest1", cMail.dest1);
                key.SetValue("dest2", cMail.dest2);
                key.SetValue("dest3", cMail.dest3);
                key.SetValue("active", cMail.active);
                key.Close();
                Close();
            } 
            catch ( Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
