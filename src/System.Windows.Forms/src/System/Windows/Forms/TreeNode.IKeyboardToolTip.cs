// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class TreeNode : IKeyboardToolTip
    {
        bool IKeyboardToolTip.AllowsChildrenToShowToolTips() => AllowToolTips;

        bool IKeyboardToolTip.AllowsToolTip() => true;

        bool IKeyboardToolTip.CanShowToolTipsNow() => AllowToolTips;

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) => ToolTipText;

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle() => TreeView.RectangleToScreen(Bounds);

        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
        {
            TreeNode nextNode = NextVisibleNode;
            TreeNode prevNode = PrevVisibleNode;
            List<Rectangle> neighboringRectangles = new List<Rectangle>();

            if (nextNode is not null)
            {
                neighboringRectangles.Add(TreeView.RectangleToScreen(nextNode.Bounds));
            }

            if (prevNode is not null)
            {
                neighboringRectangles.Add(TreeView.RectangleToScreen(prevNode.Bounds));
            }

            return neighboringRectangles;
        }

        IWin32Window IKeyboardToolTip.GetOwnerWindow() => TreeView;

        bool IKeyboardToolTip.HasRtlModeEnabled() => TreeView.RightToLeft == RightToLeft.Yes;

        bool IKeyboardToolTip.IsBeingTabbedTo() => Control.AreCommonNavigationalKeysDown();

        bool IKeyboardToolTip.IsHoveredWithMouse() => TreeView.AccessibilityObject.Bounds.Contains(Control.MousePosition);

        void IKeyboardToolTip.OnHooked(ToolTip toolTip) => OnKeyboardToolTipHook(toolTip);

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) => OnKeyboardToolTipUnhook(toolTip);

        bool IKeyboardToolTip.ShowsOwnToolTip() => true;

        private bool AllowToolTips => TreeView is not null && TreeView.ShowNodeToolTips;
    }
}
