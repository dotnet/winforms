// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for splitter events.
    ///    </para>
    /// </devdoc>
    public class SplitterCancelEventArgs : CancelEventArgs {

        private readonly int mouseCursorX;
        private readonly int mouseCursorY;
        private int splitX;
        private int splitY;

        /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs.SplitterCancelEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes an instance of the <see cref='System.Windows.Forms.SplitterCancelEventArgs'/> class with the specified coordinates
        ///       of the mouse pointer and the upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/>.
        ///    </para>
        /// </devdoc>
        public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY) 
        : base (false) {
            this.mouseCursorX = mouseCursorX;
            this.mouseCursorY = mouseCursorY;
            this.splitX = splitX;
            this.splitY = splitY;
        }

        /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the x-coordinate of the
        ///       mouse pointer (in client coordinates).
        ///    </para>
        /// </devdoc>
        public int MouseCursorX {
            get {
                return mouseCursorX;
            }
        }

        /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of the mouse pointer (in
        ///       client coordinates).
        ///    </para>
        /// </devdoc>
        public int MouseCursorY {
            get {
                return mouseCursorY;
            }
        }

        /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs.SplitX"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the x-coordinate of the
        ///       upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/> (in client coordinates).
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

        /// <include file='doc\SplitterCancelEvent.uex' path='docs/doc[@for="SplitterCancelEventArgs.SplitY"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/> (in client coordinates).
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

