// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides Arguments for the Cancelable LocationChanging Event.
    /// </devdoc>
    internal class ToolStripLocationCancelEventArgs : CancelEventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the ToolStripLocationCancelEventArgs with cancel value.
        /// </devdoc>
        public ToolStripLocationCancelEventArgs(Point newLocation, bool value) : base(value)
        {
            NewLocation = newLocation;
        }

        /// <devdoc>
        /// Returns the New Location of the ToolStrip.
        /// </devdoc>
        public Point NewLocation { get; }
    }
}
