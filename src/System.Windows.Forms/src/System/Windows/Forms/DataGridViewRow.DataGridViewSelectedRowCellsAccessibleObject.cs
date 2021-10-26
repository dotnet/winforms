﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class DataGridViewRow
    {
        private class DataGridViewSelectedRowCellsAccessibleObject : AccessibleObject
        {
            private readonly DataGridViewRow owner;
            private int[] runtimeId;

            internal DataGridViewSelectedRowCellsAccessibleObject(DataGridViewRow owner)
            {
                this.owner = owner;
            }

            public override string Name => SR.DataGridView_AccSelectedRowCellsName;

            public override AccessibleObject Parent => owner.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Grouping;

            public override AccessibleStates State
            {
                get => AccessibleStates.Selected | AccessibleStates.Selectable;
            }

            public override string Value => Name;

            internal override int[] RuntimeId
                => runtimeId ??= new int[] { RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode() };

            public override AccessibleObject GetChild(int index)
            {
                if (index < GetChildCount())
                {
                    int selectedCellsCount = -1;
                    for (int i = 1; i < owner.AccessibilityObject.GetChildCount(); i++)
                    {
                        if ((owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                        {
                            selectedCellsCount++;
                        }

                        if (selectedCellsCount == index)
                        {
                            return owner.AccessibilityObject.GetChild(i);
                        }
                    }

                    Debug.Assert(false, "we should have found already the selected cell");
                    return null;
                }
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                int selectedCellsCount = 0;

                // start the enumeration from 1, because the first acc obj in the data grid view row is the row header cell
                for (int i = 1; i < owner.AccessibilityObject.GetChildCount(); i++)
                {
                    if ((owner.AccessibilityObject.GetChild(i).State & AccessibleStates.Selected) == AccessibleStates.Selected)
                    {
                        selectedCellsCount++;
                    }
                }

                return selectedCellsCount;
            }

            public override AccessibleObject GetSelected() => this;

            public override AccessibleObject GetFocused()
            {
                if (owner.DataGridView?.CurrentCell is not null && owner.DataGridView.CurrentCell.Selected)
                {
                    return owner.DataGridView.CurrentCell.AccessibilityObject;
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
}
