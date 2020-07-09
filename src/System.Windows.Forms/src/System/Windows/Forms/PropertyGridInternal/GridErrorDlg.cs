// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    /// <summary>
    ///  Implements a dialog that is displayed when an unhandled exception occurs in
    ///  a thread. This dialog's width is defined by the summary message
    ///  in the top pane. We don't restrict dialog width in any way.
    ///  Use caution and check at all DPI scaling factors if adding a new message
    ///  to be displayed in the top pane.
    /// </summary>
    internal class GridErrorDlg : Form
    {
        private TableLayoutPanel overarchingTableLayoutPanel;
        private TableLayoutPanel buttonTableLayoutPanel;
        private PictureBox pictureBox;
        private Label lblMessage;
        private DetailsButton detailsBtn;
        private Button cancelBtn;
        private Button okBtn;
        private TableLayoutPanel pictureLabelTableLayoutPanel;
        private TextBox details;

        private readonly Bitmap expandImage;
        private readonly Bitmap collapseImage;
        private readonly PropertyGrid ownerGrid;

        private bool detailsButtonExpanded;

        public bool DetailsButtonExpanded
        {
            get
            {
                return detailsButtonExpanded;
            }
        }

        public string Details
        {
            set
            {
                details.Text = value;
            }
        }

        public string Message
        {
            set
            {
                lblMessage.Text = value;
            }
        }

        public GridErrorDlg(PropertyGrid owner)
        {
            ownerGrid = owner;
            expandImage = DpiHelper.GetBitmapFromIcon(typeof(ThreadExceptionDialog), "down");
            if (DpiHelper.IsScalingRequired)
            {
                DpiHelper.ScaleBitmapLogicalToDevice(ref expandImage);
            }
            collapseImage = DpiHelper.GetBitmapFromIcon(typeof(ThreadExceptionDialog), "up");
            if (DpiHelper.IsScalingRequired)
            {
                DpiHelper.ScaleBitmapLogicalToDevice(ref collapseImage);
            }

            InitializeComponent();

            foreach (Control c in Controls)
            {
                if (c.SupportsUseCompatibleTextRendering)
                {
                    c.UseCompatibleTextRenderingInt = ownerGrid.UseCompatibleTextRendering;
                }
            }

            pictureBox.Image = SystemIcons.Warning.ToBitmap();
            detailsBtn.Text = " " + SR.ExDlgShowDetails;

            details.AccessibleName = SR.ExDlgDetailsText;

            okBtn.Text = SR.ExDlgOk;
            cancelBtn.Text = SR.ExDlgCancel;
            detailsBtn.Image = expandImage;
        }

        /// <summary>
        ///  Called when the details button is clicked.
        /// </summary>
        private void DetailsClick(object sender, EventArgs devent)
        {
            int delta = details.Height + 8;

            if (details.Visible)
            {
                detailsBtn.Image = expandImage;
                detailsButtonExpanded = false;
                Height -= delta;
            }
            else
            {
                detailsBtn.Image = collapseImage;
                detailsButtonExpanded = true;
                details.Width = overarchingTableLayoutPanel.Width - details.Margin.Horizontal;
                Height += delta;
            }

            details.Visible = !details.Visible;

            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
            details.TabStop = !details.TabStop;

            if (details.Visible)
            {
                details.Focus();
            }
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

            detailsBtn = new DetailsButton(this);
            overarchingTableLayoutPanel = new TableLayoutPanel();
            buttonTableLayoutPanel = new TableLayoutPanel();
            okBtn = new Button();
            cancelBtn = new Button();
            pictureLabelTableLayoutPanel = new TableLayoutPanel();
            lblMessage = new Label();
            pictureBox = new PictureBox();
            details = new TextBox();
            overarchingTableLayoutPanel.SuspendLayout();
            buttonTableLayoutPanel.SuspendLayout();
            pictureLabelTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
            SuspendLayout();
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(73, 30);
            lblMessage.Margin = new Padding(3, 30, 3, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(208, 43);
            lblMessage.TabIndex = 0;
            //
            // pictureBox
            //
            pictureBox.Location = new Point(3, 3);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(64, 64);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            pictureBox.TabIndex = 5;
            pictureBox.TabStop = false;
            pictureBox.AutoSize = true;
            //
            // detailsBtn
            //
            detailsBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            detailsBtn.Location = new Point(3, 3);
            detailsBtn.Margin = new Padding(12, 3, 29, 3);
            detailsBtn.Name = "detailsBtn";
            detailsBtn.Size = new Size(100, 23);
            detailsBtn.TabIndex = 4;
            detailsBtn.Click += new EventHandler(DetailsClick);
            //
            // overarchingTableLayoutPanel
            //
            overarchingTableLayoutPanel.AutoSize = true;
            overarchingTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            overarchingTableLayoutPanel.ColumnCount = 1;
            overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            overarchingTableLayoutPanel.Controls.Add(buttonTableLayoutPanel, 0, 1);
            overarchingTableLayoutPanel.Controls.Add(pictureLabelTableLayoutPanel, 0, 0);
            overarchingTableLayoutPanel.Location = new Point(1, 0);
            overarchingTableLayoutPanel.MinimumSize = new Size(279, 50);
            overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            overarchingTableLayoutPanel.RowCount = 2;
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            overarchingTableLayoutPanel.Size = new Size(290, 108);
            overarchingTableLayoutPanel.TabIndex = 6;
            //
            // buttonTableLayoutPanel
            //
            buttonTableLayoutPanel.Anchor = ((AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            buttonTableLayoutPanel.AutoSize = true;
            buttonTableLayoutPanel.ColumnCount = 3;
            overarchingTableLayoutPanel.SetColumnSpan(buttonTableLayoutPanel, 2);
            buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            buttonTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            buttonTableLayoutPanel.Controls.Add(cancelBtn, 2, 0);
            buttonTableLayoutPanel.Controls.Add(okBtn, 1, 0);
            buttonTableLayoutPanel.Controls.Add(detailsBtn, 0, 0);
            buttonTableLayoutPanel.Location = new Point(0, 79);
            buttonTableLayoutPanel.Name = "buttonTableLayoutPanel";
            buttonTableLayoutPanel.RowCount = 1;
            buttonTableLayoutPanel.RowStyles.Add(new RowStyle());
            buttonTableLayoutPanel.Size = new Size(290, 29);
            buttonTableLayoutPanel.TabIndex = 8;
            //
            // okBtn
            //
            okBtn.AutoSize = true;
            okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            okBtn.Location = new Point(131, 3);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(75, 23);
            okBtn.TabIndex = 1;
            okBtn.Click += new EventHandler(OnButtonClick);
            //
            // cancelBtn
            //
            cancelBtn.AutoSize = true;
            cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelBtn.Location = new Point(212, 3);
            cancelBtn.Margin = new Padding(0, 3, 3, 3);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(75, 23);
            cancelBtn.TabIndex = 2;
            cancelBtn.Click += new EventHandler(OnButtonClick);
            //
            // pictureLabelTableLayoutPanel
            //
            pictureLabelTableLayoutPanel.AutoSize = true;
            pictureLabelTableLayoutPanel.AutoSizeMode = Forms.AutoSizeMode.GrowOnly;
            pictureLabelTableLayoutPanel.ColumnCount = 2;
            pictureLabelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            pictureLabelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            pictureLabelTableLayoutPanel.Controls.Add(lblMessage, 1, 0);
            pictureLabelTableLayoutPanel.Controls.Add(pictureBox, 0, 0);
            pictureLabelTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureLabelTableLayoutPanel.Location = new Point(3, 3);
            pictureLabelTableLayoutPanel.Name = "pictureLabelTableLayoutPanel";
            pictureLabelTableLayoutPanel.RowCount = 1;
            pictureLabelTableLayoutPanel.RowStyles.Add(new RowStyle(System.Windows.Forms.SizeType.AutoSize));
            pictureLabelTableLayoutPanel.Size = new Size(284, 73);
            pictureLabelTableLayoutPanel.TabIndex = 4;
            //
            // details
            //
            details.Location = new Point(4, 114);
            details.Multiline = true;
            details.Name = "details";
            details.ReadOnly = true;
            details.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            details.Size = new Size(273, 100);
            details.TabIndex = 3;
            details.TabStop = false;
            details.Visible = false;
            //
            // Form1
            //
            AcceptButton = okBtn;
            AutoSize = true;
            AutoScaleMode = Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            CancelButton = cancelBtn;
            ClientSize = new Size(299, 113);
            Controls.Add(details);
            Controls.Add(overarchingTableLayoutPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            overarchingTableLayoutPanel.ResumeLayout(false);
            overarchingTableLayoutPanel.PerformLayout();
            buttonTableLayoutPanel.ResumeLayout(false);
            buttonTableLayoutPanel.PerformLayout();
            pictureLabelTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void OnButtonClick(object s, EventArgs e)
        {
            DialogResult = ((Button)s).DialogResult;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                // make sure the details button is sized properly
                //
                using (Graphics g = CreateGraphics())
                {
                    SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(ownerGrid, g, detailsBtn.Text, detailsBtn.Font);
                    int detailsWidth = (int)Math.Ceiling(sizef.Width);
                    detailsWidth += detailsBtn.Image.Width;
                    detailsBtn.Width = (int)Math.Ceiling(detailsWidth * (ownerGrid.UseCompatibleTextRendering ? 1.15f : 1.4f));
                    detailsBtn.Height = okBtn.Height;
                }

                // Update the location of the TextBox details
                int x = details.Location.X;
                int y = detailsBtn.Location.Y + detailsBtn.Height + detailsBtn.Margin.Bottom;

                // Location is relative to its parent,
                // therefore, we need to take its parent into consideration
                Control parent = detailsBtn.Parent;
                while (parent != null && !(parent is Form))
                {
                    y += parent.Location.Y;
                    parent = parent.Parent;
                }

                details.Location = new Point(x, y);

                if (details.Visible)
                {
                    DetailsClick(details, EventArgs.Empty);
                }
            }
            okBtn.Focus();
        }
    }

    internal class DetailsButton : Button
    {
        private readonly GridErrorDlg parent;
        public DetailsButton(GridErrorDlg form)
        {
            parent = form;
        }

        public bool Expanded
        {
            get
            {
                return parent.DetailsButtonExpanded;
            }
        }
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DetailsButtonAccessibleObject(this);
        }
    }

    internal class DetailsButtonAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly DetailsButton ownerItem;

        public DetailsButtonAccessibleObject(DetailsButton owner) : base(owner)
        {
            ownerItem = owner;
        }

        internal override bool IsIAccessibleExSupported()
        {
            Debug.Assert(ownerItem != null, "AccessibleObject owner cannot be null");
            return true;
        }

        internal override object GetPropertyValue(UiaCore.UIA propertyID)
        {
            if (propertyID == UiaCore.UIA.ControlTypePropertyId)
            {
                return UiaCore.UIA.ButtonControlTypeId;
            }
            else
            {
                return base.GetPropertyValue(propertyID);
            }
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
        {
            if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
            {
                return true;
            }
            else
            {
                return base.IsPatternSupported(patternId);
            }
        }

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return ownerItem.Expanded ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
            }
        }

        internal override void Expand()
        {
            if (ownerItem != null && !ownerItem.Expanded)
            {
                DoDefaultAction();
            }
        }

        internal override void Collapse()
        {
            if (ownerItem != null && ownerItem.Expanded)
            {
                DoDefaultAction();
            }
        }
    }
}
