// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <include file='doc\ToolBarButtonClickEvent.uex' path='docs/doc[@for="ToolBarButtonClickEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ToolBar.ButtonClick'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class ToolBarButtonClickEventArgs : EventArgs {

        private ToolBarButton button;

        /// <include file='doc\ToolBarButtonClickEvent.uex' path='docs/doc[@for="ToolBarButtonClickEventArgs.ToolBarButtonClickEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ToolBarButtonClickEventArgs'/>
        ///       class.
        ///    </para>
        /// </devdoc>
        public ToolBarButtonClickEventArgs(ToolBarButton button) {
            this.button = button;
        }

        /// <include file='doc\ToolBarButtonClickEvent.uex' path='docs/doc[@for="ToolBarButtonClickEventArgs.Button"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the <see cref='System.Windows.Forms.ToolBarButton'/>
        ///       that was clicked.
        ///    </para>
        /// </devdoc>
        public ToolBarButton Button {
            get { return button;}
            set { button = value;}
        }
    }
}
