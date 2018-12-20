// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\TreeNodeMouseHoverEvent.uex' path='docs/doc[@for="TreeNodeMouseHoverEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.OnNodeMouseHover'/> event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class TreeNodeMouseHoverEventArgs : EventArgs {
        readonly TreeNode node;

        /// <include file='doc\TreeNodeMouseHoverEvent.uex' path='docs/doc[@for="TreeNodeMouseHoverEventArgs.TreeNodeMouseHoverEventArgs"]/*' />
        public TreeNodeMouseHoverEventArgs(TreeNode node) {
            this.node = node;
        }
        
        /// <include file='doc\TreeNodeMouseHoverEvent.uex' path='docs/doc[@for="TreeNodeMouseHoverEventArgs.Node"]/*' />
        public TreeNode Node {
            get { return node; }
        }
    }
}
