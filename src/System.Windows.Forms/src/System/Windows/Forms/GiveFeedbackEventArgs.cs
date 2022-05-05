// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref="Control.GiveFeedback"/> event.
    /// </summary>
    public class GiveFeedbackEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="GiveFeedbackEventArgs"/> class.
        /// </summary>
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors) : this(effect, useDefaultCursors, dragImage: default!, cursorOffset: default, useDefaultDragImage: false)
        {
        }

        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors, Bitmap dragImage, Point cursorOffset, bool useDefaultDragImage)
        {
            Effect = effect;
            UseDefaultCursors = useDefaultCursors;
            DragImage = dragImage;
            CursorOffset = cursorOffset;
            UseDefaultDragImage = useDefaultDragImage;
        }

        /// <summary>
        ///  Gets the type of drag-and-drop operation.
        /// </summary>
        public DragDropEffects Effect { get; }

        /// <summary>
        ///  Gets or sets a value indicating whether a default pointer is used.
        /// </summary>
        public bool UseDefaultCursors { get; set; }

        /// <summary>
        ///  Gets or sets the drag image bitmap.
        /// </summary>
        public Bitmap DragImage { get; set; }

        /// <summary>
        ///  Gets or sets the drag image cursor offset.
        /// </summary>
        public Point CursorOffset { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether a layered window drag image is used.
        /// </summary>
        public bool UseDefaultDragImage { get; set; }
    }
}
