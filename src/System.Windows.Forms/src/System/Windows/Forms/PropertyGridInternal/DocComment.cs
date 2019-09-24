// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class DocComment : PropertyGrid.SnappableControl
    {
        private readonly Label m_labelTitle;
        private readonly Label m_labelDesc;
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

        internal DocComment(PropertyGrid owner) : base(owner)
        {
            SuspendLayout();
            m_labelTitle = new Label
            {
                UseMnemonic = false,
                Cursor = Cursors.Default
            };
            m_labelDesc = new Label
            {
                AutoEllipsis = true,
                Cursor = Cursors.Default
            };

            UpdateTextRenderingEngine();

            Controls.Add(m_labelTitle);
            Controls.Add(m_labelDesc);
            if (DpiHelper.IsScalingRequirementMet)
            {
                cBorder = LogicalToDeviceUnits(CBORDER);
                cydef = LogicalToDeviceUnits(CYDEF);
            }

            Size = new Size(CXDEF, cydef);

            Text = SR.PBRSDocCommentPaneTitle;
            SetStyle(ControlStyles.Selectable, false);
            ResumeLayout(false);
        }

        public virtual int Lines
        {
            get
            {
                UpdateUIWithFont();
                return Height / lineHeight;
            }
            set
            {
                UpdateUIWithFont();
                Size = new Size(Width, 1 + value * lineHeight);
            }
        }

        public override int GetOptimalHeight(int width)
        {
            UpdateUIWithFont();
            // compute optimal label height as one line only.
            int height = m_labelTitle.Size.Height;

            // do this to avoid getting parented to the Parking window.
            //
            if (ownerGrid.IsHandleCreated && !IsHandleCreated)
            {
                CreateControl();
            }

            // compute optimal text height
            var isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
            Graphics g = m_labelDesc.CreateGraphicsInternal();
            SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(ownerGrid, g, m_labelTitle.Text, Font, width);
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
            needUpdateUIWithFont = true;
            PerformLayout();
            base.OnFontChanged(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            UpdateUIWithFont();
            SetChildLabelsBounds();
            m_labelDesc.Text = fullDesc;
            m_labelDesc.AccessibleName = fullDesc; // Don't crop the description for accessibility clients
            base.OnLayout(e);
        }

        protected override void OnResize(EventArgs e)
        {
            Rectangle newRect = ClientRectangle;
            if (!rect.IsEmpty && newRect.Width > rect.Width)
            {
                Rectangle rectInvalidate = new Rectangle(rect.Width - 1, 0, newRect.Width - rect.Width + 1, rect.Height);
                Invalidate(rectInvalidate);
            }
            if (DpiHelper.IsScalingRequirementMet)
            {
                var lineHeightOld = lineHeight;
                lineHeight = (int)Font.Height + LogicalToDeviceUnits(2);
                if (lineHeightOld != lineHeight)
                {
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
        ///  Setting child label bounds
        /// </summary>
        private void SetChildLabelsBounds()
        {
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
                cBorder = LogicalToDeviceUnits(CBORDER);
                cydef = LogicalToDeviceUnits(CYDEF);
            }
        }

        public virtual void SetComment(string title, string desc)
        {
            if (m_labelDesc.Text != title)
            {
                m_labelTitle.Text = title;
            }

            if (desc != fullDesc)
            {
                fullDesc = desc;
                m_labelDesc.Text = fullDesc;
                m_labelDesc.AccessibleName = fullDesc; // Don't crop the description for accessibility clients
            }
        }

        public override int SnapHeightRequest(int cyNew)
        {
            UpdateUIWithFont();
            int lines = Math.Max(MIN_LINES, cyNew / lineHeight);
            return 1 + lines * lineHeight;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control.
        /// </summary>
        /// <returns>The accessibility object for this control.</returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DocCommentAccessibleObject(this, ownerGrid);
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        internal void UpdateTextRenderingEngine()
        {
            m_labelTitle.UseCompatibleTextRendering = ownerGrid.UseCompatibleTextRendering;
            m_labelDesc.UseCompatibleTextRendering = ownerGrid.UseCompatibleTextRendering;
        }

        private void UpdateUIWithFont()
        {
            if (IsHandleCreated && needUpdateUIWithFont)
            {

                // Some fonts throw because Bold is not a valid option
                // for them.  Fail gracefully.
                try
                {
                    m_labelTitle.Font = new Font(Font, FontStyle.Bold);
                }
                catch
                {
                }

                lineHeight = (int)Font.Height + 2;
                m_labelTitle.Location = new Point(cBorder, cBorder);
                m_labelDesc.Location = new Point(cBorder, cBorder + lineHeight);

                needUpdateUIWithFont = false;
                PerformLayout();
            }
        }
    }

    /// <summary>
    ///  Represents the DocComment control accessible object.
    /// </summary>
    [Runtime.InteropServices.ComVisible(true)]
    internal class DocCommentAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly PropertyGrid _parentPropertyGrid;

        /// <summary>
        ///  Initializes new instance of DocCommentAccessibleObject.
        /// </summary>
        /// <param name="owningDocComment">The owning DocComment control.</param>
        /// <param name="parentPropertyGrid">The parent PropertyGrid control.</param>
        public DocCommentAccessibleObject(DocComment owningDocComment, PropertyGrid parentPropertyGrid) : base(owningDocComment)
        {
            _parentPropertyGrid = parentPropertyGrid;
        }

        /// <summary>
        ///  Request to return the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            if (_parentPropertyGrid.AccessibilityObject is PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                UnsafeNativeMethods.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                if (navigationTarget != null)
                {
                    return navigationTarget;
                }
            }

            return base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  Request value of specified property from an element.
        /// </summary>
        /// <param name="propertyId">Identifier indicating the property to return</param>
        /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
        internal override object GetPropertyValue(int propertyID)
            => propertyID switch
            {
                NativeMethods.UIA_ControlTypePropertyId => NativeMethods.UIA_PaneControlTypeId,
                NativeMethods.UIA_NamePropertyId => Name,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string Name
        {
            get
            {
                string name = Owner?.AccessibleName;
                if (name != null)
                {
                    return name;
                }

                return string.Format(SR.PropertyGridDocCommentAccessibleNameTemplate, _parentPropertyGrid?.Name);
            }
        }
    }
}
