// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TreeViewEvent.uex' path='docs/doc[@for="TreeViewEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.OnAfterCheck'/>, <see cref='System.Windows.Forms.TreeView.AfterCollapse'/>, <see cref='System.Windows.Forms.TreeView.AfterExpand'/>, or <see cref='System.Windows.Forms.TreeView.AfterSelect'/> event.
    ///    </para>
    /// </devdoc>
    public class TreeViewEventArgs : EventArgs {
        TreeNode node;
        TreeViewAction action = TreeViewAction.Unknown;
        
        /// <include file='doc\TreeViewEvent.uex' path='docs/doc[@for="TreeViewEventArgs.TreeViewEventArgs"]/*' />
        public TreeViewEventArgs(TreeNode node) {
            this.node = node;
        }
        
        /// <include file='doc\TreeViewEvent.uex' path='docs/doc[@for="TreeViewEventArgs.TreeViewEventArgs1"]/*' />
        public TreeViewEventArgs(TreeNode node, TreeViewAction action) {
            this.node = node;                                           
            this.action = action;
        }
        
        /// <include file='doc\TreeViewEvent.uex' path='docs/doc[@for="TreeViewEventArgs.Node"]/*' />
        public TreeNode Node {
            get {
                return node;
            }
        }

        /// <include file='doc\TreeViewEvent.uex' path='docs/doc[@for="TreeViewEventArgs.Action"]/*' />
        /// <devdoc>
        ///      An event specific action-flag.
        /// </devdoc>
        public TreeViewAction Action {
            get {
                return action;
            }
        }
    }
}
