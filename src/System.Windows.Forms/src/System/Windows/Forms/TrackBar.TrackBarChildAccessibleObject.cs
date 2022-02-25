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
                OwningTrackBar = owningTrackBar.OrThrowIfNull();
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!OwningTrackBar.IsHandleCreated || !IsDisplayed || ParentInternal.GetSystemIAccessibleInternal() is not Accessibility.IAccessible systemIAccessible)
                    {
                        return Rectangle.Empty;
                    }

                    // The "GetChildId" method returns to the id of the trackbar element,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    systemIAccessible.accLocation(out int left, out int top, out int width, out int height, GetChildId());

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

            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(OwningTrackBar.InternalHandle),
                    GetChildId()
                };

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => ParentInternal,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId => OwningTrackBar.Enabled,
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
