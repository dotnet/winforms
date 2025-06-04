// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxCell
{
    protected class DataGridViewComboBoxCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        private readonly DataGridViewComboBoxCell? _owningComboBoxCell;

        public DataGridViewComboBoxCellAccessibleObject(DataGridViewCell? owner) : base(owner)
        {
            _owningComboBoxCell = owner as DataGridViewComboBoxCell;
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => IsInComboBoxMode
                ? (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId
                : (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId,
            _ => base.GetPropertyValue(propertyID)
        };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            UIA_PATTERN_ID.UIA_ExpandCollapsePatternId => IsInComboBoxMode,
            _ => base.IsPatternSupported(patternId)
        };

        internal override ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.Properties.TryGetValue(s_propComboBoxCellEditingComboBox, out DataGridViewComboBoxEditingControl? comboBox)
                    && comboBox.IsHandleCreated)
                {
                    return comboBox.DroppedDown
                        ? ExpandCollapseState.ExpandCollapseState_Expanded
                        : ExpandCollapseState.ExpandCollapseState_Collapsed;
                }

                return ExpandCollapseState.ExpandCollapseState_Collapsed;
            }
        }

        private bool IsInComboBoxMode => _owningComboBoxCell is not null
            && (_owningComboBoxCell.DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing || _owningComboBoxCell.IsInEditMode);
    }
}
