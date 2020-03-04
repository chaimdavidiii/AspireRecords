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

namespace Aspire_Records
{
    public partial class UsersFunctionsForm : Form
    {
        // property used for checking admin or user role
        public string functionUsers;
        public string FunctionUsers
        {
            get
            {
                return functionUsers;
            }
            set
            {
                functionUsers = value;
            }
        }

        // sql declarations
        string searchparams = "";
        SqlConnection sqlconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand sqlcom;
        SqlDataAdapter sqladapt;
        DataTable dt = new DataTable();
        int Id = 0;
        public UsersFunctionsForm()
        {
            InitializeComponent();
        }

        public void HideInstructions()
        {
            // hide instruction label for adding new user/admin
            lblInstructions.Visible = false;
        }

        public void SetFieldsToBeEdited(int Id, string username, string password, string admin, string name)
        {
            // uses parameters taken from previous form to set into textboxes for editing
            this.Id = Id;
            txtUsername.Text = username;
            txtPassword.Text = password;
            txtAdmin.Text = admin;
            txtName.Text = name;
        }

        private void Add(String username, String password, String admin, String name)
        {
            // method for adding into db
            String sql = "Insert into Users(Username,Password,Admin,Name) Values (@username,@password,@admin,@name)";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@username", username);
            sqlcom.Parameters.AddWithValue("@password", password);
            sqlcom.Parameters.AddWithValue("@admin", admin);
            sqlcom.Parameters.AddWithValue("@name", name);

            try
            {

                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }


                if (sqlcom.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data Successfully Added!");
                }
                sqlconn.Close();

                
                Users newadd = new Users();
                newadd.UserUsers = FunctionUsers;
                newadd.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void Edit(int id, String nusername, String npassword, String nadmin, String nname)
        {
            // method for editing info
            string sql = "Update Users Set Username = '" + nusername + "', Password = '" + npassword + "', Admin = '" + nadmin + "', Name = '" + nname + "' where UserId = " + id + "";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.UpdateCommand = sqlconn.CreateCommand();
                sqladapt.UpdateCommand.CommandText = sql;

                if (sqladapt.UpdateCommand.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data Successfully Updated!","Info",MessageBoxButtons.OK);
                }
                sqlconn.Close();
                

                Users newadd = new Users();
                newadd.UserUsers = FunctionUsers;
                newadd.Show();
                this.Close();

                
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Search()
        {
            // method for searching info using parameters based on checked checkboxes
            searchparams = "";

            if (!chkUsername.Checked && !chkPassword.Checked && !chkAdmin.Checked && !chkName.Checked)
            {
                MessageBox.Show("Please CHECK search parameter and ENTER data into CHECKED field!", "WARNING");
                return;
            }

            try
            {
                if (chkUsername.Checked)
                {
                    if (string.IsNullOrEmpty(txtUsername.Text))
                    {
                        MessageBox.Show("Search field \"USERNAME\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Username like '" + txtUsername.Text + "%" + "'";
                    }
                    else
                    {
                        searchparams += " and Username like '" + txtUsername.Text + "%" + "'";
                    }
                }

                if (chkPassword.Checked)
                {
                    if (string.IsNullOrEmpty(txtPassword.Text))
                    {
                        MessageBox.Show("Search field \"PASSWORD\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Password like '" + txtPassword.Text + "%" + "'";
                    }
                    else
                    {
                        searchparams += " and Password like '" + txtPassword.Text + "%" + "'";
                    }
                }

                if (chkAdmin.Checked)
                {
                    if (string.IsNullOrEmpty(txtAdmin.Text))
                    {
                        MessageBox.Show("Search field \"ADMIN\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Admin like " + txtPassword.Text;
                    }
                    else
                    {
                        searchparams += " and Admin like " + txtPassword.Text;
                    }
                }

                if (chkName.Checked)
                {
                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        MessageBox.Show("Search field \"NAME\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Name like '" + txtName.Text + "%" + "'";
                    }
                    else
                    {
                        searchparams += " and Name like '" + txtName.Text + "%" + "'";
                    }
                }

                Users passdatas = new Users();
                passdatas.Trial += searchparams;
                passdatas.UserUsers = functionUsers;
                passdatas.TrialRetrieve();
                passdatas.Show();

                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error!");
            }

        }

        private void HideCheckBoxes()
        {
            // method to hide checkboxes, checkboxes are only visible in search method
            chkUsername.Visible = false;
            chkPassword.Visible = false;
            chkAdmin.Visible = false;
            chkName.Visible = false;
        }

        private Boolean UniqueUsername(string userName)
        {
            // check if username already exists in database
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }

            string checkUser = "select count(*) from Users where Username = '" + userName + "'";
            sqlcom = new SqlCommand(checkUser, sqlconn);
            int temp = Convert.ToInt32(sqlcom.ExecuteScalar().ToString());

            if (temp == 1)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // opens new object if form is search or edit, otherwise form closes
            if (this.Text == "SEARCH" || this.Text == "EDIT" || this.Text == "ADD")
            {
                Users ne31 = new Users();
                ne31.UserUsers = functionUsers;
                ne31.Show();
                this.Close();
            }

            this.Close();
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            // different method used depending upon form text   
            if(this.Text == "ADD")
            {
                if (UniqueUsername(txtUsername.Text.Trim()) == false)
                {
                    MessageBox.Show("Username already exists!");
                }
                else if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtAdmin.Text) || string.IsNullOrEmpty(txtName.Text))
                {
                    MessageBox.Show("Fields CANNOT be EMPTY!");
                }
                else
                {
                    Add(txtUsername.Text,txtPassword.Text,txtAdmin.Text,txtName.Text);
                }
            }

            if(this.Text == "EDIT")
            {
                Edit(Id,txtUsername.Text,txtPassword.Text,txtAdmin.Text,txtName.Text);
                
            }

            if(this.Text == "SEARCH")
            {
                Search();
            }
        }

        private void UsersFunctionsForm_TextChanged(object sender, EventArgs e)
        {
            // depending upon form text, name of button changes and checkboxes are hidden or shown
            if (this.Text == "ADD")
            {
                HideCheckBoxes();
                btnGo.Text = "Add";
            }
            if (this.Text == "EDIT")
            {
                HideCheckBoxes();
                btnGo.Text = "Update";
            }
            if(this.Text == "SEARCH")
            {
                btnGo.Text = "Search";
            }
        }

        private void TxtAdmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // only numbers are accepted
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
           
        }

        private void TxtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            // numbers and letters input only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // numbers and letters input only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void TxtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // numbers and letters input only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }
    }
}
