// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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
            : Owner.DataGridView?.AccessibilityObject.GetChild(0);

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
                    if (Owner.DataGridView.SelectionMode is DataGridViewSelectionMode.FullColumnSelect
                        or DataGridViewSelectionMode.ColumnHeaderSelect)
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

            if (Owner is not DataGridViewColumnHeaderCell dataGridViewCell)
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
            else if (dataGridView.SelectionMode is DataGridViewSelectionMode.FullColumnSelect or
                     DataGridViewSelectionMode.ColumnHeaderSelect)
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

            return navigationDirection switch
            {
                AccessibleNavigation.Right => Owner.DataGridView.RightToLeft == RightToLeft.No ? NavigateForward() : NavigateBackward(),
                AccessibleNavigation.Next => NavigateForward(),
                AccessibleNavigation.Left => Owner.DataGridView.RightToLeft == RightToLeft.No ? NavigateBackward() : NavigateForward(),
                AccessibleNavigation.Previous => NavigateBackward(),
                _ => null,
            };
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

            if (Owner is not DataGridViewColumnHeaderCell dataGridViewCell)
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

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (Owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => Parent,
                NavigateDirection.NavigateDirection_NextSibling => NavigateForward(),
                NavigateDirection.NavigateDirection_PreviousSibling => NavigateBackward(),
                _ => null
            };
        }

        #endregion

        #region IRawElementProviderSimple Implementation

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId.Equals(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId) ||
                patternId.Equals(UIA_PATTERN_ID.UIA_InvokePatternId);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyId)
            => propertyId switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(Owner?.DataGridView?.Enabled ?? false),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                UIA_PROPERTY_ID.UIA_IsPasswordPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyId)
            };

        #endregion
    }
}
