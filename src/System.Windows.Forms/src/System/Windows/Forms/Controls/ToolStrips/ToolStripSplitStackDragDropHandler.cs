// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This class supports the AllowItemReorder feature.
///  When reordering items ToolStrip and ToolStripItem drag/drop events are routed here.
/// </summary>
internal sealed partial class ToolStripSplitStackDragDropHandler : IDropTarget, ISupportOleDropSource
{
    private readonly ToolStrip _owner;

    public ToolStripSplitStackDragDropHandler(ToolStrip owner)
    {
        _owner = owner.OrThrowIfNull();
    }

    public void OnDragEnter(DragEventArgs e)
    {
        if (e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            e.Effect = DragDropEffects.Move;
            ShowItemDropPoint(_owner.PointToClient(new Point(e.X, e.Y)));
        }
    }

    public void OnDragLeave(EventArgs e)
    {
        _owner.ClearInsertionMark();
    }

    public void OnDragDrop(DragEventArgs e)
    {
        if (e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            ToolStripItem item = (ToolStripItem)e.Data.GetData(typeof(ToolStripItem))!;
            OnDropItem(item, _owner.PointToClient(new Point(e.X, e.Y)));
        }
    }

    public void OnDragOver(DragEventArgs e)
    {
        if (e.Data is not null && e.Data.GetDataPresent(typeof(ToolStripItem)))
        {
            if (ShowItemDropPoint(_owner.PointToClient(new Point(e.X, e.Y))))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                _owner?.ClearInsertionMark();

                e.Effect = DragDropEffects.None;
            }
        }
    }

    public void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
    }

    public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
    }

    private void OnDropItem(ToolStripItem droppedItem, Point ownerClientAreaRelativeDropPoint)
    {
        int toolStripItemIndex = GetItemInsertionIndex(ownerClientAreaRelativeDropPoint);
        if (toolStripItemIndex >= 0)
        {
            ToolStripItem item = _owner.Items[toolStripItemIndex];
            if (item == droppedItem)
            {
                _owner.ClearInsertionMark();
                return;  // optimization
            }

            RelativeLocation relativeLocation = ComparePositions(item.Bounds, ownerClientAreaRelativeDropPoint);
            droppedItem.Alignment = item.Alignment;

            // Protect against negative indicies
            int insertIndex = Math.Max(0, toolStripItemIndex);

            if (relativeLocation == RelativeLocation.Above)
            {
                insertIndex = (item.Alignment == ToolStripItemAlignment.Left) ? insertIndex : insertIndex + 1;
            }
            else if (relativeLocation == RelativeLocation.Below)
            {
                insertIndex = (item.Alignment == ToolStripItemAlignment.Left) ? insertIndex : insertIndex - 1;
            }
            else if (((item.Alignment == ToolStripItemAlignment.Left) && (relativeLocation == RelativeLocation.Left)) ||
                ((item.Alignment == ToolStripItemAlignment.Right) && (relativeLocation == RelativeLocation.Right)))
            {
                // the item alignment is Tail & dropped to right of the center of the item
                // or the item alignment is Head & dropped to the left of the center of the item

                // Normally, insert the new item after the item, however in RTL insert before the item
                insertIndex = Math.Max(0, (_owner.RightToLeft == RightToLeft.Yes) ? insertIndex + 1 : insertIndex);
            }
            else
            {
                // the item alignment is Tail & dropped to left of the center of the item
                // or the item alignment is Head & dropped to the right of the center of the item

                // Normally, insert the new item before the item, however in RTL insert after the item
                insertIndex = Math.Max(0, (_owner.RightToLeft == RightToLeft.No) ? insertIndex + 1 : insertIndex);
            }

            // If the control is moving from a lower to higher index, you actually want to set it one less than its position.
            // This is because it is being removed from its original position, which lowers the index of every control before
            // its new drop point by 1.
            if (_owner.Items.IndexOf(droppedItem) < insertIndex)
            {
                insertIndex--;
            }

            _owner.Items.MoveItem(Math.Max(0, insertIndex), droppedItem);
            _owner.ClearInsertionMark();
        }
        else if (toolStripItemIndex == -1 && _owner.Items.Count == 0)
        {
            _owner.Items.Add(droppedItem);
            _owner.ClearInsertionMark();
        }
    }

    private bool ShowItemDropPoint(Point ownerClientAreaRelativeDropPoint)
    {
        int i = GetItemInsertionIndex(ownerClientAreaRelativeDropPoint);
        if (i >= 0)
        {
            ToolStripItem item = _owner.Items[i];
            RelativeLocation relativeLocation = ComparePositions(item.Bounds, ownerClientAreaRelativeDropPoint);

            Rectangle insertionRect = Rectangle.Empty;
            switch (relativeLocation)
            {
                case RelativeLocation.Above:
                    insertionRect = new Rectangle(_owner.Margin.Left, item.Bounds.Top, _owner.Width - (_owner.Margin.Horizontal) - 1, ToolStrip.s_insertionBeamWidth);
                    break;
                case RelativeLocation.Below:
                    insertionRect = new Rectangle(_owner.Margin.Left, item.Bounds.Bottom, _owner.Width - (_owner.Margin.Horizontal) - 1, ToolStrip.s_insertionBeamWidth);
                    break;
                case RelativeLocation.Right:
                    insertionRect = new Rectangle(item.Bounds.Right, _owner.Margin.Top, ToolStrip.s_insertionBeamWidth, _owner.Height - (_owner.Margin.Vertical) - 1);
                    break;
                case RelativeLocation.Left:
                    insertionRect = new Rectangle(item.Bounds.Left, _owner.Margin.Top, ToolStrip.s_insertionBeamWidth, _owner.Height - (_owner.Margin.Vertical) - 1);
                    break;
            }

            _owner.PaintInsertionMark(insertionRect);
            return true;
        }
        else if (_owner.Items.Count == 0)
        {
            Rectangle insertionRect = _owner.DisplayRectangle;
            insertionRect.Width = ToolStrip.s_insertionBeamWidth;
            _owner.PaintInsertionMark(insertionRect);
            return true;
        }

        return false;
    }

    private int GetItemInsertionIndex(Point ownerClientAreaRelativeDropPoint)
    {
        for (int i = 0; i < _owner.DisplayedItems.Count; i++)
        {
            Rectangle bounds = _owner.DisplayedItems[i].Bounds;
            bounds.Inflate(_owner.DisplayedItems[i].Margin.Size);
            if (bounds.Contains(ownerClientAreaRelativeDropPoint))
            {
                // consider what to do about items not in the display
                return _owner.Items.IndexOf(_owner.DisplayedItems[i]);
            }
        }

        if (_owner.DisplayedItems.Count > 0)
        {
            for (int i = 0; i < _owner.DisplayedItems.Count; i++)
            {
                if (_owner.DisplayedItems[i].Alignment == ToolStripItemAlignment.Right)
                {
                    if (i > 0)
                    {
                        return _owner.Items.IndexOf(_owner.DisplayedItems[i - 1]);
                    }

                    return _owner.Items.IndexOf(_owner.DisplayedItems[i]);
                }
            }

            return _owner.Items.IndexOf(_owner.DisplayedItems[_owner.DisplayedItems.Count - 1]);
        }

        return -1;
    }

    private RelativeLocation ComparePositions(Rectangle orig, Point check)
    {
        if (_owner.Orientation == Orientation.Horizontal)
        {
            int widthUnit = orig.Width / 2;

            // we can return here if we are checking abovebelowleftright, because
            // the left right calculation is more picky than the above/below calculation
            // and the above below calculation will just override this one.
            if ((orig.Left + widthUnit) >= check.X)
            {
                return RelativeLocation.Left;
            }
            else if ((orig.Right - widthUnit) <= check.X)
            {
                return RelativeLocation.Right;
            }
        }

        if (_owner.Orientation == Orientation.Vertical)
        {
            int heightUnit = orig.Height / 2;
            return (check.Y <= (orig.Top + heightUnit)) ? RelativeLocation.Above : RelativeLocation.Below;
        }

        Debug.Fail("Could not calculate the relative position for AllowItemReorder");
        return RelativeLocation.Left;
    }
}
