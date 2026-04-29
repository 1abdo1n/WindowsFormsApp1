using System;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmEmployees : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color Danger = Color.FromArgb(185, 28, 28);
		private readonly Color DangerBg = Color.FromArgb(254, 242, 242);
		private readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
		private readonly Color WarningBg = Color.FromArgb(255, 251, 235);

		private DataGridView dgv;
		private TextBox txtSearch;
		private Label lblTotalCount, lblActiveCount, lblOnLeaveCount;

		private readonly string userDept;
		private readonly string userId;
		private readonly string userRole;

		public frmEmployees(string department = "", string employeeId = "", string role = "Admin")
		{
			userDept = department;
			userId = employeeId;
			userRole = role;

			Text = string.IsNullOrEmpty(department) ? "Employees Management" : $"Employees - {department} Department";
			Size = new Size(1200, 700);
			MinimumSize = new Size(900, 550);
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
				Text = "👥 " + Text,
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
					Size = new Size(165, 62),
					BackColor = BgCard,
					Margin = new Padding(0, 0, 12, 0)
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

			MakeCard("Total Employees", Color.FromArgb(29, 78, 216), ref lblTotalCount);
			MakeCard("Active", Color.FromArgb(21, 128, 61), ref lblActiveCount);
			MakeCard("On Leave", Color.FromArgb(180, 83, 9), ref lblOnLeaveCount);

			statsBar.Controls.Add(statsFlow);

			// Toolbar
			Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

			txtSearch = new TextBox
			{
				Size = new Size(220, 28),
				Location = new Point(20, 12),
				Font = new Font("Segoe UI", 9.5f),
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = TxtSec,
				Text = "Search by name, code, or national ID..."
			};
			txtSearch.GotFocus += (s, e) =>
			{
				if (txtSearch.Text == "Search by name, code, or national ID...")
				{ txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; }
			};
			txtSearch.LostFocus += (s, e) =>
			{
				if (txtSearch.Text == "")
				{ txtSearch.Text = "Search by name, code, or national ID..."; txtSearch.ForeColor = TxtSec; }
			};
			txtSearch.TextChanged += (s, e) => LoadData();

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

			// Admin/Manager buttons
			if (userRole != "Employee")
			{
				var btnAdd = new Button
				{
					Text = "+ Add Employee",
					Size = new Size(130, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = Blue,
					ForeColor = Color.White,
					Font = new Font("Segoe UI", 9f, FontStyle.Bold),
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnAdd.FlatAppearance.BorderSize = 0;
				btnAdd.Click += (s, e) =>
				{
					var dlg = new frmEmployeeAddEdit();
					if (dlg.ShowDialog() == DialogResult.OK)
						LoadData();
				};

				var btnEdit = new Button
				{
					Text = "✎ Edit",
					Size = new Size(75, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = BgCard,
					ForeColor = TxtPrimary,
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnEdit.FlatAppearance.BorderColor = Border;
				btnEdit.Click += BtnEdit_Click;

				var btnDelete = new Button
				{
					Text = "🗑 Delete",
					Size = new Size(90, 32),
					FlatStyle = FlatStyle.Flat,
					BackColor = DangerBg,
					ForeColor = Danger,
					Cursor = Cursors.Hand,
					Anchor = AnchorStyles.Top | AnchorStyles.Right
				};
				btnDelete.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
				btnDelete.Click += BtnDelete_Click;

				toolbar.Controls.Add(btnAdd);
				toolbar.Controls.Add(btnEdit);
				toolbar.Controls.Add(btnDelete);
				toolbar.Resize += (s, e) =>
				{
					btnAdd.Location = new Point(toolbar.Width - 145, 10);
					btnDelete.Location = new Point(toolbar.Width - 245, 10);
					btnEdit.Location = new Point(toolbar.Width - 330, 10);
					btnRefresh.Location = new Point(toolbar.Width - 430, 10);
				};
			}
			else
			{
				toolbar.Resize += (s, e) =>
					btnRefresh.Location = new Point(toolbar.Width - 105, 10);
			}

			toolbar.Controls.Add(txtSearch);
			toolbar.Controls.Add(btnRefresh);

			// DataGridView
			dgv = new DataGridView
			{
				Dock = DockStyle.Fill,
				BackgroundColor = BgCard,
				BorderStyle = BorderStyle.None,
				ColumnHeadersHeight = 40,
				RowTemplate = { Height = 38 },
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

			dgv.Columns.Add("employee_id", "ID");
			dgv.Columns["employee_id"].FillWeight = 40;
			dgv.Columns.Add("employee_code", "Code");
			dgv.Columns["employee_code"].FillWeight = 60;
			dgv.Columns.Add("full_name", "Full Name");
			dgv.Columns.Add("national_id", "National ID");
			dgv.Columns["national_id"].FillWeight = 80;
			dgv.Columns.Add("insurance_number", "Insurance No.");
			dgv.Columns["insurance_number"].FillWeight = 70;
			dgv.Columns.Add("blood_type", "Blood");
			dgv.Columns["blood_type"].FillWeight = 45;
			dgv.Columns.Add("hire_date", "Hire Date");
			dgv.Columns["hire_date"].FillWeight = 65;
			dgv.Columns.Add("job_title", "Job Title");
			dgv.Columns.Add("grade", "Grade");
			dgv.Columns["grade"].FillWeight = 40;
			dgv.Columns.Add("department_name", "Department");
			dgv.Columns.Add("contract_id", "Contract ID");
			dgv.Columns["contract_id"].FillWeight = 70;
			dgv.Columns.Add("status", "Status");
			dgv.Columns["status"].FillWeight = 55;

			dgv.CellFormatting += Dgv_CellFormatting;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		private void LoadData()
		{
			dgv.Rows.Clear();

			string search = (txtSearch.Text == "Search by name, code, or national ID...") ? "" : txtSearch.Text.Trim();

			string sql = @"
                SELECT 
                    e.employee_id,
                    e.employee_code,
                    e.full_name,
                    e.national_id,
                    e.insurance_number,
                    e.blood_type,
                    CONVERT(VARCHAR, e.hire_date, 103) AS hire_date_display,
                    e.job_title,
                    e.grade,
                    d.department_name,
                    c.contract_id,
                    e.status
                FROM Employee e
                LEFT JOIN Department d ON e.department_id = d.department_id
                LEFT JOIN Contracts c ON e.contract_id = c.contract_id
                WHERE 
                    (? = '' 
                        OR e.employee_code LIKE ?
                        OR e.full_name LIKE ?
                        OR e.national_id LIKE ?)
                    AND (? = '' OR d.department_name = ?)
                    AND (? = '' OR CAST(e.employee_id AS VARCHAR) = ?)
                ORDER BY e.full_name";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(sql, con))
				{
					string searchPattern = "%" + search + "%";
					cmd.Parameters.AddWithValue("?", search);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", searchPattern);
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");

					con.Open();

					int total = 0, active = 0, onLeave = 0;

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						total++;
						string status = reader["status"]?.ToString() ?? "Active";
						if (status == "Active") active++;
						if (status == "On Leave") onLeave++;

						dgv.Rows.Add(
							reader["employee_id"].ToString(),
							reader["employee_code"].ToString(),
							reader["full_name"].ToString(),
							reader["national_id"]?.ToString() ?? "—",
							reader["insurance_number"]?.ToString() ?? "—",
							reader["blood_type"]?.ToString() ?? "—",
							reader["hire_date_display"].ToString(),
							reader["job_title"]?.ToString() ?? "—",
							reader["grade"]?.ToString() ?? "—",
							reader["department_name"]?.ToString() ?? "—",
							reader["contract_id"]?.ToString() ?? "—",
							status
						);
					}

					lblTotalCount.Text = total.ToString();
					lblActiveCount.Text = active.ToString();
					lblOnLeaveCount.Text = onLeave.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnEdit_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null)
			{
				MessageBox.Show("Please select an employee to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["employee_id"].Value?.ToString(), out int empId))
				return;

			var dlg = new frmEmployeeAddEdit(empId);
			if (dlg.ShowDialog() == DialogResult.OK)
				LoadData();
		}

		private void BtnDelete_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null)
			{
				MessageBox.Show("Please select an employee to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["employee_id"].Value?.ToString(), out int empId))
				return;

			string empName = dgv.CurrentRow.Cells["full_name"].Value?.ToString() ?? "";

			if (MessageBox.Show($"Are you sure you want to delete '{empName}'?\n\nThis will also delete all related records (attendance, payroll, leaves, etc.)!",
				"Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("DELETE FROM Employee WHERE employee_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", empId);
					con.Open();
					int rows = cmd.ExecuteNonQuery();

					if (rows > 0)
					{
						MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
						LoadData();
					}
					else
					{
						MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error deleting employee:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var row = dgv.Rows[e.RowIndex];
			if (row.Cells["status"].Value == null) return;
			if (row.Selected) return;

			string status = row.Cells["status"].Value.ToString();

			if (status == "Active")
				row.DefaultCellStyle.BackColor = SuccessBg;
			else if (status == "On Leave")
				row.DefaultCellStyle.BackColor = WarningBg;
			else if (status == "Suspended" || status == "Terminated")
				row.DefaultCellStyle.BackColor = DangerBg;
			else
				row.DefaultCellStyle.BackColor = Color.White;
		}
	}

	// ============================================================
	//  ADD / EDIT EMPLOYEE FORM
	// ============================================================
	public class frmEmployeeAddEdit : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private readonly int? _employeeId;
		private readonly bool _isEdit;

		private TextBox txtFullName, txtNationalId, txtInsuranceNum, txtMedicalCard,
						txtJobTitle, txtPhone, txtEmail, txtAddress, txtBankAccount;
		private ComboBox cmbBloodType, cmbDepartment, cmbContract, cmbGrade, cmbStatus;
		private DateTimePicker dtpHireDate;

		public frmEmployeeAddEdit(int? employeeId = null)
		{
			_employeeId = employeeId;
			_isEdit = employeeId.HasValue;

			Text = _isEdit ? "Edit Employee" : "Add New Employee";
			Size = new Size(550, 750);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadDropdowns();

			if (_isEdit)
				LoadEmployeeData();
		}

		private void BuildUI()
		{
			var scroll = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				Padding = new Padding(20, 16, 20, 10),
				BackColor = BgPage
			};
			int y = 0;

			void AddRow(string lbl, Control ctrl)
			{
				scroll.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
					ForeColor = Color.FromArgb(55, 65, 81),
					AutoSize = true,
					Location = new Point(0, y)
				});
				ctrl.Location = new Point(0, y + 18);
				ctrl.Size = new Size(480, 28);
				scroll.Controls.Add(ctrl);
				y += 56;
			}

			txtFullName = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Full Name *", txtFullName);

			txtNationalId = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("National ID", txtNationalId);

			txtInsuranceNum = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Insurance Number", txtInsuranceNum);

			txtMedicalCard = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Medical Card Number", txtMedicalCard);

			cmbBloodType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
			cmbBloodType.Items.AddRange(new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
			AddRow("Blood Type", cmbBloodType);

			txtPhone = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Phone Number", txtPhone);

			txtEmail = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Email", txtEmail);

			txtAddress = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Address", txtAddress);

			cmbDepartment = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f), DisplayMember = "department_name", ValueMember = "department_id" };
			AddRow("Department *", cmbDepartment);

			txtJobTitle = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Job Title", txtJobTitle);

			cmbGrade = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
			cmbGrade.Items.AddRange(new[] { "A", "B", "C", "D", "E" });
			AddRow("Grade", cmbGrade);

			cmbContract = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f),
				DisplayMember = "contract_id",
				ValueMember = "contract_id"
			};
			AddRow("Contract ID", cmbContract);

			dtpHireDate = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today };
			AddRow("Hire Date", dtpHireDate);

			cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
			cmbStatus.Items.AddRange(new[] { "Active", "On Leave", "Suspended", "Terminated" });
			cmbStatus.SelectedIndex = 0;
			AddRow("Status", cmbStatus);

			txtBankAccount = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
			AddRow("Bank Account Number", txtBankAccount);

			var btnSave = new Button
			{
				Text = _isEdit ? "💾 Save Changes" : "💾 Add Employee",
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
				Size = new Size(100, 38),
				Location = new Point(185, y),
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

		private void LoadDropdowns()
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();

					using (var cmd = new OdbcCommand("SELECT department_id, department_name FROM Department ORDER BY department_name", con))
					{
						var reader = cmd.ExecuteReader();
						var dt = new System.Data.DataTable();
						dt.Load(reader);
						cmbDepartment.DataSource = dt;
						cmbDepartment.DisplayMember = "department_name";
						cmbDepartment.ValueMember = "department_id";
						cmbDepartment.SelectedIndex = -1;
					}

					using (var cmd = new OdbcCommand("SELECT contract_id, contract_id FROM Contracts WHERE status = 'Active' ORDER BY contract_id", con))
					{
						var reader = cmd.ExecuteReader();
						var dt = new System.Data.DataTable();
						dt.Load(reader);
						cmbContract.DataSource = dt;
						cmbContract.DisplayMember = "contract_id";
						cmbContract.ValueMember = "contract_id";
						cmbContract.SelectedIndex = -1;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading dropdowns: " + ex.Message);
			}
		}

		private void LoadEmployeeData()
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(@"
                    SELECT full_name, national_id, insurance_number, medical_card_number,
                           blood_type, phone_number, email, address, department_id, job_title,
                           grade, contract_id, hire_date, status, bank_account_number
                    FROM Employee WHERE employee_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", _employeeId.Value);
					con.Open();
					var reader = cmd.ExecuteReader();
					if (reader.Read())
					{
						txtFullName.Text = reader["full_name"].ToString();
						txtNationalId.Text = reader["national_id"]?.ToString();
						txtInsuranceNum.Text = reader["insurance_number"]?.ToString();
						txtMedicalCard.Text = reader["medical_card_number"]?.ToString();
						cmbBloodType.SelectedItem = reader["blood_type"]?.ToString();
						txtPhone.Text = reader["phone_number"]?.ToString();
						txtEmail.Text = reader["email"]?.ToString();
						txtAddress.Text = reader["address"]?.ToString();
						txtJobTitle.Text = reader["job_title"]?.ToString();
						txtBankAccount.Text = reader["bank_account_number"]?.ToString();

						if (reader["department_id"] != DBNull.Value)
							cmbDepartment.SelectedValue = reader["department_id"];
						if (reader["contract_id"] != DBNull.Value)
							cmbContract.SelectedValue = reader["contract_id"];
						if (reader["grade"] != DBNull.Value)
							cmbGrade.SelectedItem = reader["grade"].ToString();
						if (reader["hire_date"] != DBNull.Value)
							dtpHireDate.Value = Convert.ToDateTime(reader["hire_date"]);
						if (reader["status"] != DBNull.Value)
							cmbStatus.SelectedItem = reader["status"].ToString();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading employee data: " + ex.Message);
			}
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFullName.Text))
			{
				MessageBox.Show("Full Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (cmbDepartment.SelectedValue == null)
			{
				MessageBox.Show("Department is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				{
					con.Open();

					if (_isEdit)
					{
						string sql = @"
                            UPDATE Employee SET
                                full_name = ?,
                                national_id = ?,
                                insurance_number = ?,
                                medical_card_number = ?,
                                blood_type = ?,
                                phone_number = ?,
                                email = ?,
                                address = ?,
                                department_id = ?,
                                job_title = ?,
                                grade = ?,
                                contract_id = ?,
                                hire_date = ?,
                                status = ?,
                                bank_account_number = ?
                            WHERE employee_id = ?";

						using (var cmd = new OdbcCommand(sql, con))
						{
							cmd.Parameters.AddWithValue("?", txtFullName.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNationalId.Text) ? (object)DBNull.Value : txtNationalId.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtInsuranceNum.Text) ? (object)DBNull.Value : txtInsuranceNum.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtMedicalCard.Text) ? (object)DBNull.Value : txtMedicalCard.Text);
							cmd.Parameters.AddWithValue("?", cmbBloodType.SelectedItem ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text);
							cmd.Parameters.AddWithValue("?", cmbDepartment.SelectedValue);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtJobTitle.Text) ? (object)DBNull.Value : txtJobTitle.Text);
							cmd.Parameters.AddWithValue("?", cmbGrade.SelectedItem ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", cmbContract.SelectedValue ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", dtpHireDate.Value);
							cmd.Parameters.AddWithValue("?", cmbStatus.SelectedItem?.ToString() ?? "Active");
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtBankAccount.Text) ? (object)DBNull.Value : txtBankAccount.Text);
							cmd.Parameters.AddWithValue("?", _employeeId.Value);
							cmd.ExecuteNonQuery();
						}
						MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						string empCode = "EMP" + DateTime.Now.ToString("yyyyMMddHHmmss");

						string sql = @"
                            INSERT INTO Employee 
                                (employee_code, full_name, national_id, insurance_number, medical_card_number,
                                 blood_type, phone_number, email, address, department_id, job_title,
                                 grade, contract_id, hire_date, status, bank_account_number)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

						using (var cmd = new OdbcCommand(sql, con))
						{
							cmd.Parameters.AddWithValue("?", empCode);
							cmd.Parameters.AddWithValue("?", txtFullName.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNationalId.Text) ? (object)DBNull.Value : txtNationalId.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtInsuranceNum.Text) ? (object)DBNull.Value : txtInsuranceNum.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtMedicalCard.Text) ? (object)DBNull.Value : txtMedicalCard.Text);
							cmd.Parameters.AddWithValue("?", cmbBloodType.SelectedItem ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text);
							cmd.Parameters.AddWithValue("?", cmbDepartment.SelectedValue);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtJobTitle.Text) ? (object)DBNull.Value : txtJobTitle.Text);
							cmd.Parameters.AddWithValue("?", cmbGrade.SelectedItem ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", cmbContract.SelectedValue ?? (object)DBNull.Value);
							cmd.Parameters.AddWithValue("?", dtpHireDate.Value);
							cmd.Parameters.AddWithValue("?", cmbStatus.SelectedItem?.ToString() ?? "Active");
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtBankAccount.Text) ? (object)DBNull.Value : txtBankAccount.Text);
							cmd.ExecuteNonQuery();
						}
						MessageBox.Show("Employee added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving employee:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}