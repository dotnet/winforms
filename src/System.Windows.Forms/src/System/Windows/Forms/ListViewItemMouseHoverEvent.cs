// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\ListViewMouseHoverEvent.uex' path='docs/doc[@for="ListViewMouseHoverEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ListView.OnItemMouseHover'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ListViewItemMouseHoverEventArgs : EventArgs {
        readonly ListViewItem item;

        /// <include file='doc\ItemMouseHoverEvent.uex' path='docs/doc[@for="ListViewItemMouseHoverEventArgs.ItemMouseHoverEventArgs"]/*' />
        public ListViewItemMouseHoverEventArgs(ListViewItem item) {
            this.item = item;
        }
        
        /// <include file='doc\ItemMouseHoverEvent.uex' path='docs/doc[@for="ListViewItemMouseHoverEventArgs.Item"]/*' />
        public ListViewItem Item {
            get { return item; }
        }
    }
}
