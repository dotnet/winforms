// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.KeyPress'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class KeyPressEventArgs : EventArgs {

        /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs.keyChar"]/*' />
        /// <devdoc>
        ///     Contains the character of the current KeyPress event.
        /// </devdoc>
        private char keyChar;

        /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs.handled"]/*' />
        /// <devdoc>
        ///     Determines if this event has been handled by a handler.  If handled, the
        ///     key event will not be sent along to Windows.  If not handled, the event
        ///     will be sent to Windows for default processing.
        /// </devdoc>
        private bool handled;

        /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs.KeyPressEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new
        ///       instance of the <see cref='System.Windows.Forms.KeyPressEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public KeyPressEventArgs(char keyChar) {
            this.keyChar = keyChar;
        }

        /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs.KeyChar"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the character corresponding to the key
        ///       pressed.
        ///    </para>
        /// </devdoc>
        public char KeyChar {
            get {
                return keyChar;
            }
            set {
                keyChar = value;
            }
        }

        /// <include file='doc\KeyPressEvent.uex' path='docs/doc[@for="KeyPressEventArgs.Handled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the <see cref='System.Windows.Forms.Control.KeyPress'/>
        ///       event was handled.
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
