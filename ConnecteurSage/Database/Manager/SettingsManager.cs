using Dapper;
using Database.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Manager
{
    public class SettingsManager
    {
        #region Table entries
        private static string TABLE_NAME = "Settings";
        private static string COLONNE_ID = "id";
        private static string COLONNE_showWindow = "showWindow";
        private static string COLONNE_isACP_ComptaCPT_CompteG = "isACP_ComptaCPT_CompteG";
        private static string COLONNE_ACP_ComptaCPT_CompteG = "ACP_ComptaCPT_CompteG";
        // paths settings
        private static string COLONNE_EXE_Folder = "EXE_Folder";
        private static string COLONNE_EDI_Folder = "EDI_Folder";
        // plannerTask settings
        private static string COLONNE_plannerTask_name = "plannerTask_name";
        private static string COLONNE_plannerTask_UserId = "plannerTask_UserId";
        private static string COLONNE_plannerTask_active = "plannerTask_active";
        //private settings
        public static string COLONNE_priceType_active = "priceType_active";
        private static string COLONNE_priceType_cmdEDIPrice = "priceType_cmdEDIPrice";
        private static string COLONNE_priceType_productPrice = "priceType_productPrice";
        private static string COLONNE_priceType_categoryPrice = "priceType_categoryPrice";
        private static string COLONNE_priceType_clientPrice = "priceType_clientPrice";
        // reprocess settings
        private static string COLONNE_reprocess_active = "reprocess_active";
        private static string COLONNE_reprocess_hour = "reprocess_hour";
        private static string COLONNE_reprocess_countDown = "reprocess_countDown";
        #endregion

        #region SQLs
        public string SQL_create = @"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER UNIQUE, '" + COLONNE_showWindow + "' INTEGER NOT NULL, '" + COLONNE_isACP_ComptaCPT_CompteG + "' INTEGER NOT NULL, '" + COLONNE_ACP_ComptaCPT_CompteG + "' INTEGER NOT NULL, '"+ COLONNE_EXE_Folder + "' TEXT NOT NULL, '"+ COLONNE_EDI_Folder + "' TEXT NOT NULL, '"+ COLONNE_plannerTask_name + "' TEXT NOT NULL, '" + COLONNE_plannerTask_UserId + "' TEXT NOT NULL, '" + COLONNE_plannerTask_active + "' INTEGER NOT NULL, " +
            "'" + COLONNE_priceType_active + "' INTEGER NOT NULL, '" + COLONNE_priceType_cmdEDIPrice + "' INTEGER NOT NULL, '" + COLONNE_priceType_productPrice + "' INTEGER NOT NULL, '" + COLONNE_priceType_categoryPrice + "' INTEGER NOT NULL, '" + COLONNE_priceType_clientPrice + "' INTEGER NOT NULL, " +
            "'" + COLONNE_reprocess_active + "' INTEGER NOT NULL, '" + COLONNE_reprocess_hour + "' DECIMAL NOT NULL, '" + COLONNE_reprocess_countDown + "' INTEGER NOT NULL)";

        private string SQL_get = @"SELECT "+ COLONNE_ID + ", " + COLONNE_showWindow + ", " + COLONNE_isACP_ComptaCPT_CompteG + ", " + COLONNE_ACP_ComptaCPT_CompteG + ", " +
            "" + COLONNE_EXE_Folder + ", "+ COLONNE_EDI_Folder + ", "+ COLONNE_plannerTask_name + ", " + COLONNE_plannerTask_UserId + ", " + COLONNE_plannerTask_active + ", " +
            "" + COLONNE_priceType_active + ", " + COLONNE_priceType_cmdEDIPrice + ", " + COLONNE_priceType_productPrice + ", " + COLONNE_priceType_categoryPrice + ", " + COLONNE_priceType_clientPrice + ", " +
            "" + COLONNE_reprocess_active + ", " + COLONNE_reprocess_hour + ", " + COLONNE_reprocess_countDown + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = ";

        private string SQL_delete = @"DELETE TABLE "+TABLE_NAME;

        private string SQL_drop = @"DROP TABLE "+TABLE_NAME;
        #endregion


        public SettingsManager() { }

        // Create Reprocess Table
        public int createTable(string connectionString)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(SQL_create, conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created / Exist");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Insert
        public int insert(string connectionString, Settings settings)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_showWindow + "', '" + COLONNE_isACP_ComptaCPT_CompteG + "', '" + COLONNE_ACP_ComptaCPT_CompteG + "', '" + COLONNE_EXE_Folder + "', '" + COLONNE_EDI_Folder + "', '" + COLONNE_plannerTask_name + "', '" + COLONNE_plannerTask_UserId + "', '" + COLONNE_plannerTask_active + "', " +
                        "'" + COLONNE_priceType_active + "', '" + COLONNE_priceType_cmdEDIPrice + "', '" + COLONNE_priceType_productPrice + "', '" + COLONNE_priceType_categoryPrice + "', '" + COLONNE_priceType_clientPrice + "', " +
                        "'" + COLONNE_reprocess_active + "', '" + COLONNE_reprocess_hour + "', '" + COLONNE_reprocess_countDown + "') " +
                        "VALUES (1, " + settings.showWindow + ", " + settings.isACP_ComptaCPT_CompteG + ", " + settings.ACP_ComptaCPT_CompteG + ", '" + settings.EXE_Folder + "', '" + settings.EDI_Folder + "', '" + settings.plannerTask_name + "', '" + settings.plannerTask_UserId + "', " + settings.plannerTask_active + ", " +
                        "" + settings.priceType_active + ", " + settings.priceType_cmdEDIPrice + ", " + settings.priceType_productPrice + ", " + settings.priceType_categoryPrice + ", " + settings.priceType_clientPrice + ", " +
                        "" + settings.reprocess_active + ", " + settings.reprocess_hour.ToString().Replace(',', '.') + ", " + settings.reprocess_countDown + ")";
                    /*
                    string SQL_insert__ = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_showWindow + "', '" + COLONNE_isACP_ComptaCPT_CompteG + "', '" + COLONNE_ACP_ComptaCPT_CompteG + "', '" + COLONNE_EXE_Folder + "', '" + COLONNE_EDI_Folder + "', '" + COLONNE_plannerTask_name + "', '" + COLONNE_plannerTask_UserId + "', '" + COLONNE_plannerTask_active + "','" + COLONNE_priceType_active + "', '" + COLONNE_priceType_cmdEDIPrice + "', '" + COLONNE_priceType_productPrice + "', '" + COLONNE_priceType_categoryPrice + "', '" + COLONNE_priceType_clientPrice + "','" + COLONNE_reprocess_active + "', '" + COLONNE_reprocess_hour + "', '" + COLONNE_reprocess_countDown + "') VALUES " +
                        "(1," + settings.showWindow + ", " + settings.isACP_ComptaCPT_CompteG + ", " + settings.ACP_ComptaCPT_CompteG + ", '" + settings.EXE_Folder + "', '" + settings.EDI_Folder + "', '" + settings.plannerTask_name + "', '" + settings.plannerTask_UserId + "', " + settings.plannerTask_active + ", " +
                        "" + settings.priceType_active + ", " + settings.priceType_cmdEDIPrice + ", " + settings.priceType_productPrice + ", " + settings.priceType_categoryPrice + ", " + settings.priceType_clientPrice + ", " +
                        "" + settings.reprocess_active + ", " + settings.reprocess_hour.ToString().Replace(',', '.') + ", " + settings.reprocess_countDown + ")";
                    */

                    /*
                    INSERT INTO Settings ('id', 'showWindow', 'isACP_ComptaCPT_CompteG', 'ACP_ComptaCPT_CompteG', 'EXE_Folder', 'EDI_Folder', 'plannerTask_name', 'plannerTask_UserId', 'plannerTask_active',
                    'priceType_active', 'priceType_cmdEDIPrice', 'priceType_productPrice', 'priceType_categoryPrice', 'priceType_clientPrice',
                    'reprocess_active', 'reprocess_hour', 'reprocess_countDown')
                    VALUES(1, 5, 1, 12347, 'Path EXE Folder', 'Path EDI Folder', '', '', 0,
                    1, 0, 0, 0, 1,
                    1, 0.5, 3)
                    */


                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(SQL_insert, conn);
                        x = command.ExecuteNonQuery();
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nSettings Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get by ID
        public Settings get(string connectionString, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Settings list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<Settings>(SQL_get+""+id);
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                        }
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GET Settings [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Settings settings)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        "" + COLONNE_showWindow + " = " + settings.showWindow + ", " +
                        "" + COLONNE_isACP_ComptaCPT_CompteG + " = " + settings.isACP_ComptaCPT_CompteG + ", " +
                        "" + COLONNE_ACP_ComptaCPT_CompteG + " = " + settings.ACP_ComptaCPT_CompteG + ", " +
                        "" + COLONNE_EXE_Folder + " = '" + settings.EXE_Folder + "', " +
                        "" + COLONNE_EDI_Folder + " = '" + settings.EDI_Folder + "', " +
                        "" + COLONNE_plannerTask_name + " = '" + settings.plannerTask_name + "', " +
                        "" + COLONNE_plannerTask_UserId + " = '" + settings.plannerTask_UserId + "', " +
                        "" + COLONNE_plannerTask_active + " = " + settings.plannerTask_active + ", " +
                        "" + COLONNE_priceType_active + " = " + settings.priceType_active + ", " +
                        "" + COLONNE_priceType_cmdEDIPrice + " = " + settings.priceType_cmdEDIPrice + ", " +
                        "" + COLONNE_priceType_productPrice + " = " + settings.priceType_productPrice + ", " +
                        "" + COLONNE_priceType_categoryPrice + " = " + settings.priceType_categoryPrice + ", " +
                        "" + COLONNE_priceType_clientPrice + " = " + settings.priceType_clientPrice + ", " +
                        "" + COLONNE_reprocess_active + " = " + settings.reprocess_active + ", " +
                        "" + COLONNE_reprocess_hour + " = " + settings.reprocess_hour + ", " +
                        "" + COLONNE_reprocess_countDown + " = " + settings.reprocess_countDown + " WHERE " + COLONNE_ID + " = "+ settings.id;

                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(SQL_put, conn);
                        x = command.ExecuteNonQuery();
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Settings Update [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Delete ALL
        public bool deleteAll(string connectionString)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            SQLiteCommand command = new SQLiteCommand(SQL_delete, conn);
                            command.ExecuteNonQuery();
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDELETE Reproccess List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }

        // Delete by ID
        public bool drop(string connectionString, Reprocess reprocess)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            SQLiteCommand command = new SQLiteCommand(SQL_drop, conn);
                            command.ExecuteNonQuery();
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDELETE Reproccess [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }


        //######################################################################################################################################################
        //##### WITH LOGS

        public int createTable(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | SQL => " + SQL_create);
                        SQLiteCommand command = new SQLiteCommand(SQL_create, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | Table created");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | ##### [ERROR] INSERT");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => createTable() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Insert
        public int insert(string connectionString, Settings settings, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_showWindow + "', '" + COLONNE_isACP_ComptaCPT_CompteG + "', '" + COLONNE_ACP_ComptaCPT_CompteG + "', '" + COLONNE_EXE_Folder + "', '" + COLONNE_EDI_Folder + "', '" + COLONNE_plannerTask_name + "', '" + COLONNE_plannerTask_UserId + "', '" + COLONNE_plannerTask_active + "', " +
                        "'" + COLONNE_priceType_active + "', '" + COLONNE_priceType_cmdEDIPrice + "', '" + COLONNE_priceType_productPrice + "', '" + COLONNE_priceType_categoryPrice + "', '" + COLONNE_priceType_clientPrice + "', " +
                        "'" + COLONNE_reprocess_active + "', '" + COLONNE_reprocess_hour + "', '" + COLONNE_reprocess_countDown + "') " +
                        "VALUES (1, " + settings.showWindow + ", " + settings.isACP_ComptaCPT_CompteG + ", " + settings.ACP_ComptaCPT_CompteG + ", '" + settings.EXE_Folder + "', '" + settings.EDI_Folder + "', '" + settings.plannerTask_name + "', '" + settings.plannerTask_UserId + "', " + settings.plannerTask_active + ", " +
                        "" + settings.priceType_active + ", " + settings.priceType_cmdEDIPrice + ", " + settings.priceType_productPrice + ", " + settings.priceType_categoryPrice + ", " + settings.priceType_clientPrice + ", " +
                        "" + settings.reprocess_active + ", " + settings.reprocess_hour + ", " + settings.reprocess_countDown + ")";

                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(SQL_insert, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | Creation d'une instance");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | Reproccess Insert [ERROR]");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => insert() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get by ID
        public Settings get(string connectionString, int id, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Settings list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById(id: " + id + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById(di: " + id + ") | SQL => "+SQL_get);
                            var xxx = conn.Query<Settings>(SQL_get);
                            
                            if(xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | Settings obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | [ERROR] getById");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => getById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Settings settings, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        "" + COLONNE_showWindow + " = " + settings.showWindow + ", " +
                        "" + COLONNE_isACP_ComptaCPT_CompteG + " = " + settings.isACP_ComptaCPT_CompteG + ", " +
                        "" + COLONNE_ACP_ComptaCPT_CompteG + " = " + settings.ACP_ComptaCPT_CompteG + ", " +
                        "" + COLONNE_EXE_Folder + " = " + settings.EXE_Folder + ", " +
                        "" + COLONNE_EDI_Folder + " = " + settings.EDI_Folder + ", " +
                        "" + COLONNE_plannerTask_name + " = " + settings.plannerTask_name + ", " +
                        "" + COLONNE_plannerTask_UserId + " = '" + settings.plannerTask_UserId + "', " +
                        "" + COLONNE_plannerTask_active + " = " + settings.plannerTask_active + ", " +
                        "" + COLONNE_priceType_active + " = " + settings.priceType_active + ", " +
                        "" + COLONNE_priceType_cmdEDIPrice + " = " + settings.priceType_cmdEDIPrice + ", " +
                        "" + COLONNE_priceType_productPrice + " = " + settings.priceType_productPrice + ", " +
                        "" + COLONNE_priceType_categoryPrice + " = " + settings.priceType_categoryPrice + ", " +
                        "" + COLONNE_priceType_clientPrice + " = " + settings.priceType_clientPrice + ", " +
                        "" + COLONNE_reprocess_active + " = " + settings.reprocess_active + ", " +
                        "" + COLONNE_reprocess_hour + " = " + settings.reprocess_hour + ", " +
                        "" + COLONNE_reprocess_countDown + " = " + settings.reprocess_countDown + " WHERE " + COLONNE_ID + " = " + settings.id;

                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | SQL => "+ SQL_put);
                        SQLiteCommand command = new SQLiteCommand(SQL_put, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | id : " + settings.id + " is Updated !");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | [ERROR] update");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => update() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Delete ALL
        public bool deleteAll(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | SQL => "+SQL_delete);
                            SQLiteCommand command = new SQLiteCommand(SQL_delete, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
                            writer.Flush(); 
                            conn.Close();
                            return true;
                        }
                    }

                    writer.Flush();
                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | [ERROR] delete all");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => deleteAll() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        // drop
        public bool drop(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | SQL => "+SQL_drop);
                            SQLiteCommand command = new SQLiteCommand(SQL_drop, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | Table "+ SQL_drop + " dropped");
                            writer.Flush(); 
                            conn.Close();
                            return true;
                        }
                    }
                    writer.Flush();
                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | [ERROR] drop ()");
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: SettingsManager.dll => drop() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

    }
}
