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
        public string name;
        public string password;

        public Connexion() { }
        public Connexion(int id, string type, string dns, string name, string password)
        {
            this.id = id;
            this.type = type;
            this.dns = dns;
            this.name = name;
            this.password = password;
        }
    }
}
