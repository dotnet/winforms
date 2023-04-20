// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

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
                if (!OwningTrackBar.IsHandleCreated || !IsDisplayed)
                {
                    return Rectangle.Empty;
                }

                // The "GetChildId" method returns to the id of the trackbar element,
                // which allows to use the native "accLocation" method to get the "Bounds" property
                return ParentInternal.SystemIAccessible.TryGetLocation(GetChildId());
            }
        }

        public override string? Help => ParentInternal.SystemIAccessible.TryGetHelp(GetChildId());

        public override AccessibleRole Role
            => ParentInternal.SystemIAccessible.TryGetRole(GetChildId());

        public override AccessibleStates State
            => ParentInternal.SystemIAccessible.TryGetState(GetChildId());

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
                UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                UiaCore.UIA.IsEnabledPropertyId => OwningTrackBar.Enabled,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
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
