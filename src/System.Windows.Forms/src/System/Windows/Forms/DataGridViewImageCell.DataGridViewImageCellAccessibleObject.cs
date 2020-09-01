// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewImageCell
    {
        protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewImageCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            public override string DefaultAction
            {
                get
                {
                    return string.Empty;
                }
            }

            public override string Description
            {
                get
                {
                    if (Owner is DataGridViewImageCell imageCell)
                    {
                        return imageCell.Description;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public override string Value
            {
                get => base.Value;

                set
                {
                    // do nothing.
                }
            }

            public override void DoDefaultAction()
            {
                DataGridViewImageCell dataGridViewCell = (DataGridViewImageCell)Owner;
                DataGridView dataGridView = dataGridViewCell.DataGridView;

                if (dataGridView != null && dataGridViewCell.RowIndex != -1 &&
                    dataGridViewCell.OwningColumn != null && dataGridViewCell.OwningRow != null)
                {
                    dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
                }
            }

            public override int GetChildCount()
            {
                return 0;
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.ImageControlTypeId;
                }

                if (propertyID == UiaCore.UIA.IsInvokePatternAvailablePropertyId)
                {
                    return true;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.InvokePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
