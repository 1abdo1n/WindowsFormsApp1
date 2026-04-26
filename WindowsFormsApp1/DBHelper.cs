using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class DBHelper
    {
        private static string ConfigConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["HRMSConnection"]?.ConnectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConfigConnectionString))
                    throw new InvalidOperationException("Connection string 'HRMSConnection' not found in App.config. Add it and ensure System.Configuration is referenced.");
                return ConfigConnectionString;
            }
        }

        public static SqlConnection GetConnection()
        {
           return new SqlConnection(ConnectionString);
        }
    }
}