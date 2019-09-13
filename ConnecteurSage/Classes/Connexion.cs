using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.Common;
using ConnecteurSage.Helpers;
using System.IO;
using System.Xml.Serialization;

namespace ConnecteurSage.Classes
{
    class Connexion
    {
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static OdbcConnection CreateOdbcConnextion() 
        {
            DbConnectionStringBuilder connectionString = new DbConnectionStringBuilder();
            connectionString.Add("Dsn", "");
            connectionString.Add("Driver", "{SAGE Gestion commerciale 100}");


            string filename= pathModule+@"\Setting.xml";
            if (File.Exists(filename))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(filename);
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);
                connectionString.Add("Dsn", setting.DNS);
                connectionString.Add("uid", setting.Nom);
                connectionString.Add("pwd", Utils.Decrypt(setting.Password));
                file.Close();
            }  
            
            return new OdbcConnection(connectionString.ConnectionString);

            
        }

    }
}

