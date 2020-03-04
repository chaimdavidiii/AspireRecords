using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aspire_Records
{
    public partial class Cashier : Form
    {
        // variable declaration
        public decimal bill = 0;
        public Cashier()
        {
            InitializeComponent();
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {

            // calculate change based on bill and cash
            
            if (string.IsNullOrEmpty(txtCash.Text))
            {
                MessageBox.Show("Please enter CASH");
                txtCash.Focus();
                return;
            }
            
            if (Convert.ToDecimal(txtCash.Text) < bill)
            {
                MessageBox.Show("Insufficient CASH!");
                txtCash.Focus();
                return;
            }
            

            decimal change = 0;
            change = Convert.ToDecimal(txtCash.Text) - bill;
            MessageBox.Show("Your change is $" + change);
            MessageBox.Show("Thank you for your patronage!");
            this.Close();
        }

        private void TxtCash_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
