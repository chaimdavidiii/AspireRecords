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

    public partial class Users : Form
    {
        // property used for checking admin or user role
        private string userUsers;
        public string UserUsers
        {
            get
            {
                return userUsers;
            }
            set
            {
                userUsers = value;
            }
        }

        // string to set for search with parameters
        public static string trial = "select * from Users where ";
        public string Trial
        {
            get
            {
                return trial;
            }

            set
            {
                trial = value;
            }

        }
        // sql declarations
        SqlConnection sqlconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand sqlcom;
        SqlDataAdapter sqladapt;
        DataTable dt = new DataTable();

        // field declarations used in double-click method needed for edit and delete methods
        int Id = 0;
        string mtdUser = "";
        string mtdPass = "";
        string mtdAdmin = "";
        string mtdName = "";
        public Users()
        {
            // form initializes with all db records on list view
            InitializeComponent();
            Retrieve();
            Clear();
        }

        public void TrialRetrieve()
        {
            //retrieve with search parameters from string trial
            lvUsers.Items.Clear();


            sqlcom = new SqlCommand(trial, sqlconn);

            try
            {
                sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void FillListView(String Id, String username, String password, String admin, String name)
        {
            //fill listview with db info 
            String[] row = { Id, username, password, admin, name };
            lvUsers.Items.Add(new ListViewItem(row));
        }

        public void Clear()
        {
            //clear id and mtd fields
            Id = 0;
            mtdUser = "";
            mtdPass = "";
            mtdAdmin = "";
            mtdName = "";
        }

        public void Retrieve()
        {
            //fill list view with data from db
            lvUsers.Items.Clear();

            String sql = "Select * from Users";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void Delete(int Id)
        {
            //delete using Id parameter
            String sql = "Delete from Users where UserId = " + Id + "";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.DeleteCommand = sqlconn.CreateCommand();
                sqladapt.DeleteCommand.CommandText = sql;

                string message = "Are you sure you want to Delete?";
                string title = "ATTENTION!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    if (sqlcom.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Data Successfully deleted!");
                    }
                }
                else
                {
                    Clear();
                    return;
                }

                sqlconn.Close();
                Clear();
                Retrieve();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            // call clear and retrieve methods to show all info from db
            Clear();
            Retrieve();

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            //exit functions form

            this.Hide();
            Home admin1 = new Home();
            admin1.HomeUser = UserUsers;
            admin1.userSession(UserUsers);
            admin1.adminRole();
            admin1.Text = "Home - Admin";
            admin1.Show();


        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // open userfunctions form with ADD function
            UsersFunctionsForm adminuser1 = new UsersFunctionsForm();
            adminuser1.Text = "ADD";
            adminuser1.FunctionUsers = UserUsers;
            adminuser1.ShowDialog();
            this.Close();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            //check if double click fields are empty and allows delete if double click fields are not empty
            if (string.IsNullOrEmpty(mtdUser) || string.IsNullOrEmpty(mtdPass) || string.IsNullOrEmpty(mtdAdmin) || string.IsNullOrEmpty(mtdName))
            {
                MessageBox.Show("Select Row to be deleted!");
            }
            else
            {
                //int Id = Convert.ToInt32(lvUsers.SelectedItems[0].SubItems[0].Text);

                Delete(Id);
            }
        }

        private void LvUsers_DoubleClick(object sender, EventArgs e)
        {
            // puts info into id and mtd fields to allow edit and delete
            Clear();
            Id = Convert.ToInt32(lvUsers.SelectedItems[0].SubItems[0].Text);
            mtdUser = lvUsers.SelectedItems[0].SubItems[1].Text;
            mtdPass = lvUsers.SelectedItems[0].SubItems[2].Text;
            mtdAdmin = lvUsers.SelectedItems[0].SubItems[3].Text;
            mtdName = lvUsers.SelectedItems[0].SubItems[4].Text;
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // checks if mtd fields are empty and opens edit user function form when fields are not empty.
            if (string.IsNullOrEmpty(mtdUser) || string.IsNullOrEmpty(mtdPass) || string.IsNullOrEmpty(mtdAdmin) || string.IsNullOrEmpty(mtdName))
            {
                MessageBox.Show("Select Row to be edited.");
            }
            else
            {

                UsersFunctionsForm admin1edit = new UsersFunctionsForm();
                admin1edit.Text = "EDIT";
                admin1edit.FunctionUsers = UserUsers;
                admin1edit.SetFieldsToBeEdited(Id, mtdUser, mtdPass, mtdAdmin, mtdName);
                admin1edit.HideInstructions();
                admin1edit.ShowDialog();
                this.Close();
                Clear();


            }

        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            //opens search function form
            UsersFunctionsForm adminsearching = new UsersFunctionsForm();
            trial = "select * from Users where ";
            adminsearching.FunctionUsers = UserUsers;
            adminsearching.Text = "SEARCH";
            adminsearching.HideInstructions();
            adminsearching.ShowDialog();
            this.Close();
        }

        private void LvUsers_Click(object sender, EventArgs e)
        {
            // puts info into id and mtd fields to allow edit and delete
            Clear();
            Id = Convert.ToInt32(lvUsers.SelectedItems[0].SubItems[0].Text);
            mtdUser = lvUsers.SelectedItems[0].SubItems[1].Text;
            mtdPass = lvUsers.SelectedItems[0].SubItems[2].Text;
            mtdAdmin = lvUsers.SelectedItems[0].SubItems[3].Text;
            mtdName = lvUsers.SelectedItems[0].SubItems[4].Text;
        }
    }
}
