// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    internal class ToolStripGrip : ToolStripButton
    {
        private Cursor _oldCursor;
        private Point _startLocation = Point.Empty;
        private bool _movingToolStrip;
        private Point _lastEndLocation = ToolStrip.s_invalidMouseEnter;
        private static Size s_dragSize = LayoutUtils.MaxSize;

        private static readonly Padding _defaultPadding = new Padding(2);
        private const int GripThicknessDefault = 3;
        private const int GripThicknessVisualStylesEnabled = 5;
        private Padding _scaledDefaultPadding = _defaultPadding;
        private int _scaledGripThickness = GripThicknessDefault;
        private int _scaledGripThicknessVisualStylesEnabled = GripThicknessVisualStylesEnabled;

        internal ToolStripGrip()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(_defaultPadding);
                _scaledGripThickness = DpiHelper.LogicalToDeviceUnitsX(GripThicknessDefault);
                _scaledGripThicknessVisualStylesEnabled = DpiHelper.LogicalToDeviceUnitsX(GripThicknessVisualStylesEnabled);
            }

            // if we're using Visual Styles we've got to be a bit thicker.
            GripThickness = ToolStripManager.VisualStylesEnabled ? _scaledGripThicknessVisualStylesEnabled : _scaledGripThickness;
            SupportsItemClick = false;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected internal override Padding DefaultMargin
        {
            get
            {
                return _scaledDefaultPadding;
            }
        }

        public override bool CanSelect
        {
            get
            {
                return false;
            }
        }

        internal int GripThickness { get; private set; }

        internal bool MovingToolStrip
        {
            get
            {
                return ((ToolStripPanelRow != null) && _movingToolStrip);
            }
            set
            {
                if ((_movingToolStrip != value) && ParentInternal != null)
                {
                    if (value)
                    {
                        // dont let grips move the toolstrip
                        if (ParentInternal.ToolStripPanelRow is null)
                        {
                            return;
                        }
                    }
                    _movingToolStrip = value;
                    _lastEndLocation = ToolStrip.s_invalidMouseEnter;
                    if (_movingToolStrip)
                    {
                        ((ISupportToolStripPanel)ParentInternal).BeginDrag();
                    }
                    else
                    {
                        ((ISupportToolStripPanel)ParentInternal).EndDrag();
                    }
                }
            }
        }

        private ToolStripPanelRow ToolStripPanelRow
        {
            get
            {
                return (ParentInternal is null) ? null : ((ISupportToolStripPanel)ParentInternal).ToolStripPanelRow;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripGripAccessibleObject(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = Size.Empty;
            if (ParentInternal != null)
            {
                if (ParentInternal.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow)
                {
                    preferredSize = new Size(ParentInternal.Width, GripThickness);
                }
                else
                {
                    preferredSize = new Size(GripThickness, ParentInternal.Height);
                }
            }
            // Constrain ourselves
            if (preferredSize.Width > constrainingSize.Width)
            {
                preferredSize.Width = constrainingSize.Width;
            }

            if (preferredSize.Height > constrainingSize.Height)
            {
                preferredSize.Height = constrainingSize.Height;
            }

            return preferredSize;
        }

        private bool LeftMouseButtonIsDown()
        {
            return (Control.MouseButtons == MouseButtons.Left) && (Control.ModifierKeys == Keys.None);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // all the grip painting should be on the ToolStrip itself.
            if (ParentInternal != null)
            {
                ParentInternal.OnPaintGrip(e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="mea"></param>
        protected override void OnMouseDown(MouseEventArgs mea)
        {
            _startLocation = TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
            base.OnMouseDown(mea);
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            bool leftMouseButtonDown = LeftMouseButtonIsDown();
            if (!MovingToolStrip && leftMouseButtonDown)
            {
                // determine if we've moved far enough such that the toolstrip
                // can be considered as moving.
                Point currentLocation = TranslatePoint(mea.Location, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                int deltaX = currentLocation.X - _startLocation.X;
                deltaX = (deltaX < 0) ? deltaX * -1 : deltaX;

                if (s_dragSize == LayoutUtils.MaxSize)
                {
                    s_dragSize = SystemInformation.DragSize;
                }

                if (deltaX >= s_dragSize.Width)
                {
                    MovingToolStrip = true;
                }
                else
                {
                    int deltaY = currentLocation.Y - _startLocation.Y;
                    deltaY = (deltaY < 0) ? deltaY * -1 : deltaY;

                    if (deltaY >= s_dragSize.Height)
                    {
                        MovingToolStrip = true;
                    }
                }
            }
            if (MovingToolStrip)
            {
                if (leftMouseButtonDown)
                {
                    Point endLocation = TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                    // protect against calling when the mouse hasnt really moved.  moving the toolstrip/creating the feedback rect
                    // can cause extra mousemove events, we want to make sure we're not doing all this work
                    // for nothing.
                    if (endLocation != _lastEndLocation)
                    {
                        ToolStripPanelRow.ToolStripPanel.MoveControl(ParentInternal, /*startLocation,*/endLocation);
                        _lastEndLocation = endLocation;
                    }
                    _startLocation = endLocation;
                }
                else
                {
                    // sometimes we dont get mouseup in DT.   Release now.
                    MovingToolStrip = false;
                }
            }

            base.OnMouseMove(mea);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // only switch the cursor if we've got a rafting row.
            if ((ParentInternal != null) && (ToolStripPanelRow != null) && (!ParentInternal.IsInDesignMode))
            {
                _oldCursor = ParentInternal.Cursor;
                ParentInternal.Cursor = Cursors.SizeAll;
            }
            else
            {
                _oldCursor = null;
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (_oldCursor != null && !ParentInternal.IsInDesignMode)
            {
                ParentInternal.Cursor = _oldCursor;
            }
            if (!MovingToolStrip && LeftMouseButtonIsDown())
            {
                MovingToolStrip = true;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            if (MovingToolStrip)
            {
                Point endLocation = TranslatePoint(new Point(mea.X, mea.Y), ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
                ToolStripPanelRow.ToolStripPanel.MoveControl(ParentInternal, /*startLocation,*/endLocation);
            }

            if (!ParentInternal.IsInDesignMode)
            {
                ParentInternal.Cursor = _oldCursor;
            }
            ToolStripPanel.ClearDragFeedback();
            MovingToolStrip = false;
            base.OnMouseUp(mea);
        }

        internal override void ToolStrip_RescaleConstants(int oldDpi, int newDpi) {
            base.RescaleConstantsInternal(newDpi);
            _scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(_defaultPadding, newDpi);
            _scaledGripThickness = DpiHelper.LogicalToDeviceUnits(GripThicknessDefault, newDpi);
            _scaledGripThicknessVisualStylesEnabled = DpiHelper.LogicalToDeviceUnits(GripThicknessVisualStylesEnabled, newDpi);
            this.Margin = DefaultMargin;

            GripThickness = ToolStripManager.VisualStylesEnabled ? _scaledGripThicknessVisualStylesEnabled : _scaledGripThickness;

            OnFontChanged(EventArgs.Empty);
        }

        internal class ToolStripGripAccessibleObject : ToolStripButtonAccessibleObject
        {
            private string stockName;

            public ToolStripGripAccessibleObject(ToolStripGrip owner) : base(owner)
            {
            }

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }
                    if (string.IsNullOrEmpty(stockName))
                    {
                        stockName = SR.ToolStripGripAccessibleName;
                    }
                    return stockName;
                }
                set => base.Name = value;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    return AccessibleRole.Grip;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ThumbControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
