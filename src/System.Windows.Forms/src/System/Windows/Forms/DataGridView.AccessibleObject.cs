﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        [ComVisible(true)]
        protected class DataGridViewAccessibleObject : ControlAccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation

            readonly DataGridView owner;
            DataGridViewTopRowAccessibleObject topRowAccessibilityObject = null;
            DataGridViewSelectedCellsAccessibleObject selectedCellsAccessibilityObject = null;

            public DataGridViewAccessibleObject(DataGridView owner)
                : base(owner)
            {
                this.owner = owner;
            }

            internal override bool IsReadOnly => owner.ReadOnly;

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (!string.IsNullOrEmpty(name))
                    {
                        return name;
                    }
                    else
                    {
                        // The default name should not be localized.
                        return "DataGridView";
                    }
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = owner.AccessibleRole;
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
                    if (topRowAccessibilityObject == null)
                    {
                        topRowAccessibilityObject = new DataGridViewTopRowAccessibleObject(owner);
                    }

                    return topRowAccessibilityObject;
                }
            }

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (selectedCellsAccessibilityObject == null)
                    {
                        selectedCellsAccessibilityObject = new DataGridViewSelectedCellsAccessibleObject(owner);
                    }

                    return selectedCellsAccessibilityObject;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (owner.Columns.Count == 0)
                {
                    System.Diagnostics.Debug.Assert(GetChildCount() == 0);
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

                if (owner.horizScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return owner.horizScrollBar.AccessibilityObject;
                    }
                    else
                    {
                        index--;
                    }
                }

                if (owner.vertScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return owner.vertScrollBar.AccessibilityObject;
                    }
                }

                return null;
            }

            public override int GetChildCount()
            {
                if (owner.Columns.Count == 0)
                {
                    return 0;
                }

                int childCount = owner.Rows.GetRowCount(DataGridViewElementStates.Visible);

                // the column header collection Accessible Object
                if (owner.ColumnHeadersVisible)
                {
                    childCount++;
                }

                if (owner.horizScrollBar.Visible)
                {
                    childCount++;
                }

                if (owner.vertScrollBar.Visible)
                {
                    childCount++;
                }

                return childCount;
            }

            public override AccessibleObject GetFocused()
            {
                if (owner.Focused && owner.CurrentCell != null)
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

            public override AccessibleObject HitTest(int x, int y)
            {
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
                            return TopRowAccessibilityObject.GetChild(actualDisplayIndex + 1);
                        }
                        else
                        {
                            return TopRowAccessibilityObject.GetChild(actualDisplayIndex);
                        }
                    case DataGridViewHitTestType.RowHeader:
                        return owner.Rows[hti.RowIndex].AccessibilityObject;
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

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
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

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return false; // Only inner cell should be announced as focused by Narrator but not entire DGV.
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return owner.CanFocus;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return owner.Enabled;
                    case UiaCore.UIA.IsControlElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsTablePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.TablePatternId);
                    case UiaCore.UIA.IsGridPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.GridPatternId);
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.TableControlTypeId;
                    case UiaCore.UIA.ItemStatusPropertyId:
                        // Whether the owner DataGridView can be sorted by some column.
                        // If so, provide not-sorted/sorted-by item status.
                        bool canSort = false;
                        for (int i = 0; i < owner.Columns.Count; i++)
                        {
                            if (owner.IsSortable(owner.Columns[i]))
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

                        break;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.TablePatternId ||
                    patternId == UiaCore.UIA.GridPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UiaCore.IRawElementProviderSimple[] GetRowHeaders()
            {
                if (!owner.RowHeadersVisible)
                {
                    return null;
                }

                UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[owner.Rows.Count];
                for (int i = 0; i < owner.Rows.Count; i++)
                {
                    result[i] = owner.Rows[i].HeaderCell.AccessibilityObject;
                }
                return result;
            }

            internal override UiaCore.IRawElementProviderSimple[] GetColumnHeaders()
            {
                if (!owner.ColumnHeadersVisible)
                {
                    return null;
                }

                UiaCore.IRawElementProviderSimple[] result = new UiaCore.IRawElementProviderSimple[owner.Columns.Count];
                for (int i = 0; i < owner.Columns.Count; i++)
                {
                    result[i] = owner.Columns[i].HeaderCell.AccessibilityObject;
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

            internal override UiaCore.IRawElementProviderSimple GetItem(int row, int column)
            {
                if (row >= 0 && row < owner.Rows.Count &&
                    column >= 0 && column < owner.Columns.Count)
                {
                    return owner.Rows[row].Cells[column].AccessibilityObject;
                }

                return null;
            }

            internal override int RowCount
            {
                get
                {
                    return owner.RowCount;
                }
            }

            internal override int ColumnCount
            {
                get
                {
                    return owner.ColumnCount;
                }
            }

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
                    return this;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
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

                return null;
            }

            internal override void SetFocus()
            {
                if (owner.CanFocus)
                {
                    owner.Focus();
                }
            }

            #endregion

            #region IRawElementProviderFragmentRoot Implementation

            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                return HitTest((int)x, (int)y);
            }

            internal override UiaCore.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

            #endregion
        }

        internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
        {
            private readonly DataGridView dataGridView;
            private readonly Panel panel;

            public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
            {
                this.dataGridView = dataGridView;
                this.panel = panel;
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return panel.AccessibilityObject.Bounds;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return dataGridView.AccessibilityObject;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    return panel.AccessibilityObject.RuntimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        DataGridViewCell currentCell = dataGridView.CurrentCell;
                        if (currentCell != null && dataGridView.IsCurrentCellInEditMode)
                        {
                            return currentCell.AccessibilityObject;
                        }
                        break;
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return dataGridView.EditingControlAccessibleObject;
                }

                return null;
            }

            internal override void SetFocus()
            {
                if (panel.CanFocus)
                {
                    panel.Focus();
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return SR.DataGridView_AccEditingPanelAccName;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.PaneControlTypeId;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return true;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return dataGridView.CurrentCell != null;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return dataGridView.Enabled;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.IsControlElementPropertyId:
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return panel.AccessibilityObject.KeyboardShortcut;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.ProviderDescriptionPropertyId:
                        return SR.DataGridViewEditingPanelUiaProviderDescription;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
