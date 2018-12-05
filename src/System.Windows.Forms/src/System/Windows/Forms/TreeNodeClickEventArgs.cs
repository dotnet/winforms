// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TreeNodeMouseClickEventArgs.uex' path='docs/doc[@for="TreeNodeMouseClickEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.TreeNodeMouseClickEventArgs'/>
    ///       or <see cref='System.Windows.Forms.TreeView.OnNodeMouseClick'/> event.
    ///    </para>
    /// </devdoc>
    public class TreeNodeMouseClickEventArgs : MouseEventArgs {
        
        private TreeNode node;
        

        /// <include file='doc\TreeNodeClickEventArgs.uex' path='docs/doc[@for="TreeNodeClickEventArgs.TreeNodeClickEventArgs"]/*' />
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y)
            : base(button, clicks, x, y, 0) {
            this.node = node;
        }

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.Node"]/*' />
        public TreeNode Node {
            get {
                return node;
            }
        }
    }
}
