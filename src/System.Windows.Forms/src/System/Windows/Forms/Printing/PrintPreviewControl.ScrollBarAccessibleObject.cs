// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal class ScrollBarAccessibleObject : ScrollBar.ScrollBarAccessibleObject
    {
        public ScrollBarAccessibleObject(ScrollBar owner) : base(owner)
        {
        }

        public override AccessibleObject? Parent
            => this.TryGetOwnerAs(out ScrollBar? scrollBar) && scrollBar.Parent is PrintPreviewControl printPreviewControl
                ? printPreviewControl.AccessibilityObject : base.Parent;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out ScrollBar? scrollBar) || scrollBar.Parent is not PrintPreviewControl printPreviewControl)
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    return printPreviewControl.AccessibilityObject;

                case UiaCore.NavigateDirection.NextSibling:
                    return printPreviewControl._vScrollBar.Visible &&
                        printPreviewControl._hScrollBar.Visible &&
                        scrollBar == printPreviewControl._vScrollBar
                        ? printPreviewControl._hScrollBar.AccessibilityObject : null;

                case UiaCore.NavigateDirection.PreviousSibling:
                    return printPreviewControl._hScrollBar.Visible &&
                        printPreviewControl._vScrollBar.Visible &&
                        scrollBar == printPreviewControl._hScrollBar
                        ? printPreviewControl._vScrollBar.AccessibilityObject : null;

                default:
                    return base.FragmentNavigate(direction);
            }
        }
    }
}
