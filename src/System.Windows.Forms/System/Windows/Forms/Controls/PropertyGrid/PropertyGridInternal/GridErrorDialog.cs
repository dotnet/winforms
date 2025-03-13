// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  Implements a dialog that is displayed when an unhandled exception occurs in a thread. This dialog's width
///  is defined by the summary message in the top pane. We don't restrict the dialog width in any way. Use caution
///  and check at all DPI scaling factors if adding a new message to be displayed in the top pane.
/// </summary>
internal partial class GridErrorDialog : Form
{
    private TableLayoutPanel _overarchingTableLayoutPanel;
    private TableLayoutPanel _buttonTableLayoutPanel;
    private PictureBox _pictureBox;
    private Label _messageLabel;
    private DetailsButton _detailsButton;
    private Button _cancelButton;
    private Button _okButton;
    private TableLayoutPanel _pictureLabelTableLayoutPanel;
    private TextBox _detailsTextBox;

    private readonly Bitmap _expandImage;
    private readonly Bitmap _collapseImage;
    private readonly PropertyGrid _ownerGrid;

    public bool DetailsButtonExpanded { get; private set; }

    [AllowNull]
    public string Details
    {
        set => _detailsTextBox.Text = value;
    }

    public string Message
    {
        set => _messageLabel.Text = value;
    }

    public GridErrorDialog(PropertyGrid owner)
    {
        _ownerGrid = owner;
        _expandImage = ScaleHelper.GetSmallIconResourceAsBitmap(typeof(ThreadExceptionDialog), "down", ScaleHelper.InitialSystemDpi);
        _collapseImage = ScaleHelper.GetSmallIconResourceAsBitmap(typeof(ThreadExceptionDialog), "up", ScaleHelper.InitialSystemDpi);

        InitializeComponent();

        foreach (Control control in Controls)
        {
            if (control.SupportsUseCompatibleTextRendering)
            {
                control.UseCompatibleTextRenderingInternal = _ownerGrid.UseCompatibleTextRendering;
            }
        }

        _pictureBox.Image = SystemIcons.Warning.ToBitmap();
        _detailsButton.Text = $" {SR.ExDlgShowDetails}";

        _detailsTextBox.AccessibleName = SR.ExDlgDetailsText;

        _okButton.Text = SR.ExDlgOk;
        _cancelButton.Text = SR.ExDlgCancel;
        _detailsButton.Image = _expandImage;
    }

    /// <summary>
    ///  Called when the details button is clicked.
    /// </summary>
    private void DetailsClick(object? sender, EventArgs devent)
    {
        int delta = _detailsTextBox.Height + 8;

        if (_detailsTextBox.Visible)
        {
            _detailsButton.Image = _expandImage;
            DetailsButtonExpanded = false;
            Height -= delta;
        }
        else
        {
            _detailsButton.Image = _collapseImage;
            DetailsButtonExpanded = true;
            _detailsTextBox.Width = _overarchingTableLayoutPanel.Width - _detailsTextBox.Margin.Horizontal;
            Height += delta;
        }

        _detailsTextBox.Visible = !_detailsTextBox.Visible;

        AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
        _detailsTextBox.TabStop = !_detailsTextBox.TabStop;

        if (_detailsTextBox.Visible)
        {
            _detailsTextBox.Focus();
        }
    }

    /// <summary>
    ///  Tells whether the current resources for this dll have been localized for a RTL language.
    /// </summary>
    private static bool IsRTLResources => SR.RTL != "RTL_False";

