// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.ComponentModel;

    /// <include file='doc\DataGridViewBindingCompleteEventArgs.uex' path='docs/doc[@for="DataGridViewBindingCompleteEventArgs"]/*' />
    public class DataGridViewBindingCompleteEventArgs : EventArgs
    {
        private ListChangedType listChangedType;

        /// <include file='doc\DataGridViewBindingCompleteEventArgs.uex' path='docs/doc[@for="DataGridViewBindingCompleteEventArgs.DataGridViewBindingCompleteEventArgs"]/*' />
        public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            this.listChangedType = listChangedType;
        }

        /// <include file='doc\DataGridViewBindingCompleteEventArgs.uex' path='docs/doc[@for="DataGridViewBindingCompleteEventArgs.ListchangedType"]/*' />
        public ListChangedType ListChangedType
        {
            get
            {
                return this.listChangedType;
            }
        }
    }
}
