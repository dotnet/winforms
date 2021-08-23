// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class TrackBar
    {
        internal class TrackBarFirstButtonAccessibleObject : TrackBarChildAccessibleObject
        {
            public TrackBarFirstButtonAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
            { }

            public override string DefaultAction => SR.AccessibleActionPress;

            public override string? Name
                => OwningTrackBar.Orientation == Orientation.Horizontal
                   && (OwningTrackBar.RightToLeft == RightToLeft.No || ParentInternal.IsMirrored)
                        ? SR.TrackBarLargeDecreaseButtonName
                        : SR.TrackBarLargeIncreaseButtonName;

            internal override bool IsDisplayed
            {
                get
                {
                    if (!OwningTrackBar.IsHandleCreated || !base.IsDisplayed)
                    {
                        return false;
                    }

                    return OwningTrackBar.Orientation == Orientation.Vertical || ParentInternal.RTLLayoutDisabled
                        ? OwningTrackBar.Maximum != OwningTrackBar.Value
                        : OwningTrackBar.Minimum != OwningTrackBar.Value;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!OwningTrackBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.PreviousSibling => null,
                    UiaCore.NavigateDirection.NextSibling => IsDisplayed ? ParentInternal.ThumbAccessibleObject : null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 1;

            internal override void Invoke()
            {
                if (OwningTrackBar.IsHandleCreated)
                {
                    // The "GetChildId" method returns to the id of the trackbar element,
                    // which allows to use the native "accDoDefaultAction" method when the "Invoke" method is called
                    ParentInternal.GetSystemIAccessibleInternal()?.accDoDefaultAction(GetChildId());
                }
            }
        }
    }
}
