// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms
{

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <summary>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.TreeView.OnAfterCheck'/>, <see cref='System.Windows.Forms.TreeView.AfterCollapse'/>, <see cref='System.Windows.Forms.TreeView.AfterExpand'/>, or <see cref='System.Windows.Forms.TreeView.AfterSelect'/> event.
    ///    </para>
    /// </summary>
    public class TreeViewEventArgs : EventArgs
    {
        TreeNode node;
        TreeViewAction action = TreeViewAction.Unknown;

        public TreeViewEventArgs(TreeNode node)
        {
            this.node = node;
        }

        public TreeViewEventArgs(TreeNode node, TreeViewAction action)
        {
            this.node = node;
            this.action = action;
        }

        public TreeNode Node
        {
            get
            {
                return node;
            }
        }

        /// <summary>
        ///      An event specific action-flag.
        /// </summary>
        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }
    }
}
