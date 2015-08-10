using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ThomasJepp.SaintsRow;

namespace HashTester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            string text = InputBox.Text;
            HashVolitionOut.Text = Hashes.HashVolition(text).ToString("X8");
            CrcVolitionOut.Text = Hashes.CrcVolition(text).ToString("X8");
            CrcVolitionDecimal.Text = BitConverter.ToInt32(BitConverter.GetBytes(Hashes.CrcVolition(text)), 0).ToString();
            AudiokineticHashOut.Text = Hashes.AudiokineticHash(text).ToString("X8");
        }
    }
}
