// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class TrackBar
    {
        internal abstract class TrackBarChildAccessibleObject : AccessibleObject
        {
            public TrackBarChildAccessibleObject(TrackBar owningTrackBar)
            {
                OwningTrackBar = owningTrackBar ?? throw new ArgumentNullException(nameof(owningTrackBar));
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!OwningTrackBar.IsHandleCreated || !IsDisplayed)
                    {
                        return Rectangle.Empty;
                    }

                    int left = 0;
                    int top = 0;
                    int width = 0;
                    int height = 0;

                    // The "GetChildId" method returns to the id of the trackbar element,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    ParentInternal.GetSystemIAccessibleInternal()?.accLocation(out left, out top, out width, out height, GetChildId());

                    return new(left, top, width, height);
                }
            }

            public override string? Help => ParentInternal.GetSystemIAccessibleInternal()?.get_accHelp(GetChildId());

            public override AccessibleRole Role
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accRole(GetChildId()) is object accRole
                    ? (AccessibleRole)accRole
                    : AccessibleRole.None;

            public override AccessibleStates State
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accState(GetChildId()) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => ParentInternal;

            internal virtual bool IsDisplayed => OwningTrackBar.Visible;

            internal TrackBar OwningTrackBar { get; private set; }

            internal TrackBarAccessibleObject ParentInternal => (TrackBarAccessibleObject)OwningTrackBar.AccessibilityObject;

            internal override int[]? RuntimeId
            {
                get
                {
                    if (!OwningTrackBar.IsHandleCreated)
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = PARAM.ToInt(OwningTrackBar.InternalHandle);
                    runtimeId[2] = GetChildId();

                    return runtimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => ParentInternal,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId => OwningTrackBar.Enabled,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                    UiaCore.UIA.IsPasswordPropertyId => false,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    UiaCore.UIA.InvokePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
