// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.DragDrop'/>, <see cref='System.Windows.Forms.Control.DragEnter'/>,
    /// or <see cref='System.Windows.Forms.Control.DragOver'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class DragEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.DragEventArgs'/> class.
        /// </devdoc>
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect) {
            Data = data;
            KeyState = keyState;
            X = x;
            Y = y;
            AllowedEffect = allowedEffect;
            Effect = effect;
        }

        /// <devdoc>
        /// The <see cref='System.Windows.Forms.IDataObject'/> that contains the data associated
        /// with this event.
        /// </devdoc>
        public IDataObject Data { get; }

        /// <devdoc>
        /// Gets the current state of the SHIFT, CTRL, and ALT keys.
        /// </devdoc>
        public int KeyState { get; }

        /// <devdoc>
        /// Gets the x-coordinate of the mouse pointer.
        /// </devdoc>
        public int X { get; }

        /// <devdoc>
        /// Gets the y-coordinate of the mouse pointer.
        /// </devdoc>
        public int Y { get; }

        /// <devdoc>
        /// Gets which drag-and-drop operations are allowed by the originator (or source)
        /// of the drag event.
        /// </devdoc>
        public DragDropEffects AllowedEffect { get; }

        /// <devdoc>
        /// Gets or sets which drag-and-drop operations are allowed by the target of the drag event.
        /// </devdoc>
        public DragDropEffects Effect { get; set; }
    }
}
