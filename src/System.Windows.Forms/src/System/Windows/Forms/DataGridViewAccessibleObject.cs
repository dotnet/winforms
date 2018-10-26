// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        [
            System.Runtime.InteropServices.ComVisible(true)
        ]
        /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridView.DataGridViewAccessibleObject"]/*' />
        protected class DataGridViewAccessibleObject : ControlAccessibleObject
        {
            private int[] runtimeId = null; // Used by UIAutomation

            DataGridView owner;
            DataGridViewTopRowAccessibleObject topRowAccessibilityObject = null;
            DataGridViewSelectedCellsAccessibleObject selectedCellsAccessibilityObject = null;

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.DataGridViewAccessibleObject"]/*' />
            public DataGridViewAccessibleObject(DataGridView owner)
                : base(owner)
            {
                this.owner = owner;
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.Name"]/*' />
            public override string Name
            {
                [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")] // Don't localize string "DataGridView".
                get
                {
                    string name = this.Owner.AccessibleName;
                    if (!String.IsNullOrEmpty(name))
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

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.Role"]/*' />
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
                    if (this.topRowAccessibilityObject == null)
                    {
                        this.topRowAccessibilityObject = new DataGridViewTopRowAccessibleObject(this.owner);
                    }

                    return this.topRowAccessibilityObject;
                }
            }

            private AccessibleObject SelectedCellsAccessibilityObject
            {
                get
                {
                    if (this.selectedCellsAccessibilityObject == null)
                    {
                        this.selectedCellsAccessibilityObject = new DataGridViewSelectedCellsAccessibleObject(this.owner);
                    }

                    return this.selectedCellsAccessibilityObject;
                }
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.GetChild"]/*' />
            public override AccessibleObject GetChild(int index)
            {
                if (this.owner.Columns.Count == 0)
                {
                    System.Diagnostics.Debug.Assert(this.GetChildCount() == 0);
                    return null;
                }

                if (index < 1 && this.owner.ColumnHeadersVisible)
                {
                    return this.TopRowAccessibilityObject;
                }

                if (this.owner.ColumnHeadersVisible)
                {
                    index--;
                }

                if (index < this.owner.Rows.GetRowCount(DataGridViewElementStates.Visible))
                {
                    int actualRowIndex = this.owner.Rows.DisplayIndexToRowIndex(index);
                    return this.owner.Rows[actualRowIndex].AccessibilityObject;
                }

                index -= this.owner.Rows.GetRowCount(DataGridViewElementStates.Visible);

                if (this.owner.horizScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return this.owner.horizScrollBar.AccessibilityObject;
                    }
                    else
                    {
                        index--;
                    }
                }

                if (this.owner.vertScrollBar.Visible)
                {
                    if (index == 0)
                    {
                        return this.owner.vertScrollBar.AccessibilityObject;
                    }
                }

                return null;
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.GetChildCount"]/*' />
            public override int GetChildCount()
            {
                if (this.owner.Columns.Count == 0)
                {
                    return 0;
                }

                int childCount = this.owner.Rows.GetRowCount(DataGridViewElementStates.Visible);

                // the column header collection Accessible Object
                if (this.owner.ColumnHeadersVisible)
                {
                    childCount++;
                }

                if (this.owner.horizScrollBar.Visible)
                {
                    childCount++;
                }

                if (this.owner.vertScrollBar.Visible)
                {
                    childCount++;
                }

                return childCount;
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.GetFocused"]/*' />
            public override AccessibleObject GetFocused()
            {
                if (this.owner.Focused && this.owner.CurrentCell != null)
                {
                    return this.owner.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.GetSelected"]/*' />
            public override AccessibleObject GetSelected()
            {
                return this.SelectedCellsAccessibilityObject;
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.HitTest"]/*' />
            public override AccessibleObject HitTest(int x, int y)
            {
                Point pt = this.owner.PointToClient(new Point(x, y));
                HitTestInfo hti = this.owner.HitTest(pt.X, pt.Y);

                switch (hti.Type)
                {
                    case DataGridViewHitTestType.Cell:
                        return this.owner.Rows[hti.RowIndex].Cells[hti.ColumnIndex].AccessibilityObject;
                    case DataGridViewHitTestType.ColumnHeader:
                        // map the column index to the actual display index
                        int actualDisplayIndex = this.owner.Columns.ColumnIndexToActualDisplayIndex(hti.ColumnIndex, DataGridViewElementStates.Visible);
                        if (this.owner.RowHeadersVisible)
                        {
                            // increment the childIndex because the first child in the TopRowAccessibleObject is the TopLeftHeaderCellAccObj
                            return this.TopRowAccessibilityObject.GetChild(actualDisplayIndex + 1);
                        }
                        else
                        {
                            return this.TopRowAccessibilityObject.GetChild(actualDisplayIndex);
                        }
                    case DataGridViewHitTestType.RowHeader:
                        return this.owner.Rows[hti.RowIndex].AccessibilityObject;
                    case DataGridViewHitTestType.TopLeftHeader:
                        return this.owner.TopLeftHeaderCell.AccessibilityObject;
                    case DataGridViewHitTestType.VerticalScrollBar:
                        return this.owner.VerticalScrollBar.AccessibilityObject;
                    case DataGridViewHitTestType.HorizontalScrollBar:
                        return this.owner.HorizontalScrollBar.AccessibilityObject;
                    default:
                        return null;
                }
            }

            /// <include file='doc\DataGridViewAccessibleObject.uex' path='docs/doc[@for="DataGridViewAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                switch (navigationDirection)
                {
                    case AccessibleNavigation.FirstChild:
                        return this.GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return this.GetChild(this.GetChildCount() - 1);
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
                        runtimeId[1] = this.GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (AccessibilityImprovements.Level2)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (AccessibilityImprovements.Level3)
                {
                    switch (propertyID)
                    {
                        case NativeMethods.UIA_NamePropertyId:
                            return this.Name;
                        case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                            return owner.Focused;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return owner.CanFocus;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return owner.Enabled;
                        case NativeMethods.UIA_IsControlElementPropertyId:
                            return true;
                        case NativeMethods.UIA_IsTablePatternAvailablePropertyId:
                        case NativeMethods.UIA_IsGridPatternAvailablePropertyId:
                            return true;
                        case NativeMethods.UIA_ControlTypePropertyId:
                            return NativeMethods.UIA_TableControlTypeId;
                        case NativeMethods.UIA_ItemStatusPropertyId:
                            // Whether the owner DataGridView can be sorted by some column.
                            // If so, provide not-sorted/sorted-by item status.
                            bool canSort = false;
                            for (int i = 0; i < owner.Columns.Count; i++)
                            {
                                if (owner.CanSort(owner.Columns[i]))
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
                }

                if (propertyID == NativeMethods.UIA_IsTablePatternAvailablePropertyId)
                {
                    return IsPatternSupported(NativeMethods.UIA_TablePatternId);
                }
                else if (propertyID == NativeMethods.UIA_IsGridPatternAvailablePropertyId)
                {
                    return IsPatternSupported(NativeMethods.UIA_GridPatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(int patternId)
            {
                if (patternId == NativeMethods.UIA_TablePatternId || 
                    patternId == NativeMethods.UIA_GridPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders()
            {
                if (!this.owner.RowHeadersVisible)
                {
                    return null;
                }

                UnsafeNativeMethods.IRawElementProviderSimple[] result = new UnsafeNativeMethods.IRawElementProviderSimple[this.owner.Rows.Count];
                for (int i = 0; i < this.owner.Rows.Count; i++)
                {
                    result[i] = this.owner.Rows[i].HeaderCell.AccessibilityObject;
                }
                return result;
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal override UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaders()
            {
                if (!this.owner.ColumnHeadersVisible)
                {
                    return null;
                }

                UnsafeNativeMethods.IRawElementProviderSimple[] result = new UnsafeNativeMethods.IRawElementProviderSimple[this.owner.Columns.Count];
                for (int i = 0; i < this.owner.Columns.Count; i++)
                {
                    result[i] = this.owner.Columns[i].HeaderCell.AccessibilityObject;
                }
                return result;
            }

            internal override UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return UnsafeNativeMethods.RowOrColumnMajor.RowOrColumnMajor_RowMajor;
                }
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal override UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column)
            {
                if (row >= 0 && row < this.owner.Rows.Count &&
                    column >= 0 && column < this.owner.Columns.Count)
                {
                    return this.owner.Rows[row].Cells[column].AccessibilityObject;
                }

                return null;
            }

            internal override int RowCount
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.RowCount;
                }
            }

            internal override int ColumnCount
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.ColumnCount;
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return this.Bounds;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return this;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                        int childCount = GetChildCount();
                        if (childCount > 0)
                        {
                            return GetChild(0);
                        }
                        break;
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
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

            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                return this.HitTest((int)x, (int)y);
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

            #endregion
        }

        internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
        {
            private DataGridView dataGridView;
            private Panel panel;

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

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
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

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return dataGridView.CurrentCell.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.FirstChild:
                    case UnsafeNativeMethods.NavigateDirection.LastChild:
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

            internal override bool IsPatternSupported(int patternId)
            {
                return patternId.Equals(NativeMethods.UIA_LegacyIAccessiblePatternId);
            }

            internal override object GetPropertyValue(int propertyId)
            {
                switch (propertyId)
                {
                    case NativeMethods.UIA_NamePropertyId:
                        return string.Format(SR.DataGridView_AccEditingPanelAccName);
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_PaneControlTypeId;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return true;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return this.dataGridView.CurrentCell != null;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return dataGridView.Enabled;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return false;
                    case NativeMethods.UIA_IsControlElementPropertyId:
                    case NativeMethods.UIA_IsContentElementPropertyId:
                        return true;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return this.panel.AccessibilityObject.KeyboardShortcut;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_IsLegacyIAccessiblePatternAvailablePropertyId:
                        return true;
                    case NativeMethods.UIA_ProviderDescriptionPropertyId:
                        return SR.DataGridViewEditingPanelUiaProviderDescription;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
