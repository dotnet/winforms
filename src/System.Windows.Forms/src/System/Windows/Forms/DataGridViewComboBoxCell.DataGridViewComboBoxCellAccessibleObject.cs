// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxCell
    {
        protected class DataGridViewComboBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewComboBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ComboBoxControlTypeId;
                    case UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    DataGridViewComboBoxEditingControl comboBox = Owner.Properties.GetObject(s_propComboBoxCellEditingComboBox) as DataGridViewComboBoxEditingControl;
                    if (comboBox != null)
                    {
                        return comboBox.DroppedDown ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                    }

                    return UiaCore.ExpandCollapseState.Collapsed;
                }
            }
        }
    }
}
