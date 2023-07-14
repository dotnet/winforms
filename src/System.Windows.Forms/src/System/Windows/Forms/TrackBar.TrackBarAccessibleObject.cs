// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

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

        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
        {
            AccessibleObject? element = HitTest((int)x, (int)y);

            return element ?? base.ElementProviderFromPoint(x, y);
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out TrackBar? _))
            {
                return null;
            }

            return direction switch
            {
                UiaCore.NavigateDirection.FirstChild => GetChild(0),
                UiaCore.NavigateDirection.LastChild => (LastButtonAccessibleObject?.IsDisplayed ?? false)
                    ? LastButtonAccessibleObject
                    : ThumbAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => UiaCore.UIA.SliderControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out TrackBar? owner) && owner.Focused,
                UiaCore.UIA.IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => true,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.ValuePatternId => true,
                UiaCore.UIA.LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
