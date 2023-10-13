﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox's child (inner) edit native window control accessible object with UI Automation provider functionality.
    /// </summary>
    internal unsafe class ComboBoxChildEditUiaProvider : ChildAccessibleObject
    {
        private const string COMBO_BOX_EDIT_AUTOMATION_ID = "1001";

        private readonly ComboBox _owningComboBox;
        private readonly ComboBoxUiaTextProvider _textProvider;
        private readonly IntPtr _handle;

        /// <summary>
        ///  Initializes new instance of ComboBoxChildEditUiaProvider.
        /// </summary>
        /// <param name="owner">The ComboBox owning control.</param>
        /// <param name="childEditControlhandle">The child edit native window handle.</param>
        public ComboBoxChildEditUiaProvider(ComboBox owner, IntPtr childEditControlhandle) : base(owner, childEditControlhandle)
        {
            _owningComboBox = owner;
            _handle = childEditControlhandle;
            _textProvider = new ComboBoxUiaTextProvider(owner);
        }

        private protected override string AutomationId => COMBO_BOX_EDIT_AUTOMATION_ID;

        /// <summary>
        ///  Returns the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!_owningComboBox.IsHandleCreated ||
                // Created is set to false in WM_DESTROY, but the window Handle is released on NCDESTROY, which comes after DESTROY.
                // But between these calls, AccessibleObject can be recreated and might cause memory leaks.
                !_owningComboBox.Created)
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    return _owningComboBox.AccessibilityObject;
                case UiaCore.NavigateDirection.PreviousSibling:
                    return _owningComboBox.DroppedDown
                        ? _owningComboBox.ChildListAccessibleObject
                        : null;
                case UiaCore.NavigateDirection.NextSibling:
                    return _owningComboBox.DropDownStyle != ComboBoxStyle.Simple
                        && _owningComboBox.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                            ? comboBoxAccessibleObject.DropDownButtonUiaProvider
                            : null;
                default:
                    return base.FragmentNavigate(direction);
            }
        }

        /// <summary>
        ///  Gets the top level element.
        /// </summary>
        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => _owningComboBox.AccessibilityObject;

        public override string Name => base.Name ?? SR.ComboBoxEditDefaultAccessibleName;

        /// <summary>
        ///  Gets the accessible property value.
        /// </summary>
        /// <param name="propertyID">The accessible property ID.</param>
        /// <returns>The accessible property value.</returns>
        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => _owningComboBox.Focused,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => _owningComboBox.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => false,
                UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => _handle,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UiaCore.IRawElementProviderSimple provider);
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

        /// <summary>
        ///  Gets the runtime ID.
        /// </summary>
        internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, GetHashCode() };

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
