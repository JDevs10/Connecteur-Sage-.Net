using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Manager;
using System.ComponentModel;

namespace Database
{
    public class Database
    {
        #region Databse variables
        public static int DB_VERSION = 3;
        public static string DB_NAME = "Database.db";
        public static string DB_NAME_BACKUP = "Database_backup.db";
        public static string DB_DOSSIER = "db";
        private static string directory_db = "";
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public string connectionString = "Data Source=" + directory_db + @"\" + DB_NAME + "; Version=" + DB_VERSION;
        #endregion

        #region Tables Management
        public ReprocessManager reprocessManager { get; set; }
        public AlertMailLogManager alertMailLogManager { get; set; }
        public SettingsManager settingsManager { get; set; }
        public ConnexionManager connexionManager { get; set; }
        public EmailManager emailManager { get; set; }
        public ReliquatManager reliquatManager { get; set; }
        #endregion


        public Database()
        {
            Init.Init init = new Init.Init();
            if (init.isSettings())
            {
                init.Load();
                directory_db = init.connecteurInfo.installation_dir + @"\" + DB_DOSSIER;
                connectionString = "Data Source=" + directory_db + @"\" + DB_NAME + "; Version=" + DB_VERSION;
            }

            if (!Directory.Exists(directory_db))
            {
                Directory.CreateDirectory(directory_db);
            }
            if (!File.Exists(directory_db + @"\" + DB_NAME))
            {
                SQLiteConnection.CreateFile(directory_db + @"\" + DB_NAME);
            }
            this.reprocessManager = new ReprocessManager();
            this.alertMailLogManager = new AlertMailLogManager();
            this.settingsManager = new SettingsManager();
            this.connexionManager = new ConnexionManager();
            this.emailManager = new EmailManager();
            this.reliquatManager = new ReliquatManager();
        }

        public Database(StreamWriter writer)
        {
            Init.Init init = new Init.Init();
            if (init.isSettings())
            {
                init.Load_w_logs(writer);
                directory_db = init.connecteurInfo.installation_dir + @"\" + DB_DOSSIER;
                connectionString = "Data Source=" + directory_db + @"\" + DB_NAME + "; Version=" + DB_VERSION;
            }
            else
            {
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | Creation d'une instance");
                writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | ");
                writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | Creation d'une instance");
            }

            writer.WriteLine("");
            writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | Creation d'une instance");
            if (!Directory.Exists(directory_db))
            {
                Directory.CreateDirectory(directory_db);
                writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | Creation du repertoire => " + directory_db);
            }
            if (!File.Exists(directory_db + @"\" + DB_NAME))
            {
                SQLiteConnection.CreateFile(directory_db + @"\" + DB_NAME);
                writer.WriteLine(DateTime.Now + " :: Database.dll => Database() | Creation de la base de donnee => " + directory_db + @"\" + DB_NAME);
            }
            this.reprocessManager = new ReprocessManager();
            this.alertMailLogManager = new AlertMailLogManager();
            this.settingsManager = new SettingsManager();
            this.connexionManager = new ConnexionManager();
            this.emailManager = new EmailManager();
            this.reliquatManager = new ReliquatManager();

            writer.WriteLine("");
            writer.Flush();
        }

        public void initTables()
        {
            //#####################################################################
            //##### Create all tables #############################################

            // Reprocess Table
            this.reprocessManager.createTable(connectionString);
            this.alertMailLogManager.createTable(connectionString);
            this.settingsManager.createTable(connectionString);
            this.connexionManager.createTable(connectionString);
            this.emailManager.createTable(connectionString);
            this.reliquatManager.createTable(connectionString, null);

            // Save a backup of the db in ./Backup/Database_backup.db
            saveBackup();
            
        }

        public void initTables(StreamWriter writer)
        {
            writer.WriteLine(DateTime.Now + " :: Database.dll => initTables() | Creation d'une instance");
            //#####################################################################
            //##### Create all tables #############################################

            // Reprocess Table
            this.reprocessManager.createTable(connectionString, writer);
            this.alertMailLogManager.createTable(connectionString, writer);
            this.settingsManager.createTable(connectionString, writer);
            this.connexionManager.createTable(connectionString, writer);
            this.emailManager.createTable(connectionString, writer);
            this.reliquatManager.createTable(connectionString, writer);

            // Save a backup of the db in ./Backup/Database_backup.db
            saveBackup(writer);
            writer.WriteLine("");
            writer.WriteLine("");
        }


        public void saveBackup()
        {
            try
            {
                if (File.Exists(directory_db + @"\" + DB_NAME))
                {
                    if (!Directory.Exists(directory_db + @"\Backup\"))
                    {
                        Directory.CreateDirectory(directory_db + @"\Backup\");
                    }
                    File.Copy(directory_db + @"\" + DB_NAME, directory_db + @"\Backup\" + DB_NAME_BACKUP, true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n##### [ERROR] Database Backup");
                Console.WriteLine("Message : " + ex.Message);
                Console.WriteLine("StackTrace : " + ex.StackTrace);
            }
        }

        public void saveBackup(StreamWriter writer)
        {
            try
            {
                writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | Creation d'une instance");

                if (File.Exists(directory_db + @"\" + DB_NAME))
                {
                    writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | Le repertoire existe => " + directory_db + @"\" + DB_NAME);
                    if (!Directory.Exists(directory_db + @"\Backup\"))
                    {
                        Directory.CreateDirectory(directory_db + @"\Backup\");
                        writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | Le repertoire n'existe pas alors creer le => " + directory_db + @"\Backup\");
                    }
                    File.Copy(directory_db + @"\" + DB_NAME, directory_db + @"\Backup\" + DB_NAME_BACKUP, true);
                    writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | Copie la BDD depuis \"" + directory_db + @"\" + DB_NAME + "\" a \"" + directory_db + @"\Backup\" + DB_NAME_BACKUP + "\"");
                }
                writer.Flush();
            }
            catch (Exception ex)
            {
                writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | ##################################################");
                writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | ##### [ERROR] Database Backup ####################");
                writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | Message : " + ex.Message);
                writer.WriteLine(DateTime.Now + " :: Database.dll => saveBackup() | StackTrace : " + ex.StackTrace);
                writer.Flush();
            }
        }

        public string JsonFormat(Object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }
        public string JsonFormat(Array array)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(array, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
