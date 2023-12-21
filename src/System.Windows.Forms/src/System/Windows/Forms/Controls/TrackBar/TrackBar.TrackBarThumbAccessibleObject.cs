// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TrackBar
{
    internal sealed class TrackBarThumbAccessibleObject : TrackBarChildAccessibleObject
    {
        public TrackBarThumbAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
        { }

        public override string? Name => SR.TrackBarPositionButtonName;

        private protected override bool IsInternal => true;

        internal override bool CanGetNameInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling
                    => ParentInternal?.FirstButtonAccessibleObject?.IsDisplayed ?? false
                        ? ParentInternal.FirstButtonAccessibleObject
                        : null,
                NavigateDirection.NavigateDirection_NextSibling
                    => ParentInternal?.LastButtonAccessibleObject?.IsDisplayed ?? false
                        ? ParentInternal.LastButtonAccessibleObject
                        : null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 2;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_InvokePatternId => false,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
