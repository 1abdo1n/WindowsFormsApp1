using System;
using System.Data.Odbc;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public static class Global
	{
		// ============================================================
		//  Connection String for Azure SQL Database (ODBC)
		// ============================================================
		public static string ConnStr = "Driver={ODBC Driver 18 for SQL Server};Server=tcp:my-c-projects.database.windows.net,1433;Database=HR;Uid=abdullah;Pwd={r\"fCpkYp^_yNjr5};Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;";

		// ============================================================
		//  User Session Data
		// ============================================================
		public static int LoggedUserID { get; set; }
		public static int LoggedUserId { get; set; }  // Alias for EmployeeId
		public static string LoggedUserRole { get; set; }
		public static string LoggedUserName { get; set; }
		public static string LoggedUserFullName { get; set; }
		public static string LoggedUserDepartment { get; set; }
		public static int? LoggedUserDepartmentId { get; set; }

		// ============================================================
		//  Application Settings
		// ============================================================
		public static string AppName = "HR Management System";
		public static string AppVersion = "1.0.0";
		public static DateTime SessionStartTime { get; set; } = DateTime.Now;
		public static bool IsOfflineMode { get; set; } = false;

		// ============================================================
		//  Role Checks (Properties)
		// ============================================================
		public static bool IsAdmin => LoggedUserRole == "Admin";
		public static bool IsManager => LoggedUserRole == "Manager";
		public static bool IsEmployee => LoggedUserRole == "Employee";

		public static bool CanAccessAllData => IsAdmin || IsManager;
		public static bool CanEditData => IsAdmin || IsManager;
		public static bool CanApproveLeave => IsAdmin || IsManager;

		// ============================================================
		//  Session Management
		// ============================================================
		public static void ResetSession()
		{
			LoggedUserID = 0;
			LoggedUserId = 0;
			LoggedUserRole = null;
			LoggedUserName = null;
			LoggedUserFullName = null;
			LoggedUserDepartment = null;
			LoggedUserDepartmentId = null;
		}

		// ============================================================
		//  Load User Data from Database
		// ============================================================
		public static bool LoadUserData(int userId)
		{
			try
			{
				using (var con = new OdbcConnection(ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT 
                        u.username,
                        u.user_role,
                        e.full_name,
                        d.department_id,
                        d.department_name,
                        e.employee_id
                    FROM [User] u
                    INNER JOIN Employee e ON u.employee_id = e.employee_id
                    LEFT JOIN Department d ON e.department_id = d.department_id
                    WHERE u.user_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", userId);
					con.Open();
					var reader = cmd.ExecuteReader();
					if (reader.Read())
					{
						LoggedUserName = reader["username"].ToString();
						LoggedUserRole = reader["user_role"].ToString();
						LoggedUserFullName = reader["full_name"].ToString();
						LoggedUserID = Convert.ToInt32(reader["employee_id"]);
						LoggedUserId = LoggedUserID;
						if (reader["department_id"] != DBNull.Value)
							LoggedUserDepartmentId = Convert.ToInt32(reader["department_id"]);
						LoggedUserDepartment = reader["department_name"]?.ToString() ?? "";
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading user data: " + ex.Message);
			}
			return false;
		}

		// ============================================================
		//  Test Database Connection
		// ============================================================
		public static bool TestConnection()
		{
			try
			{
				using (var con = new OdbcConnection(ConnStr))
				{
					con.Open();
					return true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Connection failed:\n" + ex.Message, "Database Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	}
}