// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        ///new class for comparing and sorting Icons ....
        //subhag
        internal class IconComparer : IComparer
        {
            private SortOrder sortOrder;

            public IconComparer(SortOrder currentSortOrder)
            {
                sortOrder = currentSortOrder;
            }

            public SortOrder SortOrder
            {
                set
                {
                    sortOrder = value;
                }
            }

            public int Compare(object? obj1, object? obj2)
            {
                //subhag
                ListViewItem? currentItem = (ListViewItem?)obj1;
                ListViewItem? nextItem = (ListViewItem?)obj2;
                if (sortOrder == SortOrder.Ascending)
                {
                    return string.Compare(currentItem?.Text, nextItem?.Text, false, CultureInfo.CurrentCulture);
                }
                else
                {
                    return string.Compare(nextItem?.Text, currentItem?.Text, false, CultureInfo.CurrentCulture);
                }
            }
        }

        //end subhag
    }
}
