// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewDataErrorEventArgs.uex' path='docs/doc[@for="DataGridViewDataErrorEventArgs"]/*' />
    public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
    {
        private Exception exception;
        private bool throwException;
        private DataGridViewDataErrorContexts context;
    
        /// <include file='doc\DataGridViewDataErrorEventArgs.uex' path='docs/doc[@for="DataGridViewDataErrorEventArgs.DataGridViewDataErrorEventArgs"]/*' />
        public DataGridViewDataErrorEventArgs(Exception exception,
            int columnIndex,
            int rowIndex,
            DataGridViewDataErrorContexts context) : base(columnIndex, rowIndex)
        {
            Debug.Assert(rowIndex > -1);
            this.exception = exception;
            this.context = context;
        }

        /// <include file='doc\DataGridViewDataErrorEventArgs.uex' path='docs/doc[@for="DataGridViewDataErrorEventArgs.Context"]/*' />
        public DataGridViewDataErrorContexts Context
        {
            get
            {
                return this.context;
            }
        }

        /// <include file='doc\DataGridViewDataErrorEventArgs.uex' path='docs/doc[@for="DataGridViewDataErrorEventArgs.Exception"]/*' />
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        /// <include file='doc\DataGridViewDataErrorEventArgs.uex' path='docs/doc[@for="DataGridViewDataErrorEventArgs.ThrowException"]/*' />
        public bool ThrowException
        {
            get
            {
                return this.throwException;
            }
            set
            {
                if (value && this.exception == null)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_CannotThrowNullException));
                }
                this.throwException = value;
            }
        }
    }
}
