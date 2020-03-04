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
    public partial class RecordsFunctionsForm : Form
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
        public RecordsFunctionsForm()
        {
            InitializeComponent();
        }

        public void SetFieldsToBeEdited(int Id, String recordname, String description, string year, string price)
        {
            // uses parameters taken from previous form to set into textboxes for editing
            this.Id = Id;
            txtRecord.Text = recordname;
            txtDescription.Text = description;
            txtYear.Text = year;
            txtPrice.Text = price;

        }

        private void AddShipping(int RecordId, int Quantity)
        {
            // add info into shipping table with record ID from Added record
            String sql = "Insert into Shipping(RecordId,Quantity) Values (@RecordId,@Quantity)";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@RecordId", RecordId);
            sqlcom.Parameters.AddWithValue("Quantity", Quantity);

            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }

            sqlcom.ExecuteNonQuery();
            sqlconn.Close();
        }

        private int GetRecordId(string recordTitle)
        {
            // get RecordId based on RecordName from Add function
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }

            string sql = "Select RecordId from Records where RecordName = @RecordName";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@RecordName", recordTitle);

            int trylang = (int)sqlcom.ExecuteScalar();

            sqlconn.Close();
            return trylang;
        }

        private Boolean UniqueRecord(string recordName)
        {
            // check if Record already exists in database
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }

            string checkUser = "select count(*) from Records where RecordName = '" + recordName + "'";
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

        private void Add(String recordname, String description, string year, string price)
        {
            // method for adding into db
            String sql = "Insert into Records(RecordName,Description,Year,Price) Values (@recordname,@description,@year,@price)";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@recordname", recordname);
            sqlcom.Parameters.AddWithValue("@description", description);
            
            sqlcom.Parameters.AddWithValue("@year", year);
            sqlcom.Parameters.AddWithValue("@price", price);

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
                int recordID = GetRecordId(recordname);
                AddShipping(recordID, 0);
                Records newadd = new Records();
                newadd.UserUsers = FunctionUsers;
                newadd.Text = "Records - Admin";
                newadd.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void Edit(int Id, string nrecordname, string ndescription, string nyear, string nprice)
        {
            // method for editing info
            string sql = "Update Records Set RecordName = '" + nrecordname + "', Description = '" + ndescription + "', Year = '" + nyear + "', Price = '" + nprice + "' where RecordId = " + Id + "";
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
                    MessageBox.Show("Data Successfully Updated!");
                }
                sqlconn.Close();
                Records bago = new Records();
                bago.UserUsers = functionUsers;
                bago.Text = "Records - Admin";
                bago.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void Search()
        {
            // method for searching info using parameters based on checked checkboxes
            searchparams = "";
            

            if (!chkRecord.Checked && !chkDescription.Checked && !chkYear.Checked && !chkPrice.Checked)
            {
                MessageBox.Show("Please CHECK search parameter and ENTER data into CHECKED field!", "WARNING");
                return;
            }

            try
            {
                if (chkRecord.Checked)
                {
                    if (string.IsNullOrEmpty(txtRecord.Text))
                    {
                        MessageBox.Show("Search field \"RECORD NAME\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "RecordName like '" + txtRecord.Text + "%" + "'";
                    }
                    else
                    {
                        searchparams += " and RecordName like '" + txtRecord.Text + "%" + "'";
                    }

                }

                if (chkDescription.Checked)
                {
                    if (string.IsNullOrEmpty(txtDescription.Text))
                    {
                        MessageBox.Show("Search field \"DESCRIPTION\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Description like '" + txtDescription.Text + "%" + "'";
                    }
                    else
                    {
                        searchparams += " and Description like '" + txtDescription.Text + "%" + "'";
                    }
                }

                if (chkYear.Checked)
                {
                    if (string.IsNullOrEmpty(txtYear.Text))
                    {
                        MessageBox.Show("Search field \"YEAR\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Year like " + txtYear.Text;
                    }
                    else
                    {
                        searchparams += " and Year like " + txtYear.Text;
                    }
                }

                if (chkPrice.Checked)
                {
                    int plus1 = Convert.ToInt32(txtPrice.Text) + 1;
                    if (string.IsNullOrEmpty(txtPrice.Text))
                    {
                        MessageBox.Show("Search field \"PRICE\" must not be EMPTY!", "ERROR!");
                        return;
                    }
                    if (string.IsNullOrEmpty(searchparams))
                    {
                        searchparams += "Price between " + txtPrice.Text + " and " + plus1;
                    }
                    else
                    {
                        searchparams += " and Price between " + txtPrice.Text + " and " + plus1;
                    }
                }

                Records passdata = new Records();
                passdata.Trial += searchparams;
                passdata.UserUsers = FunctionUsers;
                passdata.Text = "Records - Admin";
                passdata.TrialRetrieve();
                passdata.Show();

                this.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR!");
            }

        }

        private void HideCheckBoxes()
        {
            // method to hide checkboxes, checkboxes are only visible in search method
            chkRecord.Visible = false;
            chkDescription.Visible = false;
            chkYear.Visible = false;
            chkPrice.Visible = false;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // opens new object if form is search or edit, otherwise form closes
            if (this.Text == "SEARCH" || this.Text == "EDIT" || this.Text == "ADD")
            {
                Records new1 = new Records();
                new1.UserUsers = FunctionUsers;
                new1.Text = "Records - Admin";
                new1.Show();
                this.Close();
            }
            this.Close();
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            // different method used depending upon form text   
            if (this.Text == "ADD")
            {
                if (UniqueRecord(txtRecord.Text.Trim()) == false)
                {
                    MessageBox.Show("Record already exists!");
                }
                else if (string.IsNullOrEmpty(txtRecord.Text) || string.IsNullOrEmpty(txtDescription.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtYear.Text))
                {
                    MessageBox.Show("Fields CANNOT be EMPTY!");
                }
                else
                {
                    Add(txtRecord.Text, txtDescription.Text, txtYear.Text, txtPrice.Text);
                }
            }
            if (this.Text == "EDIT")
            {
                Edit(Id, txtRecord.Text, txtDescription.Text, txtYear.Text, txtPrice.Text);
            }
            if (this.Text == "SEARCH")
            {
                
                Search();
            }
        }

        private void RecordsFunctionsForm_TextChanged(object sender, EventArgs e)
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
            if (this.Text == "SEARCH")
            {
                btnGo.Text = "Search";
            }
        }

        private void TxtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            // numbers input only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void TxtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // numbers input only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // float/decimal allowed
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void TxtRecord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void TxtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }
    }
}
