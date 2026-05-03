using System;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmMainNew : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color SidebarBg = Color.FromArgb(15, 23, 42);
		private readonly Color BorderClr = Color.FromArgb(209, 213, 219);
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtGray = Color.FromArgb(107, 114, 128);

		private Panel content;
		private FlowLayoutPanel statsRow;
		private Panel deptCard;
		private bool isLoggingOut = false;

		public frmMainNew()
		{
			Text = Global.AppName + " — Dashboard";
			Size = new Size(1100, 700);
			MinimumSize = new Size(900, 600);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			FormClosing += (s, e) => { if (!isLoggingOut) Application.Exit(); };

			BuildUI();
			LoadDashboardData();
		}

		// ════════════════════════════════════════════════════════════════════
		//  UI Builder
		// ════════════════════════════════════════════════════════════════════
		private void BuildUI()
		{
			// ── SIDEBAR ──────────────────────────────────────────────────────
			Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 250, BackColor = SidebarBg };

			sidebar.Controls.Add(MakeLabel(Global.AppName, new Font("Segoe UI", 13f, FontStyle.Bold), Color.White, new Point(18, 22)));
			sidebar.Controls.Add(MakeLabel("Management Portal", new Font("Segoe UI", 8.5f), Color.FromArgb(100, 116, 139), new Point(18, 48)));
			sidebar.Controls.Add(new Panel { Size = new Size(250, 1), Location = new Point(0, 72), BackColor = Color.FromArgb(30, 41, 59) });

			string[] navItems;
			if (Global.IsAdmin)
				navItems = new[] { "Dashboard", "Employees", "Attendance", "Departments", "Contracts", "Payroll", "Leave", "Assets", "Reports", "Performance" };
			else if (Global.IsManager)
				navItems = new[] { "Dashboard", "Employees", "Attendance", "Leave", "Assets", "Reports" };
			else
				navItems = new[] { "Dashboard", "My Attendance", "My Leave", "My Contract" };

			int navY = 86;
			foreach (string item in navItems)
			{
				var btn = new Button
				{
					Text = item,
					Size = new Size(250, 38),
					Location = new Point(0, navY),
					FlatStyle = FlatStyle.Flat,
					BackColor = Color.Transparent,
					ForeColor = Color.FromArgb(148, 163, 184),
					Font = new Font("Segoe UI", 9.5f),
					TextAlign = ContentAlignment.MiddleLeft,
					Padding = new Padding(18, 0, 0, 0),
					Cursor = Cursors.Hand,
					Tag = item
				};
				btn.FlatAppearance.BorderSize = 0;
				btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
				btn.Click += NavBtn_Click;
				sidebar.Controls.Add(btn);
				navY += 40;
			}

			var btnLogout = new Button
			{
				Text = "⇠  Logout",
				Size = new Size(226, 34),
				Location = new Point(12, sidebar.ClientSize.Height - 50),
				Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.Transparent,
				ForeColor = Color.FromArgb(148, 163, 184),
				Font = new Font("Segoe UI", 9f),
				TextAlign = ContentAlignment.MiddleLeft,
				Padding = new Padding(12, 0, 0, 0),
				Cursor = Cursors.Hand
			};
			btnLogout.FlatAppearance.BorderColor = Color.FromArgb(51, 65, 85);
			btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 29, 29);
			btnLogout.Click += (s, e) => DoLogout();
			sidebar.Controls.Add(btnLogout);

			// ── TOP BAR ───────────────────────────────────────────────────────
			Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderClr), 0, 51, topbar.Width, 51);
			topbar.Resize += (s, e) => topbar.Invalidate();

			var lblPageTitle = MakeLabel("Dashboard", new Font("Segoe UI", 13f, FontStyle.Bold), TxtPrimary, new Point(20, 14));
			var lblUserInfo = new Label
			{
				Text = Global.LoggedUserFullName + " | " + Global.LoggedUserRole,
				Font = new Font("Segoe UI", 8.5f),
				ForeColor = TxtGray,
				AutoSize = true
			};
			topbar.Controls.Add(lblPageTitle);
			topbar.Controls.Add(lblUserInfo);
			lblUserInfo.Location = new Point(topbar.ClientSize.Width - lblUserInfo.Width - 20, 17);
			lblUserInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			// ── MAIN CONTENT ──────────────────────────────────────────────────
			content = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = BgPage,
				Padding = new Padding(20, 16, 20, 16),
				AutoScroll = true
			};

			content.Controls.Add(MakeLabel($"Welcome back, {Global.LoggedUserFullName}!", new Font("Segoe UI", 15f, FontStyle.Bold), TxtPrimary, new Point(0, 0)));
			content.Controls.Add(MakeLabel(DateTime.Now.ToString("dddd, MMMM dd, yyyy"), new Font("Segoe UI", 9.5f), TxtGray, new Point(0, 32)));

			statsRow = new FlowLayoutPanel
			{
				Location = new Point(0, 62),
				Height = 90,
				Width = content.ClientSize.Width - 40,
				WrapContents = false,
				AutoScroll = false,
				BackColor = BgPage
			};
			content.Controls.Add(statsRow);

			string sectionTitle = Global.IsAdmin ? "DEPARTMENT OVERVIEW" : (Global.IsManager ? "MY TEAM" : "MY INFO");
			content.Controls.Add(new Label
			{
				Text = sectionTitle,
				Font = new Font("Segoe UI", 8f, FontStyle.Bold),
				ForeColor = TxtGray,
				AutoSize = true,
				Location = new Point(0, 164)
			});

			deptCard = new Panel
			{
				Location = new Point(0, 186),
				BackColor = BgCard,
				Width = content.ClientSize.Width - 40
			};
			deptCard.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(BorderClr), 0, 0, deptCard.Width - 1, deptCard.Height - 1);
			deptCard.Resize += (s, e) => deptCard.Invalidate();
			content.Controls.Add(deptCard);

			content.Resize += (s, e) =>
			{
				statsRow.Width = content.ClientSize.Width - 40;
				deptCard.Width = content.ClientSize.Width - 40;
			};

			Controls.Add(content);
			Controls.Add(topbar);
			Controls.Add(sidebar);
			sidebar.SendToBack();
		}

		// ════════════════════════════════════════════════════════════════════
		//  Navigation — ✅ الـ role بيتبعت صح لكل فورم
		// ════════════════════════════════════════════════════════════════════
		private void NavBtn_Click(object sender, EventArgs e)
		{
			string name = ((Button)sender).Tag?.ToString() ?? "";
			if (name == "Dashboard") return;

			Form frm = null;

			if (Global.IsAdmin)
			{
				switch (name)
				{
					case "Employees": frm = new frmEmployees(); break;
					case "Attendance": frm = new frmAttendance(); break;
					case "Departments": frm = new frmDepartments(); break;
					case "Contracts": frm = new frmContracts(); break;
					case "Payroll": frm = new frmPayroll(); break;
					case "Leave": frm = new frmLeave("", "", "Admin"); break;     // ✅ Admin
					case "Assets": frm = new frmAssets(); break;
					case "Reports": frm = new frmReports(); break;
					case "Performance": frm = new frmPerformance(); break;
				}
			}
			else if (Global.IsManager)
			{
				switch (name)
				{
					case "Employees": frm = new frmEmployees(Global.LoggedUserDepartment); break;
					case "Attendance": frm = new frmAttendance(Global.LoggedUserDepartment); break;
					case "Leave": frm = new frmLeave(Global.LoggedUserDepartment, "", "Manager"); break;  // ✅ Manager
					case "Assets": frm = new frmAssets(Global.LoggedUserDepartment); break;
					case "Reports": frm = new frmReports(Global.LoggedUserDepartment); break;
				}
			}
			else
			{
				switch (name)
				{
					case "My Attendance": frm = new frmAttendance("", Global.LoggedUserId.ToString()); break;
					case "My Leave": frm = new frmLeave("", Global.LoggedUserId.ToString(), "Employee"); break;  // ✅ Employee
					case "My Contract":
						MessageBox.Show("Contract Information will be loaded from database.", "My Contract");
						return;
				}
			}

			frm?.Show();
		}

		// ════════════════════════════════════════════════════════════════════
		//  Dashboard Data
		// ════════════════════════════════════════════════════════════════════
		private void LoadDashboardData()
		{
			statsRow.Controls.Clear();

			try
			{
				if (Global.IsAdmin)
				{
					int employeeCount = 0, departmentCount = 0, pendingLeaves = 0;
					decimal monthlyPayroll = 0;
					double avgAttendance = 0;

					using (var con = new OdbcConnection(Global.ConnStr))
					{
						con.Open();
						using (var cmd = new OdbcCommand("SELECT COUNT(*) FROM Employee", con))
							employeeCount = Convert.ToInt32(cmd.ExecuteScalar());
						using (var cmd = new OdbcCommand("SELECT COUNT(*) FROM Department", con))
							departmentCount = Convert.ToInt32(cmd.ExecuteScalar());
						using (var cmd = new OdbcCommand("SELECT ISNULL(SUM(net_salary),0) FROM Payroll WHERE MONTH(pay_date)=MONTH(GETDATE()) AND YEAR(pay_date)=YEAR(GETDATE())", con))
							monthlyPayroll = Convert.ToDecimal(cmd.ExecuteScalar());
						using (var cmd = new OdbcCommand(@"
                            SELECT ISNULL(AVG(CASE 
                                WHEN check_in_time IS NULL THEN 0
                                WHEN CAST(check_in_time AS TIME) <= '09:00' THEN 100
                                ELSE 70
                            END),0) FROM Attendance WHERE MONTH(date)=MONTH(GETDATE())", con))
							avgAttendance = Convert.ToDouble(cmd.ExecuteScalar());
						using (var cmd = new OdbcCommand("SELECT COUNT(*) FROM [Leave] WHERE approval_status='Pending'", con))
							pendingLeaves = Convert.ToInt32(cmd.ExecuteScalar());
					}

					AddStatCard("👥", employeeCount.ToString(), "Employees");
					AddStatCard("🏢", departmentCount.ToString(), "Departments");
					AddStatCard("💰", monthlyPayroll.ToString("N0") + " EGP", "Payroll");
					AddStatCard("📊", avgAttendance.ToString("F1") + "%", "Attendance");
					AddStatCard("📅", pendingLeaves.ToString(), "Pending Leaves");
					LoadDepartmentsOverview();
				}
				else if (Global.IsManager)
				{
					int teamCount = 0, pendingLeaves = 0, assetsCount = 0;
					double avgAttendance = 0;

					using (var con = new OdbcConnection(Global.ConnStr))
					{
						con.Open();
						using (var cmd = new OdbcCommand(@"
                            SELECT COUNT(*) FROM Employee e 
                            INNER JOIN Department d ON e.department_id=d.department_id 
                            WHERE d.department_name=?", con))
						{ cmd.Parameters.AddWithValue("?", Global.LoggedUserDepartment); teamCount = Convert.ToInt32(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand(@"
                            SELECT COUNT(*) FROM [Leave] l 
                            INNER JOIN Employee e ON l.employee_id=e.employee_id 
                            INNER JOIN Department d ON e.department_id=d.department_id 
                            WHERE d.department_name=? AND l.approval_status='Pending'", con))
						{ cmd.Parameters.AddWithValue("?", Global.LoggedUserDepartment); pendingLeaves = Convert.ToInt32(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand(@"
                            SELECT COUNT(*) FROM assets a 
                            LEFT JOIN Employee e ON a.employee_id=e.employee_id 
                            LEFT JOIN Department d ON e.department_id=d.department_id 
                            WHERE d.department_name=?", con))
						{ cmd.Parameters.AddWithValue("?", Global.LoggedUserDepartment); assetsCount = Convert.ToInt32(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand(@"
                            SELECT ISNULL(AVG(CASE 
                                WHEN check_in_time IS NULL THEN 0
                                WHEN CAST(check_in_time AS TIME) <= '09:00' THEN 100
                                ELSE 70
                            END),0) FROM Attendance a
                            INNER JOIN Employee e ON a.employee_id=e.employee_id
                            INNER JOIN Department d ON e.department_id=d.department_id
                            WHERE d.department_name=? AND MONTH(a.date)=MONTH(GETDATE())", con))
						{ cmd.Parameters.AddWithValue("?", Global.LoggedUserDepartment); avgAttendance = Convert.ToDouble(cmd.ExecuteScalar()); }
					}

					AddStatCard("👥", teamCount.ToString(), "My Team");
					AddStatCard("📊", avgAttendance.ToString("F1") + "%", "Attendance");
					AddStatCard("📅", pendingLeaves.ToString(), "Pending Leaves");
					AddStatCard("📦", assetsCount.ToString(), "Assets");
					LoadTeamOverview();
				}
				else
				{
					int empId = Global.LoggedUserID;
					int leaveDays = 0, pendingLeaves = 0;
					double avgAttendance = 0, avgPerformance = 0;
					decimal salary = 0;

					using (var con = new OdbcConnection(Global.ConnStr))
					{
						con.Open();
						using (var cmd = new OdbcCommand(@"
                            SELECT ISNULL(SUM(DATEDIFF(day,start_date,end_date)+1),0) FROM [Leave] 
                            WHERE employee_id=? AND approval_status='Approved' AND YEAR(start_date)=YEAR(GETDATE())", con))
						{ cmd.Parameters.AddWithValue("?", empId); leaveDays = Math.Max(0, 22 - Convert.ToInt32(cmd.ExecuteScalar())); }

						using (var cmd = new OdbcCommand("SELECT COUNT(*) FROM [Leave] WHERE employee_id=? AND approval_status='Pending'", con))
						{ cmd.Parameters.AddWithValue("?", empId); pendingLeaves = Convert.ToInt32(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand(@"
                            SELECT ISNULL(AVG(CASE 
                                WHEN check_in_time IS NULL THEN 0
                                WHEN CAST(check_in_time AS TIME) <= '09:00' THEN 100
                                ELSE 70
                            END),0) FROM Attendance WHERE employee_id=? AND MONTH(date)=MONTH(GETDATE())", con))
						{ cmd.Parameters.AddWithValue("?", empId); avgAttendance = Convert.ToDouble(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand(@"
                            SELECT ISNULL(AVG(final_rating),0) FROM Performance_Evaluation 
                            WHERE employee_id=? AND evaluation_year=YEAR(GETDATE())", con))
						{ cmd.Parameters.AddWithValue("?", empId); avgPerformance = Convert.ToDouble(cmd.ExecuteScalar()); }

						using (var cmd = new OdbcCommand("SELECT TOP 1 net_salary FROM Payroll WHERE employee_id=? ORDER BY pay_date DESC", con))
						{
							cmd.Parameters.AddWithValue("?", empId);
							var result = cmd.ExecuteScalar();
							if (result != DBNull.Value) salary = Convert.ToDecimal(result);
						}
					}

					AddStatCard("📅", leaveDays.ToString(), "Vacation Days Left");
					AddStatCard("📊", avgAttendance.ToString("F1") + "%", "Attendance");
					AddStatCard("⭐", avgPerformance.ToString("F1") + "/100", "Performance");
					AddStatCard("💰", salary.ToString("N0") + " EGP", "Last Salary");
					LoadEmployeeInfo();
				}
			}
			catch (Exception ex)
			{
				if (Global.IsAdmin)
				{
					AddStatCard("👥", "0", "Employees");
					AddStatCard("🏢", "0", "Departments");
					AddStatCard("💰", "0 EGP", "Payroll");
					AddStatCard("📊", "0%", "Attendance");
				}
				else if (Global.IsManager)
				{
					AddStatCard("👥", "0", "My Team");
					AddStatCard("📊", "0%", "Attendance");
					AddStatCard("📅", "0", "Pending Leaves");
				}
				else
				{
					AddStatCard("📅", "0", "Vacation Days Left");
					AddStatCard("📊", "0%", "Attendance");
					AddStatCard("⭐", "0", "Performance");
					AddStatCard("💰", "0 EGP", "Salary");
				}
				System.Diagnostics.Debug.WriteLine("Error loading dashboard data: " + ex.Message);
			}
		}

		private void AddStatCard(string icon, string value, string label)
		{
			var card = new Panel { Size = new Size(175, 80), BackColor = BgCard, Margin = new Padding(0, 0, 12, 0) };
			card.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(BorderClr), 0, 0, card.Width - 1, card.Height - 1);
			card.Controls.Add(MakeLabel(icon, new Font("Segoe UI", 22f), TxtPrimary, new Point(10, 22)));
			card.Controls.Add(MakeLabel(value, new Font("Segoe UI", 15f, FontStyle.Bold), Blue, new Point(68, 14)));
			card.Controls.Add(MakeLabel(label, new Font("Segoe UI", 8.5f), TxtGray, new Point(68, 48)));
			statsRow.Controls.Add(card);
		}

		// ════════════════════════════════════════════════════════════════════
		//  Department / Team / Employee info panels
		// ════════════════════════════════════════════════════════════════════
		private void LoadDepartmentsOverview()
		{
			deptCard.Controls.Clear();
			deptCard.Controls.Add(MakeLabel("Department", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 12)));
			var lblCol2 = MakeLabel("Employees", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(deptCard.Width - 200, 12));
			lblCol2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			deptCard.Controls.Add(lblCol2);

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT d.department_name, COUNT(e.employee_id) AS emp_count
                    FROM Department d
                    LEFT JOIN Employee e ON d.department_id=e.department_id
                    GROUP BY d.department_name, d.department_id
                    ORDER BY d.department_name", con))
				{
					con.Open();
					int i = 0;
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var row = new Panel
							{
								Location = new Point(1, 48 + i * 38),
								Height = 38,
								BackColor = i % 2 == 0 ? BgCard : Color.FromArgb(249, 250, 251),
								Width = deptCard.Width - 2,
								Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
							};
							var v2 = MakeLabel(reader["emp_count"].ToString(), new Font("Segoe UI", 9.5f, FontStyle.Bold), Blue, new Point(row.Width - 200, 10));
							v2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							row.Controls.Add(MakeLabel(reader["department_name"].ToString(), new Font("Segoe UI", 9.5f), TxtPrimary, new Point(20, 10)));
							row.Controls.Add(v2);
							deptCard.Controls.Add(row);
							i++;
						}
					}
					deptCard.Height = 48 + i * 38 + 15;
				}
			}
			catch (Exception ex)
			{
				deptCard.Controls.Add(MakeLabel("Error: " + ex.Message, new Font("Segoe UI", 9.5f), Color.Red, new Point(16, 48)));
				deptCard.Height = 100;
			}
		}

		private void LoadTeamOverview()
		{
			deptCard.Controls.Clear();
			deptCard.Controls.Add(MakeLabel("Team Member", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 12)));
			var lblCol2 = MakeLabel("Position", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(deptCard.Width - 200, 12));
			lblCol2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			deptCard.Controls.Add(lblCol2);

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT e.full_name, e.job_title
                    FROM Employee e
                    INNER JOIN Department d ON e.department_id=d.department_id
                    WHERE d.department_name=? AND e.employee_id!=?
                    ORDER BY e.full_name", con))
				{
					cmd.Parameters.AddWithValue("?", Global.LoggedUserDepartment);
					cmd.Parameters.AddWithValue("?", Global.LoggedUserID);
					con.Open();
					int i = 0;
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var row = new Panel
							{
								Location = new Point(1, 48 + i * 38),
								Height = 38,
								BackColor = i % 2 == 0 ? BgCard : Color.FromArgb(249, 250, 251),
								Width = deptCard.Width - 2,
								Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
							};
							var v2 = MakeLabel(reader["job_title"]?.ToString() ?? "Staff", new Font("Segoe UI", 9.5f), TxtGray, new Point(row.Width - 200, 10));
							v2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							row.Controls.Add(MakeLabel(reader["full_name"].ToString(), new Font("Segoe UI", 9.5f), TxtPrimary, new Point(20, 10)));
							row.Controls.Add(v2);
							deptCard.Controls.Add(row);
							i++;
						}
					}
					deptCard.Height = 48 + i * 38 + 15;
				}
			}
			catch (Exception ex)
			{
				deptCard.Controls.Add(MakeLabel("Error: " + ex.Message, new Font("Segoe UI", 9.5f), Color.Red, new Point(16, 48)));
				deptCard.Height = 100;
			}
		}

		private void LoadEmployeeInfo()
		{
			deptCard.Controls.Clear();
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT e.full_name, d.department_name, e.job_title, e.hire_date
                    FROM Employee e
                    LEFT JOIN Department d ON e.department_id=d.department_id
                    WHERE e.employee_id=?", con))
				{
					cmd.Parameters.AddWithValue("?", Global.LoggedUserID);
					con.Open();
					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							deptCard.Controls.Add(MakeLabel("Name:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 20)));
							deptCard.Controls.Add(MakeLabel(reader["full_name"].ToString(), new Font("Segoe UI", 9.5f), TxtPrimary, new Point(130, 20)));
							deptCard.Controls.Add(MakeLabel("Department:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 55)));
							deptCard.Controls.Add(MakeLabel(reader["department_name"]?.ToString() ?? "—", new Font("Segoe UI", 9.5f), TxtPrimary, new Point(130, 55)));
							deptCard.Controls.Add(MakeLabel("Position:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 90)));
							deptCard.Controls.Add(MakeLabel(reader["job_title"]?.ToString() ?? "Staff", new Font("Segoe UI", 9.5f), TxtPrimary, new Point(130, 90)));
							deptCard.Controls.Add(MakeLabel("Hire Date:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(20, 125)));
							deptCard.Controls.Add(MakeLabel(Convert.ToDateTime(reader["hire_date"]).ToString("dd/MM/yyyy"), new Font("Segoe UI", 9.5f), TxtPrimary, new Point(130, 125)));
							deptCard.Height = 170;
						}
						else
						{
							deptCard.Controls.Add(MakeLabel("No data found", new Font("Segoe UI", 9.5f), TxtGray, new Point(20, 48)));
							deptCard.Height = 80;
						}
					}
				}
			}
			catch (Exception ex)
			{
				deptCard.Controls.Add(MakeLabel("Error: " + ex.Message, new Font("Segoe UI", 9.5f), Color.Red, new Point(20, 48)));
				deptCard.Height = 100;
			}
		}

		// ════════════════════════════════════════════════════════════════════
		//  Helpers
		// ════════════════════════════════════════════════════════════════════
		private Label MakeLabel(string text, Font font, Color color, Point loc)
			=> new Label { Text = text, Font = font, ForeColor = color, AutoSize = true, Location = loc };

		private void DoLogout()
		{
			if (MessageBox.Show("Are you sure you want to logout?", "Logout",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Global.ResetSession();
				isLoggingOut = true;
				new frmLogin().Show();
				Close();
			}
		}
	}
}