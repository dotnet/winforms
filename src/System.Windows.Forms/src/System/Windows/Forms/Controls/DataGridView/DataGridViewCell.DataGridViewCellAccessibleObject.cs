// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Primitives;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public abstract partial class DataGridViewCell
{
    protected class DataGridViewCellAccessibleObject : AccessibleObject
    {
        private int[] _runtimeId = null!; // Used by UIAutomation
        private AccessibleObject? _child;
        private DataGridViewCell? _owner;

        public DataGridViewCellAccessibleObject()
        {
        }

        public DataGridViewCellAccessibleObject(DataGridViewCell? owner)
        {
            _owner = owner;
        }

        public override Rectangle Bounds => GetAccessibleObjectBounds(GetAccessibleObjectParent());

        public override string DefaultAction
        {
            get
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                return !Owner.ReadOnly ? SR.DataGridView_AccCellDefaultAction : string.Empty;
            }
        }

        internal override bool CanGetDefaultActionInternal => false;

        public override string? Name
        {
            get
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (_owner.OwningColumn is null || _owner.OwningRow is null)
                {
                    return string.Empty;
                }

                int rowIndex = _owner.DataGridView is null
                    ? -1
                    : _owner.DataGridView.Rows.GetVisibleIndex(_owner.OwningRow) + RowStartIndex;

                string name = string.Format(SR.DataGridView_AccDataGridViewCellName, _owner.OwningColumn.HeaderText, rowIndex).Trim();

                if (_owner.OwningColumn.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    DataGridViewCell dataGridViewCell = _owner;
                    DataGridView? dataGridView = dataGridViewCell.DataGridView;

                    if (dataGridView is not null &&
                        dataGridViewCell.OwningColumn is not null &&
                        dataGridViewCell.OwningColumn == dataGridView.SortedColumn)
                    {
                        name += ", " + (dataGridView.SortOrder == SortOrder.Ascending
                            ? SR.SortedAscendingAccessibleStatus
                            : SR.SortedDescendingAccessibleStatus);
                    }
                    else
                    {
                        name += $", {SR.NotSortedAccessibleStatus}";
                    }
                }

                return name;
            }
        }

        internal override bool CanGetNameInternal => false;

        public DataGridViewCell? Owner
        {
            get => _owner;
            set
            {
                if (_owner is not null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerAlreadySet);
                }

                _owner = value;
            }
        }

        public override AccessibleObject? Parent => ParentPrivate;

        private AccessibleObject? ParentPrivate
        {
            get
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                return _owner.OwningRow?.AccessibilityObject;
            }
        }

        public override AccessibleRole Role => AccessibleRole.Cell;

        private static int RowStartIndex => LocalAppContextSwitches.DataGridViewUIAStartRowCountAtZero ? 0 : 1;

        public override AccessibleStates State
        {
            get
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                if (_owner.DataGridView is not null && _owner == _owner.DataGridView.CurrentCell)
                {
                    state |= AccessibleStates.Focused;
                }

                if (_owner.Selected)
                {
                    state |= AccessibleStates.Selected;
                }

                if (_owner.ReadOnly)
                {
                    state |= AccessibleStates.ReadOnly;
                }

                if (_owner.DataGridView?.IsHandleCreated != true)
                {
                    return state;
                }

                Rectangle cellBounds;
                if (_owner.OwningColumn is not null && _owner.OwningRow is not null)
                {
                    cellBounds = _owner.DataGridView.GetCellDisplayRectangle(_owner.OwningColumn.Index, _owner.OwningRow.Index, cutOverflow: false);
                }
                else if (_owner.OwningRow is not null)
                {
                    cellBounds = _owner.DataGridView.GetCellDisplayRectangle(-1, _owner.OwningRow.Index, cutOverflow: false);
                }
                else if (_owner.OwningColumn is not null)
                {
                    cellBounds = _owner.DataGridView.GetCellDisplayRectangle(_owner.OwningColumn.Index, -1, cutOverflow: false);
                }
                else
                {
                    cellBounds = _owner.DataGridView.GetCellDisplayRectangle(-1, -1, cutOverflow: false);
                }

                if (!cellBounds.IntersectsWith(_owner.DataGridView.ClientRectangle))
                {
                    state |= AccessibleStates.Offscreen;
                }

                return state;
            }
        }

        public override string? Value
        {
            get
            {
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                object? formattedValue = _owner.FormattedValue;
                string? formattedValueAsString = formattedValue as string;
                if (formattedValue is null || (formattedValueAsString is not null && string.IsNullOrEmpty(formattedValueAsString)))
                {
                    return SR.DataGridView_AccNullValue;
                }
                else if (formattedValueAsString is not null)
                {
                    return formattedValueAsString;
                }
                else if (_owner.OwningColumn is not null)
                {
                    TypeConverter? converter = _owner.FormattedValueTypeConverter;
                    if (converter is not null && converter.CanConvertTo(typeof(string)))
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
                if (_owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (_owner is DataGridViewHeaderCell || _owner.ReadOnly || _owner.DataGridView is null || _owner.OwningRow is null)
                {
                    return;
                }

                if (_owner.DataGridView.IsCurrentCellInEditMode)
                {
                    // EndEdit before setting the accessible object value.
                    // This way the value being edited is validated.
                    _owner.DataGridView.EndEdit();
                }

                DataGridViewCellStyle dataGridViewCellStyle = _owner.InheritedStyle;

                // Format string "True" to boolean True.
                object? formattedValue = _owner.GetFormattedValue(
                    value,
                    _owner.OwningRow.Index,
                    ref dataGridViewCellStyle,
                    valueTypeConverter: null,
                    formattedValueTypeConverter: null,
                    DataGridViewDataErrorContexts.Formatting);

                // Parse the formatted value and push it into the back end.
                _owner.Value = _owner.ParseFormattedValue(
                    formattedValue,
                    dataGridViewCellStyle,
                    formattedValueTypeConverter: null,
                    valueTypeConverter: null);
            }
        }

        internal override bool CanGetValueInternal => false;

        internal override bool CanSetValueInternal => false;

        public override void DoDefaultAction()
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            DataGridViewCell dataGridViewCell = _owner;
            DataGridView? dataGridView = dataGridViewCell.DataGridView;
            if (dataGridViewCell is DataGridViewHeaderCell || dataGridView?.IsHandleCreated != true)
            {
                return;
            }

            if (dataGridViewCell.RowIndex == -1)
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

            if (dataGridViewCell.EditType is not null)
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
                    dataGridView.BeginEdit(selectAll: true);
                }
            }
        }

        internal Rectangle GetAccessibleObjectBounds(AccessibleObject? parentAccObject)
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (parentAccObject is null
                || _owner.DataGridView is null
                || !_owner.DataGridView.IsHandleCreated
                || _owner.OwningColumn is null)
            {
                return Rectangle.Empty;
            }

            Rectangle rowRect = parentAccObject.Bounds;
            Rectangle cellRect = rowRect;
            Rectangle columnRect = _owner.DataGridView.RectangleToScreen(
                _owner.DataGridView.GetColumnDisplayRectangle(_owner.ColumnIndex, cutOverflow: false));

            int cellRight = columnRect.Left + columnRect.Width;
            int cellLeft = columnRect.Left;

            int rightToLeftRowHeadersWidth = 0;
            int leftToRightRowHeadersWidth = 0;
            if (_owner.DataGridView.RowHeadersVisible)
            {
                if (_owner.DataGridView.RightToLeft == RightToLeft.Yes)
                {
                    rightToLeftRowHeadersWidth = _owner.DataGridView.RowHeadersWidth;
                }
                else
                {
                    leftToRightRowHeadersWidth = _owner.DataGridView.RowHeadersWidth;
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

        private AccessibleObject? GetAccessibleObjectParent()
        {
            // If this is one of our types, use the shortcut provided by ParentPrivate property.
            // Otherwise, use the Parent property.
            if (_owner is DataGridViewButtonCell
                or DataGridViewCheckBoxCell
                or DataGridViewComboBoxCell
                or DataGridViewImageCell
                or DataGridViewLinkCell
                or DataGridViewTextBoxCell)
            {
                return ParentPrivate;
            }
            else
            {
                return Parent;
            }
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (_owner.DataGridView is not null &&
                _owner.DataGridView.EditingControl is not null &&
                _owner.DataGridView.IsCurrentCellInEditMode &&
                _owner.DataGridView.CurrentCell == _owner &&
                index == 0)
            {
                return _owner.DataGridView.EditingControl.AccessibilityObject;
            }
            else
            {
                return null;
            }
        }

        public override int GetChildCount()
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (_owner.DataGridView is not null &&
                _owner.DataGridView.EditingControl is not null &&
                _owner.DataGridView.IsCurrentCellInEditMode &&
                _owner.DataGridView.CurrentCell == _owner)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override AccessibleObject? GetFocused() => null;

        public override AccessibleObject? GetSelected() => null;

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (_owner.DataGridView?.IsHandleCreated != true || _owner.OwningColumn is null || _owner.OwningRow is null)
            {
                return null;
            }

            switch (navigationDirection)
            {
                case AccessibleNavigation.Right:
                    return _owner.DataGridView.RightToLeft == RightToLeft.No
                        ? NavigateForward(wrapAround: true)
                        : NavigateBackward(wrapAround: true);

                case AccessibleNavigation.Next:
                    return NavigateForward(wrapAround: false);

                case AccessibleNavigation.Left:
                    return _owner.DataGridView.RightToLeft == RightToLeft.No
                        ? NavigateBackward(wrapAround: true)
                        : NavigateForward(wrapAround: true);

                case AccessibleNavigation.Previous:
                    return NavigateBackward(wrapAround: false);

                case AccessibleNavigation.Up:
                    if (_owner.OwningRow.Index == _owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible))
                    {
                        return _owner.DataGridView.ColumnHeadersVisible
                            ? _owner.OwningColumn.HeaderCell.AccessibilityObject // Return the column header accessible object
                            : null;
                    }
                    else
                    {
                        int previousVisibleRow = _owner.DataGridView.Rows.GetPreviousRow(_owner.OwningRow.Index, DataGridViewElementStates.Visible);
                        return _owner.DataGridView.Rows[previousVisibleRow].Cells[_owner.OwningColumn.Index].AccessibilityObject;
                    }

                case AccessibleNavigation.Down:
                    if (_owner.OwningRow.Index == _owner.DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible))
                    {
                        return null;
                    }
                    else
                    {
                        int nextVisibleRow = _owner.DataGridView.Rows.GetNextRow(_owner.OwningRow.Index, DataGridViewElementStates.Visible);
                        return _owner.DataGridView.Rows[nextVisibleRow].Cells[_owner.OwningColumn.Index].AccessibilityObject;
                    }

                default:
                    return null;
            }
        }

        private AccessibleObject? NavigateBackward(bool wrapAround)
        {
            Debug.Assert(_owner is not null);
            Debug.Assert(_owner.DataGridView is not null);
            Debug.Assert(_owner.OwningColumn is not null);
            Debug.Assert(_owner.OwningRow is not null);

            if (_owner.OwningColumn == _owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible))
            {
                if (wrapAround)
                {
                    // Return the last accessible object in the previous row
                    AccessibleObject? previousRow = _owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Previous);
                    if (previousRow is null)
                    {
                        return null;
                    }

                    int childCount = previousRow.GetChildCount();
                    return childCount > 0 ? previousRow.GetChild(childCount - 1) : null;
                }
                else
                {
                    // return the row header cell if the row headers are visible.
                    return _owner.DataGridView.RowHeadersVisible ? _owner.OwningRow.AccessibilityObject.GetChild(0) : null;
                }
            }
            else
            {
                int previousVisibleColumnIndex = _owner.DataGridView.Columns.GetPreviousColumn(
                    _owner.OwningColumn,
                    DataGridViewElementStates.Visible,
                    DataGridViewElementStates.None)!.Index;
                return _owner.OwningRow.Cells[previousVisibleColumnIndex].AccessibilityObject;
            }
        }

        private AccessibleObject? NavigateForward(bool wrapAround)
        {
            Debug.Assert(_owner is not null);
            Debug.Assert(_owner.DataGridView is not null);
            Debug.Assert(_owner.OwningColumn is not null);
            Debug.Assert(_owner.OwningRow is not null);

            if (_owner.OwningColumn == _owner.DataGridView.Columns.GetLastColumn(
                DataGridViewElementStates.Visible,
                DataGridViewElementStates.None))
            {
                if (wrapAround)
                {
                    // Return the first cell in the next visible row.
                    AccessibleObject? nextRow = _owner.OwningRow.AccessibilityObject.Navigate(AccessibleNavigation.Next);
                    if (nextRow is not null && nextRow.GetChildCount() > 0)
                    {
                        return _owner.DataGridView.RowHeadersVisible ? nextRow.GetChild(1) : nextRow.GetChild(0);
                    }
                }

                return null;
            }
            else
            {
                int nextVisibleColumnIndex = _owner.DataGridView.Columns.GetNextColumn(
                    _owner.OwningColumn,
                    DataGridViewElementStates.Visible,
                    DataGridViewElementStates.None)!.Index;
                return _owner.OwningRow.Cells[nextVisibleColumnIndex].AccessibilityObject;
            }
        }

        public override void Select(AccessibleSelection flags)
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (_owner.DataGridView?.IsHandleCreated != true)
            {
                return;
            }

            if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
            {
                _owner.DataGridView.Focus();
            }

            if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
            {
                _owner.Selected = true;
                _owner.DataGridView.CurrentCell = _owner; // Do not change old selection
            }

            if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
            {
                // it seems that in any circumstances a cell can become selected
                _owner.Selected = true;
            }

            if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                (flags & (AccessibleSelection.AddSelection | AccessibleSelection.TakeSelection)) == 0)
            {
                _owner.Selected = false;
            }
        }

        /// <summary>
        ///  Sets the detachable child accessible object which may be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="child">The child accessible object.</param>
        internal override void SetDetachableChild(AccessibleObject? child)
        {
            _child = child;
        }

        internal override void SetFocus()
        {
            if (_owner?.DataGridView?.IsHandleCreated != true)
            {
                return;
            }

            base.SetFocus();

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        internal override int[] RuntimeId => _runtimeId ??=
        [
            RuntimeIDFirstItem,
            GetHashCode()
        ];

        private protected override string AutomationId
        {
            get => string.Concat(RuntimeId);
        }

        internal override bool IsIAccessibleExSupported() => true;

        #region IRawElementProviderFragment Implementation

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => _owner?.DataGridView?.AccessibilityObject;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (_owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (_owner.DataGridView?.IsHandleCreated != true || _owner.OwningColumn is null || _owner.OwningRow is null)
            {
                return null;
            }

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return _owner.OwningRow.AccessibilityObject;

                case NavigateDirection.NavigateDirection_NextSibling:
                    return NavigateForward(wrapAround: false);

                case NavigateDirection.NavigateDirection_PreviousSibling:
                    return NavigateBackward(wrapAround: false);

                case NavigateDirection.NavigateDirection_FirstChild:
                case NavigateDirection.NavigateDirection_LastChild:
                    if (_owner.DataGridView.CurrentCell == _owner &&
                        _owner.DataGridView.IsCurrentCellInEditMode &&
                        _owner.DataGridView.EditingControl is not null)
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

        internal override unsafe VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId,
                UIA_PROPERTY_ID.UIA_GridItemContainingGridPropertyId
                    => (VARIANT)ComHelpers.GetComPointer<IUnknown>(_owner?.DataGridView?.AccessibilityObject),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focused), // Announce the cell when focusing.
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(_owner?.DataGridView?.Enabled ?? false),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                _ => base.GetPropertyValue(propertyID),
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            if (patternId is UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId or
                UIA_PATTERN_ID.UIA_InvokePatternId or
                UIA_PATTERN_ID.UIA_ValuePatternId)
            {
                return true;
            }

            if ((patternId == UIA_PATTERN_ID.UIA_TableItemPatternId || patternId == UIA_PATTERN_ID.UIA_GridItemPatternId)
                // We don't want to implement patterns for header cells
                && _owner?.ColumnIndex != -1 && _owner?.RowIndex != -1)
            {
                return true;
            }

            return base.IsPatternSupported(patternId);
        }

        #endregion

        internal override IRawElementProviderSimple.Interface[]? GetRowHeaderItems()
        {
            if (_owner is { OwningRow.HasHeaderCell: true, DataGridView: { IsHandleCreated: true, RowHeadersVisible: true } })
            {
                return [_owner.OwningRow.HeaderCell.AccessibilityObject];
            }

            return null;
        }

        internal override IRawElementProviderSimple.Interface[]? GetColumnHeaderItems()
        {
            if (_owner is { OwningColumn.HasHeaderCell: true, DataGridView: { IsHandleCreated: true, ColumnHeadersVisible: true } })
            {
                return [_owner.OwningColumn.HeaderCell.AccessibilityObject];
            }

            return null;
        }

        internal override int Row
            => _owner?.OwningRow?.Visible is true && _owner.DataGridView is not null
                ? _owner.DataGridView.Rows.GetVisibleIndex(_owner.OwningRow)
                : -1;

        internal override int Column
            => _owner?.OwningColumn?.Visible is true && _owner.DataGridView is not null
                ? _owner.DataGridView.Columns.GetVisibleIndex(_owner.OwningColumn)
                : -1;

        internal override IRawElementProviderSimple.Interface? ContainingGrid => _owner?.DataGridView?.AccessibilityObject;

        internal override bool IsReadOnly => _owner?.ReadOnly ?? false;
    }
}
