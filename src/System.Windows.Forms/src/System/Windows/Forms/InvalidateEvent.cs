// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.Invalidate'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class InvalidateEventArgs : EventArgs {

        /// <devdoc>
        ///     Rectangle that bounds the window area which has been invalidated.
        /// </devdoc>
        private readonly Rectangle invalidRect;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.InvalidateEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public InvalidateEventArgs(Rectangle invalidRect) {
            this.invalidRect = invalidRect;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating the <see cref='System.Drawing.Rectangle'/>
        ///       that contains the invalidated window area.
        ///    </para>
        /// </devdoc>
        public Rectangle InvalidRect {
            get {
                return invalidRect;
            }
        }
    }
}
