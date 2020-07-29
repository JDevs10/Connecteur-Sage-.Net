using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Database
    {
        #region Databse variables
        public static int DB_VERSION = 3;
        public static string DB_NAME = "Database.db";
        public static string DB_DOSSIER = "db";
        private static string directory_db = Directory.GetCurrentDirectory() + @"\" + DB_DOSSIER;
        private static string pathModule = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public string connectionString = "Data Source=" + directory_db + @"\" + DB_NAME + "; Version=" + DB_VERSION;
        SQLiteConnection conn { get; set; }
        #endregion

        #region Tables

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

                this.conn = new SQLiteConnection(connectionString);
            }
            else
            {
                Console.WriteLine("");
            }
        }

        public void initTables()
        {
            
        }



    }
}
