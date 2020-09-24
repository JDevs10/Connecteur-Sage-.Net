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
    public class AlertMailLogManager
    {
        #region Table entries
        private string TABLE_NAME = "AlertMail_Log";
        private string COLONNE_ID = "id";
        private string COLONNE_LOG = "log";
        #endregion

        public AlertMailLogManager() { }

        // Create AlertMailLog Table
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
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_LOG + "' TEXT NOT NULL, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created / Exist");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AlertMailLog createTable [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }


        // Insert
        public int insert(string connectionString, AlertMailLog alertMailLog)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_LOG + "') VALUES(" + alertMailLog.id + ",'" + alertMailLog.log + "')", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AlertMailLog Insert 1 [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        public int insert(string connectionString, string logText)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        //Console.WriteLine("SQL => INSERT INTO " + TABLE_NAME + "('" + COLONNE_LOG + "') VALUES('" + logText + "')");
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_LOG + "') VALUES('" + logText + "')", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AlertMailLog Insert 2 [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get List
        public List<AlertMailLog> getList(string connectionString)
        {
            using(SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    List<AlertMailLog> list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        var xxx = conn.Query<AlertMailLog>("SELECT " + COLONNE_ID + ", " + COLONNE_LOG + " FROM " + TABLE_NAME);
                        list = xxx.ToList();

                        Console.WriteLine("AlertMailLog getList :: ");
                        Console.WriteLine("AlertMailLog list size : " + list.Count);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET AlertMailLog List [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public AlertMailLog getById(string connectionString, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    AlertMailLog list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<AlertMailLog>("SELECT " + COLONNE_ID + ", " + COLONNE_LOG + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id);
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            Console.WriteLine("AlertMailLog list size : 1");
                        }
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET AlertMailLog [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, AlertMailLog alertMailLog)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_LOG + " = '" + alertMailLog.log + "' WHERE " + COLONNE_ID + " = " + alertMailLog.id, conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("ediFileID : " + alertMailLog.id + " is Updated !");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n AlertMailLog Update [ERROR]");
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(TABLE_NAME + " data have been deleted!");
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DELETE alertMailLog List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }

        // Delete by ID
        public bool deleteById(string connectionString, AlertMailLog alertMailLog)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + alertMailLog.id, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(alertMailLog.id + " data have been deleted from " + TABLE_NAME + " !");
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DELETE AlertMailLog [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }

        public bool deleteById(string connectionString, int alertMailLogID)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + alertMailLogID, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(alertMailLogID + " data have been deleted from " + TABLE_NAME + " !");
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
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
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | SQL => CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_LOG + "' TEXT NOT NULL, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))");
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_LOG + "' TEXT NOT NULL, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | Table created");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | ##### [ERROR] INSERT");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => createTable() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }


        // Insert
        public int insert(string connectionString, AlertMailLog alertMailLog, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_LOG + "') VALUES(" + alertMailLog.id + ",'" + alertMailLog.log + "')", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | Creation d'une instance");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | Reproccess Insert [ERROR]");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => insert() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get List
        public List<AlertMailLog> getList(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    List<AlertMailLog> list = null;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | Creation d'une instance");
                        var xxx = conn.Query<AlertMailLog>("SELECT " + COLONNE_ID + ", " + COLONNE_LOG + " FROM " + TABLE_NAME);
                        list = xxx.ToList();

                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | Creation d'une instance");
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | [ERROR] getList");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getList() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public AlertMailLog getById(string connectionString, int alertMailLogID, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    AlertMailLog list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById(alertMailLogID: " + alertMailLogID + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById(alertMailLogID: " + alertMailLogID + ") | SQL => SELECT " + COLONNE_ID + ", " + COLONNE_LOG + " WHERE " + COLONNE_ID + " = " + alertMailLogID);
                            var xxx = conn.Query<AlertMailLog>("SELECT " + COLONNE_ID + ", " + COLONNE_LOG + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + alertMailLogID);
                            
                            if(xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | AlertMailLog obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | [ERROR] getById");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => getById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, AlertMailLog alertMailLog, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | SQL => UPDATE " + TABLE_NAME + " SET " + COLONNE_LOG + " = '" + alertMailLog.log + "' WHERE " + COLONNE_ID + " = " + alertMailLog.id);
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_LOG + " = '" + alertMailLog.log + "' WHERE " + COLONNE_ID + " = " + alertMailLog.id, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | id : " + alertMailLog.id + " is Updated !");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | [ERROR] update");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => update() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | [ERROR] delete all");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteAll() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        // Delete by ID
        public bool deleteById(string connectionString, AlertMailLog alertMailLog, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById(alertMailLog.id: " + alertMailLog.id+ ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + alertMailLog.id, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | [ERROR] deleteById (obj)");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        public bool deleteById(string connectionString, int alertMailLogID, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById(alertMailLogID: " + alertMailLogID + ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + alertMailLogID, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | [ERROR] deleteById (int)");
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: AlertMailLog.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

    }
}
