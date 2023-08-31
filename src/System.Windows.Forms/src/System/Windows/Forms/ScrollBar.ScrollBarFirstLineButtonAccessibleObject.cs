// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop.UiaCore;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal class ScrollBarFirstLineButtonAccessibleObject : ScrollBarChildAccessibleObject
    {
        public ScrollBarFirstLineButtonAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
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
                NavigateDirection.PreviousSibling => null,
                NavigateDirection.NextSibling
                    => ParentInternal.FirstPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.FirstPageButtonAccessibleObject
                        : ParentInternal.ThumbAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 1;
    }
}
