using System;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmPayroll : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);

		private DataGridView dgv;
		private ComboBox cmbMonth;
		private TextBox txtSearch;
		private Label lblTotalBase, lblTotalAllow, lblTotalBonus, lblTotalDed, lblNetPayroll;

		private readonly string userDept;
		private readonly string userId;

		public frmPayroll(string department = "", string employeeId = "")
		{
			userDept = department;
			userId = employeeId;

			Text = "Payroll Management";
			Size = new Size(1300, 680);
			MinimumSize = new Size(900, 540);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadMonths();
			LoadData();
		}

		private void BuildUI()
		{
			// Top bar
			Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
			topbar.Controls.Add(new Label
			{
				Text = "💰 Payroll Management",
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
				{ txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; }
			};
			txtSearch.LostFocus += (s, e) =>
			{
				if (txtSearch.Text == "")
				{ txtSearch.Text = "Search employee..."; txtSearch.ForeColor = TxtSec; }
			};
			txtSearch.TextChanged += (s, e) => LoadData();

			toolbar.Controls.Add(new Label { Text = "Month:", AutoSize = true, Location = new Point(193, 17), ForeColor = TxtSec });

			cmbMonth = new ComboBox
			{
				Size = new Size(145, 28),
				Location = new Point(240, 12),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 9f)
			};
			cmbMonth.SelectedIndexChanged += (s, e) => LoadData();

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
			btnRefresh.Click += (s, e) => { LoadMonths(); LoadData(); };

			var btnExport = new Button
			{
				Text = "↓ Export to CSV",
				Size = new Size(110, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = BgCard,
				ForeColor = TxtPrimary,
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnExport.FlatAppearance.BorderColor = Border;
			btnExport.Click += BtnExport_Click;

			var btnAdd = new Button
			{
				Text = "+ Add Record",
				Size = new Size(115, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.FromArgb(240, 253, 244),
				ForeColor = Color.FromArgb(21, 128, 61),
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnAdd.FlatAppearance.BorderColor = Color.FromArgb(134, 239, 172);
			btnAdd.Click += (s, e) =>
			{
				if (new frmPayrollAdd().ShowDialog() == DialogResult.OK)
				{ LoadMonths(); LoadData(); }
			};

			var btnProcess = new Button
			{
				Text = "⚙ Process Payroll",
				Size = new Size(145, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = Blue,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnProcess.FlatAppearance.BorderSize = 0;
			btnProcess.Click += (s, e) => MessageBox.Show($"Processing payroll for: {cmbMonth.SelectedItem}\n\nThis will calculate salaries based on attendance and generate payroll records.", "Process Payroll", MessageBoxButtons.OK, MessageBoxIcon.Information);

			toolbar.Controls.Add(txtSearch);
			toolbar.Controls.Add(cmbMonth);
			toolbar.Controls.Add(btnRefresh);
			toolbar.Controls.Add(btnExport);
			toolbar.Controls.Add(btnAdd);
			toolbar.Controls.Add(btnProcess);
			toolbar.Resize += (s, e) =>
			{
				btnProcess.Location = new Point(toolbar.Width - 155, 10);
				btnAdd.Location = new Point(toolbar.Width - 280, 10);
				btnExport.Location = new Point(toolbar.Width - 400, 10);
				btnRefresh.Location = new Point(toolbar.Width - 500, 10);
			};

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
					Size = new Size(188, 62),
					BackColor = BgCard,
					Margin = new Padding(0, 0, 10, 0)
				};
				card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
				valLabel = new Label
				{
					Text = "0 EGP",
					Font = new Font("Segoe UI", 10f, FontStyle.Bold),
					ForeColor = clr,
					AutoSize = true,
					Location = new Point(10, 8)
				};
				card.Controls.Add(valLabel);
				card.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8f),
					ForeColor = TxtSec,
					AutoSize = true,
					Location = new Point(10, 36)
				});
				statsFlow.Controls.Add(card);
				return valLabel;
			}

			MakeCard("Total Base Salary", Color.FromArgb(29, 78, 216), ref lblTotalBase);
			MakeCard("Total Allowances", Color.FromArgb(21, 128, 61), ref lblTotalAllow);
			MakeCard("Total Bonuses", Color.FromArgb(109, 40, 217), ref lblTotalBonus);
			MakeCard("Total Deductions", Color.FromArgb(185, 28, 28), ref lblTotalDed);
			MakeCard("Net Payroll", Color.FromArgb(15, 118, 110), ref lblNetPayroll);

			statsBar.Controls.Add(statsFlow);

			// DataGridView
			dgv = new DataGridView
			{
				Dock = DockStyle.Fill,
				BackgroundColor = BgCard,
				BorderStyle = BorderStyle.None,
				ColumnHeadersHeight = 36,
				AllowUserToAddRows = false,
				ReadOnly = true,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				GridColor = Border,
				RowTemplate = { Height = 34 }
			};
			dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSec;
			dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
			dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
			dgv.DefaultCellStyle.ForeColor = TxtPrimary;
			dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);

			dgv.Columns.Add("payroll_id", "Pay ID");
			dgv.Columns["payroll_id"].FillWeight = 55;
			dgv.Columns.Add("employee_id", "Emp ID");
			dgv.Columns["employee_id"].FillWeight = 55;
			dgv.Columns.Add("full_name", "Employee");
			dgv.Columns.Add("base_salary", "Base Salary");
			dgv.Columns.Add("social_allowance", "Social Allow.");
			dgv.Columns.Add("risk_allowance", "Risk Allow.");
			dgv.Columns.Add("transport_allowance", "Transport");
			dgv.Columns.Add("production_bonus", "Bonus");
			dgv.Columns.Add("deductions", "Deductions");
			dgv.Columns.Add("tax", "Tax");
			dgv.Columns.Add("insurance", "Insurance");
			dgv.Columns.Add("net_salary", "Net Salary");
			dgv.Columns.Add("pay_date", "Pay Date");
			dgv.Columns["pay_date"].FillWeight = 80;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		private void LoadMonths()
		{
			string current = cmbMonth.SelectedItem?.ToString();
			cmbMonth.Items.Clear();
			cmbMonth.Items.Add("All");

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT DISTINCT
                        FORMAT(pay_date, 'MMMM yyyy') AS month_label
                    FROM Payroll
                    ORDER BY month_label DESC", con))
				{
					con.Open();
					var reader = cmd.ExecuteReader();
					while (reader.Read())
						cmbMonth.Items.Add(reader["month_label"].ToString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل الشهور:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			int idx = cmbMonth.Items.IndexOf(current ?? "All");
			cmbMonth.SelectedIndex = idx >= 0 ? idx : 0;
		}

		private void LoadData()
		{
			dgv.Rows.Clear();

			string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.Trim();
			string month = cmbMonth.SelectedItem?.ToString() ?? "All";

			string sql = @"
                SELECT 
                    p.payroll_id,
                    p.employee_id,
                    e.full_name,
                    p.base_salary,
                    p.social_allowance,
                    p.risk_allowance,
                    p.transport_allowance,
                    p.production_bonus,
                    p.deductions,
                    p.tax,
                    p.insurance,
                    p.net_salary,
                    CONVERT(VARCHAR, p.pay_date, 103) AS pay_date_display,
                    FORMAT(p.pay_date, 'MMMM yyyy') AS month_label
                FROM Payroll p
                LEFT JOIN Employee e ON p.employee_id = e.employee_id
                LEFT JOIN Department d ON e.department_id = d.department_id
                WHERE
                    (? = 'All' OR FORMAT(p.pay_date, 'MMMM yyyy') = ?)
                    AND (? = '' OR e.full_name LIKE '%' + ? + '%' OR CAST(p.employee_id AS VARCHAR) LIKE '%' + ? + '%')
                    AND (? = '' OR d.department_name = ?)
                    AND (? = '' OR CAST(p.employee_id AS VARCHAR) = ?)
                ORDER BY p.pay_date DESC, e.full_name";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(sql, con))
				{
					string searchPattern = "%" + search + "%";
					cmd.Parameters.AddWithValue("?", month);
					cmd.Parameters.AddWithValue("?", month);
					cmd.Parameters.AddWithValue("?", search);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");

					con.Open();

					decimal totalBase = 0, totalAllow = 0, totalBonus = 0, totalDed = 0, totalNet = 0;

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						decimal baseSal = ToDecimal(reader["base_salary"]);
						decimal social = ToDecimal(reader["social_allowance"]);
						decimal risk = ToDecimal(reader["risk_allowance"]);
						decimal transport = ToDecimal(reader["transport_allowance"]);
						decimal bonus = ToDecimal(reader["production_bonus"]);
						decimal ded = ToDecimal(reader["deductions"]);
						decimal tax = ToDecimal(reader["tax"]);
						decimal insurance = ToDecimal(reader["insurance"]);
						decimal net = ToDecimal(reader["net_salary"]);

						totalBase += baseSal;
						totalAllow += social + risk + transport;
						totalBonus += bonus;
						totalDed += ded + tax + insurance;
						totalNet += net;

						dgv.Rows.Add(
							reader["payroll_id"].ToString(),
							reader["employee_id"].ToString(),
							reader["full_name"]?.ToString() ?? "—",
							FormatNum(baseSal),
							FormatNum(social),
							FormatNum(risk),
							FormatNum(transport),
							FormatNum(bonus),
							FormatNum(ded),
							FormatNum(tax),
							FormatNum(insurance),
							FormatNum(net),
							reader["pay_date_display"].ToString()
						);
					}

					lblTotalBase.Text = FormatNum(totalBase) + " EGP";
					lblTotalAllow.Text = FormatNum(totalAllow) + " EGP";
					lblTotalBonus.Text = FormatNum(totalBonus) + " EGP";
					lblTotalDed.Text = FormatNum(totalDed) + " EGP";
					lblNetPayroll.Text = FormatNum(totalNet) + " EGP";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnExport_Click(object sender, EventArgs e)
		{
			if (dgv.Rows.Count == 0)
			{
				MessageBox.Show("لا توجد بيانات للتصدير.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using (var sfd = new SaveFileDialog
			{
				Filter = "CSV Files (*.csv)|*.csv",
				FileName = $"Payroll_{cmbMonth.SelectedItem}_{DateTime.Now:yyyyMMdd}.csv"
			})
			{
				if (sfd.ShowDialog() != DialogResult.OK) return;

				try
				{
					using (var writer = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
					{
						// Header
						var headers = new System.Collections.Generic.List<string>();
						foreach (DataGridViewColumn col in dgv.Columns)
							if (col.Visible) headers.Add(col.HeaderText);
						writer.WriteLine(string.Join(",", headers));

						// Data rows
						foreach (DataGridViewRow row in dgv.Rows)
						{
							var cells = new System.Collections.Generic.List<string>();
							foreach (DataGridViewCell cell in row.Cells)
								if (dgv.Columns[cell.ColumnIndex].Visible)
									cells.Add($"\"{cell.Value}\"");
							writer.WriteLine(string.Join(",", cells));
						}
					}
					MessageBox.Show("تم التصدير بنجاح!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show("خطأ في التصدير:\n" + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private static decimal ToDecimal(object val) => val == DBNull.Value ? 0 : Convert.ToDecimal(val);
		private static string FormatNum(decimal val) => val.ToString("N2");
	}

	// ============================================================
	//  ADD / EDIT PAYROLL FORM
	// ============================================================
	public class frmPayrollAdd : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private TextBox txtEmpId, txtBase, txtSocial, txtRisk, txtTransport, txtBonus, txtDed, txtTax, txtIns;
		private DateTimePicker dtpPayDate;
		private Label lblNetPreview;

		public frmPayrollAdd()
		{
			Text = "Add Payroll Record";
			Size = new Size(480, 720);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
		}

		private void BuildUI()
		{
			var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage };
			int y = 0;

			TextBox AddRow(string lbl, bool isNum = false)
			{
				scroll.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
					ForeColor = Color.FromArgb(55, 65, 81),
					AutoSize = true,
					Location = new Point(0, y)
				});
				var tb = new TextBox
				{
					Size = new Size(420, 28),
					Location = new Point(0, y + 18),
					Font = new Font("Segoe UI", 10f),
					BorderStyle = BorderStyle.FixedSingle,
					BackColor = Color.FromArgb(249, 250, 251)
				};
				if (isNum)
				{
					tb.Text = "0.00";
					tb.TextChanged += (s, e) => UpdateNetPreview();
				}
				scroll.Controls.Add(tb);
				y += 56;
				return tb;
			}

			txtEmpId = AddRow("Employee ID *");
			txtBase = AddRow("Base Salary *", isNum: true);
			txtSocial = AddRow("Social Allowance", isNum: true);
			txtRisk = AddRow("Risk Allowance", isNum: true);
			txtTransport = AddRow("Transport Allowance", isNum: true);
			txtBonus = AddRow("Production Bonus", isNum: true);
			txtDed = AddRow("Deductions", isNum: true);
			txtTax = AddRow("Tax", isNum: true);
			txtIns = AddRow("Insurance", isNum: true);

			lblNetPreview = new Label
			{
				Text = "Net Salary (auto): 0.00 EGP",
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = Blue,
				AutoSize = true,
				Location = new Point(0, y)
			};
			scroll.Controls.Add(lblNetPreview);
			y += 24;

			scroll.Controls.Add(new Label
			{
				Text = "Pay Date *",
				Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
				ForeColor = Color.FromArgb(55, 65, 81),
				AutoSize = true,
				Location = new Point(0, y)
			});
			dtpPayDate = new DateTimePicker
			{
				Size = new Size(420, 28),
				Location = new Point(0, y + 18),
				Font = new Font("Segoe UI", 9f),
				Value = DateTime.Today
			};
			scroll.Controls.Add(dtpPayDate);
			y += 56;

			var btnSave = new Button
			{
				Text = "Save Record",
				Size = new Size(180, 38),
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
				Size = new Size(100, 38),
				Location = new Point(190, y),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.White,
				ForeColor = TxtSec,
				Cursor = Cursors.Hand
			};
			btnCancel.FlatAppearance.BorderColor = Border;
			btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

			scroll.Controls.Add(btnSave);
			scroll.Controls.Add(btnCancel);
			Controls.Add(scroll);
		}

		private void UpdateNetPreview()
		{
			decimal Parse(TextBox tb) => decimal.TryParse(tb.Text, out var v) ? v : 0;
			decimal net = Parse(txtBase) + Parse(txtSocial) + Parse(txtRisk) + Parse(txtTransport) + Parse(txtBonus) - Parse(txtDed) - Parse(txtTax) - Parse(txtIns);
			lblNetPreview.Text = $"Net Salary (auto): {net:N2} EGP";
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

			if (!decimal.TryParse(txtBase.Text, out decimal baseSal) || baseSal <= 0)
			{
				MessageBox.Show("Base Salary is required and must be greater than zero.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			decimal Parse(TextBox tb) => decimal.TryParse(tb.Text, out var v) ? v : 0;
			decimal social = Parse(txtSocial);
			decimal risk = Parse(txtRisk);
			decimal transport = Parse(txtTransport);
			decimal bonus = Parse(txtBonus);
			decimal ded = Parse(txtDed);
			decimal tax = Parse(txtTax);
			decimal insurance = Parse(txtIns);
			decimal net = baseSal + social + risk + transport + bonus - ded - tax - insurance;

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

					// Check for duplicate payroll record for the same month
					using (var chk2 = new OdbcCommand(@"
                        SELECT COUNT(1) FROM Payroll 
                        WHERE employee_id = ? AND MONTH(pay_date) = ? AND YEAR(pay_date) = ?", con))
					{
						chk2.Parameters.AddWithValue("?", empId);
						chk2.Parameters.AddWithValue("?", dtpPayDate.Value.Month);
						chk2.Parameters.AddWithValue("?", dtpPayDate.Value.Year);
						if (Convert.ToInt32(chk2.ExecuteScalar()) > 0)
						{
							MessageBox.Show($"Payroll record already exists for employee {empId} in this month.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
					}

					string insertSql = @"
                        INSERT INTO Payroll 
                            (employee_id, base_salary, social_allowance, risk_allowance, 
                             transport_allowance, production_bonus, deductions, tax, 
                             insurance, net_salary, pay_date)
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

					using (var cmd = new OdbcCommand(insertSql, con))
					{
						cmd.Parameters.AddWithValue("?", empId);
						cmd.Parameters.AddWithValue("?", baseSal);
						cmd.Parameters.AddWithValue("?", social);
						cmd.Parameters.AddWithValue("?", risk);
						cmd.Parameters.AddWithValue("?", transport);
						cmd.Parameters.AddWithValue("?", bonus);
						cmd.Parameters.AddWithValue("?", ded);
						cmd.Parameters.AddWithValue("?", tax);
						cmd.Parameters.AddWithValue("?", insurance);
						cmd.Parameters.AddWithValue("?", net);
						cmd.Parameters.AddWithValue("?", dtpPayDate.Value.Date);
						cmd.ExecuteNonQuery();
					}

					MessageBox.Show($"Payroll record saved successfully!\nNet Salary: {net:N2} EGP", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving payroll record:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}