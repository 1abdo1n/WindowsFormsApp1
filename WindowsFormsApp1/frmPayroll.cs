using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class frmPayroll : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private DataGridView dgv;
        private ComboBox cmbMonth;
        private TextBox txtSearch;

        // pay_id, emp_id, emp_name, base, social, risk, transport, bonus, deductions, tax, insurance, net, pay_date, month_key
        private readonly string[][] allRows = {
            new[]{"P001","001","Ahmed Hassan","15,000","500","300","400","2,000","800","750","500","16,150","30/01/2025","January 2025"},
            new[]{"P002","002","Sara Mohamed","12,000","500","200","400","1,000","600","600","400","12,500","30/01/2025","January 2025"},
            new[]{"P001","001","Ahmed Hassan","15,000","500","300","400","2,000","800","750","500","16,150","28/02/2025","February 2025"},
            new[]{"P002","002","Sara Mohamed","12,000","500","200","400","1,000","600","600","400","12,500","28/02/2025","February 2025"},
            new[]{"P001","001","Ahmed Hassan","15,000","500","300","400","2,000","800","750","500","16,150","31/03/2025","March 2025"},
            new[]{"P002","002","Sara Mohamed","12,000","500","200","400","0",    "600","600","400","11,500","31/03/2025","March 2025"},
            new[]{"P001","001","Ahmed Hassan","15,000","500","300","400","2,000","800","750","500","16,150","30/04/2025","April 2025"},
            new[]{"P002","002","Sara Mohamed","12,000","500","200","400","1,000","600","600","400","12,500","30/04/2025","April 2025"},
            new[]{"P003","003","Omar Ali",    "7,500", "300","100","400","500",  "400","375","250","7,775", "30/04/2025","April 2025"},
            new[]{"P004","004","Nour Khaled", "10,000","400","200","400","0",    "500","500","350","9,650", "30/04/2025","April 2025"},
            new[]{"P005","005","Youssef Taha","9,000", "350","150","400","1,500","450","450","300","10,200","30/04/2025","April 2025"},
        };

        public frmPayroll()
        {
            Text = "Payroll"; Size = new Size(1300, 680); MinimumSize = new Size(900, 540);
            StartPosition = FormStartPosition.CenterScreen; BackColor = BgPage; Font = new Font("Segoe UI", 9f);

            // Top bar
            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = "Payroll", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
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

            var lblMonth = new Label { Text = "Month:", AutoSize = true, Location = new Point(193, 17), ForeColor = TxtSecondary };
            cmbMonth = new ComboBox { Size = new Size(145, 28), Location = new Point(240, 12), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbMonth.Items.AddRange(new[] { "All", "January 2025", "February 2025", "March 2025", "April 2025" });
            cmbMonth.SelectedIndex = 0;
            cmbMonth.SelectedIndexChanged += (s, e) => ApplyFilter();

            var btnProcess = new Button { Text = "⚙ Process Payroll", Size = new Size(145, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnProcess.FlatAppearance.BorderSize = 0;
            btnProcess.Click += (s, e) => MessageBox.Show($"Processing payroll for: {cmbMonth.SelectedItem}", "Process Payroll");

            var btnAdd = new Button { Text = "+ Add Record", Size = new Size(115, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(240, 253, 244), ForeColor = Color.FromArgb(21, 128, 61), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAdd.FlatAppearance.BorderColor = Color.FromArgb(134, 239, 172);
            btnAdd.Click += (s, e) => new frmPayrollAdd().ShowDialog();

            var btnExport = new Button { Text = "↓ Export", Size = new Size(85, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtPrimary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnExport.FlatAppearance.BorderColor = Border;

            toolbar.Controls.Add(txtSearch); toolbar.Controls.Add(lblMonth); toolbar.Controls.Add(cmbMonth);
            toolbar.Controls.Add(btnProcess); toolbar.Controls.Add(btnAdd); toolbar.Controls.Add(btnExport);
            toolbar.Resize += (s, e) => { btnProcess.Location = new Point(toolbar.Width - 155, 10); btnAdd.Location = new Point(toolbar.Width - 280, 10); btnExport.Location = new Point(toolbar.Width - 375, 10); };

            // Summary cards
            Panel statsBar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = BgPage, Padding = new Padding(16, 8, 16, 8) };
            (string lbl, string val, Color clr)[] stats = {
                ("Total Base Salary","532,500 EGP", Color.FromArgb(29,78,216)),
                ("Total Allowances", "87,000 EGP",  Color.FromArgb(21,128,61)),
                ("Total Bonuses",    "45,000 EGP",  Color.FromArgb(109,40,217)),
                ("Total Deductions", "28,000 EGP",  Color.FromArgb(185,28,28)),
                ("Net Payroll",      "636,500 EGP", Color.FromArgb(15,118,110)),
            };
            var statsFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = BgPage };
            foreach (var st in stats)
            {
                var card = new Panel { Size = new Size(188, 62), BackColor = BgCard, Margin = new Padding(0, 0, 10, 0) };
                card.Paint += (s2, e2) => ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
                card.Controls.Add(new Label { Text = st.val, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = st.clr, AutoSize = true, Location = new Point(10, 8) });
                card.Controls.Add(new Label { Text = st.lbl, Font = new Font("Segoe UI", 8f), ForeColor = TxtSecondary, AutoSize = true, Location = new Point(10, 36) });
                statsFlow.Controls.Add(card);
            }
            statsBar.Controls.Add(statsFlow);

            // Grid
            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f); dgv.DefaultCellStyle.ForeColor = TxtPrimary; dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;

            dgv.Columns.Add("pay_id", "Pay ID"); dgv.Columns["pay_id"].FillWeight = 55;
            dgv.Columns.Add("emp_id", "Emp ID"); dgv.Columns["emp_id"].FillWeight = 55;
            dgv.Columns.Add("emp_name", "Employee");
            dgv.Columns.Add("base_salary", "Base Salary");
            dgv.Columns.Add("social_allow", "Social Allow.");
            dgv.Columns.Add("risk_allow", "Risk Allow.");
            dgv.Columns.Add("transport_allow", "Transport");
            dgv.Columns.Add("prod_bonus", "Bonus");
            dgv.Columns.Add("deductions", "Deductions");
            dgv.Columns.Add("tax", "Tax");
            dgv.Columns.Add("insurance", "Insurance");
            dgv.Columns.Add("net_salary", "Net Salary");
            dgv.Columns.Add("pay_date", "Pay Date"); dgv.Columns["pay_date"].FillWeight = 80;
            // month_key column — hidden, used for filtering
            dgv.Columns.Add("month_key", "Month"); dgv.Columns["month_key"].Visible = false;

            ApplyFilter(); // load all rows initially

            Controls.Add(dgv); Controls.Add(statsBar); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text == "Search employee...") ? "" : txtSearch.Text.ToLower().Trim();
            string month = cmbMonth.SelectedItem?.ToString() ?? "All";
            dgv.Rows.Clear();
            foreach (var r in allRows)
            {
                bool matchSearch = string.IsNullOrEmpty(search) || r[2].ToLower().Contains(search) || r[1].Contains(search);
                bool matchMonth = month == "All" || r[13] == month;
                if (matchSearch && matchMonth) dgv.Rows.Add(r);
            }
        }
    }

    public class frmPayrollAdd : Form
    {
        public frmPayrollAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "Add Payroll Record"; Size = new Size(480, 680); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage }; int y = 0;
            (string lbl, string name)[] fields = { ("Employee ID *", "emp_id"), ("Base Salary *", "base"), ("Social Allowance", "social"), ("Risk Allowance", "risk"), ("Transport Allowance", "transport"), ("Production Bonus", "bonus"), ("Deductions", "ded"), ("Tax", "tax"), ("Insurance", "ins") };
            foreach (var f in fields) { scroll.Controls.Add(new Label { Text = f.lbl, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); scroll.Controls.Add(new TextBox { Name = f.name, Size = new Size(420, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(249, 250, 251) }); y += 56; }
            scroll.Controls.Add(new Label { Text = "Pay Date *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            scroll.Controls.Add(new DateTimePicker { Size = new Size(420, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            var btnS = new Button { Text = "Save Record", Size = new Size(180, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnS.FlatAppearance.BorderSize = 0; btnS.Click += (s, e) => { MessageBox.Show("Saved! Connect to DB.", "Saved"); Close(); };
            var btnC = new Button { Text = "Cancel", Size = new Size(100, 38), Location = new Point(190, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnC.FlatAppearance.BorderColor = Border; btnC.Click += (s, e) => Close();
            scroll.Controls.Add(btnS); scroll.Controls.Add(btnC); Controls.Add(scroll);
        }
    }
}