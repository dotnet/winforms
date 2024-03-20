// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ListViewItem : IKeyboardToolTip
{
    private bool AllowsToolTips => _listView?.ShowItemToolTips ?? false;

    bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => AllowsToolTips;

    bool IKeyboardToolTip.AllowsToolTip() => true;

    bool IKeyboardToolTip.CanShowToolTipsNow() => AllowsToolTips;

    string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => ToolTipText;

    Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => GetNativeRectangle(Index);

    IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
    {
        List<Rectangle> neighboringRectangles = [];
        if (_listView is null)
        {
            return neighboringRectangles;
        }

        switch (_listView.View)
        {
            case View.SmallIcon:
            case View.LargeIcon:
                foreach (SearchDirectionHint searchDirectionHint in Enum.GetValues(typeof(SearchDirectionHint)))
                {
                    ListViewItem? neighboringItem = FindNearestItem(searchDirectionHint);

                    if (neighboringItem is not null)
                    {
                        neighboringRectangles.Add(GetNativeRectangle(neighboringItem.Index));
                    }
                }

                break;
            case View.Details:
                neighboringRectangles.Add(AccessibilityObject.Bounds);

                if (Index > 0)
                {
                    neighboringRectangles.Add(_listView.Items[Index - 1].AccessibilityObject.Bounds);
                }

                if (Index < _listView.Items.Count - 1)
                {
                    neighboringRectangles.Add(_listView.Items[Index + 1].AccessibilityObject.Bounds);
                }

                break;
        }

        return neighboringRectangles;
    }

    IWin32Window? IKeyboardToolTip.GetOwnerWindow() => _listView;

    bool IKeyboardToolTip.HasRtlModeEnabled() => _listView?.RightToLeft == RightToLeft.Yes;

    bool IKeyboardToolTip.IsBeingTabbedTo() => Control.AreCommonNavigationalKeysDown();

    bool IKeyboardToolTip.IsHoveredWithMouse() => _listView?.AccessibilityObject.Bounds.Contains(Control.MousePosition) ?? false;

    void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

    void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

    bool IKeyboardToolTip.ShowsOwnToolTip() => AllowsToolTips;

    internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) { }

    internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) { }

    private Rectangle GetNativeRectangle(int index)
    {
        if (_listView is null)
        {
            return Rectangle.Empty;
        }

        ListViewItem item = _listView.Items[index];
        if (item.Group is not null && item.Group.CollapsedState == ListViewGroupCollapsedState.Collapsed)
        {
            return Rectangle.Empty;
        }

        Rectangle itemBounds = _listView.GetItemRect(index, ItemBoundsPortion.Label);
        Rectangle listviewBounds = _listView.AccessibilityObject.Bounds;
        Point point = new(listviewBounds.X + itemBounds.X, listviewBounds.Y + itemBounds.Y);

        return _listView.View switch
        {
            View.Details or View.List => GetDetailsListRectangle(point, item, itemBounds),
            View.Tile => GetTileRectangle(point, item, itemBounds),
            _ => new Rectangle(point, new Size(itemBounds.Width, itemBounds.Height)),
        };
    }

    private static Rectangle GetDetailsListRectangle(Point point, ListViewItem item, Rectangle itemBounds)
    {
        // The GetItemRect(index, ItemBoundsPortion.Label) method returns the width of the cell,
        // not the text for "Details" and "List" views. As result, if the text is smaller than the cell,
        // we return the width of the text. If the text is larger than the cell, we return the width of the cell
        return new Rectangle(
            point,
            new Size(Math.Min(TextRenderer.MeasureText(item.Text, item.Font).Width, itemBounds.Width),
            itemBounds.Height));
    }

    private Rectangle GetTileRectangle(Point point, ListViewItem item, Rectangle itemBounds)
    {
        // The GetItemRect(index, ItemBoundsPortion.Label) method returns the incorrect width of the item for "Tile" view.
        // When returning the rectangle, we take into account the width of the texts of the item and its sub-item,
        // because the first sub-item is also displayed in the list
        int textWidth = TextRenderer.MeasureText(item.Text, item.Font).Width;

        if (SubItems.Count > 1)
        {
            ListViewSubItem subItem = SubItems[1];
            textWidth = Math.Max(TextRenderer.MeasureText(subItem.Text, subItem.Font).Width, textWidth);
        }

        return new Rectangle(point, new Size(Math.Min(textWidth, itemBounds.Width), itemBounds.Height));
    }
}
