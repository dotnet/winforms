// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ListView.OnItemMouseHover'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ListViewItemMouseHoverEventArgs : EventArgs {
        readonly ListViewItem item;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ListViewItemMouseHoverEventArgs(ListViewItem item) {
            this.item = item;
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ListViewItem Item {
            get { return item; }
        }
    }
}
