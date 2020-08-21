// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewCheckBoxCell
    {
        protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            private int[] runtimeId = null!; // Used by UIAutomation

            public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell? owner) : base(owner)
            {
            }

            public override AccessibleStates State
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    if (!(Owner is DataGridViewButtonCell dataGridViewCheckBoxCell))
                    {
                        return base.State;
                    }

                    switch (dataGridViewCheckBoxCell.EditedFormattedValue)
                    {
                        case CheckState state:
                            return state switch
                            {
                                CheckState.Checked => AccessibleStates.Checked | base.State,
                                CheckState.Indeterminate => AccessibleStates.Indeterminate | base.State,
                                _ => base.State
                            };
                        case bool stateAsBool:
                            return stateAsBool ? AccessibleStates.Checked | base.State : base.State;
                        default:
                            return base.State;
                    }
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

                    if (Owner.ReadOnly)
                    {
                        return string.Empty;
                    }

                    bool switchToCheckedState = true;

                    switch (Owner.FormattedValue)
                    {
                        case CheckState checkState:
                            switchToCheckedState = checkState == CheckState.Unchecked;
                            break;
                        case bool boolState:
                            switchToCheckedState = !boolState;
                            break;
                    }

                    return switchToCheckedState ? SR.DataGridView_AccCheckBoxCellDefaultActionCheck : SR.DataGridView_AccCheckBoxCellDefaultActionUncheck;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewCheckBoxCell dataGridViewCell))
                {
                    return;
                }

                if (dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                DataGridView? dataGridView = dataGridViewCell.DataGridView;
                if (dataGridView?.IsHandleCreated != true)
                {
                    return;
                }

                if (!dataGridViewCell.ReadOnly && dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.CurrentCell = dataGridViewCell;
                    bool endEditMode = false;
                    if (!dataGridView.IsCurrentCellInEditMode)
                    {
                        endEditMode = true;
                        dataGridView.BeginEdit(selectAll: false);
                    }
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        if (dataGridViewCell.SwitchFormattedValue())
                        {
                            dataGridViewCell.NotifyDataGridViewOfValueChange();
                            dataGridView.InvalidateCell(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex);

                            if (Owner is DataGridViewCheckBoxCell checkBoxCell)
                            {
                                checkBoxCell.NotifyMSAAClient(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex);
                                checkBoxCell.NotifyUiaClient();
                            }
                        }
                        if (endEditMode)
                        {
                            dataGridView.EndEdit();
                        }
                    }
                }
            }

            public override int GetChildCount() => 0;

            internal override bool IsIAccessibleExSupported() => true;

            internal override int[] RuntimeId
            {
                get
                {
                    if (runtimeId is null)
                    {
                        runtimeId = new int[2];
                        runtimeId[0] = RuntimeIDFirstItem; // first item is static - 0x2a
                        runtimeId[1] = GetHashCode();
                    }

                    return runtimeId;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.IsTogglePatternAvailablePropertyId => (object)IsPatternSupported(UiaCore.UIA.TogglePatternId),
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.CheckBoxControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId == UiaCore.UIA.TogglePatternId ? true : base.IsPatternSupported(patternId);

            internal override void Toggle() => DoDefaultAction();

            internal override UiaCore.ToggleState ToggleState
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    bool toggledState;
                    switch (Owner.FormattedValue)
                    {
                        case CheckState checkState:
                            toggledState = checkState == CheckState.Unchecked;
                            break;
                        case bool boolState:
                            toggledState = !boolState;
                            break;
                        default:
                            return UiaCore.ToggleState.Indeterminate;
                    }

                    return toggledState ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;
                }
            }
        }
    }
}
