using System;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmAttendance : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
		private readonly Color DangerBg = Color.FromArgb(254, 242, 242);
		private readonly Color WarningBg = Color.FromArgb(255, 251, 235);

		private DataGridView dgv;
		private ComboBox cmbShift;
		private TextBox txtSearch;
		private DateTimePicker dtpDate;
		private CheckBox chkFilterDate;   // ✅ اختياري — لو مش مفعل يظهر كل السجلات
		private Label lblPresent, lblAbsent, lblLate, lblOnLeave, lblOvertime;

		private readonly string userDept;
		private readonly string userId;

		// ════════════════════════════════════════════════════════════════════
		//  Constructor
		// ════════════════════════════════════════════════════════════════════
		public frmAttendance(string department = "", string employeeId = "")
		{
			userDept = department;
			userId = employeeId;

			Text = string.IsNullOrEmpty(department) ? "Attendance" : $"Attendance - {department} Department";
			Size = new Size(1150, 680);
			MinimumSize = new Size(900, 540);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadData();
		}

		// ════════════════════════════════════════════════════════════════════
		//  UI Builder
		// ════════════════════════════════════════════════════════════════════
		private void BuildUI()
		{
			// ── Top bar ──────────────────────────────────────────────────────
			var topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
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

			// ── Toolbar ───────────────────────────────────────────────────────
			var toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

			// Search
			txtSearch = new TextBox
			{
				Size = new Size(150, 28),
				Location = new Point(20, 12),
				Font = new Font("Segoe UI", 9.5f),
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = TxtSec,
				Text = "Search employee..."
			};
			txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search employee...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
			txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search employee..."; txtSearch.ForeColor = TxtSec; } };
			txtSearch.TextChanged += (s, e) => LoadData();

			// ✅ Checkbox لتفعيل الفلتر بالتاريخ
			chkFilterDate = new CheckBox
			{
				Text = "Date:",
				AutoSize = true,
				Location = new Point(185, 16),
				ForeColor = TxtSec,
				Checked = false    // افتراضي: يظهر كل السجلات
			};
			chkFilterDate.CheckedChanged += (s, e) =>
			{
				dtpDate.Enabled = chkFilterDate.Checked;
				LoadData();
			};

			dtpDate = new DateTimePicker
			{
				Size = new Size(165, 28),
				Location = new Point(245, 12),
				Font = new Font("Segoe UI", 9f),
				Value = DateTime.Today,
				Enabled = false    // معطل حتى يضغط على الـ checkbox
			};
			dtpDate.ValueChanged += (s, e) => LoadData();

			// Shift filter
			toolbar.Controls.Add(new Label { Text = "Shift:", AutoSize = true, Location = new Point(425, 17), ForeColor = TxtSec });
			cmbShift = new ComboBox
			{
				Size = new Size(100, 28),
				Location = new Point(460, 12),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 9f)
			};
			cmbShift.Items.AddRange(new[] { "All", "Morning", "Evening", "Night" });
			cmbShift.SelectedIndex = 0;
			cmbShift.SelectedIndexChanged += (s, e) => LoadData();

			// Refresh
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

			toolbar.Controls.Add(txtSearch);
			toolbar.Controls.Add(chkFilterDate);
			toolbar.Controls.Add(dtpDate);
			toolbar.Controls.Add(cmbShift);
			toolbar.Controls.Add(btnRefresh);

			// Record Attendance button — للأدمن والمنجر فقط
			if (string.IsNullOrEmpty(userId))
			{
				var btnRecord = new Button
				{
					Text = "+ Record Attendance",
					Size = new Size(155, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = Blue,
					ForeColor = Color.White,
					Font = new Font("Segoe UI", 9f, FontStyle.Bold),
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnRecord.FlatAppearance.BorderSize = 0;
				btnRecord.Click += (s, e) =>
				{
					if (new frmAttendanceAdd().ShowDialog() == DialogResult.OK)
						LoadData();
				};
				toolbar.Controls.Add(btnRecord);
				toolbar.Resize += (s, e) =>
				{
					btnRecord.Location = new Point(toolbar.Width - 168, 10);
					btnRefresh.Location = new Point(toolbar.Width - 270, 10);
				};
			}
			else
			{
				toolbar.Resize += (s, e) => btnRefresh.Location = new Point(toolbar.Width - 105, 10);
			}

			// ── Stats bar ─────────────────────────────────────────────────────
			var statsBar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = BgPage, Padding = new Padding(16, 8, 16, 8) };
			var statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = BgPage };

			Label MakeCard(string lbl, Color clr, ref Label valLabel)
			{
				var card = new Panel { Size = new Size(155, 62), BackColor = BgCard, Margin = new Padding(0, 0, 10, 0) };
				card.Paint += (s2, e2) =>
					((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
				valLabel = new Label { Text = "0", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = clr, AutoSize = true, Location = new Point(12, 6) };
				card.Controls.Add(valLabel);
				card.Controls.Add(new Label { Text = lbl, Font = new Font("Segoe UI", 8.5f), ForeColor = TxtSec, AutoSize = true, Location = new Point(12, 38) });
				statsFlow.Controls.Add(card);
				return valLabel;
			}

			MakeCard("Present", Color.FromArgb(21, 128, 61), ref lblPresent);
			MakeCard("Absent", Color.FromArgb(185, 28, 28), ref lblAbsent);
			MakeCard("Late", Color.FromArgb(180, 83, 9), ref lblLate);
			MakeCard("On Leave", Color.FromArgb(29, 78, 216), ref lblOnLeave);
			if (string.IsNullOrEmpty(userId))
				MakeCard("Overtime", Color.FromArgb(109, 40, 217), ref lblOvertime);

			statsBar.Controls.Add(statsFlow);

			// ── DataGridView ──────────────────────────────────────────────────
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

			dgv.Columns.Add("attendance_id", "Att. ID"); dgv.Columns["attendance_id"].FillWeight = 55;
			dgv.Columns.Add("employee_id", "Emp ID"); dgv.Columns["employee_id"].FillWeight = 55;
			dgv.Columns.Add("employee_name", "Employee");
			dgv.Columns.Add("date", "Date"); dgv.Columns["date"].FillWeight = 75;
			dgv.Columns.Add("check_in", "Check In"); dgv.Columns["check_in"].FillWeight = 65;
			dgv.Columns.Add("check_out", "Check Out"); dgv.Columns["check_out"].FillWeight = 65;
			dgv.Columns.Add("shift", "Shift"); dgv.Columns["shift"].FillWeight = 65;
			dgv.Columns.Add("work_hours", "Work Hrs"); dgv.Columns["work_hours"].FillWeight = 60;
			dgv.Columns.Add("overtime_hours", "Overtime Hrs"); dgv.Columns["overtime_hours"].FillWeight = 65;
			dgv.Columns.Add("status", "Status"); dgv.Columns["status"].FillWeight = 65;

			dgv.CellFormatting += Dgv_CellFormatting;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		// ════════════════════════════════════════════════════════════════════
		//  Load Data  ✅ مصلح — يجيب كل السجلات أو يفلتر بالتاريخ لو اختار
		// ════════════════════════════════════════════════════════════════════
		private void LoadData()
		{
			dgv.Rows.Clear();

			string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.Trim();
			string shift = cmbShift.SelectedItem?.ToString() ?? "All";
			bool filterByDate = chkFilterDate.Checked;
			string dateFilter = dtpDate.Value.ToString("yyyy-MM-dd");

			// ✅ SQL يفلتر بالتاريخ فقط لو الـ checkbox مفعّل
			// ✅ department filter مضاف
			string sql = @"
                SELECT
                    a.attendance_id,
                    a.employee_id,
                    e.full_name                                          AS employee_name,
                    CONVERT(VARCHAR, a.date, 103)                        AS date_display,
                    ISNULL(CONVERT(VARCHAR(5), a.check_in_time,  108), '--') AS check_in,
                    ISNULL(CONVERT(VARCHAR(5), a.check_out_time, 108), '--') AS check_out,
                    ISNULL(a.shift_type, '—')                            AS shift,
                    ISNULL(CAST(a.work_hours     AS VARCHAR), '0')       AS work_hours,
                    ISNULL(CAST(a.overtime_hours AS VARCHAR), '0')       AS overtime_hours,
                    CASE
                        WHEN a.check_in_time IS NULL                           THEN 'Absent'
                        WHEN CAST(a.check_in_time AS TIME) > '09:00:00'        THEN 'Late'
                        ELSE 'Present'
                    END AS status
                FROM Attendance a
                LEFT JOIN Employee   e ON a.employee_id    = e.employee_id
                LEFT JOIN Department d ON e.department_id  = d.department_id
                WHERE
                    (? = 0  OR CONVERT(DATE, a.date) = ?)
                    AND (? = 'All' OR a.shift_type = ?)
                    AND (? = '' OR CAST(a.employee_id AS VARCHAR) = ?)
                    AND (? = '' OR d.department_name = ?)
                    AND (? = ''
                        OR e.full_name                        LIKE ?
                        OR CAST(a.employee_id AS VARCHAR)     LIKE ?)
                ORDER BY a.date DESC, e.full_name";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(sql, con))
				{
					string like = "%" + search + "%";
					int useDate = filterByDate ? 1 : 0;

					cmd.Parameters.AddWithValue("?", useDate);       // (? = 0  OR date = ?)
					cmd.Parameters.AddWithValue("?", dateFilter);    //
					cmd.Parameters.AddWithValue("?", shift);         // shift filter
					cmd.Parameters.AddWithValue("?", shift);
					cmd.Parameters.AddWithValue("?", userId ?? ""); // employee filter
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? ""); // ✅ department filter
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", search);         // name / id search
					cmd.Parameters.AddWithValue("?", like);
					cmd.Parameters.AddWithValue("?", like);

					con.Open();

					int present = 0, absent = 0, late = 0, onLeave = 0, overtime = 0;

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							string status = reader["status"].ToString();
							double otHrs = 0;
							double.TryParse(reader["overtime_hours"].ToString(), out otHrs);

							switch (status)
							{
								case "Present": present++; break;
								case "Absent": absent++; break;
								case "Late": late++; break;
								case "On Leave": onLeave++; break;
							}
							if (otHrs > 0) overtime++;

							dgv.Rows.Add(
								reader["attendance_id"].ToString(),
								reader["employee_id"].ToString(),
								reader["employee_name"]?.ToString() ?? "—",
								reader["date_display"].ToString(),
								reader["check_in"].ToString(),
								reader["check_out"].ToString(),
								reader["shift"].ToString(),
								reader["work_hours"].ToString(),
								reader["overtime_hours"].ToString(),
								status
							);
						}
					}

					lblPresent.Text = present.ToString();
					lblAbsent.Text = absent.ToString();
					lblLate.Text = late.ToString();
					lblOnLeave.Text = onLeave.ToString();
					if (lblOvertime != null) lblOvertime.Text = overtime.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message,
								"DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// ════════════════════════════════════════════════════════════════════
		//  Row coloring
		// ════════════════════════════════════════════════════════════════════
		private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var row = dgv.Rows[e.RowIndex];
			if (row.Cells["status"].Value == null || row.Selected) return;

			switch (row.Cells["status"].Value.ToString())
			{
				case "Present": row.DefaultCellStyle.BackColor = SuccessBg; break;
				case "Absent": row.DefaultCellStyle.BackColor = DangerBg; break;
				case "Late": row.DefaultCellStyle.BackColor = WarningBg; break;
				default: row.DefaultCellStyle.BackColor = Color.White; break;
			}
		}
	}

	// ════════════════════════════════════════════════════════════════════════
	//  ADD ATTENDANCE FORM
	// ════════════════════════════════════════════════════════════════════════
	public class frmAttendanceAdd : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private TextBox txtEmpId, txtCheckIn, txtCheckOut;
		private DateTimePicker dtpDate;
		private ComboBox cmbShift;

		public frmAttendanceAdd()
		{
			Text = "Record Attendance";
			Size = new Size(420, 480);
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
				Padding = new Padding(20, 16, 20, 10),
				AutoScroll = true,
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
				ctrl.Size = new Size(360, 28);
				pnl.Controls.Add(ctrl);
				y += 56;
			}

			txtEmpId = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Employee ID *", txtEmpId);

			dtpDate = new DateTimePicker { Font = new Font("Segoe UI", 9f), Value = DateTime.Today };
			AddRow("Date *", dtpDate);

			txtCheckIn = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f), Text = "HH:MM", ForeColor = Color.Gray };
			txtCheckIn.GotFocus += (s, e) => { if (txtCheckIn.Text == "HH:MM") { txtCheckIn.Text = ""; txtCheckIn.ForeColor = Color.Black; } };
			txtCheckIn.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtCheckIn.Text)) { txtCheckIn.Text = "HH:MM"; txtCheckIn.ForeColor = Color.Gray; } };
			AddRow("Check In Time (HH:MM)", txtCheckIn);

			txtCheckOut = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f), Text = "HH:MM", ForeColor = Color.Gray };
			txtCheckOut.GotFocus += (s, e) => { if (txtCheckOut.Text == "HH:MM") { txtCheckOut.Text = ""; txtCheckOut.ForeColor = Color.Black; } };
			txtCheckOut.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtCheckOut.Text)) { txtCheckOut.Text = "HH:MM"; txtCheckOut.ForeColor = Color.Gray; } };
			AddRow("Check Out Time (HH:MM)", txtCheckOut);

			cmbShift = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
			cmbShift.Items.AddRange(new[] { "Morning", "Evening", "Night" });
			cmbShift.SelectedIndex = 0;
			AddRow("Shift Type *", cmbShift);

			var btnSave = new Button
			{
				Text = "Save Record",
				Size = new Size(170, 38),
				Location = new Point(0, y),
				FlatStyle = FlatStyle.Flat,
				BackColor = Blue,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnSave.FlatAppearance.BorderSize = 0;
			btnSave.Click += BtnSave_Click;

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

			pnl.Controls.Add(btnSave);
			pnl.Controls.Add(btnCancel);
			Controls.Add(pnl);
		}

		private void BtnSave_Click(object sender, EventArgs e)
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

			TimeSpan? checkIn = null, checkOut = null;
			double workHrs = 0, otHrs = 0;

			if (txtCheckIn.Text != "HH:MM" && !string.IsNullOrWhiteSpace(txtCheckIn.Text))
			{
				if (!TimeSpan.TryParse(txtCheckIn.Text.Trim(), out TimeSpan ci))
				{
					MessageBox.Show("Invalid Check In time. Use HH:MM format", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				checkIn = ci;
			}

			if (txtCheckOut.Text != "HH:MM" && !string.IsNullOrWhiteSpace(txtCheckOut.Text))
			{
				if (!TimeSpan.TryParse(txtCheckOut.Text.Trim(), out TimeSpan co))
				{
					MessageBox.Show("Invalid Check Out time. Use HH:MM format", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				checkOut = co;
			}

			if (checkIn.HasValue && checkOut.HasValue)
			{
				workHrs = (checkOut.Value - checkIn.Value).TotalHours;
				if (workHrs < 0) workHrs += 24;
				otHrs = Math.Max(0, workHrs - 8);
			}

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();

					// Check employee exists
					using (var chk = new OdbcCommand("SELECT COUNT(1) FROM Employee WHERE employee_id = ?", con))
					{
						chk.Parameters.AddWithValue("?", empId);
						if (Convert.ToInt32(chk.ExecuteScalar()) == 0)
						{
							MessageBox.Show($"Employee with ID {empId} does not exist.",
											"Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					// Check duplicate
					using (var chk2 = new OdbcCommand(
						"SELECT COUNT(1) FROM Attendance WHERE employee_id = ? AND CONVERT(DATE, date) = ?", con))
					{
						chk2.Parameters.AddWithValue("?", empId);
						chk2.Parameters.AddWithValue("?", dtpDate.Value.Date);
						if (Convert.ToInt32(chk2.ExecuteScalar()) > 0)
						{
							MessageBox.Show("Attendance record already exists for this employee on this date.",
											"Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					using (var cmd = new OdbcCommand(@"
                        INSERT INTO Attendance
                            (employee_id, date, check_in_time, check_out_time, shift_type, work_hours, overtime_hours)
                        VALUES (?, ?, ?, ?, ?, ?, ?)", con))
					{
						cmd.Parameters.AddWithValue("?", empId);
						cmd.Parameters.AddWithValue("?", dtpDate.Value.Date);
						cmd.Parameters.AddWithValue("?", checkIn.HasValue ? (object)checkIn.Value : DBNull.Value);
						cmd.Parameters.AddWithValue("?", checkOut.HasValue ? (object)checkOut.Value : DBNull.Value);
						cmd.Parameters.AddWithValue("?", cmbShift.SelectedItem.ToString());
						cmd.Parameters.AddWithValue("?", Math.Round(workHrs, 2));
						cmd.Parameters.AddWithValue("?", Math.Round(otHrs, 2));
						cmd.ExecuteNonQuery();
					}

					MessageBox.Show($"Attendance saved!\nWork Hours: {workHrs:F2}h | Overtime: {otHrs:F2}h",
									"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving attendance:\n" + ex.Message,
								"DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}