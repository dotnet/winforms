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
    ///    Provides data for the <see cref='System.Windows.Forms.Form.OnClosed'/>,
    ///    <see cref='System.Windows.Forms.Form.OnClosed'/>
    ///    event.
    ///
    ///    </para>
    /// </devdoc>
    public class FormClosedEventArgs : EventArgs {
        private CloseReason closeReason;
        
        public FormClosedEventArgs(CloseReason closeReason) {
            this.closeReason = closeReason;                                           
        }

        /// <devdoc>
        ///    <para>
        ///       Provides the reason for the Form Close.
        ///    </para>
        /// </devdoc>
        public CloseReason CloseReason {
            get {
                return closeReason;
            }
        }
    }
}

