using SalesSystem.Factory;
using SalesSystem.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Proxy Pattern + Factory Pattern
            IAuthProxy authp = new AuthProxy();
            ViewAbstract va;
            if (authp.authenticate(textBox1.Text, textBox2.Text))
            {
                va = new AdminView(this);
            }
            else
            {
                va = new CustomerView(this);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
