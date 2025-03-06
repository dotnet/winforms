// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal unsafe class LabelEditAccessibleObject : AccessibleObject
{
    private readonly WeakReference<LabelEditNativeWindow> _labelEdit;
    private readonly LabelEditUiaTextProvider _textProvider;
    private int[]? _runtimeId;

    public LabelEditAccessibleObject(Control owningControl, LabelEditNativeWindow labelEdit)
    {
        ArgumentNullException.ThrowIfNull(owningControl);

        _labelEdit = new(labelEdit);
        UseStdAccessibleObjects(labelEdit.Handle);
        _textProvider = new(owningControl, labelEdit, this);
    }

    internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        => direction switch
        {
            NavigateDirection.NavigateDirection_Parent => Parent,
            _ => base.FragmentNavigate(direction)
        };

    internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
        => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_ProcessIdPropertyId => (VARIANT)Environment.ProcessId,
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
            UIA_PROPERTY_ID.UIA_AccessKeyPropertyId => (VARIANT)string.Empty,
            UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.True,
            UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
            UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => VARIANT.True,
            UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => UIAHelper.WindowHandleToVariant(_labelEdit.TryGetTarget(out var target) ? target.HWND : HWND.Null),
            _ => base.GetPropertyValue(propertyID),
        };

    internal override IRawElementProviderSimple* HostRawElementProvider
    {
        get
        {
            if (_labelEdit.TryGetTarget(out var target))
            {
                PInvoke.UiaHostProviderFromHwnd(target.HWND, out IRawElementProviderSimple* provider);
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
        ? PInvokeCore.GetWindowText(target)
        : null;

    private protected override bool IsInternal => true;

    internal override bool CanGetNameInternal => false;

    internal override int[] RuntimeId => _runtimeId ??=
    [
        RuntimeIDFirstItem,
        _labelEdit.TryGetTarget(out var target) ? (int)target.HWND : (int)HWND.Null
    ];

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
