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
    ///       Provides data for the <see cref='System.Windows.Forms.Control.QueryContinueDrag'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class QueryContinueDragEventArgs : EventArgs {

        private readonly int keyState;
        private readonly bool escapePressed;
        private DragAction action;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.QueryContinueDragEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action) {
            this.keyState = keyState;
            this.escapePressed = escapePressed;
            this.action = action;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating
        ///       the current state of the SHIFT, CTRL, and ALT keys.
        ///  </para>
        /// </devdoc>
        public int KeyState {
            get {
                return keyState;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the user pressed the ESC key.
        ///    </para>
        /// </devdoc>
        public bool EscapePressed {
            get {
                return escapePressed;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets the status of a drag-and-drop operation.
        ///    </para>
        /// </devdoc>
        public DragAction Action {
            get {
                return action;
            }
            set {
                action = value;
            }
        }
    }
}
