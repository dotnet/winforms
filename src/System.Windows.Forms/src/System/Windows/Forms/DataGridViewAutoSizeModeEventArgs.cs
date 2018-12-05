// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class DataGridViewAutoSizeModeEventArgs : EventArgs
    {
        private bool previousModeAutoSized;
    
        public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
        {
            this.previousModeAutoSized = previousModeAutoSized;
        }

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
