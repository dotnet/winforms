// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewCheckBoxCell
    {
        protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            private int[] runtimeId; // Used by UIAutomation

            public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            public override AccessibleStates State
            {
                get
                {
                    if ((Owner as DataGridViewCheckBoxCell).EditedFormattedValue is CheckState state)
                    {
                        switch (state)
                        {
                            case CheckState.Checked:
                                return AccessibleStates.Checked | base.State;
                            case CheckState.Indeterminate:
                                return AccessibleStates.Indeterminate | base.State;
                        }
                    }
                    else if ((Owner as DataGridViewCheckBoxCell).EditedFormattedValue is bool stateAsBool)
                    {
                        if (stateAsBool)
                        {
                            return AccessibleStates.Checked | base.State;
                        }
                    }
                    return base.State;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (!Owner.ReadOnly)
                    {
                        // determine if we switch to Checked/Unchecked value
                        bool switchToCheckedState = true;

                        object formattedValue = Owner.FormattedValue;

                        if (formattedValue is CheckState)
                        {
                            switchToCheckedState = ((CheckState)formattedValue) == CheckState.Unchecked;
                        }
                        else if (formattedValue is bool)
                        {
                            switchToCheckedState = !((bool)formattedValue);
                        }

                        if (switchToCheckedState)
                        {
                            return SR.DataGridView_AccCheckBoxCellDefaultActionCheck;
                        }
                        else
                        {
                            return SR.DataGridView_AccCheckBoxCellDefaultActionUncheck;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override void DoDefaultAction()
            {
                DataGridViewCheckBoxCell dataGridViewCell = (DataGridViewCheckBoxCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex == -1)
                {
                    throw new InvalidOperationException(SR.DataGridView_InvalidOperationOnSharedCell);
                }

                if (!dataGridViewCell.ReadOnly && dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.CurrentCell = dataGridViewCell;
                    bool endEditMode = false;
                    if (!dataGridView.IsCurrentCellInEditMode)
                    {
                        endEditMode = true;
                        dataGridView.BeginEdit(false /*selectAll*/);
                    }
                    if (dataGridView.IsCurrentCellInEditMode)
                    {
                        if (dataGridViewCell.SwitchFormattedValue())
                        {
                            dataGridViewCell.NotifyDataGridViewOfValueChange();
                            dataGridView.InvalidateCell(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex);

                            // notify MSAA clients that the default action changed
                            if (Owner is DataGridViewCheckBoxCell checkBoxCell)
                            {
                                checkBoxCell.NotifyMASSClient(new Point(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                            }
                        }
                        if (endEditMode)
                        {
                            dataGridView.EndEdit();
                        }
                    }
                }
            }

            public override int GetChildCount()
            {
                return 0;
            }

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

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsTogglePatternAvailablePropertyId)
                {
                    return (object)IsPatternSupported(UiaCore.UIA.TogglePatternId);
                }
                else if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.CheckBoxControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.TogglePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void Toggle()
            {
                DoDefaultAction();
            }

            internal override UiaCore.ToggleState ToggleState
            {
                get
                {
                    bool toggledState = true;
                    object formattedValue = Owner.FormattedValue;

                    if (formattedValue is CheckState)
                    {
                        toggledState = ((CheckState)formattedValue) == CheckState.Checked;
                    }
                    else if (formattedValue is bool)
                    {
                        toggledState = ((bool)formattedValue);
                    }
                    else
                    {
                        return UiaCore.ToggleState.Indeterminate;
                    }

                    return toggledState ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;
                }
            }
        }
    }
}
