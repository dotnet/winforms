// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
            private int[] runtimeId;
            private DataGridViewRow owner;
            private DataGridViewSelectedRowCellsAccessibleObject selectedCellsAccessibilityObject;

            public DataGridViewRowAccessibleObject()
            {
            }

            public DataGridViewRowAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Returns the index of the row, taking into account the invisibility of other rows.
            /// </summary>
            private int VisibleIndex
                => owner?.DataGridView is not null
                    ? owner.DataGridView.ColumnHeadersVisible
                        ? owner.DataGridView.Rows.GetVisibleIndex(owner) + 1
                        : owner.DataGridView.Rows.GetVisibleIndex(owner)
                    : -1;

            public override Rectangle Bounds
            {
                get
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (owner.DataGridView is null || !owner.DataGridView.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle rowRect = owner.DataGridView.RectangleToScreen(owner.DataGridView.GetRowDisplayRectangle(owner.Index, false /*cutOverflow*/));

                    int horizontalScrollBarHeight = 0;
                    if (owner.DataGridView.HorizontalScrollBarVisible)
                    {
                        horizontalScrollBarHeight = owner.DataGridView.HorizontalScrollBarHeight;
                    }

                    Rectangle dataGridViewRect = ParentPrivate.Bounds;

                    int columnHeadersHeight = 0;
                    if (owner.DataGridView.ColumnHeadersVisible)
                    {
                        columnHeadersHeight = owner.DataGridView.ColumnHeadersHeight;
                    }

                    int rowRectBottom = rowRect.Bottom;
                    if ((dataGridViewRect.Bottom - horizontalScrollBarHeight) < rowRectBottom)
                    {
                        rowRectBottom = dataGridViewRect.Bottom - owner.DataGridView.BorderWidth - horizontalScrollBarHeight;
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
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    int index = owner is { Visible: true, DataGridView: { } }
                            ? owner.DataGridView.Rows.GetVisibleIndex(owner)
                            : -1;

                    return string.Format(SR.DataGridView_AccRowName, index.ToString(CultureInfo.CurrentCulture));
                }
            }

            public DataGridViewRow Owner
            {
                get => owner;
                set
                {
                    if (owner is not null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerAlreadySet);
                    }

                    owner = value;
                }
            }

            public override AccessibleObject Parent => ParentPrivate;

            private AccessibleObject ParentPrivate
            {
                get
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    return owner.DataGridView?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role => AccessibleRole.Row;

            internal override int[] RuntimeId
                => runtimeId ??= new int[]
                {
                    RuntimeIDFirstItem, // first item is static - 0x2a
                    Parent.GetHashCode(),
                    GetHashCode()
                };

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (selectedCellsAccessibilityObject is null)
                    {
                        selectedCellsAccessibilityObject = new DataGridViewSelectedRowCellsAccessibleObject(owner);
                    }

                    return selectedCellsAccessibilityObject;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates accState = AccessibleStates.Selectable;

                    bool allCellsAreSelected = true;
                    if (owner.Selected)
                    {
                        allCellsAreSelected = true;
                    }
                    else
                    {
                        for (int i = 0; i < owner.Cells.Count; i++)
                        {
                            if (!owner.Cells[i].Selected)
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

                    if (owner.DataGridView is not null && owner.DataGridView.IsHandleCreated)
                    {
                        Rectangle rowBounds = owner.DataGridView.GetRowDisplayRectangle(owner.Index, true /*cutOverflow*/);
                        if (!rowBounds.IntersectsWith(owner.DataGridView.ClientRectangle))
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
                    if (owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                    }

                    if (owner.DataGridView is not null && owner.DataGridView.AllowUserToAddRows && owner.Index == owner.DataGridView.NewRowIndex)
                    {
                        return SR.DataGridView_AccRowCreateNew;
                    }

                    StringBuilder sb = new StringBuilder(1024);

                    int childCount = GetChildCount();

                    // filter out the row header acc object even when DataGridView::RowHeadersVisible is turned on
                    int startIndex = owner.DataGridView is not null && owner.DataGridView.RowHeadersVisible ? 1 : 0;

                    for (int i = startIndex; i < childCount; i++)
                    {
                        AccessibleObject cellAccObj = GetChild(i);
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

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView is null || index > GetChildCount() - 1)
                {
                    return null;
                }

                if (index == 0 && owner.DataGridView.RowHeadersVisible)
                {
                    return owner.HeaderCell.AccessibilityObject;
                }
                else
                {
                    // decrement the index because the first child is the RowHeaderCell AccessibilityObject
                    if (owner.DataGridView.RowHeadersVisible)
                    {
                        index--;
                    }

                    Debug.Assert(index >= 0);
                    int columnIndex = owner.DataGridView.Columns.ActualDisplayIndexToColumnIndex(index, DataGridViewElementStates.Visible);
                    return owner.Cells[columnIndex].AccessibilityObject;
                }
            }

            public override int GetChildCount()
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView is null)
                {
                    return 0;
                }

                int result = owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible);
                if (owner.DataGridView.RowHeadersVisible)
                {
                    // + 1 comes from the row header cell accessibility object
                    result++;
                }

                return result;
            }

            public override AccessibleObject GetSelected() => SelectedCellsAccessibilityObject;

            public override AccessibleObject GetFocused()
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView is not null &&
                    owner.DataGridView.Focused &&
                    owner.DataGridView.CurrentCell is not null &&
                    owner.DataGridView.CurrentCell.RowIndex == owner.Index)
                {
                    return owner.DataGridView.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView is null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Next:
                        if (owner.Index != owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            int visibleIndex = VisibleIndex;
                            return visibleIndex < 0
                                ? null
                                : owner.DataGridView.AccessibilityObject.GetChild(visibleIndex + 1);
                        }
                        else
                        {
                            return null;
                        }

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Previous:
                        if (owner.Index != owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            return owner.DataGridView.AccessibilityObject.GetChild(VisibleIndex - 1);
                        }
                        else if (owner.DataGridView.ColumnHeadersVisible)
                        {
                            // return the top row header acc obj
                            return ParentPrivate.GetChild(0);
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
                if (owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewRowAccessibleObject_OwnerNotSet);
                }

                DataGridView dataGridView = owner.DataGridView;

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
                    if (owner.Cells.Count > 0)
                    {
                        if (dataGridView.CurrentCell is not null && dataGridView.CurrentCell.OwningColumn is not null)
                        {
                            dataGridView.CurrentCell = owner.Cells[dataGridView.CurrentCell.OwningColumn.Index]; // Do not change old selection
                        }
                        else
                        {
                            int firstVisibleCell = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index;
                            if (firstVisibleCell > -1)
                            {
                                dataGridView.CurrentCell = owner.Cells[firstVisibleCell]; // Do not change old selection
                            }
                        }
                    }
                }

                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection && (flags & AccessibleSelection.TakeSelection) == 0)
                {
                    if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        owner.Selected = true;
                    }
                }

                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    owner.Selected = false;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
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

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
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

            internal override bool IsReadOnly => owner.ReadOnly;

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return Owner?.DataGridView?.Enabled ?? false;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                }

                return base.GetPropertyValue(propertyId);
            }
        }
    }
}
