// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private class DataGridViewSelectedCellsAccessibleObject : AccessibleObject
        {
            DataGridView owner;

            public DataGridViewSelectedCellsAccessibleObject(DataGridView owner)
            {
                this.owner = owner;
            }

            public override string Name
            {
                get
                {
                    return string.Format(SR.DataGridView_AccSelectedCellsName);
                }
            }

            public override AccessibleObject Parent
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.owner.AccessibilityObject;
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
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return this.Name;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < this.owner.GetCellCount(DataGridViewElementStates.Selected))
                {
                    return this.owner.SelectedCell(index).AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                return this.owner.GetCellCount(DataGridViewElementStates.Selected);
            }

            public override AccessibleObject GetSelected()
            {
                return this;
            }

            public override AccessibleObject GetFocused()
            {
                if (this.owner.CurrentCell != null && this.owner.CurrentCell.Selected)
                {
                    return this.owner.CurrentCell.AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                switch (navigationDirection)
                {
                    case AccessibleNavigation.FirstChild:
                        if (this.owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return this.owner.SelectedCell(0).AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }
                    case AccessibleNavigation.LastChild:
                        if (this.owner.GetCellCount(DataGridViewElementStates.Selected) > 0)
                        {
                            return this.owner.SelectedCell(this.owner.GetCellCount(DataGridViewElementStates.Selected) - 1).AccessibilityObject;
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