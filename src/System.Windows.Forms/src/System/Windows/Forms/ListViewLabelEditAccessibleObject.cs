// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal class ListViewLabelEditAccessibleObject : AccessibleObject
    {
        private const string LIST_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

        private readonly ListView _owningListView;
        private readonly IntPtr _handle;
        private readonly ListViewLabelEditUiaTextProvider _textProvider;
        private int[]? _runtimeId;

        public ListViewLabelEditAccessibleObject(ListView owningListView, IHandle handle)
        {
            _owningListView = owningListView.OrThrowIfNull();
            _handle = handle.Handle;
            UseStdAccessibleObjects(_handle);
            _textProvider = new ListViewLabelEditUiaTextProvider(owningListView, handle, this);
        }

        private protected override string AutomationId => LIST_VIEW_LABEL_EDIT_AUTOMATION_ID;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            AccessibleObject parent = _owningListView.AccessibilityObject;

            return direction switch
            {
                UiaCore.NavigateDirection.Parent => parent,
                UiaCore.NavigateDirection.NextSibling => parent.GetChildIndex(this) is int childId and >= 0 ? parent.GetChild(childId + 1) : null,
                UiaCore.NavigateDirection.PreviousSibling => parent.GetChildIndex(this) is int childId and >= 0 ? parent.GetChild(childId - 1) : null,
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningListView.AccessibilityObject;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ProcessIdPropertyId => Environment.ProcessId,
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                UiaCore.UIA.HasKeyboardFocusPropertyId => true,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                UiaCore.UIA.IsEnabledPropertyId => _owningListView.Enabled,
                UiaCore.UIA.IsContentElementPropertyId => true,
                UiaCore.UIA.NativeWindowHandlePropertyId => _handle,
                _ => base.GetPropertyValue(propertyID),
            };

        internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
        {
            get
            {
                UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UiaCore.IRawElementProviderSimple provider);
                return provider;
            }
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
        {
            UiaCore.UIA.TextPatternId => true,
            UiaCore.UIA.TextPattern2Id => true,
            UiaCore.UIA.ValuePatternId => true,
            UiaCore.UIA.LegacyIAccessiblePatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        public override string Name => User32.GetWindowText(_handle);

        internal override int[] RuntimeId => _runtimeId ??= new int[] { RuntimeIDFirstItem, PARAM.ToInt(_handle) };

        internal override UiaCore.ITextRangeProvider DocumentRangeInternal
            => _textProvider.DocumentRange;

        internal override UiaCore.ITextRangeProvider[]? GetTextSelection()
            => _textProvider.GetSelection();

        internal override UiaCore.ITextRangeProvider[]? GetTextVisibleRanges()
            => _textProvider.GetVisibleRanges();

        internal override UiaCore.ITextRangeProvider? GetTextRangeFromChild(UiaCore.IRawElementProviderSimple childElement)
            => _textProvider.RangeFromChild(childElement);

        internal override UiaCore.ITextRangeProvider? GetTextRangeFromPoint(Point screenLocation)
            => _textProvider.RangeFromPoint(screenLocation);

        internal override UiaCore.SupportedTextSelection SupportedTextSelectionInternal
            => _textProvider.SupportedTextSelection;

        internal override UiaCore.ITextRangeProvider? GetTextCaretRange(out BOOL isActive)
            => _textProvider.GetCaretRange(out isActive);

        internal override UiaCore.ITextRangeProvider GetRangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
            => _textProvider.RangeFromAnnotation(annotationElement);
    }
}
