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
    public partial class Home : Form
    {
        // property used for checking admin or user role
        private string homeUser;
        public string HomeUser
        {
            get
            {
                return homeUser;
            }
            set
            {
                homeUser = value;
            }
           
        }

        public Home()
        {
            InitializeComponent();
        }

        public void userSession(string username)
        {
            // put session like label to home form with username
            label1.Text = "Welcome, " + username.ToUpper() + "!";
            
        }

        public void adminRole()
        {
            // hides pos function which is for user only
            this.pOSToolStripMenuItem.Visible = false;
        }

        public void userRole()
        {
            // hides users function which is for admin only
            this.usersToolStripMenuItem1.Visible = false;
            this.recordsToolStripMenuItem.Visible = false;
            this.shippingToolStripMenuItem.Visible = false;
        }

        private void LogoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // logout then show login while closing old session
            Login fromlogout = new Login();
            fromlogout.Show();
            this.Close();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to EXIT?", "ATTENTION!",MessageBoxButtons.YesNo);
            
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void UsersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // show users form to admin

            //AdminUsers adminusers1 = new AdminUsers();
            //adminusers1.Show();


            Users admin1 = new Users();
            this.Hide();
            admin1.UserUsers = HomeUser;
            admin1.Show();

           
        }

        private void POSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show pos form to user
            POSwithCART userpos1 = new POSwithCART();
            userpos1.Text = "POS - User";
            userpos1.ShowDialog();
        }

        private void RecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show records form to admin
            Records recs = new Records();
            recs.Text = "Records - Admin";
            this.Hide();
            recs.UserUsers = HomeUser;
            recs.Show();
            
        }

        private void TransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show orders page to both users
            Transactions trans = new Transactions();
            trans.isAdmin(HomeUser);
            trans.Text = "Transactions - Orders";
            trans.TransactionUsers = HomeUser;
            trans.ShowDialog();
        }

        private void ShippingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show shipping form to admin
            Shipping ship = new Shipping();
            ship.Text = "Shipping - Admin";
            ship.ShowDialog();
        }
    }
}
