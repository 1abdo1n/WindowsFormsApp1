using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // ─────────────────────────────────────────────────────────────
    //  Shared Theme Colors  (مش مكررة في كل class)
    // ─────────────────────────────────────────────────────────────
    internal static class Theme
    {
        public static readonly Color Blue = Color.FromArgb(26, 86, 219);
        public static readonly Color BgPage = Color.FromArgb(243, 244, 246);
        public static readonly Color BgCard = Color.White;
        public static readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        public static readonly Color TxtSec = Color.FromArgb(107, 114, 128);
        public static readonly Color Border = Color.FromArgb(220, 222, 226);
        public static readonly Color Danger = Color.FromArgb(185, 28, 28);
        public static readonly Color DangerBg = Color.FromArgb(254, 242, 242);
        public static readonly Color Success = Color.FromArgb(21, 128, 61);
        public static readonly Color Warning = Color.FromArgb(180, 83, 9);
        public static readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
        public static readonly Color WarningBg = Color.FromArgb(255, 251, 235);
    }

    // ─────────────────────────────────────────────────────────────
    //  UI Factory Helpers  (factory methods للعناصر المتكررة)
    // ─────────────────────────────────────────────────────────────
    internal static class UIHelper
    {
        /// <summary>Top-bar (height=52) بعنوان وزرار "← Back".</summary>
        public static Panel MakeTopbar(string title, EventHandler onBack)
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Theme.BgCard };
            bar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Theme.Border), 0, 51, bar.Width, 51);
            bar.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Theme.TxtPrimary,
                AutoSize = true,
                Location = new Point(20, 14)
            });
            var btn = MakeFlatBtn("← Back", 80, 30, Theme.BgCard, Theme.TxtSec, Theme.Border);
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn.Click += onBack;
            bar.Controls.Add(btn);
            bar.Resize += (s, e) => btn.Location = new Point(bar.Width - 100, 11);
            return bar;
        }

        /// <summary>Flat button جاهز بكل الخصائص المشتركة.</summary>
        public static Button MakeFlatBtn(string text, int w, int h,
            Color bg, Color fg, Color border, bool bold = false, int borderSize = 1)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Font = bold ? new Font("Segoe UI", 9f, FontStyle.Bold) : new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = border;
            btn.FlatAppearance.BorderSize = borderSize;
            return btn;
        }

        /// <summary>TextBox بـ placeholder text وأحداث GotFocus / LostFocus.</summary>
        public static TextBox MakeSearchBox(string placeholder, EventHandler onChange)
        {
            var txt = new TextBox
            {
                Size = new Size(200, 28),
                Location = new Point(20, 12),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme.TxtSec,
                Text = placeholder
            };
            txt.GotFocus += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Theme.TxtPrimary; } };
            txt.LostFocus += (s, e) => { if (txt.Text == "") { txt.Text = placeholder; txt.ForeColor = Theme.TxtSec; } };
            txt.TextChanged += onChange;
            return txt;
        }

        /// <summary>Stats bar panel مع FlowLayoutPanel جوّاه.</summary>
        public static Panel MakeStatsBar(out FlowLayoutPanel flow)
        {
            flow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = Theme.BgPage };
            var bar = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Theme.BgPage, Padding = new Padding(16, 8, 16, 8) };
            bar.Controls.Add(flow);
            return bar;
        }

        /// <summary>Stat card بيُضاف للـ flow ويرجع الـ value Label.</summary>
        public static Label MakeStatCard(FlowLayoutPanel flow, string lbl, Color valColor,
            int width = 200, int descY = 40)
        {
            var card = new Panel { Size = new Size(width, 62), BackColor = Theme.BgCard, Margin = new Padding(0, 0, 12, 0) };
            card.Paint += (s2, e2) =>
                ((Panel)s2).CreateGraphics().DrawRectangle(new Pen(Theme.Border), 0, 0, card.Width - 1, card.Height - 1);

            var valLbl = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = valColor,
                AutoSize = true,
                Location = new Point(12, 6)
            };
            card.Controls.Add(valLbl);
            card.Controls.Add(new Label
            {
                Text = lbl,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Theme.TxtSec,
                AutoSize = true,
                Location = new Point(12, descY)
            });
            flow.Controls.Add(card);
            return valLbl;
        }

        /// <summary>Toolbar panel (height=52) بخط سفلي.</summary>
        public static Panel MakeToolbar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Theme.BgCard };
            bar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Theme.Border), 0, 51, bar.Width, 51);
            return bar;
        }

        /// <summary>DataGridView بالستايل الموحّد.</summary>
        public static DataGridView MakeDgv()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Theme.BgCard,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 34 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Theme.Border
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Theme.TxtSec;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgv.DefaultCellStyle.ForeColor = Theme.TxtPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            return dgv;
        }

        /// <summary>يضيف Label + Control row على panel ويزوّد y بـ 56.</summary>
        public static void AddFormRow(Panel pnl, string lbl, Control ctrl, ref int y)
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
            ctrl.Size = new Size(390, 28);
            pnl.Controls.Add(ctrl);
            y += 56;
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmDepartments  — department_id, department_name, manager_id
    // ─────────────────────────────────────────────────────────────
    public class frmDepartments : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Label lblTotalDepts, lblTotalEmployees;

        public frmDepartments(string department = "")
        {
            Text = "Departments Management";
            Size = new Size(950, 650);
            MinimumSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.BgPage;
            Font = new Font("Segoe UI", 9f);
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            var topbar = UIHelper.MakeTopbar("🏢 Departments Management", (s, e) => Close());
            var statsBar = UIHelper.MakeStatsBar(out var statsFlow);
            lblTotalDepts = UIHelper.MakeStatCard(statsFlow, "Total Departments", Color.FromArgb(29, 78, 216));
            lblTotalEmployees = UIHelper.MakeStatCard(statsFlow, "Total Employees", Color.FromArgb(21, 128, 61));

            var toolbar = UIHelper.MakeToolbar();
            txtSearch = UIHelper.MakeSearchBox("Search department...", (s, e) => LoadData());

            var btnRefresh = UIHelper.MakeFlatBtn("⟳ Refresh", 90, 32, Theme.BgCard, Theme.TxtSec, Theme.Border);
            var btnDelete = UIHelper.MakeFlatBtn("🗑 Delete", 90, 32, Theme.DangerBg, Theme.Danger, Color.FromArgb(254, 202, 202));
            var btnEdit = UIHelper.MakeFlatBtn("✎ Edit", 75, 32, Theme.BgCard, Theme.TxtPrimary, Theme.Border);
            var btnAdd = UIHelper.MakeFlatBtn("+ Add Department", 145, 32, Theme.Blue, Color.White, Theme.Border, bold: true, borderSize: 0);

            btnRefresh.Anchor = btnDelete.Anchor = btnEdit.Anchor = btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnRefresh.Click += (s, e) => LoadData();
            btnDelete.Click += BtnDelete_Click;
            btnEdit.Click += BtnEdit_Click;
            btnAdd.Click += (s, e) => { if (new frmDepartmentAdd().ShowDialog() == DialogResult.OK) LoadData(); };

            toolbar.Controls.AddRange(new Control[] { txtSearch, btnRefresh, btnDelete, btnEdit, btnAdd });
            toolbar.Resize += (s, e) =>
            {
                btnAdd.Location = new Point(toolbar.Width - 160, 10);
                btnDelete.Location = new Point(toolbar.Width - 260, 10);
                btnEdit.Location = new Point(toolbar.Width - 345, 10);
                btnRefresh.Location = new Point(toolbar.Width - 445, 10);
            };

            dgv = UIHelper.MakeDgv();
            dgv.Columns.Add("department_id", "Dept ID"); dgv.Columns["department_id"].FillWeight = 60;
            dgv.Columns.Add("department_name", "Department Name");
            dgv.Columns.Add("manager_name", "Manager");
            dgv.Columns.Add("employee_count", "Employees"); dgv.Columns["employee_count"].FillWeight = 65;

            Controls.Add(dgv);
            Controls.Add(statsBar);
            Controls.Add(toolbar);
            Controls.Add(topbar);
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            string search = (txtSearch.Text == "Search department...") ? "" : txtSearch.Text.Trim();

            const string sql = @"
                SELECT 
                    d.department_id,
                    d.department_name,
                    e.full_name AS manager_name,
                    COUNT(emp.employee_id) AS employee_count
                FROM Department d
                LEFT JOIN Employee e   ON d.manager_id    = e.employee_id
                LEFT JOIN Employee emp ON d.department_id = emp.department_id
                WHERE 
                    (? = '' 
                        OR d.department_name LIKE '%' + ? + '%'
                        OR e.full_name       LIKE '%' + ? + '%')
                GROUP BY d.department_id, d.department_name, e.full_name
                ORDER BY d.department_name";

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand(sql, con))
                {
                    string pattern = "%" + search + "%";
                    cmd.Parameters.AddWithValue("?", search);
                    cmd.Parameters.AddWithValue("?", pattern);
                    cmd.Parameters.AddWithValue("?", pattern);
                    con.Open();

                    int totalDepts = 0, totalEmployees = 0;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalDepts++;
                            int empCount = Convert.ToInt32(reader["employee_count"]);
                            totalEmployees += empCount;
                            dgv.Rows.Add(
                                reader["department_id"].ToString(),
                                reader["department_name"].ToString(),
                                reader["manager_name"]?.ToString() ?? "—",
                                empCount);
                        }
                    }
                    lblTotalDepts.Text = totalDepts.ToString();
                    lblTotalEmployees.Text = totalEmployees.ToString();
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
            { MessageBox.Show("Please select a department to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!int.TryParse(dgv.CurrentRow.Cells["department_id"].Value?.ToString(), out int deptId)) return;
            string deptName = dgv.CurrentRow.Cells["department_name"].Value?.ToString() ?? "";

            if (new frmDepartmentAdd(deptId, deptName).ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            { MessageBox.Show("Please select a department to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!int.TryParse(dgv.CurrentRow.Cells["department_id"].Value?.ToString(), out int deptId)) return;
            string deptName = dgv.CurrentRow.Cells["department_name"].Value?.ToString() ?? "";

            if (MessageBox.Show($"Are you sure you want to delete '{deptName}' department?\n\nEmployees will be unassigned!",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand("DELETE FROM Department WHERE department_id = ?", con))
                {
                    cmd.Parameters.AddWithValue("?", deptId);
                    con.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    { MessageBox.Show("Department deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadData(); }
                    else
                        MessageBox.Show("Department not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting department:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmDepartmentAdd  — Add / Edit Department
    // ─────────────────────────────────────────────────────────────
    public class frmDepartmentAdd : Form
    {
        private readonly int? _deptId;
        private readonly bool _isEdit;
        private TextBox txtDeptName;
        private ComboBox cmbManager;
        private List<KeyValuePair<int, string>> _managerList;

        public frmDepartmentAdd(int? deptId = null, string deptName = "")
        {
            _deptId = deptId;
            _isEdit = deptId.HasValue;
            Text = _isEdit ? "Edit Department" : "Add Department";
            Size = new Size(450, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.BgPage;
            Font = new Font("Segoe UI", 9f);
            BuildUI(deptName);
            LoadManagers();
            if (_isEdit) LoadDepartmentData();
        }

        private void BuildUI(string deptName)
        {
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = Theme.BgPage };
            int y = 0;

            txtDeptName = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f), Text = deptName };
            UIHelper.AddFormRow(pnl, "Department Name *", txtDeptName, ref y);

            cmbManager = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f), DisplayMember = "Value", ValueMember = "Key" };
            UIHelper.AddFormRow(pnl, "Manager", cmbManager, ref y);

            var btnSave = new Button
            {
                Text = _isEdit ? "Save Changes" : "Save Department",
                Size = new Size(160, 38),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Blue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnCancel = UIHelper.MakeFlatBtn("Cancel", 90, 38, Color.White, Theme.TxtSec, Theme.Border);
            btnCancel.Location = new Point(175, y);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            pnl.Controls.Add(btnSave);
            pnl.Controls.Add(btnCancel);
            Controls.Add(pnl);
        }

        private void LoadManagers()
        {
            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand("SELECT employee_id, full_name FROM Employee WHERE status = 'Active' ORDER BY full_name", con))
                {
                    con.Open();
                    _managerList = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "-- None --") };
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            _managerList.Add(new KeyValuePair<int, string>(Convert.ToInt32(reader["employee_id"]), reader["full_name"].ToString()));
                    cmbManager.DataSource = _managerList;
                    cmbManager.DisplayMember = "Value";
                    cmbManager.ValueMember = "Key";
                    cmbManager.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading managers: " + ex.Message); }
        }

        private void LoadDepartmentData()
        {
            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand("SELECT department_name, manager_id FROM Department WHERE department_id = ?", con))
                {
                    cmd.Parameters.AddWithValue("?", _deptId.Value);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtDeptName.Text = reader["department_name"].ToString();
                            if (reader["manager_id"] != DBNull.Value)
                            {
                                int mgrId = Convert.ToInt32(reader["manager_id"]);
                                foreach (var item in _managerList)
                                    if (item.Key == mgrId) { cmbManager.SelectedItem = item; break; }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading department: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDeptName.Text))
            { MessageBox.Show("Department name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int? managerId = (cmbManager.SelectedItem is KeyValuePair<int, string> kvp && kvp.Key > 0) ? (int?)kvp.Key : null;

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                {
                    con.Open();
                    if (_isEdit)
                    {
                        const string sql = "UPDATE Department SET department_name = ?, manager_id = ? WHERE department_id = ?";
                        using (var cmd = new OdbcCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("?", txtDeptName.Text);
                            cmd.Parameters.AddWithValue("?", managerId.HasValue ? (object)managerId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("?", _deptId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Department updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        using (var chk = new OdbcCommand("SELECT COUNT(1) FROM Department WHERE department_name = ?", con))
                        {
                            chk.Parameters.AddWithValue("?", txtDeptName.Text);
                            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                            { MessageBox.Show("Department name already exists!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                        }
                        const string sql = "INSERT INTO Department (department_name, manager_id) VALUES (?, ?)";
                        using (var cmd = new OdbcCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("?", txtDeptName.Text);
                            cmd.Parameters.AddWithValue("?", managerId.HasValue ? (object)managerId.Value : DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Department added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving department:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmAssets  — asset_id, asset_name, asset_type, employee_id, assigned_date, status
    // ─────────────────────────────────────────────────────────────
    public class frmAssets : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbStatusFilter;
        private Label lblTotalAssets, lblInUse, lblAvailable;

        private readonly string userDept;
        private readonly string userId;
        private readonly string userRole;

        public frmAssets(string department = "", string employeeId = "", string role = "Admin")
        {
            userDept = department;
            userId = employeeId;
            userRole = role;
            Text = string.IsNullOrEmpty(department) ? "Asset Management" : $"Assets - {department} Department";
            Size = new Size(1050, 650);
            MinimumSize = new Size(850, 500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.BgPage;
            Font = new Font("Segoe UI", 9f);
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            var topbar = UIHelper.MakeTopbar("📦 " + Text, (s, e) => Close());
            var statsBar = UIHelper.MakeStatsBar(out var statsFlow);
            lblTotalAssets = UIHelper.MakeStatCard(statsFlow, "Total Assets", Color.FromArgb(29, 78, 216), width: 155, descY: 38);
            lblInUse = UIHelper.MakeStatCard(statsFlow, "In Use", Theme.Success, width: 155, descY: 38);
            lblAvailable = UIHelper.MakeStatCard(statsFlow, "Available", Theme.Warning, width: 155, descY: 38);

            var toolbar = UIHelper.MakeToolbar();
            txtSearch = UIHelper.MakeSearchBox("Search assets...", (s, e) => LoadData());

            toolbar.Controls.Add(new Label { Text = "Status:", AutoSize = true, Location = new Point(235, 17), ForeColor = Theme.TxtSec });

            cmbStatusFilter = new ComboBox
            {
                Size = new Size(120, 28),
                Location = new Point(280, 12),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            cmbStatusFilter.Items.AddRange(new[] { "All", "In Use", "Available", "Under Maintenance", "Retired" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadData();

            var btnRefresh = UIHelper.MakeFlatBtn("⟳ Refresh", 90, 32, Theme.BgCard, Theme.TxtSec, Theme.Border);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += (s, e) => LoadData();

            if (userRole != "Employee")
            {
                var btnAdd = UIHelper.MakeFlatBtn("+ Add Asset", 110, 32, Theme.Blue, Color.White, Theme.Border, bold: true, borderSize: 0);
                var btnEdit = UIHelper.MakeFlatBtn("✎ Edit", 75, 32, Theme.BgCard, Theme.TxtPrimary, Theme.Border);
                var btnDelete = UIHelper.MakeFlatBtn("🗑 Delete", 90, 32, Theme.DangerBg, Theme.Danger, Color.FromArgb(254, 202, 202));

                btnAdd.Anchor = btnEdit.Anchor = btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnAdd.Click += (s, e) => { if (new frmAssetAdd().ShowDialog() == DialogResult.OK) LoadData(); };
                btnEdit.Click += BtnEdit_Click;
                btnDelete.Click += BtnDelete_Click;

                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });
                toolbar.Resize += (s, e) =>
                {
                    btnAdd.Location = new Point(toolbar.Width - 125, 10);
                    btnDelete.Location = new Point(toolbar.Width - 225, 10);
                    btnEdit.Location = new Point(toolbar.Width - 310, 10);
                    btnRefresh.Location = new Point(toolbar.Width - 410, 10);
                };
            }
            else
            {
                toolbar.Resize += (s, e) => btnRefresh.Location = new Point(toolbar.Width - 105, 10);
            }

            toolbar.Controls.AddRange(new Control[] { txtSearch, cmbStatusFilter, btnRefresh });

            dgv = UIHelper.MakeDgv();
            dgv.Columns.Add("asset_id", "Asset ID"); dgv.Columns["asset_id"].FillWeight = 60;
            dgv.Columns.Add("asset_name", "Asset Name");
            dgv.Columns.Add("asset_type", "Type"); dgv.Columns["asset_type"].FillWeight = 80;
            dgv.Columns.Add("employee_name", "Assigned To");
            dgv.Columns.Add("assigned_date", "Assigned Date"); dgv.Columns["assigned_date"].FillWeight = 80;
            dgv.Columns.Add("status", "Status"); dgv.Columns["status"].FillWeight = 70;
            dgv.CellFormatting += Dgv_CellFormatting;

            Controls.Add(dgv);
            Controls.Add(statsBar);
            Controls.Add(toolbar);
            Controls.Add(topbar);
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            string search = (txtSearch.Text == "Search assets...") ? "" : txtSearch.Text.Trim();
            string statusFilter = cmbStatusFilter.SelectedItem?.ToString() ?? "All";

            const string sql = @"
                SELECT 
                    a.asset_id,
                    a.asset_name,
                    a.asset_type,
                    e.full_name AS employee_name,
                    CONVERT(VARCHAR, a.assigned_date, 103) AS assigned_date_display,
                    a.status
                FROM Asset a
                LEFT JOIN Employee e ON a.employee_id = e.employee_id
                WHERE 
                    (? = 'All' OR a.status = ?)
                    AND (? = '' 
                        OR a.asset_name LIKE '%' + ? + '%'
                        OR a.asset_type LIKE '%' + ? + '%'
                        OR e.full_name  LIKE '%' + ? + '%')
                ORDER BY a.asset_id";

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand(sql, con))
                {
                    string pattern = "%" + search + "%";
                    cmd.Parameters.AddWithValue("?", statusFilter);
                    cmd.Parameters.AddWithValue("?", statusFilter);
                    cmd.Parameters.AddWithValue("?", search);
                    cmd.Parameters.AddWithValue("?", pattern);
                    cmd.Parameters.AddWithValue("?", pattern);
                    cmd.Parameters.AddWithValue("?", pattern);
                    con.Open();

                    int total = 0, inUse = 0, available = 0;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            total++;
                            string status = reader["status"].ToString();
                            if (status == "In Use") inUse++;
                            else if (status == "Available") available++;
                            dgv.Rows.Add(
                                reader["asset_id"].ToString(),
                                reader["asset_name"].ToString(),
                                reader["asset_type"]?.ToString() ?? "—",
                                reader["employee_name"]?.ToString() ?? "—",
                                reader["assigned_date_display"]?.ToString() ?? "—",
                                status);
                        }
                    }
                    lblTotalAssets.Text = total.ToString();
                    lblInUse.Text = inUse.ToString();
                    lblAvailable.Text = available.ToString();
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
            { MessageBox.Show("Please select an asset to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!int.TryParse(dgv.CurrentRow.Cells["asset_id"].Value?.ToString(), out int assetId)) return;
            if (new frmAssetAdd(assetId).ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            { MessageBox.Show("Please select an asset to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!int.TryParse(dgv.CurrentRow.Cells["asset_id"].Value?.ToString(), out int assetId)) return;
            string assetName = dgv.CurrentRow.Cells["asset_name"].Value?.ToString() ?? "";

            if (MessageBox.Show($"Are you sure you want to delete '{assetName}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand("DELETE FROM Asset WHERE asset_id = ?", con))
                {
                    cmd.Parameters.AddWithValue("?", assetId);
                    con.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    { MessageBox.Show("Asset deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadData(); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting asset:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].Cells["status"].Value == null) return;
            if (dgv.Rows[e.RowIndex].Selected) return;

            switch (dgv.Rows[e.RowIndex].Cells["status"].Value.ToString())
            {
                case "In Use": dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Theme.SuccessBg; break;
                case "Available": dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Theme.WarningBg; break;
                case "Under Maintenance": dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Theme.DangerBg; break;
                default: dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White; break;
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  frmAssetAdd  — Add / Edit Asset
    // ─────────────────────────────────────────────────────────────
    public class frmAssetAdd : Form
    {
        private readonly int? _assetId;
        private readonly bool _isEdit;
        private TextBox txtAssetName;
        private ComboBox cmbAssetType, cmbStatus, cmbEmployee;
        private DateTimePicker dtpAssignedDate;
        private List<KeyValuePair<int, string>> _employeeList;

        public frmAssetAdd(int? assetId = null)
        {
            _assetId = assetId;
            _isEdit = assetId.HasValue;
            Text = _isEdit ? "Edit Asset" : "Add Asset";
            Size = new Size(450, 480);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.BgPage;
            Font = new Font("Segoe UI", 9f);
            BuildUI();
            LoadEmployees();
            if (_isEdit) LoadAssetData();
        }

        private void BuildUI()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 16, 20, 10), BackColor = Theme.BgPage };
            int y = 0;

            txtAssetName = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f) };
            UIHelper.AddFormRow(pnl, "Asset Name *", txtAssetName, ref y);

            cmbAssetType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbAssetType.Items.AddRange(new[] { "Electronics", "Furniture", "Vehicle", "Equipment", "Other" });
            cmbAssetType.SelectedIndex = 0;
            UIHelper.AddFormRow(pnl, "Asset Type *", cmbAssetType, ref y);

            cmbEmployee = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f), DisplayMember = "Value", ValueMember = "Key" };
            UIHelper.AddFormRow(pnl, "Assigned To", cmbEmployee, ref y);

            dtpAssignedDate = new DateTimePicker { Font = new Font("Segoe UI", 10f), Value = DateTime.Today };
            UIHelper.AddFormRow(pnl, "Assigned Date", dtpAssignedDate, ref y);

            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbStatus.Items.AddRange(new[] { "In Use", "Available", "Under Maintenance", "Retired" });
            cmbStatus.SelectedIndex = 0;
            UIHelper.AddFormRow(pnl, "Status *", cmbStatus, ref y);

            var btnSave = new Button
            {
                Text = _isEdit ? "Save Changes" : "Save Asset",
                Size = new Size(160, 38),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Blue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnCancel = UIHelper.MakeFlatBtn("Cancel", 90, 38, Color.White, Theme.TxtSec, Theme.Border);
            btnCancel.Location = new Point(175, y);
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
                    _employeeList = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(0, "-- Unassigned --") };
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            _employeeList.Add(new KeyValuePair<int, string>(Convert.ToInt32(reader["employee_id"]), reader["full_name"].ToString()));
                    cmbEmployee.DataSource = _employeeList;
                    cmbEmployee.DisplayMember = "Value";
                    cmbEmployee.ValueMember = "Key";
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading employees: " + ex.Message); }
        }

        private void LoadAssetData()
        {
            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                using (var cmd = new OdbcCommand("SELECT asset_name, asset_type, employee_id, assigned_date, status FROM Asset WHERE asset_id = ?", con))
                {
                    cmd.Parameters.AddWithValue("?", _assetId.Value);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtAssetName.Text = reader["asset_name"].ToString();
                            cmbAssetType.SelectedItem = reader["asset_type"]?.ToString() ?? "Other";
                            if (reader["employee_id"] != DBNull.Value)
                            {
                                int empId = Convert.ToInt32(reader["employee_id"]);
                                foreach (var item in _employeeList)
                                    if (item.Key == empId) { cmbEmployee.SelectedItem = item; break; }
                            }
                            if (reader["assigned_date"] != DBNull.Value)
                                dtpAssignedDate.Value = Convert.ToDateTime(reader["assigned_date"]);
                            cmbStatus.SelectedItem = reader["status"]?.ToString() ?? "Available";
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading asset: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAssetName.Text))
            { MessageBox.Show("Asset name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbAssetType.SelectedItem == null)
            { MessageBox.Show("Please select asset type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int? employeeId = (cmbEmployee.SelectedItem is KeyValuePair<int, string> kvp && kvp.Key > 0) ? (int?)kvp.Key : null;
            string status = cmbStatus.SelectedItem?.ToString() ?? "Available";

            try
            {
                using (var con = new OdbcConnection(Global.ConnStr))
                {
                    con.Open();
                    if (_isEdit)
                    {
                        const string sql = "UPDATE Asset SET asset_name=?, asset_type=?, employee_id=?, assigned_date=?, status=? WHERE asset_id=?";
                        using (var cmd = new OdbcCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("?", txtAssetName.Text);
                            cmd.Parameters.AddWithValue("?", cmbAssetType.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("?", employeeId.HasValue ? (object)employeeId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("?", dtpAssignedDate.Value.Date);
                            cmd.Parameters.AddWithValue("?", status);
                            cmd.Parameters.AddWithValue("?", _assetId.Value);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Asset updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        const string sql = "INSERT INTO Asset (asset_name, asset_type, employee_id, assigned_date, status) VALUES (?,?,?,?,?)";
                        using (var cmd = new OdbcCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("?", txtAssetName.Text);
                            cmd.Parameters.AddWithValue("?", cmbAssetType.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("?", employeeId.HasValue ? (object)employeeId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("?", dtpAssignedDate.Value.Date);
                            cmd.Parameters.AddWithValue("?", status);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Asset added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving asset:\n" + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

	// ─────────────────────────────────────────────────────────────
	//  frmReports  — summary of all ERD entities with role filtering
	// ─────────────────────────────────────────────────────────────
	public class frmReports : Form
	{
		// ── Colors ──────────────────────────────────────────────────────────
		private readonly Color Blue = Color.FromArgb(26, 86, 219);
		private readonly Color BgPage = Color.FromArgb(243, 244, 246);
		private readonly Color BgCard = Color.White;
		private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
		private readonly Color TxtSec = Color.FromArgb(107, 114, 128);
		private readonly Color Border = Color.FromArgb(220, 222, 226);

		private Panel contentPanel;
		private string userDept;
		private string userId;
		private string userRole;

		// ── Report definitions: { key, title, description, fields } ─────────
		private string[][] GetReports()
		{
			if (string.IsNullOrEmpty(userDept) && string.IsNullOrEmpty(userId))
			{
				return new[]
				{
					new[] { "Employee",    "Employee Report",    "Full employee records",          "employee_id, name, department"         },
					new[] { "Attendance",  "Attendance Report",  "Monthly attendance summary",     "check_in, check_out, work_hours"       },
					new[] { "Payroll",     "Payroll Report",     "Salary breakdown",               "base_salary, allowances, net"          },
					new[] { "Leave",       "Leave Report",       "Leave requests",                 "leave_type, dates, status"             },
					new[] { "Contract",    "Contract Report",    "Contract status",                "contract_id, start_date, end_date"     },
					new[] { "Asset",       "Asset Report",       "Asset inventory",                "asset_name, type, assigned_to"         },
					new[] { "Department",  "Department Report",  "Dept headcount",                 "department_name, manager, count"       },
					new[] { "Performance", "Performance Report", "Evaluation scores",              "technical, attendance, safety, rating" },
				};
			}
			else if (!string.IsNullOrEmpty(userDept))
			{
				return new[]
				{
					new[] { "Employee",   "Employee Report",   $"Department employees",  "employee_id, name, position"    },
					new[] { "Attendance", "Attendance Report", "Team attendance",         "check_in, check_out, work_hours"},
					new[] { "Leave",      "Leave Report",      "Team leave requests",     "leave_type, dates, status"     },
					new[] { "Asset",      "Asset Report",      "Department assets",       "asset_name, type, assigned_to" },
				};
			}
			else
			{
				return new[]
				{
					new[] { "MyAttendance",  "My Attendance",  "My attendance record",   "date, check_in, check_out, status"},
					new[] { "MyLeave",       "My Leave",       "My leave requests",      "leave_type, from_date, to_date"  },
					new[] { "MyPerformance", "My Performance", "My evaluation scores",   "technical, attendance, safety"   },
					new[] { "MyContract",    "My Contract",    "My employment contract", "contract_id, start_date, end_date"},
				};
			}
		}

		// ════════════════════════════════════════════════════════════════════
		//  Constructor
		// ════════════════════════════════════════════════════════════════════
		public frmReports(
        string department = "", string employeeId = "", string role = "Admin")
		{
			userDept = department;
			userId = employeeId;
			userRole = role;

			Text = string.IsNullOrEmpty(department) ? "Reports Dashboard" : $"Reports - {department} Department";
			Size = new Size(1200, 750);
			MinimumSize = new Size(800, 500);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = BgPage;
			Font = new Font("Segoe UI", 9f);

			BuildUI();
		}

		// ════════════════════════════════════════════════════════════════════
		//  UI Builder — responsive FlowLayoutPanel
		// ════════════════════════════════════════════════════════════════════
		private void BuildUI()
		{
			// ── Top bar ──────────────────────────────────────────────────────
			var topbar = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = BgCard };
			topbar.Paint += (s, e) =>
				e.Graphics.DrawLine(new Pen(Border), 0, topbar.Height - 1, topbar.Width, topbar.Height - 1);

			string titleText = !string.IsNullOrEmpty(userDept) ? $"📊 Reports - {userDept} Department"
							 : !string.IsNullOrEmpty(userId) ? "📊 My Reports"
							 : "📊 Reports Dashboard";

			topbar.Controls.Add(new Label
			{
				Text = titleText,
				Font = new Font("Segoe UI", 14f, FontStyle.Bold),
				ForeColor = TxtPrimary,
				AutoSize = true,
				Location = new Point(20, 14)
			});

			var btnBack = new Button
			{
				Text = "← Back",
				Size = new Size(80, 32),
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

			// ── Section label ──────────────────────────────────────────────────
			var lblSection = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = BgPage };
			lblSection.Controls.Add(new Label
			{
				Text = "📋 GENERATE REPORTS",
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				ForeColor = TxtSec,
				AutoSize = true,
				Location = new Point(20, 10)
			});

			// ── Content scroll ─────────────────────────────────────────────────
			contentPanel = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = BgPage,
				AutoScroll = true,
				Padding = new Padding(16, 8, 16, 16)
			};

			// ✅ FlowLayoutPanel — يرتب الكاردز أوتوماتيك ويعمل wrap لو الشاشة صغيرة
			var flow = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				WrapContents = true,
				BackColor = BgPage,
				Padding = new Padding(4)
			};

			foreach (var rpt in GetReports())
			{
				string key = rpt[0];
				string name = rpt[1];
				string desc = rpt[2];
				string flds = rpt[3];

				flow.Controls.Add(MakeReportCard(key, name, desc, flds));
			}

			contentPanel.Controls.Add(flow);

			Controls.Add(contentPanel);
			Controls.Add(lblSection);
			Controls.Add(topbar);
		}

		// ════════════════════════════════════════════════════════════════════
		//  Report Card  ✅ بيتمدد مع الحجم
		// ════════════════════════════════════════════════════════════════════
		private Panel MakeReportCard(string key, string name, string desc, string fields)
		{
			var card = new Panel
			{
				Size = new Size(370, 120),
				BackColor = BgCard,
				Margin = new Padding(8),
				Cursor = Cursors.Hand
			};
			card.Paint += (s, e) =>
				e.Graphics.DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);

			// Blue left pill
			card.Controls.Add(new Panel { Size = new Size(6, 120), Location = new Point(0, 0), BackColor = Blue });

			// Title
			card.Controls.Add(new Label
			{
				Text = name,
				Font = new Font("Segoe UI", 11f, FontStyle.Bold),
				ForeColor = Blue,
				AutoSize = true,
				Location = new Point(18, 12)
			});

			// Description
			card.Controls.Add(new Label
			{
				Text = desc,
				Font = new Font("Segoe UI", 8.5f),
				ForeColor = TxtSec,
				Size = new Size(320, 20),
				Location = new Point(18, 40)
			});

			// Fields
			card.Controls.Add(new Label
			{
				Text = fields,
				Font = new Font("Segoe UI", 8f),
				ForeColor = Color.FromArgb(156, 163, 175),
				Size = new Size(320, 20),
				Location = new Point(18, 62)
			});

			// ✅ Export button — فعلي بيفتح SaveFileDialog وينزل CSV
			var btnExport = new Button
			{
				Text = "↓ Export",
				Size = new Size(85, 30),
				Location = new Point(card.Width - 105, card.Height - 38),
				FlatStyle = FlatStyle.Flat,
				BackColor = Blue,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				Cursor = Cursors.Hand,
				Anchor = AnchorStyles.Bottom | AnchorStyles.Right
			};
			btnExport.FlatAppearance.BorderSize = 0;
			btnExport.Click += (s, e) => ExportReport(key, name);

			card.Controls.Add(btnExport);

			// كمان ضغطة على الكارد نفسها بتشغل الـ export
			card.Click += (s, e) => ExportReport(key, name);

			return card;
		}

		// ════════════════════════════════════════════════════════════════════
		//  Export Logic — SaveFileDialog + CSV from DB
		// ════════════════════════════════════════════════════════════════════
		private void ExportReport(string key, string reportName)
		{
			using (var dlg = new SaveFileDialog())
			{
				string safeDate = DateTime.Now.ToString("yyyyMMdd_HHmm");
				dlg.Title = $"Export {reportName}";
				dlg.FileName = $"{key}_Report_{safeDate}.csv";
				dlg.Filter = "CSV File (*.csv)|*.csv";
				dlg.DefaultExt = "csv";
				dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

				if (dlg.ShowDialog() != DialogResult.OK) return;

				try
				{
					string csv = BuildCsv(key);
					File.WriteAllText(dlg.FileName, csv, Encoding.UTF8);

					var result = MessageBox.Show(
						$"✅ Report exported successfully!\n\n📁 {dlg.FileName}\n\nDo you want to open it now?",
						"Export Done", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

					if (result == DialogResult.Yes)
						System.Diagnostics.Process.Start(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error exporting report:\n" + ex.Message,
									"Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		// ════════════════════════════════════════════════════════════════════
		//  Build CSV string from DB query
		// ════════════════════════════════════════════════════════════════════
		private string BuildCsv(string key)
		{
			string sql = GetSqlForReport(key);
			if (string.IsNullOrEmpty(sql))
				throw new Exception("No query defined for this report.");

			var sb = new StringBuilder();

			using (var con = new OdbcConnection(Global.ConnStr))
			using (var cmd = new OdbcCommand(sql, con))
			{
				// تمرير باراميترز الـ department أو الـ employee لو موجودين
				if (sql.Contains("/*DEPT*/"))
				{
					cmd.CommandText = cmd.CommandText.Replace("/*DEPT*/", "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
					cmd.Parameters.AddWithValue("?", userDept ?? "");
				}
				if (sql.Contains("/*EMP*/"))
				{
					cmd.CommandText = cmd.CommandText.Replace("/*EMP*/", "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
					cmd.Parameters.AddWithValue("?", userId ?? "");
				}

				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					// Header row
					var headers = new string[reader.FieldCount];
					for (int i = 0; i < reader.FieldCount; i++)
						headers[i] = CsvEscape(reader.GetName(i));
					sb.AppendLine(string.Join(",", headers));

					// Data rows
					while (reader.Read())
					{
						var cols = new string[reader.FieldCount];
						for (int i = 0; i < reader.FieldCount; i++)
							cols[i] = CsvEscape(reader[i]?.ToString() ?? "");
						sb.AppendLine(string.Join(",", cols));
					}
				}
			}

			return sb.ToString();
		}

		// ════════════════════════════════════════════════════════════════════
		//  SQL queries for each report
		// ════════════════════════════════════════════════════════════════════
		private string GetSqlForReport(string key)
		{
			switch (key)
			{
				// ── Admin reports ──────────────────────────────────────────────
				case "Employee":
					return @"
                        SELECT
                            e.employee_id,
                            e.employee_code,
                            e.full_name,
                            e.national_id,
                            e.insurance_number,
                            e.blood_type,
                            e.hire_date,
                            e.job_title,
                            e.grade,
                            d.department_name,
                            c.contract_type,
                            c.status AS contract_status
                        FROM Employee e
                        LEFT JOIN Department d ON e.department_id = d.department_id
                        LEFT JOIN Contract   c ON e.contract_id   = c.contract_id
                        ORDER BY e.full_name";

				case "Attendance":
					return @"
                        SELECT
                            a.attendance_id,
                            e.employee_id,
                            e.full_name,
                            d.department_name,
                            CONVERT(VARCHAR, a.date, 103)                        AS date,
                            ISNULL(CONVERT(VARCHAR(5), a.check_in_time,  108), '') AS check_in,
                            ISNULL(CONVERT(VARCHAR(5), a.check_out_time, 108), '') AS check_out,
                            a.shift_type,
                            ISNULL(a.work_hours,     0) AS work_hours,
                            ISNULL(a.overtime_hours, 0) AS overtime_hours,
                            CASE
                                WHEN a.check_in_time IS NULL THEN 'Absent'
                                WHEN CAST(a.check_in_time AS TIME) > '09:00:00' THEN 'Late'
                                ELSE 'Present'
                            END AS status
                        FROM Attendance a
                        LEFT JOIN Employee   e ON a.employee_id   = e.employee_id
                        LEFT JOIN Department d ON e.department_id = d.department_id
                        ORDER BY a.date DESC, e.full_name";

				case "Payroll":
					return @"
                        SELECT
                            p.payroll_id,
                            e.employee_id,
                            e.full_name,
                            d.department_name,
                            e.job_title,
                            p.base_salary,
                            p.social_allowance,
                            p.risk_allowance,
                            p.transport_allowance,
                            p.production_bonus,
                            p.deductions,
                            p.tax,
                            p.insurance,
                            p.net_salary,
                            CONVERT(VARCHAR, p.pay_date, 103) AS pay_date
                        FROM Payroll p
                        LEFT JOIN Employee   e ON p.employee_id   = e.employee_id
                        LEFT JOIN Department d ON e.department_id = d.department_id
                        ORDER BY p.pay_date DESC, e.full_name";

				case "Leave":
					return @"
                        SELECT
                            l.leave_id,
                            e.employee_id,
                            e.full_name,
                            d.department_name,
                            l.leave_type,
                            CONVERT(VARCHAR, l.start_date, 103) AS start_date,
                            CONVERT(VARCHAR, l.end_date,   103) AS end_date,
                            DATEDIFF(day, l.start_date, l.end_date) + 1 AS days,
                            l.approval_status,
                            apr.full_name AS approved_by
                        FROM [Leave] l
                        LEFT JOIN Employee   e   ON l.employee_id = e.employee_id
                        LEFT JOIN Employee   apr ON l.approved_by = apr.employee_id
                        LEFT JOIN Department d   ON e.department_id = d.department_id
                        ORDER BY l.start_date DESC";

				case "Contract":
					return @"
                        SELECT
                            c.contract_id,
                            c.contract_type,
                            CONVERT(VARCHAR, c.start_date, 103) AS start_date,
                            CONVERT(VARCHAR, c.end_date,   103) AS end_date,
                            c.status,
                            d.department_name
                        FROM Contract c
                        LEFT JOIN Department d ON c.department_id = d.department_id
                        ORDER BY c.start_date DESC";

				case "Asset":
					return @"
                        SELECT
                            a.asset_id,
                            a.asset_name,
                            a.asset_type,
                            e.full_name        AS assigned_to,
                            d.department_name,
                            CONVERT(VARCHAR, a.assigned_date, 103) AS assigned_date,
                            a.status
                        FROM Asset a
                        LEFT JOIN Employee   e ON a.employee_id   = e.employee_id
                        LEFT JOIN Department d ON e.department_id = d.department_id
                        ORDER BY a.asset_name";

				case "Department":
					return @"
                        SELECT
                            d.department_id,
                            d.department_name,
                            m.full_name AS manager_name,
                            COUNT(e.employee_id) AS employee_count
                        FROM Department d
                        LEFT JOIN Employee m   ON d.manager_id    = m.employee_id
                        LEFT JOIN Employee e   ON d.department_id = e.department_id
                        GROUP BY d.department_id, d.department_name, m.full_name
                        ORDER BY d.department_name";

				case "Performance":
					return @"
                        SELECT
                            pe.evaluation_id,
                            e.employee_id,
                            e.full_name,
                            d.department_name,
                            pe.evaluation_year,
                            pe.technical_skill_score,
                            pe.attendance_score,
                            pe.safety_score,
                            pe.final_rating,
                            pe.notes
                        FROM Performance_Evaluation pe
                        LEFT JOIN Employee   e ON pe.employee_id  = e.employee_id
                        LEFT JOIN Department d ON e.department_id = d.department_id
                        ORDER BY pe.evaluation_year DESC, e.full_name";

				// ── Manager reports (filtered by department) ───────────────────
				// Note: باستخدام ISNULL+param لعمل filter اختياري
				default:
					return null;
			}
		}

		// ════════════════════════════════════════════════════════════════════
		//  CSV helper — wraps values containing commas or quotes
		// ════════════════════════════════════════════════════════════════════
		private static string CsvEscape(string value)
		{
			if (value == null) return "";
			if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
				return "\"" + value.Replace("\"", "\"\"") + "\"";
			return value;
		}
	}
}
