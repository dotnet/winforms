// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.StatusBar.OnPanelClick'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class StatusBarPanelClickEventArgs : MouseEventArgs {

        readonly StatusBarPanel statusBarPanel;


        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.StatusBarPanelClickEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y)
            : base(button, clicks, x, y, 0) {
            this.statusBarPanel = statusBarPanel;
        }

        /// <devdoc>
        ///    <para>
        ///       Specifies the <see cref='System.Windows.Forms.StatusBarPanel'/> that represents the clicked panel.
        ///    </para>
        /// </devdoc>
        public StatusBarPanel StatusBarPanel {
            get {
                return statusBarPanel;
            }
        }
    }
}
