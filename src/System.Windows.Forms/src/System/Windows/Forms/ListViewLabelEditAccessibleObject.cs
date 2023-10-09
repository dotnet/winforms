// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;
using UiaCore = Interop.UiaCore;
namespace System.Windows.Forms;

internal unsafe class ListViewLabelEditAccessibleObject : AccessibleObject
{
    private const string LIST_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

    private readonly ListView _owningListView;
    private readonly ListViewSubItem? _owningListViewSubItem;
    private readonly ListViewItem? _owingListViewItem;
    private readonly WeakReference<ListViewLabelEditNativeWindow> _labelEdit;
    private readonly ListViewLabelEditUiaTextProvider _textProvider;
    private int[]? _runtimeId;

    public ListViewLabelEditAccessibleObject(ListView owningListView, ListViewLabelEditNativeWindow labelEdit)
    {
        _owningListView = owningListView.OrThrowIfNull();
        _owningListViewSubItem = owningListView._listViewSubItem;
        _owingListViewItem = owningListView._selectedItem;
        _labelEdit = new(labelEdit);
        UseStdAccessibleObjects(labelEdit.Handle);
        _textProvider = new ListViewLabelEditUiaTextProvider(owningListView, labelEdit, this);
    }

    private protected override string AutomationId => LIST_VIEW_LABEL_EDIT_AUTOMATION_ID;

    public override AccessibleObject? Parent => _owningListViewSubItem is null
        ? _owingListViewItem?.AccessibilityObject
        : _owningListView.View == View.Tile
            ? _owingListViewItem?.AccessibilityObject
            : _owningListViewSubItem?.AccessibilityObject;

    internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        => direction switch
        {
            UiaCore.NavigateDirection.Parent => Parent,
            UiaCore.NavigateDirection.NextSibling
                => _owningListView.View == View.Tile ? _owingListViewItem?.SubItems[1].AccessibilityObject : null,
            _ => base.FragmentNavigate(direction)
        };

    internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningListView.AccessibilityObject;

    internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID)
        => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_ProcessIdPropertyId => Environment.ProcessId,
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
            UIA_PROPERTY_ID.UIA_AccessKeyPropertyId => string.Empty,
            UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => true,
            UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
            UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => _owningListView.Enabled,
            UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => true,
            UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => _labelEdit.TryGetTarget(out var target) ? (nint)target.HWND : 0,
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

    internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
    {
        UIA_PATTERN_ID.UIA_TextPatternId => true,
        UIA_PATTERN_ID.UIA_TextPattern2Id => true,
        UIA_PATTERN_ID.UIA_ValuePatternId => true,
        UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
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

    internal override ITextRangeProvider* DocumentRangeInternal
        => _textProvider.DocumentRange;

    internal override HRESULT GetTextSelection(SAFEARRAY** pRetVal)
        => _textProvider.GetSelection(pRetVal);

    internal override HRESULT GetTextVisibleRanges(SAFEARRAY** pRetVal)
        => _textProvider.GetVisibleRanges(pRetVal);

    internal override HRESULT GetTextRangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
        => _textProvider.RangeFromChild(childElement, pRetVal);

    internal override HRESULT GetTextRangeFromPoint(UiaPoint screenLocation, ITextRangeProvider** pRetVal)
        => _textProvider.RangeFromPoint(screenLocation, pRetVal);

    internal override SupportedTextSelection SupportedTextSelectionInternal
        => _textProvider.SupportedTextSelection;

    internal override HRESULT GetTextCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
        => _textProvider.GetCaretRange(isActive, pRetVal);

    internal override HRESULT GetRangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
        => _textProvider.RangeFromAnnotation(annotationElement, pRetVal);
}
