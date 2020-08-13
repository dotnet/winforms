// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewColumnHeaderCell
    {
        protected class DataGridViewColumnHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewColumnHeaderCellAccessibleObject(DataGridViewColumnHeaderCell owner) : base(owner)
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    return GetAccessibleObjectBounds(ParentPrivate);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner.OwningColumn != null)
                    {
                        if (Owner.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                        {
                            return SR.DataGridView_AccColumnHeaderCellDefaultAction;
                        }
                        else if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                                 Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                        {
                            return SR.DataGridView_AccColumnHeaderCellSelectDefaultAction;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override string Name
            {
                get
                {
                    if (Owner.OwningColumn != null)
                    {
                        return Owner.OwningColumn.HeaderText;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return ParentPrivate;
                }
            }

            private AccessibleObject ParentPrivate
            {
                get
                {
                    // return the top header row accessible object
                    return Owner.DataGridView.AccessibilityObject.GetChild(0);
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ColumnHeader;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates resultState = AccessibleStates.Selectable;

                    // get the Offscreen state from the base method.
                    AccessibleStates state = base.State;
                    if ((state & AccessibleStates.Offscreen) == AccessibleStates.Offscreen)
                    {
                        resultState |= AccessibleStates.Offscreen;
                    }

                    if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        if (Owner.OwningColumn != null && Owner.OwningColumn.Selected)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            public override string Value
            {
                get
                {
                    return Name;
                }
            }

            public override void DoDefaultAction()
            {
                DataGridViewColumnHeaderCell dataGridViewCell = (DataGridViewColumnHeaderCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridViewCell.OwningColumn != null)
                {
                    if (dataGridViewCell.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        ListSortDirection listSortDirection = ListSortDirection.Ascending;
                        if (dataGridView.SortedColumn == dataGridViewCell.OwningColumn && dataGridView.SortOrder == SortOrder.Ascending)
                        {
                            listSortDirection = ListSortDirection.Descending;
                        }
                        dataGridView.Sort(dataGridViewCell.OwningColumn, listSortDirection);
                    }
                    else if (dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect ||
                             dataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                    {
                        dataGridViewCell.OwningColumn.Selected = true;
                    }
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (Owner.OwningColumn is null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateForward();
                        }
                        else
                        {
                            return NavigateBackward();
                        }
                    case AccessibleNavigation.Next:
                        return NavigateForward();
                    case AccessibleNavigation.Left:
                        if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateBackward();
                        }
                        else
                        {
                            return NavigateForward();
                        }
                    case AccessibleNavigation.Previous:
                        return NavigateBackward();
                    case AccessibleNavigation.FirstChild:
                        // return the top left header cell accessible object
                        return Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0);
                    case AccessibleNavigation.LastChild:
                        // return the last column header cell in the top row header accessible object
                        AccessibleObject topRowHeaderAccessibleObject = Owner.DataGridView.AccessibilityObject.GetChild(0);
                        return topRowHeaderAccessibleObject.GetChild(topRowHeaderAccessibleObject.GetChildCount() - 1);
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateBackward()
            {
                if (Owner.OwningColumn == Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    if (Owner.DataGridView.RowHeadersVisible)
                    {
                        // return the row header cell accessible object for the current row
                        return Parent.GetChild(0);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    int previousVisibleColumnIndex = Owner.DataGridView.Columns.GetPreviousColumn(Owner.OwningColumn,
                                                                                                                DataGridViewElementStates.Visible,
                                                                                                                DataGridViewElementStates.None).Index;
                    int actualDisplayIndex = Owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(previousVisibleColumnIndex,
                                                                                                             DataGridViewElementStates.Visible);
                    if (Owner.DataGridView.RowHeadersVisible)
                    {
                        return Parent.GetChild(actualDisplayIndex + 1);
                    }
                    else
                    {
                        return Parent.GetChild(actualDisplayIndex);
                    }
                }
            }

            private AccessibleObject NavigateForward()
            {
                if (Owner.OwningColumn == Owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                                      DataGridViewElementStates.None))
                {
                    return null;
                }
                else
                {
                    int nextVisibleColumnIndex = Owner.DataGridView.Columns.GetNextColumn(Owner.OwningColumn,
                                                                                                        DataGridViewElementStates.Visible,
                                                                                                        DataGridViewElementStates.None).Index;
                    int actualDisplayIndex = Owner.DataGridView.Columns.ColumnIndexToActualDisplayIndex(nextVisibleColumnIndex,
                                                                                                             DataGridViewElementStates.Visible);

                    if (Owner.DataGridView.RowHeadersVisible)
                    {
                        // + 1 because the top header row accessible object has the top left header cell accessible object at the beginning
                        return Parent.GetChild(actualDisplayIndex + 1);
                    }
                    else
                    {
                        return Parent.GetChild(actualDisplayIndex);
                    }
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                DataGridViewColumnHeaderCell dataGridViewCell = (DataGridViewColumnHeaderCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView is null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }
                if (dataGridViewCell.OwningColumn != null &&
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

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Owner.OwningColumn is null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                        return NavigateForward();
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return NavigateBackward();
                    default:
                        return null;
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId) ||
                    patternId.Equals(UiaCore.UIA.InvokePatternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.HeaderControlTypeId;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return Owner.DataGridView.Enabled;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
