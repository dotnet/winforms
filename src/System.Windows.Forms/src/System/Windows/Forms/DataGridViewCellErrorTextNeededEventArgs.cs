// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewCellErrorTextNeededEventArgs : DataGridViewCellEventArgs
    {
        internal DataGridViewCellErrorTextNeededEventArgs(int columnIndex, int rowIndex, string errorText) : base(columnIndex, rowIndex)
        {
            ErrorText = errorText;
        }

        public string ErrorText { get; set; }
    }
}
