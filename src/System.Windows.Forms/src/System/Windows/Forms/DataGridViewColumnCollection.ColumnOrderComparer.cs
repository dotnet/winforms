// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class DataGridViewColumnCollection
    {
        private class ColumnOrderComparer : IComparer
        {
            public ColumnOrderComparer()
            {
            }

            public int Compare(object x, object y)
            {
                Debug.Assert(x is not null);
                Debug.Assert(y is not null);

                DataGridViewColumn dataGridViewColumn1 = x as DataGridViewColumn;
                DataGridViewColumn dataGridViewColumn2 = y as DataGridViewColumn;

                Debug.Assert(dataGridViewColumn1 is not null);
                Debug.Assert(dataGridViewColumn2 is not null);

                return dataGridViewColumn1.DisplayIndex - dataGridViewColumn2.DisplayIndex;
            }
        }
    }
}
