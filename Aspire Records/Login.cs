using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace Aspire_Records
{
    public partial class Login : Form
    {
        
        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            clearValidations();
            
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string checkusername = "select Admin from Users where Username='" + txtUsername.Text + "'";
            SqlCommand com = new SqlCommand(checkusername, conn);

            // check if username is admin or not and call function for corresponding admin or user

            int admin = Convert.ToInt32(com.ExecuteScalar());

            if (admin == 1)
            {
                adminUser();
            }
            else
                userUser();




           
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // exit app
            Application.Exit();

        }

        private void adminUser()
        {
            // if username is admin, check for password then open home form with admin privileges
            
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string checkusername = "select count(*) from Users where Username='" + txtUsername.Text + "'";
            SqlCommand com = new SqlCommand(checkusername, conn);
            int temp = Convert.ToInt32(com.ExecuteScalar().ToString());
            conn.Close();
            if (temp == 1)
            {
                conn.Open();
                string checkPasswordQuery = "select Password from Users where username='" + txtUsername.Text + "'";
                SqlCommand passCom = new SqlCommand(checkPasswordQuery, conn);
                string password = passCom.ExecuteScalar().ToString().Replace(" ", "");
                if (password == txtPassword.Text)
                {
                   
                    this.Hide();
                    Home admin1 = new Home();
                    admin1.userSession(txtUsername.Text);
                    admin1.adminRole();
                    admin1.HomeUser = txtUsername.Text;
                    admin1.Text = "Home - Admin";
                    admin1.Show();


                }
                else if (txtPassword.Text.Length == 0)
                {
                    MessageBox.Show("Password is required!");
                    txtPassword.Focus();

                }
                else
                {
                    wrongPassword();
                }


            }
            else
            {
                if (txtUsername.Text.Length == 0)
                {
                    MessageBox.Show("Username is required!", "Attention!");
                    txtUsername.Focus();
                }
                else
                {
                    label3.Text = "Incorrect username!";
                    wrongUsername();
                }
            }
        }

        private void wrongUsername()
        {
            // username is incorrect
            label3.Text = "Incorrect username!";
            txtUsername.Focus();
        }

        private void wrongPassword()
        {
            // password is incorrect
            label4.Text = "Incorrect password!";
            txtPassword.Focus();
        }

        private void userUser()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string checkusername = "select count(*) from Users where Username='" + txtUsername.Text + "'";
            SqlCommand com = new SqlCommand(checkusername, conn);
            int temp = Convert.ToInt32(com.ExecuteScalar().ToString());
            conn.Close();

            // if username is user, open home form with user privileges

            if (temp == 1)
            {
                conn.Open();
                string checkPasswordQuery = "select password from Users where Username='" + txtUsername.Text + "'";
                SqlCommand passCom = new SqlCommand(checkPasswordQuery, conn);
                string password = passCom.ExecuteScalar().ToString().Replace(" ", "");
                if (password == txtPassword.Text)
                {
                    this.Hide();
                    Home user1 = new Home();
                    user1.userSession(txtUsername.Text);
                    user1.HomeUser = txtUsername.Text;
                    user1.userRole();
                    user1.Text = "Home - User"; 
                    user1.Show();

                }
                else if (txtPassword.Text.Length == 0)
                {
                    // blank password
                    MessageBox.Show("Password is required!", "Attention");
                    txtPassword.Focus();

                }
                else
                {
                    wrongPassword();
                }


            }
            else
            {
                if (txtUsername.Text.Length == 0)
                {
                    // blank username
                    MessageBox.Show("Username is required!");
                    txtUsername.Focus();
                }
                else
                {
                    wrongUsername();
                }
            }
        }

        public void clearValidations()
        {
            // clear text validations
            label3.Text = "";
            label4.Text = "";
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // enter key from password is same as login
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void TxtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow letter and numbers input only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow letter and numbers input only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }
    }  
}
