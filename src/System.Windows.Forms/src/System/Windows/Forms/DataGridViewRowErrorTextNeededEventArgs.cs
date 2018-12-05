// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
    {
        private int rowIndex;
        private string errorText;

        internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
        {
            Debug.Assert(rowIndex >= -1);
            this.rowIndex = rowIndex;
            this.errorText = errorText;
        }

        public string ErrorText
        {
            get
            {
                return this.errorText;
            }
            set
            {
                this.errorText = value;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
