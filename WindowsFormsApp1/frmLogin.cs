using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class frmLogin : Form
    {
        // ── Colors ────────────────────────────────────────────────
        private static readonly Color Blue = Color.FromArgb(26, 86, 219);
        private static readonly Color Green = Color.FromArgb(34, 197, 94);
        private static readonly Color BgGray = Color.FromArgb(243, 244, 246);
        private static readonly Color LabelFg = Color.FromArgb(55, 65, 81);
        private static readonly Color GrayFg = Color.FromArgb(156, 163, 175);
        private static readonly Color RedFg = Color.FromArgb(185, 28, 28);
        private static readonly Color InputBg = Color.FromArgb(249, 250, 251);

        // ── In-memory users (swap out for DB calls later) ─────────
        private class User
        {
            public string Username, Password, Email, FullName, Role, Department;
        }

        private static readonly List<User> users = new List<User>
        {
            new User { Username="admin",    Password="admin123",   Email="admin@hrsystem.com",   FullName="System Administrator", Role="Admin",    Department="" },
            new User { Username="manager",  Password="manager123", Email="sara@hrsystem.com",    FullName="Sara Mohamed",         Role="Manager",  Department="Human Resources" },
            new User { Username="employee", Password="emp123",     Email="mohamed@hrsystem.com", FullName="Mohamed Ali",          Role="Employee", Department="Engineering" },
        };

        public frmLogin()
        {
            Text = "HR System — Sign In";
            Size = new Size(420, 590);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = BgGray;
            Font = new Font("Segoe UI", 9f);

            var tabs = new TabControl { Size = new Size(366, 520), Location = new Point(22, 22), Font = new Font("Segoe UI", 9f) };
            tabs.TabPages.Add(BuildLoginTab(tabs));
            tabs.TabPages.Add(BuildRegisterTab(tabs));
            Controls.Add(tabs);
        }

        // ─────────────────────────────────────────────────────────
        //  LOGIN TAB
        // ─────────────────────────────────────────────────────────
        private TabPage BuildLoginTab(TabControl tabs)
        {
            var page = new TabPage("Sign In");
            var panel = new Panel { Size = new Size(344, 460), Location = new Point(3, 3), BackColor = Color.White };

            // Blue header strip
            panel.Controls.Add(MakeHeader("HR Management System", "Employee Affairs Portal"));

            // Fields
            panel.Controls.Add(MakeLabel("Welcome Back!", new Font("Segoe UI", 11f, FontStyle.Bold), Color.FromArgb(30, 30, 30), new Point(24, 72)));
            panel.Controls.Add(MakeLabel("Username", null, LabelFg, new Point(24, 110)));

            var txtUser = MakeTextBox("txtUsername", new Point(24, 128), 292);
            panel.Controls.Add(txtUser);

            panel.Controls.Add(MakeLabel("Password", null, LabelFg, new Point(24, 172)));
            var txtPass = MakeTextBox("txtPassword", new Point(24, 190), 258, isPassword: true);
            var btnEye = MakeEyeBtn(new Point(284, 190), txtPass);
            panel.Controls.Add(txtPass);
            panel.Controls.Add(btnEye);

            var lblError = MakeLabel("", null, RedFg, new Point(24, 228));
            panel.Controls.Add(lblError);

            // Login button
            var btnLogin = MakePrimaryBtn("Sign In", Blue, new Point(24, 252), 292);
            panel.Controls.Add(btnLogin);

            // Forgot / Contact HR link
            var lnkContact = new LinkLabel { Text = "Forgot password? Contact HR", Font = new Font("Segoe UI", 8.5f), LinkColor = Blue, AutoSize = true, Location = new Point(24, 302) };
            lnkContact.Click += (s, e) => MessageBox.Show("Contact your HR administrator to reset your password.\n\nEmail: hr@hrsystem.com\nPhone: +20-2-XXXX-XXXX", "Contact HR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            panel.Controls.Add(lnkContact);

            // Demo hint
            panel.Controls.Add(MakeLabel("Demo: admin/admin123 | manager/manager123 | employee/emp123", new Font("Segoe UI", 7f), GrayFg, new Point(24, 340)));
            panel.Controls.Add(MakeLabel("HR System v1.0", new Font("Segoe UI", 8f), GrayFg, new Point(24, 420)));

            // Login logic
            btnLogin.Click += (s, e) =>
            {
                string u = txtUser.Text.Trim();
                string p = txtPass.Text;
                if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) { lblError.Text = "Please enter username and password."; return; }

                var user = users.FirstOrDefault(x => x.Username == u && x.Password == p);
                if (user != null)
                {
                    lblError.Text = "";
                    MessageBox.Show($"Welcome back, {user.FullName}!\nRole: {user.Role}", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new frmMainNew(user.Role, user.Username, user.Department).Show();
                    Hide();
                }
                else
                {
                    lblError.Text = "Invalid username or password.";
                    txtPass.Clear(); txtPass.Focus();
                }
            };
            txtPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };

            page.Controls.Add(panel);
            return page;
        }

        // ─────────────────────────────────────────────────────────
        //  REGISTER TAB
        // ─────────────────────────────────────────────────────────
        private TabPage BuildRegisterTab(TabControl tabs)
        {
            var page = new TabPage("Register");
            var panel = new Panel { Size = new Size(344, 520), Location = new Point(3, 3), BackColor = Color.White, AutoScroll = true };

            panel.Controls.Add(MakeHeader("Join Our Team", "Create your account to get started"));
            panel.Controls.Add(MakeLabel("Create New Account", new Font("Segoe UI", 11f, FontStyle.Bold), Color.FromArgb(30, 30, 30), new Point(24, 65)));

            // Build fields
            int y = 100;
            var txtFullName = AddField(panel, "Full Name *", ref y, 292);
            var txtEmail = AddField(panel, "Email Address *", ref y, 292);
            var txtRegUser = AddField(panel, "Username *", ref y, 292);
            var txtRegPass = AddField(panel, "Password *", ref y, 258, isPassword: true);
            var btnEye1 = MakeEyeBtn(new Point(284, y - 30), txtRegPass);
            panel.Controls.Add(btnEye1);
            var txtConfPass = AddField(panel, "Confirm Password *", ref y, 258, isPassword: true);
            var btnEye2 = MakeEyeBtn(new Point(284, y - 30), txtConfPass);
            panel.Controls.Add(btnEye2);

            // Department dropdown
            panel.Controls.Add(MakeLabel("Department *", null, LabelFg, new Point(24, y)));
            var cmbDept = new ComboBox { Size = new Size(292, 28), Location = new Point(24, y + 18), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            cmbDept.Items.AddRange(new[] { "Engineering", "Human Resources", "Finance", "Sales", "Marketing", "IT Support", "Operations", "Legal" });
            panel.Controls.Add(cmbDept); y += 56;

            var lblRegError = MakeLabel("", null, RedFg, new Point(24, y));
            panel.Controls.Add(lblRegError); y += 24;

            var btnRegister = MakePrimaryBtn("Create Account", Green, new Point(24, y), 292);
            panel.Controls.Add(btnRegister);

            // Register logic
            btnRegister.Click += (s, e) =>
            {
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string username = txtRegUser.Text.Trim();
                string password = txtRegPass.Text;
                string confirm = txtConfPass.Text;
                string dept = cmbDept.SelectedItem?.ToString() ?? "";

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dept))
                { lblRegError.Text = "Please fill in all required fields (*)."; return; }

                if (!IsValidEmail(email)) { lblRegError.Text = "Please enter a valid email address."; return; }
                if (password.Length < 4) { lblRegError.Text = "Password must be at least 4 characters."; return; }
                if (password != confirm) { lblRegError.Text = "Passwords do not match."; return; }
                if (users.Any(u => u.Username == username)) { lblRegError.Text = "Username already exists."; return; }
                if (users.Any(u => u.Email == email)) { lblRegError.Text = "Email already registered."; return; }

                users.Add(new User { Username = username, Password = password, Email = email, FullName = fullName, Role = "Employee", Department = dept });
                MessageBox.Show($"Account created!\n\nWelcome, {fullName}!\nDepartment: {dept}\nYou can now sign in.", "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear & switch to login tab
                txtFullName.Clear(); txtEmail.Clear(); txtRegUser.Clear(); txtRegPass.Clear(); txtConfPass.Clear(); cmbDept.SelectedIndex = -1; lblRegError.Text = "";
                tabs.SelectedIndex = 0;
            };

            txtConfPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnRegister.PerformClick(); };

            page.Controls.Add(panel);
            return page;
        }

        // ─────────────────────────────────────────────────────────
        //  HELPER METHODS  (removes all the repetition)
        // ─────────────────────────────────────────────────────────

        private Panel MakeHeader(string title, string subtitle)
        {
            var strip = new Panel { Size = new Size(344, 52), Location = new Point(0, 0), BackColor = Blue };
            strip.Controls.Add(new Label { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 11f, FontStyle.Bold), AutoSize = true, Location = new Point(16, 10) });
            strip.Controls.Add(new Label { Text = subtitle, ForeColor = Color.FromArgb(180, 210, 255), Font = new Font("Segoe UI", 8f), AutoSize = true, Location = new Point(16, 32) });
            return strip;
        }

        private Label MakeLabel(string text, Font font, Color color, Point loc)
        {
            return new Label { Text = text, Font = font ?? new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = color, AutoSize = true, Location = loc };
        }

        private TextBox MakeTextBox(string name, Point loc, int width, bool isPassword = false)
        {
            var t = new TextBox { Name = name, Size = new Size(width, 28), Location = loc, Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = InputBg };
            if (isPassword) { t.PasswordChar = '●'; t.UseSystemPasswordChar = true; }
            return t;
        }

        // Adds a labelled textbox and advances y
        private TextBox AddField(Panel panel, string label, ref int y, int width, bool isPassword = false)
        {
            panel.Controls.Add(MakeLabel(label, null, LabelFg, new Point(24, y)));
            var txt = MakeTextBox("", new Point(24, y + 18), width, isPassword);
            panel.Controls.Add(txt);
            y += 56;
            return txt;
        }

        private Button MakeEyeBtn(Point loc, TextBox target)
        {
            var btn = new Button { Size = new Size(30, 28), Location = loc, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Text = "👁", Font = new Font("Segoe UI", 11f), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            bool visible = false;
            btn.Click += (s, e) =>
            {
                visible = !visible;
                target.PasswordChar = visible ? '\0' : '●';
                target.UseSystemPasswordChar = !visible;
                btn.Text = visible ? "🙈" : "👁";
            };
            return btn;
        }

        private Button MakePrimaryBtn(string text, Color bg, Point loc, int width)
        {
            var b = new Button { Text = text, Size = new Size(width, 38), Location = loc, Font = new Font("Segoe UI", 10f, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private bool IsValidEmail(string email)
        {
            try { return new System.Net.Mail.MailAddress(email).Address == email; }
            catch { return false; }
        }
    }
}