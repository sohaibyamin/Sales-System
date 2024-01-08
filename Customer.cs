using SalesSystem.Composite;
using SalesSystem.Singleton;
using SalesSystem.State;
using SalesSystem.Strategy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesSystem
{
    public partial class Customer : Form
    {
        SqlConnection conn;
        //Composite Pattern
        Products product;
        CartComposite cartComposite = new CartComposite();
        //State Pattern
        StateContext stateContext = new StateContext();

        DataTable dt;
        
        public Customer()
        {
            InitializeComponent();
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            label1.Text="";
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Item Amount");
            dt.Columns.Add("Price X Amount");
            showTable();
        }
        public void showTable()
        {
            try
            {
                string sql = "Select * from products";
                conn = DBConn.getConn();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds, "Products");
                dataGridView1.DataSource = ds.Tables["products"].DefaultView;
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
        public void addToCart()
        {
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            int colIndex = dataGridView1.CurrentCell.ColumnIndex;
            string name = dataGridView1.Rows[rowIndex].Cells[1].Value.ToString();
            double price = Convert.ToDouble(dataGridView1.Rows[rowIndex].Cells[3].Value);
            int amount = int.Parse(textBox1.Text);

            product = new Products(name, price, amount);
            cartComposite.addProduct(product);

            foreach (DataGridViewRow drv in dataGridView1.Rows)
            {
                if (drv.Cells[colIndex].Selected == true)
                {
                    dt.Rows.Add(drv.Cells[0].Value, drv.Cells[1].Value, drv.Cells[2].Value, textBox1.Text, (amount*price));
                }
            }
            dataGridView2.DataSource = dt;
        }
        public void removeFromCart()
        {
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            int colIndex = dataGridView2.CurrentCell.ColumnIndex;
            string name = dataGridView2.Rows[rowIndex].Cells[1].Value.ToString();
            int amount = Convert.ToInt16(dataGridView2.Rows[rowIndex].Cells[3].Value.ToString());
            double price = Convert.ToDouble(dataGridView2.Rows[rowIndex].Cells[4].Value)/amount;
            

            product = new Products(name, price, amount);
            cartComposite.removeProduct(product);

            foreach (DataGridViewRow drv in dataGridView2.Rows)
            {
                if (drv.Cells[colIndex].Selected == true)
                {
                    dt.Rows.RemoveAt(rowIndex);
                }
            }
            dataGridView2.DataSource = dt;
        }
        public void calculateSum()
        {
            label1.Text = "Total: "+ cartComposite.getPrice();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (stateContext.getState().stateName().Equals("add"))
                    addToCart();
                else if (stateContext.getState().stateName().Equals("remove"))
                    removeFromCart();
            }
            catch (NullReferenceException ex)
            {

                throw;
            }
            button1.Text = "Select";
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.ClearSelection();
            button1.Text = "Remove";
            RemoveState rs = new RemoveState();
            rs.updateCart(stateContext);
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView2.ClearSelection();
            button1.Text = "Add";
            AddState a = new AddState();
            a.updateCart(stateContext);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = "Select amount from discounts where amount = '"+textBox2.Text+"'";
            try
            {
                conn = DBConn.getConn();
                conn.Open();
                int l = Convert.ToInt16(textBox2.Text);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    double discount = sdr.GetDouble(l);
                    if (discount < l)
                    {
                        BillingContext context = new BillingContext(new PercentoffStrategy());
                        conformPurchase(context.executeStrategy(cartComposite.getPrice(), discount));
                    }
                    else
                    {
                        BillingContext context = new BillingContext(new DeductStrategy());
                        conformPurchase(context.executeStrategy(cartComposite.getPrice(), discount));
                    }
                }
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void conformPurchase(double total)
        {
            MessageBox.Show("Purchase Successfully!");
            dataGridView2.RowCount = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
