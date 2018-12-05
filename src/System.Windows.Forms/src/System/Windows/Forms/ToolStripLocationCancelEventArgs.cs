// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;

    /// <devdoc>
    ///    <para>
    ///       ToolStripLocationCancelEventArgs provides Arguments for the Cancelable LocationChanging Event.
    ///       event.
    ///    </para>
    /// </devdoc>
    internal class ToolStripLocationCancelEventArgs : CancelEventArgs {

        private Point newLocation;
        
        
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the ToolStripLocationCancelEventArgs with cancel value.
        ///    </para>
        /// </devdoc>
        public ToolStripLocationCancelEventArgs(Point newLocation, bool value) : base(value) {
           
            this.newLocation = newLocation;
            
        }

        /// <devdoc>
        ///    <para>
        ///       Returns the New Location of the ToolStrip.
        ///    </para>
        /// </devdoc>
        public Point NewLocation {
            get {
                return this.newLocation;
            }
        }
    }
}

