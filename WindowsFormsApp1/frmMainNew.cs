using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class frmMainNew : Form
    {
        private readonly Color Blue = Color.FromArgb(26, 86, 219);
        private readonly Color BgPage = Color.FromArgb(243, 244, 246);
        private readonly Color BgCard = Color.White;
        private readonly Color SidebarBg = Color.FromArgb(15, 23, 42);
        private readonly Color BorderClr = Color.FromArgb(209, 213, 219);
        private readonly Color TxtPrimary = Color.FromArgb(17, 24, 39);
        private readonly Color TxtGray = Color.FromArgb(107, 114, 128);

        private string currentRole;
        private string currentUserId;
        private string currentUserDept;

        public frmMainNew(string role = "Admin", string userId = "", string userDept = "")
        {
            currentRole = role;
            currentUserId = userId;
            currentUserDept = userDept;

            Text = "HR Management System — Dashboard";
            Size = new Size(1100, 700);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BgPage;
            Font = new Font("Segoe UI", 9f);
            FormClosing += (s, e) => Application.Exit();

            // ── SIDEBAR ──────────────────────────────────────────────────
            Panel sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 210;
            sidebar.BackColor = SidebarBg;

            var lblBrand = MakeLabel("HR System", new Font("Segoe UI", 13f, FontStyle.Bold), Color.White, new Point(18, 22));
            var lblSub = MakeLabel("Management Portal", new Font("Segoe UI", 8.5f), Color.FromArgb(100, 116, 139), new Point(18, 48));
            sidebar.Controls.Add(lblBrand);
            sidebar.Controls.Add(lblSub);

            var div = new Panel { Size = new Size(210, 1), Location = new Point(0, 72), BackColor = Color.FromArgb(30, 41, 59) };
            sidebar.Controls.Add(div);

            string[] navItems;
            if (currentRole == "Admin")
            {
                navItems = new string[] { "Dashboard", "Employees", "Attendance", "Departments", "Contracts", "Payroll", "Leave", "Assets", "Reports", "Performance" };
            }
            else if (currentRole == "Manager")
            {
                navItems = new string[] { "Dashboard", "Employees", "Attendance", "Leave", "Assets", "Reports" };
            }
            else
            {
                navItems = new string[] { "Dashboard", "My Attendance", "My Leave", "My Contract" };
            }

            int navY = 86;
            foreach (string item in navItems)
            {
                var btn = new Button
                {
                    Text = item,
                    Size = new Size(210, 38),
                    Location = new Point(0, navY),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Segoe UI", 9.5f),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(18, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    Tag = item
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
                btn.Click += NavBtn_Click;
                sidebar.Controls.Add(btn);
                navY += 40;
            }

            var btnLogout = new Button
            {
                Text = "⇠  Logout",
                Size = new Size(186, 34),
                Location = new Point(12, navY + 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(51, 65, 85);
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 29, 29);
            btnLogout.Click += (s, e) => DoLogout();
            sidebar.Controls.Add(btnLogout);

            // ── TOP BAR ───────────────────────────────────────────────────
            Panel topbar = new Panel();
            topbar.Dock = DockStyle.Top;
            topbar.Height = 52;
            topbar.BackColor = BgCard;
            topbar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(BorderClr), 0, 51, topbar.Width, 51);

            var lblPageTitle = MakeLabel("Dashboard", new Font("Segoe UI", 13f, FontStyle.Bold), TxtPrimary, new Point(20, 14));
            var lblUserInfo = new Label
            {
                Text = currentRole == "Admin" ? "Admin User | HR Manager" : (currentRole == "Manager" ? "Manager | Department Head" : "Employee | Staff"),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = TxtGray,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            topbar.Controls.Add(lblPageTitle);
            topbar.Controls.Add(lblUserInfo);
            topbar.Resize += (s, e) => lblUserInfo.Location = new Point(topbar.Width - lblUserInfo.Width - 20, 17);

            // ── MAIN CONTENT ──────────────────────────────────────
            Panel content = new Panel();
            content.Dock = DockStyle.Fill;
            content.BackColor = BgPage;
            content.Padding = new Padding(20, 16, 20, 16);
            content.AutoScroll = true;

            // Welcome text
            string welcomeName = currentRole == "Admin" ? "Ahmed" : (currentRole == "Manager" ? "Sara" : "Mohamed");
            var lblWelcome = MakeLabel($"Welcome back, {welcomeName}!", new Font("Segoe UI", 15f, FontStyle.Bold), TxtPrimary, new Point(0, 0));
            var lblDate = MakeLabel(DateTime.Now.ToString("dddd, MMMM dd, yyyy"), new Font("Segoe UI", 9.5f), TxtGray, new Point(0, 32));
            content.Controls.Add(lblWelcome);
            content.Controls.Add(lblDate);

            // ── STAT CARDS ROW ────────────────────────────────────────────
            var statsRow = new FlowLayoutPanel
            {
                Location = new Point(0, 62),
                Height = 90,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = false,
                AutoScroll = false,
                BackColor = BgPage
            };
            content.Controls.Add(statsRow);
            content.Resize += (s, e) => statsRow.Width = content.ClientSize.Width - 40;
            statsRow.Width = 800;

            (string icon, string val, string lbl)[] stats;
            if (currentRole == "Admin")
            {
                stats = new (string, string, string)[] { ("👥", "156", "Employees"), ("🏢", "8", "Departments"), ("💰", "245K EGP", "Payroll"), ("📊", "94%", "Attendance") };
            }
            else if (currentRole == "Manager")
            {
                string[] teamMembersList = { "Ahmed Hassan", "Nour Khaled", "Omar Ali" };
                int teamCount = teamMembersList.Length;
                stats = new (string, string, string)[] { ("👥", teamCount.ToString(), "My Team"), ("📊", "94%", "Attendance"), ("⭐", "4.5", "Rating"), ("📅", "3", "Pending") };
            }
            else
            {
                stats = new (string, string, string)[] { ("📅", "22", "Vacation"), ("📊", "96%", "Attendance"), ("⭐", "4.8", "Rating"), ("💰", "12.5K", "Salary") };
            }

            foreach (var st in stats)
            {
                var card = new Panel { Size = new Size(175, 80), BackColor = BgCard, Margin = new Padding(0, 0, 12, 0) };
                card.Paint += (s, e) => ((Panel)s).CreateGraphics().DrawRectangle(new Pen(BorderClr), 0, 0, card.Width - 1, card.Height - 1);
                card.Controls.Add(MakeLabel(st.icon, new Font("Segoe UI", 22f), TxtPrimary, new Point(10, 22)));
                card.Controls.Add(MakeLabel(st.val, new Font("Segoe UI", 15f, FontStyle.Bold), Blue, new Point(68, 14)));
                card.Controls.Add(MakeLabel(st.lbl, new Font("Segoe UI", 8.5f), TxtGray, new Point(68, 48)));
                statsRow.Controls.Add(card);
            }

            // ── DEPARTMENT / TEAM SECTION ─────────────────────────────────────────
            string sectionTitle = currentRole == "Admin" ? "DEPARTMENT OVERVIEW" : (currentRole == "Manager" ? "MY TEAM" : "MY INFO");
            var lblSecTitle = new Label
            {
                Text = sectionTitle,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = TxtGray,
                AutoSize = true,
                Location = new Point(0, 164)
            };
            content.Controls.Add(lblSecTitle);

            var deptCard = new Panel
            {
                Location = new Point(0, 186),
                BackColor = BgCard,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            deptCard.Paint += (s, e) => ((Panel)s).CreateGraphics().DrawRectangle(new Pen(BorderClr), 0, 0, deptCard.Width - 1, deptCard.Height - 1);
            content.Controls.Add(deptCard);
            content.Resize += (s, e) => deptCard.Width = content.ClientSize.Width - 40;
            deptCard.Width = 800;

            if (currentRole == "Admin")
            {
                deptCard.Controls.Add(MakeLabel("Department", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(16, 12)));
                deptCard.Controls.Add(MakeLabel("Employees", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(260, 12)));

                string[] depts = { "Engineering", "Human Resources", "Finance", "Sales", "Marketing", "IT Support", "Operations", "Legal" };
                int[] counts = { 42, 18, 15, 28, 12, 10, 22, 9 };

                for (int i = 0; i < depts.Length; i++)
                {
                    int rowY = 48 + i * 38;
                    var row = new Panel { Location = new Point(0, rowY), Height = 38, BackColor = i % 2 == 0 ? BgCard : Color.FromArgb(249, 250, 251), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                    row.Width = 800;
                    row.Controls.Add(MakeLabel(depts[i], new Font("Segoe UI", 9.5f), TxtPrimary, new Point(16, 10)));
                    row.Controls.Add(MakeLabel(counts[i].ToString(), new Font("Segoe UI", 9.5f, FontStyle.Bold), Blue, new Point(260, 10)));
                    deptCard.Controls.Add(row);
                    content.Resize += (s, e) => row.Width = deptCard.Width;
                }
                deptCard.Height = 48 + depts.Length * 38 + 15;
            }
            else if (currentRole == "Manager")
            {
                deptCard.Controls.Add(MakeLabel("Team Member", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(16, 12)));
                deptCard.Controls.Add(MakeLabel("Position", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(260, 12)));

                string[] teamMembers = { "Ahmed Hassan", "Nour Khaled", "Omar Ali" };
                string[] positions = { "Senior Developer", "Junior Developer", "QA Engineer" };

                for (int i = 0; i < teamMembers.Length; i++)
                {
                    int rowY = 48 + i * 38;
                    var row = new Panel { Location = new Point(0, rowY), Height = 38, BackColor = i % 2 == 0 ? BgCard : Color.FromArgb(249, 250, 251), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                    row.Width = 800;
                    row.Controls.Add(MakeLabel(teamMembers[i], new Font("Segoe UI", 9.5f), TxtPrimary, new Point(16, 10)));
                    row.Controls.Add(MakeLabel(positions[i], new Font("Segoe UI", 9.5f), TxtGray, new Point(260, 10)));
                    deptCard.Controls.Add(row);
                    content.Resize += (s, e) => row.Width = deptCard.Width;
                }
                deptCard.Height = 48 + teamMembers.Length * 38 + 15;
            }
            else
            {
                // Employee - show personal info
                deptCard.Controls.Add(MakeLabel("Name:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(16, 20)));
                deptCard.Controls.Add(MakeLabel("Mohamed Ali", new Font("Segoe UI", 9.5f), TxtPrimary, new Point(120, 20)));
                deptCard.Controls.Add(MakeLabel("Department:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(16, 55)));
                deptCard.Controls.Add(MakeLabel("Engineering", new Font("Segoe UI", 9.5f), TxtPrimary, new Point(120, 55)));
                deptCard.Controls.Add(MakeLabel("Position:", new Font("Segoe UI", 9f, FontStyle.Bold), Color.FromArgb(55, 65, 81), new Point(16, 90)));
                deptCard.Controls.Add(MakeLabel("Software Developer", new Font("Segoe UI", 9.5f), TxtPrimary, new Point(120, 90)));
                deptCard.Height = 130;
            }

            this.Controls.Add(content);
            this.Controls.Add(topbar);
            this.Controls.Add(sidebar);
        }

        private Label MakeLabel(string text, Font font, Color color, Point loc)
        {
            return new Label { Text = text, Font = font, ForeColor = color, AutoSize = true, Location = loc };
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Tag?.ToString() ?? "";
            if (name == "Dashboard") return;

            Form frm = null;
            if (currentRole == "Admin")
            {
                switch (name)
                {
                    case "Employees": frm = new frmEmployees(); break;
                    case "Attendance": frm = new frmAttendance(); break;
                    case "Departments": frm = new frmDepartments(); break;
                    case "Contracts": frm = new frmContracts(); break;
                    case "Payroll": frm = new frmPayroll(); break;
                    case "Leave": frm = new frmLeave(); break;
                    case "Assets": frm = new frmAssets(); break;
                    case "Reports": frm = new frmReports(); break;
                    case "Performance": frm = new frmPerformance(); break;
                }
            }
            else if (currentRole == "Manager")
            {
                switch (name)
                {
                    case "Employees": frm = new frmEmployees(currentUserDept); break;
                    case "Attendance": frm = new frmAttendance(currentUserDept); break;
                    case "Leave": frm = new frmLeave(currentUserDept); break;
                    case "Assets": frm = new frmAssets(currentUserDept); break;
                    case "Reports": frm = new frmReports(currentUserDept); break;
                }
            }
            else
            {
                switch (name)
                {
                    case "My Attendance": frm = new frmAttendance("", currentUserId); break;
                    case "My Leave": frm = new frmLeave("", currentUserId); break;
                    case "My Contract": MessageBox.Show("Contract Information\n\nContract Type: Full Time\nStart Date: Jan 15, 2023\nEnd Date: Jan 14, 2026\nSalary: $12,500/month", "My Contract"); return;
                }
            }
            frm?.Show();
        }

        private void DoLogout()
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                frmLogin login = new frmLogin();
                login.Show();
                this.Hide();
            }
        }
    }
}