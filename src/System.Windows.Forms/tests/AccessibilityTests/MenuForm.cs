using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Menu_Toolbars_controls stripControls = new Menu_Toolbars_controls();
            stripControls.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ToolStripContainer toolStripContainer = new ToolStripContainer();
            toolStripContainer.Show();
        }
    }
}
