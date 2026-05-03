using System;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class frmPerformance : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);

		private DataGridView dgv;
		private ComboBox cmbYear;
		private Label lblExcellent, lblGood, lblAverage, lblNeedsImp;

		private readonly string userDept;
		private readonly string userId;

		public frmPerformance(string department = "", string employeeId = "")
		{
			userDept = department;
			userId = employeeId;

			Text = "Performance Evaluation";
			Size = new Size(1100, 650);
			MinimumSize = new Size(800, 500);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
			LoadYears();
			LoadData();
		}

		private void BuildUI()
		{
			// Top bar
			Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
			topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
			topbar.Controls.Add(new Label
			{
				Text = "⭐ Performance Evaluation",
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

			toolbar.Controls.Add(new Label { Text = "Year:", AutoSize = true, Location = new Point(20, 17), ForeColor = TxtSec });

			cmbYear = new ComboBox
			{
				Size = new Size(100, 28),
				Location = new Point(55, 12),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 9f)
			};
			cmbYear.SelectedIndexChanged += (s, e) => LoadData();
			toolbar.Controls.Add(cmbYear);

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
			btnRefresh.Click += (s, e) => { LoadYears(); LoadData(); };

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
				Text = "+ Add Evaluation",
				Size = new Size(140, 32),
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
				var dlg = new frmPerformanceAdd();
				if (dlg.ShowDialog() == DialogResult.OK)
				{ LoadYears(); LoadData(); }
			};

			toolbar.Controls.Add(btnRefresh);
			toolbar.Controls.Add(btnEdit);
			toolbar.Controls.Add(btnAdd);
			toolbar.Resize += (s, e) =>
			{
				btnAdd.Location = new Point(toolbar.Width - 155, 10);
				btnEdit.Location = new Point(toolbar.Width - 240, 10);
				btnRefresh.Location = new Point(toolbar.Width - 340, 10);
			};

			// Summary cards
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
					Margin = new Padding(0, 0, 12, 0)
				};
				card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
				valLabel = new Label
				{
					Text = "0",
					Font = new Font("Segoe UI", 20f, FontStyle.Bold),
					ForeColor = clr,
					AutoSize = true,
					Location = new Point(12, 6)
				};
				card.Controls.Add(valLabel);
				card.Controls.Add(new Label
				{
					Text = lbl,
					Font = new Font("Segoe UI", 8f),
					ForeColor = TxtSec,
					AutoSize = true,
					Location = new Point(12, 40)
				});
				statsFlow.Controls.Add(card);
				return valLabel;
			}

			MakeCard("Excellent (90-100)", Color.FromArgb(21, 128, 61), ref lblExcellent);
			MakeCard("Good (75-89)", Color.FromArgb(29, 78, 216), ref lblGood);
			MakeCard("Average (60-74)", Color.FromArgb(180, 83, 9), ref lblAverage);
			MakeCard("Needs Improvement", Color.FromArgb(185, 28, 28), ref lblNeedsImp);

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

			dgv.Columns.Add("evaluation_id", "Eval ID");
			dgv.Columns["evaluation_id"].FillWeight = 55;
			dgv.Columns.Add("employee_id", "Emp ID");
			dgv.Columns["employee_id"].FillWeight = 55;
			dgv.Columns.Add("full_name", "Employee");
			dgv.Columns.Add("evaluation_year", "Year");
			dgv.Columns["evaluation_year"].FillWeight = 50;
			dgv.Columns.Add("technical_skill_score", "Technical");
			dgv.Columns["technical_skill_score"].FillWeight = 65;
			dgv.Columns.Add("attendance_score", "Attendance");
			dgv.Columns["attendance_score"].FillWeight = 65;
			dgv.Columns.Add("safety_score", "Safety");
			dgv.Columns["safety_score"].FillWeight = 55;
			dgv.Columns.Add("final_rating", "Final Rating");
			dgv.Columns["final_rating"].FillWeight = 70;
			dgv.Columns.Add("notes", "Notes");

			dgv.CellFormatting += Dgv_CellFormatting;

			Controls.Add(dgv);
			Controls.Add(statsBar);
			Controls.Add(toolbar);
			Controls.Add(topbar);
		}

		private void LoadYears()
		{
			string currentYear = cmbYear.SelectedItem?.ToString();
			cmbYear.Items.Clear();
			cmbYear.Items.Add("All");

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(
					"SELECT DISTINCT evaluation_year FROM Performance_Evaluation ORDER BY evaluation_year DESC", con))
				{
					con.Open();
					var reader = cmd.ExecuteReader();
					while (reader.Read())
						cmbYear.Items.Add(reader["evaluation_year"].ToString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل السنوات:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			int idx = cmbYear.Items.IndexOf(currentYear ?? "All");
			cmbYear.SelectedIndex = idx >= 0 ? idx : 0;
		}

		private void LoadData()
		{
			dgv.Rows.Clear();

			string year = cmbYear.SelectedItem?.ToString() ?? "All";

			string sql = @"
                SELECT
                    pe.evaluation_id,
                    pe.employee_id,
                    e.full_name,
                    pe.evaluation_year,
                    pe.technical_skill_score,
                    pe.attendance_score,
                    pe.safety_score,
                    pe.final_rating,
                    pe.notes
                FROM Performance_Evaluation pe
                LEFT JOIN Employee e ON pe.employee_id = e.employee_id
                LEFT JOIN Department d ON e.department_id = d.department_id
                WHERE 
                    (? = 'All' OR CAST(pe.evaluation_year AS VARCHAR) = ?)
                    AND (? = '' OR d.department_name = ?)
                    AND (? = '' OR CAST(pe.employee_id AS VARCHAR) = ?)
                ORDER BY pe.evaluation_year DESC, e.full_name";

			try
			{
				using (var con = new OdbcConnection(Global.ConnStr))
				using (var cmd = new OdbcCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("?", year);
					cmd.Parameters.AddWithValue("?", year);
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");

					con.Open();

					int excellent = 0, good = 0, average = 0, needsImp = 0;

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						int final = reader["final_rating"] == DBNull.Value
							? 0 : Convert.ToInt32(reader["final_rating"]);

						if (final >= 90) excellent++;
						else if (final >= 75) good++;
						else if (final >= 60) average++;
						else needsImp++;

						dgv.Rows.Add(
							reader["evaluation_id"].ToString(),
							reader["employee_id"].ToString(),
							reader["full_name"]?.ToString() ?? "—",
							reader["evaluation_year"].ToString(),
							reader["technical_skill_score"].ToString(),
							reader["attendance_score"].ToString(),
							reader["safety_score"].ToString(),
							final.ToString(),
							reader["notes"]?.ToString() ?? ""
						);
					}

					lblExcellent.Text = excellent.ToString();
					lblGood.Text = good.ToString();
					lblAverage.Text = average.ToString();
					lblNeedsImp.Text = needsImp.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("خطأ في تحميل البيانات:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnEdit_Click(object sender, EventArgs e)
		{
			if (dgv.CurrentRow == null) return;

			string evalId = dgv.CurrentRow.Cells["evaluation_id"].Value?.ToString();
			string empId = dgv.CurrentRow.Cells["employee_id"].Value?.ToString();
			string year = dgv.CurrentRow.Cells["evaluation_year"].Value?.ToString();

			var dlg = new frmPerformanceAdd(evalId, empId, year,
				dgv.CurrentRow.Cells["technical_skill_score"].Value?.ToString(),
				dgv.CurrentRow.Cells["attendance_score"].Value?.ToString(),
				dgv.CurrentRow.Cells["safety_score"].Value?.ToString(),
				dgv.CurrentRow.Cells["notes"].Value?.ToString());

			if (dlg.ShowDialog() == DialogResult.OK)
				LoadData();
		}

		private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var row = dgv.Rows[e.RowIndex];
			if (row.Cells["final_rating"].Value == null) return;

			if (!int.TryParse(row.Cells["final_rating"].Value.ToString(), out int val)) return;

			Color bg;
			if (val >= 90)
				bg = Color.FromArgb(240, 253, 244);
			else if (val >= 75)
				bg = Color.FromArgb(239, 246, 255);
			else if (val >= 60)
				bg = Color.FromArgb(255, 251, 235);
			else
				bg = Color.FromArgb(254, 242, 242);

			if (!row.Selected)
				row.DefaultCellStyle.BackColor = bg;
		}
	}

	// ============================================================
	//  ADD / EDIT PERFORMANCE FORM
	// ============================================================
	public class frmPerformanceAdd : Form
	{
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color Border = Color.FromArgb(220, 222, 226);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

		private readonly string _evalId;
		private readonly bool _isEdit;

		private TextBox txtEmpId;
		private ComboBox cmbYear;
		private NumericUpDown nudTech, nudAtt, nudSafety;
		private TextBox txtNotes;
		private Label lblFinalPreview;

		// ADD mode
		public frmPerformanceAdd()
			: this(null, "", DateTime.Now.Year.ToString(), "80", "80", "80", "") { }

		// EDIT mode
		public frmPerformanceAdd(string evalId, string empId, string year,
			string tech, string att, string safety, string notes)
		{
			_evalId = evalId;
			_isEdit = !string.IsNullOrEmpty(evalId);

			Text = _isEdit ? "Edit Performance Evaluation" : "Add Performance Evaluation";
			Size = new Size(440, 600);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI(empId, year, tech, att, safety, notes);
		}

		private void BuildUI(string empId, string year, string tech, string att, string safety, string notes)
		{
			var scroll = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				Padding = new Padding(20, 16, 20, 10),
				BackColor = BgPage
			};
			int y = 0;

			void AddLabel(string text)
			{
				scroll.Controls.Add(new Label
				{
					Text = text,
					Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
					ForeColor = Color.FromArgb(55, 65, 81),
					AutoSize = true,
					Location = new Point(0, y)
				});
			}

			void Row(string lbl, Control ctrl)
			{
				AddLabel(lbl);
				ctrl.Location = new Point(0, y + 18);
				ctrl.Size = new Size(380, 28);
				scroll.Controls.Add(ctrl);
				y += 56;
			}

			// Employee ID
			txtEmpId = new TextBox
			{
				BorderStyle = BorderStyle.FixedSingle,
				Font = new Font("Segoe UI", 10f),
				Text = empId,
				ReadOnly = _isEdit
			};
			Row("Employee ID *", txtEmpId);

			// Year
			cmbYear = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
			for (int yr = DateTime.Now.Year + 1; yr >= 2020; yr--)
				cmbYear.Items.Add(yr.ToString());
			cmbYear.SelectedItem = year;
			if (cmbYear.SelectedIndex < 0) cmbYear.SelectedIndex = 0;
			Row("Evaluation Year *", cmbYear);

			// Scores
			void AddScore(string lbl, ref NumericUpDown nud, string val)
			{
				AddLabel(lbl);
				nud = new NumericUpDown
				{
					Size = new Size(380, 28),
					Location = new Point(0, y + 18),
					Font = new Font("Segoe UI", 10f),
					Minimum = 0,
					Maximum = 100,
					Value = decimal.TryParse(val, out var v) ? v : 80
				};
				nud.ValueChanged += (s, e) => UpdateFinalPreview();
				scroll.Controls.Add(nud);
				y += 56;
			}

			AddScore("Technical Skill Score (0-100) *", ref nudTech, tech);
			AddScore("Attendance Score (0-100) *", ref nudAtt, att);
			AddScore("Safety Score (0-100) *", ref nudSafety, safety);

			// Final rating preview
			lblFinalPreview = new Label
			{
				AutoSize = true,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = Blue,
				Location = new Point(0, y)
			};
			scroll.Controls.Add(lblFinalPreview);
			y += 24;
			UpdateFinalPreview();

			// Notes
			AddLabel("Notes");
			txtNotes = new TextBox
			{
				Size = new Size(380, 70),
				Location = new Point(0, y + 18),
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				Multiline = true,
				Text = notes ?? ""
			};
			scroll.Controls.Add(txtNotes);
			y += 92;

			// Buttons
			var btnSave = new Button
			{
				Text = _isEdit ? "Save Changes" : "Save Evaluation",
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

		private void UpdateFinalPreview()
		{
			if (nudTech == null || nudAtt == null || nudSafety == null) return;
			int final = ((int)nudTech.Value + (int)nudAtt.Value + (int)nudSafety.Value) / 3;
			string grade;
			if (final >= 90)
				grade = "Excellent";
			else if (final >= 75)
				grade = "Good";
			else if (final >= 60)
				grade = "Average";
			else
				grade = "Needs Improvement";
			lblFinalPreview.Text = $"Final Rating (auto): {final}  —  {grade}";
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtEmpId.Text))
			{
				MessageBox.Show("Employee ID is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int tech = (int)nudTech.Value;
			int att = (int)nudAtt.Value;
			int safety = (int)nudSafety.Value;
			int final = (tech + att + safety) / 3;
			int empId, evalYear;

			if (!int.TryParse(txtEmpId.Text.Trim(), out empId))
			{
				MessageBox.Show("Employee ID must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			evalYear = int.Parse(cmbYear.SelectedItem.ToString());

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

					if (_isEdit)
					{
						// UPDATE
						string updateSql = @"
                            UPDATE Performance_Evaluation SET
                                evaluation_year = ?,
                                technical_skill_score = ?,
                                attendance_score = ?,
                                safety_score = ?,
                                final_rating = ?,
                                notes = ?
                            WHERE evaluation_id = ?";

						using (var cmd = new OdbcCommand(updateSql, con))
						{
							cmd.Parameters.AddWithValue("?", evalYear);
							cmd.Parameters.AddWithValue("?", tech);
							cmd.Parameters.AddWithValue("?", att);
							cmd.Parameters.AddWithValue("?", safety);
							cmd.Parameters.AddWithValue("?", final);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
							cmd.Parameters.AddWithValue("?", int.Parse(_evalId));
							cmd.ExecuteNonQuery();
						}
						MessageBox.Show("Evaluation updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						// Check for duplicate
						using (var chk2 = new OdbcCommand(@"
                            SELECT COUNT(1) FROM Performance_Evaluation
                            WHERE employee_id = ? AND evaluation_year = ?", con))
						{
							chk2.Parameters.AddWithValue("?", empId);
							chk2.Parameters.AddWithValue("?", evalYear);
							if (Convert.ToInt32(chk2.ExecuteScalar()) > 0)
							{
								MessageBox.Show($"Evaluation already exists for employee {empId} in year {evalYear}.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return;
							}
						}

						// INSERT
						string insertSql = @"
                            INSERT INTO Performance_Evaluation
                                (employee_id, evaluation_year, technical_skill_score,
                                 attendance_score, safety_score, final_rating, notes)
                            VALUES (?, ?, ?, ?, ?, ?, ?)";

						using (var cmd = new OdbcCommand(insertSql, con))
						{
							cmd.Parameters.AddWithValue("?", empId);
							cmd.Parameters.AddWithValue("?", evalYear);
							cmd.Parameters.AddWithValue("?", tech);
							cmd.Parameters.AddWithValue("?", att);
							cmd.Parameters.AddWithValue("?", safety);
							cmd.Parameters.AddWithValue("?", final);
							cmd.Parameters.AddWithValue("?", string.IsNullOrEmpty(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text);
							cmd.ExecuteNonQuery();
						}
						MessageBox.Show("Evaluation saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}

				DialogResult = DialogResult.OK;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving evaluation:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}