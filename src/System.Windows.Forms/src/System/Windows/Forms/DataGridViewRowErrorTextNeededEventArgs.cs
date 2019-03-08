// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
    {
        internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
        {
            Debug.Assert(rowIndex >= -1);
            RowIndex = rowIndex;
            ErrorText = errorText;
        }

        public int RowIndex { get; }

        public string ErrorText { get; set; }
    }
}
