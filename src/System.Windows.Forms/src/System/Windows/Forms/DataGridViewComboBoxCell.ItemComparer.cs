// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxCell : DataGridViewCell
    {
        private sealed class ItemComparer : IComparer
        {
            private readonly DataGridViewComboBoxCell dataGridViewComboBoxCell;

            public ItemComparer(DataGridViewComboBoxCell dataGridViewComboBoxCell)
            {
                this.dataGridViewComboBoxCell = dataGridViewComboBoxCell;
            }

            public int Compare(object item1, object item2)
            {
                if (item1 is null)
                {
                    if (item2 is null)
                    {
                        return 0; //both null, then they are equal
                    }

                    return -1; //item1 is null, but item2 is valid (greater)
                }

                if (item2 is null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }

                string itemName1 = dataGridViewComboBoxCell.GetItemDisplayText(item1);
                string itemName2 = dataGridViewComboBoxCell.GetItemDisplayText(item2);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}
