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
        private readonly Label _labelDescription;
        private string _fullDescription;

        private int _lineHeight;
        private bool _needUpdateUIWithFont = true;

        protected const int BorderSize = 3;
        protected const int DefaultWidth = 0;
        protected const int DefaultHeight = 59;
        protected const int MinimumLines = 2;

        private int _cydef = DefaultHeight;
        private int _cBorder = BorderSize;

        private Rectangle _rect = Rectangle.Empty;

        internal DocComment(PropertyGrid owner) : base(owner)
        {
            SuspendLayout();
            _labelTitle = new()
            {
                UseMnemonic = false,
                Cursor = Cursors.Default
            };

            _labelDescription = new()
            {
                AutoEllipsis = true,
                Cursor = Cursors.Default
            };

            UpdateTextRenderingEngine();

            Controls.Add(_labelTitle);
            Controls.Add(_labelDescription);
            if (DpiHelper.IsScalingRequirementMet)
            {
                _cBorder = LogicalToDeviceUnits(BorderSize);
                _cydef = LogicalToDeviceUnits(DefaultHeight);
            }

            Size = new(DefaultWidth, _cydef);

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
                Size = new(Width, 1 + value * _lineHeight);
            }
        }

        public override int GetOptimalHeight(int width)
        {
            UpdateUIWithFont();

            // Compute optimal label height as one line only.
            int height = _labelTitle.Size.Height;

            // Do this to avoid getting parented to the Parking window.
            if (OwnerPropertyGrid.IsHandleCreated && !IsHandleCreated)
            {
                CreateControl();
            }

            // Compute optimal text height.
            bool isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
            Graphics g = _labelDescription.CreateGraphicsInternal();
            SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(OwnerPropertyGrid, g, _labelTitle.Text, Font, width);
            Size size = Size.Ceiling(sizef);
            g.Dispose();
            int padding = isScalingRequirementMet ? LogicalToDeviceUnits(2) : 2;
            height += (size.Height * 2) + padding;
            return Math.Max(height + 2 * padding, isScalingRequirementMet ? LogicalToDeviceUnits(DefaultHeight) : DefaultHeight);
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
            _labelDescription.Text = _fullDescription;
            _labelDescription.AccessibleName = _fullDescription; // Don't crop the description for accessibility clients
            base.OnLayout(e);
        }

        protected override void OnResize(EventArgs e)
        {
            Rectangle newRect = ClientRectangle;
            if (!_rect.IsEmpty && newRect.Width > _rect.Width)
            {
                Rectangle rectInvalidate = new(_rect.Width - 1, 0, newRect.Width - _rect.Width + 1, _rect.Height);
                Invalidate(rectInvalidate);
            }

            if (DpiHelper.IsScalingRequirementMet)
            {
                int oldLineHeight = _lineHeight;
                _lineHeight = Font.Height + LogicalToDeviceUnits(2);
                if (oldLineHeight != _lineHeight)
                {
                    _labelTitle.Location = new(_cBorder, _cBorder);
                    _labelDescription.Location = new(_cBorder, _cBorder + _lineHeight);

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

            // If the client size is 0, setting this to a negative number will force an extra layout.
            size.Width = Math.Max(0, size.Width - 2 * _cBorder);
            size.Height = Math.Max(0, size.Height - 2 * _cBorder);

            _labelTitle.SetBounds(
                _labelTitle.Top,
                _labelTitle.Left,
                size.Width,
                Math.Min(_lineHeight, size.Height),
                BoundsSpecified.Size);

            _labelDescription.SetBounds(
                _labelDescription.Top,
                _labelDescription.Left,
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
        ///  Rescales constants when DPI of the window has changed.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI.</param>
        /// <param name="deviceDpiNew">New DPI.</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            if (DpiHelper.IsScalingRequirementMet)
            {
                _cBorder = LogicalToDeviceUnits(BorderSize);
                _cydef = LogicalToDeviceUnits(DefaultHeight);
            }
        }

        public virtual void SetComment(string title, string description)
        {
            if (_labelDescription.Text != title)
            {
                _labelTitle.Text = title;
            }

            if (description != _fullDescription)
            {
                _fullDescription = description;
                _labelDescription.Text = _fullDescription;
                _labelDescription.AccessibleName = _fullDescription; // Don't crop the description for accessibility clients
            }
        }

        public override int SnapHeightRequest(int newHeight)
        {
            UpdateUIWithFont();
            int lines = Math.Max(MinimumLines, newHeight / _lineHeight);
            return 1 + lines * _lineHeight;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance() => new DocCommentAccessibleObject(this, OwnerPropertyGrid);

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        internal void UpdateTextRenderingEngine()
        {
            _labelTitle.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
            _labelDescription.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
        }

        private void UpdateUIWithFont()
        {
            if (IsHandleCreated && _needUpdateUIWithFont)
            {
                // Some fonts throw because Bold is not a valid option for them.  Fail gracefully.
                try
                {
                    _labelTitle.Font = new(Font, FontStyle.Bold);
                }
                catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                {
                }

                _lineHeight = Font.Height + 2;
                _labelTitle.Location = new(_cBorder, _cBorder);
                _labelDescription.Location = new(_cBorder, _cBorder + _lineHeight);

                _needUpdateUIWithFont = false;
                PerformLayout();
            }
        }
    }
}
