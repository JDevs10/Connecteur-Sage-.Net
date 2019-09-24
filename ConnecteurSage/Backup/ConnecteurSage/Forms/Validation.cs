using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConnecteurSage.Helpers;


namespace ConnecteurSage.Forms
{
    public partial class Validation : Form
    {
        private string motcle = "";
        public static Boolean isValide = false;

        public Validation()
        {
            InitializeComponent();
            Enregistre_bouton.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            motcle = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text;

            if(textBox1.Text.Length == 4)
            {
                textBox2.Focus();
            }

            if (motcle.Length == 16)
            {
                Enregistre_bouton.Enabled = true;
            }
            else
            {
                Enregistre_bouton.Enabled = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            motcle = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text;
            if (textBox2.Text.Length == 4)
            {
                textBox3.Focus();
            }
            if (motcle.Length == 16)
            {
                Enregistre_bouton.Enabled = true;
            }
            else
            {
                Enregistre_bouton.Enabled = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            motcle = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text;
            if (textBox3.Text.Length == 4)
            {
                textBox4.Focus();
            }
            if (motcle.Length == 16)
            {
                Enregistre_bouton.Enabled = true;
            }
            else
            {
                Enregistre_bouton.Enabled = false;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            motcle = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text;
            if (textBox4.Text.Length == 4)
            {
                Enregistre_bouton.Focus();
            }
            if (motcle.Length == 16)
            {
                Enregistre_bouton.Enabled = true;
            }
            else
            {
                Enregistre_bouton.Enabled = false;
            }
        }

        private void Enregistre_bouton_Click(object sender, EventArgs e)
        {

            if (ConnecteurSage.Classes.GenererCle.decrypter(motcle))
            {
                Random random = new Random();
                char[] tab0 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                string value0 = tab0[random.Next(0, 9)] + "" + tab0[random.Next(0, 9)] + "" + tab0[random.Next(0, 9)] + "" + tab0[random.Next(0, 9)] + "" + tab0[random.Next(0, 9)];
                //MessageBox.Show(value0);
                // Enregister clé dans le registre
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\TrackBack\\Values");
                key.SetValue("Version", "0.1");
                key.SetValue("Name", "");
                key.SetValue("Key", Utils.Encrypt(value0+""+motcle));
                key.SetValue("Value0", Utils.Encrypt(value0));
                key.SetValue("Value1", "1709");
                key.Close();

                isValide = true;

                MessageBox.Show("Votre clé de licence est enregistrée", "",
                     MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                Close();

            }
            else
            {
                MessageBox.Show("Votre clé de licence n'est pas valide", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }


        }

    }
}
