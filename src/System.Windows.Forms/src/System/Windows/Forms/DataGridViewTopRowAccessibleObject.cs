﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        [
            System.Runtime.InteropServices.ComVisible(true)
        ]
        protected class DataGridViewTopRowAccessibleObject : AccessibleObject
        {
            private int[] runtimeId;
            DataGridView owner;

            public DataGridViewTopRowAccessibleObject() : base()
            {
            }

            public DataGridViewTopRowAccessibleObject(DataGridView owner) : base()
            {
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet));
                    }
                    if (this.owner.ColumnHeadersVisible)
                    {
                        Rectangle rect = Rectangle.Union(this.owner.layout.ColumnHeaders, this.owner.layout.TopLeftHeader);
                        return this.owner.RectangleToScreen(rect);
                    }
                    else
                    {
                        return Rectangle.Empty;
                    }
                }
            }

            public override string Name
            {
                get
                {
                    return string.Format(SR.DataGridView_AccTopRow);
                }
            }

            public DataGridView Owner
            {
                get
                {
                    return this.owner;
                }
                set
                {
                    if (this.owner != null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerAlreadySet));
                    }
                    this.owner = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    if (this.owner == null)
                    {
                        throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet));
                    }
                    return this.owner.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Row;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = this.Parent.GetHashCode();
                        runtimeId[2] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            public override string Value
            {
                get
                {
                    return this.Name;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet));
                }
                
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index == 0 && this.owner.RowHeadersVisible)
                {
                    return this.owner.TopLeftHeaderCell.AccessibilityObject;
                }

                if (this.owner.RowHeadersVisible)
                {
                    // decrement the index because the first child is the top left header cell
                    index --;
                }

                Debug.Assert(index >= 0);

                if (index < this.owner.Columns.GetColumnCount(DataGridViewElementStates.Visible))
                {
                    int actualColumnIndex = this.owner.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return this.owner.Columns[actualColumnIndex].HeaderCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet));
                }
                int result = this.owner.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (this.owner.RowHeadersVisible)
                {
                    // + 1 is the top left header cell accessibility object
                    result ++;
                }

                return result;
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (this.owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet));
                }
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (this.owner.AccessibilityObject.GetChildCount() > 1)
                        {
                            return this.owner.AccessibilityObject.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.FirstChild:
                        return this.GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return this.GetChild(this.GetChildCount() - 1);
                    default:
                        return null;
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return this.Bounds;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this.owner.AccessibilityObject;
                }
            }

            [return: MarshalAs(UnmanagedType.IUnknown)]
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return Parent;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        if (Parent.GetChildCount() > 1)
                        {
                            return Parent.GetChild(1);
                        }
                        break;
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(0);
                        }
                        break;
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(GetChildCount() - 1);
                        }
                        break;
                }

                return null;
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object GetPropertyValue(int propertyId)
            {
                switch (propertyId)
                {
                    case NativeMethods.UIA_NamePropertyId:
                        return string.Format(SR.DataGridView_AccTopRow);
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return false;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return owner.Enabled;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return false;
                    case NativeMethods.UIA_IsContentElementPropertyId:
                        return true;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_IsLegacyIAccessiblePatternAvailablePropertyId:
                        return true;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
