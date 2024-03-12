// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class TreeNode : IKeyboardToolTip
{
    bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => AllowToolTips;

    bool IKeyboardToolTip.AllowsToolTip() => true;

    bool IKeyboardToolTip.CanShowToolTipsNow() => AllowToolTips;

    string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => ToolTipText;

    Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => RectangleToScreen(Bounds);

    IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
    {
        TreeNode? nextNode = NextVisibleNode;
        TreeNode? prevNode = PrevVisibleNode;
        List<Rectangle> neighboringRectangles = [];

        if (nextNode is not null)
        {
            neighboringRectangles.Add(RectangleToScreen(nextNode.Bounds));
        }

        if (prevNode is not null)
        {
            neighboringRectangles.Add(RectangleToScreen(prevNode.Bounds));
        }

        return neighboringRectangles;
    }

    IWin32Window? IKeyboardToolTip.GetOwnerWindow() => TreeView;

    bool IKeyboardToolTip.HasRtlModeEnabled() => TreeView?.RightToLeft == RightToLeft.Yes;

    bool IKeyboardToolTip.IsBeingTabbedTo() => Control.AreCommonNavigationalKeysDown();

    bool IKeyboardToolTip.IsHoveredWithMouse() => TreeView?.AccessibilityObject.Bounds.Contains(Control.MousePosition) ?? false;

    void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

    void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

    bool IKeyboardToolTip.ShowsOwnToolTip() => AllowToolTips;

    private bool AllowToolTips => TreeView?.ShowNodeToolTips ?? false;

    internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) { }

    internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) { }

    private Rectangle RectangleToScreen(Rectangle bounds)
    {
        return TreeView?.RectangleToScreen(bounds) ?? Rectangle.Empty;
    }
}
