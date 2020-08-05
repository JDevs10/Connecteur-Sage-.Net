using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connexion.Classes.Custom
{
    public class ODBC
    {
        public int ID;
        public string DNS;
        public string USER;
        public string PWD;

        public ODBC() { }
        public ODBC(int ID, string DNS_, string USER_, string PWD_)
        {
            this.ID = ID;
            this.DNS = DNS_;
            this.USER = USER_;
            this.PWD = PWD_;
        }
        public ODBC(string DNS_, string USER_, string PWD_)
        {
            this.DNS = DNS_;
            this.USER = USER_;
            this.PWD = PWD_;
        }
    }
}
