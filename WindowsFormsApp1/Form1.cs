using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{


      public Form1()
		{
          InitializeComponent();
		}



		private void ShowDashboard()
		{
          // hide only the login controls (do not move or re-parent any controls)
			if (this.txtUsername != null) this.txtUsername.Visible = false;
			if (this.txtPassword != null) this.txtPassword.Visible = false;
			if (this.lblUser != null) this.lblUser.Visible = false;
			if (this.lblPass != null) this.lblPass.Visible = false;
			if (this.btnLogin != null) this.btnLogin.Visible = false;

			// show dashboard and resize form
			pnlDashboard.Visible = true;
			this.ClientSize = new Size(540, 360);
		}

		private void BtnDepartments_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Departments module not implemented yet.");
		}

		private void BtnEmployeesModule_Click(object sender, EventArgs e)
		{
			frmEmployees f = new frmEmployees();
			f.ShowDialog();
		}

		private void BtnContracts_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Contracts module not implemented yet.");
		}

		private void BtnAssets_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Assets module not implemented yet.");
		}

		private void BtnAttendanceModule_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Attendance module not implemented yet.");
		}

		private void BtnLeave_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Leave Requests module not implemented yet.");
		}

		private void BtnPayrollModule_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Payroll module not implemented yet.");
		}

		private void BtnReports_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Reports module not implemented yet.");
		}

		private void BtnLogout_Click(object sender, EventArgs e)
		{
			Application.Restart();
		}



		private void BtnLogin_Click(object sender, EventArgs e)
		{
			string username = txtUsername.Text.Trim();
			string password = txtPassword.Text; // In real app, use hash

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Please enter username and password.");
				return;
			}

			try
			{
				using (SqlConnection conn = DBHelper.GetConnection())
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SELECT TOP 1 Role, UserID, EmpID FROM Users WHERE Username=@user AND PasswordHash=@pass", conn);
					cmd.Parameters.AddWithValue("@user", username);
					cmd.Parameters.AddWithValue("@pass", password);
					using (SqlDataReader rdr = cmd.ExecuteReader())
					{
						if (rdr.Read())
						{
							string role = rdr.IsDBNull(0) ? "Employee" : rdr.GetString(0);
							Global.LoggedUserRole = role;
							Global.LoggedUserID = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);

                  MessageBox.Show($"Welcome {username} - Role: {role}");
					ShowDashboard();
						}
						else
						{
							MessageBox.Show("Invalid username or password.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Login error: " + ex.Message);
			}
		}
	}
}
