using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class frmMain : Form
    {
        private Button btnEmployees;
        private Button btnPayroll;
        private Button btnAttendance;

        private string userRole;

        public frmMain(string role)
        {
            userRole = role;
            InitializeComponents();
            ConfigureMenu();
        }

        private void InitializeComponents()
        {
            this.Text = "HRMS - Main";
            btnEmployees = new Button() { Text = "Employees", Left = 20, Top = 20, Width = 120 };
            btnPayroll = new Button() { Text = "Payroll", Left = 20, Top = 60, Width = 120 };
            btnAttendance = new Button() { Text = "Attendance", Left = 20, Top = 100, Width = 120 };

            btnEmployees.Click += BtnEmployees_Click;
            btnPayroll.Click += BtnPayroll_Click;
            btnAttendance.Click += BtnAttendance_Click;

            this.Controls.Add(btnEmployees);
            this.Controls.Add(btnPayroll);
            this.Controls.Add(btnAttendance);

            this.ClientSize = new System.Drawing.Size(800, 600);
        }

        private void ConfigureMenu()
        {
            if (userRole != "HR")
            {
                btnEmployees.Enabled = false;
                btnPayroll.Enabled = false;
            }
            if (userRole == "Employee")
            {
                btnAttendance.Enabled = false;
            }
        }

        private void BtnEmployees_Click(object sender, EventArgs e)
        {
            frmEmployees f = new frmEmployees();
            f.ShowDialog();
        }

        private void BtnPayroll_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Payroll form not implemented in this workspace yet.");
        }

        private void BtnAttendance_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Attendance form not implemented in this workspace yet.");
        }
    }
}