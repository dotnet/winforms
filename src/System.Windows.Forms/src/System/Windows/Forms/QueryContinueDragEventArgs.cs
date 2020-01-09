// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.QueryContinueDrag'/> event.
    /// </summary>
    [ComVisible(true)]
    public class QueryContinueDragEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='QueryContinueDragEventArgs'/> class.
        /// </summary>
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action)
        {
            KeyState = keyState;
            EscapePressed = escapePressed;
            Action = action;
        }

        /// <summary>
        ///  Gets a value indicating the current state of the SHIFT, CTRL, and ALT keys.
        /// </summary>
        public int KeyState { get; }

        /// <summary>
        ///  Gets a value indicating whether the user pressed the ESC key.
        /// </summary>
        public bool EscapePressed { get; }

        /// <summary>
        ///  Gets or sets the status of a drag-and-drop operation.
        /// </summary>
        public DragAction Action { get; set; }
    }
}
