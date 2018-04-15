using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HBCI = libfintx;

namespace test_pushTAN
{
    public partial class TAN : Form
    {
        public TAN()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(HBCI.Transaction.TAN(Program.connectionDetails, textBox1.Text));
        }
    }
}
