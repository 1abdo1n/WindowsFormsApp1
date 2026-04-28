using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace WindowsFormsApp1
{
    public class frmPerformance : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private DataGridView dgv;
        private ComboBox cmbYear;
        private string[][] allRows;

        public frmPerformance()
        {
            Text = "Performance Evaluation"; Size = new Size(1100, 650); MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen; BackColor = BgPage; Font = new Font("Segoe UI", 9f);

            // Sample data
            allRows = new string[][]
            {
                new[]{"EV001","001","Ahmed Hassan","2025","92","88","95","91","Excellent performance"},
                new[]{"EV002","002","Sara Mohamed","2025","85","90","88","87","Good team player"},
                new[]{"EV003","003","Omar Ali",    "2025","78","82","80","80","Meeting expectations"},
                new[]{"EV004","004","Nour Khaled", "2025","65","70","72","69","Needs improvement in technical skills"},
                new[]{"EV005","005","Youssef Taha","2025","88","85","90","87","Strong sales performance"},
                new[]{"EV006","001","Ahmed Hassan","2024","90","85","92","89","Good performance"},
                new[]{"EV007","002","Sara Mohamed","2024","82","88","85","85","Solid contributor"},
                new[]{"EV008","003","Omar Ali",    "2024","75","78","76","76","Average performance"},
            };

            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = "Performance Evaluation", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
            var btnBack = new Button { Text = "← Back", Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border; btnBack.Click += (s, e) => Close();
            topbar.Controls.Add(btnBack); topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

            var lblYear = new Label { Text = "Year:", AutoSize = true, Location = new Point(20, 17), ForeColor = TxtSecondary };
            cmbYear = new ComboBox { Size = new Size(100, 28), Location = new Point(55, 12), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbYear.Items.AddRange(new[] { "All", "2023", "2024", "2025" });
            cmbYear.SelectedIndex = 0;
            cmbYear.SelectedIndexChanged += (s, e) => ApplyYearFilter();

            var btnAdd = new Button { Text = "+ Add Evaluation", Size = new Size(140, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => { new frmPerformanceAdd().ShowDialog(); ApplyYearFilter(); };

            var btnEdit = new Button { Text = "✎ Edit", Size = new Size(75, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtPrimary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnEdit.FlatAppearance.BorderColor = Border;

            toolbar.Controls.Add(lblYear); toolbar.Controls.Add(cmbYear); toolbar.Controls.Add(btnAdd); toolbar.Controls.Add(btnEdit);
            toolbar.Resize += (s, e) => { btnAdd.Location = new Point(toolbar.Width - 155, 10); btnEdit.Location = new Point(toolbar.Width - 240, 10); };

            // Summary cards
            Panel statsBar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = BgPage, Padding = new Padding(16, 8, 16, 8) };
            (string lbl, string val, Color clr)[] stats = {
                ("Excellent (90-100)", "12", Color.FromArgb(21,128,61)),
                ("Good (75-89)",       "24", Color.FromArgb(29,78,216)),
                ("Average (60-74)",    "11", Color.FromArgb(180,83,9)),
                ("Needs Improvement",  "5",  Color.FromArgb(185,28,28)),
            };
            var statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = BgPage };
            foreach (var st in stats)
            {
                var card = new Panel { Size = new Size(188, 62), BackColor = BgCard, Margin = new Padding(0, 0, 12, 0) };
                card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
                card.Controls.Add(new Label { Text = st.val, Font = new Font("Segoe UI", 20f, FontStyle.Bold), ForeColor = st.clr, AutoSize = true, Location = new Point(12, 6) });
                card.Controls.Add(new Label { Text = st.lbl, Font = new Font("Segoe UI", 8f), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(12, 40) });
                statsFlow.Controls.Add(card);
            }
            statsBar.Controls.Add(statsFlow);

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("eval_id", "Eval ID"); dgv.Columns["eval_id"].FillWeight = 55;
            dgv.Columns.Add("emp_id", "Emp ID"); dgv.Columns["emp_id"].FillWeight = 55;
            dgv.Columns.Add("emp_name", "Employee");
            dgv.Columns.Add("year", "Year"); dgv.Columns["year"].FillWeight = 50;
            dgv.Columns.Add("tech", "Technical"); dgv.Columns["tech"].FillWeight = 65;
            dgv.Columns.Add("att_score", "Attendance"); dgv.Columns["att_score"].FillWeight = 65;
            dgv.Columns.Add("safety", "Safety"); dgv.Columns["safety"].FillWeight = 55;
            dgv.Columns.Add("final", "Final Rating"); dgv.Columns["final"].FillWeight = 70;
            dgv.Columns.Add("notes", "Notes");

            ApplyYearFilter();

            Controls.Add(dgv); Controls.Add(statsBar); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyYearFilter()
        {
            string selectedYear = cmbYear.SelectedItem?.ToString() ?? "All";

            dgv.Rows.Clear();
            foreach (var row in allRows)
            {
                string year = row[3]; // Year is at index 3
                if (selectedYear == "All" || year == selectedYear)
                {
                    dgv.Rows.Add(row);
                }
            }
        }
    }

    public class frmPerformanceAdd : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color Border = Color.FromArgb(220, 222, 226);
        private readonly Color TxtSec = Color.FromArgb(107, 114, 128);

        public frmPerformanceAdd()
        {
            Text = "Add Performance Evaluation"; Size = new Size(440, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage };
            int y = 0;

            void Row(string lbl, Control ctrl)
            {
                scroll.Controls.Add(new Label { Text = lbl, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
                ctrl.Location = new Point(0, y + 18); ctrl.Size = new Size(380, 28);
                scroll.Controls.Add(ctrl); y += 56;
            }

            Row("Employee ID *", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });

            var cmbYear = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbYear.Items.AddRange(new[] { "2023", "2024", "2025" }); cmbYear.SelectedIndex = 2;
            Row("Evaluation Year *", cmbYear);

            // Score fields with range note
            NumericUpDown nudTech = null, nudAtt = null, nudSafety = null;

            scroll.Controls.Add(new Label { Text = "Technical Skill Score (0-100) *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            nudTech = new NumericUpDown { Size = new Size(380, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), Minimum = 0, Maximum = 100, Value = 80 };
            scroll.Controls.Add(nudTech); y += 56;

            scroll.Controls.Add(new Label { Text = "Attendance Score (0-100) *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            nudAtt = new NumericUpDown { Size = new Size(380, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), Minimum = 0, Maximum = 100, Value = 80 };
            scroll.Controls.Add(nudAtt); y += 56;

            scroll.Controls.Add(new Label { Text = "Safety Score (0-100) *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            nudSafety = new NumericUpDown { Size = new Size(380, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), Minimum = 0, Maximum = 100, Value = 80 };
            scroll.Controls.Add(nudSafety); y += 56;

            // Notes
            scroll.Controls.Add(new Label { Text = "Notes", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            TextBox txtNotes = new TextBox { Size = new Size(380, 70), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, Multiline = true };
            scroll.Controls.Add(txtNotes); y += 92;

            var btnSave = new Button { Text = "Save Evaluation", Size = new Size(180, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => {
                int tech = (int)nudTech.Value;
                int att = (int)nudAtt.Value;
                int safety = (int)nudSafety.Value;
                int final = (tech + att + safety) / 3;
                MessageBox.Show($"Evaluation saved!\nFinal Score: {final}\nConnect to DB to store data.", "Saved");
                Close();
            };
            var btnCancel = new Button { Text = "Cancel", Size = new Size(100, 38), Location = new Point(190, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderColor = Border; btnCancel.Click += (s, e) => Close();
            scroll.Controls.Add(btnSave); scroll.Controls.Add(btnCancel);
            Controls.Add(scroll);
        }
    }
}