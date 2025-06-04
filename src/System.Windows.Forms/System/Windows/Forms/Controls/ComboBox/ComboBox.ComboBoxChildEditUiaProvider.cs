// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox's child (inner) edit native window control accessible object with UI Automation
    ///  provider functionality.
    /// </summary>
    internal unsafe class ComboBoxChildEditUiaProvider : ChildAccessibleObject
    {
        private const string COMBO_BOX_EDIT_AUTOMATION_ID = "1001";

        private readonly ComboBox _owningComboBox;
        private readonly ComboBoxUiaTextProvider _textProvider;
        private readonly HWND _handle;

        /// <summary>
        ///  Initializes new instance of ComboBoxChildEditUiaProvider.
        /// </summary>
        /// <param name="owner">The ComboBox owning control.</param>
        /// <param name="childEditControlhandle">The child edit native window handle.</param>
        public ComboBoxChildEditUiaProvider(ComboBox owner, HWND childEditControlhandle) : base(owner, childEditControlhandle)
        {
            _owningComboBox = owner;
            _handle = childEditControlhandle;
            _textProvider = new ComboBoxUiaTextProvider(owner);
        }

        private protected override string AutomationId => COMBO_BOX_EDIT_AUTOMATION_ID;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!_owningComboBox.IsHandleCreated ||
                // Created is set to false in WM_DESTROY, but the window Handle is released on NCDESTROY, which comes after DESTROY.
                // But between these calls, AccessibleObject can be recreated and might cause memory leaks.
                !_owningComboBox.Created)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => _owningComboBox.AccessibilityObject,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _owningComboBox.DroppedDown
                        ? _owningComboBox.ChildListAccessibleObject
                        : null,
                NavigateDirection.NavigateDirection_NextSibling
                    => _owningComboBox.DropDownStyle != ComboBoxStyle.Simple
                        && _owningComboBox.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                        ? comboBoxAccessibleObject.DropDownButtonUiaProvider
                        : null,
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningComboBox.AccessibilityObject;

        public override string Name => base.Name ?? SR.ComboBoxEditDefaultAccessibleName;

        private protected override bool IsInternal => true;

        internal override bool CanGetNameInternal => false;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)_owningComboBox.Focused,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningComboBox.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => UIAHelper.WindowHandleToVariant(_handle),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple* HostRawElementProvider
        {
            get
            {
                PInvoke.UiaHostProviderFromHwnd(new HandleRef<HWND>(this, _handle), out IRawElementProviderSimple* provider);
                return provider;
            }
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) =>
            patternId switch
            {
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                UIA_PATTERN_ID.UIA_TextPatternId => true,
                UIA_PATTERN_ID.UIA_TextPattern2Id => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal override int[] RuntimeId => [RuntimeIDFirstItem, GetHashCode()];

        internal override unsafe ITextRangeProvider* DocumentRangeInternal
            => _textProvider.DocumentRange;

        internal override unsafe HRESULT GetTextSelection(SAFEARRAY** pRetVal) => _textProvider.GetSelection(pRetVal);

        internal override unsafe HRESULT GetTextVisibleRanges(SAFEARRAY** pRetVal) => _textProvider.GetVisibleRanges(pRetVal);

        internal override unsafe HRESULT GetTextRangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
            => _textProvider.RangeFromChild(childElement, pRetVal);

        internal override unsafe HRESULT GetTextRangeFromPoint(UiaPoint screenLocation, ITextRangeProvider** pRetVal)
            => _textProvider.RangeFromPoint(screenLocation, pRetVal);

        internal override SupportedTextSelection SupportedTextSelectionInternal => _textProvider.SupportedTextSelection;

        internal override unsafe HRESULT GetTextCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
            => _textProvider.GetCaretRange(isActive, pRetVal);

        internal override unsafe HRESULT GetRangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
            => _textProvider.RangeFromAnnotation(annotationElement, pRetVal);
    }
}
