using System;
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
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private DataGridView dgv;
        private ComboBox cmbStatus;
        private TextBox txtSearch;
        private string userDept;
        private string userId;

        // leave_id, emp_id, emp_name, dept, type, start, end, days, approval_status, approved_by
        private readonly string[][] allRows = {
            new[]{"L001","001","Ahmed Hassan","Engineering","Annual",   "01/05/2025","05/05/2025","5","Approved","Sara Mohamed"},
            new[]{"L002","002","Sara Mohamed","Human Resources","Sick",     "22/04/2025","23/04/2025","2","Approved","Ahmed Hassan"},
            new[]{"L003","003","Omar Ali","Finance","Annual",   "10/05/2025","17/05/2025","7","Pending","--"},
            new[]{"L004","004","Nour Khaled","Engineering","Emergency","27/04/2025","28/04/2025","2","Pending","--"},
            new[]{"L005","005","Youssef Taha","Sales","Annual",   "01/06/2025","14/06/2025","14","Rejected","Sara Mohamed"},
            new[]{"L006","employee","Mohamed Ali","Engineering","Annual","15/05/2026","20/05/2026","5","Pending","--"},
        };

        public frmLeave(string department = "", string employeeId = "")
        {
            userDept = department;
            userId = employeeId;
            Text = string.IsNullOrEmpty(department) ? "Leave Management" : $"Leave Management - {department} Department";
            Size = new Size(1050, 680);
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

            txtSearch = new TextBox { Size = new Size(160, 28), Location = new Point(20, 12), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, ForeColor = TxtSecondary, Text = "Search employee..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search employee...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search employee..."; txtSearch.ForeColor = TxtSecondary; } };
            txtSearch.TextChanged += (s, e) => ApplyFilter();

            var lblStatus = new Label { Text = "Status:", AutoSize = true, Location = new Point(192, 17), ForeColor = TxtSecondary };
            cmbStatus = new ComboBox { Size = new Size(120, 28), Location = new Point(235, 12), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbStatus.Items.AddRange(new[] { "All", "Pending", "Approved", "Rejected" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += (s, e) => ApplyFilter();

            var btnNew = new Button { Text = "+ New Request", Size = new Size(120, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnNew.FlatAppearance.BorderSize = 0; btnNew.Click += (s, e) => new frmLeaveAdd().ShowDialog();

            toolbar.Controls.Add(txtSearch);
            toolbar.Controls.Add(lblStatus);
            toolbar.Controls.Add(cmbStatus);
            toolbar.Controls.Add(btnNew);

            // Only show Approve/Reject buttons for Admin and Manager (not for Employee)
            if (string.IsNullOrEmpty(userId))
            {
                var btnApprove = new Button { Text = "✓ Approve", Size = new Size(95, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(240, 253, 244), ForeColor = Color.FromArgb(21, 128, 61), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnApprove.FlatAppearance.BorderColor = Color.FromArgb(134, 239, 172);
                btnApprove.Click += (s, e) => {
                    if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Select a leave request first.", "Approve"); return; }
                    dgv.SelectedRows[0].Cells["approval"].Value = "Approved";
                    dgv.SelectedRows[0].Cells["approved_by"].Value = "Admin User";
                };
                toolbar.Controls.Add(btnApprove);

                var btnReject = new Button { Text = "✗ Reject", Size = new Size(85, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(254, 242, 242), ForeColor = Color.FromArgb(185, 28, 28), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnReject.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
                btnReject.Click += (s, e) => {
                    if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Select a leave request first.", "Reject"); return; }
                    dgv.SelectedRows[0].Cells["approval"].Value = "Rejected";
                };
                toolbar.Controls.Add(btnReject);

                toolbar.Resize += (s, e) => {
                    btnNew.Location = new Point(toolbar.Width - 135, 10);
                    btnReject.Location = new Point(toolbar.Width - 230, 10);
                    btnApprove.Location = new Point(toolbar.Width - 335, 10);
                };
            }
            else
            {
                toolbar.Resize += (s, e) => {
                    btnNew.Location = new Point(toolbar.Width - 135, 10);
                };
            }


            // Calculate stats based on filtered data
            int pending = 0, approved = 0, rejected = 0;

            foreach (var r in allRows)
            {
                string empId = r[1];
                string status = r[8];

                // Filter by employee ID if userId is provided
                if (!string.IsNullOrEmpty(userId) && empId != userId)
                    continue;
                // Filter by department if userDept is provided
                if (!string.IsNullOrEmpty(userDept) && r[3] != userDept)
                    continue;

                switch (status)
                {
                    case "Pending": pending++; break;
                    case "Approved": approved++; break;
                    case "Rejected": rejected++; break;
                }
            }

            // Stat cards
            Panel statsBar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = BgPage, Padding = new Padding(16, 8, 16, 8) };
            (string lbl, string val, Color clr)[] stats;

            if (string.IsNullOrEmpty(userId))
            {
                // Admin/Manager stats
                stats = new (string, string, Color)[]
                {
                    ("Pending", pending.ToString(), Color.FromArgb(180, 83, 9)),
                    ("Approved", approved.ToString(), Color.FromArgb(21, 128, 61)),
                    ("Rejected", rejected.ToString(), Color.FromArgb(185, 28, 28)),
                    ("Total", (pending + approved + rejected).ToString(), Color.FromArgb(29, 78, 216)),
                };
            }
            else
            {
                // Employee personal stats
                stats = new (string, string, Color)[]
                {
                    ("Pending", pending.ToString(), Color.FromArgb(180, 83, 9)),
                    ("Approved", approved.ToString(), Color.FromArgb(21, 128, 61)),
                    ("Rejected", rejected.ToString(), Color.FromArgb(185, 28, 28)),
                };
            }

            var statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = BgPage };
            foreach (var st in stats)
            {
                var card = new Panel { Size = new Size(155, 62), BackColor = BgCard, Margin = new Padding(0, 0, 10, 0) };
                card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
                card.Controls.Add(new Label { Text = st.val, Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = st.clr, AutoSize = true, Location = new Point(12, 6) });
                card.Controls.Add(new Label { Text = st.lbl, Font = new Font("Segoe UI", 8.5f), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(12, 38) });
                statsFlow.Controls.Add(card);
            }
            statsBar.Controls.Add(statsFlow);

            // Grid
            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("leave_id", "Leave ID"); dgv.Columns["leave_id"].FillWeight = 60;
            dgv.Columns.Add("emp_id", "Emp ID"); dgv.Columns["emp_id"].FillWeight = 55;
            dgv.Columns.Add("emp_name", "Employee");
            dgv.Columns.Add("type", "Leave Type"); dgv.Columns["type"].FillWeight = 75;
            dgv.Columns.Add("start", "Start Date"); dgv.Columns["start"].FillWeight = 80;
            dgv.Columns.Add("end", "End Date"); dgv.Columns["end"].FillWeight = 80;
            dgv.Columns.Add("days", "Days"); dgv.Columns["days"].FillWeight = 45;
            dgv.Columns.Add("approval", "Status"); dgv.Columns["approval"].FillWeight = 70;
            dgv.Columns.Add("approved_by", "Approved By"); dgv.Columns["approved_by"].FillWeight = 80;

            // Make all read-only except status/approved_by (updated via buttons)
            foreach (DataGridViewColumn col in dgv.Columns) col.ReadOnly = true;
            dgv.Columns["approval"].ReadOnly = false;
            dgv.Columns["approved_by"].ReadOnly = false;

            ApplyFilter();

            Controls.Add(dgv); Controls.Add(statsBar); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.ToLower().Trim();
            string status = cmbStatus.SelectedItem?.ToString() ?? "All";
            dgv.Rows.Clear();
            foreach (var r in allRows)
            {
                string empDept = r[3];
                string empId = r[1];
                bool matchDept = string.IsNullOrEmpty(userDept) || empDept == userDept;
                bool matchUser = string.IsNullOrEmpty(userId) || empId == userId;
                bool matchSearch = string.IsNullOrEmpty(search) || r[2].ToLower().Contains(search);
                bool matchStatus = status == "All" || r[8] == status;
                if (matchDept && matchUser && matchSearch && matchStatus)
                    dgv.Rows.Add(r[0], r[1], r[2], r[4], r[5], r[6], r[7], r[8], r[9]);
            }
        }
    }

    public class frmLeaveAdd : Form
    {
        public frmLeaveAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "New Leave Request"; Size = new Size(420, 440); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage }; int y = 0;
            pnl.Controls.Add(new Label { Text = "Employee ID *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle }); y += 56;
            var cmbType = new ComboBox { Size = new Size(360, 28), Location = new Point(0, y + 18), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbType.Items.AddRange(new[] { "Annual", "Sick", "Emergency", "Maternity", "Paternity", "Unpaid" }); cmbType.SelectedIndex = 0;
            pnl.Controls.Add(new Label { Text = "Leave Type *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); pnl.Controls.Add(cmbType); y += 56;
            pnl.Controls.Add(new Label { Text = "Start Date *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            pnl.Controls.Add(new Label { Text = "End Date *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            pnl.Controls.Add(new Label { Text = "Notes", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 55), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, Multiline = true }); y += 80;
            var btnS = new Button { Text = "Submit Request", Size = new Size(170, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnS.FlatAppearance.BorderSize = 0; btnS.Click += (s, e) => { MessageBox.Show("Request submitted! Connect to DB.", "Submitted"); Close(); };
            var btnC = new Button { Text = "Cancel", Size = new Size(90, 38), Location = new Point(185, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnC.FlatAppearance.BorderColor = Border; btnC.Click += (s, e) => Close();
            pnl.Controls.Add(btnS); pnl.Controls.Add(btnC); Controls.Add(pnl);
        }
    }
}