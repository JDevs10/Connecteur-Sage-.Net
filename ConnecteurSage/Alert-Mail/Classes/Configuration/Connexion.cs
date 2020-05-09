using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Classes.Configuration
{
    public class Connexion
    {
        public string smtp { get; set; }
        public string port { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public Connexion()
        {

        }

        public Connexion(string smtp, string port, string login, string password)
        {
            this.smtp = smtp;
            this.port = port;
            this.login = login;
            this.password = password;
        }
    }
}
