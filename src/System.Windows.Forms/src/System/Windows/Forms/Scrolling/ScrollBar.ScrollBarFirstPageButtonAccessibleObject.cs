// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal class ScrollBarFirstPageButtonAccessibleObject : ScrollBarChildAccessibleObject
    {
        public ScrollBarFirstPageButtonAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
        {
        }

        public override AccessibleStates State
            => OwningScrollBar.IsHandleCreated && !IsDisplayed
                ? AccessibleStates.Invisible
                : AccessibleStates.None;

        internal override bool IsDisplayed
        {
            get
            {
                if (!base.IsDisplayed)
                {
                    return false;
                }

                return OwningScrollBar._scrollOrientation == ScrollOrientation.HorizontalScroll
                    && OwningScrollBar.RightToLeft == RightToLeft.Yes
                        ? ParentInternal.UIMaximum > OwningScrollBar.Value
                        : OwningScrollBar.Minimum != OwningScrollBar.Value;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!OwningScrollBar.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling => IsDisplayed ? ParentInternal.FirstLineButtonAccessibleObject : null,
                NavigateDirection.NavigateDirection_NextSibling => IsDisplayed ? ParentInternal.ThumbAccessibleObject : null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 2;
    }
}
