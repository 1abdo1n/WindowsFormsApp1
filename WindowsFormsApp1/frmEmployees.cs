using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class frmEmployees : Form
    {
        private DataGridView dgvEmployees;
        private Button btnSave;
        private TextBox txtFullName, txtNationalID, txtJobTitle, txtGrade, txtBloodType;
        private DateTimePicker dtHireDate;

        public frmEmployees()
        {
            InitializeComponents();
            LoadEmployees();
            LoadDepartments();
        }

        private void InitializeComponents()
        {
            this.Text = "Employees";
            this.ClientSize = new Size(900, 600);

            dgvEmployees = new DataGridView() { Left = 10, Top = 150, Width = 860, Height = 400, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            Label lblName = new Label() { Text = "Full Name", Left = 10, Top = 10, AutoSize = true };
            txtFullName = new TextBox() { Left = 100, Top = 8, Width = 200 };

            Label lblNat = new Label() { Text = "National ID", Left = 320, Top = 10, AutoSize = true };
            txtNationalID = new TextBox() { Left = 410, Top = 8, Width = 150 };

            Label lblJob = new Label() { Text = "Job Title", Left = 10, Top = 40, AutoSize = true };
            txtJobTitle = new TextBox() { Left = 100, Top = 38, Width = 200 };

            Label lblGrade = new Label() { Text = "Grade", Left = 320, Top = 40, AutoSize = true };
            txtGrade = new TextBox() { Left = 410, Top = 38, Width = 150 };

            Label lblBlood = new Label() { Text = "Blood Type", Left = 10, Top = 70, AutoSize = true };
            txtBloodType = new TextBox() { Left = 100, Top = 68, Width = 100 };

            Label lblHire = new Label() { Text = "Hire Date", Left = 220, Top = 70, AutoSize = true };
            dtHireDate = new DateTimePicker() { Left = 280, Top = 66, Width = 120 };

            btnSave = new Button() { Text = "Save", Left = 10, Top = 100, Width = 100 };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblName);
            this.Controls.Add(txtFullName);
            this.Controls.Add(lblNat);
            this.Controls.Add(txtNationalID);
            this.Controls.Add(lblJob);
            this.Controls.Add(txtJobTitle);
            this.Controls.Add(lblGrade);
            this.Controls.Add(txtGrade);
            this.Controls.Add(lblBlood);
            this.Controls.Add(txtBloodType);
            this.Controls.Add(lblHire);
            this.Controls.Add(dtHireDate);
            this.Controls.Add(btnSave);
            this.Controls.Add(dgvEmployees);
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT EmpID, FullName, NationalID, JobTitle, Grade, BloodType, HireDate, DeptID FROM Employees", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmployees.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load employees error: " + ex.Message);
            }
        }

        private void LoadDepartments()
        {
            // TODO: load departments into a dropdown if needed
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO Employees (NationalID, FullName, JobTitle, Grade, BloodType, HireDate) 
                                   VALUES (@nat, @name, @title, @grade, @blood, @hire)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@nat", txtNationalID.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@title", txtJobTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@grade", txtGrade.Text.Trim());
                    cmd.Parameters.AddWithValue("@blood", txtBloodType.Text.Trim());
                    cmd.Parameters.AddWithValue("@hire", dtHireDate.Value.Date);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Employee saved.");
                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save error: " + ex.Message);
            }
        }
    }
}
