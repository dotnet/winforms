// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The ResizeBehavior is pushed onto the BehaviorStack in response to a positively hit tested SelectionGlyph.
///  The ResizeBehavior simply tracks the MouseMove messages and updates the bounds of the related
///  control based on the new mouse location and the resize Rules.
/// </summary>
internal class ResizeBehavior : Behavior
{
    private struct ResizeComponent
    {
        public Control resizeControl;
        public Rectangle resizeBounds;
        public SelectionRules resizeRules;
    }

    private ResizeComponent[] _resizeComponents;
    private readonly IServiceProvider _serviceProvider;
    private BehaviorService _behaviorService;
    private SelectionRules _targetResizeRules; // rules dictating which sizes we can change
    private Point _initialPoint; // the initial point of the mouse down
    private bool _dragging; // indicates that the behavior is currently 'dragging'
    private bool _pushedBehavior;
    private bool _initialResize; // true for the first resize of the control, false after that.
    private DesignerTransaction _resizeTransaction; // the transaction we create for the resize
    private const int MINSIZE = 10;
    private const int BorderSize = 2;
    private DragAssistanceManager _dragManager; // this object will integrate SnapLines into the resize
    private Point _lastMouseLoc; // helps us avoid re-entering code if the mouse hasn't moved
    private Point _parentLocation; // used to snap resize ops to the grid
    private Size _parentGridSize; // used to snap resize ops to the grid
    private Point _lastMouseAbs; // last absolute mouse position
    private Point _lastSnapOffset; // the last snapoffset we used.
    private bool _didSnap; // did we actually snap.
    private Control _primaryControl; // the primary control the status bar will queue off of

    private Cursor _cursor = Cursors.Default; // used to set the correct cursor during resizing
    private readonly StatusCommandUI _statusCommandUI; // used to update the StatusBar Information.

    private Region _lastResizeRegion;
    private bool _captureLost;

