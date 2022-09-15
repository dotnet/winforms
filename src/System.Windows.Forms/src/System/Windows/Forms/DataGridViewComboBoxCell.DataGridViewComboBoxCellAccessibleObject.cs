// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => IsInComboBoxMode
                        ? UiaCore.UIA.ComboBoxControlTypeId
                        : UiaCore.UIA.DataItemControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ExpandCollapsePatternId => IsInComboBoxMode,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    if (Owner is null)
                    {
                        throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                    }

                    if (Owner.Properties.GetObject(s_propComboBoxCellEditingComboBox) is DataGridViewComboBoxEditingControl comboBox && comboBox.IsHandleCreated)
                    {
                        return comboBox.DroppedDown ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                    }

                    return UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            private bool IsInComboBoxMode
                => _owningComboBoxCell is not null &&
                (_owningComboBoxCell.DisplayStyle != DataGridViewComboBoxDisplayStyle.Nothing || _owningComboBoxCell.IsInEditMode);
        }
    }
}
