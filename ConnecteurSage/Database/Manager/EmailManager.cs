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
    public class EmailManager
    {
        #region Table entries
        private static string TABLE_NAME = "Notification";
        private static string COLONNE_ID = "id";
        private static string COLONNE_active = "active";
        // connexion settings
        private static string COLONNE_connexion_smtp = "connexion_smtp";
        private static string COLONNE_connexion_port = "connexion_port";
        private static string COLONNE_connexion_login = "connexion_login";
        private static string COLONNE_connexion_password = "connexion_password";
        // email list settings
        private static string COLONNE_email_client_list_active = "email_client_list_active";
        private static string COLONNE_email_client_list = "email_client_list";
        private static string COLONNE_email_team_list_active = "email_team_list_active";
        private static string COLONNE_email_team_list = "email_team_list";
        //email error settings
        public static string COLONNE_email_error_active = "email_error_active";
        private static string COLONNE_email_error_informClient = "email_error_informClient";
        private static string COLONNE_email_error_informTeam = "email_error_informTeam";
        // email summary settings
        private static string COLONNE_email_summary_active = "email_summary_active";
        private static string COLONNE_email_summary_hours = "email_summary_hours";
        private static string COLONNE_email_summary_lastActivated = "email_summary_lastActivated";

        #endregion

        #region SQLs
        public string SQL_create = @"CREATE TABLE IF NOT EXISTS '" + TABLE_NAME + "' ('" + COLONNE_ID + "' INTEGER UNIQUE, '" + COLONNE_active + "' INTEGER NOT NULL, '" + COLONNE_connexion_smtp + "' TEXT NOT NULL, '" + COLONNE_connexion_port + "' INTEGER NOT NULL, '"+ COLONNE_connexion_login + "' TEXT NOT NULL, '"+ COLONNE_connexion_password + "' TEXT NOT NULL, " +
            "'"+ COLONNE_email_client_list_active + "' INTEGER NOT NULL, '" + COLONNE_email_client_list + "' TEXT NOT NULL, '" + COLONNE_email_team_list_active + "' INTEGER NOT NULL, '" + COLONNE_email_team_list + "' TEXT NOT NULL, " +
            "'" + COLONNE_email_error_active + "' INTEGER NOT NULL, '" + COLONNE_email_error_informClient + "' INTEGER NOT NULL, '" + COLONNE_email_error_informTeam + "' INTEGER NOT NULL, '" + COLONNE_email_summary_active + "' INTEGER NOT NULL, '" + COLONNE_email_summary_hours + "' INTEGER NOT NULL, " +
            "'" + COLONNE_email_summary_lastActivated + "' INTEGER(15) NOT NULL)";

        private string SQL_get = @"SELECT "+ COLONNE_ID + ", " + COLONNE_active + ", " + COLONNE_connexion_smtp + ", " + COLONNE_connexion_port + ", " +
            "" + COLONNE_connexion_login + ", "+ COLONNE_connexion_password + ", "+ COLONNE_email_client_list_active + ", " + COLONNE_email_client_list + ", " + COLONNE_email_team_list_active + ", " +
            "" + COLONNE_email_team_list + ", " + COLONNE_email_error_active + ", " + COLONNE_email_error_informClient + ", " + COLONNE_email_error_informTeam + ", " + COLONNE_email_summary_active + ", " +
            "" + COLONNE_email_summary_hours + ", " + COLONNE_email_summary_lastActivated + " FROM " + TABLE_NAME + " WHERE " + COLONNE_ID + " = ";

        private string SQL_delete = @"DELETE TABLE "+TABLE_NAME;

        private string SQL_drop = @"DROP TABLE "+TABLE_NAME;
        #endregion


        public EmailManager() { }

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
        public int insert(string connectionString, Email email)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_active + "', '" + COLONNE_connexion_smtp + "', '" + COLONNE_connexion_port + "', '" + COLONNE_connexion_login + "', '" + COLONNE_connexion_password + "', " +
                        "'" + COLONNE_email_client_list_active + "', '" + COLONNE_email_client_list + "', '" + COLONNE_email_team_list_active + "', '" + COLONNE_email_team_list + "', " +
                        "'" + COLONNE_email_error_active + "', '" + COLONNE_email_error_informClient + "', '" + COLONNE_email_error_informTeam + "', '" + COLONNE_email_summary_active + "', '" + COLONNE_email_summary_hours + "', " +
                        "'" + COLONNE_email_summary_lastActivated + "') " +
                        "VALUES (1, " + email.active + ", '" + email.connexion_smtp + "', " + email.connexion_port + ", '" + email.connexion_login + "', '" + email.connexion_password + "', " + email.email_client_list_active + ", '" + email.email_client_list + "', " + email.email_team_list_active + ", '" + email.email_team_list + "', " +
                        "" + email.email_error_active + ", " + email.email_error_informClient + ", " + email.email_error_informTeam + ", " + email.email_summary_active + ", " + email.email_summary_hours + ", " +
                        "" + email.email_summary_lastActivated + ")";


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
                    Console.WriteLine("\nEmail Insert [ERROR]");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("\nStackTrace : " + ex.StackTrace);
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get by ID
        public Email get(string connectionString, int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Email list = null;
                    conn.Open();

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            var xxx = conn.Query<Email>(SQL_get+""+id);
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
                    Console.WriteLine("GET Email [ERROR]");
                    Console.WriteLine(ex.Message);
                    //conn.Close();
                    return null;
                }
            }
        }


        // Update 
        public int update(string connectionString, Email email)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString)) 
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        ""+ COLONNE_active + " = " + email.active + ", " +
                        "" + COLONNE_connexion_smtp + " = '" + email.connexion_smtp + "', " +
                        "" + COLONNE_connexion_port + " = " + email.connexion_port + ", " +
                        "" + COLONNE_connexion_login + " = '" + email.connexion_login + "', " +
                        "" + COLONNE_connexion_password + " = '" + email.connexion_password + "', " +
                        "" + COLONNE_email_client_list_active + " = " + email.email_client_list_active + ", " +
                        "" + COLONNE_email_client_list + " = '" + email.email_client_list + "', " +
                        "" + COLONNE_email_team_list_active + " = " + email.email_team_list_active + ", " +
                        "" + COLONNE_email_team_list + " = '" + email.email_team_list + "', " +
                        "" + COLONNE_email_error_active + " = " + email.email_error_active + ", " +
                        "" + COLONNE_email_error_informClient + " = " + email.email_error_informClient + ", " +
                        "" + COLONNE_email_error_informTeam + " = " + email.email_error_informTeam + ", " +
                        "" + COLONNE_email_summary_active + " = " + email.email_summary_active + ", " +
                        "" + COLONNE_email_summary_hours + " = " + email.email_summary_hours + ", " +
                        "" + COLONNE_email_summary_lastActivated + " = " + email.email_summary_lastActivated + " WHERE " + COLONNE_ID + " = "+ email.id;

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
                    Console.WriteLine("\n Email Update [ERROR]");
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
                    Console.WriteLine("\nDELETE Email List [ERROR]");
                    Console.WriteLine(ex.Message);
                    conn.Close();
                    return false;
                }
            }
            
        }

        // Delete by ID
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
                    Console.WriteLine("\nDELETE Email [ERROR]");
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
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | SQL => " + SQL_create);
                        SQLiteCommand command = new SQLiteCommand(SQL_create, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | Table created");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | ##### [ERROR] INSERT");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => createTable() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Insert
        public int insert(string connectionString, Email email, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_insert = @"INSERT INTO " + TABLE_NAME + " ('" + COLONNE_ID + "', '" + COLONNE_active + "', '" + COLONNE_connexion_smtp + "', '" + COLONNE_connexion_port + "', '" + COLONNE_connexion_login + "', '" + COLONNE_connexion_password + "', " +
                        "'" + COLONNE_email_client_list_active + "', '" + COLONNE_email_client_list + "', '" + COLONNE_email_team_list_active + "', '" + COLONNE_email_team_list + "', " +
                        "'" + COLONNE_email_error_active + "', '" + COLONNE_email_error_informClient + "', '" + COLONNE_email_error_informTeam + "', '" + COLONNE_email_summary_active + "', '" + COLONNE_email_summary_hours + "', " +
                        "'" + COLONNE_email_summary_lastActivated + "') " +
                        "VALUES (1, " + email.active + ", '" + email.connexion_smtp + "', " + email.connexion_port + ", '" + email.connexion_login + "', '" + email.connexion_password + "', " + email.email_client_list_active + ", '" + email.email_client_list + "', " + email.email_team_list_active + ", '" + email.email_team_list + "', " +
                        "" + email.email_error_active + ", " + email.email_error_informClient + ", " + email.email_error_informTeam + ", " + email.email_summary_active + ", " + email.email_summary_hours + ", " +
                        "" + email.email_summary_lastActivated + ")";

                    int x = -9;
                    conn.Open();
                    writer.WriteLine("");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | Creation d'une instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | Creation d'une instance");
                        SQLiteCommand command = new SQLiteCommand(SQL_insert, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | Creation d'une instance");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | Reproccess Insert [ERROR]");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => insert() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return -99;
                }
            }
        }

        // Get by ID
        public Email get(string connectionString, int id, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    Email list = null;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById(id: " + id + ") | Creation d'un Instance.");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById(di: " + id + ") | SQL => "+SQL_get);
                            var xxx = conn.Query<Email>(SQL_get);
                            
                            if(xxx.ToList() == null || xxx.ToList().Count == 0)
                            {
                                writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | Aucun resulta.");
                                writer.Flush();
                                conn.Close();
                                return null;
                            }
                            list = xxx.ToList()[0];
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | Settings obj recu");
                        }
                    }
                    writer.Flush();
                    //conn.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | [ERROR] getById");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => getById() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    //conn.Close();
                    return null;
                }
            }
        }

        // Update 
        public int update(string connectionString, Email email, StreamWriter writer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    string SQL_put = @"UPDATE " + TABLE_NAME + " SET " +
                        "" + COLONNE_active + " = " + email.active + ", " +
                        "" + COLONNE_connexion_smtp + " = '" + email.connexion_smtp + "', " +
                        "" + COLONNE_connexion_port + " = " + email.connexion_port + ", " +
                        "" + COLONNE_connexion_login + " = '" + email.connexion_login + "', " +
                        "" + COLONNE_connexion_password + " = '" + email.connexion_password + "', " +
                        "" + COLONNE_email_client_list_active + " = " + email.email_client_list_active + ", " +
                        "" + COLONNE_email_client_list + " = '" + email.email_client_list + "', " +
                        "" + COLONNE_email_team_list_active + " = " + email.email_team_list_active + ", " +
                        "" + COLONNE_email_team_list + " = '" + email.email_team_list + "', " +
                        "" + COLONNE_email_error_active + " = " + email.email_error_active + ", " +
                        "" + COLONNE_email_error_informClient + " = " + email.email_error_informClient + ", " +
                        "" + COLONNE_email_error_informTeam + " = " + email.email_error_informTeam + ", " +
                        "" + COLONNE_email_summary_active + " = " + email.email_summary_active + ", " +
                        "" + COLONNE_email_summary_hours + " = " + email.email_summary_hours + ", " +
                        "" + COLONNE_email_summary_lastActivated + " = " + email.email_summary_lastActivated + " WHERE " + COLONNE_ID + " = " + email.id;

                    int x = -9;
                    conn.Open();
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | SQL => "+ SQL_put);
                        SQLiteCommand command = new SQLiteCommand(SQL_put, conn);
                        x = command.ExecuteNonQuery();
                        writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | id : " + email.id + " is Updated !");
                    }
                    writer.Flush();
                    conn.Close();
                    return x;
                }
                catch (Exception ex)
                {
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | [ERROR] update");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => update() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | SQL => "+SQL_delete);
                            SQLiteCommand command = new SQLiteCommand(SQL_delete, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | Les donnees de la table " + TABLE_NAME + " sont supprime!");
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
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | [ERROR] delete all");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => deleteAll() | StackTrace : " + ex.StackTrace);
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
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | Creation d'une Instance");

                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (IDbConnection db = conn)
                        {
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | SQL => "+SQL_drop);
                            SQLiteCommand command = new SQLiteCommand(SQL_drop, conn);
                            command.ExecuteNonQuery();
                            writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | Table "+ SQL_drop + " dropped");
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
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | ##################################################");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | [ERROR] drop ()");
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | Message : " + ex.Message);
                    writer.WriteLine(DateTime.Now + " :: EmailManager.dll => drop() | StackTrace : " + ex.StackTrace);
                    writer.WriteLine("");
                    writer.Flush();
                    conn.Close();
                    return false;
                }
            }

        }

    }
}
