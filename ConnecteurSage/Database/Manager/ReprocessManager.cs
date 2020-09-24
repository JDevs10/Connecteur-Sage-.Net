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
    public class ReprocessManager
    {
        #region Table entries
        private string TABLE_NAME = "Reprocess";
        private string COLONNE_EDIFILEID = "ediFileID";
        private string COLONNE_FILENAME = "fileName";
        private string COLONNE_FILEPATH = "filePath";
        private string COLONNE_COUNT = "fileReprocessCount";
        #endregion

        //public List<Reprocess> reprocessList;

        public ReprocessManager() { }

        /*
        public ReprocessManager(Reprocess reprocess)
        {
            this.reprocess = reprocess;
        }
        */

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
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_EDIFILEID + "' INTEGER UNIQUE, '" + COLONNE_FILENAME + "' TEXT NOT NULL, '" + COLONNE_FILEPATH + "' TEXT NOT NULL, '" + COLONNE_COUNT + "' TEXT NOT NULL)", conn);
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
        public int insert(string connectionString, Reprocess reprocess)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_EDIFILEID + "', '" + COLONNE_FILENAME + "', '" + COLONNE_FILEPATH + "', '" + COLONNE_COUNT + "') VALUES(" + reprocess.ediFileID + ",'" + reprocess.fileName + "','" + reprocess.filePath + "','" + reprocess.fileReprocessCount + "')", conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nReproccess Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get List
        public List<Reprocess> getList(string connectionString)
        {
            using(SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    List<Reprocess> list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        var xxx = conn.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME);
                        //list = (List<Reprocess>) conn.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME);

                        list = xxx.ToList();

                        Console.WriteLine("Reprocess getList :: ");
                        //Console.WriteLine("Reprocess list size : "+xxx.ToList().Count);
                        Console.WriteLine("Reprocess list size : " + list.Count);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Reproccess List [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public Reprocess getById(string connectionString, int ediFileID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Reprocess list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + ediFileID);
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            Console.WriteLine("Reprocess list size : 1");
                        }
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Reproccess [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Reprocess reprocess)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_FILENAME + " = '" + reprocess.fileName + "', " + COLONNE_FILEPATH + " = '" + reprocess.filePath + "', " + COLONNE_COUNT + " = '" + reprocess.fileReprocessCount + "' WHERE " + COLONNE_EDIFILEID + " = " + reprocess.ediFileID, conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("ediFileID : " + reprocess.ediFileID + " is Updated !");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Reproccess Update [ERROR]");
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
                    Console.WriteLine("\nDELETE Reproccess List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }

        // Delete by ID
        public bool deleteById(string connectionString, Reprocess reprocess)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + reprocess.ediFileID, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(reprocess.ediFileID + " data have been deleted from " + TABLE_NAME + " !");
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

        public bool deleteById(string connectionString, int reprocessID)
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
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + reprocessID, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(reprocessID + " data have been deleted from " + TABLE_NAME + " !");
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | SQL => CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_EDIFILEID + "' INTEGER UNIQUE, '" + COLONNE_FILENAME + "' TEXT NOT NULL, '" + COLONNE_FILEPATH + "' TEXT NOT NULL, '" + COLONNE_COUNT + "' TEXT NOT NULL)");
                        SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_EDIFILEID + "' INTEGER UNIQUE, '" + COLONNE_FILENAME + "' TEXT NOT NULL, '" + COLONNE_FILEPATH + "' TEXT NOT NULL, '" + COLONNE_COUNT + "' TEXT NOT NULL)", conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("Table created");
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | Table created");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | ##### [ERROR] INSERT");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => createTable() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }


        // Insert
        public int insert(string connectionString, Reprocess reprocess, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(@"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_EDIFILEID + "', '" + COLONNE_FILENAME + "', '" + COLONNE_FILEPATH + "', '" + COLONNE_COUNT + "') VALUES(" + reprocess.ediFileID + ",'" + reprocess.fileName + "','" + reprocess.filePath + "','" + reprocess.fileReprocessCount + "')", conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("Table created");
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | Creation d'une instance");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | Reproccess Insert [ERROR]");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => insert() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | Creation d'une instance");
                        var xxx = conn.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME);
                        list = xxx.ToList();

                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | Creation d'une instance");
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | [ERROR] getList");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getList() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Get by ID
        public Reprocess getById(string connectionString, int ediFileID, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Reprocess list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById(ediFileID: " + ediFileID + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById(ediFileID: " + ediFileID + ") | SQL => SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + ediFileID);
                            var xxx = conn.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + ediFileID);
                            
                            if(xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | Reprocess obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | [ERROR] getById");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => getById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Reprocess reprocess, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | SQL => UPDATE " + TABLE_NAME + " SET " + COLONNE_FILENAME + " = '" + reprocess.fileName + "', " + COLONNE_FILEPATH + " = '" + reprocess.filePath + "', " + COLONNE_COUNT + " = '" + reprocess.fileReprocessCount + "' WHERE " + COLONNE_EDIFILEID + " = " + reprocess.ediFileID);
                        SQLiteCommand command = new SQLiteCommand(@"UPDATE " + TABLE_NAME + " SET " + COLONNE_FILENAME + " = '" + reprocess.fileName + "', " + COLONNE_FILEPATH + " = '" + reprocess.filePath + "', " + COLONNE_COUNT + " = '" + reprocess.fileReprocessCount + "' WHERE " + COLONNE_EDIFILEID + " = " + reprocess.ediFileID, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | ediFileID : " + reprocess.ediFileID + " is Updated !");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | [ERROR] update");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => update() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | [ERROR] delete all");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteAll() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        // Delete by ID
        public bool deleteById(string connectionString, Reprocess reprocess, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById(reprocess .ediFileID: " + reprocess .ediFileID+ ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + reprocess.ediFileID, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | [ERROR] deleteById (obj)");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

        public bool deleteById(string connectionString, int reprocessID, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById(reprocessID: " + reprocessID + ") | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | SQL => DELETE FROM " + TABLE_NAME);
                            SQLiteCommand command = new SQLiteCommand(@"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + " = " + reprocessID, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | [ERROR] deleteById (int)");
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: ReprocessManager.dll => deleteById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

    }
}
