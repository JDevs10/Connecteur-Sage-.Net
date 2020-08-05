using Dapper;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Manager
{
    public class ConnexionManager
    {
        #region Entities
        public string ODBC = "ODBC";
        public string SQL = "SQL";
        #endregion
        #region Table entries
        private string TABLE_NAME = "Connexion";
        private string COLONNE_ID = "id";
        private string COLONNE_TYPE = "type";
        private string COLONNE_DNS = "dns";
        private string COLONNE_NAME = "name";
        private string COLONNE_PASSWORD = "password";
        #endregion

        public ConnexionManager() { }

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
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_TYPE + "' TEXT NOT NULL, '" + COLONNE_DNS + "' TEXT NOT NULL,  '" + COLONNE_NAME + "' TEXT,  '" + COLONNE_PASSWORD + "' TEXT, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))", conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("Table created / Exist");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connexion createTable [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
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
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '"+ COLONNE_TYPE + "', '" + COLONNE_DNS + "', '" + COLONNE_NAME + "', '" + COLONNE_PASSWORD + "') VALUES(" + connexion.id + ", '"+connexion.type+"','" + connexion.dns + "','" + connexion.name + "','" + connexion.password + "')", conn);
                        x = command.ExecuteNonQuery();
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nConnexion Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get List
        public List<Connexion> getList(string connectionString)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    List<Connexion> list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        var xxx = conn.Query<Connexion>("SELECT " + COLONNE_ID + ", "+ COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME);
                        list = xxx.ToList();
                        Console.WriteLine("Connexion list size : " + list.Count);
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Connexion List [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public Connexion getById(string connectionString, int id)
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
                            var xxx = conn.Query<Connexion>("SELECT " + COLONNE_ID + ",  "+ COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id);
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            Console.WriteLine("Connexion list size : 1");
                        }
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Connexion [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by TYPE
        public Connexion getByType(string connectionString, string type)
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
                            var xxx = conn.Query<Connexion>("SELECT " + COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_TYPE + " = '" + type + "'");

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
                    Console.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | ##################################################");
                    Console.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | [ERROR] getByType");
                    Console.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Message : " + ex.Message);
                    Console.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | StackTrace : " + ex.StackTrace);
                    Console.WriteLine("");
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
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_DNS + " = '" + connexion.dns + "', " + COLONNE_NAME + " = '" + connexion.name + "', " + COLONNE_PASSWORD + " = '" + connexion.password + "' WHERE " + COLONNE_ID + " = " + connexion.id, conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("id : " + connexion.id + " is Updated !");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Connexion Update [ERROR]");
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
                    Console.WriteLine("\nDELETE Connexion List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }

        }

        // Delete by ID
        public bool deleteById(string connectionString, Connexion connexion)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + connexion.id, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(connexion.id + " data have been deleted from " + TABLE_NAME + " !");
                            conn.Close();
                            return true;
                        }
                    }

                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDELETE Connexion [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }

        }

        public bool deleteById(string connectionString, int id)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(id + " data have been deleted from " + TABLE_NAME + " !");
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => createTable() | SQL => CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_TYPE + "' TEXT NOT NULL,  '" + COLONNE_DNS + "' TEXT NOT NULL,  '" + COLONNE_NAME + "' TEXT,  '" + COLONNE_PASSWORD + "' TEXT, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))");
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER, '" + COLONNE_TYPE + "' TEXT NOT NULL, '" + COLONNE_DNS + "' TEXT NOT NULL,  '" + COLONNE_NAME + "' TEXT,  '" + COLONNE_PASSWORD + "' TEXT, PRIMARY KEY('" + COLONNE_ID + "' AUTOINCREMENT))", conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("Table created");
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
                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '"+ COLONNE_TYPE + "', '" + COLONNE_DNS + "', '" + COLONNE_NAME + "', '" + COLONNE_PASSWORD + "') VALUES(" + connexion.id + ",'" + connexion.dns + "','" + connexion.name + "','" + connexion.password + "')", conn);
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

        // Get List
        public List<Reprocess> getList(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    List<Reprocess> list = null;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | Creation d'une instance");
                        var xxx = conn.Query<Reprocess>("SELECT " + COLONNE_ID + ", "+ COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME);
                        list = xxx.ToList();

                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | Creation d'une instance");
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | [ERROR] getList");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getList() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public Connexion getById(string connectionString, int id, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Connexion list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById(id: " + id + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById(id: " + id + ") | SQL => SELECT " + COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id);
                            var xxx = conn.Query<Connexion>("SELECT " + COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id);

                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Connexion obj recu");
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

        // Get by TYPE
        public Connexion getByType(string connectionString, string type, StreamWriter writer)
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
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById(type: " + type + ") | SQL => SELECT " + COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_TYPE + " = '" + type + "'");
                            var xxx = conn.Query<Connexion>("SELECT " + COLONNE_ID + ", " + COLONNE_TYPE + ", " + COLONNE_DNS + ", " + COLONNE_NAME + ", " + COLONNE_PASSWORD + " FROM " + TABLE_NAME + " WHERE " + COLONNE_TYPE + " = '" + type +"'");

                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | Connexion obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => getById() | [ERROR] getByType");
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
                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => update() | SQL => UPDATE " + TABLE_NAME + " SET " + COLONNE_DNS + " = '" + connexion.dns + "', " + COLONNE_NAME + " = '" + connexion.name + "', " + COLONNE_PASSWORD + " = '" + connexion.password + "' WHERE " + COLONNE_ID + " = " + connexion.id);
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_DNS + " = '" + connexion.dns + "', " + COLONNE_NAME + " = '" + connexion.name + "', " + COLONNE_PASSWORD + " = '" + connexion.password + "' WHERE " + COLONNE_ID + " = " + connexion.id, conn);
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
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteAll() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME, conn);
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

        // Delete by ID
        public bool deleteById(string connectionString, Connexion connexion, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById(reprocess .ediFileID: " + connexion.id + ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + connexion.id, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | [ERROR] deleteById (obj)");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        public bool deleteById(string connectionString, int id, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById(id: " + id + ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = " + id, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | [ERROR] deleteById (int)");
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ConnexionManager.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }


    }
}
