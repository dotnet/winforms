// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  ComboBox control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ComboBoxExAccessibleObject and ComboBoxAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        internal class ComboBoxAccessibleObject : ControlAccessibleObject
        {
            private const int COMBOBOX_ACC_ITEM_INDEX = 1;

            private ComboBoxChildDropDownButtonUiaProvider? _dropDownButtonUiaProvider;
            private readonly ComboBox _owningComboBox;

            /// <summary>
            ///  Initializes new instance of ComboBoxAccessibleObject.
            /// </summary>
            /// <param name="owningComboBox">The owning ComboBox control.</param>
            public ComboBoxAccessibleObject(ComboBox owningComboBox) : base(owningComboBox)
            {
                _owningComboBox = owningComboBox;
                ItemAccessibleObjects = new ComboBoxItemAccessibleObjectCollection(owningComboBox);
            }

            private void ComboBoxDefaultAction(bool expand)
            {
                if (_owningComboBox.IsHandleCreated && _owningComboBox.DroppedDown != expand)
                {
                    _owningComboBox.DroppedDown = expand;
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owningComboBox != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
                {
                    if (_owningComboBox.DropDownStyle == ComboBoxStyle.Simple)
                    {
                        return false;
                    }

                    return true;
                }

                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override int[]? RuntimeId
            {
                get
                {
                    if (_owningComboBox != null)
                    {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item is static - 0x2a (RuntimeIDFirstItem)
                        // second item can be anything, but here it is a hash

                        var runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem;
                        runtimeId[1] = (int)(long)_owningComboBox.InternalHandle;
                        runtimeId[2] = _owningComboBox.GetHashCode();

                        return runtimeId;
                    }

                    return base.RuntimeId;
                }
            }

            internal override void Expand()
            {
                ComboBoxDefaultAction(true);
            }

            internal override void Collapse()
            {
                ComboBoxDefaultAction(false);
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _owningComboBox.IsHandleCreated && _owningComboBox.DroppedDown ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override string? get_accNameInternal(object childID)
            {
                ValidateChildID(ref childID);

                if ((int)childID == COMBOBOX_ACC_ITEM_INDEX)
                {
                    return Name;
                }

                return base.get_accNameInternal(childID);
            }

            internal override string? get_accKeyboardShortcutInternal(object childID)
            {
                ValidateChildID(ref childID);
                if ((int)childID == COMBOBOX_ACC_ITEM_INDEX)
                {
                    return KeyboardShortcut;
                }

                return base.get_accKeyboardShortcutInternal(childID);
            }

            /// <summary>
            ///  Gets the collection of item accessible objects.
            /// </summary>
            public ComboBoxItemAccessibleObjectCollection ItemAccessibleObjects { get; }

            /// <summary>
            ///  Gets the DropDown button accessible object. (UI Automation provider)
            /// </summary>
            public ComboBoxChildDropDownButtonUiaProvider DropDownButtonUiaProvider
            {
                get
                {
                    if (_dropDownButtonUiaProvider is null)
                    {
                        _dropDownButtonUiaProvider = new ComboBoxChildDropDownButtonUiaProvider(_owningComboBox, _owningComboBox.InternalHandle);
                    }

                    return _dropDownButtonUiaProvider;
                }
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.FirstChild)
                {
                    return GetChildFragment(0);
                }
                else if (direction == UiaCore.NavigateDirection.LastChild)
                {
                    var childFragmentCount = GetChildFragmentCount();
                    if (childFragmentCount > 0)
                    {
                        return GetChildFragment(childFragmentCount - 1);
                    }
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this;
                }
            }

            internal override UiaCore.IRawElementProviderSimple? GetOverrideProviderForHwnd(IntPtr hwnd)
            {
                if (hwnd == _owningComboBox._childEdit.Handle)
                {
                    return _owningComboBox.ChildEditAccessibleObject;
                }
                else if (
                    hwnd == _owningComboBox._childListBox.Handle ||
                    hwnd == _owningComboBox._dropDownHandle)
                {
                    return _owningComboBox.ChildListAccessibleObject;
                }

                return null;
            }

            /// <summary>
            ///  Gets the accessible child corresponding to the specified index.
            /// </summary>
            /// <param name="index">The child index.</param>
            /// <returns>The accessible child.</returns>
            /// <remarks>
            ///  GetChild method should be unchanged to not break the MSAA scenarios.
            /// </remarks>
            internal AccessibleObject? GetChildFragment(int index)
            {
                if (_owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    if (index == 0)
                    {
                        return _owningComboBox.ChildTextAccessibleObject;
                    }

                    index--;
                }

                if (index == 0 && _owningComboBox.DropDownStyle != ComboBoxStyle.Simple)
                {
                    return DropDownButtonUiaProvider;
                }

                return null;
            }

            /// <summary>
            ///  Gets the number of children belonging to an accessible object.
            /// </summary>
            /// <returns>The number of children.</returns>
            /// <remarks>
            ///  GetChildCount method should be unchanged to not break the MSAA scenarios.
            /// </remarks>
            internal int GetChildFragmentCount()
            {
                int childFragmentCount = 0;

                if (_owningComboBox.DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    childFragmentCount++; // Text instead of edit for style is DropDownList but not DropDown.
                }

                if (_owningComboBox.DropDownStyle != ComboBoxStyle.Simple)
                {
                    childFragmentCount++; // DropDown button.
                }

                return childFragmentCount;
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ComboBoxControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _owningComboBox.Focused;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _owningComboBox.InternalHandle;
                    case UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
                    case UiaCore.UIA.IsValuePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.ValuePatternId);

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal void ResetListItemAccessibleObjects()
            {
                ItemAccessibleObjects.Clear();
            }

            internal void SetComboBoxItemFocus()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                var selectedItem = _owningComboBox.SelectedItem;
                if (selectedItem is null)
                {
                    return;
                }

                if (ItemAccessibleObjects[selectedItem] is ComboBoxItemAccessibleObject itemAccessibleObject)
                {
                    itemAccessibleObject.SetFocus();
                }
            }

            internal void SetComboBoxItemSelection()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                var selectedItem = _owningComboBox.SelectedItem;
                if (selectedItem is null)
                {
                    return;
                }

                if (ItemAccessibleObjects[selectedItem] is ComboBoxItemAccessibleObject itemAccessibleObject)
                {
                    itemAccessibleObject.RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
                }
            }

            internal override void SetFocus()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                base.SetFocus();

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }
        }
    }
}
