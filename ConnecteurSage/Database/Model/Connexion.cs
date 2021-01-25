using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Model
{
    public class Connexion
    {
        public int id;
        public string type;
        public string dns; 
        public string user;
        public string pwd;

        public Connexion() { }
        public Connexion(int id, string type, string dns, string pwd)
        {
            this.id = id;
            this.type = type;
            this.dns = dns;
            this.pwd = pwd;
        }
    }
}
