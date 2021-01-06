// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    ///<summary>
    /// Custom class for displaying keyboard tooltips for TreeView and ListView
    ///</summary>
    ///<remark>
    /// This class is required to override the standard GetTOOLINFO method. In the original implementation,
    /// this method is designed to not use the TreeView or ListView tooltip caption for their child element
    /// tooltips (TreeNode and TreeViewItems). Unfortunately, this one interferes with the display of the
    /// keyboard tooltip, since it blocks the assignment of our custom value. The new implementation bypasses
    /// this problem and helps the keyboard tooltip work properly.
    ///</remark>
    internal class KeyboardToolTip : ToolTip
    {
        private protected override unsafe ToolInfoWrapper<Control> GetTOOLINFO(Control control, string caption)
        {
            TTF flags = TTF.TRANSPARENT | TTF.SUBCLASS;

            // RightToLeft reading order
            if (TopLevelControl?.RightToLeft == RightToLeft.Yes && !control.IsMirrored)
            {
                // Indicates that the ToolTip text will be displayed in the opposite direction
                // to the text in the parent window.
                flags |= TTF.RTLREADING;
            }

            var info = new ToolInfoWrapper<Control>(control, flags, caption ?? GetToolTip(control));
            info.Info.lpszText = (char*)(-1);

            return info;
        }
    }
}
