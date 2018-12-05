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
    ///       Provides data for the <see cref='System.Windows.Forms.StatusBar.OnDrawItem'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class StatusBarDrawItemEventArgs : DrawItemEventArgs {
        readonly StatusBarPanel panel;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarDrawItemEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public StatusBarDrawItemEventArgs(System.Drawing.Graphics g, Font font, Rectangle r, int itemId,
            DrawItemState itemState, StatusBarPanel panel) : base(g, font, r, itemId, itemState) {
            this.panel = panel;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarDrawItemEventArgs'/>
        ///       class using the Forecolor and Backcolor.
        ///    </para>
        /// </devdoc>
        public StatusBarDrawItemEventArgs(System.Drawing.Graphics g, Font font, Rectangle r, int itemId,
            DrawItemState itemState, StatusBarPanel panel, Color foreColor, Color backColor) : base(g, font, r, itemId, itemState, foreColor, backColor) {
            this.panel = panel;
        }

        /// <devdoc>
        ///    <para>
        ///       Specifies the <see cref='System.Windows.Forms.StatusBarPanel'/>
        ///       to
        ///       draw.
        ///    </para>
        /// </devdoc>
        public StatusBarPanel Panel {
            get {
                return panel;
            }
        }

    }
}
