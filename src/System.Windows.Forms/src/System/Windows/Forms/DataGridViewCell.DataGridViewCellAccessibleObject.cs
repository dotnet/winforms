// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class DataGridViewCell
    {
        protected class DataGridViewCellAccessibleObject : AccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation
            private AccessibleObject _child = null;

            DataGridViewCell owner;

            public DataGridViewCellAccessibleObject()
            {
            }

            public DataGridViewCellAccessibleObject(DataGridViewCell owner)
            {
                this.owner = owner;
            }

            public override Rectangle Bounds
            {
                get
                {
                    return GetAccessibleObjectBounds(GetAccessibleObjectParent());
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }
                    if (!Owner.ReadOnly)
                    {
                        return SR.DataGridView_AccCellDefaultAction;
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
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }
                    if (owner.OwningColumn != null)
                    {
                        string name = string.Format(SR.DataGridView_AccDataGridViewCellName, owner.OwningColumn.HeaderText, owner.OwningRow.Index);

                        if (owner.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                        {
                            DataGridViewCell dataGridViewCell = Owner;
                            DataGridView dataGridView = dataGridViewCell.DataGridView;

                            if (dataGridViewCell.OwningColumn != null &&
                                dataGridViewCell.OwningColumn == dataGridView.SortedColumn)
                            {
                                name += ", " + (dataGridView.SortOrder == SortOrder.Ascending
                                    ? SR.SortedAscendingAccessibleStatus
                                    : SR.SortedDescendingAccessibleStatus);
                            }
                            else
                            {
                                name += ", " + SR.NotSortedAccessibleStatus;
                            }
                        }

                        return name;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public DataGridViewCell Owner
            {
                get
                {
                    return owner;
                }
                set
                {
                    if (owner != null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerAlreadySet);
                    }
                    owner = value;
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
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    return owner.OwningRow?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (owner.DataGridView != null && owner == owner.DataGridView.CurrentCell)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    if (owner.Selected)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    if (owner.ReadOnly)
                    {
                        state |= AccessibleStates.ReadOnly;
                    }

                    if (Owner.DataGridView != null)
                    {
                        Rectangle cellBounds;
                        if (owner.OwningColumn != null && owner.OwningRow != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(owner.OwningColumn.Index, owner.OwningRow.Index, false /*cutOverflow*/);
                        }
                        else if (owner.OwningRow != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(-1, owner.OwningRow.Index, false /*cutOverflow*/);
                        }
                        else if (owner.OwningColumn != null)
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(owner.OwningColumn.Index, -1, false /*cutOverflow*/);
                        }
                        else
                        {
                            cellBounds = owner.DataGridView.GetCellDisplayRectangle(-1, -1, false /*cutOverflow*/);
                        }

                        if (!cellBounds.IntersectsWith(owner.DataGridView.ClientRectangle))
                        {
                            state |= AccessibleStates.Offscreen;
                        }
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    if (owner == null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    object formattedValue = owner.FormattedValue;
                    string formattedValueAsString = formattedValue as string;
                    if (formattedValue == null || (formattedValueAsString != null && string.IsNullOrEmpty(formattedValueAsString)))
                    {
                        return SR.DataGridView_AccNullValue;
                    }
                    else if (formattedValueAsString != null)
                    {
                        return formattedValueAsString;
                    }
                    else if (owner.OwningColumn != null)
                    {
                        TypeConverter converter = owner.FormattedValueTypeConverter;
                        if (converter != null && converter.CanConvertTo(typeof(string)))
                        {
                            return converter.ConvertToString(formattedValue);
                        }
                        else
                        {
                            return formattedValue.ToString();
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                set
                {
                    if (owner is DataGridViewHeaderCell)
                    {
                        return;
                    }

                    if (owner.ReadOnly)
                    {
                        return;
                    }

                    if (owner.OwningRow == null)
                    {
                        return;
                    }

                    if (owner.DataGridView.IsCurrentCellInEditMode)
                    {
                        // EndEdit before setting the accessible object value.
                        // This way the value being edited is validated.
                        owner.DataGridView.EndEdit();
                    }

                    DataGridViewCellStyle dataGridViewCellStyle = owner.InheritedStyle;

                    // Format string "True" to boolean True.
                    object formattedValue = owner.GetFormattedValue(value,
                                                                         owner.OwningRow.Index,
                                                                         ref dataGridViewCellStyle,
                                                                         null /*formattedValueTypeConverter*/ ,
                                                                         null /*valueTypeConverter*/,
                                                                         DataGridViewDataErrorContexts.Formatting);
                    // Parse the formatted value and push it into the back end.
                    owner.Value = owner.ParseFormattedValue(formattedValue,
                                                                 dataGridViewCellStyle,
                                                                 null /*formattedValueTypeConverter*/,
                                                                 null /*valueTypeConverter*/);
                }
            }

            public override void DoDefaultAction()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                DataGridViewCell dataGridViewCell = (DataGridViewCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridViewCell is DataGridViewHeaderCell)
                {
                    return;
                }

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                Select(AccessibleSelection.TakeFocus | AccessibleSelection.TakeSelection);
                Debug.Assert(dataGridView.CurrentCell == dataGridViewCell, "the result of selecting the cell should have made this cell the current cell");

                if (dataGridViewCell.ReadOnly)
                {
                    // don't edit if the cell is read only
                    return;
                }

                if (dataGridViewCell.EditType != null)
                {
                    if (dataGridView.InBeginEdit || dataGridView.InEndEdit)
                    {
                        // don't enter or exit editing mode if the control
                        // is in the middle of doing that already.
                        return;
                    }
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        // stop editing
                        dataGridView.EndEdit();
                    }
                    else if (dataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
                    {
                        // start editing
                        dataGridView.BeginEdit(true /*selectAll*/);
                    }
                }
            }

            internal Rectangle GetAccessibleObjectBounds(AccessibleObject parentAccObject)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null)
                {
                    return Rectangle.Empty;
                }

                Rectangle rowRect = parentAccObject.Bounds;
                Rectangle cellRect = rowRect;
                Rectangle columnRect = owner.DataGridView.RectangleToScreen(owner.DataGridView.GetColumnDisplayRectangle(owner.ColumnIndex, false /*cutOverflow*/));

                var cellRight = columnRect.Left + columnRect.Width;
                var cellLeft = columnRect.Left;

                int rightToLeftRowHeadersWidth = 0;
                int leftToRightRowHeadersWidth = 0;
                if (owner.DataGridView.RowHeadersVisible)
                {
                    if (owner.DataGridView.RightToLeft == RightToLeft.Yes)
                    {
                        rightToLeftRowHeadersWidth = owner.DataGridView.RowHeadersWidth;
                    }
                    else
                    {
                        leftToRightRowHeadersWidth = owner.DataGridView.RowHeadersWidth;
                    }
                }

                if (cellLeft < rowRect.Left + leftToRightRowHeadersWidth)
                {
                    cellLeft = rowRect.Left + leftToRightRowHeadersWidth;
                }
                cellRect.X = cellLeft;

                if (cellRight > rowRect.Right - rightToLeftRowHeadersWidth)
                {
                    cellRight = rowRect.Right - rightToLeftRowHeadersWidth;
                }

                if ((cellRight - cellLeft) >= 0)
                {
                    cellRect.Width = cellRight - cellLeft;
                }
                else
                {
                    cellRect.Width = 0;
                }

                return cellRect;
            }

            private AccessibleObject GetAccessibleObjectParent()
            {
                // If this is one of our types, use the shortcut provided by ParentPrivate property.
                // Otherwise, use the Parent property.
                if (owner is DataGridViewButtonCell ||
                    owner is DataGridViewCheckBoxCell ||
                    owner is DataGridViewComboBoxCell ||
                    owner is DataGridViewImageCell ||
                    owner is DataGridViewLinkCell ||
                    owner is DataGridViewTextBoxCell)
                {
                    return ParentPrivate;
                }
                else
                {
                    return Parent;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView != null &&
                    owner.DataGridView.EditingControl != null &&
                    owner.DataGridView.IsCurrentCellInEditMode &&
                    owner.DataGridView.CurrentCell == owner &&
                    index == 0)
                {
                    return owner.DataGridView.EditingControl.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.DataGridView != null &&
                    owner.DataGridView.EditingControl != null &&
                    owner.DataGridView.IsCurrentCellInEditMode &&
                    owner.DataGridView.CurrentCell == owner)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            public override AccessibleObject GetFocused()
            {
                return null;
            }

            public override AccessibleObject GetSelected()
            {
                return null;
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null || owner.OwningRow == null)
                {
                    return null;
                }

                switch (navigationDirection)
                {
                    case AccessibleNavigation.Right:
                        if (owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateForward(true /*wrapAround*/);
                        }
                        else
                        {
                            return NavigateBackward(true /*wrapAround*/);
                        }
                    case AccessibleNavigation.Next:
                        return NavigateForward(false /*wrapAround*/);
                    case AccessibleNavigation.Left:
                        if (owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateBackward(true /*wrapAround*/);
                        }
                        else
                        {
                            return NavigateForward(true /*wrapAround*/);
                        }
                    case AccessibleNavigation.Previous:
                        return NavigateBackward(false /*wrapAround*/);
                    case AccessibleNavigation.Up:
                        if (owner.OwningRow.Index == owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                        {
                            if (owner.DataGridView.ColumnHeadersVisible)
                            {
                                // Return the column header accessible object.
                                return owner.OwningColumn.HeaderCell.AccessibilityObject;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            int previousVisibleRow = owner.DataGridView.Rows.GetPreviousRow(owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return owner.DataGridView.Rows[previousVisibleRow].Cells[owner.OwningColumn.Index].AccessibilityObject;
                        }
                    case AccessibleNavigation.Down:
                        if (owner.OwningRow.Index == owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                        {
                            return null;
                        }
                        else
                        {
                            int nextVisibleRow = owner.DataGridView.Rows.GetNextRow(owner.OwningRow.Index, DataGridViewElementStates.Visible);
                            return owner.DataGridView.Rows[nextVisibleRow].Cells[owner.OwningColumn.Index].AccessibilityObject;
                        }
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateBackward(bool wrapAround)
            {
                if (owner.OwningColumn == owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
                {
                    if (wrapAround)
                    {
                        // Return the last accessible object in the previous row
                        AccessibleObject previousRow = Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Previous);
                        if (previousRow != null && previousRow.GetChildCount() > 0)
                        {
                            return previousRow.GetChild(previousRow.GetChildCount() - 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // return the row header cell if the row headers are visible.
                        if (owner.DataGridView.RowHeadersVisible)
                        {
                            return owner.OwningRow.AccessibilityObject.GetChild(0);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    int previousVisibleColumnIndex = owner.DataGridView.Columns.GetPreviousColumn(owner.OwningColumn,
                                                                                                       DataGridViewElementStates.Visible,
                                                                                                       DataGridViewElementStates.None).Index;
                    return owner.OwningRow.Cells[previousVisibleColumnIndex].AccessibilityObject;
                }
            }

            private AccessibleObject NavigateForward(bool wrapAround)
            {
                if (owner.OwningColumn == owner.DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible,
                                                                                             DataGridViewElementStates.None))
                {
                    if (wrapAround)
                    {
                        // Return the first cell in the next visible row.
                        //
                        AccessibleObject nextRow = Owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Next);
                        if (nextRow != null && nextRow.GetChildCount() > 0)
                        {
                            if (Owner.DataGridView.RowHeadersVisible)
                            {
                                return nextRow.GetChild(1);
                            }
                            else
                            {
                                return nextRow.GetChild(0);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    int nextVisibleColumnIndex = owner.DataGridView.Columns.GetNextColumn(owner.OwningColumn,
                                                                                               DataGridViewElementStates.Visible,
                                                                                               DataGridViewElementStates.None).Index;
                    return owner.OwningRow.Cells[nextVisibleColumnIndex].AccessibilityObject;
                }
            }

            public override void Select(AccessibleSelection flags)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    owner.DataGridView?.Focus();
                }
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    owner.Selected = true;
                    if (owner.DataGridView != null)
                    {
                        owner.DataGridView.CurrentCell = owner; // Do not change old selection
                    }
                }
                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
                {
                    // it seems that in any circumstances a cell can become selected
                    owner.Selected = true;
                }
                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
                {
                    owner.Selected = false;
                }
            }

            /// <summary>
            ///  Sets the detachable child accessible object which may be added or removed to/from hierachy nodes.
            /// </summary>
            /// <param name="child">The child accessible object.</param>
            internal override void SetDetachableChild(AccessibleObject child)
            {
                _child = child;
            }

            internal override void SetFocus()
            {
                base.SetFocus();

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId == null)
                    {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            private string AutomationId
            {
                get
                {
                    string automationId = string.Empty;
                    foreach (int runtimeIdPart in RuntimeId)
                    {
                        automationId += runtimeIdPart.ToString();
                    }

                    return automationId;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

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
                    return owner.DataGridView.AccessibilityObject;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (owner == null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (owner.OwningColumn == null || owner.OwningRow == null)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return owner.OwningRow.AccessibilityObject;
                    case UiaCore.NavigateDirection.NextSibling:
                        return NavigateForward(false);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return NavigateBackward(false);
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        if (owner.DataGridView.CurrentCell == owner &&
                            owner.DataGridView.IsCurrentCellInEditMode &&
                            owner.DataGridView.EditingControl != null)
                        {
                            return _child;
                        }
                        break;
                    default:
                        return null;
                }

                return null;
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return (State & AccessibleStates.Focused) == AccessibleStates.Focused; // Announce the cell when focusing.
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return owner.DataGridView.Enabled;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return AutomationId;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.GridItemContainingGridPropertyId:
                        return Owner.DataGridView.AccessibilityObject;
                    case UiaCore.UIA.IsTableItemPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.TableItemPatternId);
                    case UiaCore.UIA.IsGridItemPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.GridItemPatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                    patternId == UiaCore.UIA.InvokePatternId ||
                    patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                if ((patternId == UiaCore.UIA.TableItemPatternId ||
                    patternId == UiaCore.UIA.GridItemPatternId) &&
                    // We don't want to implement patterns for header cells
                    owner.ColumnIndex != -1 && owner.RowIndex != -1)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            #endregion

            internal override UiaCore.IRawElementProviderSimple[] GetRowHeaderItems()
            {
                if (owner.DataGridView.RowHeadersVisible && owner.OwningRow.HasHeaderCell)
                {
                    return new UiaCore.IRawElementProviderSimple[1] { owner.OwningRow.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            internal override UiaCore.IRawElementProviderSimple[] GetColumnHeaderItems()
            {
                if (owner.DataGridView.ColumnHeadersVisible && owner.OwningColumn.HasHeaderCell)
                {
                    return new UiaCore.IRawElementProviderSimple[1] { owner.OwningColumn.HeaderCell.AccessibilityObject };
                }

                return null;
            }

            internal override int Row
            {
                get
                {
                    return owner.OwningRow != null ? owner.OwningRow.Index : -1;
                }
            }

            internal override int Column
            {
                get
                {
                    return owner.OwningColumn != null ? owner.OwningColumn.Index : -1;
                }
            }

            internal override UiaCore.IRawElementProviderSimple ContainingGrid
            {
                get
                {
                    return owner.DataGridView.AccessibilityObject;
                }
            }

            internal override bool IsReadOnly => owner.ReadOnly;
        }
    }
}
