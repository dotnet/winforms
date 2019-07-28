// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private class DataGridViewSelectedCellsAccessibleObject : AccessibleObject
        {
            readonly DataGridView owner;

            public DataGridViewSelectedCellsAccessibleObject(DataGridView owner)
            {
                this.owner = owner;
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
                    return owner.AccessibilityObject;
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
                if (index >= 0 && index < owner.GetCellCount(DataGridViewElementStates.Selected))
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
                return owner.GetCellCount(DataGridViewElementStates.Selected);
            }

            public override AccessibleObject GetSelected()
            {
                return this;
            }

            public override AccessibleObject GetFocused()
            {
                if (owner.CurrentCell != null && owner.CurrentCell.Selected)
                {
                    return owner.CurrentCell.AccessibilityObject;
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
                        if (owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return owner.SelectedCell(0).AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.LastChild:
                        if (owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
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
}
