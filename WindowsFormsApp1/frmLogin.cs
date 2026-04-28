using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp1
{
    public partial class frmLogin : Form
    {
        // In-memory user storage (replace with database later)
        private class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Department { get; set; }
            public DateTime RegisteredDate { get; set; }
        }

        private static List<User> users = new List<User>();

        public frmLogin()
        {
            InitializeComponent();
            LoadSampleUsers();
        }

        private void LoadSampleUsers()
        {
            if (users.Count == 0)
            {
                // Admin User
                users.Add(new User
                {
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@hrsystem.com",
                    FullName = "System Administrator",
                    Role = "Admin",
                    Department = "",
                    RegisteredDate = DateTime.Now
                });

                // Manager User (Engineering Department)
                users.Add(new User
                {
                    Username = "manager",
                    Password = "manager123",
                    Email = "sara@hrsystem.com",
                    FullName = "Sara Mohamed",
                    Role = "Manager",
                    Department = "Human Resources",
                    RegisteredDate = DateTime.Now
                });

                // Employee User
                users.Add(new User
                {
                    Username = "employee",
                    Password = "emp123",
                    Email = "mohamed@hrsystem.com",
                    FullName = "Mohamed Ali",
                    Role = "Employee",
                    Department = "Engineering",
                    RegisteredDate = DateTime.Now
                });
            }
        }

        private void InitializeComponent()
        {
            // ── Form ──────────────────────────────────────────────
            this.Text = "HR System — Sign In / Register";
            this.Size = new Size(420, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(243, 244, 246);
            this.Font = new Font("Segoe UI", 9f);

            // ── Tab Control for Login/Register ────────────────────
            TabControl tabControl = new TabControl();
            tabControl.Size = new Size(360, 500);
            tabControl.Location = new Point(25, 25);
            tabControl.Font = new Font("Segoe UI", 9f);

            // ==================== LOGIN TAB ====================
            TabPage tabLogin = new TabPage("Sign In");

            // Login Panel
            Panel loginPanel = new Panel();
            loginPanel.Size = new Size(340, 440);
            loginPanel.Location = new Point(3, 3);
            loginPanel.BackColor = Color.White;

            // Logo strip
            Panel logoStrip = new Panel();
            logoStrip.Size = new Size(340, 52);
            logoStrip.Location = new Point(0, 0);
            logoStrip.BackColor = Color.FromArgb(26, 86, 219);

            Label lblAppName = new Label();
            lblAppName.Text = "HR Management System";
            lblAppName.ForeColor = Color.White;
            lblAppName.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblAppName.AutoSize = true;
            lblAppName.Location = new Point(16, 10);

            Label lblAppSub = new Label();
            lblAppSub.Text = "Employee Affairs Portal";
            lblAppSub.ForeColor = Color.FromArgb(180, 210, 255);
            lblAppSub.Font = new Font("Segoe UI", 8f);
            lblAppSub.AutoSize = true;
            lblAppSub.Location = new Point(16, 32);

            logoStrip.Controls.Add(lblAppName);
            logoStrip.Controls.Add(lblAppSub);

            // Login fields
            Label lblTitle = new Label();
            lblTitle.Text = "Welcome Back!";
            lblTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(24, 72);

            // Username
            Label lblUser = new Label();
            lblUser.Text = "Username";
            lblUser.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblUser.ForeColor = Color.FromArgb(55, 65, 81);
            lblUser.AutoSize = true;
            lblUser.Location = new Point(24, 110);

            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(292, 28);
            txtUsername.Location = new Point(24, 128);
            txtUsername.Font = new Font("Segoe UI", 10f);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.BackColor = Color.FromArgb(249, 250, 251);

            // Password
            Label lblPass = new Label();
            lblPass.Text = "Password";
            lblPass.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblPass.ForeColor = Color.FromArgb(55, 65, 81);
            lblPass.AutoSize = true;
            lblPass.Location = new Point(24, 172);

            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(260, 28);
            txtPassword.Location = new Point(24, 190);
            txtPassword.Font = new Font("Segoe UI", 10f);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.BackColor = Color.FromArgb(249, 250, 251);
            txtPassword.PasswordChar = '●';
            txtPassword.UseSystemPasswordChar = true;

            // Demo accounts hint
            Label lblDemoHint = new Label();
            lblDemoHint.Text = "Demo Accounts: admin/admin123 | manager/manager123 | employee/emp123";
            lblDemoHint.Font = new Font("Segoe UI", 7f);
            lblDemoHint.ForeColor = Color.FromArgb(156, 163, 175);
            lblDemoHint.AutoSize = true;
            lblDemoHint.Location = new Point(24, 340);

            // Eye button for password
            Button btnShowPass = new Button();
            btnShowPass.Size = new Size(30, 28);
            btnShowPass.Location = new Point(286, 190);
            btnShowPass.FlatStyle = FlatStyle.Flat;
            btnShowPass.FlatAppearance.BorderSize = 0;
            btnShowPass.BackColor = Color.Transparent;
            btnShowPass.Text = "👁";
            btnShowPass.Font = new Font("Segoe UI", 11f);
            btnShowPass.Cursor = Cursors.Hand;

            // Login error label
            Label lblLoginError = new Label();
            lblLoginError.Name = "lblLoginError";
            lblLoginError.Text = "";
            lblLoginError.ForeColor = Color.FromArgb(185, 28, 28);
            lblLoginError.Font = new Font("Segoe UI", 8.5f);
            lblLoginError.AutoSize = true;
            lblLoginError.Location = new Point(24, 228);

            // Login button
            Button btnLogin = new Button();
            btnLogin.Text = "Sign In";
            btnLogin.Size = new Size(292, 38);
            btnLogin.Location = new Point(24, 255);
            btnLogin.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.BackColor = Color.FromArgb(26, 86, 219);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;

            // Forgot password link
            LinkLabel lblForgotPass = new LinkLabel();
            lblForgotPass.Text = "Forgot Password?";
            lblForgotPass.Font = new Font("Segoe UI", 8.5f);
            lblForgotPass.LinkColor = Color.FromArgb(26, 86, 219);
            lblForgotPass.AutoSize = true;
            lblForgotPass.Location = new Point(24, 305);
            lblForgotPass.Click += (s, e) => MessageBox.Show("Contact your HR administrator to reset your password.", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Version footer
            Label lblVersion = new Label();
            lblVersion.Text = "HR System v1.0";
            lblVersion.Font = new Font("Segoe UI", 8f);
            lblVersion.ForeColor = Color.FromArgb(156, 163, 175);
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(24, 400);

            // Login Tab controls assembly
            loginPanel.Controls.Add(logoStrip);
            loginPanel.Controls.Add(lblTitle);
            loginPanel.Controls.Add(lblUser);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(lblPass);
            loginPanel.Controls.Add(txtPassword);
            loginPanel.Controls.Add(btnShowPass);
            loginPanel.Controls.Add(lblLoginError);
            loginPanel.Controls.Add(btnLogin);
            loginPanel.Controls.Add(lblForgotPass);
            loginPanel.Controls.Add(lblDemoHint);
            loginPanel.Controls.Add(lblVersion);

            tabLogin.Controls.Add(loginPanel);

            // ==================== REGISTER TAB ====================
            TabPage tabRegister = new TabPage("Register");

            // Register Panel
            Panel registerPanel = new Panel();
            registerPanel.Size = new Size(340, 470);
            registerPanel.Location = new Point(3, 3);
            registerPanel.BackColor = Color.White;
            registerPanel.AutoScroll = true;

            // Register logo
            Panel regLogoStrip = new Panel();
            regLogoStrip.Size = new Size(340, 52);
            regLogoStrip.Location = new Point(0, 0);
            regLogoStrip.BackColor = Color.FromArgb(26, 86, 219);

            Label lblRegAppName = new Label();
            lblRegAppName.Text = "Join Our Team";
            lblRegAppName.ForeColor = Color.White;
            lblRegAppName.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblRegAppName.AutoSize = true;
            lblRegAppName.Location = new Point(16, 10);

            Label lblRegAppSub = new Label();
            lblRegAppSub.Text = "Create your account to get started";
            lblRegAppSub.ForeColor = Color.FromArgb(180, 210, 255);
            lblRegAppSub.Font = new Font("Segoe UI", 8f);
            lblRegAppSub.AutoSize = true;
            lblRegAppSub.Location = new Point(16, 32);

            regLogoStrip.Controls.Add(lblRegAppName);
            regLogoStrip.Controls.Add(lblRegAppSub);

            // Register title
            Label lblRegTitle = new Label();
            lblRegTitle.Text = "Create New Account";
            lblRegTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblRegTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblRegTitle.AutoSize = true;
            lblRegTitle.Location = new Point(24, 65);

            // Full Name
            Label lblFullName = new Label();
            lblFullName.Text = "Full Name *";
            lblFullName.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblFullName.ForeColor = Color.FromArgb(55, 65, 81);
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(24, 100);

            TextBox txtFullName = new TextBox();
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(292, 28);
            txtFullName.Location = new Point(24, 118);
            txtFullName.Font = new Font("Segoe UI", 10f);
            txtFullName.BorderStyle = BorderStyle.FixedSingle;

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email Address *";
            lblEmail.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(55, 65, 81);
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(24, 158);

            TextBox txtEmail = new TextBox();
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(292, 28);
            txtEmail.Location = new Point(24, 176);
            txtEmail.Font = new Font("Segoe UI", 10f);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;

            // Register Username
            Label lblRegUser = new Label();
            lblRegUser.Text = "Username *";
            lblRegUser.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblRegUser.ForeColor = Color.FromArgb(55, 65, 81);
            lblRegUser.AutoSize = true;
            lblRegUser.Location = new Point(24, 216);

            TextBox txtRegUsername = new TextBox();
            txtRegUsername.Name = "txtRegUsername";
            txtRegUsername.Size = new Size(292, 28);
            txtRegUsername.Location = new Point(24, 234);
            txtRegUsername.Font = new Font("Segoe UI", 10f);
            txtRegUsername.BorderStyle = BorderStyle.FixedSingle;

            // Register Password
            Label lblRegPass = new Label();
            lblRegPass.Text = "Password *";
            lblRegPass.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblRegPass.ForeColor = Color.FromArgb(55, 65, 81);
            lblRegPass.AutoSize = true;
            lblRegPass.Location = new Point(24, 274);

            TextBox txtRegPassword = new TextBox();
            txtRegPassword.Name = "txtRegPassword";
            txtRegPassword.Size = new Size(260, 28);
            txtRegPassword.Location = new Point(24, 292);
            txtRegPassword.Font = new Font("Segoe UI", 10f);
            txtRegPassword.BorderStyle = BorderStyle.FixedSingle;
            txtRegPassword.PasswordChar = '●';
            txtRegPassword.UseSystemPasswordChar = true;

            // Eye button for register password
            Button btnRegShowPass = new Button();
            btnRegShowPass.Size = new Size(30, 28);
            btnRegShowPass.Location = new Point(286, 292);
            btnRegShowPass.FlatStyle = FlatStyle.Flat;
            btnRegShowPass.FlatAppearance.BorderSize = 0;
            btnRegShowPass.BackColor = Color.Transparent;
            btnRegShowPass.Text = "👁";
            btnRegShowPass.Font = new Font("Segoe UI", 11f);
            btnRegShowPass.Cursor = Cursors.Hand;

            // Confirm Password
            Label lblConfirmPass = new Label();
            lblConfirmPass.Text = "Confirm Password *";
            lblConfirmPass.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblConfirmPass.ForeColor = Color.FromArgb(55, 65, 81);
            lblConfirmPass.AutoSize = true;
            lblConfirmPass.Location = new Point(24, 332);

            TextBox txtConfirmPassword = new TextBox();
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(260, 28);
            txtConfirmPassword.Location = new Point(24, 350);
            txtConfirmPassword.Font = new Font("Segoe UI", 10f);
            txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.UseSystemPasswordChar = true;

            // Eye button for confirm password
            Button btnConfirmShowPass = new Button();
            btnConfirmShowPass.Size = new Size(30, 28);
            btnConfirmShowPass.Location = new Point(286, 350);
            btnConfirmShowPass.FlatStyle = FlatStyle.Flat;
            btnConfirmShowPass.FlatAppearance.BorderSize = 0;
            btnConfirmShowPass.BackColor = Color.Transparent;
            btnConfirmShowPass.Text = "👁";
            btnConfirmShowPass.Font = new Font("Segoe UI", 11f);
            btnConfirmShowPass.Cursor = Cursors.Hand;

            // Department dropdown for registration
            Label lblDept = new Label();
            lblDept.Text = "Department *";
            lblDept.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblDept.ForeColor = Color.FromArgb(55, 65, 81);
            lblDept.AutoSize = true;
            lblDept.Location = new Point(24, 390);

            ComboBox cmbDepartment = new ComboBox();
            cmbDepartment.Name = "cmbDepartment";
            cmbDepartment.Size = new Size(292, 28);
            cmbDepartment.Location = new Point(24, 408);
            cmbDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDepartment.Font = new Font("Segoe UI", 10f);
            cmbDepartment.Items.AddRange(new[] { "Engineering", "Human Resources", "Finance", "Sales", "Marketing", "IT Support", "Operations", "Legal" });

            // Register error label
            Label lblRegError = new Label();
            lblRegError.Name = "lblRegError";
            lblRegError.Text = "";
            lblRegError.ForeColor = Color.FromArgb(185, 28, 28);
            lblRegError.Font = new Font("Segoe UI", 8.5f);
            lblRegError.AutoSize = true;
            lblRegError.Location = new Point(24, 450);

            // Register button
            Button btnRegister = new Button();
            btnRegister.Text = "Create Account";
            btnRegister.Size = new Size(292, 38);
            btnRegister.Location = new Point(24, 475);
            btnRegister.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.BackColor = Color.FromArgb(34, 197, 94);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;

            // Register Tab controls assembly
            registerPanel.Controls.Add(regLogoStrip);
            registerPanel.Controls.Add(lblRegTitle);
            registerPanel.Controls.Add(lblFullName);
            registerPanel.Controls.Add(txtFullName);
            registerPanel.Controls.Add(lblEmail);
            registerPanel.Controls.Add(txtEmail);
            registerPanel.Controls.Add(lblRegUser);
            registerPanel.Controls.Add(txtRegUsername);
            registerPanel.Controls.Add(lblRegPass);
            registerPanel.Controls.Add(txtRegPassword);
            registerPanel.Controls.Add(btnRegShowPass);
            registerPanel.Controls.Add(lblConfirmPass);
            registerPanel.Controls.Add(txtConfirmPassword);
            registerPanel.Controls.Add(btnConfirmShowPass);
            registerPanel.Controls.Add(lblDept);
            registerPanel.Controls.Add(cmbDepartment);
            registerPanel.Controls.Add(lblRegError);
            registerPanel.Controls.Add(btnRegister);

            tabRegister.Controls.Add(registerPanel);

            // ==================== EVENT HANDLERS ====================

            // Login password eye toggle
            bool loginPassVisible = false;
            btnShowPass.Click += (s, e) =>
            {
                loginPassVisible = !loginPassVisible;
                if (loginPassVisible)
                {
                    txtPassword.PasswordChar = '\0';
                    txtPassword.UseSystemPasswordChar = false;
                    btnShowPass.Text = "🙈";
                }
                else
                {
                    txtPassword.PasswordChar = '●';
                    txtPassword.UseSystemPasswordChar = true;
                    btnShowPass.Text = "👁";
                }
            };

            // Register password eye toggle
            bool regPassVisible = false;
            btnRegShowPass.Click += (s, e) =>
            {
                regPassVisible = !regPassVisible;
                if (regPassVisible)
                {
                    txtRegPassword.PasswordChar = '\0';
                    txtRegPassword.UseSystemPasswordChar = false;
                    btnRegShowPass.Text = "🙈";
                }
                else
                {
                    txtRegPassword.PasswordChar = '●';
                    txtRegPassword.UseSystemPasswordChar = true;
                    btnRegShowPass.Text = "👁";
                }
            };

            // Confirm password eye toggle
            bool confirmPassVisible = false;
            btnConfirmShowPass.Click += (s, e) =>
            {
                confirmPassVisible = !confirmPassVisible;
                if (confirmPassVisible)
                {
                    txtConfirmPassword.PasswordChar = '\0';
                    txtConfirmPassword.UseSystemPasswordChar = false;
                    btnConfirmShowPass.Text = "🙈";
                }
                else
                {
                    txtConfirmPassword.PasswordChar = '●';
                    txtConfirmPassword.UseSystemPasswordChar = true;
                    btnConfirmShowPass.Text = "👁";
                }
            };

            // Login click event
            btnLogin.Click += (s, e) =>
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    lblLoginError.Text = "Please enter both username and password.";
                    return;
                }

                var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    lblLoginError.Text = "";
                    MessageBox.Show($"Welcome back, {user.FullName}!\nRole: {user.Role}\nDepartment: {user.Department}",
                        "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frmMainNew main = new frmMainNew(user.Role, user.Username, user.Department);
                    main.Show();
                    this.Hide();
                }
                else
                {
                    lblLoginError.Text = "Invalid username or password. Please try again.";
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            };

            // Register click event
            btnRegister.Click += (s, e) =>
            {
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string username = txtRegUsername.Text.Trim();
                string password = txtRegPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;
                string department = cmbDepartment.SelectedItem?.ToString() ?? "";

                // Validation
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(department))
                {
                    lblRegError.Text = "Please fill in all required fields (*).";
                    return;
                }

                if (!IsValidEmail(email))
                {
                    lblRegError.Text = "Please enter a valid email address.";
                    return;
                }

                if (password.Length < 4)
                {
                    lblRegError.Text = "Password must be at least 4 characters long.";
                    return;
                }

                if (password != confirmPassword)
                {
                    lblRegError.Text = "Passwords do not match.";
                    return;
                }

                if (users.Any(u => u.Username == username))
                {
                    lblRegError.Text = "Username already exists. Please choose another.";
                    return;
                }

                if (users.Any(u => u.Email == email))
                {
                    lblRegError.Text = "Email already registered. Please use another.";
                    return;
                }

                // Create new user (default role is Employee)
                users.Add(new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    FullName = fullName,
                    Role = "Employee",
                    Department = department,
                    RegisteredDate = DateTime.Now
                });

                MessageBox.Show($"Account created successfully!\n\nWelcome to HR System, {fullName}!\nDepartment: {department}\nYou can now login with your credentials.",
                    "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear registration form
                txtFullName.Clear();
                txtEmail.Clear();
                txtRegUsername.Clear();
                txtRegPassword.Clear();
                txtConfirmPassword.Clear();
                cmbDepartment.SelectedIndex = -1;
                lblRegError.Text = "";

                // Switch to login tab
                tabControl.SelectedTab = tabLogin;
                txtUsername.Text = username;
                txtPassword.Focus();
            };

            // Enter key events
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnLogin.PerformClick();
            };

            txtRegPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnRegister.PerformClick();
            };

            txtConfirmPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnRegister.PerformClick();
            };

            // Add tabs to control
            tabControl.TabPages.Add(tabLogin);
            tabControl.TabPages.Add(tabRegister);

            this.Controls.Add(tabControl);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}