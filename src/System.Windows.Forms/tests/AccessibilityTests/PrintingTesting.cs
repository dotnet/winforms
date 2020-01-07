// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccessibilityTests
{
    public partial class PrintingTesting : Form
    {
        public PrintingTesting()
        {
            InitializeComponent();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Times New Roman", 12);
            Brush bru = Brushes.Blue;
            for (int i = 1; i <= 5; i++)
            {
                e.Graphics.DrawString(this.txtPrint.Text.ToString(), font, bru, i * 20, i * 20);
            }
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument1;
            this.pageSetupDialog1.ShowDialog();
        }

        private void BtnPreView_Click(object sender, EventArgs e)
        {
            this.printPreviewDialog1.ShowDialog();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (this.printDialog1.ShowDialog() == DialogResult.OK)
            {
                this.printDocument1.Print();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.printPreviewControl1.Document = this.printDocument1;
        }
    }
}
