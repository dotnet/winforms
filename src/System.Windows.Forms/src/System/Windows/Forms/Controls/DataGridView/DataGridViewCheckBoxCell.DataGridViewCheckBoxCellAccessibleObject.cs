// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewCheckBoxCell
{
    protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        private int[] _runtimeId = null!;

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

                if (Owner is not DataGridViewButtonCell dataGridViewCheckBoxCell)
                {
                    return base.State;
                }

                return dataGridViewCheckBoxCell.EditedFormattedValue switch
                {
                    CheckState state => state switch
                    {
                        CheckState.Checked => AccessibleStates.Checked | base.State,
                        CheckState.Indeterminate => AccessibleStates.Indeterminate | base.State,
                        _ => base.State
                    },
                    bool stateAsBool => stateAsBool ? AccessibleStates.Checked | base.State : base.State,
                    _ => base.State,
                };
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

            if (Owner is not DataGridViewCheckBoxCell dataGridViewCell)
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

            if (!dataGridViewCell.ReadOnly && dataGridViewCell.OwningColumn is not null && dataGridViewCell.OwningRow is not null)
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

        internal override int[] RuntimeId => _runtimeId ??= [RuntimeIDFirstItem, GetHashCode()];

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId == UIA_PATTERN_ID.UIA_TogglePatternId || base.IsPatternSupported(patternId);

        internal override void Toggle() => DoDefaultAction();

        internal override ToggleState ToggleState
        {
            get
            {
                if (Owner is not DataGridViewCheckBoxCell)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerWrongType, Owner is null ? "null" : Owner.GetType().Name));
                }

                return ((Owner as DataGridViewCheckBoxCell)?.CheckState) switch
                {
                    CheckState.Checked => ToggleState.ToggleState_On,
                    CheckState.Unchecked => ToggleState.ToggleState_Off,
                    _ => ToggleState.ToggleState_Indeterminate,
                };
            }
        }
    }
}
