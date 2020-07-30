using Dapper;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
        public int createTable(SQLiteConnection conn)
        {
            try
            {
                int x = -9;
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS '"+ TABLE_NAME + "' ('"+ COLONNE_EDIFILEID + "' INTEGER UNIQUE, '" + COLONNE_FILENAME + "' TEXT NOT NULL, '" + COLONNE_FILEPATH + "' TEXT NOT NULL, '" + COLONNE_COUNT + "' TEXT NOT NULL, PRIMARY KEY('" + COLONNE_EDIFILEID + "' AUTOINCREMENT))", conn);
                    x = command.ExecuteNonQuery();
                    Console.WriteLine("Table created");
                }

                conn.Close();
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -99;
            }
        }


        // Insert
        public int insert(SQLiteConnection conn, Reprocess reprocess)
        {
            try
            {
                int x = -9;
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    SQLiteCommand command = new SQLiteCommand(@"INSERT INTO '" + TABLE_NAME + "' ('" + COLONNE_EDIFILEID + "', '" + COLONNE_FILENAME + "', '" + COLONNE_FILEPATH + "', '" + COLONNE_COUNT + "') VALUES('" + reprocess.ediFileID + "','" + reprocess.fileName + "','" + reprocess.filePath + "','" + reprocess.fileReprocessCount + "')", conn);
                    x = command.ExecuteNonQuery();
                    Console.WriteLine("Table created");
                }

                conn.Close();
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -99;
            }
        }

        // Get List
        public List<Reprocess> getList(SQLiteConnection conn)
        {
            try
            {
                List<Reprocess> list = null;
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using(IDbConnection db = conn)
                    {
                        list = db.Query<Reprocess>("SELECT ediFileID, fileName, filePath, fileReprocessCount FROM Reprocess").ToList();
                        Console.WriteLine("Reprocess list size : " + list.Count);
                        return list;
                    }
                }

                conn.Close();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Get by ID
        public Reprocess getById(SQLiteConnection conn, int ediFileID)
        {
            try
            {
                Reprocess list = null;
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (IDbConnection db = conn)
                    {
                        list = db.Query<Reprocess>("SELECT ediFileID, fileName, filePath, fileReprocessCount FROM Reprocess WHERE ediFileID="+ ediFileID).ToList()[0];
                        Console.WriteLine("Reprocess list size : 1");
                        return list;
                    }
                }

                conn.Close();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Update 
        public int update(SQLiteConnection conn, Reprocess reprocess)
        {
            try
            {
                int x = -9;
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    SQLiteCommand command = new SQLiteCommand(@"UPDATE '" + TABLE_NAME + "' SET '" + COLONNE_FILENAME + "' = " + reprocess.fileName + ", '" + COLONNE_FILEPATH + "' = " + reprocess.filePath + ", '" + COLONNE_COUNT + "' = " + reprocess.fileReprocessCount + " WHERE '" + COLONNE_EDIFILEID + "' = " + reprocess.ediFileID, conn);
                    x = command.ExecuteNonQuery();
                    Console.WriteLine("ediFileID : " + reprocess.ediFileID + " is Updated !");
                }

                conn.Close();
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -99;
            }
        }

        // Delete ALL
        public bool deleteAll(SQLiteConnection conn, Reprocess reprocess)
        {
            try
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (IDbConnection db = conn)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"DELETE FROM '" + TABLE_NAME + "'", conn);
                        command.ExecuteNonQuery();
                        Console.WriteLine(TABLE_NAME + " data have been deleted!");
                        return true;
                    }
                }

                conn.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Delete by ID
        public bool deleteById(SQLiteConnection conn, Reprocess reprocess)
        {
            try
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (IDbConnection db = conn)
                    {
                        SQLiteCommand command = new SQLiteCommand(@"DELETE FROM '" + TABLE_NAME + "' WHERE '" + COLONNE_EDIFILEID + "' = " + reprocess.ediFileID, conn);
                        command.ExecuteNonQuery();
                        Console.WriteLine(reprocess.ediFileID + " data have been deleted from " + TABLE_NAME + " !");
                        return true;
                    }
                }

                conn.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
