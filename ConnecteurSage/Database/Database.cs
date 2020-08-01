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
        private static string directory_db = Directory.GetCurrentDirectory() + @"\" + DB_DOSSIER;
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public string connectionString = "Data Source=" + directory_db + @"\" + DB_NAME + "; Version=" + DB_VERSION;
        //SQLiteConnection conn { get; set; }
        #endregion

        #region Tables Management
        public ReprocessManager reprocessManager { get; set; }
        #endregion


        public Database()
        {
            if (!Directory.Exists(directory_db))
            {
                Directory.CreateDirectory(directory_db);
            }
            if (!File.Exists(directory_db + @"\" + DB_NAME))
            {
                SQLiteConnection.CreateFile(directory_db + @"\" + DB_NAME);

                //this.conn = new SQLiteConnection(connectionString);
            }/*
            else
            {
                this.conn = new SQLiteConnection(connectionString);
                Console.WriteLine("Databse exist so establish sqliteConnection object.");
            }
            */

            reprocessManager = new ReprocessManager();
        }

        public void initTables()
        {
            //#####################################################################
            //##### Create all tables #############################################
            
            // Reprocess Table
            reprocessManager.createTable(connectionString);

            // Save a backup of the db in ./Backup/Database_backup.db
            saveBackup();

            /*
            reprocessManager.insert(connectionString, new Model.Reprocess(1, "ReprocessTest", "FilePathTest", 10));
            reprocessManager.insert(connectionString, new Model.Reprocess(101,"ReprocessTest 11", "FilePathTest 1", 2));
            reprocessManager.insert(connectionString, new Model.Reprocess(102, "ReprocessTest 22", "FilePathTest 2", 1));
            reprocessManager.insert(connectionString, new Model.Reprocess(103, "ReprocessTest 33", "FilePathTest 3", 5));


            reprocessManager.update(connectionString, new Model.Reprocess(1, "ReprocessTest", "FilePathTest", 10000));


            Console.WriteLine("Nub of rows 1st");
            List<Model.Reprocess> testList = reprocessManager.getList(connectionString);
            Console.WriteLine("Nub of rows : " + testList.Count);

            reprocessManager.deleteById(connectionString, 103);

            
            Console.WriteLine("Nub of rows 2nd");
            List<Model.Reprocess> testList_ = reprocessManager.getList(connectionString);
            Console.WriteLine("Nub of rows : " + testList_.Count);
            

            reprocessManager.getById(connectionString, 1);
            */

            Console.WriteLine("Done");
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
    }
}
