// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewButtonCell
    {
        protected class DataGridViewButtonCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewButtonCellAccessibleObject(DataGridViewCell? owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.DataGridView_AccButtonCellDefaultAction;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewButtonCell dataGridViewCell))
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

                if (dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            public override int GetChildCount() => 0;

            internal override bool IsIAccessibleExSupported() => true;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID == UiaCore.UIA.ControlTypePropertyId
                    ? UiaCore.UIA.ButtonControlTypeId
                    : base.GetPropertyValue(propertyID);
        }
    }
}
