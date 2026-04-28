using System;
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
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private DataGridView dgv;
        private ComboBox cmbShift;
        private TextBox txtSearch;
        private string userDept;
        private string userId;

        // att_id, emp_id, emp_name, dept, date, check_in, check_out, shift, work_hrs, status, ot_hrs
        private readonly string[][] allRows = {
    new[]{"A001","001","Ahmed Hassan","Engineering","27/04/2025","08:55","17:10","Morning","8.25","Present","1.25"},
    new[]{"A002","002","Sara Mohamed","Human Resources","27/04/2025","09:02","17:00","Morning","7.97","Present","0.00"},
    new[]{"A003","003","Omar Ali","Finance","27/04/2025","10:15","17:00","Morning","6.75","Late","0.00"},
    new[]{"A004","004","Nour Khaled","Engineering","27/04/2025","--","--","--","--","Absent","--"},
    new[]{"A005","005","Youssef Taha","Sales","27/04/2025","08:30","18:30","Morning","9.00","Present","2.00"},
    new[]{"A006","001","Ahmed Hassan","Engineering","26/04/2025","20:00","04:00","Night","8.00","Present","0.00"},
    new[]{"A007","003","Omar Ali","Finance","26/04/2025","14:00","22:00","Evening","8.00","Present","0.00"},
    new[]{"A008","employee","Mohamed Ali","Engineering","28/04/2026","09:00","17:00","Morning","8.00","Present","0.00"},
};

        public frmAttendance(string department = "", string employeeId = "")
        {
            userDept = department;
            userId = employeeId;
            Text = string.IsNullOrEmpty(department) ? "Attendance" : $"Attendance - {department} Department";
            Size = new Size(1100, 680);
            MinimumSize = new Size(800, 540);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BgPage;
            Font = new Font("Segoe UI", 9f);

            // Top bar
            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = Text, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
            var btnBack = new Button { Text = "← Back", Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border; btnBack.Click += (s, e) => Close();
            topbar.Controls.Add(btnBack); topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            // Toolbar
            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

            txtSearch = new TextBox { Size = new Size(155, 28), Location = new Point(20, 12), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, ForeColor = TxtSecondary, Text = "Search employee..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search employee...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search employee..."; txtSearch.ForeColor = TxtSecondary; } };
            txtSearch.TextChanged += (s, e) => ApplyFilter();

            var lblDate = new Label { Text = "Date:", AutoSize = true, Location = new Point(188, 17), ForeColor = TxtSecondary };
            var dtp = new DateTimePicker { Size = new Size(200, 28), Location = new Point(225, 12), Font = new Font("Segoe UI", 9f) };
            dtp.Value = DateTime.Today;
            dtp.ValueChanged += (s, e) => ApplyFilter(dtp.Value.ToString("dd/MM/yyyy"));

            var lblShift = new Label { Text = "Shift:", AutoSize = true, Location = new Point(440, 17), ForeColor = TxtSecondary };
            cmbShift = new ComboBox { Size = new Size(110, 28), Location = new Point(475, 12), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbShift.Items.AddRange(new[] { "All", "Morning", "Evening", "Night" });
            cmbShift.SelectedIndex = 0;
            cmbShift.SelectedIndexChanged += (s, e) => ApplyFilter();

            // Only show Record button for Admin and Manager (not for Employee)
            if (string.IsNullOrEmpty(userId))
            {
                var btnRecord = new Button { Text = "+ Record Attendance", Size = new Size(155, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnRecord.FlatAppearance.BorderSize = 0;
                btnRecord.Click += (s, e) => new frmAttendanceAdd().ShowDialog();
                toolbar.Controls.Add(btnRecord);
                toolbar.Resize += (s, e) => btnRecord.Location = new Point(toolbar.Width - 168, 10);
            }

            toolbar.Controls.Add(txtSearch); toolbar.Controls.Add(lblDate); toolbar.Controls.Add(dtp);
            toolbar.Controls.Add(lblShift); toolbar.Controls.Add(cmbShift);

            // Stat cards
            Panel statsBar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = BgPage, Padding = new Padding(16, 8, 16, 8) };
            var statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = BgPage };

            // Calculate stats based on filtered data
            int present = 0, absent = 0, late = 0, onLeave = 0, overtime = 0;

            foreach (var r in allRows)
            {
                string empId = r[1];
                string empDept = r[3];
                string status = r[9];
                string otHrs = r[10];

                // Filter by department if userDept is provided (Manager)
                if (!string.IsNullOrEmpty(userDept) && empDept != userDept)
                    continue;

                // Filter by employee ID if userId is provided (Employee)
                if (!string.IsNullOrEmpty(userId) && empId != userId)
                    continue;

                switch (status)
                {
                    case "Present": present++; break;
                    case "Absent": absent++; break;
                    case "Late": late++; break;
                    case "On Leave": onLeave++; break;
                }

                double workHrs;
                if (double.TryParse(otHrs, out workHrs) && workHrs > 0)
                    overtime++;
            }

            (string lbl, string val, Color clr)[] stats;
            if (string.IsNullOrEmpty(userId))
            {
                // Admin/Manager stats
                stats = new (string, string, Color)[]
                {
        ("Present", present.ToString(), Color.FromArgb(21, 128, 61)),
        ("Absent", absent.ToString(), Color.FromArgb(185, 28, 28)),
        ("Late", late.ToString(), Color.FromArgb(180, 83, 9)),
        ("On Leave", onLeave.ToString(), Color.FromArgb(29, 78, 216)),
        ("Overtime", overtime.ToString(), Color.FromArgb(109, 40, 217)),
                };
            }
            else
            {
                // Employee personal stats
                stats = new (string, string, Color)[]
                {
        ("Present", present.ToString(), Color.FromArgb(21, 128, 61)),
        ("Absent", absent.ToString(), Color.FromArgb(185, 28, 28)),
        ("Late", late.ToString(), Color.FromArgb(180, 83, 9)),
        ("On Leave", onLeave.ToString(), Color.FromArgb(29, 78, 216)),
                };
            }

            foreach (var st in stats)
            {
                var card = new Panel { Size = new Size(155, 62), BackColor = BgCard, Margin = new Padding(0, 0, 10, 0) };
                card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
                card.Controls.Add(new Label { Text = st.val, Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = st.clr, AutoSize = true, Location = new Point(12, 6) });
                card.Controls.Add(new Label { Text = st.lbl, Font = new Font("Segoe UI", 8.5f), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(12, 38) });
                statsFlow.Controls.Add(card);
            }
            statsBar.Controls.Add(statsFlow);
            Controls.Add(statsBar);

            // Grid
            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("att_id", "Att. ID");
            dgv.Columns.Add("emp_id", "Emp ID");
            dgv.Columns.Add("emp_name", "Employee");
            dgv.Columns.Add("date", "Date");
            dgv.Columns.Add("check_in", "Check In");
            dgv.Columns.Add("check_out", "Check Out");
            dgv.Columns.Add("shift", "Shift");
            dgv.Columns.Add("work_hrs", "Work Hrs");
            dgv.Columns.Add("ot_hrs", "Overtime Hrs");

            ApplyFilter();

            Controls.Add(dgv); Controls.Add(statsBar); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyFilter(string dateFilter = null)
        {
            string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.ToLower().Trim();
            string shift = cmbShift.SelectedItem?.ToString() ?? "All";

            dgv.Rows.Clear();
            foreach (var r in allRows)
            {
                string empDept = r[3];
                string empId = r[1];
                bool matchDept = string.IsNullOrEmpty(userDept) || empDept == userDept;
                bool matchUser = string.IsNullOrEmpty(userId) || empId == userId;
                bool matchSearch = string.IsNullOrEmpty(search) || r[2].ToLower().Contains(search) || empId.Contains(search);
                bool matchShift = shift == "All" || r[7] == shift;
                bool matchDate = dateFilter == null || r[4] == dateFilter;

                if (matchDept && matchUser && matchSearch && matchShift && matchDate)
                {
                    dgv.Rows.Add(r[0], r[1], r[2], r[4], r[5], r[6], r[7], r[8], r[10]);
                }
            }
        }
    }

    public class frmAttendanceAdd : Form
    {
        public frmAttendanceAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "Record Attendance"; Size = new Size(420, 440); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage }; int y = 0;
            pnl.Controls.Add(new Label { Text = "Employee ID *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle }); y += 56;
            pnl.Controls.Add(new Label { Text = "Date *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            pnl.Controls.Add(new Label { Text = "Check In Time (HH:MM)", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle }); y += 56;
            pnl.Controls.Add(new Label { Text = "Check Out Time (HH:MM)", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle }); y += 56;
            var cmbShift = new ComboBox { Size = new Size(360, 28), Location = new Point(0, y + 18), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbShift.Items.AddRange(new[] { "Morning", "Evening", "Night" }); cmbShift.SelectedIndex = 0;
            pnl.Controls.Add(new Label { Text = "Shift Type *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); pnl.Controls.Add(cmbShift); y += 56;
            var btnS = new Button { Text = "Save Record", Size = new Size(170, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnS.FlatAppearance.BorderSize = 0; btnS.Click += (s, e) => { MessageBox.Show("Attendance saved! Connect to DB.", "Saved"); Close(); };
            var btnC = new Button { Text = "Cancel", Size = new Size(90, 38), Location = new Point(185, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnC.FlatAppearance.BorderColor = Border; btnC.Click += (s, e) => Close();
            pnl.Controls.Add(btnS); pnl.Controls.Add(btnC); Controls.Add(pnl);
        }
    }
}