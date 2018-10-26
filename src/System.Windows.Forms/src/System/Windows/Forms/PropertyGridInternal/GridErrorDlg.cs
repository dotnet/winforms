// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System;
    using System.Collections;   
    using System.Windows.Forms;
    using System.Windows.Forms.ComponentModel;
    using System.Windows.Forms.Design;    
    using System.ComponentModel.Design;
    using System.IO;
    using System.Drawing;
    using Microsoft.Win32;
    using Message = System.Windows.Forms.Message;
    using System.Drawing.Drawing2D;    

    /// <include file='doc\GridErrorDlg.uex' path='docs/doc[@for="GridErrorDlg"]/*' />
    /// <devdoc>
    ///     Implements a dialog that is displayed when an unhandled exception occurs in
    ///     a thread. This dialog's width is defined by the summary message
    ///     in the top pane. We don't restrict dialog width in any way.  
    ///     Use caution and check at all DPI scaling factors if adding a new message
    ///     to be displayed in the top pane.
    /// </devdoc>
    internal class GridErrorDlg : Form {
        private TableLayoutPanel overarchingTableLayoutPanel;
        private TableLayoutPanel buttonTableLayoutPanel;
        private PictureBox pictureBox;
        private Label lblMessage;
        private DetailsButton detailsBtn;
        private Button cancelBtn;
        private Button okBtn;
        private TableLayoutPanel pictureLabelTableLayoutPanel;
        private TextBox details;

        private Bitmap expandImage = null;
        private Bitmap collapseImage = null;
        private PropertyGrid ownerGrid;

        private bool detailsButtonExpanded = false;

        public bool DetailsButtonExpanded {
            get {
                return detailsButtonExpanded;
            }
        }

        public string Details {
            set {
                this.details.Text = value;
            }
        }
       

        public string Message {
            set {
                 this.lblMessage.Text = value;
            }
        }

        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // We use " " for the text so we leave a small amount of test.
                                                                                                        // So we don't have to localize it.
        ]
        public GridErrorDlg(PropertyGrid owner) {
            ownerGrid = owner;
            expandImage = new Bitmap(typeof(ThreadExceptionDialog), "down.bmp");
            expandImage.MakeTransparent();
            if (DpiHelper.IsScalingRequired) {
                DpiHelper.ScaleBitmapLogicalToDevice(ref expandImage);
            }
            collapseImage = new Bitmap(typeof(ThreadExceptionDialog), "up.bmp");
            collapseImage.MakeTransparent();
            if (DpiHelper.IsScalingRequired) {
                DpiHelper.ScaleBitmapLogicalToDevice(ref collapseImage);
            }

            InitializeComponent();

            foreach( Control c in this.Controls ){
                if( c.SupportsUseCompatibleTextRendering ){
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

        /// <include file='doc\GridErrorDlg.uex' path='docs/doc[@for="GridErrorDlg.DetailsClick"]/*' />
        /// <devdoc>
        ///     Called when the details button is clicked.
        /// </devdoc>
        private void DetailsClick(object sender, EventArgs devent) {
            int delta = details.Height + 8;

            if (details.Visible) {
                detailsBtn.Image = expandImage;
                detailsButtonExpanded = false;
                Height -= delta;
            }
            else {
                detailsBtn.Image = collapseImage;
                detailsButtonExpanded = true;
                details.Width = overarchingTableLayoutPanel.Width - details.Margin.Horizontal;
                Height += delta;
            }

            details.Visible = !details.Visible;

            if (AccessibilityImprovements.Level1) {
                AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);
                details.TabStop = !details.TabStop;

                if (details.Visible) {
                    details.Focus();
                }
            }
        }

        /// <devdoc>
        ///     Tells whether the current resources for this dll have been
        ///     localized for a RTL language.
        /// </devdoc>
        private static bool IsRTLResources {
            get {
                return SR.RTL != "RTL_False";
            }
        }

        private void InitializeComponent() {
            if (IsRTLResources) {
                this.RightToLeft = RightToLeft.Yes;
            }

            this.detailsBtn = new DetailsButton(this);
            this.overarchingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.pictureLabelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.details = new System.Windows.Forms.TextBox();
            this.overarchingTableLayoutPanel.SuspendLayout();
            this.buttonTableLayoutPanel.SuspendLayout();
            this.pictureLabelTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(73, 30);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(3, 30, 3, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(208, 43);
            this.lblMessage.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(3, 3);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(64, 64);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            this.pictureBox.AutoSize = true;
            // 
            // detailsBtn
            // 
            this.detailsBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.detailsBtn.Location = new System.Drawing.Point(3, 3);
            this.detailsBtn.Margin = new System.Windows.Forms.Padding(12, 3, 29, 3);
            this.detailsBtn.Name = "detailsBtn";
            this.detailsBtn.Size = new System.Drawing.Size(100, 23);
            this.detailsBtn.TabIndex = 4;
            this.detailsBtn.Click += new System.EventHandler(this.DetailsClick);
            // 
            // overarchingTableLayoutPanel
            // 
            this.overarchingTableLayoutPanel.AutoSize = true;
            this.overarchingTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.overarchingTableLayoutPanel.ColumnCount = 1;
            this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overarchingTableLayoutPanel.Controls.Add(this.buttonTableLayoutPanel, 0, 1);
            this.overarchingTableLayoutPanel.Controls.Add(this.pictureLabelTableLayoutPanel, 0, 0);
            this.overarchingTableLayoutPanel.Location = new System.Drawing.Point(1, 0);
            this.overarchingTableLayoutPanel.MinimumSize = new System.Drawing.Size(279, 50);
            this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            this.overarchingTableLayoutPanel.RowCount = 2;
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.overarchingTableLayoutPanel.Size = new System.Drawing.Size(290, 108);
            this.overarchingTableLayoutPanel.TabIndex = 6;
            // 
            // buttonTableLayoutPanel
            // 
            this.buttonTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTableLayoutPanel.AutoSize = true;
            this.buttonTableLayoutPanel.ColumnCount = 3;
            this.overarchingTableLayoutPanel.SetColumnSpan(this.buttonTableLayoutPanel, 2);
            this.buttonTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonTableLayoutPanel.Controls.Add(this.cancelBtn, 2, 0);
            this.buttonTableLayoutPanel.Controls.Add(this.okBtn, 1, 0);
            this.buttonTableLayoutPanel.Controls.Add(this.detailsBtn, 0, 0);
            this.buttonTableLayoutPanel.Location = new System.Drawing.Point(0, 79);
            this.buttonTableLayoutPanel.Name = "buttonTableLayoutPanel";
            this.buttonTableLayoutPanel.RowCount = 1;
            this.buttonTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buttonTableLayoutPanel.Size = new System.Drawing.Size(290, 29);
            this.buttonTableLayoutPanel.TabIndex = 8;
            // 
            // okBtn
            // 
            this.okBtn.AutoSize = true;
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(131, 3);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // cancelBtn
            // 
            this.cancelBtn.AutoSize = true;
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(212, 3);
            this.cancelBtn.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // pictureLabelTableLayoutPanel
            // 
            this.pictureLabelTableLayoutPanel.AutoSize = true;
            this.pictureLabelTableLayoutPanel.AutoSizeMode = Forms.AutoSizeMode.GrowOnly;
            this.pictureLabelTableLayoutPanel.ColumnCount = 2;
            this.pictureLabelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pictureLabelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pictureLabelTableLayoutPanel.Controls.Add(this.lblMessage, 1, 0);
            this.pictureLabelTableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
            this.pictureLabelTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureLabelTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.pictureLabelTableLayoutPanel.Name = "pictureLabelTableLayoutPanel";
            this.pictureLabelTableLayoutPanel.RowCount = 1;
            this.pictureLabelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.pictureLabelTableLayoutPanel.Size = new System.Drawing.Size(284, 73);
            this.pictureLabelTableLayoutPanel.TabIndex = 4;
            // 
            // details
            // 
            this.details.Location = new System.Drawing.Point(4, 114);
            this.details.Multiline = true;
            this.details.Name = "details";
            this.details.ReadOnly = true;
            this.details.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.details.Size = new System.Drawing.Size(273, 100);
            this.details.TabIndex = 3;
            this.details.TabStop = false;
            this.details.Visible = false;
            // 
            // Form1
            // 
            this.AcceptButton = this.okBtn;
            this.AutoSize = true;
            this.AutoScaleMode = Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(299, 113);
            this.Controls.Add(this.details);
            this.Controls.Add(this.overarchingTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.overarchingTableLayoutPanel.ResumeLayout(false);
            this.overarchingTableLayoutPanel.PerformLayout();
            this.buttonTableLayoutPanel.ResumeLayout(false);
            this.buttonTableLayoutPanel.PerformLayout();
            this.pictureLabelTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void OnButtonClick(object s, EventArgs e) {
            DialogResult = ((Button)s).DialogResult;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e) {
            if (this.Visible) {
                // make sure the details button is sized properly
                //
                using (Graphics g = CreateGraphics()) {
                    SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText( this.ownerGrid, g, detailsBtn.Text, detailsBtn.Font);
                    int detailsWidth = (int) Math.Ceiling(sizef.Width);
                    detailsWidth += detailsBtn.Image.Width;
                    detailsBtn.Width = (int) Math.Ceiling(detailsWidth * (ownerGrid.UseCompatibleTextRendering ? 1.15f : 1.4f));
                    detailsBtn.Height = okBtn.Height;
                }

                // Update the location of the TextBox details
                int x = details.Location.X;
                int y = detailsBtn.Location.Y + detailsBtn.Height + detailsBtn.Margin.Bottom;

                // Location is relative to its parent,
                // therefore, we need to take its parent into consideration
                Control parent = detailsBtn.Parent;
                while(parent != null && !(parent is Form)) {
                    y += parent.Location.Y;
                    parent = parent.Parent;
                }

                details.Location = new System.Drawing.Point(x, y);

                if (details.Visible) {
                    DetailsClick(details, EventArgs.Empty);
                }
            }
            okBtn.Focus();
        }
    }

    internal class DetailsButton: Button {
        private GridErrorDlg parent;
        public DetailsButton(GridErrorDlg form){
            parent = form;
        }

        public bool Expanded { 
            get { 
                return parent.DetailsButtonExpanded; 
            } 
        }
        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level1) {
                return new DetailsButtonAccessibleObject(this);
            }
            else {
                return base.CreateAccessibilityInstance();
            }
        }
    }

    internal class DetailsButtonAccessibleObject: Control.ControlAccessibleObject {
        
        private DetailsButton ownerItem = null; 
        
        public DetailsButtonAccessibleObject(DetailsButton owner): base(owner) {
            ownerItem = owner;
        }

        internal override bool IsIAccessibleExSupported() {
            Debug.Assert(ownerItem != null, "AccessibleObject owner cannot be null");
            return true;
        }

        internal override object GetPropertyValue(int propertyID) {
            if (propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                return NativeMethods.UIA_ButtonControlTypeId;
            }
            else {
                return base.GetPropertyValue(propertyID);
            }
        }

        internal override bool IsPatternSupported(int patternId) {
            if (patternId == NativeMethods.UIA_ExpandCollapsePatternId) {
                return true;
            }
            else {
                return base.IsPatternSupported(patternId);
            }
        }

        internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState {
            get {
                return ownerItem.Expanded ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
            }
        }

        internal override void Expand() {
            if (ownerItem !=null && !ownerItem.Expanded) {
                DoDefaultAction();
            }
        }

        internal override void Collapse() {
            if (ownerItem != null && ownerItem.Expanded) {
                DoDefaultAction();
            }
        }
    }
}

