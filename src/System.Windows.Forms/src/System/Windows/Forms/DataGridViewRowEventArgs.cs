// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewRowEventArgs : EventArgs
    {
        public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
        {
            Row = dataGridViewRow ?? throw new ArgumentNullException(nameof(dataGridViewRow));
        }

        public DataGridViewRow Row { get; }
    }
}
