namespace EAInnovator.Views
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.ServerURLBox = new System.Windows.Forms.TextBox();
            this.DatabaseBox = new System.Windows.Forms.ComboBox();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.RefreshDbBtn = new System.Windows.Forms.Button();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.urllabel = new System.Windows.Forms.Label();
            this.databaseLabel = new System.Windows.Forms.Label();
            this.userName = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ServerURLBox
            // 
            this.ServerURLBox.Location = new System.Drawing.Point(24, 66);
            this.ServerURLBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ServerURLBox.Name = "ServerURLBox";
            this.ServerURLBox.Size = new System.Drawing.Size(678, 26);
            this.ServerURLBox.TabIndex = 0;
            // 
            // DatabaseBox
            // 
            this.DatabaseBox.FormattingEnabled = true;
            this.DatabaseBox.Location = new System.Drawing.Point(24, 126);
            this.DatabaseBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DatabaseBox.Name = "DatabaseBox";
            this.DatabaseBox.Size = new System.Drawing.Size(391, 28);
            this.DatabaseBox.TabIndex = 1;
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(33, 249);
            this.UsernameBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(382, 26);
            this.UsernameBox.TabIndex = 2;
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(33, 311);
            this.PasswordBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(382, 26);
            this.PasswordBox.TabIndex = 3;
            this.PasswordBox.UseSystemPasswordChar = true;
            // 
            // RefreshDbBtn
            // 
            this.RefreshDbBtn.BackgroundImage = global::EAInnovator.Properties.Resources.refresh;
            this.RefreshDbBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.RefreshDbBtn.Location = new System.Drawing.Point(442, 126);
            this.RefreshDbBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.RefreshDbBtn.Name = "RefreshDbBtn";
            this.RefreshDbBtn.Size = new System.Drawing.Size(46, 43);
            this.RefreshDbBtn.TabIndex = 4;
            this.RefreshDbBtn.UseVisualStyleBackColor = true;
            this.RefreshDbBtn.Click += new System.EventHandler(this.RefreshDbBtn_Click);
            // 
            // LoginBtn
            // 
            this.LoginBtn.Location = new System.Drawing.Point(591, 245);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(112, 35);
            this.LoginBtn.TabIndex = 5;
            this.LoginBtn.Text = "Login";
            this.LoginBtn.UseVisualStyleBackColor = true;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(591, 306);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(112, 35);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // urllabel
            // 
            this.urllabel.AutoSize = true;
            this.urllabel.Location = new System.Drawing.Point(20, 40);
            this.urllabel.Name = "urllabel";
            this.urllabel.Size = new System.Drawing.Size(92, 20);
            this.urllabel.TabIndex = 7;
            this.urllabel.Text = "Server URL";
            // 
            // databaseLabel
            // 
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.Location = new System.Drawing.Point(20, 100);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(79, 20);
            this.databaseLabel.TabIndex = 8;
            this.databaseLabel.Text = "Database";
            // 
            // userName
            // 
            this.userName.AutoSize = true;
            this.userName.Location = new System.Drawing.Point(32, 220);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(83, 20);
            this.userName.TabIndex = 9;
            this.userName.Text = "Username";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(28, 285);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(78, 20);
            this.passwordLabel.TabIndex = 10;
            this.passwordLabel.Text = "Password";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.LoginBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.databaseLabel);
            this.Controls.Add(this.urllabel);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.RefreshDbBtn);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.UsernameBox);
            this.Controls.Add(this.DatabaseBox);
            this.Controls.Add(this.ServerURLBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Text = "Login to Innovator";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ServerURLBox;
        private System.Windows.Forms.ComboBox DatabaseBox;
        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.Button RefreshDbBtn;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label urllabel;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.Label userName;
        private System.Windows.Forms.Label passwordLabel;
    }
}