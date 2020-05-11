using Connexion.Classes.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connexion.Classes
{
    public class ConfigurationConnexion
    {
        public ODBC ODBC;
        public SQL SQL;

        public ConfigurationConnexion() { }
        public ConfigurationConnexion(ODBC ODBC_, SQL SQL_)
        {
            this.ODBC = ODBC_;
            this.SQL = SQL_;
        }
    }
}
