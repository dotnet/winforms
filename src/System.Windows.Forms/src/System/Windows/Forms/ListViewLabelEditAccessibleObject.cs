// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms;

internal class ListViewLabelEditAccessibleObject : AccessibleObject
{
    private const string LIST_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

    private readonly ListView _owningListView;
    private readonly ListViewSubItem _owningListViewSubItem;
    private readonly WeakReference<ListViewLabelEditNativeWindow> _labelEdit;
    private readonly ListViewLabelEditUiaTextProvider _textProvider;
    private int[]? _runtimeId;

    public ListViewLabelEditAccessibleObject(ListView owningListView, ListViewLabelEditNativeWindow labelEdit)
    {
        _owningListView = owningListView.OrThrowIfNull();
        _owningListViewSubItem = owningListView._listViewSubItem.OrThrowIfNull();
        _labelEdit = new(labelEdit);
        UseStdAccessibleObjects(labelEdit.Handle);
        _textProvider = new ListViewLabelEditUiaTextProvider(owningListView, labelEdit, this);
    }

    private protected override string AutomationId => LIST_VIEW_LABEL_EDIT_AUTOMATION_ID;

    public override AccessibleObject? Parent => _owningListViewSubItem.AccessibilityObject;

    internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
    {
        return direction switch
        {
            UiaCore.NavigateDirection.Parent => Parent,
            UiaCore.NavigateDirection.NextSibling => Parent?.GetChildIndex(this) is int childId and >= 0 ? Parent.GetChild(childId + 1) : null,
            UiaCore.NavigateDirection.PreviousSibling => Parent?.GetChildIndex(this) is int childId and >= 0 ? Parent.GetChild(childId - 1) : null,
            _ => base.FragmentNavigate(direction),
        };
    }

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
            UiaCore.UIA.NativeWindowHandlePropertyId => _labelEdit.TryGetTarget(out var target) ? (nint)target.HWND : 0,
            _ => base.GetPropertyValue(propertyID),
        };

    internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
    {
        get
        {
            if (_labelEdit.TryGetTarget(out var target))
            {
                UiaCore.UiaHostProviderFromHwnd(target.HWND, out UiaCore.IRawElementProviderSimple provider);
                return provider;
            }

            return null;
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

    public override string? Name => _labelEdit.TryGetTarget(out var target)
        ? PInvoke.GetWindowText(target)
        : null;

    internal override int[] RuntimeId => _runtimeId ??= new int[]
    {
        RuntimeIDFirstItem,
        _labelEdit.TryGetTarget(out var target) ? (int)target.HWND : (int)HWND.Null
    };

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
