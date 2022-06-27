// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class TrackBar
    {
        internal class TrackBarAccessibleObject : ControlAccessibleObject
        {
            private readonly TrackBar _owningTrackBar;
            private TrackBarFirstButtonAccessibleObject? _firstButtonAccessibleObject;
            private TrackBarLastButtonAccessibleObject? _lastButtonAccessibleObject;
            private TrackBarThumbAccessibleObject? _thumbAccessibleObject;

            public TrackBarAccessibleObject(TrackBar owningTrackBar) : base(owningTrackBar)
            {
                _owningTrackBar = owningTrackBar;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningTrackBar.IsHandleCreated || GetSystemIAccessibleInternal() is not Accessibility.IAccessible systemIAccessible)
                    {
                        return Rectangle.Empty;
                    }

                    // The "NativeMethods.CHILDID_SELF" constant returns to the id of the trackbar,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    systemIAccessible.accLocation(out int left, out int top, out int width, out int height, NativeMethods.CHILDID_SELF);

                    return new(left, top, width, height);
                }
            }

            public override string? DefaultAction => _owningTrackBar.AccessibleDefaultActionDescription;

            public override AccessibleRole Role
                => Owner.AccessibleRole != AccessibleRole.Default
                    ? Owner.AccessibleRole
                    : AccessibleRole.Slider;

            public override AccessibleStates State

                // The "NativeMethods.CHILDID_SELF" constant returns to the id of the trackbar,
                // which allows to use the native "get_accState" method to get the "State" property
                => GetSystemIAccessibleInternal()?.get_accState(NativeMethods.CHILDID_SELF) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal TrackBarFirstButtonAccessibleObject FirstButtonAccessibleObject
                => _firstButtonAccessibleObject ??= new(_owningTrackBar);

            internal bool IsMirrored
                => _owningTrackBar.RightToLeft == RightToLeft.Yes && _owningTrackBar.RightToLeftLayout;

            internal TrackBarLastButtonAccessibleObject LastButtonAccessibleObject
                => _lastButtonAccessibleObject ??= new(_owningTrackBar);

            internal bool RTLLayoutDisabled
                => _owningTrackBar.RightToLeft == RightToLeft.Yes && !_owningTrackBar.RightToLeftLayout;

            internal TrackBarThumbAccessibleObject ThumbAccessibleObject
                => _thumbAccessibleObject ??= new TrackBarThumbAccessibleObject(_owningTrackBar);

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningTrackBar.IsHandleCreated)
                {
                    return null;
                }

                return index switch
                {
                    0 => FirstButtonAccessibleObject.IsDisplayed ? FirstButtonAccessibleObject : ThumbAccessibleObject,
                    1 => FirstButtonAccessibleObject.IsDisplayed ? ThumbAccessibleObject : LastButtonAccessibleObject,
                    2 => FirstButtonAccessibleObject.IsDisplayed && LastButtonAccessibleObject.IsDisplayed
                         ? LastButtonAccessibleObject
                         : null,
                    _ => null
                };
            }

            public override int GetChildCount()
            {
                if (!_owningTrackBar.IsHandleCreated)
                {
                    return -1;
                }

                // Both buttons cannot be hidden at the same time. Even if the minimum and maximum values are equal,
                // the placeholder for one of the buttons will still be displayed
                return FirstButtonAccessibleObject.IsDisplayed && LastButtonAccessibleObject.IsDisplayed
                    ? 3
                    : 2;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningTrackBar.IsHandleCreated)
                {
                    return null;
                }

                Point point = new(x, y);
                if (ThumbAccessibleObject.Bounds.Contains(point))
                {
                    return ThumbAccessibleObject;
                }

                if (FirstButtonAccessibleObject.IsDisplayed && FirstButtonAccessibleObject.Bounds.Contains(point))
                {
                    return FirstButtonAccessibleObject;
                }

                if (LastButtonAccessibleObject.IsDisplayed && LastButtonAccessibleObject.Bounds.Contains(point))
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
                if (!_owningTrackBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => LastButtonAccessibleObject.IsDisplayed
                                                            ? LastButtonAccessibleObject
                                                            : ThumbAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => _owningTrackBar.AccessibleRole == AccessibleRole.Default
                                                            ? UiaCore.UIA.SliderControlTypeId
                                                            : base.GetPropertyValue(propertyID),
                    UiaCore.UIA.AutomationIdPropertyId => _owningTrackBar.Name,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningTrackBar.Focused,
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
}
