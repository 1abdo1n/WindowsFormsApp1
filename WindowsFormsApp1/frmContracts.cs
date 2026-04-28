using System;
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
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbStatus;

        private readonly string[][] allRows = {
            new[]{"C001","Ahmed Hassan","Full-time","01/01/2023","31/12/2025","Active"},
            new[]{"C002","Sara Mohamed","Full-time","15/03/2022","14/03/2025","Expired"},
            new[]{"C003","Omar Ali",    "Part-time","01/06/2024","31/05/2026","Active"},
            new[]{"C004","Nour Khaled", "Full-time","01/09/2023","31/08/2026","Active"},
            new[]{"C005","Youssef Taha","Contract", "01/01/2025","30/06/2025","Pending"},
        };

        public frmContracts()
        {
            Text = "Contracts"; Size = new Size(1000, 620); MinimumSize = new Size(750, 500);
            StartPosition = FormStartPosition.CenterScreen; BackColor = BgPage; Font = new Font("Segoe UI", 9f);

            Panel topbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, topbar.Width, 51);
            topbar.Controls.Add(new Label { Text = "Contracts", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TxtPrimary, AutoSize = true, Location = new Point(20, 14) });
            var btnBack = new Button { Text = "← Back", Size = new Size(80, 30), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtSecondary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnBack.FlatAppearance.BorderColor = Border; btnBack.Click += (s, e) => Close();
            topbar.Controls.Add(btnBack); topbar.Resize += (s, e) => btnBack.Location = new Point(topbar.Width - 100, 11);

            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BgCard };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, toolbar.Width, 51);

            txtSearch = new TextBox { Size = new Size(170, 28), Location = new Point(20, 12), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, ForeColor = TxtSecondary, Text = "Search..." };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search..."; txtSearch.ForeColor = TxtSecondary; } };
            txtSearch.TextChanged += (s, e) => ApplyFilter();

            var lblStatus = new Label { Text = "Status:", AutoSize = true, Location = new Point(202, 17), ForeColor = TxtSecondary };
            cmbStatus = new ComboBox { Size = new Size(115, 28), Location = new Point(245, 12), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbStatus.Items.AddRange(new[] { "All", "Active", "Expired", "Pending" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += (s, e) => ApplyFilter();

            var btnAdd = new Button { Text = "+ New Contract", Size = new Size(130, 32), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnAdd.FlatAppearance.BorderSize = 0; btnAdd.Click += (s, e) => new frmContractAdd().ShowDialog();

            var btnEdit = new Button { Text = "✎ Edit", Size = new Size(75, 32), FlatStyle = FlatStyle.Flat, BackColor = BgCard, ForeColor = TxtPrimary, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnEdit.FlatAppearance.BorderColor = Border;

            var btnDel = new Button { Text = "🗑 Delete", Size = new Size(90, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(254, 242, 242), ForeColor = Color.FromArgb(185, 28, 28), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnDel.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnDel.Click += (s, e) => { if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Select a row first.", "Delete"); return; } if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) dgv.Rows.Remove(dgv.SelectedRows[0]); };

            toolbar.Controls.Add(txtSearch); toolbar.Controls.Add(lblStatus); toolbar.Controls.Add(cmbStatus);
            toolbar.Controls.Add(btnAdd); toolbar.Controls.Add(btnEdit); toolbar.Controls.Add(btnDel);
            toolbar.Resize += (s, e) => { btnAdd.Location = new Point(toolbar.Width - 145, 10); btnDel.Location = new Point(toolbar.Width - 245, 10); btnEdit.Location = new Point(toolbar.Width - 330, 10); };

            dgv = BuildGrid();
            dgv.Columns.Add("con_id", "Contract ID"); dgv.Columns["con_id"].FillWeight = 70;
            dgv.Columns.Add("emp_name", "Employee");
            dgv.Columns.Add("con_type", "Type"); dgv.Columns["con_type"].FillWeight = 75;
            dgv.Columns.Add("start", "Start Date"); dgv.Columns["start"].FillWeight = 80;
            dgv.Columns.Add("end", "End Date"); dgv.Columns["end"].FillWeight = 80;
            dgv.Columns.Add("status", "Status"); dgv.Columns["status"].FillWeight = 65;
            foreach (var r in allRows) dgv.Rows.Add(r);

            Controls.Add(dgv); Controls.Add(toolbar); Controls.Add(topbar);
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text == "Search...") ? "" : txtSearch.Text.ToLower().Trim();
            string status = cmbStatus.SelectedItem?.ToString() ?? "All";
            dgv.Rows.Clear();
            foreach (var r in allRows)
            {
                bool matchSearch = string.IsNullOrEmpty(search) || string.Join(" ", r).ToLower().Contains(search);
                bool matchStatus = status == "All" || r[5] == status;
                if (matchSearch && matchStatus) dgv.Rows.Add(r);
            }
        }

        private DataGridView BuildGrid()
        {
            var g = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = BgCard, BorderStyle = BorderStyle.None, ColumnHeadersHeight = 36, RowTemplate = { Height = 34 }, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Border };
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251); g.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary; g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9f); g.DefaultCellStyle.ForeColor = TxtPrimary; g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); g.DefaultCellStyle.SelectionForeColor = TxtPrimary;
            return g;
        }
    }

    public class frmContractAdd : Form
    {
        public frmContractAdd()
        {
            var Blue = Color.FromArgb(26, 86, 219); var BgPage = Color.FromArgb(243, 244, 246); var Border = Color.FromArgb(220, 222, 226); var TxtSec = Color.FromArgb(107, 114, 128);
            Text = "New Contract"; Size = new Size(420, 380); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; StartPosition = FormStartPosition.CenterParent; BackColor = BgPage; Font = new Font("Segoe UI", 9f);
            var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 10), BackColor = BgPage }; int y = 0;
            pnl.Controls.Add(new Label { Text = "Employee ID *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new TextBox { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle }); y += 56;
            var cmbType = new ComboBox { Size = new Size(360, 28), Location = new Point(0, y + 18), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbType.Items.AddRange(new[] { "Full-time", "Part-time", "Contract", "Internship" }); cmbType.SelectedIndex = 0;
            pnl.Controls.Add(new Label { Text = "Contract Type *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) }); pnl.Controls.Add(cmbType); y += 56;
            pnl.Controls.Add(new Label { Text = "Start Date *", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            pnl.Controls.Add(new Label { Text = "End Date", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 81), AutoSize = true, Location = new Point(0, y) });
            pnl.Controls.Add(new DateTimePicker { Size = new Size(360, 28), Location = new Point(0, y + 18), Font = new Font("Segoe UI", 9f) }); y += 56;
            var btnS = new Button { Text = "Save", Size = new Size(160, 38), Location = new Point(0, y), FlatStyle = FlatStyle.Flat, BackColor = Blue, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnS.FlatAppearance.BorderSize = 0; btnS.Click += (s, e) => { MessageBox.Show("Saved! Connect to DB.", "Saved"); Close(); };
            var btnC = new Button { Text = "Cancel", Size = new Size(90, 38), Location = new Point(170, y), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = TxtSec, Cursor = Cursors.Hand }; btnC.FlatAppearance.BorderColor = Border; btnC.Click += (s, e) => Close();
            pnl.Controls.Add(btnS); pnl.Controls.Add(btnC); Controls.Add(pnl);
        }
    }
}