// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;    
    using Microsoft.Win32;


    /// <include file='doc\HelpEvent.uex' path='docs/doc[@for="HelpEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the Control.HelpRequest event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class HelpEventArgs : EventArgs {
        private readonly Point mousePos;
        private bool           handled = false;

        /// <include file='doc\HelpEvent.uex' path='docs/doc[@for="HelpEventArgs.HelpEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.HelpEventArgs'/>class.
        ///    </para>
        /// </devdoc>
        public HelpEventArgs(Point mousePos) {
            this.mousePos = mousePos;
        }

        /// <include file='doc\HelpEvent.uex' path='docs/doc[@for="HelpEventArgs.MousePos"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the screen coordinates of the mouse pointer.
        ///    </para>
        /// </devdoc>
        public Point MousePos {
            get {
                return mousePos;
            }
        }

        /// <include file='doc\HelpEvent.uex' path='docs/doc[@for="HelpEventArgs.Handled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets a value indicating
        ///       whether the Help event was handled.
        ///    </para>
        /// </devdoc>
        public bool Handled {
            get {
                return handled;
            }
            set {
                handled = value;
            }
        }
    }
}
