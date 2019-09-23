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
        private static string logDirectoryName = Directory.GetCurrentDirectory() + @"\" + "LOG_Connexion";
        private static StreamWriter logFileConnexionWriter = null;
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

        public static OdbcConnection CreateOdbcConnexionSQL()
        {
            //Check if the Log directory exists
            if (!Directory.Exists(logDirectoryName))
            {
                //Create log directory
                Directory.CreateDirectory(logDirectoryName);
            }

            //Create log file
            var logFileName = logDirectoryName + @"\" + string.Format("LOG_Connexion {0:dd-MM-yyyy HH.mm.ss}.txt", DateTime.Now);
            var logFile = File.Create(logFileName);

            DbConnectionStringBuilder connectionString = new DbConnectionStringBuilder();
            connectionString.Add("Dsn", "");
            connectionString.Add("Driver", "{SAGE Gestion commerciale 100}");

            string filename = pathModule+@"\Setting.xml";
            if(File.Exists(filename)){
                XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationDNS));
                StreamReader file = new System.IO.StreamReader(filename);
                ConfigurationDNS settings = new ConfigurationDNS();
                settings = (ConfigurationDNS)reader.Deserialize(file);

                if (settings.DNS.Equals(settings.DNS + "_SQL"))
                {
                    //Write in the log file 
                    logFileConnexionWriter = new StreamWriter(logFile);
                    logFileConnexionWriter.WriteLine("###################################################################################################");
                    logFileConnexionWriter.WriteLine("################################ ConnecteurSage Sage BDD Connexion ################################");
                    logFileConnexionWriter.WriteLine("###################################################################################################");
                    logFileConnexionWriter.WriteLine("");
                    // Connexion Log if DNS settings are changed
                    logFileConnexionWriter.WriteLine(DateTime.Now + " | insertStock() || Connexion => CreateOdbcConnexionSQL() : file location = " + filename);
                    logFileConnexionWriter.WriteLine(DateTime.Now + " | insertStock() || Connexion => CreateOdbcConnexionSQL() : DSN = " + settings.DNS);
                    logFileConnexionWriter.WriteLine(DateTime.Now + " | insertStock() || Connexion => CreateOdbcConnexionSQL() : UID = " + settings.Nom);
                    logFileConnexionWriter.WriteLine(DateTime.Now + " | insertStock() || Connexion => CreateOdbcConnexionSQL() : PWD = " + Utils.Decrypt(settings.Password));
                    logFileConnexionWriter.Close();
                    return null;
                }
                connectionString.Add("Dsn", settings.DNS + "_SQL");
                connectionString.Add("uid", settings.Nom);
                connectionString.Add("pwd", Utils.Decrypt(settings.Password));
                file.Close();
            }
            else
            {
                //Write in the log file 
                logFileConnexionWriter = new StreamWriter(logFile);
                logFileConnexionWriter.WriteLine("###################################################################################################");
                logFileConnexionWriter.WriteLine("################################ ConnecteurSage Sage BDD Connexion ################################");
                logFileConnexionWriter.WriteLine("###################################################################################################");
                logFileConnexionWriter.WriteLine("");
                // Connexion Log if DNS settings are changed
                logFileConnexionWriter.WriteLine(DateTime.Now + " | insertStock() || Connexion => CreateOdbcConnexionSQL() : Le fichier 'Setting.xml' n'existe pas dans le répertoire : " + pathModule);
                logFileConnexionWriter.Close();
            }

            return new OdbcConnection(connectionString.ConnectionString);
        }
    }
}
