using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class RemainingControls : Form
    {
        public RemainingControls()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = domainUpDown1;
            propertyGrid2.SelectedObject = trackBar1;

        }
    }
}
