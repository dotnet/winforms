// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox's child (inner) list native window control accessible object
    ///  with UI Automation provider functionality.
    /// </summary>
    internal sealed class ComboBoxChildListUiaProvider : ChildAccessibleObject
    {
        private const string COMBO_BOX_LIST_AUTOMATION_ID = "1000";

        private readonly ComboBox _owningComboBox;
        private readonly HWND _childListControlhandle;

        public ComboBoxChildListUiaProvider(ComboBox owningComboBox, HWND childListControlhandle) : base(owningComboBox, childListControlhandle)
        {
            _owningComboBox = owningComboBox;
            _childListControlhandle = childListControlhandle;
        }

        private protected override string AutomationId => COMBO_BOX_LIST_AUTOMATION_ID;

        internal override Rectangle BoundingRectangle
        {
            get
            {
                PInvokeCore.GetWindowRect(_owningComboBox.GetListNativeWindow(), out var rect);
                return rect;
            }
        }

        internal override unsafe IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
            if (result.Succeeded)
            {
                result = accessible.Value->accHitTest((int)x, (int)y, out VARIANT child);
                if (result.Succeeded && child.vt == VARENUM.VT_I4)
                {
                    return GetChildFragment((int)child - 1);
                }
                else
                {
                    child.Dispose();
                    return null;
                }
            }

            return base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
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
                case NavigateDirection.NavigateDirection_Parent:
                    return _owningComboBox.AccessibilityObject;
                case NavigateDirection.NavigateDirection_FirstChild:
                    return GetChildFragment(0);
                case NavigateDirection.NavigateDirection_LastChild:
                    int childFragmentCount = GetChildFragmentCount();
                    if (childFragmentCount > 0)
                    {
                        return GetChildFragment(childFragmentCount - 1);
                    }

                    return null;
                case NavigateDirection.NavigateDirection_NextSibling:
                    return _owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList
                        ? _owningComboBox.ChildTextAccessibleObject
                        : _owningComboBox.ChildEditAccessibleObject;
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    // A workaround for an issue with an Inspect not responding. It also simulates native control behavior.
                    return _owningComboBox.DropDownStyle == ComboBoxStyle.Simple
                        ? _owningComboBox.ChildListAccessibleObject
                        : null;
                default:
                    return base.FragmentNavigate(direction);
            }
        }

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningComboBox.AccessibilityObject;

        public AccessibleObject? GetChildFragment(int index)
        {
            if (index < 0 || index >= _owningComboBox.Items.Count)
            {
                return null;
            }

            if (_owningComboBox.AccessibilityObject is not ComboBoxAccessibleObject comboBoxAccessibleObject)
            {
                return null;
            }

            Entry item = _owningComboBox.Entries[index];

            return item is null ? null : comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(item);
        }

        public int GetChildFragmentCount()
        {
            return _owningComboBox.Items.Count;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ListControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId =>
                    // Narrator should keep the keyboard focus on th ComboBox itself but not on the DropDown.
                    VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningComboBox.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => UIAHelper.WindowHandleToVariant(_childListControlhandle),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderFragment.Interface? GetFocus() => GetFocused();

        public override AccessibleObject? GetFocused()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return null;
            }

            int selectedIndex = _owningComboBox.SelectedIndex;
            return GetChildFragment(selectedIndex);
        }

        private protected override bool IsInternal => true;

        internal override IRawElementProviderSimple.Interface[] GetSelection()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return [];
            }

            int selectedIndex = _owningComboBox.SelectedIndex;

            AccessibleObject? itemAccessibleObject = GetChildFragment(selectedIndex);

            return itemAccessibleObject is not null
                ? [itemAccessibleObject]
                : [];
        }

        internal override bool CanSelectMultiple => false;

        internal override bool IsSelectionRequired => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId or UIA_PATTERN_ID.UIA_SelectionPatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        internal override unsafe IRawElementProviderSimple* HostRawElementProvider
        {
            get
            {
                PInvoke.UiaHostProviderFromHwnd(new HandleRef<HWND>(this, _childListControlhandle), out IRawElementProviderSimple* provider);
                return provider;
            }
        }

        internal override int[] RuntimeId =>
            [
                RuntimeIDFirstItem,
                (int)_owningComboBox.InternalHandle,
                _owningComboBox.GetListNativeWindowRuntimeIdPart()
            ];

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;
                if (_owningComboBox.Focused)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }
    }
}
