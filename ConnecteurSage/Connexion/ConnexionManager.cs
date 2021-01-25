using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connexion
{
    public class ConnexionManager
    {
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static OdbcConnection CreateOdbcConnextion()
        {

            DbConnectionStringBuilder connectionString = new DbConnectionStringBuilder();
            connectionString.Add("Dsn", "");
            connectionString.Add("Driver", "{SAGE Gestion commerciale 100}");

            ConnexionSaveLoad settings = new ConnexionSaveLoad();

            if (settings.isSettings())
            {
                settings.Load();

                connectionString.Add("Dsn", settings.configurationConnexion.ODBC.DNS);
                connectionString.Add("uid", settings.configurationConnexion.ODBC.USER);
                connectionString.Add("pwd", settings.configurationConnexion.ODBC.PWD);

                //connectionString__ = @"Data Source=" + directory_db + ";uid="+ settings.configurationConnexion.ODBC.USER + ";pwd="+ settings.configurationConnexion.ODBC.PWD + ";";
            }

            return new OdbcConnection(connectionString.ConnectionString);
            //return new OdbcConnection(connectionString__);
        }

        public static OdbcConnection CreateOdbcConnexionSQL()
        {
            DbConnectionStringBuilder connectionString = new DbConnectionStringBuilder();
            connectionString.Add("Dsn", "");
            connectionString.Add("Driver", "{SAGE Gestion commerciale 100}");

            ConnexionSaveLoad settings = new ConnexionSaveLoad();

            if (settings.isSettings())
            {
                settings.Load();

                connectionString.Add("Dsn", settings.configurationConnexion.SQL.DNS);
                connectionString.Add("uid", settings.configurationConnexion.SQL.USER);
                connectionString.Add("pwd", settings.configurationConnexion.SQL.PWD);
            }

            return new OdbcConnection(connectionString.ConnectionString);
        }
    }
}
