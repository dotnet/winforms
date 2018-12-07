// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;

    /// <include file='doc\NavigateEvent.uex' path='docs/doc[@for="NavigateEventArgs"]/*' />
    /// <devdoc>
    ///
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class NavigateEventArgs : EventArgs {
        private bool isForward = true;

        /// <include file='doc\NavigateEvent.uex' path='docs/doc[@for="NavigateEventArgs.Forward"]/*' />
        public bool Forward {
            get {
                return isForward;
            }
        }

        /// <include file='doc\NavigateEvent.uex' path='docs/doc[@for="NavigateEventArgs.NavigateEventArgs"]/*' />
        public NavigateEventArgs(bool isForward) {
            this.isForward = isForward;
        }
    }
}
