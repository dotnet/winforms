// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox child (inner) DropDown button accessible object with UI Automation functionality.
    /// </summary>
    internal sealed class ComboBoxChildDropDownButtonUiaProvider : AccessibleObject
    {
        private const int COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX = 2;

        // Made up constant from MSAA proxy. When MSAA proxy is used as an accessibility provider, the similar Runtime ID
        // is returned (for consistency purpose).
        private const int GeneratedRuntimeId = 61453;
        private readonly ComboBox _owner;

        /// <summary>
        ///  Initializes new instance of ComboBoxChildDropDownButtonUiaProvider.
        /// </summary>
        /// <param name="owner">The owning ComboBox control.</param>
        public ComboBoxChildDropDownButtonUiaProvider(ComboBox owner)
        {
            _owner = owner;
            UseStdAccessibleObjects(owner.InternalHandle);
        }

        /// <inheritdoc/>
        /// <remarks>
        ///  <para>
        ///   "Open" or "Close" depending on the state of the DropDown.
        ///  </para>
        /// </remarks>
        public override string Name => _owner.DroppedDown ? SR.ComboboxDropDownButtonCloseName : SR.ComboboxDropDownButtonOpenName;

        internal override bool CanGetNameInternal => false;

        public override unsafe Rectangle Bounds => SystemIAccessible.TryGetLocation(GetChildId());

        public override string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

        private protected override bool IsInternal => true;

        internal override BSTR GetDefaultActionInternal() => SystemIAccessible.TryGetDefaultAction(GetChildId());

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!_owner.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => _owner.AccessibilityObject,
                NavigateDirection.NavigateDirection_PreviousSibling =>
                    _owner.DropDownStyle == ComboBoxStyle.DropDownList
                        ? _owner.ChildTextAccessibleObject
                        : _owner.ChildEditAccessibleObject,
                // We should return null for NextSibling because it is always the last item in the tree
                NavigateDirection.NavigateDirection_NextSibling => null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owner.AccessibilityObject;

        internal override int GetChildId() => COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)_owner.Focused,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owner.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Help => GetHelpInternal().ToNullableStringAndFree();

        internal override BSTR GetHelpInternal() => SystemIAccessible.TryGetHelp(GetChildId());

        public override string? KeyboardShortcut => GetKeyboardShortcutInternal((VARIANT)GetChildId()).ToNullableStringAndFree();

        internal override BSTR GetKeyboardShortcutInternal(VARIANT childID) => SystemIAccessible.TryGetKeyboardShortcut(childID);

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId or UIA_PATTERN_ID.UIA_InvokePatternId => true,
            _ => base.IsPatternSupported(patternId)
        };

        public override AccessibleRole Role => SystemIAccessible.TryGetRole(GetChildId());

        internal override int[] RuntimeId =>
            [
                RuntimeIDFirstItem,
                (int)_owner.InternalHandle,
                _owner.GetHashCode(),
                GeneratedRuntimeId,
                COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX
            ];

        public override AccessibleStates State => SystemIAccessible.TryGetState(GetChildId());
    }
}
