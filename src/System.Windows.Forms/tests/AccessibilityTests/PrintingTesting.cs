// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            using Font font = new("Times New Roman", 12);
            for (int i = 1; i <= 5; i++)
            {
                e.Graphics.DrawString(txtPrint.Text.ToString(), font, Brushes.Blue, i * 20, i * 20);
            }
        }

        // Printer settings dialog belongs to ComDlg32 - PageSetupDlgW
        private void BtnSetting_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument1;
            pageSetupDialog1.ShowDialog();
        }

        private void BtnPreView_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
        }

        // Print dialog belongs to ComDlg32 - PrintDlgEx
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Document = this.printDocument1;
        }
    }
}
