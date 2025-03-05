// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewLinkCell
{
    protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        public DataGridViewLinkCellAccessibleObject(DataGridViewCell? owner) : base(owner)
        {
        }

        public override string DefaultAction => SR.DataGridView_AccLinkCellDefaultAction;

        public override void DoDefaultAction()
        {
            if (Owner is null)
            {
                throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
            }

            if (Owner is not DataGridViewLinkCell dataGridViewCell)
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

            if (dataGridViewCell.OwningColumn is not null && dataGridViewCell.OwningRow is not null)
            {
                dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
            }
        }

        public override int GetChildCount() => 0;

        internal override bool IsIAccessibleExSupported() => true;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
