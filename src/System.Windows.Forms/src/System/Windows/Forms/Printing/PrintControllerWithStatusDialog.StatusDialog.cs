// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms
{
    public partial class PrintControllerWithStatusDialog
    {
        private class StatusDialog : Form
        {
            internal Label label1;
            private Button button1;
            private TableLayoutPanel tableLayoutPanel1;
            private readonly BackgroundThread backgroundThread;

            internal StatusDialog(BackgroundThread backgroundThread, string dialogTitle)
            {
                InitializeComponent();
                this.backgroundThread = backgroundThread;
                Text = dialogTitle;
                MinimumSize = Size;
            }

            /// <summary>
            ///  Tells whether the current resources for this dll have been
            ///  localized for a RTL language.
            /// </summary>
            private static bool IsRTLResources
            {
                get
                {
                    return SR.RTL != "RTL_False";
                }
            }

            private void InitializeComponent()
            {
                if (IsRTLResources)
                {
                    RightToLeft = RightToLeft.Yes;
                }

                tableLayoutPanel1 = new TableLayoutPanel();
                label1 = new Label();
                button1 = new Button();

                label1.AutoSize = true;
                label1.Location = new Point(8, 16);
                label1.TextAlign = ContentAlignment.MiddleCenter;
                label1.Size = new Size(240, 64);
                label1.TabIndex = 1;
                label1.Anchor = AnchorStyles.None;

                button1.AutoSize = true;
                button1.Size = new Size(75, 23);
                button1.TabIndex = 0;
                button1.Text = SR.PrintControllerWithStatusDialog_Cancel;
                button1.Location = new Point(88, 88);
                button1.Anchor = AnchorStyles.None;
                button1.Click += new EventHandler(button1_Click);

                tableLayoutPanel1.AutoSize = true;
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
                tableLayoutPanel1.Location = new Point(0, 0);
                tableLayoutPanel1.RowCount = 2;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.TabIndex = 0;
                tableLayoutPanel1.Controls.Add(label1, 0, 0);
                tableLayoutPanel1.Controls.Add(button1, 0, 1);

                AutoScaleDimensions = new Size(6, 13);
                AutoScaleMode = AutoScaleMode.Font;
                MaximizeBox = false;
                ControlBox = false;
                MinimizeBox = false;
                Size clientSize = new Size(256, 122);
                if (DpiHelper.IsScalingRequired)
                {
                    ClientSize = DpiHelper.LogicalToDeviceUnits(clientSize);
                }
                else
                {
                    ClientSize = clientSize;
                }

                CancelButton = button1;
                SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                Controls.Add(tableLayoutPanel1);
            }

            private void button1_Click(object sender, EventArgs e)
            {
                button1.Enabled = false;
                label1.Text = SR.PrintControllerWithStatusDialog_Canceling;
                backgroundThread.canceled = true;
            }
        }
    }
}
