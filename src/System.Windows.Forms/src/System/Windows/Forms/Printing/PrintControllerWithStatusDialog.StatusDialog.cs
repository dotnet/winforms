// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
    private class StatusDialog : Form
    {
        internal Label _label1;
        private Button _button1;
        private TableLayoutPanel? _tableLayoutPanel1;
        private readonly BackgroundThread _backgroundThread;

        internal StatusDialog(BackgroundThread backgroundThread, string dialogTitle)
        {
            InitializeComponent();
            _backgroundThread = backgroundThread;
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

        [MemberNotNull(nameof(_label1))]
        [MemberNotNull(nameof(_button1))]
        private void InitializeComponent()
        {
            if (IsRTLResources)
            {
                RightToLeft = RightToLeft.Yes;
            }

            _tableLayoutPanel1 = new TableLayoutPanel();
            _label1 = new Label();
            _button1 = new Button();

            _label1.AutoSize = true;
            _label1.Location = new Point(8, 16);
            _label1.TextAlign = ContentAlignment.MiddleCenter;
            _label1.Size = new Size(240, 64);
            _label1.TabIndex = 1;
            _label1.Anchor = AnchorStyles.None;

            _button1.AutoSize = true;
            _button1.Size = new Size(75, 23);
            _button1.TabIndex = 0;
            _button1.Text = SR.PrintControllerWithStatusDialog_Cancel;
            _button1.Location = new Point(88, 88);
            _button1.Anchor = AnchorStyles.None;
            _button1.Click += new EventHandler(button1_Click);

            _tableLayoutPanel1.AutoSize = true;
            _tableLayoutPanel1.ColumnCount = 1;
            _tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            _tableLayoutPanel1.Location = new Point(0, 0);
            _tableLayoutPanel1.RowCount = 2;
            _tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tableLayoutPanel1.TabIndex = 0;
            _tableLayoutPanel1.Controls.Add(_label1, 0, 0);
            _tableLayoutPanel1.Controls.Add(_button1, 0, 1);

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

            CancelButton = _button1;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Controls.Add(_tableLayoutPanel1);
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            _button1.Enabled = false;
            _label1.Text = SR.PrintControllerWithStatusDialog_Canceling;
            _backgroundThread._canceled = true;
        }
    }
}
