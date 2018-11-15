// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for splitter events.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class SplitterEventArgs : EventArgs {

        private readonly int x;
        private readonly int y;
        private int splitX;
        private int splitY;

        /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs.SplitterEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes an instance of the <see cref='System.Windows.Forms.SplitterEventArgs'/> class with the specified coordinates
        ///       of the mouse pointer and the upper-left corner of the <see cref='System.Windows.Forms.Splitter'/>.
        ///    </para>
        /// </devdoc>
        public SplitterEventArgs(int x, int y, int splitX, int splitY) {
            this.x = x;
            this.y = y;
            this.splitX = splitX;
            this.splitY = splitY;
        }

        /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the x-coordinate of the
        ///       mouse pointer (in client coordinates).
        ///    </para>
        /// </devdoc>
        public int X {
            get {
                return x;
            }
        }

        /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of the mouse pointer (in
        ///       client coordinates).
        ///    </para>
        /// </devdoc>
        public int Y {
            get {
                return y;
            }
        }

        /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs.SplitX"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the x-coordinate of the
        ///       upper-left corner of the <see cref='System.Windows.Forms.Splitter'/> (in client coordinates).
        ///    </para>
        /// </devdoc>
        public int SplitX {
            get {
                return splitX;
            }
            set {
                splitX = value;
            }
        }

        /// <include file='doc\SplitterEvent.uex' path='docs/doc[@for="SplitterEventArgs.SplitY"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.Splitter'/> (in client coordinates).
        ///    </para>
        /// </devdoc>
        public int SplitY {
            get {
                return splitY;
            }
            set {
                splitY = value;
            }
        }
    }
}
