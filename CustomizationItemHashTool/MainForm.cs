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

namespace CustomizationItemHashTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ItemName_TextChanged(object sender, EventArgs e)
        {
            UpdateHash();
        }

        private void MaleMeshFilename_TextChanged(object sender, EventArgs e)
        {
            UpdateHash();
        }

        private void VariantID_ValueChanged(object sender, EventArgs e)
        {
            UpdateHash();
        }

        private void UpdateHash()
        {
            string itemName = ItemName.Text;
            string maleMeshFilename = MaleMeshFilename.Text;
            uint variantId = (uint)VariantID.Value;

            int hash = Hashes.CustomizationItemCrc(itemName, maleMeshFilename, variantId);

            MaleStr2PCName.Text = String.Format("custmesh_{0}.str2_pc", hash);
            FemaleStr2PCName.Text = String.Format("custmesh_{0}f.str2_pc", hash);
        }
    }
}
