﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class PrintingControls : Form
    {
        public PrintingControls()
        {
            InitializeComponent();
        }

        int totalNumber;//this is for total number of items of the list or array
        int itemPerpage;//this is for no of item per page 
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float currentY = 50;// declare  one variable for height measurement
            Font font = new Font("Times New Roman", 30);
            Brush brush = Brushes.Blue;

            while (totalNumber <= 500) // check the number of items
            {
                //print each item
                e.Graphics.DrawString($"{txtPrint.Text} {totalNumber}", font, brush, 50, currentY);
                currentY += 50; // set a gap between every item
                totalNumber += 1; //increment count by 1
                if (itemPerpage < 20) // check whether  the number of item(per page) is more than 20 or not
                {
                    itemPerpage += 1; // increment itemperpage by 1
                    e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added
                }

                else // if the number of item(per page) is more than 20 then add one page
                {
                    itemPerpage = 0; //initiate itemperpage to 0 .
                    e.HasMorePages = true; //e.HasMorePages raised the PrintPage event once per page .
                    return;//It will call PrintPage event again
                }
            }
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument1;
            pageSetupDialog1.ShowDialog();
        }

        private void BtnPreView_Click(object sender, EventArgs e)
        {
            //here we are printing 50 numbers sequentially by using loop. 
            //For each button click event we have to reset below two variables to 0     
            // because every time  PrintPage event fires automatically. 

            itemPerpage = totalNumber = 0;
            printPreviewDialog1.Document = printDocument1;

            ((ToolStripButton)((ToolStrip)printPreviewDialog1.Controls[1]).Items[0]).Enabled = false;//disable the direct print from printpreview.as when we click that Print button PrintPage event fires again.

            printPreviewDialog1.ShowDialog();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Document = printDocument1;
        }
    }
}
