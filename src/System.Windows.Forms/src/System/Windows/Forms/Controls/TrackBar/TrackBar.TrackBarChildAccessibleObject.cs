// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TrackBar
{
    internal abstract class TrackBarChildAccessibleObject : AccessibleObject, IOwnedObject<TrackBar>
    {
        private readonly WeakReference<TrackBar> _owningTrackBar;
        private int[]? _runtimeId;

        public TrackBarChildAccessibleObject(TrackBar owningTrackBar)
            => _owningTrackBar = new(owningTrackBar.OrThrowIfNull());

        public override Rectangle Bounds
        {
            get
            {
                if (!this.IsOwnerHandleCreated(out TrackBar? _) || !IsDisplayed)
                {
                    return Rectangle.Empty;
                }

                // The "GetChildId" method returns to the id of the trackbar element,
                // which allows to use the native "accLocation" method to get the "Bounds" property
                return ParentInternal?.SystemIAccessible.TryGetLocation(GetChildId()) ?? Rectangle.Empty;
            }
        }

        public override string? Help => GetHelpInternal().ToNullableStringAndFree();

        private protected override bool IsInternal => true;

        internal override BSTR GetHelpInternal()
            => ParentInternal is { } parent ? parent.SystemIAccessible.TryGetHelp(GetChildId()) : default;

        public override AccessibleRole Role
            => ParentInternal?.SystemIAccessible.TryGetRole(GetChildId()) ?? AccessibleRole.None;

        public override AccessibleStates State
            => ParentInternal?.SystemIAccessible.TryGetState(GetChildId()) ?? AccessibleStates.None;

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => ParentInternal;

        internal virtual bool IsDisplayed => this.TryGetOwnerAs(out TrackBar? trackbar) && trackbar.Visible;

        TrackBar? IOwnedObject<TrackBar>.Owner => _owningTrackBar.TryGetTarget(out TrackBar? target) ? target : null;

        internal TrackBarAccessibleObject? ParentInternal => this.TryGetOwnerAs(out TrackBar? owner)
            ? owner.AccessibilityObject as TrackBarAccessibleObject
            : null;

        internal override int[] RuntimeId => _runtimeId ??=
        [
            RuntimeIDFirstItem,
            (int)(this.TryGetOwnerAs(out TrackBar? owner) ? owner.InternalHandle : HWND.Null),
            GetChildId()
        ];

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent => ParentInternal,
                _ => base.FragmentNavigate(direction)
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(this.TryGetOwnerAs(out TrackBar? owner) && owner.Enabled),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                UIA_PATTERN_ID.UIA_InvokePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
