// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewColumnHeaderCell
    {
        protected class DataGridViewColumnHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewColumnHeaderCellAccessibleObject(DataGridViewColumnHeaderCell? owner) : base(owner)
            {
            }

            /// <summary>
            ///  Returns the index of a column, taking into account DisplayIndex properties
            ///  and the invisibility of other columns
            /// </summary>
            private int VisibleIndex
                => Owner?.DataGridView is not null && Owner.OwningColumn is not null
                    ? Owner.DataGridView.RowHeadersVisible
                        ? Owner.DataGridView.Columns.GetVisibleIndex(Owner.OwningColumn) + 1
                        : Owner.DataGridView.Columns.GetVisibleIndex(Owner.OwningColumn)
                    : -1;

            public override Rectangle Bounds => Owner is null
                ? throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet)
                : (Owner.DataGridView?.IsHandleCreated == true) ? GetAccessibleObjectBounds(Parent) : Rectangle.Empty;

            public override string DefaultAction
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    if (Owner.OwningColumn is not null)
                    {
                        if (Owner.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                        {
                            return SR.DataGridView_AccColumnHeaderCellDefaultAction;
                        }
                        else if (Owner.DataGridView is not null && (
                                Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                                Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                        {
                            return SR.DataGridView_AccColumnHeaderCellSelectDefaultAction;
                        }
                    }

                    return string.Empty;
                }
            }

            public override string Name => Owner is null
                ? throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet)
                : Owner.OwningColumn?.HeaderText ?? string.Empty;

            // return the top header row accessible object
            public override AccessibleObject? Parent => Owner is null
                ? throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet)
                : Owner.DataGridView?.AccessibilityObject?.GetChild(0);

            public override AccessibleRole Role => AccessibleRole.ColumnHeader;

            public override AccessibleStates State
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates resultState = AccessibleStates.Selectable;

                    // get the Offscreen state from the base method.
                    AccessibleStates state = base.State;
                    if ((state & AccessibleStates.Offscreen) == AccessibleStates.Offscreen)
                    {
                        resultState |= AccessibleStates.Offscreen;
                    }

                    if (Owner.DataGridView is not null && Owner.OwningColumn is not null && Owner.OwningColumn.Selected)
                    {
                        if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            public override string Value => Name;

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewColumnHeaderCell dataGridViewCell))
                {
                    return;
                }

                DataGridView? dataGridView = dataGridViewCell.DataGridView;
                if (dataGridView?.IsHandleCreated != true || dataGridViewCell.OwningColumn is null)
                {
                    return;
                }

                if (dataGridViewCell.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                {
                    ListSortDirection listSortDirection = dataGridView.SortedColumn == dataGridViewCell.OwningColumn && dataGridView.SortOrder == SortOrder.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;

                    dataGridView.Sort(dataGridViewCell.OwningColumn, listSortDirection);
                }
                else if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                         dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                {
                    dataGridViewCell.OwningColumn.Selected = true;
                }
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.OwningColumn is null || Owner.DataGridView is null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        return Owner.DataGridView.RightToLeft == RightToLeft.No ? NavigateForward() : NavigateBackward();
                    case AccessibleNavigation.Next:
                        return NavigateForward();
                    case AccessibleNavigation.Left:
                        return Owner.DataGridView.RightToLeft == RightToLeft.No ? NavigateBackward() : NavigateForward();
                    case AccessibleNavigation.Previous:
                        return NavigateBackward();
                    default:
                        return null;
                }
            }

            private AccessibleObject? NavigateBackward()
            {
                Debug.Assert(Owner is not null);

                // This method is called after _owner and its properties are validated
                if (Owner.OwningColumn is null || Owner.DataGridView is null)
                {
                    return null;
                }

                if (Owner.OwningColumn == Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    // return the row header cell accessible object for the current row if Owner.DataGridView.RowHeadersVisible == false
                    return Owner.DataGridView.RowHeadersVisible ? Parent?.GetChild(0) : null;
                }
                else
                {
                    // Subtract 1 because the top header row accessible object has the top left header cell accessible object at the beginning
                    return Parent?.GetChild(VisibleIndex - 1);
                }
            }

            private AccessibleObject? NavigateForward()
            {
                Debug.Assert(Owner is not null);

                // This method is called after _owner and its properties are validated
                if (Owner.OwningColumn is null ||
                    Owner.DataGridView is null ||
                    Owner.OwningColumn == Owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                   DataGridViewElementStates.None))
                {
                    return null;
                }

                // Add 1 because the top header row accessible object has the top left header cell accessible object at the beginning
                int visibleIndex = VisibleIndex;
                return visibleIndex < 0 ? null : Parent?.GetChild(visibleIndex + 1);
            }

            public override void Select(AccessibleSelection flags)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewColumnHeaderCell dataGridViewCell))
                {
                    return;
                }

                DataGridView? dataGridView = dataGridViewCell.DataGridView;
                if (dataGridView?.IsHandleCreated != true)
                {
                    return;
                }

                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }

                if (dataGridViewCell.OwningColumn is not null &&
                    (dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                     dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
                {
                    if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.AddSelection)) != 0)
                    {
                        dataGridViewCell.OwningColumn.Selected = true;
                    }
                    else if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection)
                    {
                        dataGridViewCell.OwningColumn.Selected = false;
                    }
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => Parent,
                    UiaCore.NavigateDirection.NextSibling => NavigateForward(),
                    UiaCore.NavigateDirection.PreviousSibling => NavigateBackward(),
                    _ => null
                };
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId) ||
                    patternId.Equals(UiaCore.UIA.InvokePatternId);

            internal override object? GetPropertyValue(UiaCore.UIA propertyId)
                => propertyId switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.HeaderControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsEnabledPropertyId => Owner?.DataGridView?.Enabled ?? false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.IsPasswordPropertyId => false,
                    _ => base.GetPropertyValue(propertyId)
                };

            #endregion
        }
    }
}
