// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.QueryContinueDrag'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class QueryContinueDragEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.QueryContinueDragEventArgs'/> class.
        /// </devdoc>
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action)
        {
            KeyState = keyState;
            EscapePressed = escapePressed;
            Action = action;
        }

        /// <devdoc>
        /// Gets a value indicating the current state of the SHIFT, CTRL, and ALT keys.
        /// </devdoc>
        public int KeyState { get; }

        /// <devdoc>
        /// Gets a value indicating whether the user pressed the ESC key.
        /// </devdoc>
        public bool EscapePressed { get; }

        /// <devdoc>
        /// Gets or sets the status of a drag-and-drop operation.
        /// </devdoc>
        public DragAction Action { get; set; }
    }
}
