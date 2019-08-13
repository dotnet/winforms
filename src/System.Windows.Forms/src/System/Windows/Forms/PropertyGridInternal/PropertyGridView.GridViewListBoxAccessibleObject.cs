// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        /// <summary>
        ///  Represents the PropertyGridView ListBox accessibility object.
        /// </summary>
        [ComVisible(true)]
        private class GridViewListBoxAccessibleObject : ControlAccessibleObject
        {
            private readonly GridViewListBox _owningGridViewListBox;
            private readonly PropertyGridView _owningPropertyGridView;
            private readonly GridViewListBoxItemAccessibleObjectCollection _itemAccessibleObjects;

            /// <summary>
            ///  Constructs the new instance of GridViewListBoxAccessibleObject.
            /// </summary>
            /// <param name="owningGridViewListBox">The owning GridViewListBox.</param>
            public GridViewListBoxAccessibleObject(GridViewListBox owningGridViewListBox) : base(owningGridViewListBox)
            {
                _owningGridViewListBox = owningGridViewListBox;
                _owningPropertyGridView = owningGridViewListBox.OwningPropertyGridView;
                _itemAccessibleObjects = new GridViewListBoxItemAccessibleObjectCollection(owningGridViewListBox);
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                if (direction == UnsafeNativeMethods.NavigateDirection.Parent)
                {
                    return _owningPropertyGridView.SelectedGridEntry.AccessibilityObject;
                }
                else if (direction == UnsafeNativeMethods.NavigateDirection.FirstChild)
                {
                    return GetChildFragment(0);
                }
                else if (direction == UnsafeNativeMethods.NavigateDirection.LastChild)
                {
                    var childFragmentCount = GetChildFragmentCount();
                    if (childFragmentCount > 0)
                    {
                        return GetChildFragment(childFragmentCount - 1);
                    }
                }
                else if (direction == UnsafeNativeMethods.NavigateDirection.NextSibling)
                {
                    return _owningPropertyGridView.Edit.AccessibilityObject;
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningPropertyGridView.AccessibilityObject;
                }
            }

            public AccessibleObject GetChildFragment(int index)
            {
                if (index < 0 || index >= _owningGridViewListBox.Items.Count)
                {
                    return null;
                }

                var item = _owningGridViewListBox.Items[index];
                return _itemAccessibleObjects[item] as AccessibleObject;
            }

            public int GetChildFragmentCount()
            {
                return _owningGridViewListBox.Items.Count;
            }

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyId">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_ListControlTypeId;
                }
                else if (propertyID == NativeMethods.UIA_NamePropertyId)
                {
                    return Name;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);

                base.SetFocus();
            }

            internal void SetListBoxItemFocus()
            {
                var selectedItem = _owningGridViewListBox.SelectedItem;
                if (_itemAccessibleObjects[selectedItem] is AccessibleObject itemAccessibleObject)
                {
                    itemAccessibleObject.SetFocus();
                }
            }
        }
    }
}
