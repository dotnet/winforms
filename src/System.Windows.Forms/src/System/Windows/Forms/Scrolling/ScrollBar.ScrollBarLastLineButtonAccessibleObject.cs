// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal class ScrollBarLastLineButtonAccessibleObject : ScrollBarChildAccessibleObject
    {
        public ScrollBarLastLineButtonAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
        {
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!OwningScrollBar.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling => null,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => ParentInternal.LastPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.LastPageButtonAccessibleObject
                        : ParentInternal.ThumbAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 5;
    }
}
