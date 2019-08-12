// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='RichTextBox.ContentsResized'/> event.
    /// </summary>
    public class ContentsResizedEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='ContentsResizedEventArgs'/>
        ///  class.
        /// </summary>
        public ContentsResizedEventArgs(Rectangle newRectangle)
        {
            NewRectangle = newRectangle;
        }

        /// <summary>
        ///  Represents the requested size of the <see cref='RichTextBox'/> control.
        /// </summary>
        public Rectangle NewRectangle { get; }
    }
}
