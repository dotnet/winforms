// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public class DataGridViewColumnEventArgs : EventArgs
    {
        public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }

            Debug.Assert(dataGridViewColumn.Index >= -1);
            Column = dataGridViewColumn;
        }

        public DataGridViewColumn Column { get; }
    }
}