    /// <summary>
    ///  Constructor that caches all values for perf. reasons.
    /// </summary>
    internal ResizeBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dragging = false;
        _pushedBehavior = false;
        _lastSnapOffset = Point.Empty;
        _didSnap = false;
        _statusCommandUI = new StatusCommandUI(serviceProvider);
    }

    /// <summary>
    ///  Demand creates the BehaviorService.
    /// </summary>
    private BehaviorService BehaviorService
    {
        get
        {
            _behaviorService ??= (BehaviorService)_serviceProvider.GetService(typeof(BehaviorService));

            return _behaviorService;
        }
    }

    public override Cursor Cursor
    {
        get
        {
            return _cursor;
        }
    }

    /// <summary>
    ///  Called during the resize operation, we'll try to determine an offset so that
    ///  the controls snap to the grid settings of the parent.
    /// </summary>
    private Rectangle AdjustToGrid(Rectangle controlBounds, SelectionRules rules)
    {
        Rectangle rect = controlBounds;

        if ((rules & SelectionRules.RightSizeable) != 0)
        {
            int xDelta = controlBounds.Right % _parentGridSize.Width;
            if (xDelta > _parentGridSize.Width / 2)
            {
                rect.Width += _parentGridSize.Width - xDelta;
            }
            else
            {
                rect.Width -= xDelta;
            }
        }
        else if ((rules & SelectionRules.LeftSizeable) != 0)
        {
            int xDelta = controlBounds.Left % _parentGridSize.Width;
            if (xDelta > _parentGridSize.Width / 2)
            {
                rect.X += _parentGridSize.Width - xDelta;
                rect.Width -= _parentGridSize.Width - xDelta;
            }
            else
            {
                rect.X -= xDelta;
                rect.Width += xDelta;
            }
        }

        if ((rules & SelectionRules.BottomSizeable) != 0)
        {
            int yDelta = controlBounds.Bottom % _parentGridSize.Height;
            if (yDelta > _parentGridSize.Height / 2)
            {
                rect.Height += _parentGridSize.Height - yDelta;
            }
            else
            {
                rect.Height -= yDelta;
            }
        }
        else if ((rules & SelectionRules.TopSizeable) != 0)
        {
            int yDelta = controlBounds.Top % _parentGridSize.Height;
            if (yDelta > _parentGridSize.Height / 2)
            {
                rect.Y += _parentGridSize.Height - yDelta;
                rect.Height -= _parentGridSize.Height - yDelta;
            }
            else
            {
                rect.Y -= yDelta;
                rect.Height += yDelta;
            }
        }

        // validate our dimensions
        rect.Width = Math.Max(rect.Width, _parentGridSize.Width);
        rect.Height = Math.Max(rect.Height, _parentGridSize.Height);

        return rect;
    }

    /// <summary>
    ///  Builds up an array of snaplines used during resize to adjust/snap the controls bounds.
    /// </summary>
    private SnapLine[] GenerateSnapLines(SelectionRules rules, Point loc)
    {
        List<SnapLine> lines = new(2);
        // the four margins and edges of our control
        if ((rules & SelectionRules.BottomSizeable) != 0)
        {
            lines.Add(new SnapLine(SnapLineType.Bottom, loc.Y - 1));
            if (_primaryControl is not null)
            {
                lines.Add(new SnapLine(SnapLineType.Horizontal, loc.Y + _primaryControl.Margin.Bottom, SnapLine.MarginBottom, SnapLinePriority.Always));
            }
        }
        else if ((rules & SelectionRules.TopSizeable) != 0)
        {
            lines.Add(new SnapLine(SnapLineType.Top, loc.Y));
            if (_primaryControl is not null)
            {
                lines.Add(new SnapLine(SnapLineType.Horizontal, loc.Y - _primaryControl.Margin.Top, SnapLine.MarginTop, SnapLinePriority.Always));
            }
        }

        if ((rules & SelectionRules.RightSizeable) != 0)
        {
            lines.Add(new SnapLine(SnapLineType.Right, loc.X - 1));
            if (_primaryControl is not null)
            {
                lines.Add(new SnapLine(SnapLineType.Vertical, loc.X + _primaryControl.Margin.Right, SnapLine.MarginRight, SnapLinePriority.Always));
            }
        }
        else if ((rules & SelectionRules.LeftSizeable) != 0)
        {
            lines.Add(new SnapLine(SnapLineType.Left, loc.X));
            if (_primaryControl is not null)
            {
                lines.Add(new SnapLine(SnapLineType.Vertical, loc.X - _primaryControl.Margin.Left, SnapLine.MarginLeft, SnapLinePriority.Always));
            }
        }

        return [.. lines];
    }

    /// <summary>
    ///  This is called in response to the mouse moving far enough away from its initial point.
    ///  Basically, we calculate the bounds for each control we're resizing and disable any adorners.
    /// </summary>
    private void InitiateResize()
    {
        bool useSnapLines = BehaviorService.UseSnapLines;
        List<IComponent> components = [];
        // check to see if the current designer participate with SnapLines cache the control bounds
        for (int i = 0; i < _resizeComponents.Length; i++)
        {
            _resizeComponents[i].resizeBounds = _resizeComponents[i].resizeControl.Bounds;
            if (useSnapLines)
            {
                components.Add(_resizeComponents[i].resizeControl);
            }

            if (_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost designerHost)
            {
                if (designerHost.GetDesigner(_resizeComponents[i].resizeControl) is ControlDesigner designer)
                {
                    _resizeComponents[i].resizeRules = designer.SelectionRules;
                }
                else
                {
                    Debug.Fail($"Initiating resize. Could not get the designer for {_resizeComponents[i].resizeControl}");
                    _resizeComponents[i].resizeRules = SelectionRules.None;
                }
            }
        }

        // disable all glyphs in all adorners
        BehaviorService.EnableAllAdorners(false);
        // build up our resize transaction
        IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            string locString;
            if (_resizeComponents.Length == 1)
            {
                string name = TypeDescriptor.GetComponentName(_resizeComponents[0].resizeControl);
                if (name is null || name.Length == 0)
                {
                    name = _resizeComponents[0].resizeControl.GetType().Name;
                }

                locString = string.Format(SR.BehaviorServiceResizeControl, name);
            }
            else
            {
                locString = string.Format(SR.BehaviorServiceResizeControls, _resizeComponents.Length);
            }

            _resizeTransaction = host.CreateTransaction(locString);
        }

        _initialResize = true;
        if (useSnapLines)
        {
            // instantiate our class to manage snap/margin lines...
            _dragManager = new DragAssistanceManager(_serviceProvider, components, true);
        }
        else if (_resizeComponents.Length > 0)
        {
            // try to get the parents grid and snap settings
            if (_resizeComponents[0].resizeControl is Control control && control.Parent is not null)
            {
                PropertyDescriptor snapProp = TypeDescriptor.GetProperties(control.Parent)["SnapToGrid"];
                if (snapProp is not null && (bool)snapProp.GetValue(control.Parent))
                {
                    PropertyDescriptor gridProp = TypeDescriptor.GetProperties(control.Parent)["GridSize"];
                    if (gridProp is not null)
                    {
                        // cache of the gridsize and the location of the parent on the adornerwindow
                        _parentGridSize = (Size)gridProp.GetValue(control.Parent);
                        _parentLocation = _behaviorService.ControlToAdornerWindow(control);
                        _parentLocation.X -= control.Location.X;
                        _parentLocation.Y -= control.Location.Y;
                    }
                }
            }
        }

        _captureLost = false;
    }

    /// <summary>
    ///  In response to a MouseDown, the SelectionBehavior will push (initiate) a dragBehavior by
    ///  alerting the SelectionMananger that a new control has been selected and the mouse is down.
    ///  Note that this is only if we find the related control's Dock property == none.
    /// </summary>
    public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
    {
        // we only care about the right mouse button for resizing
        if (button != MouseButtons.Left)
        {
            // pass any other mouse click along - unless we've already started our resize in which case we'll ignore it
            return _pushedBehavior;
        }

        // start with no selection rules and try to obtain this info from the glyph
        _targetResizeRules = SelectionRules.None;
        if (g is SelectionGlyphBase sgb)
        {
            _targetResizeRules = sgb.SelectionRules;
            _cursor = sgb.HitTestCursor;
        }

        if (_targetResizeRules == SelectionRules.None)
        {
            return false;
        }

        ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
        if (selSvc is null)
        {
            return false;
        }

        _initialPoint = mouseLoc;
        _lastMouseLoc = mouseLoc;
        // build up a list of our selected controls
        _primaryControl = selSvc.PrimarySelection as Control;

        // Since we don't know exactly how many valid objects we are going to have we use this temp
        List<Control> components = [];
        foreach (object component in selSvc.GetSelectedComponents())
        {
            if (component is Control control)
            {
                // don't drag locked controls
                PropertyDescriptor prop = TypeDescriptor.GetProperties(control)["Locked"];
                if (prop is not null)
                {
                    if ((bool)prop.GetValue(control))
                    {
                        continue;
                    }
                }

                components.Add(control);
            }
        }

        if (components.Count == 0)
        {
            return false;
        }

        _resizeComponents = new ResizeComponent[components.Count];
        for (int i = 0; i < components.Count; i++)
        {
            _resizeComponents[i].resizeControl = components[i];
        }

        // push this resizebehavior
        _pushedBehavior = true;
        BehaviorService.PushCaptureBehavior(this);
        return false;
    }

    /// <summary>
    ///  This method is called when we lose capture, which can occur when another window requests capture or
    ///  the user presses ESC during a drag. We check to see if we are currently dragging,
    ///  and if we are we abort the transaction. We pop our behavior off the stack at this time.
    /// </summary>
    public override void OnLoseCapture(Glyph g, EventArgs e)
    {
        _captureLost = true;
        if (_pushedBehavior)
        {
            _pushedBehavior = false;
            Debug.Assert(BehaviorService is not null, "We should have a behavior service.");
            if (BehaviorService is not null)
            {
                if (_dragging)
                {
                    _dragging = false;
                    // make sure we get rid of the selection rectangle
                    for (int i = 0; !_captureLost && i < _resizeComponents.Length; i++)
                    {
                        Control control = _resizeComponents[i].resizeControl;
                        Rectangle borderRect = BehaviorService.ControlRectInAdornerWindow(control);
                        if (!borderRect.IsEmpty)
                        {
                            using Graphics graphics = BehaviorService.AdornerWindowGraphics;
                            graphics.SetClip(borderRect);
                            using (Region newRegion = new(borderRect))
                            {
                                newRegion.Exclude(Rectangle.Inflate(borderRect, -BorderSize, -BorderSize));
                                BehaviorService.Invalidate(newRegion);
                            }

                            graphics.ResetClip();
                        }
                    }

                    // re-enable all glyphs in all adorners
                    BehaviorService.EnableAllAdorners(true);
                }

                BehaviorService.PopBehavior(this);

                if (_lastResizeRegion is not null)
                {
                    BehaviorService.Invalidate(_lastResizeRegion); // might be the same, might not.
                    _lastResizeRegion.Dispose();
                    _lastResizeRegion = null;
                }
            }
        }

        Debug.Assert(!_dragging, "How can we be dragging without pushing a behavior?");
        // If we still have a transaction, roll it back.
        if (_resizeTransaction is not null)
        {
            DesignerTransaction t = _resizeTransaction;
            _resizeTransaction = null;
            using (t)
            {
                t.Cancel();
            }
        }
    }

    internal static int AdjustPixelsForIntegralHeight(Control control, int pixelsMoved)
    {
        PropertyDescriptor propIntegralHeight = TypeDescriptor.GetProperties(control)["IntegralHeight"];
        if (propIntegralHeight is not null)
        {
            object value = propIntegralHeight.GetValue(control);
            if (value is bool boolValue && boolValue)
            {
                PropertyDescriptor propItemHeight = TypeDescriptor.GetProperties(control)["ItemHeight"];
                if (propItemHeight is not null)
                {
                    if (pixelsMoved >= 0)
                    {
                        return pixelsMoved - (pixelsMoved % (int)propItemHeight.GetValue(control));
                    }
                    else
                    {
                        int integralHeight = (int)propItemHeight.GetValue(control);
                        return pixelsMoved - (integralHeight - (Math.Abs(pixelsMoved) % integralHeight));
                    }
                }
            }
        }

        // if the control does not have the IntegralHeight property, then the pixels moved are fine
        return pixelsMoved;
    }

    /// <summary>
    ///  This method will either initiate a new resize operation or continue with an existing one.
    ///  If we're currently dragging (i.e. resizing) then we look at the resize rules and set the
    ///  bounds of each control to the new location of the mouse pointer.
    /// </summary>
    public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
    {
        if (!_pushedBehavior)
        {
            return false;
        }

        bool altKeyPressed = Control.ModifierKeys == Keys.Alt;
        if (altKeyPressed && _dragManager is not null)
        {
            // erase any snaplines (if we had any)
            _dragManager.EraseSnapLines();
        }

        if (!altKeyPressed && mouseLoc.Equals(_lastMouseLoc))
        {
            return true;
        }

        // When DesignerWindowPane has scrollbars and we resize, shrinking the DesignerWindowPane
        // makes it look like the mouse has moved to the BS.
        // To compensate for that we keep track of the mouse's previous position in screen coordinates,
        // and use that to compare if the mouse has really moved.
        if (_lastMouseAbs != Point.Empty)
        {
            Point mouseLocAbs = new(mouseLoc.X, mouseLoc.Y);
            PInvoke.ClientToScreen(_behaviorService.AdornerWindowControl, ref mouseLocAbs);
            if (mouseLocAbs.X == _lastMouseAbs.X && mouseLocAbs.Y == _lastMouseAbs.Y)
            {
                return true;
            }
        }

        if (!_dragging)
        {
            if (Math.Abs(_initialPoint.X - mouseLoc.X) > DesignerUtils.MinDragSize.Width / 2 || Math.Abs(_initialPoint.Y - mouseLoc.Y) > DesignerUtils.MinDragSize.Height / 2)
            {
                InitiateResize();
                _dragging = true;
            }
            else
            {
                return false;
            }
        }

        if (_resizeComponents is null || _resizeComponents.Length == 0)
        {
            return false;
        }

        // we do these separately so as not to disturb the cached sizes for values we're not actually changing.
        // For example, if a control is docked top and we modify the height, the width shouldn't be modified.
        PropertyDescriptor propWidth = null;
        PropertyDescriptor propHeight = null;
        PropertyDescriptor propTop = null;
        PropertyDescriptor propLeft = null;

        // We do this to make sure that Undo works correctly.
        if (_initialResize)
        {
            propWidth = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Width"];
            propHeight = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Height"];
            propTop = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Top"];
            propLeft = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Left"];

            // validate each of the property descriptors.
            if (propWidth is not null && !typeof(int).IsAssignableFrom(propWidth.PropertyType))
            {
                propWidth = null;
            }

            if (propHeight is not null && !typeof(int).IsAssignableFrom(propHeight.PropertyType))
            {
                propHeight = null;
            }

            if (propTop is not null && !typeof(int).IsAssignableFrom(propTop.PropertyType))
            {
                propTop = null;
            }

            if (propLeft is not null && !typeof(int).IsAssignableFrom(propLeft.PropertyType))
            {
                propLeft = null;
            }
        }

        Control targetControl = _resizeComponents[0].resizeControl;
        _lastMouseLoc = mouseLoc;
        _lastMouseAbs = new Point(mouseLoc.X, mouseLoc.Y);
        PInvoke.ClientToScreen(_behaviorService.AdornerWindowControl, ref _lastMouseAbs);
        int minHeight = Math.Max(targetControl.MinimumSize.Height, MINSIZE);
        int minWidth = Math.Max(targetControl.MinimumSize.Width, MINSIZE);
        if (_dragManager is not null)
        {
            bool shouldSnap = true;
            bool shouldSnapHorizontally = true;
            // if the targetcontrol is at min-size then we do not want to offer up snaplines
            if ((((_targetResizeRules & SelectionRules.BottomSizeable) != 0) || ((_targetResizeRules & SelectionRules.TopSizeable) != 0)) &&
                (targetControl.Height == minHeight))
            {
                shouldSnap = false;
            }
            else if ((((_targetResizeRules & SelectionRules.RightSizeable) != 0) || ((_targetResizeRules & SelectionRules.LeftSizeable) != 0)) &&
                (targetControl.Width == minWidth))
            {
                shouldSnap = false;
            }

            // if the targetControl has IntegralHeight turned on, then don't snap if the control can be resized vertically
            PropertyDescriptor propIntegralHeight = TypeDescriptor.GetProperties(targetControl)["IntegralHeight"];
            if (propIntegralHeight is not null)
            {
                object value = propIntegralHeight.GetValue(targetControl);
                if (value is bool boolValue && boolValue)
                {
                    shouldSnapHorizontally = false;
                }
            }

            if (!altKeyPressed && shouldSnap)
            {
                // here, ask the snapline engine to suggest an offset during our resize
                // Remembering the last snapoffset allows us to correctly erase snaplines,
                // if the user subsequently holds down the Alt-Key. Remember that we don't physically move the mouse,
                // we move the control. So if we didn't remember the last snapoffset and the user then hit the Alt-Key,
                // we would actually redraw the control at the actual mouse location,
                // which would make the control "jump" which is not what the user would expect.
                // Why does the control "jump"? Because when a control is snapped,
                // we have offset the control relative to where the mouse is,
                // but we have not update the physical mouse position.
                // When the user hits the Alt-Key they expect the control to be where it was (whether snapped or not).
                // we can't rely on lastSnapOffset to check whether we snapped. We used to check if it was empty,
                // but it can be empty and we still snapped (say the control was snapped,
                // as you continue to move the mouse, it will stay snapped for a while.
                // During that while the snapoffset will got from x to -x (or vice versa) and a one point hit 0.
                // Since we have to calculate the new size/location differently based on whether we snapped or not,
                // we have to know for sure if we snapped. We do different math because of bug 264996:
                //  - if you snap, we want to move the control edge.
                //  - otherwise, we just want to change the size by the number of pixels moved.
                _lastSnapOffset = _dragManager.OnMouseMove(targetControl, GenerateSnapLines(_targetResizeRules, mouseLoc), ref _didSnap, shouldSnapHorizontally);
            }
            else
            {
                /*just an invalid rect - so we won't snap*/// );
                _dragManager.OnMouseMove(new Rectangle(-100, -100, 0, 0));
            }

            // If there's a line to snap to, the offset will come back non-zero.
            // In that case we should adjust the mouse position with the offset such that
            // the size calculation below takes that offset into account. If there's no line,
            // then the offset is 0, and there's no harm in adding the offset.
            mouseLoc.X += _lastSnapOffset.X;
            mouseLoc.Y += _lastSnapOffset.Y;
        }

        // IF WE ARE SNAPPING TO A CONTROL, then we also need to adjust for the offset between the
        // initialPoint (where the MouseDown happened) and the edge of the control otherwise we
        // would be those pixels off when resizing the control. Remember that snaplines are based on the targetControl,
        // so we need to use the targetControl to figure out the offset.
        Rectangle controlBounds = new(_resizeComponents[0].resizeBounds.X, _resizeComponents[0].resizeBounds.Y,
                                                  _resizeComponents[0].resizeBounds.Width, _resizeComponents[0].resizeBounds.Height);
        if ((_didSnap) && (targetControl.Parent is not null))
        {
            controlBounds.Location = _behaviorService.MapAdornerWindowPoint(targetControl.Parent.Handle, controlBounds.Location);
            if (targetControl.Parent.IsMirrored)
            {
                controlBounds.Offset(-controlBounds.Width, 0);
            }
        }

        Rectangle newBorderRect = Rectangle.Empty;
        Rectangle targetBorderRect = Rectangle.Empty;
        bool drawSnapline = true;
        Color backColor = targetControl.Parent is not null ? targetControl.Parent.BackColor : Color.Empty;
        for (int i = 0; i < _resizeComponents.Length; i++)
        {
            Control control = _resizeComponents[i].resizeControl;
            Rectangle bounds = control.Bounds;
            Rectangle oldBounds = bounds;
            // We need to compute the offset based on the original cached Bounds ...
            // ListBox doesn't allow drag on the top boundary if this is not done when it is "IntegralHeight"
            Rectangle baseBounds = _resizeComponents[i].resizeBounds;
            Rectangle oldBorderRect = BehaviorService.ControlRectInAdornerWindow(control);
            bool needToUpdate = true;
            // The ResizeBehavior can easily get into a situation where we are fighting with a layout engine.
            // E.g., We resize control to 50px, LayoutEngine lays out and finds 50px was too small
            // and resized back to 100px. This is what should happen, but it looks bad in the designer.
            // To avoid the flicker we temporarily turn off painting while we do the resize.
            PInvokeCore.SendMessage(control, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)false);
            try
            {
                bool fRTL = false;
                // If the container is mirrored the control origin is in upper-right,
                // so we need to adjust our math for that.
                // Remember that mouse coords have origin in upper left.
                if (control.Parent is not null && control.Parent.IsMirrored)
                {
                    fRTL = true;
                }

                // figure out which ones we're actually changing so we don't blow away the controls cached sizing state.
                // This is important if things are docked we don't want to destroy their "pre-dock" size.
                BoundsSpecified specified = BoundsSpecified.None;
                // When we check if we should change height, width, location, we first have to check
                // if the targetControl allows resizing, and then if the control we are currently resizin
                // g allows it as well.
                SelectionRules resizeRules = _resizeComponents[i].resizeRules;
                if (((_targetResizeRules & SelectionRules.BottomSizeable) != 0) &&
                    ((resizeRules & SelectionRules.BottomSizeable) != 0))
                {
                    int pixelHeight;
                    if (_didSnap)
                    {
                        pixelHeight = mouseLoc.Y - controlBounds.Bottom;
                    }
                    else
                    {
                        pixelHeight = AdjustPixelsForIntegralHeight(control, mouseLoc.Y - _initialPoint.Y);
                    }

                    bounds.Height = Math.Max(minHeight, baseBounds.Height + pixelHeight);
                    specified |= BoundsSpecified.Height;
                }

                if (((_targetResizeRules & SelectionRules.TopSizeable) != 0) &&
                    ((resizeRules & SelectionRules.TopSizeable) != 0))
                {
                    int yOffset;
                    if (_didSnap)
                    {
                        yOffset = controlBounds.Y - mouseLoc.Y;
                    }
                    else
                    {
                        yOffset = AdjustPixelsForIntegralHeight(control, _initialPoint.Y - mouseLoc.Y);
                    }

                    specified |= BoundsSpecified.Height;
                    bounds.Height = Math.Max(minHeight, baseBounds.Height + yOffset);
                    if ((bounds.Height != minHeight) ||
                         ((bounds.Height == minHeight) && (oldBounds.Height != minHeight)))
                    {
                        specified |= BoundsSpecified.Y;
                        // if you do it fast enough, we actually could end up placing the control off the parent
                        // (say off the form), so enforce a "minimum" location
                        bounds.Y = Math.Min(baseBounds.Bottom - minHeight, baseBounds.Y - yOffset);
                    }
                }

                if (((((_targetResizeRules & SelectionRules.RightSizeable) != 0) && ((resizeRules & SelectionRules.RightSizeable) != 0)) && (!fRTL)) ||
                   ((((_targetResizeRules & SelectionRules.LeftSizeable) != 0) && ((resizeRules & SelectionRules.LeftSizeable) != 0)) && (fRTL)))
                {
                    specified |= BoundsSpecified.Width;
                    int xOffset = _initialPoint.X;
                    if (_didSnap)
                    {
                        xOffset = !fRTL ? controlBounds.Right : controlBounds.Left;
                    }

                    bounds.Width = Math.Max(minWidth, baseBounds.Width + (!fRTL ? (mouseLoc.X - xOffset) : (xOffset - mouseLoc.X)));
                }

                if (((((_targetResizeRules & SelectionRules.RightSizeable) != 0) && ((resizeRules & SelectionRules.RightSizeable) != 0)) && (fRTL)) ||
                   ((((_targetResizeRules & SelectionRules.LeftSizeable) != 0) && ((resizeRules & SelectionRules.LeftSizeable) != 0)) && (!fRTL)))
                {
                    specified |= BoundsSpecified.Width;
                    int xPos = _initialPoint.X;
                    if (_didSnap)
                    {
                        xPos = !fRTL ? controlBounds.Left : controlBounds.Right;
                    }

                    int xOffset = !fRTL ? (xPos - mouseLoc.X) : (mouseLoc.X - xPos);
                    bounds.Width = Math.Max(minWidth, baseBounds.Width + xOffset);
                    if ((bounds.Width != minWidth) ||
                         ((bounds.Width == minWidth) && (oldBounds.Width != minWidth)))
                    {
                        specified |= BoundsSpecified.X;
                        // if you do it fast enough, we actually could end up placing the control off the parent
                        // (say off the form), so enforce a "minimum" location
                        bounds.X = Math.Min(baseBounds.Right - minWidth, baseBounds.X - xOffset);
                    }
                }

                if (!_parentGridSize.IsEmpty)
                {
                    bounds = AdjustToGrid(bounds, _targetResizeRules);
                }

                // Checking specified (check the diff) rather than bounds.<foo> != resizeBounds[i].<foo>
                // also handles the following corner cases:
                // 1. Create a form and add 2 buttons. Make sure that they are snapped to the left edge.
                //    Now grab the left edge of button 1, and start resizing to the left,
                //    past the snapline you will initially get, and then back to the right.
                //    What you would expect is to get the left edge snapline again.
                //    But without the specified check you wouldn't.
                //    This is because the bounds.<foo> != resizeBounds[i].<foo> checks would fail,
                //    since the new size would now be the original size.
                //    We could probably live with that, except that we draw the snapline below,
                //    since we correctly identified one. We could hack it so that we didn't draw the snapline,
                //    but that would confuse the user even more.
                // 2. Create a form and add a single button. Place it at 100,100.
                //     Now start resizing it to the left and then back to the right.
                // Note that with the original check (see diff), you would never be able to
                // resize it back to position 100,100. You would get to 99,100 and then to 101,100.
                if (((specified & BoundsSpecified.Width) == BoundsSpecified.Width) &&
                    _dragging && _initialResize && propWidth is not null)
                {
                    propWidth.SetValue(_resizeComponents[i].resizeControl, bounds.Width);
                }

                if (((specified & BoundsSpecified.Height) == BoundsSpecified.Height) &&
                    _dragging && _initialResize && propHeight is not null)
                {
                    propHeight.SetValue(_resizeComponents[i].resizeControl, bounds.Height);
                }

                if (((specified & BoundsSpecified.X) == BoundsSpecified.X) &&
                    _dragging && _initialResize && propLeft is not null)
                {
                    propLeft.SetValue(_resizeComponents[i].resizeControl, bounds.X);
                }

                if (((specified & BoundsSpecified.Y) == BoundsSpecified.Y) &&
                    _dragging && _initialResize && propTop is not null)
                {
                    propTop.SetValue(_resizeComponents[i].resizeControl, bounds.Y);
                }

                // We check the dragging bit here at every turn,
                // because if there was a popup we may have lost capture and we are terminated.
                // At that point we shouldn't make any changes.
                if (_dragging)
                {
                    control.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);
                    // Get the new resize border
                    newBorderRect = BehaviorService.ControlRectInAdornerWindow(control);
                    if (control.Equals(targetControl))
                    {
                        Debug.Assert(i == 0, "The first control in the Selection should be the target control");
                        targetBorderRect = newBorderRect;
                    }

                    // Check that the control really did resize itself.
                    // Some controls (like ListBox, MonthCalendar)
                    // might adjust to a slightly different size than the one we pass in SetBounds.
                    // If if didn't size, then there's no need to invalidate anything
                    if (control.Bounds == oldBounds)
                    {
                        needToUpdate = false;
                    }

                    // We would expect the bounds now to be what we set it to above,
                    // but this might not be the case. If the control is hosted with e.g. a FLP,
                    // then setting the bounds above actually might force a re-layout,
                    // and the control will get moved to another spot. In this case,
                    // we don't really want to draw a snapline.
                    // Even if we snapped to a snapline, if the control got moved,
                    // the snapline would be in the wrong place.
                    if (control.Bounds != bounds)
                    {
                        drawSnapline = false;
                    }
                }

                if (control == _primaryControl && _statusCommandUI is not null)
                {
                    _statusCommandUI.SetStatusInformation(control);
                }
            }
            finally
            {
                // While we were resizing we discarded painting messages to reduce flicker.
                // We now turn painting back on and manually refresh the controls.
                PInvokeCore.SendMessage(control, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)true);
                // update the control
                if (needToUpdate)
                {
                    Control parent = control.Parent;
                    if (parent is not null)
                    {
                        control.Invalidate(/* invalidateChildren = */ true);
                        parent.Invalidate(oldBounds, /* invalidateChildren = */ true);
                        parent.Update();
                    }
                    else
                    {
                        control.Refresh();
                    }
                }

                // render the resize border
                if (!newBorderRect.IsEmpty)
                {
                    using Region newRegion = new(newBorderRect);
                    newRegion.Exclude(Rectangle.Inflate(newBorderRect, -BorderSize, -BorderSize));
                    // No reason to get smart about only invalidating part of the border.
                    // Thought we could be but no.The reason is the order: ...
                    // the new border is drawn (last resize) On next mousemove,
                    // the control is resized which redraws the control AND ERASES THE BORDER
                    // Then we draw the new border - flash baby. Thus this will always flicker.
                    if (needToUpdate)
                    {
                        using Region oldRegion = new(oldBorderRect);
                        oldRegion.Exclude(Rectangle.Inflate(oldBorderRect, -BorderSize, -BorderSize));
                        BehaviorService.Invalidate(oldRegion);
                    }

                    // draw the new border captureLost could be true if a popup came up and caused a lose focus
                    if (!_captureLost)
                    {
                        using (Graphics graphics = BehaviorService.AdornerWindowGraphics)
                        {
                            if (_lastResizeRegion is not null)
                            {
                                if (!_lastResizeRegion.Equals(newRegion, graphics))
                                {
                                    _lastResizeRegion.Exclude(newRegion); // we don't want to invalidate this region.
                                    BehaviorService.Invalidate(_lastResizeRegion); // might be the same, might not.
                                    _lastResizeRegion.Dispose();
                                    _lastResizeRegion = null;
                                }
                            }

                            DesignerUtils.DrawResizeBorder(graphics, newRegion, backColor);
                        }

                        _lastResizeRegion ??= newRegion.Clone(); // we will need to dispose it later.
                    }
                }
            }
        }

        if ((drawSnapline) && (!altKeyPressed) && (_dragManager is not null))
        {
            _dragManager.RenderSnapLinesInternal(targetBorderRect);
        }

        _initialResize = false;
        return true;
    }

    /// <summary>
    ///  This ends the Behavior by popping itself from the BehaviorStack.
    ///  Also, all Adorners are re-enabled at the end of a successful drag.
    /// </summary>
    public override bool OnMouseUp(Glyph g, MouseButtons button)
    {
        try
        {
            if (_dragging)
            {
                if (_dragManager is not null)
                {
                    _dragManager.OnMouseUp();
                    _dragManager = null;
                    _lastSnapOffset = Point.Empty;
                    _didSnap = false;
                }

                if (_resizeComponents is not null && _resizeComponents.Length > 0)
                {
                    // we do these separately so as not to disturb the cached sizes for values
                    // we're not actually changing. For example, if a control is docked top and
                    // we modify the height, the width shouldn't be modified.
                    PropertyDescriptor propWidth = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Width"];
                    PropertyDescriptor propHeight = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Height"];
                    PropertyDescriptor propTop = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Top"];
                    PropertyDescriptor propLeft = TypeDescriptor.GetProperties(_resizeComponents[0].resizeControl)["Left"];
                    for (int i = 0; i < _resizeComponents.Length; i++)
                    {
                        if (propWidth is not null && _resizeComponents[i].resizeControl.Width != _resizeComponents[i].resizeBounds.Width)
                        {
                            propWidth.SetValue(_resizeComponents[i].resizeControl, _resizeComponents[i].resizeControl.Width);
                        }

                        if (propHeight is not null && _resizeComponents[i].resizeControl.Height != _resizeComponents[i].resizeBounds.Height)
                        {
                            propHeight.SetValue(_resizeComponents[i].resizeControl, _resizeComponents[i].resizeControl.Height);
                        }

                        if (propTop is not null && _resizeComponents[i].resizeControl.Top != _resizeComponents[i].resizeBounds.Y)
                        {
                            propTop.SetValue(_resizeComponents[i].resizeControl, _resizeComponents[i].resizeControl.Top);
                        }

                        if (propLeft is not null && _resizeComponents[i].resizeControl.Left != _resizeComponents[i].resizeBounds.X)
                        {
                            propLeft.SetValue(_resizeComponents[i].resizeControl, _resizeComponents[i].resizeControl.Left);
                        }

                        if (_resizeComponents[i].resizeControl == _primaryControl && _statusCommandUI is not null)
                        {
                            _statusCommandUI.SetStatusInformation(_primaryControl);
                        }
                    }
                }
            }

            if (_resizeTransaction is not null)
            {
                DesignerTransaction t = _resizeTransaction;
                _resizeTransaction = null;
                using (t)
                {
                    t.Commit();
                }
            }
        }
        finally
        {
            // This pops us off the stack, re-enables adorners and clears the "dragging" flag.
            OnLoseCapture(g, EventArgs.Empty);
        }

        return false;
    }
}
