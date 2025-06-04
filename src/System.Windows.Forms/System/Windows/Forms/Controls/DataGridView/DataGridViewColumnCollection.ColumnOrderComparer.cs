// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridViewColumnCollection
{
    private class ColumnOrderComparer : IComparer<DataGridViewColumn?>
    {
        public ColumnOrderComparer()
        {
        }

        public int Compare(DataGridViewColumn? x, DataGridViewColumn? y)
        {
            Debug.Assert(x is not null);
            Debug.Assert(y is not null);

            return x.DisplayIndex - y.DisplayIndex;
        }
    }
}
