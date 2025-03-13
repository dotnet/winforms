// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TrackBar
{
    internal sealed class TrackBarAccessibleObject : ControlAccessibleObject
    {
        private TrackBarFirstButtonAccessibleObject? _firstButtonAccessibleObject;
        private TrackBarLastButtonAccessibleObject? _lastButtonAccessibleObject;
        private TrackBarThumbAccessibleObject? _thumbAccessibleObject;

        public TrackBarAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
        {
        }

        public override Rectangle Bounds
        {
            get
            {
                if (!this.IsOwnerHandleCreated(out TrackBar? _))
                {
                    return Rectangle.Empty;
                }

                // The CHILDID_SELF constant returns to the id of the trackbar, which allows to use the native
                // "accLocation" method to get the "Bounds" property
                return SystemIAccessible.TryGetLocation(CHILDID_SELF);
            }
        }

        public override string? DefaultAction => this.TryGetOwnerAs(out TrackBar? owner)
            ? owner.AccessibleDefaultActionDescription
            : null;

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Slider);

        public override AccessibleStates State

            // The CHILDID_SELF constant returns to the id of the trackbar, which allows to use the native
            // "get_accState" method to get the "State" property
            => SystemIAccessible.TryGetState(CHILDID_SELF);

        internal TrackBarFirstButtonAccessibleObject? FirstButtonAccessibleObject
            => _firstButtonAccessibleObject ??= (this.TryGetOwnerAs(out TrackBar? owner) ? new(owner) : null);

        internal bool IsMirrored
            => this.TryGetOwnerAs(out TrackBar? owner) && owner.RightToLeft == RightToLeft.Yes && owner.RightToLeftLayout;

        internal TrackBarLastButtonAccessibleObject? LastButtonAccessibleObject
            => _lastButtonAccessibleObject ??= (this.TryGetOwnerAs(out TrackBar? owner) ? new(owner) : null);

        internal bool RTLLayoutDisabled
            => this.TryGetOwnerAs(out TrackBar? owner) && owner.RightToLeft == RightToLeft.Yes && !owner.RightToLeftLayout;

        internal TrackBarThumbAccessibleObject? ThumbAccessibleObject
            => _thumbAccessibleObject ??= (this.TryGetOwnerAs(out TrackBar? owner) ? new(owner) : null);

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            return index switch
            {
                0 => (FirstButtonAccessibleObject?.IsDisplayed ?? false) ? FirstButtonAccessibleObject : ThumbAccessibleObject,
                1 => (FirstButtonAccessibleObject?.IsDisplayed ?? false) ? ThumbAccessibleObject : LastButtonAccessibleObject,
                2 => (FirstButtonAccessibleObject?.IsDisplayed ?? false) && (LastButtonAccessibleObject?.IsDisplayed ?? false)
                     ? LastButtonAccessibleObject
                     : null,
                _ => null
            };
        }

        public override int GetChildCount()
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return -1;
            }

            // Both buttons cannot be hidden at the same time. Even if the minimum and maximum values are equal,
            // the placeholder for one of the buttons will still be displayed
            return (FirstButtonAccessibleObject?.IsDisplayed ?? false) && (LastButtonAccessibleObject?.IsDisplayed ?? false)
                ? 3
                : 2;
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            Point point = new(x, y);
            if (ThumbAccessibleObject?.Bounds.Contains(point) ?? false)
            {
                return ThumbAccessibleObject;
            }

            if ((FirstButtonAccessibleObject?.IsDisplayed ?? false) && FirstButtonAccessibleObject.Bounds.Contains(point))
            {
                return FirstButtonAccessibleObject;
            }

            if ((LastButtonAccessibleObject?.IsDisplayed ?? false) && LastButtonAccessibleObject.Bounds.Contains(point))
            {
                return LastButtonAccessibleObject;
            }

            return null;
        }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            AccessibleObject? element = HitTest((int)x, (int)y);

            return element ?? base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                NavigateDirection.NavigateDirection_LastChild => (LastButtonAccessibleObject?.IsDisplayed ?? false)
                    ? LastButtonAccessibleObject
                    : ThumbAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_SliderControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out TrackBar? owner) && owner.Focused),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
