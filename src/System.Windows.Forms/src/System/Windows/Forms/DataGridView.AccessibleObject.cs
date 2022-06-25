// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        protected class DataGridViewAccessibleObject : ControlAccessibleObject
        {
            private int[]? runtimeId; // Used by UIAutomation
            private bool? _isModal;

            private readonly DataGridView _ownerDataGridView;
            private DataGridViewTopRowAccessibleObject? _topRowAccessibilityObject;
            private DataGridViewSelectedCellsAccessibleObject? _selectedCellsAccessibilityObject;

            public DataGridViewAccessibleObject(DataGridView owner)
                : base(owner) => _ownerDataGridView = owner;

            internal override bool IsReadOnly => _ownerDataGridView.ReadOnly;

            private bool IsModal
            {
                get
                {
                    _isModal ??= _ownerDataGridView.TopMostParent is Form { Modal: true };
                    return _isModal.Value;
                }
            }

            internal void ReleaseChildUiaProviders()
            {
                if (!OsVersion.IsWindows8OrGreater())
                {
                    return;
                }

                if (_topRowAccessibilityObject is not null)
                {
                    UiaCore.UiaDisconnectProvider(_topRowAccessibilityObject);
                    _topRowAccessibilityObject = null;
                }

                if (_selectedCellsAccessibilityObject is not null)
                {
                    UiaCore.UiaDisconnectProvider(_selectedCellsAccessibilityObject);
                    _selectedCellsAccessibilityObject = null;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = _ownerDataGridView.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    // the Default AccessibleRole is Table
                    return AccessibleRole.Table;
                }
            }

            private AccessibleObject TopRowAccessibilityObject
            {
                get
                {
                    _topRowAccessibilityObject ??= new DataGridViewTopRowAccessibleObject(_ownerDataGridView);

                    return _topRowAccessibilityObject;
                }
            }

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    _selectedCellsAccessibilityObject ??= new DataGridViewSelectedCellsAccessibleObject(_ownerDataGridView);

                    return _selectedCellsAccessibilityObject;
                }
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (index < 0)
                {
                    return null;
                }

                if (_ownerDataGridView.Columns.Count == 0)
                {
                    Diagnostics.Debug.Assert(GetChildCount() == 0);
                    return null;
                }

                if (index < 1 && _ownerDataGridView.ColumnHeadersVisible)
                {
                    return TopRowAccessibilityObject;
                }

                if (_ownerDataGridView.ColumnHeadersVisible)
                {
                    index--;
                }

                if (index < _ownerDataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible))
                {
                    int actualRowIndex = _ownerDataGridView.Rows.DisplayIndexToRowIndex(index);
                    return _ownerDataGridView.Rows[actualRowIndex].AccessibilityObject;
                }

                index -= _ownerDataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible);

                if (_ownerDataGridView._horizScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return _ownerDataGridView._horizScrollBar.AccessibilityObject;
                    }
                    else
                    {
                        index--;
                    }
                }

                if (_ownerDataGridView._vertScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return _ownerDataGridView._vertScrollBar.AccessibilityObject;
                    }
                }

                return null;
            }

            public override int GetChildCount()
            {
                if (_ownerDataGridView.Columns.Count == 0)
                {
                    return 0;
                }

                int childCount = _ownerDataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible);

                // the column header collection Accessible Object
                if (_ownerDataGridView.ColumnHeadersVisible)
                {
                    childCount++;
                }

                if (_ownerDataGridView._horizScrollBar.Visible)
                {
                    childCount++;
                }

                if (_ownerDataGridView._vertScrollBar.Visible)
                {
                    childCount++;
                }

                return childCount;
            }

            public override AccessibleObject? GetFocused()
            {
                if (_ownerDataGridView.Focused && _ownerDataGridView.CurrentCell is not null)
                {
                    return _ownerDataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject GetSelected()
            {
                return SelectedCellsAccessibilityObject;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_ownerDataGridView.IsHandleCreated)
                {
                    return null;
                }

                Point pt = _ownerDataGridView.PointToClient(new Point(x, y));
                HitTestInfo hti = _ownerDataGridView.HitTest(pt.X, pt.Y);

                switch (hti.Type)
                {
                    case DataGridViewHitTestType.Cell:
                        return _ownerDataGridView.Rows[hti.RowIndex].Cells[hti.ColumnIndex].AccessibilityObject;
                    case DataGridViewHitTestType.ColumnHeader:
                        // map the column index to the actual display index
                        int actualDisplayIndex = _ownerDataGridView.Columns.ColumnIndexToActualDisplayIndex(hti.ColumnIndex, DataGridViewElementStates.Visible);
                        if (_ownerDataGridView.RowHeadersVisible)
                        {
                            // increment the childIndex because the first child in the TopRowAccessibleObject is the TopLeftHeaderCellAccObj
                            return TopRowAccessibilityObject.GetChild(actualDisplayIndex + 1);
                        }

                        return TopRowAccessibilityObject.GetChild(actualDisplayIndex);

                    case DataGridViewHitTestType.RowHeader:
                        return _ownerDataGridView.Rows[hti.RowIndex].HeaderCell.AccessibilityObject;
                    case DataGridViewHitTestType.TopLeftHeader:
                        return _ownerDataGridView.TopLeftHeaderCell.AccessibilityObject;
                    case DataGridViewHitTestType.VerticalScrollBar:
                        return _ownerDataGridView.VerticalScrollBar.AccessibilityObject;
                    case DataGridViewHitTestType.HorizontalScrollBar:
                        return _ownerDataGridView.HorizontalScrollBar.AccessibilityObject;
                    default:
                        return null;
                }
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                switch (navigationDirection)
                {
                    case AccessibleNavigation.FirstChild:
                        return GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return GetChild(GetChildCount() - 1);
                    default:
                        return null;
                }
            }

            /* Microsoft: why is this method defined and not used?
            // this method is called when the accessible object needs to be reset
            // Example: when the user changes the display index on a column or when the user modifies the column collection
            internal void Reset()
            {
                this.NotifyClients(AccessibleEvents.Reorder);
            }
            */

            internal override int[] RuntimeId
                => runtimeId ??= new int[]
                {
                    RuntimeIDFirstItem, // first item is static - 0x2a
                    GetHashCode()
                };

            internal override bool IsIAccessibleExSupported() => true;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return _ownerDataGridView.AccessibleRole == AccessibleRole.Default
                            ? UiaCore.UIA.DataGridControlTypeId
                            : base.GetPropertyValue(propertyID);
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        // If no inner cell entire DGV should be announced as focused by Narrator.
                        // Else only inner cell should be announced as focused by Narrator but not entire DGV.
                        return (IsModal || RowCount == 0) && _ownerDataGridView.Focused;
                    case UiaCore.UIA.IsControlElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return _ownerDataGridView.CanFocus;
                    case UiaCore.UIA.ItemStatusPropertyId:
                        var canSort = false;
                        for (int i = 0; i < ColumnCount; i++)
                        {
                            int columnIndex = _ownerDataGridView.Columns.ActualDisplayIndexToColumnIndex(i, DataGridViewElementStates.Visible);
                            if (_ownerDataGridView.IsSortable(_ownerDataGridView.Columns[columnIndex]))
                            {
                                canSort = true;
                                break;
                            }
                        }

                        if (canSort)
                        {
                            switch (_ownerDataGridView.SortOrder)
                            {
                                case SortOrder.None:
                                    return SR.NotSortedAccessibleStatus;
                                case SortOrder.Ascending:
                                    return string.Format(SR.DataGridViewSortedAscendingAccessibleStatusFormat, _ownerDataGridView.SortedColumn?.HeaderText);
                                case SortOrder.Descending:
                                    return string.Format(SR.DataGridViewSortedDescendingAccessibleStatusFormat, _ownerDataGridView.SortedColumn?.HeaderText);
                            }
                        }

                        if (ColumnCount > 0 && RowCount > 0)
                        {
                            return SR.NotSortedAccessibleStatus;
                        }

                        return base.GetPropertyValue(propertyID);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return (patternId == UiaCore.UIA.TablePatternId && RowCount > 0) ||
                    patternId == UiaCore.UIA.GridPatternId ||
                    base.IsPatternSupported(patternId);
            }

            internal override UiaCore.IRawElementProviderSimple[]? GetRowHeaders()
            {
                if (!_ownerDataGridView.RowHeadersVisible)
                {
                    return null;
                }

                UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[RowCount];
                for (int i = 0; i < RowCount; i++)
                {
                    int rowIndex = _ownerDataGridView.Rows.DisplayIndexToRowIndex(i);
                    result[i] = _ownerDataGridView.Rows[rowIndex].HeaderCell.AccessibilityObject;
                }

                return result;
            }

            internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaders()
            {
                if (!_ownerDataGridView.ColumnHeadersVisible)
                {
                    return null;
                }

                UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[ColumnCount];
                for (int i = 0; i < ColumnCount; i++)
                {
                    int columnIndex = _ownerDataGridView.Columns.ActualDisplayIndexToColumnIndex(i, DataGridViewElementStates.Visible);
                    result[i] = _ownerDataGridView.Columns[columnIndex].HeaderCell.AccessibilityObject;
                }

                return result;
            }

            internal override UiaCore.RowOrColumnMajor RowOrColumnMajor
            {
                get
                {
                    return UiaCore.RowOrColumnMajor.RowMajor;
                }
            }

            internal override UiaCore.IRawElementProviderSimple? GetItem(int row, int column)
            {
                if (row >= 0 && row < RowCount &&
                    column >= 0 && column < ColumnCount)
                {
                    row = _ownerDataGridView.Rows.DisplayIndexToRowIndex(row);
                    column = _ownerDataGridView.Columns.ActualDisplayIndexToColumnIndex(column, DataGridViewElementStates.Visible);
                    return _ownerDataGridView.Rows[row].Cells[column].AccessibilityObject;
                }

                return null;
            }

            internal override int RowCount
            {
                get
                {
                    return _ownerDataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible);
                }
            }

            internal override int ColumnCount
            {
                get
                {
                    return _ownerDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        int childCount = GetChildCount();
                        if (childCount > 0)
                        {
                            return GetChild(0);
                        }

                        break;
                    case UiaCore.NavigateDirection.LastChild:
                        childCount = GetChildCount();
                        if (childCount > 0)
                        {
                            int lastChildIndex = childCount - 1;
                            return GetChild(lastChildIndex);
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override void SetFocus()
            {
                if (_ownerDataGridView.IsHandleCreated && _ownerDataGridView.CanFocus)
                {
                    _ownerDataGridView.Focus();
                }
            }

            #endregion

            #region IRawElementProviderFragmentRoot Implementation

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => _ownerDataGridView.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            internal override UiaCore.IRawElementProviderFragment? GetFocus() => GetFocused();

            #endregion
        }
    }
}
