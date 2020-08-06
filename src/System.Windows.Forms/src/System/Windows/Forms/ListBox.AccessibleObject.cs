// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Accessibility;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  ListBox control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ListBoxExAccessibleObject and ListBoxAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        [ComVisible(true)]
        internal class ListBoxAccessibleObject : ControlAccessibleObject
        {
            private readonly Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject> _itemAccessibleObjects;
            private readonly ListBox _owningListBox;
            private readonly IAccessible _systemIAccessible;

            /// <summary>
            ///  Initializes new instance of ListBoxAccessibleObject.
            /// </summary>
            /// <param name="owningListBox">The owning ListBox control.</param>
            public ListBoxAccessibleObject(ListBox owningListBox) : base(owningListBox)
            {
                _owningListBox = owningListBox;
                _itemAccessibleObjects = new Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject>();
                _systemIAccessible = GetSystemIAccessibleInternal();
            }

            internal override Rectangle BoundingRectangle => _owningListBox.GetToolNativeScreenRectangle();

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot => this;

            internal override bool IsSelectionRequired => true;

            internal override int[] RuntimeId
            {
                get
                {
                    if (_owningListBox == null)
                    {
                        return base.RuntimeId;
                    }

                    // we need to provide a unique ID
                    // others are implementing this in the same manner
                    // first item is static - 0x2a (RuntimeIDFirstItem)
                    // second item can be anything, but here it is a hash

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningListBox.Handle;
                    runtimeId[2] = _owningListBox.GetHashCode();

                    return runtimeId;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;

                    if (_owningListBox.Focused)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            /// <param name="x">X coordinate.</param>
            /// <param name="y">Y coordinate.</param>
            /// <returns>The accessible object of corresponding element in the provided coordinates.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject element = HitTest((int)x, (int)y);

                if (element != null)
                {
                    return element;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                int childCount = _owningListBox.Items.Count;

                if (childCount == 0)
                {
                    return null;
                }

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        return GetChild(0);
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        return GetChild(childCount - 1);
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(int propertyID)
            {
                switch (propertyID)
                {
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ListControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        bool result = _owningListBox.HasKeyboardFocus && _owningListBox.Focused;
                        if (GetChildCount() > 0)
                        {
                            _owningListBox.HasKeyboardFocus = false;
                        }
                        return result;
                    case NativeMethods.UIA_NativeWindowHandlePropertyId:
                        return _owningListBox.Handle;
                    case NativeMethods.UIA_IsSelectionPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_SelectionPatternId);
                    case NativeMethods.UIA_IsScrollPatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_ScrollPatternId);
                    case NativeMethods.UIA_IsLegacyIAccessiblePatternAvailablePropertyId:
                        return IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetSelection()
            {
                AccessibleObject itemAccessibleObject = GetSelected();
                if (itemAccessibleObject != null)
                {
                    return new UnsafeNativeMethods.IRawElementProviderSimple[]
                    {
                        itemAccessibleObject
                    };
                }

                return new UnsafeNativeMethods.IRawElementProviderSimple[0];
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owningListBox != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_ScrollPatternId ||
                    patternId == NativeMethods.UIA_SelectionPatternId ||
                    patternId == NativeMethods.UIA_LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal void ResetListItemAccessibleObjects()
            {
                _itemAccessibleObjects.Clear();
            }

            internal override void SelectItem()
            {
                GetChild(_owningListBox.FocusedIndex).SelectItem();
            }

            internal override void SetFocus()
            {
                AccessibleObject focusedItem = GetFocused();
                focusedItem.RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
                focusedItem.SetFocus();
            }

            internal override void SetValue(string newValue)
            {
                Value = newValue;
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0 || index >= _owningListBox.Items.Count)
                {
                    return null;
                }

                ItemArray.Entry item = _owningListBox.Items.InnerArray.Entries[index];
                if (!_itemAccessibleObjects.ContainsKey(item))
                {
                    _itemAccessibleObjects.Add(item, new ListBoxItemAccessibleObject(_owningListBox, item, this));
                }

                return _itemAccessibleObjects[item];
            }

            public override int GetChildCount()
            {
                return _owningListBox.Items.Count;
            }

            public override AccessibleObject GetFocused()
            {
                int index = _owningListBox.FocusedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject GetSelected()
            {
                int index = _owningListBox.SelectedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                // Within a child element?
                //
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);
                    if (child.Bounds.Contains(x, y))
                    {
                        _owningListBox.HasKeyboardFocus = false;
                        return child;
                    }
                }

                // Within the ListBox bounds?
                //
                if (Bounds.Contains(x, y))
                {
                    _owningListBox.HasKeyboardFocus = true;
                    return this;
                }

                return null;
            }
        }
    }
}
