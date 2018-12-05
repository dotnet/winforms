// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.RichTextBox.ContentsResized'/> event.
    ///    </para>
    /// </devdoc>
    public class ContentsResizedEventArgs : EventArgs {
        readonly Rectangle newRectangle;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ContentsResizedEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>

        public ContentsResizedEventArgs(Rectangle newRectangle) {
            this.newRectangle = newRectangle;
        }
        
        /// <devdoc>
        ///    <para>
        ///       Represents the requested size of the <see cref='System.Windows.Forms.RichTextBox'/> control.
        ///    </para>
        /// </devdoc>
        public Rectangle NewRectangle {
            get {
                return newRectangle;
            }
        }
    }
}
