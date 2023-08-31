// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

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

        public override unsafe Rectangle Bounds => SystemIAccessible.TryGetLocation(GetChildId());

        public override unsafe string? DefaultAction => SystemIAccessible.TryGetDefaultAction(GetChildId());

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!_owner.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                UiaCore.NavigateDirection.Parent => _owner.AccessibilityObject,
                UiaCore.NavigateDirection.PreviousSibling =>
                    _owner.DropDownStyle == ComboBoxStyle.DropDownList
                        ? _owner.ChildTextAccessibleObject
                        : _owner.ChildEditAccessibleObject,
                // We should return null for NextSibling because it is always the last item in the tree
                UiaCore.NavigateDirection.NextSibling => null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _owner.AccessibilityObject;

        internal override int GetChildId() => COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => _owner.Focused,
                UiaCore.UIA.IsEnabledPropertyId => _owner.Enabled,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Help => SystemIAccessible.TryGetHelp(GetChildId());

        public override string? KeyboardShortcut
            => SystemIAccessible.TryGetKeyboardShortcut(GetChildId());

        internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
        {
            UiaCore.UIA.LegacyIAccessiblePatternId or UiaCore.UIA.InvokePatternId => true,
            _ => base.IsPatternSupported(patternId)
        };

        public override AccessibleRole Role => SystemIAccessible.TryGetRole(GetChildId());

        internal override int[] RuntimeId
            => new int[]
            {
                RuntimeIDFirstItem,
                PARAM.ToInt(_owner.InternalHandle),
                _owner.GetHashCode(),
                GeneratedRuntimeId,
                COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX
            };

        public override AccessibleStates State => SystemIAccessible.TryGetState(GetChildId());
    }
}
