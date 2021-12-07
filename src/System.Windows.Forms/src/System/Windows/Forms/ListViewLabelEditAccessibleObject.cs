// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal class ListViewLabelEditAccessibleObject : AccessibleObject
    {
        private const string LIST_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

        private ListView _owner;
        private IntPtr _handle;
        private int[]? _runtimeId;

        public ListViewLabelEditAccessibleObject(ListView owner, IHandle handle)
        {
            _owner = owner;
            _handle = handle.Handle;
            UseStdAccessibleObjects(_handle);

            ListViewLabelEditUiaTextProvider textProvider = new(owner, handle, this);
            UseTextProviders(textProvider, textProvider);
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                UiaCore.UIA.ProcessIdPropertyId => Environment.ProcessId,
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                UiaCore.UIA.NamePropertyId => Name,
                UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                UiaCore.UIA.HasKeyboardFocusPropertyId => true,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                UiaCore.UIA.IsEnabledPropertyId => Enabled,
                UiaCore.UIA.AutomationIdPropertyId => LIST_VIEW_LABEL_EDIT_AUTOMATION_ID,
                UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                UiaCore.UIA.IsContentElementPropertyId => true,
                UiaCore.UIA.IsPasswordPropertyId => false,
                UiaCore.UIA.NativeWindowHandlePropertyId => _handle,
                UiaCore.UIA.IsOffscreenPropertyId => false,
                UiaCore.UIA.IsTextPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPatternId),
                UiaCore.UIA.IsTextPattern2AvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPattern2Id),
                UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId),
                _ => base.GetPropertyValue(propertyID),
            };

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            AccessibleObject parent = _owner.AccessibilityObject;

            return direction switch
            {
                UiaCore.NavigateDirection.Parent => parent,
                UiaCore.NavigateDirection.NextSibling => parent.GetChildIndex(this) is int childId and >= 0 ? parent.GetChild(childId + 1) : null,
                UiaCore.NavigateDirection.PreviousSibling => parent.GetChildIndex(this) is int childId and >= 0 ? parent.GetChild(childId - 1) : null,
                UiaCore.NavigateDirection.FirstChild or UiaCore.NavigateDirection.LastChild => null,
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
        {
            UiaCore.UIA.TextPatternId => true,
            UiaCore.UIA.TextPattern2Id => true,
            UiaCore.UIA.ValuePatternId => true,
            UiaCore.UIA.LegacyIAccessiblePatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        internal override int[] RuntimeId => _runtimeId ??= new int[] { RuntimeIDFirstItem, PARAM.ToInt(_handle) };

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => _owner.AccessibilityObject;

        internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
        {
            get
            {
                UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UiaCore.IRawElementProviderSimple provider);
                return provider;
            }
        }

        public override string Name => User32.GetWindowText(_handle);

        private bool Enabled
        {
            get
            {
                Debug.Assert(_owner.Enabled);
                return _owner.Enabled;
            }
        }
    }
}
