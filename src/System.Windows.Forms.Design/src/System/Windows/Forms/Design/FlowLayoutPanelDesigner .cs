// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable warnings

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the <see cref="Forms.FlowLayoutPanel"/>
///  control. Basically, this designer carefully watches drag operations. During a drag, we attempt to
///  draw an "I" bar for insertion/feedback purposes. When a control is added to our designer, we check
///  some cached state to see if we believe that it needs to be inserted at a particular index. If
///  so, we re-insert the control appropriately.
/// </summary>
internal partial class FlowLayoutPanelDesigner : FlowPanelDesigner
{
    private ChildInfo[] _childInfo;

    /// <summary>
    ///  The controls that are actually being dragged -- used for an internal drag.
    /// </summary>
    private List<Control> _dragControls;

    private Control _primaryDragControl;
    private Point _lastMouseLocation;

    /// <summary>
    ///  Store the maximum height/width of each row/column.
    /// </summary>
    private readonly List<(int Min, int Max, int Size, int LastIndex)> _commonSizes = [];

    private const int InvalidIndex = -1;

    /// <summary>
    ///  The index which we will re-insert a newly added child.
    /// </summary>
    private int _insertionIndex = InvalidIndex;

    /// <summary>
    ///  Tracks the top or left last rendered I-bar location.
    /// </summary>
    private Point _oldPoint1 = Point.Empty;

    /// <summary>
    ///  Tracks the bottom or right last rendered I-bar location.
    /// </summary>
    private Point _oldPoint2 = Point.Empty;

    /// <summary>
    ///  If space for IBar is less than or equal to minIBar we draw a simple IBar.
    /// </summary>
    private const int MinIBar = 10;
    private const int IBarHatHeight = 3;
    private const int IBarSpace = 2;
    private const int IBarHatWidth = 5;
    private const int IBarHalfSize = 2;

    private const int IBarLineOffset = IBarHatHeight + IBarSpace;

