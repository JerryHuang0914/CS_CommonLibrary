using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace dev.jerry_h.pc_tools.CommonLibrary
{
    public static class SQL_Client
    {
        //private SqlConnection sql_conn = null;
        //private SqlCommand sql_cmd = null;
        //private static SQL_Client me;
        private static String connectionString = "";

        public static bool Connected
        {
            get
            {
                return testConnection();
            }
        }

        private static void Initialize(String ServerName, String InstanceName, String UserName, String Password, String DatabaseName)
        {
            connectionString = "Data Source=" + ServerName +";"+
                                              "User ID=" + UserName + ";" +
                                              "Password=" + Password + ";" +
                                              "Database=" + DatabaseName + ";" +
                                              "Timeout=2";
            if (InstanceName != null && InstanceName.Length > 0)
            {
                connectionString += ";User Instance=" + InstanceName;
            }
        }

        public static void NonQueryCommand(String command)
        {
            SqlConnection sql_conn = new SqlConnection();
            SqlCommand sql_cmd = new SqlCommand();
            try
            {
                sql_conn.ConnectionString = connectionString;
                sql_conn.Open();
                sql_cmd.CommandText = command;
                sql_cmd.Connection = sql_conn;
                sql_cmd.ExecuteNonQuery();
            }
            catch
            { }
            finally
            {
                if (sql_conn != null)
                {
                    sql_conn.Close();
                }
                sql_cmd = null;
            }
        }

        public static SqlDataReader Query(String command)
        {
            SqlConnection sql_conn = new SqlConnection();
            SqlCommand sql_cmd = new SqlCommand();
            SqlDataReader myDataReader = null;
            try
            {
                sql_conn.ConnectionString = connectionString;
                sql_conn.Open();
                sql_cmd.CommandText = command;
                sql_cmd.Connection = sql_conn;
                myDataReader = sql_cmd.ExecuteReader();
            }
            catch
            { }
            finally
            {
                if (sql_conn != null)
                {
                    sql_conn.Close();
                }
                sql_cmd = null;
            }
            return myDataReader;
        }

        public static bool Connect(String ServerName, String UserName, String Password, String DatabaseName)
        {
            return Connect(ServerName,"", UserName, Password, DatabaseName);
        }

        public static bool Connect(String ServerName, String InstanceName, String UserName, String Password, String DatabaseName)
        {
            Initialize(ServerName, InstanceName, UserName, Password, DatabaseName);
            return testConnection();
        }

        private static bool testConnection()
        {
            SqlConnection sql_conn = new SqlConnection();
            bool connected = false;
            try
            {
                sql_conn.ConnectionString = connectionString;
                sql_conn.Open();
                connected = sql_conn.State == System.Data.ConnectionState.Open;
            }
            catch (Exception ex)
            {
                connected = false;
            }
            finally
            {
                if (sql_conn != null)
                {
                    sql_conn.Close();
                }
            }
            return connected;
        }
    }
}
