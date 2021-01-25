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
    public class ConnexionManager
    {
        #region Table entries
        private static string TABLE_NAME = "Settings";
        private static string COLONNE_ID = "id";
        private static string COLONNE_TYPE = "TYPE";
        private static string COLONNE_DNS = "DNS";
        private static string COLONNE_USER = "USER";
        private static string COLONNE_PASSWORD = "PASSWORD";
        #endregion

        #region SQLs
        public string SQL_create = @"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER UNIQUE, '" + COLONNE_TYPE + "' TEXT NOT NULL, '" + COLONNE_DNS + "' TEXT NOT NULL, '" + COLONNE_USER + "' TEXT NOT NULL, '"+ COLONNE_PASSWORD + "' TEXT NOT NULL)";

        private string SQL_get = @"SELECT "+ COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_USER + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_TYPE + " = ";

        private string SQL_delete = @"DELETE TABLE "+TABLE_NAME;

        private string SQL_drop = @"DROP TABLE "+TABLE_NAME;
        #endregion


        public ConnexionManager() { }

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
        public int insert(string connectionString, Connexion connexion)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_TYPE + "', '" + COLONNE_DNS + "', '" + COLONNE_USER + "', '" + COLONNE_PASSWORD + "'') " +
                        "VALUES (1, " + connexion.id + ", " + connexion.type + ", " + connexion.dns + ", '" + connexion.user + "', '" + Utilities.Utils.Encrypt(connexion.pwd) + "')";


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
        public Connexion get_by_type(string connectionString, string type)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Connexion list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<Connexion>(SQL_get+"'"+ type+"'");
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            xxx.ToList()[0].pwd = Utilities.Utils.Decrypt(xxx.ToList()[0].pwd);
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
        public int update(string connectionString, Connexion connexion)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        "" + COLONNE_DNS + " = '" + connexion.dns + "', " +
                        "" + COLONNE_USER + " = '" + connexion.user + "', " +
                        "" + COLONNE_PASSWORD + " = '" + Utilities.Utils.Encrypt(connexion.pwd) + "' WHERE " + COLONNE_ID + " = "+ connexion.id;

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

        // Drope
        public bool drop(string connectionString)
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | SQL => " + SQL_create);
                        SQLiteCommand command = new SQLiteCommand(SQL_create, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | Table created");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | ##### [ERROR] INSERT");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Insert
        public int insert(string connectionString, Connexion connexion, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_TYPE + "', '" + COLONNE_DNS + "', '" + COLONNE_USER + "', '" + COLONNE_PASSWORD + "'') " +
                        "VALUES (1, " + connexion.id + ", " + connexion.type + ", " + connexion.dns + ", '" + connexion.user + "', '" + Utilities.Utils.Encrypt(connexion.pwd) + "')";

                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(SQL_insert, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Creation d'une instance");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Reproccess Insert [ERROR]");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get by ID
        public Connexion get_by_type(string connectionString, string type, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Connexion list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById(type: " + type + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById(type: " + type + ") | SQL => "+SQL_get);
                            var xxx = conn.Query<Connexion>(SQL_get + "'" + type + "'");
                            
                            if(xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            xxx.ToList()[0].pwd = Utilities.Utils.Decrypt(xxx.ToList()[0].pwd);
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Settings obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | [ERROR] getById");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Connexion connexion, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        "" + COLONNE_DNS + " = '" + connexion.dns + "', " +
                        "" + COLONNE_USER + " = '" + connexion.user + "', " +
                        "" + COLONNE_PASSWORD + " = '" + Utilities.Utils.Encrypt(connexion.pwd) + "' WHERE " + COLONNE_ID + " = " + connexion.id;

                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | SQL => "+ SQL_put);
                        SQLiteCommand command = new SQLiteCommand(SQL_put, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | id : " + connexion.id + " is Updated !");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | [ERROR] update");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | SQL => "+SQL_delete);
                            SQLiteCommand command = new SQLiteCommand(SQL_delete, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | [ERROR] delete all");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | SQL => "+SQL_drop);
                            SQLiteCommand command = new SQLiteCommand(SQL_drop, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | Table "+ SQL_drop + " dropped");
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | [ERROR] drop ()");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => drop() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

    }
}
