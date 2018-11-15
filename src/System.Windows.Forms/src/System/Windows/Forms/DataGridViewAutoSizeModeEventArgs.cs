// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewAutoSizeModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeModeEventArgs"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class DataGridViewAutoSizeModeEventArgs : EventArgs
    {
        private bool previousModeAutoSized;
    
        /// <include file='doc\DataGridViewAutoSizeModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeModeEventArgs.DataGridViewAutoSizeModeEventArgs"]/*' />
        public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
        {
            this.previousModeAutoSized = previousModeAutoSized;
        }

        /// <include file='doc\DataGridViewAutoSizeModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeModeEventArgs.PreviousModeAutoSized"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool PreviousModeAutoSized
        {
            get
            {
                return this.previousModeAutoSized;
            }
        }
    }
}
