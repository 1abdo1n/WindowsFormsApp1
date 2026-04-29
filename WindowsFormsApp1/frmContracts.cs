using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmContracts : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);

		private DataGridView dgv;
		private TextBox txtSearch;
		private ComboBox cmbStatus;
		private Label lblTotalCount, lblActiveCount, lblExpiringCount;

		public frmContracts()
		{
			Text = "Contracts Management";
			Size = new Size(1100, 650);
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
				Text = "📄 Contracts Management",
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
					Size = new Size(180, 62),
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
					Location = new Point(12, 40)
				});
				statsFlow.Controls.Add(card);
				return valLabel;
			}

			MakeCard("Total Contracts", Blue, ref lblTotalCount);
			MakeCard("Active Contracts", Color.FromArgb(21, 128, 61), ref lblActiveCount);
			MakeCard("Expiring Soon (30 days)", Color.FromArgb(180, 83, 9), ref lblExpiringCount);

			statsBar.Controls.Add(statsFlow);

			// Toolbar
			Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

			txtSearch = new TextBox
			{
				Size = new Size(180, 28),
				Location = new Point(20, 12),
				Font = new Font("Segoe UI", 9.5f),
				BorderStyle = BorderStyle.FixedSingle,
				ForeColor = TxtSec,
				Text = "Search by employee or contract #..."
			};
			txtSearch.GotFocus += (s, e) =>
			{
				if (txtSearch.Text == "Search by employee or contract #...")
				{
					txtSearch.Text = "";
					txtSearch.ForeColor = TxtPrimary;
				}
			};
			txtSearch.LostFocus += (s, e) =>
			{
				if (txtSearch.Text == "")
				{
					txtSearch.Text = "Search by employee or contract #...";
					txtSearch.ForeColor = TxtSec;
				}
			};
			txtSearch.TextChanged += (s, e) => LoadData();

			toolbar.Controls.Add(new Label
			{
				Text = "Status:",
				AutoSize = true,
				Location = new Point(215, 17),
				ForeColor = TxtSec
			});

			cmbStatus = new ComboBox
			{
				Size = new Size(120, 28),
				Location = new Point(265, 12),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 9f)
			};
			cmbStatus.Items.AddRange(new[] { "All", "Active", "Expired", "Pending" });
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

			var btnDelete = new Button
			{
				Text = "🗑 Delete",
				Size = new Size(90, 32),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.FromArgb(254, 242, 242),
				ForeColor = Color.FromArgb(185, 28, 28),
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			btnDelete.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
			btnDelete.Click += BtnDelete_Click;

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

			var btnAdd = new Button
			{
				Text = "+ New Contract",
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
				if (new frmContractAdd().ShowDialog() == DialogResult.OK)
					LoadData();
			};

			toolbar.Controls.Add(txtSearch);
			toolbar.Controls.Add(cmbStatus);
			toolbar.Controls.Add(btnRefresh);
			toolbar.Controls.Add(btnDelete);
			toolbar.Controls.Add(btnEdit);
			toolbar.Controls.Add(btnAdd);

			toolbar.Resize += (s, e) =>
			{
				btnAdd.Location = new Point(toolbar.Width - 145, 10);
				btnDelete.Location = new Point(toolbar.Width - 245, 10);
				btnEdit.Location = new Point(toolbar.Width - 330, 10);
				btnRefresh.Location = new Point(toolbar.Width - 430, 10);
			};

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

			dgv.Columns.Add("contract_id", "Contract ID");
			dgv.Columns["contract_id"].FillWeight = 70;
			dgv.Columns.Add("employee_name", "Employee");
			dgv.Columns.Add("contract_type", "Type");
			dgv.Columns["contract_type"].FillWeight = 80;
			dgv.Columns.Add("start_date", "Start Date");
			dgv.Columns["start_date"].FillWeight = 80;
			dgv.Columns.Add("end_date", "End Date");
			dgv.Columns["end_date"].FillWeight = 80;
			dgv.Columns.Add("status", "Status");
			dgv.Columns["status"].FillWeight = 70;

			dgv.CellFormatting += Dgv_CellFormatting;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		private void LoadData()
		{
			dgv.Rows.Clear();

			string search = (txtSearch.Text == "Search by employee or contract #...") ? "" : txtSearch.Text.Trim();
			string statusFilter = cmbStatus.SelectedItem?.ToString() ?? "All";

			// Modified query: join through Employee.contract_id instead of Contract.employee_id
			string sql = @"
                SELECT 
                    c.contract_id,
                    c.contract_type,
                    c.start_date,
                    c.end_date,
                    c.status,
                    e.full_name AS employee_name,
                    CASE
                        WHEN c.status = 'Active' AND c.end_date < GETDATE() THEN 'Expired'
                        WHEN c.status = 'Active' AND c.end_date <= DATEADD(day, 30, GETDATE()) THEN 'Expiring Soon'
                        ELSE c.status
                    END AS status_display
                FROM Contract c
                INNER JOIN Employee e ON e.contract_id = c.contract_id
                WHERE 
                    (? = 'All' OR 
                        CASE
                            WHEN c.status = 'Active' AND c.end_date < GETDATE() THEN 'Expired'
                            WHEN c.status = 'Active' AND c.end_date <= DATEADD(day, 30, GETDATE()) THEN 'Expiring Soon'
                            ELSE c.status
                        END = ?)
                    AND (? = '' 
                        OR e.full_name LIKE '%' + ? + '%'
                        OR CAST(c.contract_id AS VARCHAR) LIKE '%' + ? + '%')
                ORDER BY c.end_date ASC";

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

					con.Open();

					int total = 0, active = 0, expiring = 0;

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						total++;
						string statusDisplay = reader["status_display"].ToString();
						if (statusDisplay == "Active") active++;
						if (statusDisplay == "Expiring Soon") expiring++;

						dgv.Rows.Add(
							reader["contract_id"].ToString(),
							reader["employee_name"]?.ToString() ?? "—",
							reader["contract_type"]?.ToString() ?? "—",
							Convert.ToDateTime(reader["start_date"]).ToString("dd/MM/yyyy"),
							Convert.ToDateTime(reader["end_date"]).ToString("dd/MM/yyyy"),
							statusDisplay
						);
					}

					lblTotalCount.Text = total.ToString();
					lblActiveCount.Text = active.ToString();
					lblExpiringCount.Text = expiring.ToString();
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
				MessageBox.Show("Please select a contract to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["contract_id"].Value?.ToString(), out int contractId))
				return;

			var dlg = new frmContractAdd(contractId);
			if (dlg.ShowDialog() == DialogResult.OK)
				LoadData();
		}

		private void BtnDelete_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null)
			{
				MessageBox.Show("Please select a contract to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!int.TryParse(dgv.CurrentRow.Cells["contract_id"].Value?.ToString(), out int contractId))
				return;

			string empName = dgv.CurrentRow.Cells["employee_name"].Value?.ToString() ?? "this contract";

			if (MessageBox.Show($"Are you sure you want to delete {empName}'s contract?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("DELETE FROM Contract WHERE contract_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", contractId);
					con.Open();
					int rows = cmd.ExecuteNonQuery();

					if (rows > 0)
					{
						MessageBox.Show("Contract deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
						LoadData();
					}
					else
					{
						MessageBox.Show("Contract not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error deleting contract:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
			else if (status == "Expired")
				row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
			else if (status == "Expiring Soon")
				row.DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
			else
				row.DefaultCellStyle.BackColor = Color.White;
		}
	}

	// ============================================================
	//  ADD / EDIT CONTRACT FORM
	// ============================================================
	public class frmContractAdd : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private readonly int? _contractId;
		private readonly bool _isEdit;

		private ComboBox cmbEmployee;
		private ComboBox cmbContractType;
		private DateTimePicker dtpStart, dtpEnd;
		private ComboBox cmbStatus;
		private TextBox txtNotes;

		public frmContractAdd(int? contractId = null)
		{
			_contractId = contractId;
			_isEdit = contractId.HasValue;

			Text = _isEdit ? "Edit Contract" : "New Contract";
			Size = new Size(500, 520);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadEmployees();

			if (_isEdit)
				LoadContractData();
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
				ctrl.Size = new Size(440, 28);
				pnl.Controls.Add(ctrl);
				y += 56;
			}

			// Employee
			cmbEmployee = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f),
				DisplayMember = "Name",
				ValueMember = "Id"
			};
			AddRow("Employee *", cmbEmployee);

			// Contract Type
			cmbContractType = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f)
			};
			cmbContractType.Items.AddRange(new[] { "Full-time", "Part-time", "Contract", "Internship", "Freelance" });
			cmbContractType.SelectedIndex = 0;
			AddRow("Contract Type *", cmbContractType);

			// Start Date
			dtpStart = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today };
			AddRow("Start Date *", dtpStart);

			// End Date
			dtpEnd = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today.AddYears(1) };
			AddRow("End Date", dtpEnd);

			// Status
			cmbStatus = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 10f)
			};
			cmbStatus.Items.AddRange(new[] { "Active", "Expired", "Pending", "Cancelled" });
			cmbStatus.SelectedIndex = 0;
			AddRow("Status", cmbStatus);

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
				Size = new Size(440, 60),
				Location = new Point(0, y + 18),
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				Multiline = true
			};
			pnl.Controls.Add(txtNotes);
			y += 80;

			// Buttons
			var btnSave = new Button
			{
				Text = _isEdit ? "💾 Save Changes" : "💾 Create Contract",
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

			pnl.Controls.Add(btnSave);
			pnl.Controls.Add(btnCancel);
			Controls.Add(pnl);
		}

		private void LoadEmployees()
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("SELECT employee_id, full_name FROM Employee WHERE status = 'Active' ORDER BY full_name", con))
				{
					con.Open();
					var reader = cmd.ExecuteReader();
					var items = new System.Collections.Generic.List<KeyValuePair<int, string>>();
					while (reader.Read())
					{
						items.Add(new KeyValuePair<int, string>(Convert.ToInt32(reader["employee_id"]), reader["full_name"].ToString()));
					}
					cmbEmployee.DataSource = items;
					cmbEmployee.DisplayMember = "Value";
					cmbEmployee.ValueMember = "Key";
					cmbEmployee.SelectedIndex = -1;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading employees: " + ex.Message);
			}
		}

		private void LoadContractData()
		{
			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand("SELECT contract_type, start_date, end_date, status, notes FROM Contract WHERE contract_id = ?", con))
				{
					cmd.Parameters.AddWithValue("?", _contractId.Value);
					con.Open();
					var reader = cmd.ExecuteReader();
					if (reader.Read())
					{
						cmbContractType.SelectedItem = reader["contract_type"]?.ToString();
						dtpStart.Value = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]) : DateTime.Today;
						dtpEnd.Value = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]) : DateTime.Today.AddYears(1);
						cmbStatus.SelectedItem = reader["status"]?.ToString() ?? "Active";
						txtNotes.Text = reader["notes"]?.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading contract data: " + ex.Message);
			}
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			if (cmbEmployee.SelectedValue == null)
			{
				MessageBox.Show("Please select an employee.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (cmbContractType.SelectedItem == null)
			{
				MessageBox.Show("Please select contract type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int employeeId = Convert.ToInt32(cmbEmployee.SelectedValue);
			string contractType = cmbContractType.SelectedItem.ToString();
			DateTime startDate = dtpStart.Value;
			DateTime endDate = dtpEnd.Value;
			string status = cmbStatus.SelectedItem?.ToString() ?? "Active";

			if (endDate < startDate)
			{
				MessageBox.Show("End date cannot be before start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            UPDATE Contract SET
                                contract_type = ?,
                                start_date = ?,
                                end_date = ?,
                                status = ?,
                                notes = ?
                            WHERE contract_id = ?";

						using (var cmd = new OdbcCommand(sql, con))
						{
							cmd.Parameters.AddWithValue("?", contractType);
							cmd.Parameters.AddWithValue("?", startDate);
							cmd.Parameters.AddWithValue("?", endDate);
							cmd.Parameters.AddWithValue("?", status);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
							cmd.Parameters.AddWithValue("?", _contractId.Value);
							cmd.ExecuteNonQuery();
						}
						MessageBox.Show("Contract updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						// Check if employee already has a contract
						using (var chk = new OdbcCommand("SELECT contract_id FROM Employee WHERE employee_id = ?", con))
						{
							chk.Parameters.AddWithValue("?", employeeId);
							var existingContractId = chk.ExecuteScalar();
							if (existingContractId != null && existingContractId != DBNull.Value)
							{
								MessageBox.Show("This employee already has a contract assigned.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
						}

						// Insert new contract
						string sql = @"
                            INSERT INTO Contract (contract_type, start_date, end_date, status, notes)
                            VALUES (?, ?, ?, ?, ?);
                            SELECT SCOPE_IDENTITY()";

						int newContractId;
						using (var cmd = new OdbcCommand(sql, con))
						{
							cmd.Parameters.AddWithValue("?", contractType);
							cmd.Parameters.AddWithValue("?", startDate);
							cmd.Parameters.AddWithValue("?", endDate);
							cmd.Parameters.AddWithValue("?", status);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
							newContractId = Convert.ToInt32(cmd.ExecuteScalar());
						}

						// Update employee with the new contract_id
						using (var cmd2 = new OdbcCommand("UPDATE Employee SET contract_id = ? WHERE employee_id = ?", con))
						{
							cmd2.Parameters.AddWithValue("?", newContractId);
							cmd2.Parameters.AddWithValue("?", employeeId);
							cmd2.ExecuteNonQuery();
						}

						MessageBox.Show("Contract created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving contract:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}