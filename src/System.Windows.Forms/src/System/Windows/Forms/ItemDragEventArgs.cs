// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ItemDragEvent.uex' path='docs/doc[@for="ItemDragEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.ListView.OnItemDrag'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ItemDragEventArgs : EventArgs {
        readonly MouseButtons button;
        readonly object item;

        /// <include file='doc\ItemDragEvent.uex' path='docs/doc[@for="ItemDragEventArgs.ItemDragEventArgs"]/*' />
        public ItemDragEventArgs(MouseButtons button) {
            this.button = button;
            this.item = null;
        }
        
        /// <include file='doc\ItemDragEvent.uex' path='docs/doc[@for="ItemDragEventArgs.ItemDragEventArgs1"]/*' />
        public ItemDragEventArgs(MouseButtons button, object item) {
            this.button = button;
            this.item = item;
        }
        
        /// <include file='doc\ItemDragEvent.uex' path='docs/doc[@for="ItemDragEventArgs.Button"]/*' />
        public MouseButtons Button {
            get { return button; }
        }

        /// <include file='doc\ItemDragEvent.uex' path='docs/doc[@for="ItemDragEventArgs.Item"]/*' />
        public object Item {
            get { return item; }
        }
    }
}
