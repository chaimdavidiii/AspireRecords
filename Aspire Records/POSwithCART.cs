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
    public partial class POSwithCART : Form
    { 
        // sql and variable declarations
        SqlConnection sqlconn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Aspire2 Student\Documents\dbAspireRecords.mdf;Integrated Security=True;Connect Timeout=30");
                                                    
        SqlCommand sqlcom;
        SqlDataAdapter sqladapt;
        SqlDataAdapter sqladapt2;
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();

        int id = 0;
        int orderNumber = 0;
        int orderId = 0;
        decimal total = 0;
        decimal totaltotal = 0;
        decimal subtractfromtotal = 0;
        string title = "";
        string price = "";
        string stock = "";
        string priceText = "$";

        public POSwithCART()
        {
            InitializeComponent();
            Retrieve();
            orderNumber = GetOrderNumber();
            nudQuantity.Enabled = false;
            btnRemove.Enabled = false;
            
        }

        private void Clear()
        {
            // clear some fields and restore to default
            priceText = "$";
            id = 0;
            nudQuantity.Value = 1;
        }

        private void ClearCartVariables()
        {
            // clear fields
            orderId = 0;
            title = "";
        }

        public void FillListView(String RecordId, String RecordName, String Description, String Stock, String Year, String Price)
        {
            // fill POS list view with parameters
            String[] row = { RecordId, RecordName, Description, Stock, Year, Price };
            lvPOS.Items.Add(new ListViewItem(row));
        }

        public void FillCartListView(String OrderId, String Name, String OrderNumber, String Total, String RecordName)
        {
            // fill cart list view with parameters
            String[] row = { OrderId, Name, OrderNumber, Total, RecordName };
            lvCart.Items.Add(new ListViewItem(row));
        }

        public void AddToCart()
        {
            // select data from database to be added to cart
            lvCart.Items.Clear();
            string sql = "Select OrdersCart.OrderId, OrdersCart.Name, OrdersCart.OrderNumber, OrdersCart.Total, Records.RecordName from OrdersCart inner join Records on OrdersCart.RecordId = Records.RecordId where OrderNumber = " + orderNumber;
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                    sqlconn.Open();

                sqladapt2 = new SqlDataAdapter(sqlcom);
                sqladapt2.Fill(dt2);

                foreach (DataRow row in dt2.Rows)
                {
                    FillCartListView(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString());
                }

                sqlconn.Close();

                dt2.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        public void Add(string name, int ordernumber, decimal total, int recordId, int numberofitems)
        {
            // add data to database
            if (sqlconn.State == ConnectionState.Open)
                sqlconn.Close();

            String sql = "Insert into OrdersCart(Name,OrderNumber,Total,RecordId,Date,NumberOfItems) values (@name,@ordernumber,@total,@recordid,@date,@numberofitems)";
            sqlcom = new SqlCommand(sql, sqlconn);
            sqlcom.Parameters.AddWithValue("@name", name);
            sqlcom.Parameters.AddWithValue("@ordernumber", ordernumber);
            sqlcom.Parameters.AddWithValue("@total", total);
            sqlcom.Parameters.AddWithValue("@recordid", recordId);
            sqlcom.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            sqlcom.Parameters.AddWithValue("@numberofitems", numberofitems);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }
                sqlcom.ExecuteNonQuery();
                sqlconn.Close();
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }


        }

        public void Retrieve()
        {
            // fill list view with info from db
            lvPOS.Items.Clear();

            String sql = "Select Records.RecordId, Records.RecordName, Records.Description, Shipping.Quantity, Records.Year, Records.Price from Records inner join Shipping on Records.RecordId = Shipping.RecordId";
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if(sqlconn.State == ConnectionState.Closed)
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

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // exit application
            if(lvCart.Items.Count != 0)
            {
                MessageBox.Show("Remove Items from Cart before EXITING!", "WARNING");
                return;
            }

            this.Close();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // search using search text box as parameter
            lvPOS.Items.Clear();

            String sql = "Select Records.RecordId, Records.RecordName, Records.Description, Shipping.Quantity, Records.Year, Records.Price from Records inner join Shipping on Records.RecordId = Shipping.RecordId where RecordName like '" + txtSearch.Text + "%" + "'";

            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // button for adding data into cart list view
            if (string.IsNullOrEmpty(txtCustomerName.Text))
            {
                MessageBox.Show("Enter Customer Name!");
                txtCustomerName.Focus();
                return;
            }
            else if (!string.IsNullOrEmpty(txtCustomerName.Text) && string.IsNullOrEmpty(price) || string.IsNullOrEmpty(stock))
            {
                MessageBox.Show("Click on Record to Add!");
                return;
            }
            else
            {
                Add(txtCustomerName.Text,orderNumber,total,id,Convert.ToInt32(nudQuantity.Value));
                AddToCart();
                totaltotal += total;
                lblTotal.Text = totaltotal.ToString();
                UpdateQuantity(Convert.ToInt32(stock),Convert.ToInt32(nudQuantity.Value),id);
                ClearCartVariables();
                Clear();
                nudQuantity.Value = 1;
                btnRemove.Enabled = true;
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            // button to remove data from cart list view
            if (orderId == 0)
            {
                MessageBox.Show("Please click on Row you want to remove from the cart!");
                return;
            }

            lvCart.SelectedItems[0].Remove();

            int numofitems = GetNumberOfItems(orderId);
            int recId = GetRecordId(title);
            int quant = GetQuantity(recId);
            totaltotal -= subtractfromtotal;
            lblTotal.Text = totaltotal.ToString();
            UpdateQuantityRemove(quant, numofitems, recId);
            Retrieve();
            RemoveFromCart(orderId);
        }

        public int GetOrderNumber()
        {
            // get unique order number
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            String sql = "select max(OrderNumber)+1 from OrdersCart";
            sqlcom = new SqlCommand(sql, sqlconn);


            int ordnum = (int)sqlcom.ExecuteScalar();
            sqlconn.Close();
            return ordnum;

        }

        public string GetTitle()
        {
            // get title using id
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            string sql = "Select RecordName from Records where RecordId = " + id;
            sqlcom = new SqlCommand(sql, sqlconn);
            string titolo = sqlcom.ExecuteScalar().ToString();
            return titolo;
        }

        private int GetNumberOfItems(int orderId)
        {
            // get number of items using orderId
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            String sql = "select NumberOfItems from OrdersCart where OrderId = "+orderId;
            sqlcom = new SqlCommand(sql, sqlconn);
            int ordnum = (int)sqlcom.ExecuteScalar();
            sqlconn.Close();
            return ordnum;
        }

        private int GetQuantity(int recId)
        {
            // get quantity using recordId
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            string sql = "Select Quantity from Shipping where RecordId = " + recId;
            sqlcom = new SqlCommand(sql, sqlconn);
            int quantity = (int)sqlcom.ExecuteScalar();
            return quantity;
        }

        private int GetRecordId(string title)
        {
            // get recordId using title/record name
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            string sql = "Select RecordId from Records where RecordName = '" + title + "'";
            sqlcom = new SqlCommand(sql, sqlconn);
            int recId = (int)sqlcom.ExecuteScalar();
            return recId;
        }

        private void LvPOS_Click(object sender, EventArgs e)
        {
            // assign data to variables when clicking on POS listview
            Clear();
            if (!nudQuantity.Enabled)
                nudQuantity.Enabled = true;
            id = Convert.ToInt32(lvPOS.SelectedItems[0].SubItems[0].Text);
            title = lvPOS.SelectedItems[0].SubItems[1].Text;
            stock = lvPOS.SelectedItems[0].SubItems[3].Text;
            price = lvPOS.SelectedItems[0].SubItems[5].Text;
            total = Convert.ToDecimal(price);
            priceText += price;
            txtPrice.Text = priceText;
        }

        private void LvCart_Click(object sender, EventArgs e)
        {
            // assign data to variables when clicking on CART listview
            orderId = 0;
            subtractfromtotal = 0;
            title = "";
            orderId = Convert.ToInt32(lvCart.SelectedItems[0].SubItems[0].Text);
            subtractfromtotal = Convert.ToDecimal(lvCart.SelectedItems[0].SubItems[3].Text);
            title = lvCart.SelectedItems[0].SubItems[4].Text;
        }

        public void UpdateQuantityRemove(int quantity, int noOfItems, int recordId)
        {
            // add back quantity when removed from cart listview
            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                    sqlconn.Open();

                int stockupdate = quantity + noOfItems;
                string sql = "Update Shipping Set Quantity = " + stockupdate + " where RecordId = " + recordId + "";
                sqlcom = new SqlCommand(sql, sqlconn);
                sqlcom.ExecuteNonQuery();
                sqlconn.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void UpdateQuantity(int quantity, int noOfItems, int recordId)
        {
            // update shipping table quantity column based on no of items ordered
            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                    sqlconn.Open();

                int stockupdate = quantity - noOfItems;
                string sql = "Update Shipping Set Quantity = " + stockupdate + " where RecordId = " + recordId + "";
                sqlcom = new SqlCommand(sql, sqlconn);
                sqlcom.ExecuteNonQuery();
                sqlconn.Close();
                Retrieve();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }



        }

        private void NudQuantity_ValueChanged(object sender, EventArgs e)
        {
            // change price when numeric up down is changed
            if (Convert.ToInt32(stock) < nudQuantity.Value)
            {
                MessageBox.Show("Not Enough Stock!", "Stock Error!");
                nudQuantity.Value = Convert.ToInt32(stock);
            }
            else
            {
                int quanquantity = (int)nudQuantity.Value;
                decimal tryprice = Convert.ToDecimal(price);
                total = tryprice * quanquantity;

                //lblSubTotal.Text = "Sub Total: $" + total;
                
                txtPrice.Text = "$" + total;
            }
        }

        private void RemoveFromCart(int orderId)
        {
            // when removed from cart, delete from database using orderId
            String sql = "delete from OrdersCart where OrderId = "+orderId;
            sqlcom = new SqlCommand(sql, sqlconn);

            try
            {
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }
                sqlcom.ExecuteNonQuery();
                sqlconn.Close();
                AddToCart();
                Retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
        
        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow numbers and letters only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void TxtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow numbers and letters only
            if ((e.KeyChar == '\''))
            {
                e.Handled = true;
            }
        }

        private void NudQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow float and decimal
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            // open cashier form to calculate change
            Cashier cash1 = new Cashier();
            cash1.bill = totaltotal;
            cash1.ShowDialog();
            this.Close();
        }
    }
}
