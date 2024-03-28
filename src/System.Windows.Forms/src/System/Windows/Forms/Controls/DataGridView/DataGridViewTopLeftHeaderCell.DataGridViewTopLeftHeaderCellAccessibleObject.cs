// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewTopLeftHeaderCell
{
    protected class DataGridViewTopLeftHeaderCellAccessibleObject : DataGridViewColumnHeaderCellAccessibleObject
    {
        public DataGridViewTopLeftHeaderCellAccessibleObject(DataGridViewTopLeftHeaderCell owner) : base(owner)
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

                if (Owner.DataGridView is null || !Owner.DataGridView.IsHandleCreated)
                {
                    return Rectangle.Empty;
                }

                Rectangle cellRect = Owner.DataGridView.GetCellDisplayRectangle(-1, -1, cutOverflow: false);
                return Owner.DataGridView.RectangleToScreen(cellRect);
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

                if (Owner.DataGridView?.MultiSelect ?? false)
                {
                    return SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction;
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
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                object? value = Owner.Value;
                if (value is not null and not string)
                {
                    // The user set the Value on the DataGridViewTopLeftHeaderCell and it did not set it to a string.
                    // Then the name of the DataGridViewTopLeftHeaderAccessibleObject is String.Empty;
                    //
                    return string.Empty;
                }

                string? strValue = value as string;
                if (string.IsNullOrEmpty(strValue))
                {
                    if (Owner.DataGridView is not null)
                    {
                        if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return SR.DataGridView_AccTopLeftColumnHeaderCellName;
                        }
                        else
                        {
                            return SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL;
                        }
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

                // If all the cells are selected, then the top left header cell accessible object is considered to be selected as well.
                if (Owner.DataGridView is not null && Owner.DataGridView.AreAllCellsSelected(includeInvisibleCells: false))
                {
                    resultState |= AccessibleStates.Selected;
                }

                return resultState;
            }
        }

        public override string Value
        {
            get
            {
                // We changed DataGridViewTopLeftHeaderCellAccessibleObject::Name to return a string
                // However, DataGridViewTopLeftHeaderCellAccessibleObject::Value should still return String.Empty.
                return string.Empty;
            }
        }

        public override void DoDefaultAction()
        {
            if (Owner?.DataGridView?.IsHandleCreated is true)
            {
                Owner.DataGridView.SelectAll();
            }
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            if (Owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (Owner.DataGridView is null)
            {
                return null;
            }

            Debug.Assert(Owner.DataGridView.RowHeadersVisible, "if the row headers are not visible how did you get the top left header cell acc object?");
            switch (navigationDirection)
            {
                case AccessibleNavigation.Previous:
                    return null;
                case AccessibleNavigation.Left:
                    if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                    {
                        return null;
                    }
                    else
                    {
                        return NavigateForward();
                    }

                case AccessibleNavigation.Next:
                    return NavigateForward();
                case AccessibleNavigation.Right:
                    if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                    {
                        return NavigateForward();
                    }
                    else
                    {
                        return null;
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

            if (Owner.DataGridView?.IsHandleCreated != true)
            {
                return;
            }

            // AccessibleSelection.TakeFocus should focus the grid and then focus the first data grid view data cell
            if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
            {
                // Focus the grid
                Owner.DataGridView.Focus();
                if (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0 &&
                    Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible) > 0)
                {
                    // This means that there are visible rows and columns.
                    // Focus the first data cell.
                    DataGridViewRow row = Owner.DataGridView.Rows[Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible)];
                    DataGridViewColumn col = Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible)!;

                    // DataGridView::set_CurrentCell clears the previous selection.
                    // So use SetCurrenCellAddressCore directly.
                    Owner.DataGridView.SetCurrentCellAddressCoreInternal(
                        col.Index,
                        row.Index,
                        setAnchorCellAddress: false,
                        validateCurrentCell: true,
                        throughMouseClick: false);
                }
            }

            // AddSelection selects the entire grid.
            if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
            {
                if (Owner.DataGridView.MultiSelect)
                {
                    Owner.DataGridView.SelectAll();
                }
            }

            // RemoveSelection clears the selection on the entire grid.
            // But only if AddSelection is not set.
            if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                (flags & AccessibleSelection.AddSelection) == 0)
            {
                Owner.DataGridView.ClearSelection();
            }
        }

        private AccessibleObject? NavigateForward()
        {
            if (Owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (Owner.DataGridView is null || Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) == 0)
            {
                return null;
            }

            // return the acc object for the first visible column
            return Owner.DataGridView.AccessibilityObject.GetChild(0)?.GetChild(1);
        }
        #region IRawElementProviderFragment Implementation

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (Owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (Owner.DataGridView is null)
            {
                return null;
            }

            DataGridView dataGridView = Owner.DataGridView;

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return dataGridView.AccessibilityObject.GetChild(0);
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    return null;
                case NavigateDirection.NavigateDirection_NextSibling:
                    if (dataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) == 0)
                    {
                        return null;
                    }

                    return NavigateForward();
                default:
                    return null;
            }
        }

        #endregion

        #region IRawElementProviderSimple Implementation

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyId) =>
            propertyId switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(Owner?.DataGridView?.Enabled ?? false),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyId)
            };

        #endregion
    }
}
