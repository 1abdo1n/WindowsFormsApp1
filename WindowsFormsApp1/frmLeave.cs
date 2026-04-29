using System;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmLeave : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color Approved = Color.FromArgb(21, 128, 61);
		private readonly Color Pending = Color.FromArgb(180, 83, 9);
		private readonly Color Rejected = Color.FromArgb(185, 28, 28);
		private readonly Color ApprovedBg = Color.FromArgb(240, 253, 244);
		private readonly Color PendingBg = Color.FromArgb(255, 251, 235);
		private readonly Color RejectedBg = Color.FromArgb(254, 242, 242);

		private DataGridView dgv;
		private ComboBox cmbStatus;
		private TextBox txtSearch;
		private Label lblPending, lblApproved, lblRejected, lblTotal;

		private readonly string userDept;
		private readonly string userId;
		private readonly string userRole;

		public frmLeave(string department = "", string employeeId = "", string role = "Employee")
		{
			userDept = department;
			userId = employeeId;
			userRole = role;

			Text = string.IsNullOrEmpty(department) ? "Leave Management" : $"Leave Management - {department} Department";
			Size = new Size(1100, 680);
			MinimumSize = new Size(900, 500);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadData();
		}

		private void BuildUI()
		{
			// Top bar
			Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
			topbar.Controls.Add(new Label
			{
				Text = Text,
				Font = new Font("Segoe UI", 13f, FontStyle.Bold),
				ForeColor = TxtPrimary,
				AutoSize = true,
				Location = new Point(20, 14)
			});
			var btnBack = new Button
			{
				Text = "← Back",
				Size = new Size(80, 30),
				FlatStyle = FlatStyle.Flat,
				BackColor = BgCard,
				ForeColor = TxtSec,
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnBack.FlatAppearance.BorderColor = Border;
			btnBack.Click += (s, e) => Close();
			topbar.Controls.Add(btnBack);
			topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

			// Stats bar
			Panel statsBar = new Panel
			{
				Dock = DockStyle.Top,
				Height = 80,
				BackColor = BgPage,
				Padding = new Padding(16, 8, 16, 8)
			};
			var statsFlow = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				WrapContents = false,
				BackColor = BgPage
			};

			Label MakeCard(string lbl, Color clr, ref Label valLabel)
			{
				var card = new Panel
				{
					Size = new Size(155, 62),
					BackColor = BgCard,
					Margin = new Padding(0, 0, 10, 0)
				};
				card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
				valLabel = new Label
				{
					Text = "0",
					Font = new Font("Segoe UI", 18f, FontStyle.Bold),
					ForeColor = clr,
					AutoSize = true,
					Location = new Point(12, 6)
				};
				card.Controls.Add(valLabel);
				card.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8.5f),
					ForeColor = TxtSec,
					AutoSize = true,
					Location = new Point(12, 38)
				});
				statsFlow.Controls.Add(card);
				return valLabel;
			}

			MakeCard("Pending", Pending, ref lblPending);
			MakeCard("Approved", Approved, ref lblApproved);
			MakeCard("Rejected", Rejected, ref lblRejected);

			if (userRole != "Employee")
				MakeCard("Total Requests", Blue, ref lblTotal);

			statsBar.Controls.Add(statsFlow);

			// Toolbar
			Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

			txtSearch = new TextBox
			{
				Size = new Size(160, 28),
				Location = new Point(20, 12),
				Font = new Font("Segoe UI", 9.5f),
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = TxtSec,
				Text = "Search employee..."
			};
			txtSearch.GotFocus += (s, e) =>
			{
				if (txtSearch.Text == "Search employee...")
				{
					txtSearch.Text = "";
					txtSearch.ForeColor = TxtPrimary;
				}
			};
			txtSearch.LostFocus += (s, e) =>
			{
				if (txtSearch.Text == "")
				{
					txtSearch.Text = "Search employee...";
					txtSearch.ForeColor = TxtSec;
				}
			};
			txtSearch.TextChanged += (s, e) => LoadData();

			toolbar.Controls.Add(new Label
			{
				Text = "Status:",
				AutoSize = true,
				Location = new Point(192, 17),
				ForeColor = TxtSec
			});

			cmbStatus = new ComboBox
			{
				Size = new Size(120, 28),
				Location = new Point(235, 12),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 9f)
			};
			cmbStatus.Items.AddRange(new[] { "All", "Pending", "Approved", "Rejected" });
			cmbStatus.SelectedIndex = 0;
			cmbStatus.SelectedIndexChanged += (s, e) => LoadData();

			var btnRefresh = new Button
			{
				Text = "⟳ Refresh",
				Size = new Size(90, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = BgCard,
				ForeColor = TxtSec,
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnRefresh.FlatAppearance.BorderColor = Border;
			btnRefresh.Click += (s, e) => LoadData();

			var btnNew = new Button
			{
				Text = "+ New Request",
				Size = new Size(120, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = Blue,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnNew.FlatAppearance.BorderSize = 0;
			btnNew.Click += (s, e) =>
			{
				var dlg = new frmLeaveAdd(userId);
				if (dlg.ShowDialog() == DialogResult.OK)
					LoadData();
			};

			toolbar.Controls.Add(txtSearch);
			toolbar.Controls.Add(cmbStatus);
			toolbar.Controls.Add(btnRefresh);
			toolbar.Controls.Add(btnNew);

			// Approve/Reject buttons for Admin and Manager only
			if (userRole != "Employee")
			{
				var btnApprove = new Button
				{
					Text = "✓ Approve",
					Size = new Size(95, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = ApprovedBg,
					ForeColor = Approved,
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnApprove.FlatAppearance.BorderColor = Color.FromArgb(134, 239, 172);
				btnApprove.Click += BtnApprove_Click;

				var btnReject = new Button
				{
					Text = "✗ Reject",
					Size = new Size(85, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = RejectedBg,
					ForeColor = Rejected,
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnReject.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
				btnReject.Click += BtnReject_Click;

				toolbar.Controls.Add(btnApprove);
				toolbar.Controls.Add(btnReject);
				toolbar.Resize += (s, e) =>
				{
					btnNew.Location = new Point(toolbar.Width - 135, 10);
					btnReject.Location = new Point(toolbar.Width - 230, 10);
					btnApprove.Location = new Point(toolbar.Width - 335, 10);
					btnRefresh.Location = new Point(toolbar.Width - 435, 10);
				};
			}
			else
			{
				toolbar.Resize += (s, e) =>
				{
					btnNew.Location = new Point(toolbar.Width - 135, 10);
					btnRefresh.Location = new Point(toolbar.Width - 235, 10);
				};
			}

			// DataGridView
			dgv = new DataGridView
			{
				Dock = DockStyle.Fill,
				BackgroundColor = BgCard,
				BorderStyle = BorderStyle.None,
				ColumnHeadersHeight = 36,
				RowTemplate = { Height = 34 },
				AllowUserToAddRows = false,
				ReadOnly = true,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				GridColor = Border
			};
			dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSec;
			dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
			dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
			dgv.DefaultCellStyle.ForeColor = TxtPrimary;
			dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);

			dgv.Columns.Add("leave_id", "Leave ID");
			dgv.Columns["leave_id"].FillWeight = 60;
			dgv.Columns.Add("employee_id", "Emp ID");
			dgv.Columns["employee_id"].FillWeight = 55;
			dgv.Columns.Add("full_name", "Employee");
			dgv.Columns.Add("leave_type", "Leave Type");
			dgv.Columns["leave_type"].FillWeight = 75;
			dgv.Columns.Add("start_date", "Start Date");
			dgv.Columns["start_date"].FillWeight = 80;
			dgv.Columns.Add("end_date", "End Date");
			dgv.Columns["end_date"].FillWeight = 80;
			dgv.Columns.Add("days", "Days");
			dgv.Columns["days"].FillWeight = 45;
			dgv.Columns.Add("approval_status", "Status");
			dgv.Columns["approval_status"].FillWeight = 70;
			dgv.Columns.Add("approved_by_name", "Approved By");
			dgv.Columns["approved_by_name"].FillWeight = 80;
			dgv.Columns.Add("notes", "Notes");
			dgv.Columns["notes"].FillWeight = 100;

			dgv.CellFormatting += Dgv_CellFormatting;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		private void LoadData()
		{
			dgv.Rows.Clear();

			string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.Trim();
			string statusFilter = cmbStatus.SelectedItem?.ToString() ?? "All";

			string sql = @"
                SELECT 
                    l.leave_id,
                    l.employee_id,
                    e.full_name,
                    l.leave_type,
                    CONVERT(VARCHAR, l.start_date, 103) AS start_date_display,
                    CONVERT(VARCHAR, l.end_date, 103) AS end_date_display,
                    DATEDIFF(day, l.start_date, l.end_date) + 1 AS days,
                    l.approval_status,
                    l.notes,
                    apr.full_name AS approved_by_name
                FROM [Leave] l
                LEFT JOIN Employee e ON l.employee_id = e.employee_id
                LEFT JOIN Employee apr ON l.approved_by = apr.employee_id
                LEFT JOIN Department d ON e.department_id = d.department_id
                WHERE 
                    (? = 'All' OR l.approval_status = ?)
                    AND (? = '' 
                        OR e.full_name LIKE ?
                        OR CAST(l.employee_id AS VARCHAR) LIKE ?)
                    AND (? = '' OR d.department_name = ?)
                    AND (? = '' OR CAST(l.employee_id AS VARCHAR) = ?)
                ORDER BY l.start_date DESC";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(sql, con))
				{
					string searchPattern = "%" + search + "%";
					cmd.Parameters.AddWithValue("?", statusFilter);
					cmd.Parameters.AddWithValue("?", statusFilter);
					cmd.Parameters.AddWithValue("?", search);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");

					con.Open();

					int pending = 0, approved = 0, rejected = 0;

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						string status = reader["approval_status"]?.ToString() ?? "Pending";
						if (status == "Pending") pending++;
						else if (status == "Approved") approved++;
						else if (status == "Rejected") rejected++;

						dgv.Rows.Add(
							reader["leave_id"].ToString(),
							reader["employee_id"].ToString(),
							reader["full_name"]?.ToString() ?? "—",
							reader["leave_type"]?.ToString() ?? "—",
							reader["start_date_display"].ToString(),
							reader["end_date_display"].ToString(),
							reader["days"].ToString(),
							status,
							reader["approved_by_name"]?.ToString() ?? "—",
							reader["notes"]?.ToString() ?? ""
						);
					}

					lblPending.Text = pending.ToString();
					lblApproved.Text = approved.ToString();
					lblRejected.Text = rejected.ToString();
					if (lblTotal != null)
						lblTotal.Text = (pending + approved + rejected).ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnApprove_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null)
			{
				MessageBox.Show("Please select a leave request first.", "Approve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["leave_id"].Value?.ToString(), out int leaveId))
				return;

			string currentStatus = dgv.CurrentRow.Cells["approval_status"].Value?.ToString() ?? "";
			if (currentStatus != "Pending")
			{
				MessageBox.Show($"This request is already {currentStatus}.", "Cannot Approve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string empName = dgv.CurrentRow.Cells["full_name"].Value?.ToString() ?? "employee";

			if (MessageBox.Show($"Approve {empName}'s leave request?", "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    UPDATE [Leave] 
                    SET approval_status = 'Approved', approved_by = ? 
                    WHERE leave_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(userId) ? 1 : Convert.ToInt32(userId));
					cmd.Parameters.AddWithValue("?", leaveId);
					con.Open();
					cmd.ExecuteNonQuery();
				}

				MessageBox.Show("Leave request approved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadData();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error approving request:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnReject_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null)
			{
				MessageBox.Show("Please select a leave request first.", "Reject", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["leave_id"].Value?.ToString(), out int leaveId))
				return;

			string currentStatus = dgv.CurrentRow.Cells["approval_status"].Value?.ToString() ?? "";
			if (currentStatus != "Pending")
			{
				MessageBox.Show($"This request is already {currentStatus}.", "Cannot Reject", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string empName = dgv.CurrentRow.Cells["full_name"].Value?.ToString() ?? "employee";

			if (MessageBox.Show($"Reject {empName}'s leave request?", "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    UPDATE [Leave] 
                    SET approval_status = 'Rejected', approved_by = ? 
                    WHERE leave_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(userId) ? 1 : Convert.ToInt32(userId));
					cmd.Parameters.AddWithValue("?", leaveId);
					con.Open();
					cmd.ExecuteNonQuery();
				}

				MessageBox.Show("Leave request rejected.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadData();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error rejecting request:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var row = dgv.Rows[e.RowIndex];
			if (row.Cells["approval_status"].Value == null) return;
			if (row.Selected) return;

			string status = row.Cells["approval_status"].Value.ToString();

			if (status == "Approved")
				row.DefaultCellStyle.BackColor = ApprovedBg;
			else if (status == "Pending")
				row.DefaultCellStyle.BackColor = PendingBg;
			else if (status == "Rejected")
				row.DefaultCellStyle.BackColor = RejectedBg;
			else
				row.DefaultCellStyle.BackColor = Color.White;
		}
	}

	// ============================================================
	//  ADD LEAVE REQUEST FORM
	// ============================================================
	public class frmLeaveAdd : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private readonly string _employeeId;

		private TextBox txtEmpId;
		private ComboBox cmbLeaveType;
		private DateTimePicker dtpStart, dtpEnd;
		private TextBox txtNotes;
		private Label lblDaysCalc;

		public frmLeaveAdd(string employeeId = "")
		{
			_employeeId = employeeId;

			Text = "New Leave Request";
			Size = new Size(450, 520);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
		}

		private void BuildUI()
		{
			var pnl = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				Padding = new Padding(20, 16, 20, 10),
				BackColor = BgPage
			};
			int y = 0;

			void AddRow(string lbl, Control ctrl)
			{
				pnl.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
					ForeColor = Color.FromArgb(55, 65, 81),
					AutoSize = true,
					Location = new Point(0, y)
				});
				ctrl.Location = new Point(0, y + 18);
				ctrl.Size = new Size(390, 28);
				pnl.Controls.Add(ctrl);
				y += 56;
			}

			// Employee ID
			txtEmpId = new TextBox
			{
				BorderStyle = BorderStyle.FixedSingle,
				Font = new Font("Segoe UI", 10f),
				Text = _employeeId,
				ReadOnly = !string.IsNullOrEmpty(_employeeId)
			};
			AddRow("Employee ID *", txtEmpId);

			// Leave Type
			cmbLeaveType = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f)
			};
			cmbLeaveType.Items.AddRange(new[] { "Annual", "Sick", "Emergency", "Maternity", "Paternity", "Unpaid" });
			cmbLeaveType.SelectedIndex = 0;
			AddRow("Leave Type *", cmbLeaveType);

			// Start Date
			dtpStart = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today };
			dtpStart.ValueChanged += (s, e) => CalculateDays();
			AddRow("Start Date *", dtpStart);

			// End Date
			dtpEnd = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today.AddDays(1) };
			dtpEnd.ValueChanged += (s, e) => CalculateDays();
			AddRow("End Date *", dtpEnd);

			// Days calculation
			lblDaysCalc = new Label
			{
				Text = "Days: 1 day(s)",
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = Blue,
				AutoSize = true,
				Location = new Point(0, y)
			};
			pnl.Controls.Add(lblDaysCalc);
			y += 24;

			// Notes
			pnl.Controls.Add(new Label
			{
				Text = "Notes",
				Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
				ForeColor = Color.FromArgb(55, 65, 81),
				AutoSize = true,
				Location = new Point(0, y)
			});
			txtNotes = new TextBox
			{
				Size = new Size(390, 60),
				Location = new Point(0, y + 18),
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				Multiline = true
			};
			pnl.Controls.Add(txtNotes);
			y += 80;

			// Buttons
			var btnSubmit = new Button
			{
				Text = "Submit Request",
				Size = new Size(170, 38),
				Location = new Point(0, y),
				FlatStyle = FlatStyle.Flat,
				BackColor = Blue,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnSubmit.FlatAppearance.BorderSize = 0;
			btnSubmit.Click += BtnSubmit_Click;

			var btnCancel = new Button
			{
				Text = "Cancel",
				Size = new Size(90, 38),
				Location = new Point(185, y),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.White,
				ForeColor = TxtSec,
				Cursor = Cursors.Hand
			};
			btnCancel.FlatAppearance.BorderColor = Border;
			btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

			pnl.Controls.Add(btnSubmit);
			pnl.Controls.Add(btnCancel);
			Controls.Add(pnl);
		}

		private void CalculateDays()
		{
			if (dtpEnd.Value < dtpStart.Value)
			{
				lblDaysCalc.Text = "Days: End date cannot be before start date!";
				lblDaysCalc.ForeColor = Color.FromArgb(185, 28, 28);
				return;
			}

			int days = (dtpEnd.Value - dtpStart.Value).Days + 1;
			lblDaysCalc.Text = $"Days: {days} day(s)";
			lblDaysCalc.ForeColor = Blue;
		}

		private void BtnSubmit_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtEmpId.Text))
			{
				MessageBox.Show("Employee ID is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(txtEmpId.Text.Trim(), out int empId))
			{
				MessageBox.Show("Employee ID must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (dtpEnd.Value < dtpStart.Value)
			{
				MessageBox.Show("End date cannot be before start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int days = (dtpEnd.Value - dtpStart.Value).Days + 1;
			string leaveType = cmbLeaveType.SelectedItem?.ToString() ?? "Annual";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();

					// Check if employee exists
					using (var chk = new OdbcCommand("SELECT COUNT(1) FROM Employee WHERE employee_id = ?", con))
					{
						chk.Parameters.AddWithValue("?", empId);
						if (Convert.ToInt32(chk.ExecuteScalar()) == 0)
						{
							MessageBox.Show($"Employee with ID {empId} does not exist.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					// Check for overlapping leave requests
					using (var chk2 = new OdbcCommand(@"
                        SELECT COUNT(1) FROM [Leave] 
                        WHERE employee_id = ? 
                          AND approval_status IN ('Pending', 'Approved')
                          AND ((start_date BETWEEN ? AND ?) 
                            OR (end_date BETWEEN ? AND ?)
                            OR (? BETWEEN start_date AND end_date))
                        ", con))
					{
						chk2.Parameters.AddWithValue("?", empId);
						chk2.Parameters.AddWithValue("?", dtpStart.Value.Date);
						chk2.Parameters.AddWithValue("?", dtpEnd.Value.Date);
						chk2.Parameters.AddWithValue("?", dtpStart.Value.Date);
						chk2.Parameters.AddWithValue("?", dtpEnd.Value.Date);
						chk2.Parameters.AddWithValue("?", dtpStart.Value.Date);
						if (Convert.ToInt32(chk2.ExecuteScalar()) > 0)
						{
							MessageBox.Show("You already have an overlapping leave request.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					string sql = @"
                        INSERT INTO [Leave] 
                            (employee_id, leave_type, start_date, end_date, approval_status, notes)
                        VALUES (?, ?, ?, ?, 'Pending', ?)";

					using (var cmd = new OdbcCommand(sql, con))
					{
						cmd.Parameters.AddWithValue("?", empId);
						cmd.Parameters.AddWithValue("?", leaveType);
						cmd.Parameters.AddWithValue("?", dtpStart.Value.Date);
						cmd.Parameters.AddWithValue("?", dtpEnd.Value.Date);
						cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
						cmd.ExecuteNonQuery();
					}

					MessageBox.Show($"Leave request submitted successfully!\n{days} day(s) requested.", "Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error submitting request:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}