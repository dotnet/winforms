// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see langword='MouseUp'/>,
    ///    <see langword='MouseDown'/>, and <see langword='MouseMove '/>
    ///    events.
    /// </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class MouseEventArgs : EventArgs {

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.button"]/*' />
        /// <devdoc>
        ///     Which button generated this event [if applicable]
        /// </devdoc>
        private readonly MouseButtons button;

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.clicks"]/*' />
        /// <devdoc>
        ///     If the user has clicked the mouse more than once, this contains the
        ///     count of clicks so far.
        /// </devdoc>
        private readonly int clicks;

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.x"]/*' />
        /// <devdoc>
        ///     The x portion of the coordinate where this event occurred.
        /// </devdoc>
        private readonly int x;

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.y"]/*' />
        /// <devdoc>
        ///     The y portion of the coordinate where this event occurred.
        /// </devdoc>
        private readonly int y;

        private readonly int delta;

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.MouseEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.MouseEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) {
            Debug.Assert((button & (MouseButtons.Left | MouseButtons.None | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)) ==
                         button, "Invalid information passed into MouseEventArgs constructor!");

            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.Button"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets which mouse button was pressed.
        ///    </para>
        /// </devdoc>
        public MouseButtons Button {
            get {
                return button;
            }
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.Clicks"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       number of times the mouse
        ///       button was pressed and released.
        ///    </para>
        /// </devdoc>
        public int Clicks {
            get {
                return clicks;
            }
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.X"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the x-coordinate
        ///       of a mouse click.
        ///    </para>
        /// </devdoc>
        public int X {
            get {
                return x;
            }
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.Y"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the y-coordinate of a mouse click.
        ///    </para>
        /// </devdoc>
        public int Y {
            get {
                return y;
            }
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.Delta"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       a signed count of the number of detents the mouse wheel has rotated.
        ///    </para>
        /// </devdoc>
        public int Delta {
            get {
                return delta;
            }
        }

        /// <include file='doc\MouseEvent.uex' path='docs/doc[@for="MouseEventArgs.Location"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the location of the mouse during MouseEvent.
        ///    </para>
        /// </devdoc>
        public Point Location {
            get {
                return new Point(x,y);
            }
        }
    }
}
