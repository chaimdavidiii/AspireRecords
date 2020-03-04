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
using Microsoft.Office.Interop.Excel;

namespace Aspire_Records
{
    public partial class Transactions : Form
    {
        // property used for checking admin or user role
        public string transactionUsers;
        public string TransactionUsers
        {
            get
            {
                return transactionUsers;
            }
            set
            {
                transactionUsers = value;
            }
        }

        // initialize search parameters
        string searchRetrieveParam = "select OrdersCart.OrderId, OrdersCart.Name, OrdersCart.OrderNumber, OrdersCart.Total, Records.RecordName, OrdersCart.Date, OrdersCart.NumberOfItems from OrdersCart inner join Records on OrdersCart.RecordId = Records.RecordId where ";
        string searchParam = "";

        // sql declarations
        SqlConnection sqlconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand sqlcom;
        SqlDataAdapter sqladapt;
        System.Data.DataTable dt = new System.Data.DataTable();
        public Transactions()
        {
            InitializeComponent();
            Retrieve();
        }

        private void Clear()
        {
            // clear textboxes
            txtCustomerName.Text = txtDate.Text = txtTitle.Text = txtTotal.Text = "";
        }

        public void isAdmin(string userName)
        {
            if (sqlconn.State == ConnectionState.Closed)
                sqlconn.Open();


            string checkusername = "select Admin from Users where Username='" + userName + "'";
            SqlCommand com = new SqlCommand(checkusername, sqlconn);

            // check if username is admin or not and call function for corresponding admin or user

            int admin = Convert.ToInt32(com.ExecuteScalar());

            if (admin == 1)
            {
                btnReport.Enabled = true;
            }
            else
                btnReport.Enabled = false;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // close transactions form
            this.Close();
        }

        public void FillListView(String OrderId, String CustomerName, String OrderNumber, String Total, String Title, String Date, String Items)
        {
            // fill list view with parameters
            String[] row = { OrderId, CustomerName, OrderNumber, Total, Title, Date, Items };
            lvOrders.Items.Add(new ListViewItem(row));
        }

        public void Retrieve()
        {
            // fill list view with info from db
            lvOrders.Items.Clear();

            String sql = "select OrdersCart.OrderId, OrdersCart.Name, OrdersCart.OrderNumber, OrdersCart.Total, Records.RecordName, OrdersCart.Date, OrdersCart.NumberOfItems from OrdersCart inner join Records on OrdersCart.RecordId = Records.RecordId";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                    sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void RetrieveSearch(string searchString)
        {
            // fill list view with search parameters
            lvOrders.Items.Clear();
            sqlcom = new SqlCommand(searchString, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                    sqlconn.Open();

                sqladapt = new SqlDataAdapter(sqlcom);
                sqladapt.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    FillListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString());
                }

                sqlconn.Close();

                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void LvOrders_Click(object sender, EventArgs e)
        {
            // populate textboxes with data from listview
            Clear();

            txtCustomerName.Text = lvOrders.SelectedItems[0].SubItems[1].Text;
            txtDate.Text = lvOrders.SelectedItems[0].SubItems[5].Text;
            txtTotal.Text = lvOrders.SelectedItems[0].SubItems[3].Text;
            txtTitle.Text = lvOrders.SelectedItems[0].SubItems[4].Text;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // search function
            if (!chbSearchName.Checked && !chbSearchDate.Checked)
            {
                MessageBox.Show("Please check NAME or DATE checkbox.");
                return;
            }

            try
            {
                if (chbSearchName.Checked)
                {
                    if (string.IsNullOrEmpty(txtSearch.Text))
                    {
                        MessageBox.Show("Search field \"CUSTOMERNAME\" must not be empty!");
                        txtSearch.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(searchParam))
                    {
                        searchParam += "CustomerName like '" + txtSearch.Text + "%'";
                    }
                    else
                    {
                        searchParam += " and CustomerName like '" + txtSearch.Text + "%'";
                    }
                }

                if (chbSearchDate.Checked)
                {
                    if (string.IsNullOrEmpty(searchParam))
                    {
                        searchParam += "Date between '" + dtpFrom.Value.ToString("yyyy-MM-dd") + " 00:00:00' and '" + dtpTo.Value.ToString("yyyy-MM-dd") + "12:00:00'";
                    }
                    else
                    {
                        searchParam += " and Date between '" + dtpFrom.Value.ToString("yyyy-MM-dd") + " 00:00:00' and '" + dtpTo.Value.ToString("yyyy-MM-dd") + "12:00:00'";
                    }
                }


                searchRetrieveParam += searchParam;
                RetrieveSearch(searchRetrieveParam);
                searchParam = "";
                searchRetrieveParam = "select OrdersCart.OrderId, OrdersCart.Name, OrdersCart.OrderNumber, OrdersCart.Total, Records.RecordName, OrdersCart.Date, OrdersCart.NumberOfItems from OrdersCart inner join Records on OrdersCart.RecordId = Records.RecordId where ";


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            // export data from listview to excel sheet
            if (lvOrders.Items.Count == 0)
            {
                MessageBox.Show("No Report to Generate!");
                return;
            }

            Microsoft.Office.Interop.Excel.Application xlss = new Microsoft.Office.Interop.Excel.Application();
            xlss.Visible = true;
            Workbook wb = xlss.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)xlss.ActiveSheet;
            ws.Cells[1, 1] = "REPORT";
            ws.Cells[1, 2] = DateTime.Now.ToString();
            ws.Cells[3, 1] = "Order ID";
            ws.Cells[3, 2] = "Customer Name";
            ws.Cells[3, 3] = "Order No.";
            ws.Cells[3, 4] = "Total";
            ws.Cells[3, 5] = "Record Title";
            ws.Cells[3, 6] = "Date";
            ws.Cells[3, 7] = "No. Of Items";
            int i = 4;
            foreach (ListViewItem item in lvOrders.Items)
            {
                ws.Cells[i, 1] = item.SubItems[0].Text;
                ws.Cells[i, 2] = item.SubItems[1].Text;
                ws.Cells[i, 3] = item.SubItems[2].Text;
                ws.Cells[i, 4] = item.SubItems[3].Text;
                ws.Cells[i, 5] = item.SubItems[4].Text;
                ws.Cells[i, 6] = item.SubItems[5].Text;
                ws.Cells[i, 7] = item.SubItems[6].Text;
                i++;
            }
            ws.Columns.AutoFit();
            ws.Columns.HorizontalAlignment = HorizontalAlignment.Center;
            ws.Rows.HorizontalAlignment = HorizontalAlignment.Center;

        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow numbers and letters only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }
    }
}
