// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Windows.Forms;
    
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Windows.Forms.Layout;

    internal class DocComment : PropertyGrid.SnappableControl {
        
        private Label m_labelTitle;
        private Label m_labelDesc;
        private string fullDesc;
        
        protected int lineHeight;
        private bool needUpdateUIWithFont = true;

        protected const int CBORDER = 3;
        protected const int CXDEF = 0;
        protected const int CYDEF = 59;
        protected const int MIN_LINES = 2;

        private int cydef = CYDEF;
        private int cBorder = CBORDER;

        internal Rectangle rect = Rectangle.Empty;

        internal DocComment(PropertyGrid owner) : base(owner) {
            SuspendLayout();
            m_labelTitle = new Label();
            m_labelTitle.UseMnemonic = false;
            m_labelTitle.Cursor = Cursors.Default;
            m_labelDesc = new Label();
            m_labelDesc.AutoEllipsis = true;
            m_labelDesc.Cursor = Cursors.Default;

            UpdateTextRenderingEngine();

            Controls.Add(m_labelTitle);
            Controls.Add(m_labelDesc);
            if (DpiHelper.IsScalingRequirementMet) {
                cBorder = LogicalToDeviceUnits(CBORDER);
                cydef = LogicalToDeviceUnits(CYDEF);
            }

            Size = new Size(CXDEF, cydef);

            this.Text = SR.PBRSDocCommentPaneTitle;
            SetStyle(ControlStyles.Selectable, false);
            ResumeLayout(false);
        }

        public virtual int Lines {
            get {
                UpdateUIWithFont();
                return Height/lineHeight;
            }
            set {
                UpdateUIWithFont();
                Size = new Size(Width, 1 + value * lineHeight);
            }
        }

        public override int GetOptimalHeight(int width) {
            UpdateUIWithFont();
            // compute optimal label height as one line only.
            int height = m_labelTitle.Size.Height;

            // do this to avoid getting parented to the Parking window.
            //
            if (this.ownerGrid.IsHandleCreated && !IsHandleCreated) {
                CreateControl();
            }

            // compute optimal text height
            var isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
            Graphics g = m_labelDesc.CreateGraphicsInternal();
            SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText( this.ownerGrid, g, m_labelTitle.Text, Font, width);
            Size sz = Size.Ceiling(sizef);
            g.Dispose();
            var padding = isScalingRequirementMet ? LogicalToDeviceUnits(2) : 2;
            height += (sz.Height * 2) + padding;
            return Math.Max(height + 2 * padding, isScalingRequirementMet ? LogicalToDeviceUnits(CYDEF) : CYDEF);
        }

        internal virtual void LayoutWindow() {
        }

        protected override void OnFontChanged(EventArgs e) {
            needUpdateUIWithFont = true;
            PerformLayout();
            base.OnFontChanged(e);
        }

        protected override void OnLayout(LayoutEventArgs e) {
            UpdateUIWithFont();
            SetChildLabelsBounds();
            m_labelDesc.Text = this.fullDesc;
            m_labelDesc.AccessibleName = this.fullDesc; // Don't crop the description for accessibility clients
            base.OnLayout(e);
        }
        
        protected override void OnResize(EventArgs e) {
            Rectangle newRect = ClientRectangle;
            if (!rect.IsEmpty && newRect.Width > rect.Width) {
                Rectangle rectInvalidate = new Rectangle(rect.Width - 1, 0, newRect.Width - rect.Width + 1, rect.Height);
                Invalidate(rectInvalidate);
            }
            if (DpiHelper.IsScalingRequirementMet) {
                var lineHeightOld = lineHeight;
                lineHeight = (int)Font.Height + LogicalToDeviceUnits(2);
                if (lineHeightOld != lineHeight) {
                    m_labelTitle.Location = new Point(cBorder, cBorder);
                    m_labelDesc.Location = new Point(cBorder, cBorder + lineHeight);
                    // Labels were explicitly set bounds. resize of parent is not rescaling labels.
                    SetChildLabelsBounds();
                }
            }

                rect = newRect;
            base.OnResize(e);
        }

        /// <summary>
        /// Setting child label bounds
        /// </summary>
        private void SetChildLabelsBounds() {
            Size size = ClientSize;

            // if the client size is 0, setting this to a negative number
            // will force an extra layout.  
            size.Width = Math.Max(0, size.Width - 2 * cBorder);
            size.Height = Math.Max(0, size.Height - 2 * cBorder);

            m_labelTitle.SetBounds(m_labelTitle.Top,
                                   m_labelTitle.Left,
                                   size.Width,
                                   Math.Min(lineHeight, size.Height),
                                   BoundsSpecified.Size);

            m_labelDesc.SetBounds(m_labelDesc.Top,
                                  m_labelDesc.Left,
                                  size.Width,
                                  Math.Max(0, size.Height - lineHeight - (DpiHelper.IsScalingRequirementMet ? LogicalToDeviceUnits(1) : 1)),
                                  BoundsSpecified.Size);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            UpdateUIWithFont();
        }

        /// <summary>
        /// Rescaling constants when DPi of the window changed.
        /// </summary>
        /// <param name="deviceDpiOld"> old dpi</param>
        /// <param name="deviceDpiNew"> new dpi</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            if (DpiHelper.IsScalingRequirementMet) {
                cBorder = LogicalToDeviceUnits(CBORDER);
                cydef = LogicalToDeviceUnits(CYDEF);
            }
        }

        public virtual void SetComment(string title, string desc) {
            if (m_labelDesc.Text != title) {
                m_labelTitle.Text = title;
            }
            
            if (desc != fullDesc) {
                this.fullDesc = desc;
                m_labelDesc.Text = fullDesc;
                m_labelDesc.AccessibleName = this.fullDesc; // Don't crop the description for accessibility clients
            }
        }

        public override int SnapHeightRequest(int cyNew) {
            UpdateUIWithFont();
            int lines = Math.Max(MIN_LINES, cyNew/lineHeight);
            return 1 + lines*lineHeight;
        }

        internal void UpdateTextRenderingEngine() {
            m_labelTitle.UseCompatibleTextRendering = this.ownerGrid.UseCompatibleTextRendering;
            m_labelDesc.UseCompatibleTextRendering = this.ownerGrid.UseCompatibleTextRendering;
        }

        private void UpdateUIWithFont() {
            if (IsHandleCreated && needUpdateUIWithFont) {

                // Some fonts throw because Bold is not a valid option
                // for them.  Fail gracefully.
                try {
                    m_labelTitle.Font = new Font(Font, FontStyle.Bold);
                }
                catch {
                }

                lineHeight = (int)Font.Height + 2;
                m_labelTitle.Location = new Point(cBorder, cBorder);
                m_labelDesc.Location = new Point(cBorder, cBorder + lineHeight);

                needUpdateUIWithFont = false;
                PerformLayout();
            }
        }
    }
}
