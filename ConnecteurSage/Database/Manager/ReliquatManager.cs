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
    public class ReliquatManager
    {
        #region Table entries
        private string TABLE_NAME = "Reliquat";
        private string COLONNE_ID = "id";
        private string COLONNE_Date = "date";
        private string COLONNE_Modification = "modification";
        private string COLONNE_DO_PIECE = "do_piece";
        private string COLONNE_DO_REF = "do_ref";
        private string COLONNE_DL_Ligne = "dl_ligne";
        private string COLONNE_DL_Design = "dl_design";
        private string COLONNE_DL_Qte = "dl_qte";
        private string COLONNE_DL_QtePL = "dl_qtepl";
        private string COLONNE_AR_Ref = "ar_ref";
        private string COLONNE_AR_CODEBARRE = "ar_codebarre";
        #endregion

        public ReliquatManager() { }


        // Create Reliquat Table
        public int createTable(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    int x = -9;
                    conn.Open();
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => createTable() | Creation d'une instance"); }

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = @"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "'('" + COLONNE_ID + "' INTEGER UNIQUE, '" + COLONNE_Date + "' TEXT NOT NULL, '" + COLONNE_Modification + "' TEXT NOT NULL, '" + COLONNE_DO_PIECE + "' TEXT NOT NULL, '" + COLONNE_DO_REF + "' TEXT NOT NULL" +
                            ", '" + COLONNE_DL_Ligne + "' TEXT NOT NULL, '" + COLONNE_DL_Design + "' TEXT NOT NULL, '" + COLONNE_DL_Qte + "' TEXT NOT NULL, '" + COLONNE_DL_QtePL + "' TEXT NOT NULL, '" + COLONNE_AR_Ref + "' TEXT NOT NULL, '" + COLONNE_AR_CODEBARRE + "' TEXT NOT NULL)";
                        if (writer != null) { writer.WriteLine("SQL: "+sql); }

                        SQLiteCommand command = new SQLiteCommand(sql, conn);
                        x = command.ExecuteNonQuery();
                        // Console.WriteLine("Table created / Exist");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (writer != null) { writer.WriteLine("#### Error Exception : \n" + ex.Message); }
                    if (writer != null) { writer.Flush(); }
                    conn.Close();
                    return -99;
                }
            }
        }

        // Insert
        public int insert(string connectionString, Reliquat reliquat, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => insert() | Creation d'une instance"); }

                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_Date + "', '" + COLONNE_Modification + "', '" + COLONNE_DO_PIECE + "', '" + COLONNE_DO_REF + "', '" + COLONNE_DL_Ligne + "', " +
                            "'" + COLONNE_DL_Design + "', '" + COLONNE_DL_Qte + "', '" + COLONNE_DL_QtePL + "', '" + COLONNE_AR_Ref + "', '" + COLONNE_AR_CODEBARRE + "') " +
                            "VALUES(" + reliquat.id + ",'" + reliquat.date + "','" + reliquat.modification + "','" + reliquat.do_piece + "','" + reliquat.do_ref + "','" + reliquat.dl_ligne + "','" + reliquat.dl_design + "','" + reliquat.dl_qte + "','" + reliquat.dl_qtepl + "'" +
                            ",'" + reliquat.ar_ref + "','" + reliquat.ar_codebarre + "')";
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        SQLiteCommand command = new SQLiteCommand(sql, conn);
                        x = command.ExecuteNonQuery();
                        //Console.WriteLine("Table created");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nReliquat Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);

                    if (writer != null) { writer.WriteLine("Reliquat Insert [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    conn.Close();
                    return -99;
                }
            }
        }

        public int insert(string connectionString, List<Reliquat> reliquatList, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => insert("+ reliquatList.Count+ ") | Creation d'une instance"); }

                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        foreach(Reliquat reliquat in reliquatList)
                        {
                            string sql = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_Date + "', '" + COLONNE_Modification + "', '" + COLONNE_DO_PIECE + "', '" + COLONNE_DO_REF + "', '" + COLONNE_DL_Ligne + "', " +
                                "'" + COLONNE_DL_Design + "', '" + COLONNE_DL_Qte + "', '" + COLONNE_DL_QtePL + "', '" + COLONNE_AR_Ref + "', '" + COLONNE_AR_CODEBARRE + "') " +
                                "VALUES(" + reliquat.id + ",'" + reliquat.date + "','" + reliquat.modification + "','" + reliquat.do_piece + "','" + reliquat.do_ref + "','" + reliquat.dl_ligne + "','" + reliquat.dl_design + "','" + reliquat.dl_qte + "','" + reliquat.dl_qtepl + "'" +
                                ",'" + reliquat.ar_ref + "','" + reliquat.ar_codebarre + "')";
                            if (writer != null) { writer.WriteLine("SQL: " + sql); }

                            SQLiteCommand command = new SQLiteCommand(sql, conn);
                            x = command.ExecuteNonQuery();
                        }
                        
                        //Console.WriteLine("Table created");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nReliquat Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);

                    if (writer != null) { writer.WriteLine("Reliquat Insert [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    conn.Close();
                    return -99;
                }
            }
        }


        // Get List
        public List<Reliquat> getList(string connectionString, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => getList() | Creation d'une instance"); }

                    List<Reliquat> list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = "SELECT * FROM " + TABLE_NAME;
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        var xxx = conn.Query<Reliquat>(sql);
                        list = xxx.ToList();

                        Console.WriteLine("Reliquat getList :: ");
                        Console.WriteLine("Reliquat list size : " + list.Count);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Reliquat List [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    //conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Insert [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }

                    return null;
                }
            }
        }

        // Get by ID
        public Reliquat getById(string connectionString, StreamWriter writer, string do_piece, string dl_ligne, string ar_codebarre)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => getById() | Creation d'une instance"); }

                    Reliquat list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = "SELECT * FROM " + TABLE_NAME + " WHERE " + COLONNE_DO_PIECE + " = '" + do_piece + "' AND " + COLONNE_DL_Ligne + " = " + dl_ligne + " AND " + COLONNE_AR_CODEBARRE + " = '" + ar_codebarre + "'";
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<Reliquat>(sql);
                            if (xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            Console.WriteLine("Reliquat list size : 1");
                        }
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Reliquat [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Get [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    return null;
                }
            }
        }

        public List<Reliquat> getByDo_Piece(string connectionString, StreamWriter writer, string do_piece)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => getList() | Creation d'une instance"); }

                    List<Reliquat> list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = "SELECT * FROM " + TABLE_NAME + " WHERE " + COLONNE_DO_PIECE + " = '" + do_piece + "'";
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        var xxx = conn.Query<Reliquat>(sql);
                        list = xxx.ToList();

                        Console.WriteLine("Reliquat getList :: ");
                        Console.WriteLine("Reliquat list size : " + list.Count);
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
                    }

                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nGET Reliquat List [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("");
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    //conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Insert [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }

                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, StreamWriter writer, Reliquat reliquat)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => update() | Creation d'une instance"); }

                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = @"UPDATE " + TABLE_NAME + " SET " +
                            "" + COLONNE_Modification + " = '" + reliquat.modification + "', " +
                            "" + COLONNE_DL_Ligne + " = '" + reliquat.dl_ligne + "', " +
                            "" + COLONNE_DL_Qte + " = '" + reliquat.dl_qte + "', " +
                            "" + COLONNE_DL_QtePL + " = '" + reliquat.dl_qtepl + "' " +
                            "WHERE " + COLONNE_DO_PIECE + " = " + reliquat.do_piece + " AND " + COLONNE_DL_Ligne + " = " + reliquat.dl_ligne + " AND " + COLONNE_AR_CODEBARRE + " = " + reliquat.ar_codebarre + " ";
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        SQLiteCommand command = new SQLiteCommand(sql, conn);
                        x = command.ExecuteNonQuery();
                        Console.WriteLine("In do_piece " + reliquat.do_piece + " the article "+reliquat.ar_codebarre+" on line "+reliquat.dl_ligne+",  is Updated !");
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Reliquat Update [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Update [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    return -99;
                }
            }
        }

        public int update(string connectionString, StreamWriter writer, List<Reliquat> reliquatList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => update("+ reliquatList .Count+ ") | Creation d'une instance"); }

                    int x = -9;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        foreach(Reliquat reliquat in reliquatList)
                        {
                            string sql = @"UPDATE " + TABLE_NAME + " SET " +
                                "" + COLONNE_Modification + " = '" + reliquat.modification + "', " +
                                "" + COLONNE_DL_Ligne + " = '" + reliquat.dl_ligne + "', " +
                                "" + COLONNE_DL_Qte + " = '" + reliquat.dl_qte + "', " +
                                "" + COLONNE_DL_QtePL + " = '" + reliquat.dl_qtepl + "' " +
                                "WHERE " + COLONNE_DO_PIECE + " = " + reliquat.do_piece + " AND " + COLONNE_DL_Ligne + " = " + reliquat.dl_ligne + " AND " + COLONNE_AR_CODEBARRE + " = " + reliquat.ar_codebarre + " ";
                            if (writer != null) { writer.WriteLine("SQL: " + sql); }

                            SQLiteCommand command = new SQLiteCommand(sql, conn);
                            x = command.ExecuteNonQuery();
                            Console.WriteLine("In do_piece " + reliquat.do_piece + " the article " + reliquat.ar_codebarre + " on line " + reliquat.dl_ligne + ",  is Updated !");
                        }
                    }

                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Reliquat Update [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Update [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
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
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => deleteAll() | Creation d'une instance"); }

                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = @"DELETE FROM " + TABLE_NAME;
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        using (IDbConnection db = conn)
                        {
                            SQLiteCommand command = new SQLiteCommand(sql, conn);
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
                    Console.WriteLine("\nDELETE Reliquat List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();

                    if (writer != null) { writer.WriteLine("Reliquat Delete all [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    return false;
                }
            }
        }

        // delete a row
        public bool deleteById(string connectionString, StreamWriter writer, Reliquat reliquat)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine(DateTime.Now + " :: ReliquatManager => deleteById() | Creation d'une instance"); }

                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string sql = @"DELETE FROM " + TABLE_NAME + " WHERE " + COLONNE_DO_PIECE + " = " + reliquat.do_piece + " AND " + COLONNE_DL_Ligne + " = " + reliquat.dl_ligne + " AND " + COLONNE_AR_CODEBARRE + " = " + reliquat.ar_codebarre + " ";
                        if (writer != null) { writer.WriteLine("SQL: " + sql); }

                        using (IDbConnection db = conn)
                        {
                            SQLiteCommand command = new SQLiteCommand(sql, conn);
                            command.ExecuteNonQuery();
                            Console.WriteLine(" data have been deleted from " + TABLE_NAME + " !");
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

                    if (writer != null) { writer.WriteLine("Reliquat Delete by id [ERROR]"); }
                    if (writer != null) { writer.WriteLine("Message : " + ex.Message); }
                    if (writer != null) { writer.WriteLine(""); }
                    if (writer != null) { writer.WriteLine("StackTrace : " + ex.StackTrace); }
                    if (writer != null) { writer.Flush(); }
                    return false;
                }
            }
        }


    }
}