    [MemberNotNull(nameof(_messageLabel))]
    [MemberNotNull(nameof(_pictureBox))]
    [MemberNotNull(nameof(_detailsButton))]
    [MemberNotNull(nameof(_okButton))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_buttonTableLayoutPanel))]
    [MemberNotNull(nameof(_pictureLabelTableLayoutPanel))]
    [MemberNotNull(nameof(_overarchingTableLayoutPanel))]
    [MemberNotNull(nameof(_detailsTextBox))]
    private void InitializeComponent()
    {
        if (IsRTLResources)
        {
            RightToLeft = RightToLeft.Yes;
        }

        SuspendLayout();

        _messageLabel = new()
        {
            AutoSize = true,
            Location = new(73, 30),
            Margin = new(3, 30, 3, 0),
            Name = "lblMessage",
            Size = new(208, 43),
            TabIndex = 0
        };

        _pictureBox = new()
        {
            Location = new(3, 3),
            Name = "pictureBox",
            Size = new(64, 64),
            SizeMode = PictureBoxSizeMode.CenterImage,
            TabIndex = 5,
            TabStop = false,
            AutoSize = true
        };

        _detailsButton = new(this)
        {
            ImageAlign = ContentAlignment.MiddleLeft,
            Location = new(3, 3),
            Margin = new(12, 3, 29, 3),
            Name = "detailsBtn",
            Size = new(100, 23),
            TabIndex = 4
        };

        _detailsButton.Click += DetailsClick;

        _okButton = new()
        {
            AutoSize = true,
            DialogResult = DialogResult.OK,
            Location = new(131, 3),
            Name = "okBtn",
            Size = new(75, 23),
            TabIndex = 1
        };

        _okButton.Click += OnButtonClick;

        _cancelButton = new Button
        {
            AutoSize = true,
            DialogResult = DialogResult.Cancel,
            Location = new(212, 3),
            Margin = new(0, 3, 3, 3),
            Name = "cancelBtn",
            Size = new(75, 23),
            TabIndex = 2
        };

        _cancelButton.Click += OnButtonClick;

        _buttonTableLayoutPanel = new TableLayoutPanel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            AutoSize = true,
            ColumnCount = 3,
            Location = new(0, 79),
            Name = "buttonTableLayoutPanel",
            RowCount = 1,
            Size = new(290, 29),
            TabIndex = 8
        };

        _buttonTableLayoutPanel.SuspendLayout();

        _buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _buttonTableLayoutPanel.Controls.Add(_cancelButton, 2, 0);
        _buttonTableLayoutPanel.Controls.Add(_okButton, 1, 0);
        _buttonTableLayoutPanel.Controls.Add(_detailsButton, 0, 0);
        _buttonTableLayoutPanel.RowStyles.Add(new RowStyle());

        _pictureLabelTableLayoutPanel = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Location = new(3, 3),
            Name = "pictureLabelTableLayoutPanel",
            RowCount = 1,
            Size = new(284, 73),
            TabIndex = 4
        };

        _pictureLabelTableLayoutPanel.SuspendLayout();

        _pictureLabelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _pictureLabelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _pictureLabelTableLayoutPanel.Controls.Add(_messageLabel, 1, 0);
        _pictureLabelTableLayoutPanel.Controls.Add(_pictureBox, 0, 0);
        _pictureLabelTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _overarchingTableLayoutPanel = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Location = new(1, 0),
            Size = new(290, 108),
            MinimumSize = new(279, 50),
            Name = "overarchingTableLayoutPanel",
            ColumnCount = 1,
            RowCount = 2,
            TabIndex = 6
        };

        _overarchingTableLayoutPanel.SuspendLayout();

        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _overarchingTableLayoutPanel.Controls.Add(_buttonTableLayoutPanel, 0, 1);
        _overarchingTableLayoutPanel.Controls.Add(_pictureLabelTableLayoutPanel, 0, 0);
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _overarchingTableLayoutPanel.SetColumnSpan(_buttonTableLayoutPanel, 2);

        _detailsTextBox = new()
        {
            Location = new(4, 114),
            Multiline = true,
            Name = "details",
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Size = new(273, 100),
            TabIndex = 3,
            TabStop = false,
            Visible = false
        };

        ((System.ComponentModel.ISupportInitialize)(_pictureBox)).BeginInit();

        AcceptButton = _okButton;
        AutoSize = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancelButton;
        ClientSize = new(299, 113);
        Controls.Add(_detailsTextBox);
        Controls.Add(_overarchingTableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Form1";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        _overarchingTableLayoutPanel.ResumeLayout(performLayout: false);
        _overarchingTableLayoutPanel.PerformLayout();
        _buttonTableLayoutPanel.ResumeLayout(performLayout: false);
        _buttonTableLayoutPanel.PerformLayout();
        _pictureLabelTableLayoutPanel.ResumeLayout(performLayout: false);
        ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
        ResumeLayout(performLayout: false);
        PerformLayout();
    }

    private void OnButtonClick(object? s, EventArgs e)
    {
        DialogResult = ((Button)s!).DialogResult;
        Close();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        if (Visible)
        {
            // Make sure the details button is sized properly.
            using (Graphics g = CreateGraphics())
            {
                SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(_ownerGrid, g, _detailsButton.Text, _detailsButton.Font);
                int detailsWidth = (int)Math.Ceiling(sizef.Width);
                detailsWidth += _detailsButton.Image!.Width;
                _detailsButton.Width = (int)Math.Ceiling(detailsWidth * (_ownerGrid.UseCompatibleTextRendering ? 1.15f : 1.4f));
                _detailsButton.Height = _okButton.Height;
            }

            // Update the location of the TextBox details.
            int x = _detailsTextBox.Location.X;
            int y = _detailsButton.Location.Y + _detailsButton.Height + _detailsButton.Margin.Bottom;

            // Location is relative to its parent.
            Control? parent = _detailsButton.Parent;
            while (parent is not null and not Form)
            {
                y += parent.Location.Y;
                parent = parent.Parent;
            }

            _detailsTextBox.Location = new Point(x, y);

            if (_detailsTextBox.Visible)
            {
                DetailsClick(_detailsTextBox, EventArgs.Empty);
            }
        }

        _okButton.Focus();
    }
}
