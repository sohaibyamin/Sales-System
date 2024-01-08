using SalesSystem.Singleton;
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
    public partial class Admin : Form
    {
        //Singleton Pattern
        SqlConnection conn;
        public Admin()
        {
            InitializeComponent();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
