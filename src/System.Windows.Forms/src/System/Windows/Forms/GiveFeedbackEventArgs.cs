// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.GiveFeedback'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class GiveFeedbackEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.GiveFeedbackEventArgs'/> class.
        /// </devdoc>
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
        {
            Effect = effect;
            UseDefaultCursors = useDefaultCursors;
        }

        /// <devdoc>
        /// Gets the type of drag-and-drop operation.
        /// </devdoc>
        public DragDropEffects Effect { get; }

        /// <devdoc>
        /// Gets or sets a value indicating whether a default pointer is used.
        /// </devdoc>
        public bool UseDefaultCursors { get; set; }
    }
}
