// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;

    /// <devdoc>
    ///
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class NavigateEventArgs : EventArgs {
        private bool isForward = true;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool Forward {
            get {
                return isForward;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public NavigateEventArgs(bool isForward) {
            this.isForward = isForward;
        }
    }
}
