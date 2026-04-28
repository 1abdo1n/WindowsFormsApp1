using System;
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
        private readonly Color TxtSecondary = Color.FromArgb(107, 114, 128);
        private readonly Color Border = Color.FromArgb(220, 222, 226);

        private string userDept;

        public frmEmployees(string department = "")
        {
            userDept = department;
            this.Text = string.IsNullOrEmpty(department) ? "Employees" : $"Employees - {department} Department";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgPage;
            this.Font = new Font("Segoe UI", 9f);

            Panel topbar = new Panel();
            topbar.Size = new Size(1100, 52);
            topbar.Location = new Point(0, 0);
            topbar.BackColor = BgCard;
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, 1100, 51);

            Label lblTitle = new Label();
            lblTitle.Text = this.Text;
            lblTitle.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTitle.ForeColor = TxtPrimary;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 14);
            topbar.Controls.Add(lblTitle);

            Button btnBack = new Button();
            btnBack.Text = "← Back";
            btnBack.Size = new Size(80, 30);
            btnBack.Location = new Point(990, 11);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderColor = Border;
            btnBack.BackColor = BgCard;
            btnBack.ForeColor = TxtSecondary;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();
            topbar.Controls.Add(btnBack);

            Panel toolbar = new Panel();
            toolbar.Size = new Size(1100, 52);
            toolbar.Location = new Point(0, 52);
            toolbar.BackColor = BgCard;
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Border), 0, 51, 1100, 51);

            TextBox txtSearch = new TextBox();
            txtSearch.Size = new Size(220, 28);
            txtSearch.Location = new Point(20, 12);
            txtSearch.Font = new Font("Segoe UI", 9.5f);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Text = "Search employees...";
            txtSearch.ForeColor = TxtSecondary;
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search employees...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search employees..."; txtSearch.ForeColor = TxtSecondary; } };
            toolbar.Controls.Add(txtSearch);

            Button btnAdd = new Button();
            btnAdd.Text = "+ Add Employee";
            btnAdd.Size = new Size(130, 32);
            btnAdd.Location = new Point(940, 10);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.BackColor = Blue;
            btnAdd.ForeColor = Color.White;
            btnAdd.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => MessageBox.Show("Add Employee form goes here.", "Add Employee");
            toolbar.Controls.Add(btnAdd);

            Button btnEdit = new Button();
            btnEdit.Text = "✎ Edit";
            btnEdit.Size = new Size(75, 32);
            btnEdit.Location = new Point(770, 10);
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderColor = Border;
            btnEdit.BackColor = BgCard;
            btnEdit.ForeColor = TxtPrimary;
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += (s, e) => MessageBox.Show("Select a row to edit.", "Edit");
            toolbar.Controls.Add(btnEdit);

            Button btnDelete = new Button();
            btnDelete.Text = "🗑 Delete";
            btnDelete.Size = new Size(85, 32);
            btnDelete.Location = new Point(850, 10);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnDelete.BackColor = Color.FromArgb(254, 242, 242);
            btnDelete.ForeColor = Color.FromArgb(185, 28, 28);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += (s, e) => MessageBox.Show("Select a row to delete.", "Delete");
            toolbar.Controls.Add(btnDelete);

            DataGridView dgv = new DataGridView();
            dgv.Size = new Size(1060, 490);
            dgv.Location = new Point(20, 116);
            dgv.BackgroundColor = BgCard;
            dgv.BorderStyle = BorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TxtSecondary;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 38;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgv.DefaultCellStyle.ForeColor = TxtPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = TxtPrimary;
            dgv.GridColor = Border;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns.Add("ID", "ID");
            dgv.Columns.Add("Code", "Code");
            dgv.Columns.Add("Name", "Full Name");
            dgv.Columns.Add("NationalID", "National ID");
            dgv.Columns.Add("Insurance", "Insurance No.");
            dgv.Columns.Add("Blood", "Blood");
            dgv.Columns.Add("HireDate", "Hire Date");
            dgv.Columns.Add("JobTitle", "Job Title");
            dgv.Columns.Add("Grade", "Grade");
            dgv.Columns.Add("Department", "Department");
            dgv.Columns.Add("Contract", "Contract");

            // All employees data
            var allEmployees = new[]
            {
                new object[] { "001", "EMP-001", "Ahmed Hassan", "30001234567890", "INS-001", "A+", "01/01/2020", "Senior Developer", "A", "Engineering", "C001" },
                new object[] { "002", "EMP-002", "Sara Mohamed", "30002345678901", "INS-002", "O+", "15/03/2021", "HR Manager", "B", "Human Resources", "C002" },
                new object[] { "003", "EMP-003", "Omar Ali", "30003456789012", "INS-003", "B+", "01/06/2022", "Accountant", "B", "Finance", "C003" },
                new object[] { "004", "EMP-004", "Nour Khaled", "30004567890123", "INS-004", "AB+", "10/09/2022", "Junior Developer", "C", "Engineering", "C004" },
                new object[] { "005", "EMP-005", "Youssef Taha", "30005678901234", "INS-005", "O-", "01/01/2023", "Sales Rep", "C", "Sales", "C005" },
            };

            // Filter by department if Manager
            foreach (var emp in allEmployees)
            {
                string empDept = emp[9].ToString();
                if (string.IsNullOrEmpty(userDept) || empDept == userDept)
                {
                    dgv.Rows.Add(emp);
                }
            }

            this.Controls.Add(topbar);
            this.Controls.Add(toolbar);
            this.Controls.Add(dgv);
        }
    }
}