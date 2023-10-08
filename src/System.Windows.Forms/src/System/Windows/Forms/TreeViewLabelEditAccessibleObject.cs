// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;
using UiaCore = Interop.UiaCore;

namespace System.Windows.Forms;

internal unsafe class TreeViewLabelEditAccessibleObject : AccessibleObject
{
    private const string TREE_VIEW_LABEL_EDIT_AUTOMATION_ID = "1";

    private readonly TreeView _owningTreeView;
    private readonly WeakReference<TreeViewLabelEditNativeWindow> _labelEdit;
    private readonly TreeViewLabelEditUiaTextProvider _textProvider;
    private int[]? _runtimeId;

    public TreeViewLabelEditAccessibleObject(TreeView owningTreeView, TreeViewLabelEditNativeWindow labelEdit)
    {
        _owningTreeView = owningTreeView.OrThrowIfNull();
        _labelEdit = new(labelEdit);
        UseStdAccessibleObjects(labelEdit.Handle);
        _textProvider = new TreeViewLabelEditUiaTextProvider(owningTreeView, labelEdit, this);
    }

    private protected override string AutomationId => TREE_VIEW_LABEL_EDIT_AUTOMATION_ID;

    public override AccessibleObject? Parent => _owningTreeView._editNode?.AccessibilityObject;

    internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
    {
        switch (direction)
        {
            case UiaCore.NavigateDirection.Parent:
                return Parent;
        }

        return base.FragmentNavigate(direction);
    }

    internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owningTreeView.AccessibilityObject;

    internal override object? GetPropertyValue(UiaCore.UIA propertyID)
        => propertyID switch
        {
            UiaCore.UIA.ProcessIdPropertyId => Environment.ProcessId,
            UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
            UiaCore.UIA.AccessKeyPropertyId => string.Empty,
            UiaCore.UIA.HasKeyboardFocusPropertyId => true,
            UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
            UiaCore.UIA.IsEnabledPropertyId => _owningTreeView.Enabled,
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
