// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewRow
    {
        protected class DataGridViewRowAccessibleObject : AccessibleObject
        {
            private int[]? _runtimeId;
            private DataGridViewRow? _owningDataGridViewRow;
            private DataGridViewSelectedRowCellsAccessibleObject? _selectedCellsAccessibilityObject;

            public DataGridViewRowAccessibleObject()
            {
            }

            public DataGridViewRowAccessibleObject(DataGridViewRow owner)
            {
                _owningDataGridViewRow = owner;
            }

            /// <summary>
            ///  Returns the index of the row, taking into account the invisibility of other rows.
            /// </summary>
            private int VisibleIndex
                => _owningDataGridViewRow?.DataGridView is DataGridView dataGridView
                    ? dataGridView.ColumnHeadersVisible
                        ? dataGridView.Rows.GetVisibleIndex(_owningDataGridViewRow) + 1
                        : dataGridView.Rows.GetVisibleIndex(_owningDataGridViewRow)
                    : -1;

            public override Rectangle Bounds
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (_owningDataGridViewRow.DataGridView is null || !_owningDataGridViewRow.DataGridView.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle rowRect = _owningDataGridViewRow.DataGridView.RectangleToScreen(_owningDataGridViewRow.DataGridView.GetRowDisplayRectangle(_owningDataGridViewRow.Index, false /*cutOverflow*/));

                    int horizontalScrollBarHeight = 0;
                    if (_owningDataGridViewRow.DataGridView.HorizontalScrollBarVisible)
                    {
                        horizontalScrollBarHeight = _owningDataGridViewRow.DataGridView.HorizontalScrollBarHeight;
                    }

                    Rectangle dataGridViewRect = ParentPrivate!.Bounds;

                    int columnHeadersHeight = 0;
                    if (_owningDataGridViewRow.DataGridView.ColumnHeadersVisible)
                    {
                        columnHeadersHeight = _owningDataGridViewRow.DataGridView.ColumnHeadersHeight;
                    }

                    int rowRectBottom = rowRect.Bottom;
                    if ((dataGridViewRect.Bottom - horizontalScrollBarHeight) < rowRectBottom)
                    {
                        rowRectBottom = dataGridViewRect.Bottom - _owningDataGridViewRow.DataGridView.BorderWidth - horizontalScrollBarHeight;
                    }

                    if ((dataGridViewRect.Top + columnHeadersHeight) > rowRect.Top)
                    {
                        rowRect.Height = 0;
                    }
                    else
                    {
                        rowRect.Height = rowRectBottom - rowRect.Top;
                    }

                    return rowRect;
                }
            }

            public override string Name
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    int index = _owningDataGridViewRow is { Visible: true, DataGridView: { } }
                            ? _owningDataGridViewRow.DataGridView.Rows.GetVisibleIndex(_owningDataGridViewRow)
                            : -1;

                    return string.Format(SR.DataGridView_AccRowName, index.ToString(CultureInfo.CurrentCulture));
                }
            }

            public DataGridViewRow? Owner
            {
                get => _owningDataGridViewRow;
                set
                {
                    if (_owningDataGridViewRow is not null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerAlreadySet);
                    }

                    _owningDataGridViewRow = value;
                }
            }

            public override AccessibleObject? Parent => ParentPrivate;

            private AccessibleObject? ParentPrivate
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    return _owningDataGridViewRow.DataGridView?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role => AccessibleRole.Row;

            internal override int[] RuntimeId
                => _runtimeId ??= new int[]
                {
                    RuntimeIDFirstItem, // first item is static - 0x2a
                    Parent?.GetHashCode() ?? 0,
                    GetHashCode()
                };

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (_selectedCellsAccessibilityObject is null)
                    {
                        _selectedCellsAccessibilityObject = new DataGridViewSelectedRowCellsAccessibleObject(_owningDataGridViewRow);
                    }

                    return _selectedCellsAccessibilityObject;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates accState = AccessibleStates.Selectable;

                    bool allCellsAreSelected = true;
                    if (_owningDataGridViewRow.Selected)
                    {
                        allCellsAreSelected = true;
                    }
                    else
                    {
                        for (int i = 0; i < _owningDataGridViewRow.Cells.Count; i++)
                        {
                            if (!_owningDataGridViewRow.Cells[i].Selected)
                            {
                                allCellsAreSelected = false;
                                break;
                            }
                        }
                    }

                    if (allCellsAreSelected)
                    {
                        accState |= AccessibleStates.Selected;
                    }

                    if (_owningDataGridViewRow.DataGridView is not null && _owningDataGridViewRow.DataGridView.IsHandleCreated)
                    {
                        Rectangle rowBounds = _owningDataGridViewRow.DataGridView.GetRowDisplayRectangle(_owningDataGridViewRow.Index, true /*cutOverflow*/);
                        if (!rowBounds.IntersectsWith(_owningDataGridViewRow.DataGridView.ClientRectangle))
                        {
                            accState |= AccessibleStates.Offscreen;
                        }
                    }

                    return accState;
                }
            }

            public override string Value
            {
                get
                {
                    if (_owningDataGridViewRow is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (_owningDataGridViewRow.DataGridView is not null && _owningDataGridViewRow.DataGridView.AllowUserToAddRows && _owningDataGridViewRow.Index == _owningDataGridViewRow.DataGridView.NewRowIndex)
                    {
                        return SR.DataGridView_AccRowCreateNew;
                    }

                    StringBuilder sb = new StringBuilder(1024);

                    int childCount = GetChildCount();

                    // filter out the row header acc object even when DataGridView::RowHeadersVisible is turned on
                    int startIndex = _owningDataGridViewRow.DataGridView is not null && _owningDataGridViewRow.DataGridView.RowHeadersVisible ? 1 : 0;

                    for (int i = startIndex; i < childCount; i++)
                    {
                        AccessibleObject? cellAccObj = GetChild(i);
                        if (cellAccObj is not null)
                        {
                            sb.Append(cellAccObj.Value);
                        }

                        if (i != childCount - 1)
                        {
                            sb.Append(';');
                        }
                    }

                    return sb.ToString();
                }
            }

            public override AccessibleObject? GetChild(int index)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (_owningDataGridViewRow is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (_owningDataGridViewRow.DataGridView is null || index > GetChildCount() - 1)
                {
                    return null;
                }

                if (index == 0 && _owningDataGridViewRow.DataGridView.RowHeadersVisible)
                {
                    return _owningDataGridViewRow.HeaderCell.AccessibilityObject;
                }
                else
                {
                    // decrement the index because the first child is the RowHeaderCell AccessibilityObject
                    if (_owningDataGridViewRow.DataGridView.RowHeadersVisible)
                    {
                        index--;
                    }

                    Debug.Assert(index >= 0);
                    int columnIndex = _owningDataGridViewRow.DataGridView.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return _owningDataGridViewRow.Cells[columnIndex].AccessibilityObject;
                }
            }

            public override int GetChildCount()
            {
                if (_owningDataGridViewRow is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (_owningDataGridViewRow.DataGridView is null)
                {
                    return 0;
                }

                int result = _owningDataGridViewRow.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (_owningDataGridViewRow.DataGridView.RowHeadersVisible)
                {
                    // + 1 comes from the row header cell accessibility object
                    result++;
                }

                return result;
            }

            public override AccessibleObject GetSelected() => SelectedCellsAccessibilityObject;

            public override AccessibleObject? GetFocused()
            {
                if (_owningDataGridViewRow is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (_owningDataGridViewRow.DataGridView is DataGridView dataGridView &&
                    dataGridView.Focused &&
                    dataGridView.CurrentCell is not null &&
                    dataGridView.CurrentCell.RowIndex == _owningDataGridViewRow.Index)
                {
                    return _owningDataGridViewRow.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                if (_owningDataGridViewRow is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (_owningDataGridViewRow.DataGridView is null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (_owningDataGridViewRow.Index != _owningDataGridViewRow.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            int visibleIndex = VisibleIndex;
                            return visibleIndex < 0
                                ? null
                                : _owningDataGridViewRow.DataGridView.AccessibilityObject.GetChild(visibleIndex + 1);
                        }
                        else
                        {
                            return null;
                        }

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Previous:
                        if (_owningDataGridViewRow.Index != _owningDataGridViewRow.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            return _owningDataGridViewRow.DataGridView.AccessibilityObject.GetChild(VisibleIndex - 1);
                        }
                        else if (_owningDataGridViewRow.DataGridView.ColumnHeadersVisible)
                        {
                            // return the top row header acc obj
                            return ParentPrivate?.GetChild(0);
                        }
                        else
                        {
                            // if this is the first row and the DataGridView RowHeaders are not visible return null;
                            return null;
                        }

                    case AccessibleNavigation.FirstChild:
                        if (GetChildCount() == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return GetChild(0);
                        }

                    case AccessibleNavigation.LastChild:
                        int childCount = GetChildCount();
                        if (childCount == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return GetChild(childCount - 1);
                        }

                    default:
                        return null;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (_owningDataGridViewRow is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                DataGridView? dataGridView = _owningDataGridViewRow.DataGridView;

                if (dataGridView is null || !dataGridView.IsHandleCreated)
                {
                    return;
                }

                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }

                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    if (_owningDataGridViewRow.Cells.Count > 0)
                    {
                        if (dataGridView.CurrentCell is not null && dataGridView.CurrentCell.OwningColumn is not null)
                        {
                            dataGridView.CurrentCell = _owningDataGridViewRow.Cells[dataGridView.CurrentCell.OwningColumn.Index]; // Do not change old selection
                        }
                        else
                        {
                            int firstVisibleCell = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index;
                            if (firstVisibleCell > -1)
                            {
                                dataGridView.CurrentCell = _owningDataGridViewRow.Cells[firstVisibleCell]; // Do not change old selection
                            }
                        }
                    }
                }

                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection && (flags & AccessibleSelection.TakeSelection) == 0)
                {
                    if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        _owningDataGridViewRow.Selected = true;
                    }
                }

                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    _owningDataGridViewRow.Selected = false;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.Parent:
                            return Parent;
                        case UiaCore.NavigateDirection.NextSibling:
                            return Navigate(AccessibleNavigation.Next);
                        case UiaCore.NavigateDirection.PreviousSibling:
                            return Navigate(AccessibleNavigation.Previous);
                        case UiaCore.NavigateDirection.FirstChild:
                            return Navigate(AccessibleNavigation.FirstChild);
                        case UiaCore.NavigateDirection.LastChild:
                            return Navigate(AccessibleNavigation.LastChild);
                        default:
                            return null;
                    }
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
            {
                get
                {
                    return ParentPrivate;
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId);
            }

            internal override bool IsReadOnly => _owningDataGridViewRow?.ReadOnly ?? false;

            internal override object? GetPropertyValue(UiaCore.UIA propertyId) =>
                propertyId switch
                {
                    UiaCore.UIA.HasKeyboardFocusPropertyId => string.Empty,
                    UiaCore.UIA.IsEnabledPropertyId => Owner?.DataGridView?.Enabled ?? false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => string.Empty,
                    _ => base.GetPropertyValue(propertyId)
                };
        }
    }
}
