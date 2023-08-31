// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridView
{
    private class DataGridViewSelectedCellsAccessibleObject : AccessibleObject
    {
        private readonly DataGridViewAccessibleObject _parentAccessibleObject;

        public DataGridViewSelectedCellsAccessibleObject(DataGridViewAccessibleObject parentAccessibleObject)
        {
            _parentAccessibleObject = parentAccessibleObject;
        }

        public override string Name
        {
            get
            {
                return SR.DataGridView_AccSelectedCellsName;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return _parentAccessibleObject;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Grouping;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                return AccessibleStates.Selected | AccessibleStates.Selectable;
            }
        }

        public override string Value
        {
            get
            {
                return Name;
            }
        }

        internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode() };

        public override AccessibleObject? GetChild(int index)
        {
            if (_parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner) && index >= 0 && index < owner.GetCellCount(DataGridViewElementStates.Selected))
            {
                return owner.SelectedCell(index).AccessibilityObject;
            }
            else
            {
                return null;
            }
        }

        public override int GetChildCount()
        {
            return _parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner) ? owner.GetCellCount(DataGridViewElementStates.Selected) : base.GetChildCount();
        }

        public override AccessibleObject GetSelected()
        {
            return this;
        }

        public override AccessibleObject? GetFocused()
        {
            if (_parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner) && owner.CurrentCell is not null && owner.CurrentCell.Selected)
            {
                return owner.CurrentCell.AccessibilityObject;
            }
            else
            {
                return null;
            }
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            DataGridView? owner;

            switch (navigationDirection)
            {
                case AccessibleNavigation.FirstChild:
                    if (_parentAccessibleObject.TryGetOwnerAs(out owner) && owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        return owner.SelectedCell(0).AccessibilityObject;
                    }
                    else
                    {
                        return null;
                    }

                case AccessibleNavigation.LastChild:
                    if (_parentAccessibleObject.TryGetOwnerAs(out owner) && owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                    {
                        return owner.SelectedCell(owner.GetCellCount(DataGridViewElementStates.Selected) - 1).AccessibilityObject;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    {
                        return null;
                    }
            }
        }
    }
}
