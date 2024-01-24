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

        public override string Name => SR.DataGridView_AccSelectedCellsName;

        internal override bool CanGetNameInternal => false;

        public override AccessibleObject Parent => _parentAccessibleObject;

        private protected override bool IsInternal => true;

        public override AccessibleRole Role => AccessibleRole.Grouping;

        public override AccessibleStates State => AccessibleStates.Selected | AccessibleStates.Selectable;

        public override string Value => Name;

        internal override bool CanGetValueInternal => false;

        internal override int[] RuntimeId => [RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode()];

        public override AccessibleObject? GetChild(int index) =>
            _parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner) && index >= 0 && index < owner.GetCellCount(DataGridViewElementStates.Selected)
                ? owner.SelectedCell(index)?.AccessibilityObject
                : null;

        public override int GetChildCount() =>
            _parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner)
                ? owner.GetCellCount(DataGridViewElementStates.Selected)
                : base.GetChildCount();

        public override AccessibleObject GetSelected() => this;

        public override AccessibleObject? GetFocused() =>
            _parentAccessibleObject.TryGetOwnerAs(out DataGridView? owner) && owner.CurrentCell is not null && owner.CurrentCell.Selected
                ? owner.CurrentCell.AccessibilityObject
                : null;

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            DataGridView? owner;

            switch (navigationDirection)
            {
                case AccessibleNavigation.FirstChild:
                    return _parentAccessibleObject.TryGetOwnerAs(out owner) && owner.GetCellCount(DataGridViewElementStates.Selected) > 0
                        ? owner.SelectedCell(0)?.AccessibilityObject
                        : null;

                case AccessibleNavigation.LastChild:
                    return _parentAccessibleObject.TryGetOwnerAs(out owner) && owner.GetCellCount(DataGridViewElementStates.Selected) > 0
                        ? owner.SelectedCell(owner.GetCellCount(DataGridViewElementStates.Selected) - 1)?.AccessibilityObject
                        : null;

                default:
                    {
                        return null;
                    }
            }
        }
    }
}
