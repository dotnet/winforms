// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        [ComVisible(true)]
        private class GridViewListBoxItemAccessibleObject : AccessibleObject
        {
            private readonly GridViewListBox _owningGridViewListBox;
            private readonly object _owningItem;

            public GridViewListBoxItemAccessibleObject(GridViewListBox owningGridViewListBox, object owningItem)
            {
                _owningGridViewListBox = owningGridViewListBox;
                _owningItem = owningItem;

                UseStdAccessibleObjects(_owningGridViewListBox.Handle);
            }

            /// <summary>
            ///  Gets the DropDown button bounds.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    systemIAccessible.accLocation(out int left, out int top, out int width, out int height, GetChildId());
                    return new Rectangle(left, top, width, height);
                }
            }

            /// <summary>
            ///  Gets the DropDown button default action.
            /// </summary>
            public override string DefaultAction
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.accDefaultAction[GetChildId()];
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return _owningGridViewListBox.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        int currentIndex = GetCurrentIndex();
                        if (_owningGridViewListBox.AccessibilityObject is GridViewListBoxAccessibleObject gridViewListBoxAccessibleObject)
                        {
                            int itemsCount = gridViewListBoxAccessibleObject.GetChildFragmentCount();
                            int nextItemIndex = currentIndex + 1;
                            if (itemsCount > nextItemIndex)
                            {
                                return gridViewListBoxAccessibleObject.GetChildFragment(nextItemIndex);
                            }
                        }
                        break;
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        currentIndex = GetCurrentIndex();
                        gridViewListBoxAccessibleObject = _owningGridViewListBox.AccessibilityObject as GridViewListBoxAccessibleObject;
                        if (gridViewListBoxAccessibleObject != null)
                        {
                            var itemsCount = gridViewListBoxAccessibleObject.GetChildFragmentCount();
                            int previousItemIndex = currentIndex - 1;
                            if (previousItemIndex >= 0)
                            {
                                return gridViewListBoxAccessibleObject.GetChildFragment(previousItemIndex);
                            }
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningGridViewListBox.AccessibilityObject;
                }
            }

            private int GetCurrentIndex()
            {
                return _owningGridViewListBox.Items.IndexOf(_owningItem);
            }

            internal override int GetChildId()
            {
                return GetCurrentIndex() + 1; // Index is zero-based, Child ID is 1-based.
            }

            internal override object GetPropertyValue(int propertyID)
            {
                switch (propertyID)
                {
                    case NativeMethods.UIA_RuntimeIdPropertyId:
                        return RuntimeId;
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ListItemControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return KeyboardShortcut;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return _owningGridViewListBox.Focused;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return _owningGridViewListBox.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            ///  Gets the help text.
            /// </summary>
            public override string Help
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.accHelp[GetChildId()];
                }
            }

            /// <summary>
            ///  Gets the keyboard shortcut.
            /// </summary>
            public override string KeyboardShortcut
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.get_accKeyboardShortcut(GetChildId());
                }
            }

            /// <summary>
            ///  Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_LegacyIAccessiblePatternId ||
                    patternId == NativeMethods.UIA_InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            ///  Gets or sets the accessible name.
            /// </summary>
            public override string Name
            {
                get
                {
                    if (_owningGridViewListBox != null)
                    {
                        return _owningItem.ToString();
                    }

                    return base.Name;
                }

                set
                {
                    base.Name = value;
                }
            }

            /// <summary>
            ///  Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return (AccessibleRole)systemIAccessible.get_accRole(GetChildId());
                }
            }

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningGridViewListBox.Handle;
                    runtimeId[2] = _owningItem.GetHashCode();

                    return runtimeId;
                }
            }

            /// <summary>
            ///  Gets the accessible state.
            /// </summary>
            public override AccessibleStates State
            {
                get
                {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return (AccessibleStates)systemIAccessible.get_accState(GetChildId());
                }
            }

            internal override void SetFocus()
            {
                RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);

                base.SetFocus();
            }
        }
    }
}
