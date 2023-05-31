// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

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
                if (!this.TryGetOwnerAs(out TrackBar? owner) || !owner.IsHandleCreated || !IsDisplayed)
                {
                    return Rectangle.Empty;
                }

                // The "GetChildId" method returns to the id of the trackbar element,
                // which allows to use the native "accLocation" method to get the "Bounds" property
                return ParentInternal?.SystemIAccessible.TryGetLocation(GetChildId()) ?? Rectangle.Empty;
            }
        }

        public override string? Help => ParentInternal?.SystemIAccessible.TryGetHelp(GetChildId());

        public override AccessibleRole Role
            => ParentInternal?.SystemIAccessible.TryGetRole(GetChildId()) ?? AccessibleRole.None;

        public override AccessibleStates State
            => ParentInternal?.SystemIAccessible.TryGetState(GetChildId()) ?? AccessibleStates.None;

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => ParentInternal;

        internal virtual bool IsDisplayed => this.TryGetOwnerAs(out TrackBar? trackbar) && trackbar.Visible;

        TrackBar? IOwnedObject<TrackBar>.Owner => _owningTrackBar.TryGetTarget(out TrackBar? target) ? target : null;

        internal TrackBarAccessibleObject? ParentInternal => this.TryGetOwnerAs(out TrackBar? owner)
            ? owner.AccessibilityObject as TrackBarAccessibleObject
            : null;

        internal override int[] RuntimeId
            => _runtimeId ??= new int[]
            {
                RuntimeIDFirstItem,
                (int)(this.TryGetOwnerAs(out TrackBar? owner) ? owner.InternalHandle : HWND.Null),
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
                UiaCore.UIA.IsEnabledPropertyId => this.TryGetOwnerAs(out TrackBar? owner) && owner.Enabled,
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
