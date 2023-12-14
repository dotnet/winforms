// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TrackBar
{
    internal sealed class TrackBarLastButtonAccessibleObject : TrackBarChildAccessibleObject
    {
        public TrackBarLastButtonAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
        { }

        public override string DefaultAction => SR.AccessibleActionPress;

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        public override string? Name => !this.TryGetOwnerAs(out TrackBar? owner) || ParentInternal is not { } parent
            ? null
            : owner.Orientation == Orientation.Horizontal && (owner.RightToLeft == RightToLeft.No || parent.IsMirrored)
                ? SR.TrackBarLargeIncreaseButtonName
                : SR.TrackBarLargeDecreaseButtonName;

        internal override bool CanGetNameInternal => false;

        public override AccessibleStates State
            => !this.IsOwnerHandleCreated(out TrackBar? _) || IsDisplayed
                ? AccessibleStates.None
                : AccessibleStates.Invisible;

        internal override bool IsDisplayed
        {
            get
            {
                if (!this.IsOwnerHandleCreated(out TrackBar? owner)
                    || ParentInternal is not { } parent
                    || !base.IsDisplayed)
                {
                    return false;
                }

                if (owner.Minimum == owner.Maximum)
                {
                    return true;
                }

                return owner.Orientation == Orientation.Vertical || parent.RTLLayoutDisabled
                    ? owner.Minimum != owner.Value
                    : owner.Maximum != owner.Value;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling => IsDisplayed ? ParentInternal?.ThumbAccessibleObject : null,
                NavigateDirection.NavigateDirection_NextSibling => null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 3;

        internal override void Invoke()
        {
            if (this.IsOwnerHandleCreated(out TrackBar? _))
            {
                // The "GetChildId" method returns to the id of the trackbar element,
                // which allows to use the native "accDoDefaultAction" method when the "Invoke" method is called
                ParentInternal?.SystemIAccessible.TryDoDefaultAction(GetChildId());
            }
        }
    }
}
