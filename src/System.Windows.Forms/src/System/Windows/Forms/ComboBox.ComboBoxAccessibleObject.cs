﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.ComboBox.ObjectCollection;
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
                if (_owningComboBox is not null)
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
                    if (_owningComboBox is not null)
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
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetFirstChild();
                    case UiaCore.NavigateDirection.LastChild:
                        return GetLastChild();
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this;
                }
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
                        // If we don't set a default role for the accessible object
                        // it will be retrieved from Windows.
                        // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                        return _owningComboBox.AccessibleRole == AccessibleRole.Default
                               ? UiaCore.UIA.ComboBoxControlTypeId
                               : base.GetPropertyValue(propertyID);
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

                GetSelectedComboBoxItemAccessibleObject()?.SetFocus();
            }

            internal void SetComboBoxItemSelection()
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return;
                }

                GetSelectedComboBoxItemAccessibleObject()?.RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
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

            private ComboBoxItemAccessibleObject? GetSelectedComboBoxItemAccessibleObject()
            {
                // We should use the SelectedIndex property instead of the SelectedItem to avoid the problem of getting
                // the incorrect item when the list contains duplicate items https://github.com/dotnet/winforms/issues/3590
                int selectedIndex = _owningComboBox.SelectedIndex;

                if (selectedIndex < 0 || selectedIndex > _owningComboBox.Items.Count - 1)
                {
                    return null;
                }

                Entry selectedItem = _owningComboBox.Entries[selectedIndex];
                return ItemAccessibleObjects.GetComboBoxItemAccessibleObject(selectedItem);
            }

            private AccessibleObject? GetFirstChild()
            {
                if (_owningComboBox.DroppedDown)
                {
                    return _owningComboBox.ChildListAccessibleObject;
                }

                return _owningComboBox.DropDownStyle switch
                {
                    ComboBoxStyle.DropDown => _owningComboBox.ChildEditAccessibleObject,
                    ComboBoxStyle.DropDownList => _owningComboBox.ChildTextAccessibleObject,
                    ComboBoxStyle.Simple => null,
                    _ => null
                };
            }

            private AccessibleObject? GetLastChild() =>
                _owningComboBox.DropDownStyle == ComboBoxStyle.Simple
                    ? _owningComboBox.ChildEditAccessibleObject
                    : DropDownButtonUiaProvider;
        }
    }
}
