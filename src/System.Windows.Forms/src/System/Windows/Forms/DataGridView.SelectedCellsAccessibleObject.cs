// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private class DataGridViewSelectedCellsAccessibleObject : AccessibleObject
        {
            private readonly DataGridView _owner;

            public DataGridViewSelectedCellsAccessibleObject(DataGridView owner)
            {
                _owner = owner;
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
                    return _owner.AccessibilityObject;
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

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < _owner.GetCellCount(DataGridViewElementStates.Selected))
                {
                    return _owner.SelectedCell(index).AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                return _owner.GetCellCount(DataGridViewElementStates.Selected);
            }

            public override AccessibleObject GetSelected()
            {
                return this;
            }

            public override AccessibleObject GetFocused()
            {
                if (_owner.CurrentCell != null && _owner.CurrentCell.Selected)
                {
                    return _owner.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                switch (navigationDirection)
                {
                    case AccessibleNavigation.FirstChild:
                        if (_owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return _owner.SelectedCell(0).AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.LastChild:
                        if (_owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return _owner.SelectedCell(_owner.GetCellCount(DataGridViewElementStates.Selected) - 1).AccessibilityObject;
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
