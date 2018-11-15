// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewCellErrorTextNeededEventArgs.uex' path='docs/doc[@for="DataGridViewCellErrorTextNeededEventArgs"]/*' />
    public class DataGridViewCellErrorTextNeededEventArgs : DataGridViewCellEventArgs
    {
        private string errorText;

        internal DataGridViewCellErrorTextNeededEventArgs(
            int columnIndex, 
            int rowIndex,
            string errorText) : base(columnIndex, rowIndex)
        {
            this.errorText = errorText;
        }

        /// <include file='doc\DataGridViewCellErrorTextNeededEventArgs.uex' path='docs/doc[@for="DataGridViewCellErrorTextNeededEventArgs.ErrorText"]/*' />
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
    }
}
