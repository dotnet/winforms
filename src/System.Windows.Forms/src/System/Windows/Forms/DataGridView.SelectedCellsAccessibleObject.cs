// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private class DataGridViewSelectedCellsAccessibleObject : AccessibleObject
        {
            private readonly DataGridView _ownerDataGridView;

            public DataGridViewSelectedCellsAccessibleObject(DataGridView owner)
            {
                _ownerDataGridView = owner;
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
                    return _ownerDataGridView.AccessibilityObject;
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
                if (index >= 0 && index < _ownerDataGridView.GetCellCount(DataGridViewElementStates.Selected))
                {
                    return _ownerDataGridView.SelectedCell(index).AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                return _ownerDataGridView.GetCellCount(DataGridViewElementStates.Selected);
            }

            public override AccessibleObject GetSelected()
            {
                return this;
            }

            public override AccessibleObject? GetFocused()
            {
                if (_ownerDataGridView.CurrentCell is not null && _ownerDataGridView.CurrentCell.Selected)
                {
                    return _ownerDataGridView.CurrentCell.AccessibilityObject;
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
                        if (_ownerDataGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return _ownerDataGridView.SelectedCell(0).AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }

                    case AccessibleNavigation.LastChild:
                        if (_ownerDataGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return _ownerDataGridView.SelectedCell(_ownerDataGridView.GetCellCount(DataGridViewElementStates.Selected) - 1).AccessibilityObject;
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
}
