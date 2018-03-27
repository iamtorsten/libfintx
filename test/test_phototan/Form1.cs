using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using libfintx;

namespace test_phototan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var PhotoCode = File.ReadAllText($"{Application.StartupPath}\\..\\..\\assets\\matrixcode.txt");

            var mCode = new MatrixCode(PhotoCode);

            pictureBox1.Image = mCode.CodeImage;
        }
    }
}
