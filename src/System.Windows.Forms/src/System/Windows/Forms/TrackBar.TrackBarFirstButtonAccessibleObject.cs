// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

public partial class TrackBar
{
    internal class TrackBarFirstButtonAccessibleObject : TrackBarChildAccessibleObject
    {
        public TrackBarFirstButtonAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
        { }

        public override string DefaultAction => SR.AccessibleActionPress;

        public override string? Name => !this.TryGetOwnerAs(out TrackBar? owner) || ParentInternal is not { } parent
            ? null
            : owner.Orientation == Orientation.Horizontal && (owner.RightToLeft == RightToLeft.No || parent.IsMirrored)
                ? SR.TrackBarLargeDecreaseButtonName
                : SR.TrackBarLargeIncreaseButtonName;

        internal override bool IsDisplayed
        {
            get
            {
                if (!this.TryGetOwnerAs(out TrackBar? owner)
                    || ParentInternal is not { } parent
                    || !owner.IsHandleCreated
                    || !base.IsDisplayed)
                {
                    return false;
                }

                return owner.Orientation == Orientation.Vertical || parent.RTLLayoutDisabled
                    ? owner.Maximum != owner.Value
                    : owner.Minimum != owner.Value;
            }
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out TrackBar? owner) || !owner.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                UiaCore.NavigateDirection.PreviousSibling => null,
                UiaCore.NavigateDirection.NextSibling => IsDisplayed ? ParentInternal?.ThumbAccessibleObject : null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 1;

        internal override void Invoke()
        {
            if (this.TryGetOwnerAs(out TrackBar? owner) && owner.IsHandleCreated)
            {
                // The "GetChildId" method returns to the id of the trackbar element,
                // which allows to use the native "accDoDefaultAction" method when the "Invoke" method is called
                ParentInternal?.SystemIAccessible.TryDoDefaultAction(GetChildId());
            }
        }
    }
}
