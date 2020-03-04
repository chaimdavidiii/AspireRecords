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
    public partial class Records : Form
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

        // string declaration for setting sql string used for search method
        public static string trial = "Select Records.RecordId, Records.RecordName, Records.Description, Shipping.Quantity, Records.Year, Records.Price from Records inner join Shipping on Records.RecordId = Shipping.RecordId where ";

        // property for setting trial sql string
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

        // field declarations used in double-click method for edit and delete methods
        int Id = 0;
        string mtdStock = "";
        string mtdYear = "";
        string mtdPrice = "";
        string mtdRecord = "";
        string mtdDesc = "";

        public Records()
        {
            // form initializes with all db records on list view
            InitializeComponent();
            Retrieve();
            Clear();
        }

        public void Clear()
        {
            // clear id and mtd fields
            Id = 0;
            mtdStock = "";
            mtdYear = "";
            mtdPrice = "";
            mtdRecord = "";
            mtdDesc = "";
        }

        public void FillListView(String RecordId, String RecordName, String Description, String Stock, String Year, String Price)
        {
            // fill list view with parameters
            String[] row = { RecordId, RecordName, Description, Stock, Year, Price };
            lvRecords.Items.Add(new ListViewItem(row));
        }

        public void TrialRetrieve()
        {
            // fill db using trial string for search method
            lvRecords.Items.Clear();


            sqlcom = new SqlCommand(trial, sqlconn);

            try
            {
                sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void Retrieve()
        {
            // fill list view with info from db
            lvRecords.Items.Clear();

            String sql = "Select Records.RecordId, Records.RecordName, Records.Description, Shipping.Quantity, Records.Year, Records.Price from Records inner join Shipping on Records.RecordId = Shipping.RecordId";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void Delete(int DeleteId)
        {
            // delete sql method
            String sql = "Delete from Records where RecordId = " + DeleteId + "";
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
                string title = "WARNING!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    if (sqlcom.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Record Successfully deleted!");
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
            // call clear and retrieve methods to show all db info in list view
            Clear();
            Retrieve();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // exit functions form

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
            // call form and change form text to ADD
            RecordsFunctionsForm adminadd = new RecordsFunctionsForm();
            adminadd.Text = "ADD";
            adminadd.FunctionUsers = UserUsers;
            adminadd.ShowDialog();
            this.Close();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            //check if double click fields are empty and allows delete if double click fields are not empty
            if (string.IsNullOrEmpty(mtdRecord) || string.IsNullOrEmpty(mtdDesc) || string.IsNullOrEmpty(mtdYear) || Id == 0)
            {
                MessageBox.Show("Select Row to be deleted!");
            }
            else
            {
                //int Id = Convert.ToInt32(lvRecords.SelectedItems[0].SubItems[0].Text);

                Delete(Id);
            }
        }

        private void LvRecords_DoubleClick(object sender, EventArgs e)
        {
            // puts info into id and mtd fields to allow edit and delete
            Clear();
            Id = Convert.ToInt32(lvRecords.SelectedItems[0].SubItems[0].Text);
            mtdRecord = lvRecords.SelectedItems[0].SubItems[1].Text;
            mtdDesc = lvRecords.SelectedItems[0].SubItems[2].Text;
            mtdStock = lvRecords.SelectedItems[0].SubItems[3].Text;
            mtdYear = lvRecords.SelectedItems[0].SubItems[4].Text;
            mtdPrice = lvRecords.SelectedItems[0].SubItems[5].Text;
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // checks if mtd fields are empty and opens edit user function form when fields are not empty.
            if (string.IsNullOrEmpty(mtdRecord) || string.IsNullOrEmpty(mtdDesc) || string.IsNullOrEmpty(mtdYear) || Id == 0)
            {
                MessageBox.Show("Select Row to be Edited!");
            }
            else
            {
                RecordsFunctionsForm admineditrecords1 = new RecordsFunctionsForm();
                admineditrecords1.Text = "EDIT";
                admineditrecords1.SetFieldsToBeEdited(Id, mtdRecord, mtdDesc, mtdYear, mtdPrice);
                admineditrecords1.FunctionUsers = UserUsers;
                admineditrecords1.ShowDialog();
                this.Close();
                Clear();

            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            //opens search function form
            RecordsFunctionsForm adminrecordssearch1 = new RecordsFunctionsForm();
            trial = "Select Records.RecordId, Records.RecordName, Records.Description, Shipping.Quantity, Records.Year, Records.Price from Records inner join Shipping on Records.RecordId = Shipping.RecordId where ";
            adminrecordssearch1.Text = "SEARCH";
            adminrecordssearch1.FunctionUsers = userUsers;
            adminrecordssearch1.ShowDialog();
            this.Close();
        }

        private void LvRecords_Click(object sender, EventArgs e)
        {
            // put data into variables
            Clear();
            Id = Convert.ToInt32(lvRecords.SelectedItems[0].SubItems[0].Text);
            mtdRecord = lvRecords.SelectedItems[0].SubItems[1].Text;
            mtdDesc = lvRecords.SelectedItems[0].SubItems[2].Text;
            mtdStock = lvRecords.SelectedItems[0].SubItems[3].Text;
            mtdYear = lvRecords.SelectedItems[0].SubItems[4].Text;
            mtdPrice = lvRecords.SelectedItems[0].SubItems[5].Text;
        }
    }
}
