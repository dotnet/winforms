// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewRowHeaderCell
    {
        protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell owner) : base(owner)
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (Owner.OwningRow is null)
                    {
                        return Rectangle.Empty;
                    }

                    // use the parent row acc obj bounds
                    Rectangle rowRect = ParentPrivate.Bounds;
                    Rectangle cellRect = rowRect;
                    cellRect.Width = Owner.DataGridView.RowHeadersWidth;
                    if (Owner.DataGridView.RightToLeft == RightToLeft.Yes)
                    {
                        cellRect.X = rowRect.Right - cellRect.Width;
                    }

                    return cellRect;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        return SR.DataGridView_RowHeaderCellAccDefaultAction;
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
                    if (ParentPrivate != null)
                    {
                        return ParentPrivate.Name;
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
                    if (Owner.OwningRow is null)
                    {
                        return null;
                    }
                    else
                    {
                        return Owner.OwningRow.AccessibilityObject;
                    }
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.RowHeader;
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

                    if (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                        Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                    {
                        if (Owner.OwningRow != null && Owner.OwningRow.Selected)
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
                    return string.Empty;
                }
            }

            public override void DoDefaultAction()
            {
                if ((Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                    Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect) &&
                    Owner.OwningRow != null)
                {
                    Owner.OwningRow.Selected = true;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                Debug.Assert(Owner.DataGridView.RowHeadersVisible, "if the rows are not visible how did you get the row headers acc obj?");
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Next:
                        if (Owner.OwningRow != null && Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return ParentPrivate.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.Down:
                        if (Owner.OwningRow is null)
                        {
                            return null;
                        }
                        else
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                            {
                                return null;
                            }
                            else
                            {
                                int nextVisibleRow = Owner.DataGridView.Rows.GetNextRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);

                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return Owner.DataGridView.AccessibilityObject.GetChild(1 + actualDisplayIndex).GetChild(0);
                                }
                                else
                                {
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    case AccessibleNavigation.Previous:
                        return null;
                    case AccessibleNavigation.Up:
                        if (Owner.OwningRow is null)
                        {
                            return null;
                        }
                        else
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                            {
                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // Return the top left header cell accessible object.
                                    Debug.Assert(Owner.DataGridView.TopLeftHeaderCell.AccessibilityObject == Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0));
                                    return Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(0);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                int previousVisibleRow = Owner.DataGridView.Rows.GetPreviousRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // + 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex + 1).GetChild(0);
                                }
                                else
                                {
                                    return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex).GetChild(0);
                                }
                            }
                        }
                    default:
                        return null;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                DataGridViewRowHeaderCell dataGridViewCell = (DataGridViewRowHeaderCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView is null)
                {
                    return;
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    dataGridView.Focus();
                }
                if (dataGridViewCell.OwningRow != null &&
                    (dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                     dataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
                {
                    if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.AddSelection)) != 0)
                    {
                        dataGridViewCell.OwningRow.Selected = true;
                    }
                    else if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection)
                    {
                        dataGridViewCell.OwningRow.Selected = false;
                    }
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Owner.OwningRow is null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Owner.OwningRow.AccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                        {
                            // go to the next sibling
                            return Owner.OwningRow.AccessibilityObject.GetChild(1);
                        }
                        else
                        {
                            return null;
                        }
                    case UiaCore.NavigateDirection.PreviousSibling:
                    default:
                        return null;
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

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
