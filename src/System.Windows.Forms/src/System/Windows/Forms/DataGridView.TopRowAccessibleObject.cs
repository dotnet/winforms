// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        protected class DataGridViewTopRowAccessibleObject : AccessibleObject
        {
            private int[] runtimeId;
            private DataGridView owner;

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
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                    }

                    if (owner.IsHandleCreated && owner.ColumnHeadersVisible)
                    {
                        Rectangle rect = Rectangle.Union(owner._layout.ColumnHeaders, owner._layout.TopLeftHeader);
                        return owner.RectangleToScreen(rect);
                    }
                    return Rectangle.Empty;
                }
            }

            public override string Name
            {
                get
                {
                    return SR.DataGridView_AccTopRow;
                }
            }

            public DataGridView Owner
            {
                get
                {
                    return owner;
                }
                set
                {
                    if (owner != null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerAlreadySet);
                    }
                    owner = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                    }
                    return owner.AccessibilityObject;
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
                    if (runtimeId is null)
                    {
                        runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = Parent.GetHashCode();
                        runtimeId[2] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            public override string Value
            {
                get
                {
                    return Name;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index == 0 && owner.RowHeadersVisible)
                {
                    return owner.TopLeftHeaderCell.AccessibilityObject;
                }

                if (owner.RowHeadersVisible)
                {
                    // decrement the index because the first child is the top left header cell
                    index--;
                }

                Debug.Assert(index >= 0);

                if (index < owner.Columns.GetColumnCount(DataGridViewElementStates.Visible))
                {
                    int actualColumnIndex = owner.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return owner.Columns[actualColumnIndex].HeaderCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }
                int result = owner.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (owner.RowHeadersVisible)
                {
                    // + 1 is the top left header cell accessibility object
                    result++;
                }

                return result;
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (owner.AccessibilityObject.GetChildCount() > 1)
                        {
                            return owner.AccessibilityObject.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.FirstChild:
                        return GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return GetChild(GetChildCount() - 1);
                    default:
                        return null;
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return Bounds;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return owner.AccessibilityObject;
                }
            }

            [return: MarshalAs(UnmanagedType.IUnknown)]
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (Parent.GetChildCount() > 1)
                        {
                            return Parent.GetChild(1);
                        }
                        break;
                    case UiaCore.NavigateDirection.FirstChild:
                        if (GetChildCount() > 0)
                        {
                            return GetChild(0);
                        }
                        break;
                    case UiaCore.NavigateDirection.LastChild:
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

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId))
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return SR.DataGridView_AccTopRow;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return false;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return owner.Enabled;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                    case UiaCore.UIA.HelpTextPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return true;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
