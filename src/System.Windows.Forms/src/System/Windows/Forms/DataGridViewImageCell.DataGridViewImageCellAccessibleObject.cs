// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewImageCell
    {
        protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewImageCellAccessibleObject(DataGridViewCell? owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    return string.Empty;
                }
            }

            public override string? Description => Owner is DataGridViewImageCell imageCell ? imageCell.Description : null;

            public override string? Value
            {
                get => base.Value;
                set
                {
                    // do nothing.
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (!(Owner is DataGridViewImageCell dataGridViewCell))
                {
                    return;
                }

                DataGridView? dataGridView = dataGridViewCell.DataGridView;
                if (dataGridView != null &&
                    dataGridView.IsHandleCreated &&
                    dataGridViewCell.RowIndex != -1 &&
                    dataGridViewCell.OwningColumn != null &&
                    dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            public override int GetChildCount() => 0;

            internal override bool IsIAccessibleExSupported() => true;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ImageControlTypeId,
                    UiaCore.UIA.IsInvokePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId == UiaCore.UIA.InvokePatternId ? true : base.IsPatternSupported(patternId);
        }
    }
}
