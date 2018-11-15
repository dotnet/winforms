// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.OnBeforeLabelEdit'/>
    ///       or <see cref='System.Windows.Forms.TreeView.OnAfterLabelEdit'/> event.
    ///    </para>
    /// </devdoc>
    public class NodeLabelEditEventArgs : EventArgs {
        private readonly string label;
        private readonly TreeNode node;
        private bool cancelEdit = false;

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.NodeLabelEditEventArgs"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public NodeLabelEditEventArgs(TreeNode node) {
            this.node = node;
            this.label = null;
        }

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.NodeLabelEditEventArgs1"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public NodeLabelEditEventArgs(TreeNode node, string label) {
            this.node = node;
            this.label = label;
        }

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.CancelEdit"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool CancelEdit {
            get {
                return cancelEdit;
            }
            set {
                cancelEdit = value;
            }
        }

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.Label"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Label {
            get {
                return label;
            }
        }

        /// <include file='doc\NodeLabelEditEvent.uex' path='docs/doc[@for="NodeLabelEditEventArgs.Node"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TreeNode Node {
            get {
                return node;
            }
        }
    }
}
