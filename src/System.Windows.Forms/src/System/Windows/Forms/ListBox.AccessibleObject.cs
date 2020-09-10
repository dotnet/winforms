// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        /// <summary>
        ///  ListBox control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ListBoxAccessibleObject : ControlAccessibleObject
        {
            private readonly Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject> _itemAccessibleObjects;
            private readonly ListBox _owningListBox;

            /// <summary>
            ///  Initializes new instance of ListBoxAccessibleObject.
            /// </summary>
            /// <param name="owningListBox">The owning ListBox control.</param>
            public ListBoxAccessibleObject(ListBox owningListBox) : base(owningListBox)
            {
                _owningListBox = owningListBox;
                _itemAccessibleObjects = new Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject>();
            }

            internal override Rectangle BoundingRectangle => _owningListBox.IsHandleCreated ?
                                                             _owningListBox.GetToolNativeScreenRectangle() : Rectangle.Empty;

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            internal override bool IsSelectionRequired => true;

            internal override int[]? RuntimeId
            {
                get
                {
                    if (_owningListBox is null)
                    {
                        return base.RuntimeId;
                    }

                    // we need to provide a unique ID
                    // others are implementing this in the same manner
                    // first item is static - 0x2a (RuntimeIDFirstItem)
                    // second item can be anything, but here it is a hash

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningListBox.InternalHandle;
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
            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return base.ElementProviderFromPoint(x, y);
                }

                AccessibleObject? element = HitTest((int)x, (int)y);

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
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                int childCount = _owningListBox.Items.Count;

                if (childCount == 0)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => GetChild(childCount - 1),
                    _ => base.FragmentNavigate(direction),
                };
            }

            internal override UiaCore.IRawElementProviderFragment? GetFocus() => _owningListBox.IsHandleCreated ? GetFocused() : null;

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ListControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        bool result = _owningListBox.HasKeyboardFocus && _owningListBox.Focused;
                        if (GetChildCount() > 0)
                        {
                            _owningListBox.HasKeyboardFocus = false;
                        }
                        return result;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _owningListBox.InternalHandle;
                    case UiaCore.UIA.IsSelectionPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.SelectionPatternId);
                    case UiaCore.UIA.IsScrollPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.ScrollPatternId);
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override UiaCore.IRawElementProviderSimple[] GetSelection()
            {
                AccessibleObject? itemAccessibleObject = GetSelected();
                if (itemAccessibleObject != null)
                {
                    return new UiaCore.IRawElementProviderSimple[]
                    {
                        itemAccessibleObject
                    };
                }

                return Array.Empty<UiaCore.IRawElementProviderSimple>();
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owningListBox != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ScrollPatternId ||
                    patternId == UiaCore.UIA.SelectionPatternId)
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
                if (_owningListBox.IsHandleCreated)
                {
                    GetChild(_owningListBox.FocusedIndex)?.SelectItem();
                }
            }

            internal override void SetFocus()
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return;
                }

                AccessibleObject? focusedItem = GetFocused();
                focusedItem?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                focusedItem?.SetFocus();
            }

            internal override void SetValue(string? newValue)
            {
                Value = newValue;
            }

            public override AccessibleObject? GetChild(int index)
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

            public override AccessibleObject? GetFocused()
            {
                int index = _owningListBox.FocusedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject? GetSelected()
            {
                int index = _owningListBox.SelectedIndex;

                if (index >= 0)
                {
                    return GetChild(index);
                }

                return null;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return null;
                }

                // Within a child element?
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject? child = GetChild(index);
                    Debug.Assert(child != null, $"GetChild({index}) returned null");
                    if (child != null && child.Bounds.Contains(x, y))
                    {
                        _owningListBox.HasKeyboardFocus = false;
                        return child;
                    }
                }

                // Within the ListBox bounds?
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
