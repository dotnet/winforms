// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.OnBeforeCheck'/>,
    ///    <see cref='System.Windows.Forms.TreeView.OnBeforeCollapse'/>,
    ///    <see cref='System.Windows.Forms.TreeView.OnBeforeExpand'/>,
    ///       or <see cref='System.Windows.Forms.TreeView.OnBeforeSelect'/> event.
    ///
    ///    </para>
    /// </devdoc>
    public class TreeViewCancelEventArgs : CancelEventArgs {
        private TreeNode node;
        private TreeViewAction action;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action)
        : base(cancel) {
            this.node = node;                                           
            this.action = action;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TreeNode Node {
            get {
                return node;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TreeViewAction Action {
            get {
                return action;
            }
        }
    }
}
