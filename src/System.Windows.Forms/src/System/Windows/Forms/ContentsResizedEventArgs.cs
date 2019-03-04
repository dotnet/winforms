// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.RichTextBox.ContentsResized'/> event.
    /// </devdoc>
    public class ContentsResizedEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.ContentsResizedEventArgs'/>
        /// class.
        /// </devdoc>
        public ContentsResizedEventArgs(Rectangle newRectangle)
        {
            NewRectangle = newRectangle;
        }
        
        /// <devdoc>
        /// Represents the requested size of the <see cref='System.Windows.Forms.RichTextBox'/> control.
        /// </devdoc>
        public Rectangle NewRectangle { get; }
    }
}
