// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

internal partial class ToolStripGrip : ToolStripButton
{
    private Cursor? _oldCursor;
    private Point _startLocation = Point.Empty;
    private bool _movingToolStrip;
    private Point _lastEndLocation = ToolStrip.s_invalidMouseEnter;
    private static Size s_dragSize = LayoutUtils.s_maxSize;

    private Padding _defaultPadding;

    internal ToolStripGrip()
    {
        ScaleConstants(ScaleHelper.InitialSystemDpi);
        SupportsItemClick = false;
    }

    protected internal override Padding DefaultMargin => _defaultPadding;

    public override bool CanSelect => false;

    internal int GripThickness { get; private set; }

    [MemberNotNullWhen(true, nameof(ToolStripPanelRow))]
    internal bool MovingToolStrip
    {
        get => (ToolStripPanelRow is not null) && _movingToolStrip;
        set
        {
            if ((_movingToolStrip != value) && ParentInternal is not null)
            {
                if (value)
                {
                    // don't let grips move the toolstrip
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

    private ToolStripPanelRow? ToolStripPanelRow
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
        if (ParentInternal is not null)
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

    private static bool LeftMouseButtonIsDown()
    {
        return (Control.MouseButtons == MouseButtons.Left) && (Control.ModifierKeys == Keys.None);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // all the grip painting should be on the ToolStrip itself.
        ParentInternal?.OnPaintGrip(e);
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

            if (s_dragSize == LayoutUtils.s_maxSize)
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
                // protect against calling when the mouse hasn't really moved. Moving the toolstrip/creating the feedback rect
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
                // sometimes we don't get mouseup in DT. Release now.
                MovingToolStrip = false;
            }
        }

        base.OnMouseMove(mea);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        // only switch the cursor if we've got a rafting row.
        if ((ParentInternal is not null) && (ToolStripPanelRow is not null) && (!ParentInternal.IsInDesignMode))
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

    protected override void OnMouseLeave(EventArgs e)
    {
        if (_oldCursor is not null && ParentInternal is not null && !ParentInternal.IsInDesignMode)
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

        if (ParentInternal is not null && !ParentInternal.IsInDesignMode)
        {
            ParentInternal.Cursor = _oldCursor;
        }

        ToolStripPanel.ClearDragFeedback();
        MovingToolStrip = false;
        base.OnMouseUp(mea);
    }

    internal override void ToolStrip_RescaleConstants(int oldDpi, int newDpi)
    {
        RescaleConstantsInternal(newDpi);
        ScaleConstants(newDpi);
        Margin = DefaultMargin;
        OnFontChanged(EventArgs.Empty);
    }

    private void ScaleConstants(int dpi)
    {
        const int LogicalDefaultPadding = 2;
        const int LogicalGripThickness = 3;
        const int LogicalGripThicknessVisualStylesEnabled = 5;

        _defaultPadding = new(ScaleHelper.ScaleToDpi(LogicalDefaultPadding, dpi));
        GripThickness = ScaleHelper.ScaleToDpi(
            ToolStripManager.VisualStylesEnabled ? LogicalGripThicknessVisualStylesEnabled : LogicalGripThickness,
            dpi);
    }
}
