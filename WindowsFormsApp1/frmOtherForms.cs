using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // ─────────────────────────────────────────────────────────────
    //  frmDepartments  — department_id, department_name, manager_id
    // ─────────────────────────────────────────────────────────────
    public class frmDepartments : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);
        private DataGridView dgv;

        public frmDepartments()
        {
            Text = "Departments"; Size = new Size(900, 600); MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterScreen; BackColor = BgPage; Font = new Font("Segoe UI", 9f);

            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = "Departments", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
            var btnBack = new Button { Text = "← Back", Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border; btnBack.Click += (s, e) => Close();
            topbar.Controls.Add(btnBack); topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

            var btnAdd = new Button { Text = "+ Add Department", Size = new Size(145, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => new frmDepartmentAdd().ShowDialog();
            var btnEdit = new Button { Text = "✎ Edit", Size = new Size(75, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtPrimary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnEdit.FlatAppearance.BorderColor = Border;
            var btnDel = new Button { Text = "🗑 Delete", Size = new Size(90, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(254, 242, 242), ForeColor = Color.FromArgb(185, 28, 28), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnDel.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            toolbar.Controls.Add(btnAdd); toolbar.Controls.Add(btnEdit); toolbar.Controls.Add(btnDel);
            toolbar.Resize += (s, e) => { btnAdd.Location = new Point(toolbar.Width - 160, 10); btnDel.Location = new Point(toolbar.Width - 260, 10); btnEdit.Location = new Point(toolbar.Width - 345, 10); };

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("dept_id", "Dept ID"); dgv.Columns["dept_id"].FillWeight = 60;
            dgv.Columns.Add("name", "Department Name");
            dgv.Columns.Add("mgr_id", "Manager ID"); dgv.Columns["mgr_id"].FillWeight = 70;
            dgv.Columns.Add("mgr_name", "Manager Name");
            dgv.Columns.Add("count", "Employees"); dgv.Columns["count"].FillWeight = 65;
            dgv.Columns.Add("location", "Location"); dgv.Columns["location"].FillWeight = 80;

            dgv.Rows.Add("D01", "Engineering", "001", "Ahmed Hassan", "42", "Floor 3");
            dgv.Rows.Add("D02", "Human Resources", "002", "Sara Mohamed", "18", "Floor 1");
            dgv.Rows.Add("D03", "Finance", "003", "Omar Ali", "15", "Floor 2");
            dgv.Rows.Add("D04", "Sales", "004", "Nour Khaled", "28", "Floor 1");
            dgv.Rows.Add("D05", "Operations", "005", "Youssef Taha", "22", "Floor 4");

            Controls.Add(dgv); Controls.Add(toolbar); Controls.Add(topbar);
        }
    }

    public class frmDepartmentAdd : Form
    {
        public frmDepartmentAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "Add Department"; Size = new Size(420, 310); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage };
            int y = 0;
            void Row(string lbl, Control ctrl) { pnl.Controls.Add(new Label { Text = lbl, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); ctrl.Location = new Point(0, y + 18); ctrl.Size = new Size(360, 28); pnl.Controls.Add(ctrl); y += 56; }
            Row("Department Name *", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });
            Row("Manager ID", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });
            Row("Location", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });
            var btnSave = new Button { Text = "Save", Size = new Size(160, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += (s, e) => { MessageBox.Show("Saved! Connect to DB.", "Saved"); Close(); };
            var btnCancel = new Button { Text = "Cancel", Size = new Size(90, 38), Location = new Point(175, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnCancel.FlatAppearance.BorderColor = Border; btnCancel.Click += (s, e) => Close();
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnCancel); Controls.Add(pnl);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmAssets  — asset_id, asset_name, asset_type, employee_id, assigned_date, status
    // ─────────────────────────────────────────────────────────────
    public class frmAssets : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);
        private DataGridView dgv;
        private TextBox txtSearch;
        private string userDept;

        public frmAssets(string department = "")
        {
            userDept = department;
            Text = string.IsNullOrEmpty(department) ? "Assets" : $"Assets - {department} Department";
            Size = new Size(1000, 620);
            MinimumSize = new Size(750, 480);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BgPage;
            Font = new Font("Segoe UI", 9f);

            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = Text, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
            var btnBack = new Button { Text = "← Back", Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border; btnBack.Click += (s, e) => Close();
            topbar.Controls.Add(btnBack); topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

            txtSearch = new TextBox { Size = new Size(200, 28), Location = new Point(20, 12), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, ForeColor = TxtSecondary, Text = "Search assets..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search assets...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search assets..."; txtSearch.ForeColor = TxtSecondary; } };
            txtSearch.TextChanged += (s, e) => ApplyAssetFilter();

            // Only show Add/Edit/Delete buttons for Admin (no department filter)
            if (string.IsNullOrEmpty(userDept))
            {
                var btnAdd = new Button { Text = "+ Add Asset", Size = new Size(110, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnAdd.FlatAppearance.BorderSize = 0; btnAdd.Click += (s, e) => new frmAssetAdd().ShowDialog();
                toolbar.Controls.Add(btnAdd);

                var btnEdit = new Button { Text = "✎ Edit", Size = new Size(75, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtPrimary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnEdit.FlatAppearance.BorderColor = Border;
                toolbar.Controls.Add(btnEdit);

                var btnDel = new Button { Text = "🗑 Delete", Size = new Size(90, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(254, 242, 242), ForeColor = Color.FromArgb(185, 28, 28), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
                btnDel.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
                toolbar.Controls.Add(btnDel);

                toolbar.Resize += (s, e) => {
                    btnAdd.Location = new Point(toolbar.Width - 125, 10);
                    btnDel.Location = new Point(toolbar.Width - 225, 10);
                    btnEdit.Location = new Point(toolbar.Width - 310, 10);
                };
            }

            toolbar.Controls.Add(txtSearch);

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("asset_id", "Asset ID"); dgv.Columns["asset_id"].FillWeight = 60;
            dgv.Columns.Add("name", "Asset Name");
            dgv.Columns.Add("type", "Asset Type"); dgv.Columns["type"].FillWeight = 80;
            dgv.Columns.Add("emp_id", "Emp ID"); dgv.Columns["emp_id"].FillWeight = 55;
            dgv.Columns.Add("emp_name", "Assigned To");
            dgv.Columns.Add("assign_date", "Assigned Date"); dgv.Columns["assign_date"].FillWeight = 80;
            dgv.Columns.Add("status", "Status"); dgv.Columns["status"].FillWeight = 65;

            // All assets data with department info
            var allAssets = new[]
            {
            new object[] { "A001", "Dell Laptop", "Electronics", "001", "Ahmed Hassan", "Engineering", "15/01/2023", "In Use" },
            new object[] { "A002", "Office Chair", "Furniture", "002", "Sara Mohamed", "Human Resources", "01/03/2022", "In Use" },
            new object[] { "A003", "iPhone 14", "Electronics", "003", "Omar Ali", "Finance", "10/06/2023", "In Use" },
            new object[] { "A004", "MacBook Pro", "Electronics", "--", "Unassigned", "Engineering", "20/09/2023", "Available" },
            new object[] { "A005", "Projector", "Electronics", "--", "Meeting Room", "Sales", "05/02/2022", "In Use" },
        };

            // Filter by department if Manager
            foreach (var asset in allAssets)
            {
                string assetDept = asset[5].ToString();
                if (string.IsNullOrEmpty(userDept) || assetDept == userDept)
                {
                    dgv.Rows.Add(asset[0], asset[1], asset[2], asset[3], asset[4], asset[6], asset[7]);
                }
            }

            Controls.Add(dgv); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyAssetFilter()
        {
            string search = txtSearch.Text.ToLower().Trim();
            if (search == "search assets...")
            {
                search = "";
            }

            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool visible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(search))
                    {
                        visible = true;
                        break;
                    }
                }
                row.Visible = visible;
            }
        }
    }

    public class frmAssetAdd : Form
    {
        public frmAssetAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "Add Asset"; Size = new Size(420, 400); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage };
            int y = 0;
            void Row(string lbl, Control ctrl) { pnl.Controls.Add(new Label { Text = lbl, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); ctrl.Location = new Point(0, y + 18); ctrl.Size = new Size(360, 28); pnl.Controls.Add(ctrl); y += 56; }
            Row("Asset Name *", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });
            var cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbType.Items.AddRange(new[] { "Electronics", "Furniture", "Vehicle", "Equipment", "Other" }); cmbType.SelectedIndex = 0;
            Row("Asset Type *", cmbType);
            Row("Assigned To (Emp ID)", new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) });
            pnl.Controls.Add(new Label { Text = "Assigned Date", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            var btnSave = new Button { Text = "Save Asset", Size = new Size(160, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += (s, e) => { MessageBox.Show("Asset saved! Connect to DB.", "Saved"); Close(); };
            var btnCancel = new Button { Text = "Cancel", Size = new Size(90, 38), Location = new Point(175, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnCancel.FlatAppearance.BorderColor = Border; btnCancel.Click += (s, e) => Close();
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnCancel); Controls.Add(pnl);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmReports  — summary of all ERD entities with role filtering
    // ─────────────────────────────────────────────────────────────
    public class frmReports : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private Panel contentPanel;
        private FlowLayoutPanel statsFlow;
        private string userDept;
        private string userId;

        public frmReports(string department = "", string employeeId = "")
        {
            userDept = department;
            userId = employeeId;
            Text = "Reports";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgPage;
            this.Font = new Font("Segoe UI", 9f);
            this.WindowState = FormWindowState.Normal;
            this.FormClosing += (s, e) => { if (e.CloseReason == CloseReason.UserClosing) this.Hide(); e.Cancel = true; };

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Top bar
            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, topbar.Height - 1, topbar.Width, topbar.Height - 1);

            string titleText = "Reports";
            if (!string.IsNullOrEmpty(userDept))
                titleText = $"Reports - {userDept} Department";
            else if (!string.IsNullOrEmpty(userId))
                titleText = "My Reports";

            Label lblTitle = new Label { Text = titleText, Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 15) };

            Button btnBack = new Button { Text = "← Back", Size = new Size(80, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border;
            btnBack.Click += (s, e) => this.Hide();

            topbar.Controls.Add(lblTitle);
            topbar.Controls.Add(btnBack);
            topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            // Stats Bar (different stats based on role)
            Panel statsBar = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = BgPage, Padding = new Padding(20, 12, 20, 8) };
            statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, BackColor = BgPage };

            (string lbl, string val, Color clr)[] stats;

            if (string.IsNullOrEmpty(userDept) && string.IsNullOrEmpty(userId))
            {
                // Admin stats
                stats = new (string, string, Color)[]
                {
                ("Total Employees", "52", Color.FromArgb(29,78,216)),
                ("Departments", "5", Color.FromArgb(109,40,217)),
                ("Active Contracts", "48", Color.FromArgb(21,128,61)),
                ("Monthly Payroll", "636K", Color.FromArgb(15,118,110)),
                ("Pending Leaves", "4", Color.FromArgb(180,83,9)),
                ("Total Assets", "37", Color.FromArgb(185,28,28)),
                };
            }
            else if (!string.IsNullOrEmpty(userDept))
            {
                // Manager stats (department only)
                stats = new (string, string, Color)[]
                {
                ($"{userDept} Employees", "18", Color.FromArgb(29,78,216)),
                ("Pending Leaves", "3", Color.FromArgb(180,83,9)),
                ("Assets in Dept", "12", Color.FromArgb(185,28,28)),
                ("Attendance Rate", "94%", Color.FromArgb(21,128,61)),
                };
            }
            else
            {
                // Employee stats
                stats = new (string, string, Color)[]
                {
                ("Vacation Days", "22", Color.FromArgb(29,78,216)),
                ("My Attendance", "96%", Color.FromArgb(21,128,61)),
                ("My Performance", "4.8", Color.FromArgb(109,40,217)),
                ("Leave Requests", "3", Color.FromArgb(180,83,9)),
                };
            }

            foreach (var st in stats)
            {
                Panel card = new Panel { Size = new Size(170, 72), BackColor = BgCard, Margin = new Padding(0, 0, 15, 8) };
                card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);

                Label lblValue = new Label { Text = st.val, Font = new Font("Segoe UI", 20f, FontStyle.Bold), ForeColor = st.clr, AutoSize = true, Location = new Point(12, 10) };
                Label lblLabel = new Label { Text = st.lbl, Font = new Font("Segoe UI", 8.5f), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(12, 48) };

                card.Controls.Add(lblValue);
                card.Controls.Add(lblLabel);
                statsFlow.Controls.Add(card);
            }
            statsBar.Controls.Add(statsFlow);

            // Content Panel (Scrollable)
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = BgPage, AutoScroll = true };

            Label lblGenerate = new Label { Text = "GENERATE REPORTS", Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(20, 15) };
            contentPanel.Controls.Add(lblGenerate);

            // Reports based on role
            (string Name, string Desc, string Fields, Color Accent, Color AText)[] rpts;

            if (string.IsNullOrEmpty(userDept) && string.IsNullOrEmpty(userId))
            {
                // Admin reports (all)
                rpts = new (string, string, string, Color, Color)[]
                {
                ("Employee Report",    "Full employee records",           "employee_id, name, national_id",        Color.FromArgb(219,234,254), Color.FromArgb(29,78,216)),
                ("Attendance Report",  "Monthly attendance",              "check_in, check_out, work_hours",       Color.FromArgb(220,252,231), Color.FromArgb(21,128,61)),
                ("Payroll Report",     "Salary breakdown",                "base, allowances, bonuses, net",        Color.FromArgb(237,233,254), Color.FromArgb(109,40,217)),
                ("Leave Report",       "Leave requests",                  "leave_type, dates, status",             Color.FromArgb(254,243,199), Color.FromArgb(180,83,9)),
                ("Contract Report",    "Contract status",                 "contract_type, start_date, end_date",   Color.FromArgb(204,251,241), Color.FromArgb(15,118,110)),
                ("Asset Report",       "Asset inventory",                 "asset_name, type, assigned_to",         Color.FromArgb(255,237,213), Color.FromArgb(194,65,12)),
                ("Department Report",  "Dept headcount",                  "department_name, manager, count",       Color.FromArgb(243,244,246), Color.FromArgb(75,85,99)),
                ("Performance Report", "Evaluation scores",               "technical, attendance, safety, rating", Color.FromArgb(254,226,226), Color.FromArgb(185,28,28)),
                };
            }
            else if (!string.IsNullOrEmpty(userDept))
            {
                // Manager reports (department only)
                rpts = new (string, string, string, Color, Color)[]
                {
                ("Employee Report",    $"Department employees",           "employee_id, name, position",           Color.FromArgb(219,234,254), Color.FromArgb(29,78,216)),
                ("Attendance Report",  "Team attendance",                 "check_in, check_out, work_hours",       Color.FromArgb(220,252,231), Color.FromArgb(21,128,61)),
                ("Leave Report",       "Team leave requests",              "leave_type, dates, status",             Color.FromArgb(254,243,199), Color.FromArgb(180,83,9)),
                ("Asset Report",       "Department assets",                "asset_name, type, assigned_to",         Color.FromArgb(255,237,213), Color.FromArgb(194,65,12)),
                };
            }
            else
            {
                // Employee reports (personal only)
                rpts = new (string, string, string, Color, Color)[]
                {
                ("My Attendance",      "My attendance record",             "date, check_in, check_out, status",     Color.FromArgb(220,252,231), Color.FromArgb(21,128,61)),
                ("My Leave",           "My leave requests",                "leave_type, from_date, to_date",        Color.FromArgb(254,243,199), Color.FromArgb(180,83,9)),
                ("My Performance",     "My evaluation scores",             "technical, attendance, safety",         Color.FromArgb(219,234,254), Color.FromArgb(29,78,216)),
                ("My Contract",        "My employment contract",           "contract_type, start_date, end_date",   Color.FromArgb(204,251,241), Color.FromArgb(15,118,110)),
                };
            }

            int cardWidth = 340;
            int cardHeight = 115;
            int margin = 20;
            int cardsPerRow = 3;
            int startY = 55;

            for (int i = 0; i < rpts.Length; i++)
            {
                int col = i % cardsPerRow;
                int row = i / cardsPerRow;
                int x = 20 + col * (cardWidth + margin);
                int y = startY + row * (cardHeight + margin);

                Panel card = new Panel { Size = new Size(cardWidth, cardHeight), Location = new Point(x, y), BackColor = BgCard, Cursor = Cursors.Hand };
                card.Paint += (s, e) => ((Panel)s).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);

                Panel pill = new Panel { Size = new Size(6, cardHeight), Location = new Point(0, 0), BackColor = rpts[i].Accent };

                Label lName = new Label { Text = rpts[i].Name, Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = rpts[i].AText, AutoSize = true, Location = new Point(20, 12) };
                Label lDesc = new Label { Text = rpts[i].Desc, Font = new Font("Segoe UI", 8.5f), ForeColor = TxtSecondary, Size = new Size(300, 20), Location = new Point(20, 40) };
                Label lFields = new Label { Text = rpts[i].Fields, Font = new Font("Segoe UI", 8f), ForeColor = Color.FromArgb(156, 163, 175), Size = new Size(300, 20), Location = new Point(20, 62) };

                Button btnExp = new Button { Text = "↓ Export", Size = new Size(85, 30), Location = new Point(cardWidth - 105, cardHeight - 38), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
                btnExp.FlatAppearance.BorderSize = 0;
                string nm = rpts[i].Name;
                string roleInfo = !string.IsNullOrEmpty(userDept) ? $" for {userDept} Department" : (!string.IsNullOrEmpty(userId) ? " for you" : "");
                btnExp.Click += (s, e) => MessageBox.Show($"Exporting {nm}{roleInfo}...\nConnect to DB to generate real data.", "Export");

                card.Controls.Add(pill);
                card.Controls.Add(lName);
                card.Controls.Add(lDesc);
                card.Controls.Add(lFields);
                card.Controls.Add(btnExp);
                contentPanel.Controls.Add(card);
            }

            // Set content panel size for scrolling
            int totalRows = (int)Math.Ceiling((double)rpts.Length / cardsPerRow);
            contentPanel.AutoScrollMinSize = new Size(0, startY + totalRows * (cardHeight + margin) + 50);

            this.Controls.Add(contentPanel);
            this.Controls.Add(statsBar);
            this.Controls.Add(topbar);
        }
    }
}