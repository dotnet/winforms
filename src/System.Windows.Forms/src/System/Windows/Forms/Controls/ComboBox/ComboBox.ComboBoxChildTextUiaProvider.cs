// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox's child text (is used instead of inner Edit when style is DropDownList
    ///  but not DropDown) accessible object.
    /// </summary>
    internal sealed class ComboBoxChildTextUiaProvider : AccessibleObject
    {
        private const int COMBOBOX_TEXT_ACC_ITEM_INDEX = 1;

        private readonly ComboBox _owner;

        /// <summary>
        ///  Initializes new instance of ComboBoxChildTextUiaProvider.
        /// </summary>
        /// <param name="owner">The owning ComboBox control.</param>
        public ComboBoxChildTextUiaProvider(ComboBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  Gets the bounds.
        /// </summary>
        public override Rectangle Bounds
        {
            get
            {
                return _owner.AccessibilityObject.Bounds;
            }
        }

        /// <summary>
        ///  Gets the child ID.
        /// </summary>
        /// <returns>The child ID.</returns>
        internal override int GetChildId()
        {
            return COMBOBOX_TEXT_ACC_ITEM_INDEX;
        }

        /// <summary>
        ///  Gets or sets the accessible Name of ComboBox's child text element.
        /// </summary>
        [AllowNull]
        public override string Name
        {
            get => _owner.AccessibilityObject.Name ?? string.Empty;
            set
            {
                // Do nothing.
            }
        }

        internal override bool CanGetNameInternal => false;

        internal override bool CanSetNameInternal => false;

        private protected override bool IsInternal => true;

        internal override BSTR GetNameInternal()
        {
            BSTR name = _owner.AccessibilityObject.GetNameInternal();
            return name.IsNull ? new(string.Empty) : name;
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!_owner.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => _owner.AccessibilityObject,
                NavigateDirection.NavigateDirection_NextSibling
                    => _owner.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                        ? comboBoxAccessibleObject.DropDownButtonUiaProvider
                        : null,
                NavigateDirection.NavigateDirection_PreviousSibling
                    => _owner.DroppedDown
                        ? _owner.ChildListAccessibleObject
                        : null,
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owner.AccessibilityObject;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TextControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)_owner.Focused,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owner.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override int[] RuntimeId =>
            [
                RuntimeIDFirstItem,
                (int)_owner.InternalHandle,
                _owner.GetHashCode(),
                GetHashCode(),
                GetChildId()
            ];

        /// <summary>
        ///  Gets the accessible state.
        /// </summary>
        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;
                if (_owner.Focused)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }
    }
}
