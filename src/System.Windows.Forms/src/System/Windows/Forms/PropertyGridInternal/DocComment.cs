// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class DocComment : PropertyGrid.SnappableControl
    {
        private readonly Label _labelTitle;
        private readonly Label _labelDesc;
        private string _fullDesc;

        private int _lineHeight;
        private bool _needUpdateUIWithFont = true;

        protected const int CBORDER = 3;
        protected const int CXDEF = 0;
        protected const int CYDEF = 59;
        protected const int MIN_LINES = 2;

        private int _cydef = CYDEF;
        private int _cBorder = CBORDER;

        private Rectangle _rect = Rectangle.Empty;

        internal DocComment(PropertyGrid owner) : base(owner)
        {
            SuspendLayout();
            _labelTitle = new Label
            {
                UseMnemonic = false,
                Cursor = Cursors.Default
            };
            _labelDesc = new Label
            {
                AutoEllipsis = true,
                Cursor = Cursors.Default
            };

            UpdateTextRenderingEngine();

            Controls.Add(_labelTitle);
            Controls.Add(_labelDesc);
            if (DpiHelper.IsScalingRequirementMet)
            {
                _cBorder = LogicalToDeviceUnits(CBORDER);
                _cydef = LogicalToDeviceUnits(CYDEF);
            }

            Size = new Size(CXDEF, _cydef);

            Text = SR.PBRSDocCommentPaneTitle;
            SetStyle(ControlStyles.Selectable, false);
            ResumeLayout(false);
        }

        public virtual int Lines
        {
            get
            {
                UpdateUIWithFont();
                return Height / _lineHeight;
            }
            set
            {
                UpdateUIWithFont();
                Size = new Size(Width, 1 + value * _lineHeight);
            }
        }

        public override int GetOptimalHeight(int width)
        {
            UpdateUIWithFont();
            // compute optimal label height as one line only.
            int height = _labelTitle.Size.Height;

            // do this to avoid getting parented to the Parking window.
            //
            if (OwnerPropertyGrid.IsHandleCreated && !IsHandleCreated)
            {
                CreateControl();
            }

            // compute optimal text height
            var isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
            Graphics g = _labelDesc.CreateGraphicsInternal();
            SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(OwnerPropertyGrid, g, _labelTitle.Text, Font, width);
            Size sz = Size.Ceiling(sizef);
            g.Dispose();
            var padding = isScalingRequirementMet ? LogicalToDeviceUnits(2) : 2;
            height += (sz.Height * 2) + padding;
            return Math.Max(height + 2 * padding, isScalingRequirementMet ? LogicalToDeviceUnits(CYDEF) : CYDEF);
        }

        internal virtual void LayoutWindow()
        {
        }

        protected override void OnFontChanged(EventArgs e)
        {
            _needUpdateUIWithFont = true;
            PerformLayout();
            base.OnFontChanged(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            UpdateUIWithFont();
            SetChildLabelsBounds();
            _labelDesc.Text = _fullDesc;
            _labelDesc.AccessibleName = _fullDesc; // Don't crop the description for accessibility clients
            base.OnLayout(e);
        }

        protected override void OnResize(EventArgs e)
        {
            Rectangle newRect = ClientRectangle;
            if (!_rect.IsEmpty && newRect.Width > _rect.Width)
            {
                Rectangle rectInvalidate = new Rectangle(_rect.Width - 1, 0, newRect.Width - _rect.Width + 1, _rect.Height);
                Invalidate(rectInvalidate);
            }

            if (DpiHelper.IsScalingRequirementMet)
            {
                var lineHeightOld = _lineHeight;
                _lineHeight = (int)Font.Height + LogicalToDeviceUnits(2);
                if (lineHeightOld != _lineHeight)
                {
                    _labelTitle.Location = new Point(_cBorder, _cBorder);
                    _labelDesc.Location = new Point(_cBorder, _cBorder + _lineHeight);
                    // Labels were explicitly set bounds. resize of parent is not rescaling labels.
                    SetChildLabelsBounds();
                }
            }

            _rect = newRect;
            base.OnResize(e);
        }

        /// <summary>
        ///  Setting child label bounds
        /// </summary>
        private void SetChildLabelsBounds()
        {
            Size size = ClientSize;

            // if the client size is 0, setting this to a negative number
            // will force an extra layout.
            size.Width = Math.Max(0, size.Width - 2 * _cBorder);
            size.Height = Math.Max(0, size.Height - 2 * _cBorder);

            _labelTitle.SetBounds(_labelTitle.Top,
                                   _labelTitle.Left,
                                   size.Width,
                                   Math.Min(_lineHeight, size.Height),
                                   BoundsSpecified.Size);

            _labelDesc.SetBounds(_labelDesc.Top,
                                  _labelDesc.Left,
                                  size.Width,
                                  Math.Max(0, size.Height - _lineHeight - (DpiHelper.IsScalingRequirementMet ? LogicalToDeviceUnits(1) : 1)),
                                  BoundsSpecified.Size);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateUIWithFont();
        }

        /// <summary>
        ///  Rescaling constants when DPi of the window changed.
        /// </summary>
        /// <param name="deviceDpiOld"> old dpi</param>
        /// <param name="deviceDpiNew"> new dpi</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            if (DpiHelper.IsScalingRequirementMet)
            {
                _cBorder = LogicalToDeviceUnits(CBORDER);
                _cydef = LogicalToDeviceUnits(CYDEF);
            }
        }

        public virtual void SetComment(string title, string desc)
        {
            if (_labelDesc.Text != title)
            {
                _labelTitle.Text = title;
            }

            if (desc != _fullDesc)
            {
                _fullDesc = desc;
                _labelDesc.Text = _fullDesc;
                _labelDesc.AccessibleName = _fullDesc; // Don't crop the description for accessibility clients
            }
        }

        public override int SnapHeightRequest(int cyNew)
        {
            UpdateUIWithFont();
            int lines = Math.Max(MIN_LINES, cyNew / _lineHeight);
            return 1 + lines * _lineHeight;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DocCommentAccessibleObject(this, OwnerPropertyGrid);
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        internal void UpdateTextRenderingEngine()
        {
            _labelTitle.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
            _labelDesc.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
        }

        private void UpdateUIWithFont()
        {
            if (IsHandleCreated && _needUpdateUIWithFont)
            {
                // Some fonts throw because Bold is not a valid option
                // for them.  Fail gracefully.
                try
                {
                    _labelTitle.Font = new Font(Font, FontStyle.Bold);
                }
                catch
                {
                }

                _lineHeight = (int)Font.Height + 2;
                _labelTitle.Location = new Point(_cBorder, _cBorder);
                _labelDesc.Location = new Point(_cBorder, _cBorder + _lineHeight);

                _needUpdateUIWithFont = false;
                PerformLayout();
            }
        }
    }
}
