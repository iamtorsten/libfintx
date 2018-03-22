using System.Windows.Forms;

namespace test_Flicker
{
    public partial class Flicker : Form
    {
        public Flicker()
        {
            InitializeComponent();

            Program.pictureBox = this.pictureBox1; 
        }
    }
}
