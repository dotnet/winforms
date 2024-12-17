// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class PrintControllerWithStatusDialog
{
    private partial class StatusDialog : Form
    {
        internal FocusableLabel _messageLabel;
        private Button _cancelButton;
        private TableLayoutPanel? _tableLayoutPanel;
        private readonly BackgroundThread _backgroundThread;

        internal StatusDialog(BackgroundThread backgroundThread, string dialogTitle)
        {
            InitializeComponent();
            _backgroundThread = backgroundThread;
            Text = dialogTitle;
            MinimumSize = Size;
        }

        [MemberNotNull(nameof(_messageLabel))]
        [MemberNotNull(nameof(_cancelButton))]
        private void InitializeComponent()
        {
            if (SR.RTL != "RTL_False")
            {
                // Resources have been localized for an RTL language.
                RightToLeft = RightToLeft.Yes;
            }

            _messageLabel = new FocusableLabel()
            {
                AutoSize = true,
                Location = new Point(8, 16),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(240, 64),
                TabIndex = 1,
                Anchor = AnchorStyles.None,
            };

            _cancelButton = new Button()
            {
                Size = ScaleHelper.ScaleToDpi(new Size(75, 23), ScaleHelper.InitialSystemDpi),
                TabIndex = 0,
                Text = SR.PrintControllerWithStatusDialog_Cancel,
                Location = new Point(88, 88),
                Anchor = AnchorStyles.None
            };

            _cancelButton.Click += CancelClick;

            _tableLayoutPanel = new TableLayoutPanel()
            {
                AutoSize = true,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                RowCount = 2,
                TabIndex = 0,
            };

            _tableLayoutPanel.ColumnStyles.Add(new(SizeType.Percent, 100F));
            _tableLayoutPanel.RowStyles.Add(new(SizeType.Percent, 50F));
            _tableLayoutPanel.RowStyles.Add(new(SizeType.Percent, 50F));
            _tableLayoutPanel.Controls.Add(_messageLabel, 0, 0);
            _tableLayoutPanel.Controls.Add(_cancelButton, 0, 1);

            AutoScaleDimensions = new Size(6, 13);
            AutoScaleMode = AutoScaleMode.Font;
            MaximizeBox = false;
            ControlBox = false;
            MinimizeBox = false;
            ClientSize = ScaleHelper.ScaleToDpi(new Size(256, 122), ScaleHelper.InitialSystemDpi);

            CancelButton = _cancelButton;
            SizeGripStyle = SizeGripStyle.Hide;
            Controls.Add(_tableLayoutPanel);
        }

        private void CancelClick(object? sender, EventArgs e)
        {
            _cancelButton.Enabled = false;
            _messageLabel.Text = SR.PrintControllerWithStatusDialog_Canceling;
            _backgroundThread._canceled = true;
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new StatusDialogAccessibility(this);
    }
}
