// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class DataGridViewRow
    {
        private class DataGridViewSelectedRowCellsAccessibleObject : AccessibleObject
        {
            private readonly DataGridViewRow _owner;
            private int[]? _runtimeId;

            internal DataGridViewSelectedRowCellsAccessibleObject(DataGridViewRow owner)
            {
                _owner = owner;
            }

            public override string Name => SR.DataGridView_AccSelectedRowCellsName;

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Grouping;

            public override AccessibleStates State
            {
                get => AccessibleStates.Selected | AccessibleStates.Selectable;
            }

            public override string Value => Name;

            internal override int[] RuntimeId
                => _runtimeId ??= new int[] { RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode() };

            public override AccessibleObject? GetChild(int index)
            {
                if (index < GetChildCount())
                {
                    int selectedCellsCount = -1;
                    for (int i = 1; i < _owner.AccessibilityObject.GetChildCount(); i++)
                    {
                        AccessibleObject? child = _owner.AccessibilityObject.GetChild(i);
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
                else
                {
                    return null;
                }
            }

            public override int GetChildCount()
            {
                int selectedCellsCount = 0;

                // start the enumeration from 1, because the first acc obj in the data grid view row is the row header cell
                for (int i = 1; i < _owner.AccessibilityObject.GetChildCount(); i++)
                {
                    AccessibleObject? child = _owner.AccessibilityObject.GetChild(i);
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
                if (_owner.DataGridView?.CurrentCell is not null && _owner.DataGridView.CurrentCell.Selected)
                {
                    return _owner.DataGridView.CurrentCell.AccessibilityObject;
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
}
