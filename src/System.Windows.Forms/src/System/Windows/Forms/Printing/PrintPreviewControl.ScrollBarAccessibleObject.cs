// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal sealed class ScrollBarAccessibleObject : ScrollBar.ScrollBarAccessibleObject
    {
        public ScrollBarAccessibleObject(ScrollBar owner) : base(owner)
        {
        }

        public override AccessibleObject? Parent =>
            this.TryGetOwnerAs(out ScrollBar? scrollBar) && scrollBar.Parent is PrintPreviewControl printPreviewControl
                ? printPreviewControl.AccessibilityObject
                : base.Parent;

        private protected override bool IsInternal => true;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out ScrollBar? scrollBar) || scrollBar.Parent is not PrintPreviewControl printPreviewControl)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => printPreviewControl.AccessibilityObject,
                NavigateDirection.NavigateDirection_NextSibling
                    => printPreviewControl._vScrollBar.Visible &&
                        printPreviewControl._hScrollBar.Visible &&
                        scrollBar == printPreviewControl._vScrollBar
                        ? printPreviewControl._hScrollBar.AccessibilityObject
                        : null,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => printPreviewControl._hScrollBar.Visible &&
                        printPreviewControl._vScrollBar.Visible &&
                        scrollBar == printPreviewControl._hScrollBar
                        ? printPreviewControl._vScrollBar.AccessibilityObject
                        : null,
                _ => base.FragmentNavigate(direction),
            };
        }
    }
}
