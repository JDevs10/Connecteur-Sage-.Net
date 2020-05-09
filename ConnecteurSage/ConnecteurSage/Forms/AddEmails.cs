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
    public partial class AddEmails : Form
    {
        private List<string> emailList_ = new List<string>();
        BindingList<string> emailList = new BindingList<string>();
        public List<string> mEmailList 
        {
            get { return this.emailList_; }
            set { this.emailList_ = value; }
        }

        private string mFrom = "";

        public AddEmails(string to, List<string> list)
        {
            InitializeComponent();

            this.mFrom = to;
            Text = "Ajouter E-Mail " + mFrom;

            this.emailList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                this.emailList.Add(list[i].ToString());
            }
            listBox1.DataSource = emailList;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index != -1)
            {
                DialogResult resultDialog = MessageBox.Show("Voulez-vous vraiment supprimer \"" + listBox1.SelectedItem.ToString() + "\" de la list de diffusion des e-mails " + mFrom + " ?",
                                                            "Supprimer l'e-mail",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question);
                if (resultDialog == DialogResult.Yes)
                {
                    emailList.RemoveAt(index);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button_add_email_Click(object sender, EventArgs e)
        {
            // check if input is valid email
            if(!textBox1.Text.Trim().Equals(""))
            {
                if (new Alert_Mail.Classes.ConfigurationEmail().isValid_Email(textBox1.Text.Trim()))
                {
                    emailList.Add(textBox1.Text.Trim());
                    textBox1.Text = "";
                }
                else
                {
                    MessageBox.Show("l'adresse e-mail suivante \"" + textBox1.Text.Trim() + "\" n'est pas valide !",
                                    "Format e-mail",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            mEmailList.Clear();
            for(int i = 0; i < emailList.Count; i++)
            {
                mEmailList.Add(emailList[i].ToString());
            }
        }

        
    }
}
