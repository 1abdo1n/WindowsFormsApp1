namespace WindowsFormsApp1
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.btnLogin = new System.Windows.Forms.Button();
			this.lblUser = new System.Windows.Forms.Label();
			this.lblPass = new System.Windows.Forms.Label();
			this.pnlDashboard = new System.Windows.Forms.Panel();
			this.btnDepartments = new System.Windows.Forms.Button();
			this.btnEmployeesModule = new System.Windows.Forms.Button();
			this.btnContracts = new System.Windows.Forms.Button();
			this.btnAssets = new System.Windows.Forms.Button();
			this.btnAttendanceModule = new System.Windows.Forms.Button();
			this.btnLeave = new System.Windows.Forms.Button();
			this.btnPayrollModule = new System.Windows.Forms.Button();
			this.btnReports = new System.Windows.Forms.Button();
			this.btnLogout = new System.Windows.Forms.Button();
			this.pnlDashboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(453, 39);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(200, 22);
			this.txtUsername.TabIndex = 1;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(453, 67);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(200, 22);
			this.txtPassword.TabIndex = 3;
			// 
			// btnLogin
			// 
			this.btnLogin.Location = new System.Drawing.Point(506, 95);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(100, 23);
			this.btnLogin.TabIndex = 4;
			this.btnLogin.Text = "Login";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
			// 
			// lblUser
			// 
			this.lblUser.AutoSize = true;
			this.lblUser.Location = new System.Drawing.Point(234, 29);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(73, 16);
			this.lblUser.TabIndex = 0;
			this.lblUser.Text = "Username:";
			// 
			// lblPass
			// 
			this.lblPass.AutoSize = true;
			this.lblPass.Location = new System.Drawing.Point(234, 57);
			this.lblPass.Name = "lblPass";
			this.lblPass.Size = new System.Drawing.Size(70, 16);
			this.lblPass.TabIndex = 2;
			this.lblPass.Text = "Password:";
			// 
			// pnlDashboard
			// 
			this.pnlDashboard.Controls.Add(this.btnDepartments);
			this.pnlDashboard.Controls.Add(this.btnEmployeesModule);
			this.pnlDashboard.Controls.Add(this.btnContracts);
			this.pnlDashboard.Controls.Add(this.btnAssets);
			this.pnlDashboard.Controls.Add(this.btnAttendanceModule);
			this.pnlDashboard.Controls.Add(this.btnLeave);
			this.pnlDashboard.Controls.Add(this.btnPayrollModule);
			this.pnlDashboard.Controls.Add(this.btnReports);
			this.pnlDashboard.Controls.Add(this.btnLogout);
			this.pnlDashboard.Location = new System.Drawing.Point(10, 150);
			this.pnlDashboard.Name = "pnlDashboard";
			this.pnlDashboard.Size = new System.Drawing.Size(1000, 300);
			this.pnlDashboard.TabIndex = 5;
			this.pnlDashboard.Visible = false;
			// 
			// btnDepartments
			// 
			this.btnDepartments.Location = new System.Drawing.Point(261, 131);
			this.btnDepartments.Name = "btnDepartments";
			this.btnDepartments.Size = new System.Drawing.Size(120, 23);
			this.btnDepartments.TabIndex = 0;
			this.btnDepartments.Text = "Departments";
			this.btnDepartments.UseVisualStyleBackColor = true;
			this.btnDepartments.Click += new System.EventHandler(this.BtnDepartments_Click);
			// 
			// btnEmployeesModule
			// 
			this.btnEmployeesModule.Location = new System.Drawing.Point(261, 60);
			this.btnEmployeesModule.Name = "btnEmployeesModule";
			this.btnEmployeesModule.Size = new System.Drawing.Size(120, 23);
			this.btnEmployeesModule.TabIndex = 1;
			this.btnEmployeesModule.Text = "Employees";
			this.btnEmployeesModule.UseVisualStyleBackColor = true;
			this.btnEmployeesModule.Click += new System.EventHandler(this.BtnEmployeesModule_Click);
			// 
			// btnContracts
			// 
			this.btnContracts.Location = new System.Drawing.Point(463, 131);
			this.btnContracts.Name = "btnContracts";
			this.btnContracts.Size = new System.Drawing.Size(120, 23);
			this.btnContracts.TabIndex = 2;
			this.btnContracts.Text = "Contracts";
			this.btnContracts.UseVisualStyleBackColor = true;
			this.btnContracts.Click += new System.EventHandler(this.BtnContracts_Click);
			// 
			// btnAssets
			// 
			this.btnAssets.Location = new System.Drawing.Point(261, 209);
			this.btnAssets.Name = "btnAssets";
			this.btnAssets.Size = new System.Drawing.Size(120, 23);
			this.btnAssets.TabIndex = 3;
			this.btnAssets.Text = "Assets";
			this.btnAssets.UseVisualStyleBackColor = true;
			this.btnAssets.Click += new System.EventHandler(this.BtnAssets_Click);
			// 
			// btnAttendanceModule
			// 
			this.btnAttendanceModule.Location = new System.Drawing.Point(463, 60);
			this.btnAttendanceModule.Name = "btnAttendanceModule";
			this.btnAttendanceModule.Size = new System.Drawing.Size(120, 23);
			this.btnAttendanceModule.TabIndex = 4;
			this.btnAttendanceModule.Text = "Attendance";
			this.btnAttendanceModule.UseVisualStyleBackColor = true;
			this.btnAttendanceModule.Click += new System.EventHandler(this.BtnAttendanceModule_Click);
			// 
			// btnLeave
			// 
			this.btnLeave.Location = new System.Drawing.Point(463, 210);
			this.btnLeave.Name = "btnLeave";
			this.btnLeave.Size = new System.Drawing.Size(120, 23);
			this.btnLeave.TabIndex = 5;
			this.btnLeave.Text = "Leave Requests";
			this.btnLeave.UseVisualStyleBackColor = true;
			this.btnLeave.Click += new System.EventHandler(this.BtnLeave_Click);
			// 
			// btnPayrollModule
			// 
			this.btnPayrollModule.Location = new System.Drawing.Point(629, 131);
			this.btnPayrollModule.Name = "btnPayrollModule";
			this.btnPayrollModule.Size = new System.Drawing.Size(120, 23);
			this.btnPayrollModule.TabIndex = 6;
			this.btnPayrollModule.Text = "Payroll";
			this.btnPayrollModule.UseVisualStyleBackColor = true;
			this.btnPayrollModule.Click += new System.EventHandler(this.BtnPayrollModule_Click);
			// 
			// btnReports
			// 
			this.btnReports.Location = new System.Drawing.Point(629, 60);
			this.btnReports.Name = "btnReports";
			this.btnReports.Size = new System.Drawing.Size(120, 23);
			this.btnReports.TabIndex = 7;
			this.btnReports.Text = "Reports";
			this.btnReports.UseVisualStyleBackColor = true;
			this.btnReports.Click += new System.EventHandler(this.BtnReports_Click);
			// 
			// btnLogout
			// 
			this.btnLogout.Location = new System.Drawing.Point(629, 210);
			this.btnLogout.Name = "btnLogout";
			this.btnLogout.Size = new System.Drawing.Size(120, 23);
			this.btnLogout.TabIndex = 8;
			this.btnLogout.Text = "Logout";
			this.btnLogout.UseVisualStyleBackColor = true;
			this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1298, 618);
			this.Controls.Add(this.pnlDashboard);
			this.Controls.Add(this.lblUser);
			this.Controls.Add(this.lblPass);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.btnLogin);
			this.Name = "Form1";
			this.Text = "HRMS - Login";
			this.pnlDashboard.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblPass;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Panel pnlDashboard;
		private System.Windows.Forms.Button btnDepartments;
		private System.Windows.Forms.Button btnEmployeesModule;
		private System.Windows.Forms.Button btnContracts;
		private System.Windows.Forms.Button btnAssets;
		private System.Windows.Forms.Button btnAttendanceModule;
		private System.Windows.Forms.Button btnLeave;
		private System.Windows.Forms.Button btnPayrollModule;
		private System.Windows.Forms.Button btnReports;
		private System.Windows.Forms.Button btnLogout;

		#endregion
	}
}

