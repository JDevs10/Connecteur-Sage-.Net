using Dapper;
using Database.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
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
                        Console.WriteLine("Table created");
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
                        Console.WriteLine("Table created");
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
                            list = xxx.ToList()[0];
                            //list = ((List<Reprocess>) db.Query<Reprocess>("SELECT " + COLONNE_EDIFILEID + ", " + COLONNE_FILENAME + ", " + COLONNE_FILEPATH + ", " + COLONNE_COUNT + " FROM " + TABLE_NAME + " WHERE " + COLONNE_EDIFILEID + "=" + ediFileID))[0];
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

    }
}
