// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridViewRow
{
    private class DataGridViewSelectedRowCellsAccessibleObject : AccessibleObject
    {
        private readonly DataGridViewRow _owningDataGridViewRow;
        private int[]? _runtimeId;

        internal DataGridViewSelectedRowCellsAccessibleObject(DataGridViewRow owner)
        {
            _owningDataGridViewRow = owner;
        }

        public override string Name => SR.DataGridView_AccSelectedRowCellsName;

        internal override bool CanGetNameInternal => false;

        public override AccessibleObject Parent => _owningDataGridViewRow.AccessibilityObject;

        private protected override bool IsInternal => true;

        public override AccessibleRole Role => AccessibleRole.Grouping;

        public override AccessibleStates State
        {
            get => AccessibleStates.Selected | AccessibleStates.Selectable;
        }

        public override string Value => Name;

        internal override bool CanGetValueInternal => false;

        internal override int[] RuntimeId => _runtimeId ??= [RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode()];

        public override AccessibleObject? GetChild(int index)
        {
            if (index >= GetChildCount())
            {
                return null;
            }

            int selectedCellsCount = -1;
            for (int i = 1; i < _owningDataGridViewRow.AccessibilityObject.GetChildCount(); i++)
            {
                AccessibleObject? child = _owningDataGridViewRow.AccessibilityObject.GetChild(i);
                if (child is not null && (child.State & AccessibleStates.Selected) == AccessibleStates.Selected)
                {
                    selectedCellsCount++;
                }

                if (selectedCellsCount == index)
                {
                    return child;
                }
            }

            Debug.Assert(false, "we should have found already the selected cell");
            return null;
        }

        public override int GetChildCount()
        {
            int selectedCellsCount = 0;

            // start the enumeration from 1, because the first acc obj in the data grid view row is the row header cell
            for (int i = 1; i < _owningDataGridViewRow.AccessibilityObject.GetChildCount(); i++)
            {
                AccessibleObject? child = _owningDataGridViewRow.AccessibilityObject.GetChild(i);
                if (child is not null && (child.State & AccessibleStates.Selected) == AccessibleStates.Selected)
                {
                    selectedCellsCount++;
                }
            }

            return selectedCellsCount;
        }

        public override AccessibleObject GetSelected() => this;

        public override AccessibleObject? GetFocused()
        {
            DataGridViewCell? currentCell = _owningDataGridViewRow.DataGridView?.CurrentCell;
            if (currentCell is not null && currentCell.Selected)
            {
                return currentCell.AccessibilityObject;
            }
            else
            {
                return null;
            }
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            switch (navigationDirection)
            {
                case AccessibleNavigation.FirstChild:
                    if (GetChildCount() > 0)
                    {
                        return GetChild(0);
                    }
                    else
                    {
                        return null;
                    }

                case AccessibleNavigation.LastChild:
                    if (GetChildCount() > 0)
                    {
                        return GetChild(GetChildCount() - 1);
                    }
                    else
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }
    }
}
