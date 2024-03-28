// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxCell : DataGridViewCell
{
    private sealed class ItemComparer : IComparer<object?>
    {
        private readonly DataGridViewComboBoxCell _dataGridViewComboBoxCell;

        public ItemComparer(DataGridViewComboBoxCell dataGridViewComboBoxCell)
        {
            _dataGridViewComboBoxCell = dataGridViewComboBoxCell;
        }

        public int Compare(object? item1, object? item2)
        {
            if (IComparerHelpers.CompareReturnIfNull(item1, item2, out int? returnValue))
            {
                return (int)returnValue;
            }

            string? itemName1 = _dataGridViewComboBoxCell.GetItemDisplayText(item1);
            string? itemName2 = _dataGridViewComboBoxCell.GetItemDisplayText(item2);

            CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
            return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
        }
    }
}
