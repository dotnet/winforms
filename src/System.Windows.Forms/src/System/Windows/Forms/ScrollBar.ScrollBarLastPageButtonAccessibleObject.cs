// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarLastPageButtonAccessibleObject : ScrollBarChildAccessibleObject
        {
            public ScrollBarLastPageButtonAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
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
                            ? OwningScrollBar.Minimum != OwningScrollBar.Value
                            : ParentInternal.UIMaximum > OwningScrollBar.Value;
                }
            }

            internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            {
                if (!OwningScrollBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    NavigateDirection.PreviousSibling => IsDisplayed ? ParentInternal.ThumbAccessibleObject : null,
                    NavigateDirection.NextSibling => IsDisplayed ? ParentInternal.LastLineButtonAccessibleObject : null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 4;
        }
    }
}
