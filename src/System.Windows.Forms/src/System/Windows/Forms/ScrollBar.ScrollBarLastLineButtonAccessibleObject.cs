// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarLastLineButtonAccessibleObject : ScrollBarChildAccessibleObject
        {
            public ScrollBarLastLineButtonAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
            {
            }

            internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            {
                if (!OwningScrollBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    NavigateDirection.NextSibling => null,
                    NavigateDirection.PreviousSibling
                        => ParentInternal.LastPageButtonAccessibleObject.IsDisplayed
                            ? ParentInternal.LastPageButtonAccessibleObject
                            : ParentInternal.ThumbAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 5;
        }
    }
}
