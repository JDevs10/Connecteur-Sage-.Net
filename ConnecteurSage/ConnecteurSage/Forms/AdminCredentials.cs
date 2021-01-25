using ConnecteurSage.Classes;
using Newtonsoft.Json;
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
    

    public partial class AdminCredentials : Form
    {
        public Credentials admin = new Credentials();

        public AdminCredentials(object admin)
        {
            InitializeComponent();
            this.admin = (Credentials) admin;
            this.text_user.Text = this.admin.User;

            //this.label1.Text = ""+JsonConvert.SerializeObject(this.admin, Newtonsoft.Json.Formatting.Indented);
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            admin.User = this.text_user.Text;
            admin.Password = this.text_pwd.Text;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
