// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.DragDrop'/>, <see cref='Control.DragEnter'/>,
    ///  or <see cref='Control.DragOver'/> event.
    /// </summary>
    public class DragEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='DragEventArgs'/> class.
        /// </summary>
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            Data = data;
            KeyState = keyState;
            X = x;
            Y = y;
            AllowedEffect = allowedEffect;
            Effect = effect;
        }

        /// <summary>
        ///  The <see cref='IDataObject'/> that contains the data associated
        ///  with this event.
        /// </summary>
        public IDataObject Data { get; }

        /// <summary>
        ///  Gets the current state of the SHIFT, CTRL, and ALT keys.
        /// </summary>
        public int KeyState { get; }

        /// <summary>
        ///  Gets the x-coordinate of the mouse pointer.
        /// </summary>
        public int X { get; }

        /// <summary>
        ///  Gets the y-coordinate of the mouse pointer.
        /// </summary>
        public int Y { get; }

        /// <summary>
        ///  Gets which drag-and-drop operations are allowed by the originator (or source)
        ///  of the drag event.
        /// </summary>
        public DragDropEffects AllowedEffect { get; }

        /// <summary>
        ///  Gets or sets which drag-and-drop operations are allowed by the target of the drag event.
        /// </summary>
        public DragDropEffects Effect { get; set; }
    }
}
