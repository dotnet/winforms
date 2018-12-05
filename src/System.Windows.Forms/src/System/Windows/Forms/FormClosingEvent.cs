// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///    Provides data for the <see cref='System.Windows.Forms.Form.OnClosing'/>,
    ///    <see cref='System.Windows.Forms.Form.OnClosing'/>
    ///    event.
    ///    </para>
    /// </devdoc>
    public class FormClosingEventArgs : CancelEventArgs {
        private CloseReason closeReason;
        
        public FormClosingEventArgs(CloseReason closeReason, bool cancel)
        : base(cancel) {
            this.closeReason = closeReason;                                           
        }

        /// <devdoc>
        ///    <para>
        ///       Provides the reason for the Form close.
        ///    </para>
        /// </devdoc>
        public CloseReason CloseReason {
            get {
                return closeReason;
            }
        }
    }
}

