using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnecteurSage.Classes
{
    public class Credentials
    {
        public string User { get; set; }
        public string Password { get; set; }

        public Credentials()
        {

        }
        public Credentials(string User, string Password)
        {
            this.User = User;
            this.Password = Password;
        }
    }
}
