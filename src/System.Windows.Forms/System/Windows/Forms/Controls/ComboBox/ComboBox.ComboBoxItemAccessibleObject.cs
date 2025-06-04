// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the <see cref="ComboBox"/> item accessible object.
    /// </summary>
    internal sealed class ComboBoxItemAccessibleObject : AccessibleObject
    {
        private readonly ComboBox _owningComboBox;
        private readonly Entry _owningItem;

        /// <summary>
        ///  Initializes new instance of <see cref="ComboBox"/> item accessible object.
        /// </summary>
        /// <param name="owningComboBox">The owning <see cref="ComboBox"/>.</param>
        /// <param name="owningItem">The owning <see cref="ComboBox"/> item.</param>
        public ComboBoxItemAccessibleObject(ComboBox owningComboBox, Entry owningItem)
        {
            _owningComboBox = owningComboBox.OrThrowIfNull();
            _owningItem = owningItem;
        }

        public override Rectangle Bounds
        {
            get
            {
                int currentIndex = GetCurrentIndex();
                var listHandle = _owningComboBox.GetListHandle();
                RECT itemRect = default;

                int result = (int)PInvokeCore.SendMessage(
                    listHandle,
                    PInvoke.LB_GETITEMRECT,
                    (WPARAM)currentIndex,
                    ref itemRect);

                if (result == PInvoke.LB_ERR)
                {
                    return Rectangle.Empty;
                }

                // Translate the item rect to screen coordinates
                RECT translated = itemRect;
                PInvokeCore.MapWindowPoints(listHandle, HWND.Null, ref translated);
                return translated;
            }
        }

        public override string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

        internal override BSTR GetDefaultActionInternal() =>
            _owningComboBox.ChildListAccessibleObject.SystemIAccessible.TryGetDefaultAction(GetChildId());

        private protected override bool IsInternal => true;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (direction == NavigateDirection.NavigateDirection_Parent)
            {
                return _owningComboBox.ChildListAccessibleObject;
            }

            if (_owningComboBox.ChildListAccessibleObject is not ComboBoxChildListUiaProvider comboBoxChildListUiaProvider)
            {
                return base.FragmentNavigate(direction);
            }

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_NextSibling:
                    int currentIndex = GetCurrentIndex();
                    int itemsCount = comboBoxChildListUiaProvider.GetChildFragmentCount();
                    int nextItemIndex = currentIndex + 1;
                    if (itemsCount > nextItemIndex)
                    {
                        return comboBoxChildListUiaProvider.GetChildFragment(nextItemIndex);
                    }

                    break;
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    currentIndex = GetCurrentIndex();
                    int previousItemIndex = currentIndex - 1;
                    if (previousItemIndex >= 0)
                    {
                        return comboBoxChildListUiaProvider.GetChildFragment(previousItemIndex);
                    }

                    break;
            }

            return base.FragmentNavigate(direction);
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningComboBox.AccessibilityObject;

        private int GetCurrentIndex() => _owningComboBox.Items.InnerList.IndexOf(_owningItem);

        // Index is zero-based, Child ID is 1-based.
        internal override int GetChildId() => GetCurrentIndex() + 1;

        internal override unsafe VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(_owningComboBox.Focused && _owningComboBox.SelectedIndex == GetCurrentIndex()),
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningComboBox.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_SelectionItemIsSelectedPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Selected),
                UIA_PROPERTY_ID.UIA_SelectionItemSelectionContainerPropertyId => (VARIANT)ComHelpers.GetComPointer<IUnknown>(_owningComboBox.ChildListAccessibleObject),
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Help => GetHelpInternal().ToNullableStringAndFree();

        internal override BSTR GetHelpInternal() => _owningComboBox.ChildListAccessibleObject.SystemIAccessible.TryGetHelp(GetChildId());

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            return patternId switch
            {
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId
                    or UIA_PATTERN_ID.UIA_InvokePatternId
                    or UIA_PATTERN_ID.UIA_ScrollItemPatternId
                    or UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
                _ => base.IsPatternSupported(patternId),
            };
        }

        public override string? Name => _owningComboBox is null ? base.Name : _owningComboBox.GetItemText(_owningItem.Item);

        internal override bool CanGetNameInternal => _owningComboBox is null;

        public override AccessibleRole Role
            => _owningComboBox.ChildListAccessibleObject.SystemIAccessible.TryGetRole(GetChildId());

        internal override int[] RuntimeId =>
        [
            RuntimeIDFirstItem,
            (int)_owningComboBox.InternalHandle,
            _owningComboBox.GetListNativeWindowRuntimeIdPart(),
            _owningItem.GetHashCode()
        ];

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = _owningComboBox.ChildListAccessibleObject.SystemIAccessible.TryGetState(GetChildId());

                if (!_owningComboBox.DroppedDown || !_owningComboBox.ChildListAccessibleObject.Bounds.IntersectsWith(Bounds))
                {
                    state |= AccessibleStates.Offscreen;
                }

                return state;
            }
        }

        internal override void ScrollIntoView()
        {
            if (!_owningComboBox.IsHandleCreated || !_owningComboBox.Enabled)
            {
                return;
            }

            Rectangle listBounds = _owningComboBox.ChildListAccessibleObject.Bounds;
            if (listBounds.IntersectsWith(Bounds))
            {
                // Do nothing because the item is already visible
                return;
            }

            PInvokeCore.SendMessage(_owningComboBox, PInvoke.CB_SETTOPINDEX, (WPARAM)GetCurrentIndex());
        }

        internal override void SetFocus()
        {
            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);

            base.SetFocus();
        }

        internal override unsafe void SelectItem()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

            _owningComboBox.SelectedIndex = GetCurrentIndex();
            PInvoke.InvalidateRect(_owningComboBox.GetListHandle(), lpRect: null, bErase: false);
        }

        internal override void AddToSelection()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

            SelectItem();
        }

        internal override void RemoveFromSelection()
        {
            // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
        }

        internal override bool IsItemSelected => (State & AccessibleStates.Selected) != 0;

        internal override IRawElementProviderSimple.Interface ItemSelectionContainer => _owningComboBox.ChildListAccessibleObject;
    }
}
