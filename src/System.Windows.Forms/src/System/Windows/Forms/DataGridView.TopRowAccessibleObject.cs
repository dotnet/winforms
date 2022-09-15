// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            private int[]? runtimeId;
            private DataGridView? _ownerDataGridView;

            public DataGridViewTopRowAccessibleObject()
            {
            }

            public DataGridViewTopRowAccessibleObject(DataGridView owner) : base()
            {
                _ownerDataGridView = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (_ownerDataGridView is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                    }

                    if (_ownerDataGridView.IsHandleCreated && _ownerDataGridView.ColumnHeadersVisible)
                    {
                        Rectangle rect = Rectangle.Union(_ownerDataGridView._layout.ColumnHeaders, _ownerDataGridView._layout.TopLeftHeader);
                        return _ownerDataGridView.RectangleToScreen(rect);
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

            public DataGridView? Owner
            {
                get
                {
                    return _ownerDataGridView;
                }
                set
                {
                    if (_ownerDataGridView is not null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerAlreadySet);
                    }

                    _ownerDataGridView = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    if (_ownerDataGridView is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                    }

                    return _ownerDataGridView.AccessibilityObject;
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
                => runtimeId ??= new int[]
                {
                    RuntimeIDFirstItem, // first item is static - 0x2a
                    Parent.GetHashCode(),
                    GetHashCode()
                };

            public override string Value
            {
                get
                {
                    return Name;
                }
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (_ownerDataGridView is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index > GetChildCount() - 1)
                {
                    return null;
                }

                if (index == 0 && _ownerDataGridView.RowHeadersVisible)
                {
                    return _ownerDataGridView.TopLeftHeaderCell.AccessibilityObject;
                }

                if (_ownerDataGridView.RowHeadersVisible)
                {
                    // decrement the index because the first child is the top left header cell
                    index--;
                }

                Debug.Assert(index >= 0);

                if (index < _ownerDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
                {
                    int actualColumnIndex = _ownerDataGridView.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return _ownerDataGridView.Columns[actualColumnIndex].HeaderCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                if (_ownerDataGridView is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }

                int result = _ownerDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (_ownerDataGridView.RowHeadersVisible)
                {
                    // + 1 is the top left header cell accessibility object
                    result++;
                }

                return result;
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                if (_ownerDataGridView is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (_ownerDataGridView.AccessibilityObject.GetChildCount() > 1)
                        {
                            return _ownerDataGridView.AccessibilityObject.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }

                    case AccessibleNavigation.FirstChild:
                        return GetChildCount() > 0
                            ? GetChild(0)
                            : null;
                    case AccessibleNavigation.LastChild:
                        int childCount = GetChildCount();
                        return childCount > 0
                            ? GetChild(childCount - 1)
                            : null;
                    default:
                        return null;
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    if (_ownerDataGridView is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                    }

                    return _ownerDataGridView.AccessibilityObject;
                }
            }

            [return: MarshalAs(UnmanagedType.IUnknown)]
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (Parent.GetChildCount() > 1)
                        {
                            if (_ownerDataGridView is null)
                            {
                                throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet);
                            }

                            return _ownerDataGridView.Rows.Count == 0 ? null : Parent.GetChild(1);
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

            internal override object? GetPropertyValue(UiaCore.UIA propertyId) =>
                propertyId switch
                {
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsContentElementPropertyId => true,
                    UiaCore.UIA.IsEnabledPropertyId => _ownerDataGridView is null
                        ? throw new InvalidOperationException(SR.DataGridViewTopRowAccessibleObject_OwnerNotSet)
                        : _ownerDataGridView.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    UiaCore.UIA.IsOffscreenPropertyId => false,
                    _ => base.GetPropertyValue(propertyId)
                };

            #endregion
        }
    }
}
