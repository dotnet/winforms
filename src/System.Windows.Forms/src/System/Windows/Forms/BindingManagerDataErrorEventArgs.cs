// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\BindingManagerDataErrorEventArgs.uex' path='docs/doc[@for="BindingManagerDataErrorEventArgs"]/*' />
    /// <devdoc>
    /// </devdoc>
    public class BindingManagerDataErrorEventArgs : EventArgs {
        private	Exception exception;

        /// <include file='doc\BindingManagerDataErrorEventArgs.uex' path='docs/doc[@for="BindingManagerDataErrorEventArgs.BindingManagerDataErrorEventArgs"]/*' />
        /// <devdoc>
        /// </devdoc>
        public BindingManagerDataErrorEventArgs(Exception exception) {
            this.exception = exception;
        }

        /// <include file='doc\BindingManagerDataErrorEventArgs.uex' path='docs/doc[@for="BindingManagerDataErrorEventArgs.Exception"]/*' />
        /// <devdoc>
        /// </devdoc>
        public Exception Exception
        {
            get {
                return this.exception;
            }
        }
    }
}
