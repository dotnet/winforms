// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewRowHeaderCell
    {
        protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell? owner) : base(owner)
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    if (Owner.DataGridView is null || Owner.OwningRow is null || ParentPrivate is null)
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
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    if (Owner.DataGridView != null &&
                        (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                         Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
                    {
                        return SR.DataGridView_RowHeaderCellAccDefaultAction;
                    }

                    return string.Empty;
                }
            }

            public override string? Name => ParentPrivate?.Name ?? string.Empty;

            public override AccessibleObject? Parent => ParentPrivate;

            private AccessibleObject? ParentPrivate
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    return Owner.OwningRow?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role => AccessibleRole.RowHeader;

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

                    if (Owner.DataGridView != null &&
                        (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                         Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
                    {
                        if (Owner.OwningRow != null && Owner.OwningRow.Selected)
                        {
                            resultState |= AccessibleStates.Selected;
                        }
                    }

                    return resultState;
                }
            }

            public override string Value => string.Empty;

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.DataGridView?.IsHandleCreated == true &&
                    Owner.OwningRow != null &&
                    (Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
                     Owner.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect))
                {
                    Owner.OwningRow.Selected = true;
                }
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.OwningRow is null || Owner.DataGridView is null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Next:
                        return (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                            ? ParentPrivate?.GetChild(1) // go to the next sibling
                            : null;

                    case AccessibleNavigation.Down:
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                            {
                                return null;
                            }

                            int nextVisibleRow = Owner.DataGridView.Rows.GetNextRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, nextVisibleRow);

                            if (Owner.DataGridView.ColumnHeadersVisible)
                            {
                                // Add 1 because the first child in the data grid view acc obj is the top row header acc obj
                                actualDisplayIndex++;
                            }

                            return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex)?.GetChild(0);
                        }

                    case AccessibleNavigation.Up:
                        {
                            if (Owner.OwningRow.Index == Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                            {
                                if (!Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    return null;
                                }

                                // Return the top left header cell accessible object.
                                Debug.Assert(Owner.DataGridView.TopLeftHeaderCell.AccessibilityObject == Owner.DataGridView.AccessibilityObject.GetChild(0)!.GetChild(0));
                                return Owner.DataGridView.AccessibilityObject.GetChild(0)?.GetChild(0);
                            }
                            else
                            {
                                int previousVisibleRow = Owner.DataGridView.Rows.GetPreviousRow(Owner.OwningRow.Index, DataGridViewElementStates.Visible);
                                int actualDisplayIndex = Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible, 0, previousVisibleRow);
                                if (Owner.DataGridView.ColumnHeadersVisible)
                                {
                                    // Add 1 because the first child in the data grid view acc obj is the top row header acc obj
                                    actualDisplayIndex++;
                                }

                                return Owner.DataGridView.AccessibilityObject.GetChild(actualDisplayIndex)?.GetChild(0);
                            }
                        }

                    case AccessibleNavigation.Previous:
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

                if (!(Owner is DataGridViewRowHeaderCell dataGridViewCell))
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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.OwningRow is null)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => Owner.OwningRow.AccessibilityObject,
                    UiaCore.NavigateDirection.NextSibling =>
                            (Owner.DataGridView != null && Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                                ? Owner.OwningRow.AccessibilityObject.GetChild(1) // go to the next sibling
                                : null,
                    _ => null,
                };
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object? GetPropertyValue(UiaCore.UIA propertyId)
                => propertyId switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.HeaderControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId => Owner?.DataGridView?.Enabled ?? false,
                    UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsPasswordPropertyId => false,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                    _ => base.GetPropertyValue(propertyId),
                };

            #endregion
        }
    }
}