    /// <summary>
    ///  Since we don't always know which IBar we are going draw we want to invalidate max area.
    /// </summary>
    private readonly int _maxIBarWidth = Math.Max(IBarHalfSize, (IBarHatWidth - 1) / 2);

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // If the FLP is InheritedReadOnly, so should be all of the children.
        if (IsInheritedReadOnly)
        {
            foreach (object child in Control.Controls)
            {
                TypeDescriptor.AddAttributes(child, InheritanceAttribute.InheritedReadOnly);
            }
        }
    }

    private FlowLayoutPanel FlowLayoutPanel => Control;

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        PropertyDescriptor flowDirection = (PropertyDescriptor)properties["FlowDirection"];

        if (flowDirection is not null)
        {
            properties["FlowDirection"] = TypeDescriptor.CreateProperty(typeof(FlowLayoutPanelDesigner), flowDirection, []);
        }
    }

    /// <summary>
    ///  This is called to check whether the z-order of dragged controls should be maintained when dropped on a
    ///  ParentControlDesigner. By default it will, but e.g. FlowLayoutPanelDesigner wants to do its own z-ordering.
    ///
    ///  If this returns true, then the DropSourceBehavior will attempt to set the index of the controls being
    ///  dropped to preserve the original order (in the dragSource). If it returns false, the index will not
    ///  be set.
    ///
    ///  If this is set to false, then the DropSourceBehavior will not treat a drag as a local drag even
    ///  if the dragSource and the dragTarget are the same. This will allow a ParentControlDesigner to hook
    ///  OnChildControlAdded to set the right child index, since in this case, the control(s) being dragged
    ///  will be removed from the dragSource and then added to the dragTarget.
    /// </summary>
    protected internal override bool AllowSetChildIndexOnDrop => false;

    protected override bool AllowGenericDragBox => false;

    /// <summary>
    ///  Returns true if flow direction is right-to-left or left-to-right
    /// </summary>
    private bool HorizontalFlow =>
        FlowLayoutPanel.FlowDirection is FlowDirection.RightToLeft or FlowDirection.LeftToRight;

    /// <summary>
    ///  Get and cache the selection service
    /// </summary>
    internal ISelectionService SelectionService => GetService<ISelectionService>();

    private FlowDirection RTLTranslateFlowDirection(FlowDirection direction)
    {
        return !IsRtl
            ? direction
            : direction switch
            {
                FlowDirection.LeftToRight => FlowDirection.RightToLeft,
                FlowDirection.RightToLeft => FlowDirection.LeftToRight,
                FlowDirection.TopDown or FlowDirection.BottomUp => direction,
                _ => direction,
            };
    }

    private bool IsRtl => Control.RightToLeft == RightToLeft.Yes;

    /// <summary>
    ///  Returns a Rectangle representing the margin bounds of the control.
    /// </summary>
    private Rectangle GetMarginBounds(Control control)
    {
        // If the FLP is RightToLeft.Yes, then the values of Right and Left margins are swapped,
        // account for that here.
        var bounds = control.Bounds;
        var margin = control.Margin;

        return new Rectangle(
            bounds.Left - (IsRtl ? margin.Right : margin.Left),
            bounds.Top - margin.Top,
            bounds.Width + margin.Horizontal,
            bounds.Height + margin.Vertical);
    }

    /// <summary>
    ///  Called when we receive a DragEnter notification - here we attempt to cache child position and information
    ///  intended to be used by drag move and drop messages. Basically we pass through the children twice - first
    ///  we build up an array of rectangles representing the children bounds (w/margins) and identify where the row/
    ///  column changes are. Secondly, we normalize the child rectangles so that children in each row/column are the
    ///  same height/width;
    /// </summary>
    private void CreateMarginBoundsList()
    {
        _commonSizes.Clear();

        var children = Control.Controls;
        if (children.Count == 0)
        {
            _childInfo = [];
            return;
        }

        // This will cache row/column placement and alignment info for the children.
        _childInfo = new ChildInfo[children.Count];

        FlowDirection flowDirection = RTLTranslateFlowDirection(FlowLayoutPanel.FlowDirection);
        bool horizontalFlow = HorizontalFlow;

        int currentMinTopLeft = int.MaxValue;
        int currentMaxBottomRight = -1;
        int lastOffset = -1;

        if ((horizontalFlow && flowDirection == FlowDirection.RightToLeft) ||
            (!horizontalFlow && flowDirection == FlowDirection.BottomUp))
        {
            lastOffset = int.MaxValue;
        }

        Point offset = Control.PointToScreen(Point.Empty);
        int i;

        // Pass 1 - store off the original margin rectangles & identify row/column sizes
        for (i = 0; i < children.Count; i++)
        {
            var currentControl = children[i];
            var marginBounds = GetMarginBounds(currentControl);
            var bounds = currentControl.Bounds;
            var margin = currentControl.Margin;

            // Fix up bounds such that the IBar is not drawn right on top of the control.
            if (horizontalFlow)
            {
                // Difference between bounds and rect is that we do not adjust top, bottom, height
                // If the FLP is RightToLeft.Yes, then the values of Right and Left margins are swapped, account for that here.
                bounds.X -= IsRtl ? margin.Right : margin.Left;
                // To draw correctly in dead areas
                bounds.Width += margin.Horizontal;
                // We want the IBar to stop at the very edge of the control. Offset height
                // by 1 pixel to ensure that. This is the classic - how many pixels to draw when you
                // draw from Bounds.Top to Bounds.Bottom.
                bounds.Height -= 1;
            }
            else
            {
                // Difference between bounds and rect is that we do not adjust left, right, width
                bounds.Y -= margin.Top;
                // To draw correctly in dead areas
                bounds.Height += margin.Vertical;
                // We want the IBar to stop at the very edge of the control. Offset width
                // by 1 pixel to ensure that. This is the classic - how many pixels to draw when you
                // draw from Bounds.Left to Bounds.Right.
                bounds.Width -= 1;
            }

            // Convert to screen coordinates.
            marginBounds.Offset(offset.X, offset.Y);
            bounds.Offset(offset.X, offset.Y);

            _childInfo[i].MarginBounds = marginBounds;
            _childInfo[i].ControlBounds = bounds;
            _childInfo[i].InSelectionCollection = _dragControls?.Contains(currentControl) == true;

            if (horizontalFlow)
            {
                // Identify a new row.
                if (flowDirection == FlowDirection.LeftToRight ? marginBounds.X < lastOffset : marginBounds.X > lastOffset)
                {
                    Debug.Assert(currentMinTopLeft > 0 && currentMaxBottomRight > 0, "How can we not have a min/max value?");
                    if (currentMinTopLeft > 0 && currentMaxBottomRight > 0)
                    {
                        _commonSizes.Add((
                            currentMinTopLeft,
                            currentMaxBottomRight,
                            currentMaxBottomRight - currentMinTopLeft,
                            i));
                        currentMinTopLeft = int.MaxValue;
                        currentMaxBottomRight = -1;
                    }
                }

                lastOffset = marginBounds.X;

                // Be sure to track the largest row size.
                if (marginBounds.Top < currentMinTopLeft)
                {
                    currentMinTopLeft = marginBounds.Top;
                }

                if (marginBounds.Bottom > currentMaxBottomRight)
                {
                    currentMaxBottomRight = marginBounds.Bottom;
                }
            }
            else
            {
                // Identify a new column.
                if (flowDirection == FlowDirection.TopDown ? marginBounds.Y < lastOffset : marginBounds.Y > lastOffset)
                {
                    Debug.Assert(currentMinTopLeft > 0 && currentMaxBottomRight > 0, "How can we not have a min/max value?");
                    if (currentMinTopLeft > 0 && currentMaxBottomRight > 0)
                    {
                        _commonSizes.Add((
                            currentMinTopLeft,
                            currentMaxBottomRight,
                            currentMaxBottomRight - currentMinTopLeft,
                            i));
                        currentMinTopLeft = int.MaxValue;
                        currentMaxBottomRight = -1;
                    }
                }

                lastOffset = marginBounds.Y;

                // Be sure to track the column size.
                if (marginBounds.Left < currentMinTopLeft)
                {
                    currentMinTopLeft = marginBounds.Left;
                }

                if (marginBounds.Right > currentMaxBottomRight)
                {
                    currentMaxBottomRight = marginBounds.Right;
                }
            }
        }

        // Add the last row/column to our common sizes.
        if (currentMinTopLeft > 0 && currentMaxBottomRight > 0)
        {
            // Store off the max size for this row.
            _commonSizes.Add((
                currentMinTopLeft,
                currentMaxBottomRight,
                currentMaxBottomRight - currentMinTopLeft,
                i));
        }

        // Pass2 - adjust all controls to max width/height according to their row/column.
        int controlIndex = 0;
        foreach (var size in _commonSizes)
        {
            while (controlIndex < size.LastIndex)
            {
                if (horizontalFlow)
                {
                    // Min is Top, align all controls horizontally.
                    _childInfo[controlIndex].MarginBounds.Y = size.Min;
                    _childInfo[controlIndex].MarginBounds.Height = size.Size;
                }
                else
                {
                    // Min is Left, align controls vertically.
                    _childInfo[controlIndex].MarginBounds.X = size.Min;
                    _childInfo[controlIndex].MarginBounds.Width = size.Size;
                }

                controlIndex++;
            }
        }
    }

    private string GetTransactionDescription(bool performCopy)
    {
        if (_dragControls.Count == 1)
        {
            string name = TypeDescriptor.GetComponentName(_dragControls[0]);
            if (string.IsNullOrEmpty(name))
            {
                name = _dragControls[0].GetType().Name;
            }

            return string.Format(performCopy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl, name);
        }

        return string.Format(performCopy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls, _dragControls.Count);
    }

    /// <summary>
    ///  Simply returns the designer's control as a FlowLayoutPanel
    /// </summary>
    private new FlowLayoutPanel Control => base.Control as FlowLayoutPanel;

    // per VSWhidbey #424850 adding this to this class...
    protected override InheritanceAttribute InheritanceAttribute
    {
        get
        {
            if ((base.InheritanceAttribute == InheritanceAttribute.Inherited)
                || (base.InheritanceAttribute == InheritanceAttribute.InheritedReadOnly))
            {
                return InheritanceAttribute.InheritedReadOnly;
            }

            return base.InheritanceAttribute;
        }
    }

    /// <summary>
    ///  Shadows the FlowDirection property. We do this so that we can update the areas
    ///  covered by glyphs correctly. VSWhidbey# 232910.
    /// </summary>
    private FlowDirection FlowDirection
    {
        get => Control.FlowDirection;
        set
        {
            if (value != Control.FlowDirection)
            {
                // Since we don't know which control is going to go where,
                // we just invalidate the area corresponding to the ClientRectangle in the adornerWindow
                BehaviorService.Invalidate(BehaviorService.ControlRectInAdornerWindow(Control));
                Control.FlowDirection = value;
            }
        }
    }

    private void DrawIBarBeforeRectangle(Rectangle bounds)
    {
        switch (RTLTranslateFlowDirection(FlowLayoutPanel.FlowDirection))
        {
            case FlowDirection.LeftToRight:
                ReDrawIBar(new Point(bounds.Left, bounds.Top), new Point(bounds.Left, bounds.Bottom));
                break;
            case FlowDirection.RightToLeft:
                ReDrawIBar(new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom));
                break;
            case FlowDirection.TopDown:
                ReDrawIBar(new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
                break;
            case FlowDirection.BottomUp:
                ReDrawIBar(new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Bottom));
                break;
        }
    }

    private void DrawIBarAfterRectangle(Rectangle bounds)
    {
        switch (RTLTranslateFlowDirection(FlowLayoutPanel.FlowDirection))
        {
            case FlowDirection.LeftToRight:
                ReDrawIBar(new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom));
                break;
            case FlowDirection.RightToLeft:
                ReDrawIBar(new Point(bounds.Left, bounds.Top), new Point(bounds.Left, bounds.Bottom));
                break;
            case FlowDirection.TopDown:
                ReDrawIBar(new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Bottom));
                break;
            case FlowDirection.BottomUp:
                ReDrawIBar(new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
                break;
        }
    }

    private void EraseIBar()
       => ReDrawIBar(Point.Empty, Point.Empty);

    /// <summary>
    ///  Given two points, we'll draw an I-Bar. Note that we only erase at our
    ///  old points if they are different from the new ones. Also note that if
    ///  the points are empty - we will simply erase and not draw.
    /// </summary>
    private void ReDrawIBar(Point point1, Point point2)
    {
        var pen = SystemPens.ControlText;
        var backColor = Control.BackColor;
        if (backColor != Color.Empty && backColor.GetBrightness() < .5)
        {
            pen = SystemPens.ControlLight;
        }

        // Don't off set if point1 is empty. Empty really just means that we want to erase the IBar.
        if (point1 != Point.Empty)
        {
            Point offset = BehaviorService.AdornerWindowToScreen();
            point1.Offset(-offset.X, -offset.Y);
            point2.Offset(-offset.X, -offset.Y);
        }

        // Only erase the I-Bar if the points are different from last time.
        // Only invalidate if there's something to invalidate.
        if (point1 != _oldPoint1 && point2 != _oldPoint2 && _oldPoint1 != Point.Empty)
        {
            Rectangle invalidRect = new(
                _oldPoint1.X,
                _oldPoint1.Y,
                _oldPoint2.X - _oldPoint1.X + 1,
                _oldPoint2.Y - _oldPoint1.Y + 1);

            // Always invalidate max area.
            invalidRect.Inflate(_maxIBarWidth, _maxIBarWidth);
            BehaviorService.Invalidate(invalidRect);
        }

        // Cache this for next time around -- but do so before changing point1 and point2 below.
        _oldPoint1 = point1;
        _oldPoint2 = point2;

        // If we have valid new points - redraw our I-Bar.
        // We always want to redraw the line. This is because part of it could have been erased when
        // the drag image (see DropSourceBehavior) is being moved over the IBar.
        if (point1.IsEmpty)
        {
            return;
        }

        using Graphics graphics = BehaviorService.AdornerWindowGraphics;
        if (HorizontalFlow)
        {
            if (Math.Abs(point1.Y - point2.Y) <= MinIBar)
            {
                // Draw the smaller, simpler IBar
                graphics.DrawLine(pen, point1, point2); // vertical line
                graphics.DrawLine(pen, point1.X - IBarHalfSize, point1.Y, point1.X + IBarHalfSize, point1.Y); // top hat
                graphics.DrawLine(pen, point2.X - IBarHalfSize, point2.Y, point2.X + IBarHalfSize, point2.Y); // bottom hat
            }
            else
            {
                // Top and bottom hat.
                for (int i = 0; i < IBarHatHeight - 1; i++)
                {
                    // Stop 1 pixel before, since we can't draw a 1 pixel line
                    // reducing the width of the hat with 2 pixel on each iteration
                    graphics.DrawLine(
                        pen,
                        point1.X - (IBarHatWidth - 1 - i * 2) / 2,
                        point1.Y + i,
                        point1.X + (IBarHatWidth - 1 - i * 2) / 2,
                        point1.Y + i); // top hat

                    graphics.DrawLine(
                        pen,
                        point2.X - (IBarHatWidth - 1 - i * 2) / 2,
                        point2.Y - i,
                        point2.X + (IBarHatWidth - 1 - i * 2) / 2,
                        point2.Y - i); // bottom hat
                }

                // Can't draw a 1 pixel line, so draw a vertical line.
                graphics.DrawLine(pen, point1.X, point1.Y, point1.X, point1.Y + IBarHatHeight - 1); // top hat
                graphics.DrawLine(pen, point2.X, point2.Y, point2.X, point2.Y - IBarHatHeight + 1); // bottom hat

                // Vertical line
                graphics.DrawLine(pen, point1.X, point1.Y + IBarLineOffset, point2.X, point2.Y - IBarLineOffset);
            }
        }
        else
        {
            if (Math.Abs(point1.X - point2.X) <= MinIBar)
            {
                // Draw the smaller, simpler IBar.
                graphics.DrawLine(pen, point1, point2); // horizontal line
                graphics.DrawLine(pen, point1.X, point1.Y - IBarHalfSize, point1.X, point1.Y + IBarHalfSize); // top hat
                graphics.DrawLine(pen, point2.X, point2.Y - IBarHalfSize, point2.X, point2.Y + IBarHalfSize); // bottom hat
            }
            else
            {
                // Left and right hat.
                for (int i = 0; i < IBarHatHeight - 1; i++)
                {
                    // Stop 1 pixel before, since we can't draw a 1 pixel line
                    // reducing the width of the hat with 2 pixel on each iteration.
                    graphics.DrawLine(
                        pen,
                        point1.X + i,
                        point1.Y - (IBarHatWidth - 1 - i * 2) / 2,
                        point1.X + i,
                        point1.Y + (IBarHatWidth - 1 - i * 2) / 2); // left hat

                    graphics.DrawLine(
                        pen,
                        point2.X - i,
                        point2.Y - (IBarHatWidth - 1 - i * 2) / 2,
                        point2.X - i,
                        point2.Y + (IBarHatWidth - 1 - i * 2) / 2); // right hat
                }

                // Can't draw a 1 pixel line, so draw a horizontal line.
                graphics.DrawLine(pen, point1.X, point1.Y, point1.X + IBarHatHeight - 1, point1.Y); // left hat
                graphics.DrawLine(pen, point2.X, point2.Y, point2.X - IBarHatHeight + 1, point2.Y); // right hat

                // Horizontal line
                graphics.DrawLine(pen, point1.X + IBarLineOffset, point1.Y, point2.X - IBarLineOffset, point2.Y);
            }
        }
    }

    private void ReorderControls(DragEventArgs de)
    {
        bool performCopy = de.Effect == DragDropEffects.Copy;

        // create our transaction
        DesignerTransaction designerTransaction = TryGetService(out IDesignerHost host)
            ? host.CreateTransaction(GetTransactionDescription(performCopy))
            : null;

        try
        {
            // In order to be able to set the index correctly, we need to create a backwards move.
            // We do this by first finding the control foo that corresponds to _insertionIndex.
            // We then remove all the drag controls from the FLP.
            // Then we get the new childIndex for the control foo.
            // Finally we loop:
            //      add the ith drag control
            //      set its child index to (index of control foo) - 1
            // On each iteration, the child index of control foo will change.
            //
            // This ensures that we can move both contiguous and non-contiguous selections.

            // Special case when the element we are inserting before is a part of the dragControls.
            while (_insertionIndex < _childInfo.Length - 1 && _childInfo[_insertionIndex].InSelectionCollection)
            {
                // Find the next control that is not a part of the selection.
                ++_insertionIndex;
            }

            PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(Component)["Controls"];
            if (controlsProperty is not null)
            {
                RaiseComponentChanging(controlsProperty);
            }

            Control control = null;
            var children = Control.Controls;
            if (_insertionIndex != _childInfo.Length)
            {
                control = children[_insertionIndex];
            }
            else
            {
                // We are inserting past the last control.
                _insertionIndex = InvalidIndex;
            }

            // We use this list when doing a Drag-Copy, so that we can correctly restore state when we are done.
            List<Control> originalControls = [];

            // Remove the controls in the drag collection - don't need to do this if we are copying.
            if (!performCopy)
            {
                foreach (var dragControl in _dragControls)
                {
                    children.Remove(dragControl);
                }

                // Get the new index -- if we are performing a copy, then the index is the same.
                if (control is not null)
                {
                    _insertionIndex = children.GetChildIndex(control, throwException: false);
                }
            }
            else
            {
                // We are doing a copy, so let's copy the controls.
                List<IComponent> tempList = DesignerUtils.CopyDragObjects(_dragControls, Component.Site);

                if (tempList is null)
                {
                    return;
                }

                // And stick the copied controls back into the dragControls array.
                for (int j = 0; j < tempList.Count; j++)
                {
                    // Save off the old controls first.
                    originalControls.Add(_dragControls[j]);

                    // Remember to set the new primary control.
                    if (_primaryDragControl.Equals(_dragControls[j]))
                    {
                        _primaryDragControl = (Control)tempList[j];
                    }

                    _dragControls[j] = (Control)tempList[j];
                }
            }

            if (_insertionIndex == InvalidIndex)
            {
                // Either _insertionIndex was _childInfo.Length (inserting past the end) or
                // _insertionIndex was _childInfo.Length - 1 and the control at that index was also
                // a part of the dragCollection. In either case, the new index is equal to the count
                // of existing controls in the ControlCollection. Helps to draw this out.
                _insertionIndex = children.Count;
            }

            children.Add(_primaryDragControl);
            children.SetChildIndex(_primaryDragControl, _insertionIndex);
            ++_insertionIndex;

            // Set the Selection ..
            SelectionService.SetSelectedComponents(new IComponent[] { _primaryDragControl }, SelectionTypes.Primary | SelectionTypes.Replace);

            // Note _dragControls are in opposite order than what FLP uses,
            // so add from the end.
            for (int i = _dragControls.Count - 1; i >= 0; i--)
            {
                if (_primaryDragControl.Equals(_dragControls[i]))
                {
                    continue;
                }

                children.Add(_dragControls[i]);
                children.SetChildIndex(_dragControls[i], _insertionIndex);
                ++_insertionIndex;

                SelectionService.SetSelectedComponents(new IComponent[] { _dragControls[i] }, SelectionTypes.Add);
            }

            if (controlsProperty is not null)
            {
                RaiseComponentChanging(controlsProperty);
            }

            // If we did a Copy, then restore the old controls to make sure we set state correctly.
            if (originalControls is not null)
            {
                for (int i = 0; i < originalControls.Count; i++)
                {
                    _dragControls[i] = originalControls[i];
                }
            }

            base.OnDragComplete(de);

            designerTransaction?.Commit();
        }
        catch
        {
            designerTransaction?.Cancel();
        }
    }

    /// <summary>
    ///  When a child is added -we check to see if we cached an index
    ///  representing where this control should be inserted. If so, we
    ///  re-insert the new child.
    ///  This is only done on an external drag-drop.
    /// </summary>
    private void OnChildControlAdded(object sender, ControlEventArgs e)
    {
        try
        {
            if (_insertionIndex == InvalidIndex)
            {
                return;
            }

            // This will only be true on a drag-drop.
            PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(Component)["Controls"];

            if (controlsProperty is not null)
            {
                RaiseComponentChanging(controlsProperty);

                // On an external drag/drop, the control will have been inserted at the end, so we can safely
                // set the index and increment it, since we are moving the control backwards. Check out
                // SetChildIndex and MoveElement.
                Control.Controls.SetChildIndex(e.Control, _insertionIndex);
                ++_insertionIndex;
                RaiseComponentChanging(controlsProperty);
            }
        }
        finally
        {
            Control.ControlAdded -= OnChildControlAdded;
            _insertionIndex = InvalidIndex;
        }
    }

    /// <summary>
    ///  When we receive a drag enter notification - we clear our recommended insertion
    ///  index and mouse location - then call our method to cache all the bounds of the children.
    /// </summary>
    protected override void OnDragEnter(DragEventArgs de)
    {
        base.OnDragEnter(de);

        _insertionIndex = InvalidIndex;
        _lastMouseLocation = Point.Empty;
        _primaryDragControl = null;

        // Get the sorted drag controls. We use these for an internal drag.
        if (de.Data is DropSourceBehavior.BehaviorDataObject data)
        {
            _dragControls = [..data.GetSortedDragControls(out int primaryIndex).OfType<Control>()];
            _primaryDragControl = _dragControls[primaryIndex];
        }

        // Cache all child bounds and identify rows/columns.
        CreateMarginBoundsList();
    }

    protected override void OnDragLeave(EventArgs e)
    {
        EraseIBar();

        _insertionIndex = InvalidIndex;
        _primaryDragControl = null;
        _dragControls?.Clear();

        base.OnDragLeave(e);
    }

    /// <summary>
    ///  During a drag over, if we have successfully cached margin/row/col information
    ///  we will attempt to render an "I-bar" for the user based on where we think the
    ///  user is attempting to insert the control at. Note that we also cache off this
    ///  guessed-index so that if a control is dropped/added we can re-insert it at this
    ///  spot.
    /// </summary>
    protected override void OnDragOver(DragEventArgs de)
    {
        base.OnDragOver(de);

        Point mouseLocation = new(de.X, de.Y);

        if (mouseLocation.Equals(_lastMouseLocation)
            || _childInfo is null
            || _childInfo.Length == 0
            || _commonSizes.Count == 0)
        {
            // No layout data to work with.
            return;
        }

        _lastMouseLocation = mouseLocation;

        Point controlOffset = Control.PointToScreen(Point.Empty);
        if (IsRtl)
        {
            controlOffset.X += Control.Width;
        }

        _insertionIndex = InvalidIndex;

        // Brute force hit testing to first determine if we're over one
        // of our margin bounds.
        int i;
        Rectangle bounds = Rectangle.Empty;
        for (i = 0; i < _childInfo.Length; i++)
        {
            if (_childInfo[i].MarginBounds.Contains(mouseLocation))
            {
                bounds = _childInfo[i].ControlBounds;
                break;
            }
        }

        // If we found the bounds - then we need to draw our "I-Beam"
        // If the mouse is over one of the MarginBounds, then the dragged control
        // will always be inserted before the control the margin area represents. Thus
        // we will always draw the I-Beam to the left or above (FlowDirection.LRT | TB) or
        // to the right or below (FlowDirection.RTL | BT).
        if (!bounds.IsEmpty)
        {
            // The insertion index will always be the boxed area(called margin area) we are over.
            _insertionIndex = i;
            if (_childInfo[i].InSelectionCollection)
            {
                // If the marginBounds is part of the selection, then don't draw the IBar. But actually
                // setting insertIndex, will allows us to correctly drop the control in the right place.
                EraseIBar();
            }
            else
            {
                DrawIBarBeforeRectangle(bounds);
            }
        }
        else
        {
            // Here, we're in a dead area - see what row / column we're in for a
            // best-guess at the insertion index.
            int offset = HorizontalFlow ? controlOffset.Y : controlOffset.X;
            foreach (var size in _commonSizes)
            {
                bool match;
                if (IsRtl)
                {
                    // Size is height/width of row/column.
                    offset -= size.Size;
                    match = (HorizontalFlow && mouseLocation.Y <= offset) || (!HorizontalFlow && mouseLocation.X >= offset);
                }
                else
                {
                    offset += size.Size;
                    match = (HorizontalFlow ? mouseLocation.Y : mouseLocation.X) <= offset;
                }

                if (match)
                {
                    _insertionIndex = size.LastIndex;
                    bounds = _childInfo[_insertionIndex - 1].ControlBounds;
                    if (_childInfo[_insertionIndex - 1].InSelectionCollection)
                    {
                        EraseIBar();
                    }
                    else
                    {
                        DrawIBarAfterRectangle(bounds);
                    }

                    break;
                }
            }
        }

        if (_insertionIndex == InvalidIndex)
        {
            // Here, we're at the 'end' of the FlowLayoutPanel - not over
            // any controls and not in a row/column.
            _insertionIndex = FlowLayoutPanel.Controls.Count;
            EraseIBar();
        }
    }

    /// <summary>
    ///  On a drop, if we have cached a special index where we think a control
    ///  should be inserted - we check to see if this was a pure-local drag
    ///  (i.e. we dragged a child control inside ourselves). If so, we re-insert the
    ///  child to the appropriate index. Otherwise, we'll do this in the ChildAdded
    ///  event.
    /// </summary>
    protected override void OnDragDrop(DragEventArgs de)
    {
        if (_dragControls is not null &&
            _primaryDragControl is not null &&
            Control.Controls.Contains(_primaryDragControl))
        {
            // Manipulating our controls. We do it ourselves, so that we can set the indices right.
            ReorderControls(de);

            _insertionIndex = InvalidIndex;
        }
        else
        {
            // If we are not reordering our controls then just let the base handle it.
            Control.ControlAdded += OnChildControlAdded;
            base.OnDragDrop(de);
        }
    }
}
