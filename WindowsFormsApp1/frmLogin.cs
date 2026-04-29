using System;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmLogin : Form
	{
        // Enable temporary debug output for password hashes. Set to false in production.
		private readonly bool DebugShowHashes = true;

		// ── Colors ────────────────────────────────────────────────
		private static readonly Color Blue = Color.FromArgb(26, 86, 219);
		private static readonly Color Green = Color.FromArgb(34, 197, 94);
		private static readonly Color BgGray = Color.FromArgb(243, 244, 246);
		private static readonly Color LabelFg = Color.FromArgb(55, 65, 81);
		private static readonly Color GrayFg = Color.FromArgb(156, 163, 175);
		private static readonly Color RedFg = Color.FromArgb(185, 28, 28);
		private static readonly Color InputBg = Color.FromArgb(249, 250, 251);

		public frmLogin()
		{
			Text = "HR System — Sign In";
			Size = new Size(420, 590);
			StartPosition = FormStartPosition.CenterScreen;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			BackColor = BgGray;
			Font = new Font("Segoe UI", 9f);

			// Test connection first
			if (!TestDatabaseConnection())
			{
				DialogResult result = MessageBox.Show(
					"Cannot connect to the database.\n\nPlease check your connection and try again.\n\nDo you want to exit?",
					"Connection Error",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Error);
				if (result == DialogResult.Yes)
					Environment.Exit(0);
			}

			var tabs = new TabControl { Size = new Size(366, 520), Location = new Point(22, 22), Font = new Font("Segoe UI", 9f) };
			tabs.TabPages.Add(BuildLoginTab(tabs));
			tabs.TabPages.Add(BuildRegisterTab(tabs));
			Controls.Add(tabs);
		}

		private bool TestDatabaseConnection()
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		// ─────────────────────────────────────────────────────────
		//  LOGIN TAB
		// ─────────────────────────────────────────────────────────
		private TabPage BuildLoginTab(TabControl tabs)
		{
			var page = new TabPage("Sign In");
			var panel = new Panel { Size = new Size(344, 460), Location = new Point(3, 3), BackColor = Color.White };

			// Blue header strip
			panel.Controls.Add(MakeHeader("HR Management System", "Employee Affairs Portal"));

			// Fields
			panel.Controls.Add(MakeLabel("Welcome Back!", new Font("Segoe UI", 11f, FontStyle.Bold), Color.FromArgb(30, 30, 30), new Point(24, 72)));
			panel.Controls.Add(MakeLabel("Username", null, LabelFg, new Point(24, 110)));

			var txtUser = MakeTextBox("txtUsername", new Point(24, 128), 292);
			panel.Controls.Add(txtUser);

			panel.Controls.Add(MakeLabel("Password", null, LabelFg, new Point(24, 172)));
			var txtPass = MakeTextBox("txtPassword", new Point(24, 190), 258, isPassword: true);
			var btnEye = MakeEyeBtn(new Point(284, 190), txtPass);
			panel.Controls.Add(txtPass);
			panel.Controls.Add(btnEye);

			var lblError = MakeLabel("", null, RedFg, new Point(24, 228));
			panel.Controls.Add(lblError);

			// Login button
			var btnLogin = MakePrimaryBtn("Sign In", Blue, new Point(24, 252), 292);
			panel.Controls.Add(btnLogin);

			// Forgot / Contact HR link
			var lnkContact = new LinkLabel { Text = "Forgot password? Contact HR", Font = new Font("Segoe UI", 8.5f), LinkColor = Blue, AutoSize = true, Location = new Point(24, 302) };
			lnkContact.Click += (s, e) => MessageBox.Show("Contact your HR administrator to reset your password.\n\nEmail: hr@hrsystem.com", "Contact HR", MessageBoxButtons.OK, MessageBoxIcon.Information);
			panel.Controls.Add(lnkContact);

			panel.Controls.Add(MakeLabel("HR System v1.0", new Font("Segoe UI", 8f), GrayFg, new Point(24, 420)));

			// Login logic with database
			btnLogin.Click += (s, e) =>
			{
				string username = txtUser.Text.Trim();
				string password = txtPass.Text;

				if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				{
					lblError.Text = "Please enter username and password.";
					return;
				}

				var user = AuthenticateUser(username, password);
				if (user != null)
				{
					lblError.Text = "";

					// Set global session variables
					Global.LoggedUserID = user.EmployeeId;
					Global.LoggedUserId = user.EmployeeId;
					Global.LoggedUserRole = user.Role;
					Global.LoggedUserFullName = user.FullName;
					Global.LoggedUserDepartment = user.Department;
					Global.LoadUserData(user.UserId);

					MessageBox.Show($"Welcome back, {user.FullName}!\nRole: {user.Role}", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

					var mainForm = new frmMainNew();
					mainForm.Show();
					Hide();
				}
                else
				{
					lblError.Text = "Invalid username or password.";
					txtPass.Clear();
					txtPass.Focus();
					// Debug: show entered hash vs stored hash for the username (do not enable in production)
					if (DebugShowHashes)
					{
						try
						{
							string enteredHash = HashPassword(password);
							string storedHash = GetStoredPasswordHash(username);
							MessageBox.Show($"Entered hash:\n{enteredHash}\n\nStored hash:\n{storedHash ?? "<no user found>"}", "Debug: Password Hashes", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						catch (Exception ex)
						{
							MessageBox.Show("Debug error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			};

			txtPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };

			page.Controls.Add(panel);
			return page;
		}

		// ─────────────────────────────────────────────────────────
		//  REGISTER TAB
		// ─────────────────────────────────────────────────────────
		private TabPage BuildRegisterTab(TabControl tabs)
		{
			var page = new TabPage("Register");
			var panel = new Panel { Size = new Size(344, 520), Location = new Point(3, 3), BackColor = Color.White, AutoScroll = true };

			panel.Controls.Add(MakeHeader("Join Our Team", "Create your account to get started"));
			panel.Controls.Add(MakeLabel("Create New Account", new Font("Segoe UI", 11f, FontStyle.Bold), Color.FromArgb(30, 30, 30), new Point(24, 65)));

			// Build fields
			int y = 100;
			var txtFullName = AddField(panel, "Full Name *", ref y, 292);
			var txtEmail = AddField(panel, "Email Address *", ref y, 292);
			var txtPhone = AddField(panel, "Phone Number", ref y, 292);
			var txtRegUser = AddField(panel, "Username *", ref y, 292);
			var txtRegPass = AddField(panel, "Password *", ref y, 258, isPassword: true);
			var btnEye1 = MakeEyeBtn(new Point(284, y - 30), txtRegPass);
			panel.Controls.Add(btnEye1);
			var txtConfPass = AddField(panel, "Confirm Password *", ref y, 258, isPassword: true);
			var btnEye2 = MakeEyeBtn(new Point(284, y - 30), txtConfPass);
			panel.Controls.Add(btnEye2);

			// Department dropdown (from database)
			panel.Controls.Add(MakeLabel("Department *", null, LabelFg, new Point(24, y)));
			var cmbDept = new ComboBox
			{
				Size = new Size(292, 28),
				Location = new Point(24, y + 18),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f),
				DisplayMember = "department_name",
				ValueMember = "department_id"
			};
			LoadDepartments(cmbDept);
			panel.Controls.Add(cmbDept);
			y += 56;

			var lblRegError = MakeLabel("", null, RedFg, new Point(24, y));
			panel.Controls.Add(lblRegError);
			y += 24;

			var btnRegister = MakePrimaryBtn("Create Account", Green, new Point(24, y), 292);
			panel.Controls.Add(btnRegister);

			// Register logic with database
			btnRegister.Click += (s, e) =>
			{
				string fullName = txtFullName.Text.Trim();
				string email = txtEmail.Text.Trim();
				string phone = txtPhone.Text.Trim();
				string username = txtRegUser.Text.Trim();
				string password = txtRegPass.Text;
				string confirm = txtConfPass.Text;
				int? departmentId = cmbDept.SelectedValue as int?;

				if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || !departmentId.HasValue)
				{
					lblRegError.Text = "Please fill in all required fields (*).";
					return;
				}

				if (!IsValidEmail(email))
				{
					lblRegError.Text = "Please enter a valid email address.";
					return;
				}

				if (password.Length < 4)
				{
					lblRegError.Text = "Password must be at least 4 characters.";
					return;
				}

				if (password != confirm)
				{
					lblRegError.Text = "Passwords do not match.";
					return;
				}

				string result = RegisterUser(username, password, fullName, email, phone, departmentId.Value);
				if (result == "SUCCESS")
				{
					MessageBox.Show($"Account created!\n\nWelcome, {fullName}!\nYou can now sign in.", "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Clear form
					txtFullName.Clear();
					txtEmail.Clear();
					txtPhone.Clear();
					txtRegUser.Clear();
					txtRegPass.Clear();
					txtConfPass.Clear();
					cmbDept.SelectedIndex = -1;
					lblRegError.Text = "";
					tabs.SelectedIndex = 0;
				}
				else
				{
					lblRegError.Text = result;
				}
			};

			txtConfPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnRegister.PerformClick(); };

			page.Controls.Add(panel);
			return page;
		}

		// ─────────────────────────────────────────────────────────
		//  DATABASE METHODS
		// ─────────────────────────────────────────────────────────

		private class UserInfo
		{
			public int UserId { get; set; }
			public int EmployeeId { get; set; }
			public string Username { get; set; }
			public string FullName { get; set; }
			public string Role { get; set; }
			public string Department { get; set; }
		}

		private UserInfo AuthenticateUser(string username, string password)
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT 
                        u.user_id,
                        e.employee_id,
                        u.username,
                        e.full_name,
                        u.user_role,
                        d.department_name
                    FROM [User] u
                    INNER JOIN Employee e ON u.employee_id = e.employee_id
                    LEFT JOIN Department d ON e.department_id = d.department_id
                    WHERE u.username = ? AND u.password_hash = ? AND u.is_active = 1", con))
				{
					cmd.Parameters.AddWithValue("?", username);
					cmd.Parameters.AddWithValue("?", HashPassword(password));

					con.Open();
					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							return new UserInfo
							{
								UserId = Convert.ToInt32(reader["user_id"]),
								EmployeeId = Convert.ToInt32(reader["employee_id"]),
								Username = reader["username"].ToString(),
								FullName = reader["full_name"].ToString(),
								Role = reader["user_role"]?.ToString() ?? "Employee",
								Department = reader["department_name"]?.ToString() ?? ""
							};
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            return null;
		}

		// Debug helper to retrieve stored password hash for a username
		private string GetStoredPasswordHash(string username)
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("SELECT password_hash FROM [User] WHERE username = ?", con))
				{
					cmd.Parameters.AddWithValue("?", username);
					con.Open();
					var result = cmd.ExecuteScalar();
					return result == null || result == DBNull.Value ? null : result.ToString();
				}
			}
			catch { return null; }
		}

		private void LoadDepartments(ComboBox cmb)
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("SELECT department_id, department_name FROM Department ORDER BY department_name", con))
				{
					con.Open();
					var reader = cmd.ExecuteReader();
					var dt = new System.Data.DataTable();
					dt.Load(reader);
					cmb.DataSource = dt;
					cmb.DisplayMember = "department_name";
					cmb.ValueMember = "department_id";
					cmb.SelectedIndex = -1;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading departments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private string RegisterUser(string username, string password, string fullName, string email, string phone, int departmentId)
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();

					// Check if username exists
					using (var chk = new OdbcCommand("SELECT COUNT(1) FROM [User] WHERE username = ?", con))
					{
						chk.Parameters.AddWithValue("?", username);
						if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
							return "Username already exists.";
					}

					// Check if email exists
					using (var chk = new OdbcCommand("SELECT COUNT(1) FROM Employee WHERE email = ?", con))
					{
						chk.Parameters.AddWithValue("?", email);
						if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
							return "Email already registered.";
					}

					// Generate employee code
					string empCode = "EMP" + DateTime.Now.ToString("yyyyMMddHHmmss");

					// Insert Employee
					using (var cmd = new OdbcCommand(@"
                        INSERT INTO Employee (employee_code, full_name, email, phone_number, department_id, hire_date, status)
                        VALUES (?, ?, ?, ?, ?, ?, 'Active');
                        SELECT @@IDENTITY", con))
					{
						cmd.Parameters.AddWithValue("?", empCode);
						cmd.Parameters.AddWithValue("?", fullName);
						cmd.Parameters.AddWithValue("?", email);
						cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone);
						cmd.Parameters.AddWithValue("?", departmentId);
						cmd.Parameters.AddWithValue("?", DateTime.Today);

						int newEmpId = Convert.ToInt32(cmd.ExecuteScalar());

						// Insert User account
						using (var cmd2 = new OdbcCommand(@"
                            INSERT INTO [User] (username, password_hash, employee_id, user_role, is_active, created_date)
                            VALUES (?, ?, ?, 'Employee', 1, ?)", con))
						{
							cmd2.Parameters.AddWithValue("?", username);
							cmd2.Parameters.AddWithValue("?", HashPassword(password));
							cmd2.Parameters.AddWithValue("?", newEmpId);
							cmd2.Parameters.AddWithValue("?", DateTime.Now);
							cmd2.ExecuteNonQuery();
						}
					}

					return "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				return "Database error: " + ex.Message;
			}
		}

		private string HashPassword(string password)
		{
			using (var sha = System.Security.Cryptography.SHA256.Create())
			{
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
				byte[] hash = sha.ComputeHash(bytes);
				return Convert.ToBase64String(hash);
			}
		}

		// ─────────────────────────────────────────────────────────
		//  HELPER METHODS
		// ─────────────────────────────────────────────────────────

		private Panel MakeHeader(string title, string subtitle)
		{
			var strip = new Panel { Size = new Size(344, 52), Location = new Point(0, 0), BackColor = Blue };
			strip.Controls.Add(new Label { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 11f, FontStyle.Bold), AutoSize = true, Location = new Point(16, 10) });
			strip.Controls.Add(new Label { Text = subtitle, ForeColor = Color.FromArgb(180, 210, 255), Font = new Font("Segoe UI", 8f), AutoSize = true, Location = new Point(16, 32) });
			return strip;
		}

		private Label MakeLabel(string text, Font font, Color color, Point loc)
		{
			return new Label { Text = text, Font = font ?? new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = color, AutoSize = true, Location = loc };
		}

		private TextBox MakeTextBox(string name, Point loc, int width, bool isPassword = false)
		{
			var t = new TextBox
			{
				Name = name,
				Size = new Size(width, 28),
				Location = loc,
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				BackColor = InputBg
			};
			if (isPassword)
			{
				t.PasswordChar = '●';
				t.UseSystemPasswordChar = true;
			}
			return t;
		}

		private TextBox AddField(Panel panel, string label, ref int y, int width, bool isPassword = false)
		{
			panel.Controls.Add(MakeLabel(label, null, LabelFg, new Point(24, y)));
			var txt = MakeTextBox("", new Point(24, y + 18), width, isPassword);
			panel.Controls.Add(txt);
			y += 56;
			return txt;
		}

		private Button MakeEyeBtn(Point loc, TextBox target)
		{
			var btn = new Button
			{
				Size = new Size(30, 28),
				Location = loc,
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.Transparent,
				Text = "👁",
				Font = new Font("Segoe UI", 11f),
				Cursor = Cursors.Hand
			};
			btn.FlatAppearance.BorderSize = 0;
			bool visible = false;
			btn.Click += (s, e) =>
			{
				visible = !visible;
				target.PasswordChar = visible ? '\0' : '●';
				target.UseSystemPasswordChar = !visible;
				btn.Text = visible ? "🙈" : "👁";
			};
			return btn;
		}

		private Button MakePrimaryBtn(string text, Color bg, Point loc, int width)
		{
			var b = new Button
			{
				Text = text,
				Size = new Size(width, 38),
				Location = loc,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				FlatStyle = FlatStyle.Flat,
				BackColor = bg,
				ForeColor = Color.White,
				Cursor = Cursors.Hand
			};
			b.FlatAppearance.BorderSize = 0;
			return b;
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				return new System.Net.Mail.MailAddress(email).Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}