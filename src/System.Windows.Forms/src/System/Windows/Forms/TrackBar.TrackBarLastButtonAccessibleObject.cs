// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class TrackBar
    {
        internal class TrackBarLastButtonAccessibleObject : TrackBarChildAccessibleObject
        {
            public TrackBarLastButtonAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
            { }

            public override string DefaultAction => SR.AccessibleActionPress;

            public override string? Name
                => OwningTrackBar.Orientation == Orientation.Horizontal
                   && (OwningTrackBar.RightToLeft == RightToLeft.No || ParentInternal.IsMirrored)
                        ? SR.TrackBarLargeIncreaseButtonName
                        : SR.TrackBarLargeDecreaseButtonName;

            public override AccessibleStates State
                => !OwningTrackBar.IsHandleCreated || IsDisplayed
                    ? AccessibleStates.None
                    : AccessibleStates.Invisible;

            internal override bool IsDisplayed
            {
                get
                {
                    if (!OwningTrackBar.IsHandleCreated || !base.IsDisplayed)
                    {
                        return false;
                    }

                    if (OwningTrackBar.Minimum == OwningTrackBar.Maximum)
                    {
                        return true;
                    }

                    return OwningTrackBar.Orientation == Orientation.Vertical || ParentInternal.RTLLayoutDisabled
                            ? OwningTrackBar.Minimum != OwningTrackBar.Value
                            : OwningTrackBar.Maximum != OwningTrackBar.Value;
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
                    UiaCore.NavigateDirection.PreviousSibling => IsDisplayed ? ParentInternal.ThumbAccessibleObject : null,
                    UiaCore.NavigateDirection.NextSibling => null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 3;

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
