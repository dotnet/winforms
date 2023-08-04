﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

public partial class DataGridView
{
    protected class DataGridViewAccessibleObject : ControlAccessibleObject
    {
        private int[]? runtimeId; // Used by UIAutomation
        private bool? _isModal;

        private DataGridViewTopRowAccessibleObject? _topRowAccessibilityObject;
        private DataGridViewSelectedCellsAccessibleObject? _selectedCellsAccessibilityObject;

        public DataGridViewAccessibleObject(DataGridView owner)
            : base(owner)
        {
        }

        internal override bool IsReadOnly => this.TryGetOwnerAs(out DataGridView? owner) ? owner.ReadOnly : base.IsReadOnly;

        private bool IsModal
        {
            get
            {
                _isModal ??= this.TryGetOwnerAs(out DataGridView? owner) && owner.TopMostParent is Form { Modal: true };
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
            => this.GetOwnerAccessibleRole(AccessibleRole.Table);

        private AccessibleObject? TopRowAccessibilityObject
        {
            get
            {
                _topRowAccessibilityObject ??= this.TryGetOwnerAs(out DataGridView? owner) ? new DataGridViewTopRowAccessibleObject(owner) : null;

                return _topRowAccessibilityObject;
            }
        }

        private AccessibleObject SelectedCellsAccessibilityObject
        {
            get
            {
                _selectedCellsAccessibilityObject ??= new DataGridViewSelectedCellsAccessibleObject(this);

                return _selectedCellsAccessibilityObject;
            }
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (index < 0 || !this.TryGetOwnerAs(out DataGridView? owner))
            {
                return null;
            }

            if (owner.Columns.Count == 0)
            {
                Diagnostics.Debug.Assert(GetChildCount() == 0);
                return null;
            }

            if (index < 1 && owner.ColumnHeadersVisible)
            {
                return TopRowAccessibilityObject;
            }

            if (owner.ColumnHeadersVisible)
            {
                index--;
            }

            if (index < owner.Rows.GetRowCount(DataGridViewElementStates.Visible))
            {
                int actualRowIndex = owner.Rows.DisplayIndexToRowIndex(index);
                return owner.Rows[actualRowIndex].AccessibilityObject;
            }

            index -= owner.Rows.GetRowCount(DataGridViewElementStates.Visible);

            if (owner._horizScrollBar.Visible)
            {
                if (index == 0)
                {
                    return owner._horizScrollBar.AccessibilityObject;
                }
                else
                {
                    index--;
                }
            }

            if (owner._vertScrollBar.Visible)
            {
                if (index == 0)
                {
                    return owner._vertScrollBar.AccessibilityObject;
                }
            }

            return null;
        }

        public override int GetChildCount()
        {
            if (!this.TryGetOwnerAs(out DataGridView? owner) || owner.Columns.Count == 0)
            {
                return 0;
            }

            int childCount = owner.Rows.GetRowCount(DataGridViewElementStates.Visible);

            // the column header collection Accessible Object
            if (owner.ColumnHeadersVisible)
            {
                childCount++;
            }

            if (owner._horizScrollBar.Visible)
            {
                childCount++;
            }

            if (owner._vertScrollBar.Visible)
            {
                childCount++;
            }

            return childCount;
        }

        public override AccessibleObject? GetFocused()
        {
            if (this.TryGetOwnerAs(out DataGridView? owner) && owner.Focused && owner.CurrentCell is not null)
            {
                return owner.CurrentCell.AccessibilityObject;
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
            if (!this.IsOwnerHandleCreated(out DataGridView? owner))
            {
                return null;
            }

            Point pt = owner.PointToClient(new Point(x, y));
            HitTestInfo hti = owner.HitTest(pt.X, pt.Y);

            switch (hti.Type)
            {
                case DataGridViewHitTestType.Cell:
                    return owner.Rows[hti.RowIndex].Cells[hti.ColumnIndex].AccessibilityObject;
                case DataGridViewHitTestType.ColumnHeader:
                    // map the column index to the actual display index
                    int actualDisplayIndex = owner.Columns.ColumnIndexToActualDisplayIndex(hti.ColumnIndex, DataGridViewElementStates.Visible);
                    if (owner.RowHeadersVisible)
                    {
                        // increment the childIndex because the first child in the TopRowAccessibleObject is the TopLeftHeaderCellAccObj
                        return TopRowAccessibilityObject?.GetChild(actualDisplayIndex + 1);
                    }

                    return TopRowAccessibilityObject?.GetChild(actualDisplayIndex);

                case DataGridViewHitTestType.RowHeader:
                    return owner.Rows[hti.RowIndex].HeaderCell.AccessibilityObject;
                case DataGridViewHitTestType.TopLeftHeader:
                    return owner.TopLeftHeaderCell.AccessibilityObject;
                case DataGridViewHitTestType.VerticalScrollBar:
                    return owner.VerticalScrollBar.AccessibilityObject;
                case DataGridViewHitTestType.HorizontalScrollBar:
                    return owner.HorizontalScrollBar.AccessibilityObject;
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
            DataGridView? owner;

            switch (propertyID)
            {
                case UiaCore.UIA.ControlTypePropertyId:
                    return (this.TryGetOwnerAs(out owner) && owner.AccessibleRole == AccessibleRole.Default)
                        ? UiaCore.UIA.DataGridControlTypeId
                        : base.GetPropertyValue(propertyID);
                case UiaCore.UIA.HasKeyboardFocusPropertyId:
                    // If no inner cell entire DGV should be announced as focused by Narrator.
                    // Else only inner cell should be announced as focused by Narrator but not entire DGV.
                    return this.TryGetOwnerAs(out owner) && (IsModal || RowCount == 0) && owner.Focused;
                case UiaCore.UIA.IsControlElementPropertyId:
                    return true;
                case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                    return this.TryGetOwnerAs(out owner) && owner.CanFocus;
                case UiaCore.UIA.ItemStatusPropertyId:
                    var canSort = false;
                    if (!this.TryGetOwnerAs(out owner))
                    {
                        return base.GetPropertyValue(propertyID);
                    }

                    for (int i = 0; i < ColumnCount; i++)
                    {
                        int columnIndex = owner.Columns.ActualDisplayIndexToColumnIndex(i, DataGridViewElementStates.Visible);
                        if (owner.IsSortable(owner.Columns[columnIndex]))
                        {
                            canSort = true;
                            break;
                        }
                    }

                    if (canSort)
                    {
                        switch (owner.SortOrder)
                        {
                            case SortOrder.None:
                                return SR.NotSortedAccessibleStatus;
                            case SortOrder.Ascending:
                                return string.Format(SR.DataGridViewSortedAscendingAccessibleStatusFormat, owner.SortedColumn?.HeaderText);
                            case SortOrder.Descending:
                                return string.Format(SR.DataGridViewSortedDescendingAccessibleStatusFormat, owner.SortedColumn?.HeaderText);
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
            if (!this.TryGetOwnerAs(out DataGridView? owner) || !owner.RowHeadersVisible)
            {
                return null;
            }

            UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[RowCount];
            for (int i = 0; i < RowCount; i++)
            {
                int rowIndex = owner.Rows.DisplayIndexToRowIndex(i);
                result[i] = owner.Rows[rowIndex].HeaderCell.AccessibilityObject;
            }

            return result;
        }

        internal override UiaCore.IRawElementProviderSimple[]? GetColumnHeaders()
        {
            if (!this.TryGetOwnerAs(out DataGridView? owner) || !owner.ColumnHeadersVisible)
            {
                return null;
            }

            UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[ColumnCount];
            for (int i = 0; i < ColumnCount; i++)
            {
                int columnIndex = owner.Columns.ActualDisplayIndexToColumnIndex(i, DataGridViewElementStates.Visible);
                result[i] = owner.Columns[columnIndex].HeaderCell.AccessibilityObject;
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
            if (!this.TryGetOwnerAs(out DataGridView? owner))
            {
                return null;
            }

            if (row >= 0 && row < RowCount &&
                column >= 0 && column < ColumnCount)
            {
                row = owner.Rows.DisplayIndexToRowIndex(row);
                column = owner.Columns.ActualDisplayIndexToColumnIndex(column, DataGridViewElementStates.Visible);
                return owner.Rows[row].Cells[column].AccessibilityObject;
            }

            return null;
        }

        internal override int RowCount
        {
            get
            {
                return this.TryGetOwnerAs(out DataGridView? owner) ? owner.Rows.GetRowCount(DataGridViewElementStates.Visible) : base.RowCount;
            }
        }

        internal override int ColumnCount
        {
            get
            {
                return this.TryGetOwnerAs(out DataGridView? owner) ? owner.Columns.GetColumnCount(DataGridViewElementStates.Visible) : base.ColumnCount;
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
            if (this.IsOwnerHandleCreated(out DataGridView? owner) && owner.CanFocus)
            {
                owner.Focus();
            }
        }

        #endregion

        #region IRawElementProviderFragmentRoot Implementation

        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            => this.IsOwnerHandleCreated(out DataGridView? _) ? HitTest((int)x, (int)y) : null;

        internal override UiaCore.IRawElementProviderFragment? GetFocus() => GetFocused();

        #endregion
    }
}
