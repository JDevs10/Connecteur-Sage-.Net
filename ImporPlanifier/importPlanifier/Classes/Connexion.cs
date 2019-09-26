using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.Common;
using importPlanifier.Helpers;
using System.IO;
using System.Xml.Serialization;

namespace importPlanifier.Classes
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
                connectionString.Add("Dsn", setting.DNS_1);
                connectionString.Add("uid", setting.Nom_2);
                connectionString.Add("pwd", Utils.Decrypt(setting.Password_1));
                file.Close();
            }  
            
            return new OdbcConnection(connectionString.ConnectionString);  
        }

        public static OdbcConnection CreateOdbcConnexionSQL()
        {
            DbConnectionStringBuilder connectionString = new DbConnectionStringBuilder();
            connectionString.Add("Dsn", "");
            connectionString.Add("Driver", "{SAGE Gestion commerciale 100}");

            string filename = pathModule + @"\SettingSQL.xml";
            if (File.Exists(filename))
            {
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(filename);
                ConfigurationDNS setting = new ConfigurationDNS();
                setting = (ConfigurationDNS)reader.Deserialize(file);
                connectionString.Add("Dsn", setting.DNS_2);
                connectionString.Add("uid", setting.Nom_2);
                connectionString.Add("pwd", Utils.Decrypt(setting.Password_2));
                file.Close();
            }

            return new OdbcConnection(connectionString.ConnectionString);
        }

    }
}

