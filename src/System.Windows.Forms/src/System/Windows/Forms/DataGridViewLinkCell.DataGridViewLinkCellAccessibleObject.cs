// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewLinkCell
    {
        protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewLinkCellAccessibleObject(DataGridViewCell? owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.DataGridView_AccLinkCellDefaultAction;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewLinkCell dataGridViewCell))
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
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            public override int GetChildCount() => 0;

            internal override bool IsIAccessibleExSupported() => true;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.HyperlinkControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
