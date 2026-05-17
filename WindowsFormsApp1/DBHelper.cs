using System;
using System.Configuration;
using System.Data.Odbc;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public static class DBHelper
	{
		private static string ConfigConnectionString => ConfigurationManager.ConnectionStrings["HRMSConnection"]?.ConnectionString;

		public static string ConnectionString
		{
			get
			{
				if (string.IsNullOrWhiteSpace(ConfigConnectionString))
					throw new InvalidOperationException("Connection string 'HRMSConnection' not found in App.config. Add it and ensure System.Configuration is referenced.");
				return ConfigConnectionString;
			}
		}

		// Get ODBC Connection
		public static OdbcConnection GetConnection()
		{
			return new OdbcConnection(ConnectionString);
		}

		// Test connection
		public static bool TestConnection()
		{
			try
			{
				using (var con = GetConnection())
				{
					con.Open();
					return true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Connection failed: {ex.Message}", "Database Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		// Execute non-query (INSERT, UPDATE, DELETE)
		public static int ExecuteNonQuery(string sql, params OdbcParameter[] parameters)
		{
			using (var con = GetConnection())
			using (var cmd = new OdbcCommand(sql, con))
			{
				if (parameters != null)
					cmd.Parameters.AddRange(parameters);
				con.Open();
				return cmd.ExecuteNonQuery();
			}
		}

		// Execute scalar (SELECT COUNT, etc.)
		public static object ExecuteScalar(string sql, params OdbcParameter[] parameters)
		{
			using (var con = GetConnection())
			using (var cmd = new OdbcCommand(sql, con))
			{
				if (parameters != null)
					cmd.Parameters.AddRange(parameters);
				con.Open();
				return cmd.ExecuteScalar();
			}
		}

		// Execute reader (SELECT)
		public static OdbcDataReader ExecuteReader(string sql, params OdbcParameter[] parameters)
		{
			var con = GetConnection();
			var cmd = new OdbcCommand(sql, con);

			if (parameters != null)
				cmd.Parameters.AddRange(parameters);

			con.Open();
			return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
		}

		// Helper method to create parameter
		public static OdbcParameter CreateParameter(object value)
		{
			// ODBC uses '?' as parameter placeholder
			return new OdbcParameter("?", value ?? DBNull.Value);
		}

		// Helper method to create parameter with specific type
		public static OdbcParameter CreateParameter(object value, OdbcType type)
		{
			return new OdbcParameter("?", value ?? DBNull.Value) { OdbcType = type };
		}
	}
}