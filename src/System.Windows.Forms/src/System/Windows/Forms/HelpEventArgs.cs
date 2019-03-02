// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the Control.HelpRequest event.
    /// </devdoc>
    [ComVisible(true)]
    public class HelpEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.HelpEventArgs'/>class.
        /// </devdoc>
        public HelpEventArgs(Point mousePos)
        {
            MousePos = mousePos;
        }

        /// <devdoc>
        /// Gets the screen coordinates of the mouse pointer.
        /// </devdoc>
        public Point MousePos { get; }

        /// <devdoc>
        /// Gets or sets a value indicating whether the Help event was handled.
        /// </devdoc>
        public bool Handled { get; set; }
    }
}
