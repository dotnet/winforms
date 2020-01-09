// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.Invalidate'/> event.
    /// </summary>
    public class InvalidateEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='InvalidateEventArgs'/>
        ///  class.
        /// </summary>
        public InvalidateEventArgs(Rectangle invalidRect)
        {
            InvalidRect = invalidRect;
        }

        /// <summary>
        ///  Gets a value indicating the <see cref='Rectangle'/> that contains the
        ///  invalidated window area.
        /// </summary>
        public Rectangle InvalidRect { get; }
    }
}
