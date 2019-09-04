using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EAInnovator.Helpers;
using EAInnovator.Services;

namespace EAInnovator.Views
{
    public partial class LoginForm : Form
    {
        string serverURL;
        string database;

        public LoginForm(string serverURL = "", string database = "")
        {
            this.serverURL = serverURL;
            this.database = database;

            InitializeComponent();
        }

        private void RefreshDbBtn_Click(object sender, EventArgs e)
        {
            ReloadDBList();
        }

        private void ReloadDBList(string selectedDb = "")
        {
            string[] dbList;

            if (!string.IsNullOrWhiteSpace(ServerURLBox.Text))
            {
                try
                {
                    dbList = InnovatorService.GetDatabases(ServerURLBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DatabaseBox.Items.Clear();

                for (var i = 0; i < dbList.Count(); i++)
                {
                    DatabaseBox.Items.Add(dbList[i]);
                    if (dbList[i] == selectedDb)
                        DatabaseBox.SelectedIndex = i;
                }

                if (DatabaseBox.Items.Count > 0 && DatabaseBox.SelectedIndex < 0)
                    DatabaseBox.SelectedIndex = 0;
            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (InnovatorService._.IsLoggedIn)
                InnovatorService._.Logout();

            if (!InnovatorService._.Login(ServerURLBox.Text, DatabaseBox.SelectedItem.ToString(), UsernameBox.Text, PasswordBox.Text))
            {
                DialogResult = DialogResult.None;

                MessageBox.Show(InnovatorService._.LastError, "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                IEACConfig._["ServerURL"] = ServerURLBox.Text;
                IEACConfig._["Database"] = DatabaseBox.SelectedItem.ToString();
                IEACConfig._["Username"] = UsernameBox.Text;
                IEACConfig._["Password"] = PasswordBox.Text;

                IEACConfig._.save();
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string selectedDb;

            if (string.IsNullOrEmpty(serverURL) || string.IsNullOrEmpty(database))
            {
                ServerURLBox.Text = IEACConfig._["ServerURL"] ?? "";
                selectedDb = IEACConfig._["Database"] ?? "";
            }
            else
            {
                ServerURLBox.Text = serverURL;
                selectedDb = database;

                ServerURLBox.Enabled = false;
                DatabaseBox.Enabled = false;
            }

            if (!string.IsNullOrWhiteSpace(ServerURLBox.Text))
            {
                ReloadDBList(selectedDb);
            }

            UsernameBox.Text = IEACConfig._["Username"] ?? "";
            PasswordBox.Text = IEACConfig._["Password"] ?? "";
        }
    }
}