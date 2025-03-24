// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewImageCell
{
    protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        public DataGridViewImageCellAccessibleObject(DataGridViewCell? owner) : base(owner)
        {
        }

        public override string DefaultAction => string.Empty;

        public override string? Description => Owner is DataGridViewImageCell imageCell ? imageCell.Description : null;

        internal override bool CanGetDescriptionInternal => false;

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

            if (Owner is not DataGridViewImageCell dataGridViewCell)
            {
                return;
            }

            DataGridView? dataGridView = dataGridViewCell.DataGridView;
            if (dataGridView is not null &&
                dataGridView.IsHandleCreated &&
                dataGridViewCell.RowIndex != -1 &&
                dataGridViewCell.OwningColumn is not null &&
                dataGridViewCell.OwningRow is not null)
            {
                dataGridView.OnCellContentClickInternal(new DataGridViewCellEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex));
            }
        }

        public override int GetChildCount() => 0;

        internal override bool IsIAccessibleExSupported() => true;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId == UIA_PATTERN_ID.UIA_InvokePatternId || base.IsPatternSupported(patternId);
    }
}
