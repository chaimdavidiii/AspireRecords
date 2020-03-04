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
    public partial class Shipping : Form
    {
        // declarations
        SqlConnection sqlconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand sqlcom;
        SqlDataAdapter sqladapt;
        DataTable dt = new DataTable();
        public Shipping()
        {
            InitializeComponent();
            FillCombo();
            Retrieve();
            SetQuantity();
        }

        public void FillCombo()
        {
            // fill combo box with recordnames from records
            string query = "Select RecordName from Records";
            sqlcom = new SqlCommand(query, sqlconn);
            SqlDataReader myReader;
            try
            {
                if(sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

                myReader = sqlcom.ExecuteReader();
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("RecordName", typeof(String));
                dt1.Load(myReader);

                cbRecordName.ValueMember = "RecordName";
                cbRecordName.DataSource = dt1;

                sqlconn.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void SetQuantity()
        {
            // set quantity on numeric up down based on recordId
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            int vals = GetValue();
            string sql = "Select Quantity from Shipping where RecordId = @recordId";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@recordId", vals);

            int Quantity = Convert.ToInt32(sqlcom.ExecuteScalar());
            nudShipping.Value = Quantity;
            sqlconn.Close();

        }

        public int GetValue()
        {
            // get recordId based on selected value of combo box
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }

            string recordValue = cbRecordName.SelectedValue.ToString();
            string sql = "Select RecordId from Records where RecordName = '" + recordValue + "'";
            sqlcom = new SqlCommand(sql, sqlconn);
         

            int recordID = Convert.ToInt32(sqlcom.ExecuteScalar());
            return recordID;

        }

        public void FillListView(String RecordId, String RecordName, String Quantity)
        {
            // fill list view
            String[] row = { RecordId, RecordName, Quantity };
            lvShipping.Items.Add(new ListViewItem(row));
            
        }

        private void Edit(int recordId, int quantity)
        {
            // method for editing info
            string sql = "Update Shipping Set Quantity = " + quantity + " where RecordId = " + recordId + "";
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void Retrieve()
        {
            // fill list view with info from db
            lvShipping.Items.Clear();
            string sql = "Select Records.RecordId, Records.RecordName, Shipping.Quantity from Records inner join Shipping on Records.RecordId = Shipping.RecordId";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                sqlconn.Open();
                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // exit shipping form
            this.Close();
        }

        private void CbRecordName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // change quantity depending on combo box selected name
            SetQuantity();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // update value
            int vals = GetValue();
            Edit(vals, Convert.ToInt32(nudShipping.Value));
            Retrieve();
        }

        private void NudShipping_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow numbers only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
