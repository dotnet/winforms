// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.KeyPress'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class KeyPressEventArgs : EventArgs {

        /// <devdoc>
        ///     Contains the character of the current KeyPress event.
        /// </devdoc>
        private char keyChar;

        /// <devdoc>
        ///     Determines if this event has been handled by a handler.  If handled, the
        ///     key event will not be sent along to Windows.  If not handled, the event
        ///     will be sent to Windows for default processing.
        /// </devdoc>
        private bool handled;

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
