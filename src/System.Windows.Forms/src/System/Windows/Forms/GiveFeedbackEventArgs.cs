// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.GiveFeedback'/> event.
    /// </summary>
    public class GiveFeedbackEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='GiveFeedbackEventArgs'/> class.
        /// </summary>
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
        {
            Effect = effect;
            UseDefaultCursors = useDefaultCursors;
        }

        /// <summary>
        ///  Gets the type of drag-and-drop operation.
        /// </summary>
        public DragDropEffects Effect { get; }

        /// <summary>
        ///  Gets or sets a value indicating whether a default pointer is used.
        /// </summary>
        public bool UseDefaultCursors { get; set; }
    }
}
