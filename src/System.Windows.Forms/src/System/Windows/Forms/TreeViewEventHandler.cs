// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;


    /// <include file='doc\TreeViewEventHandler.uex' path='docs/doc[@for="TreeViewEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the method that will handle the <see cref='System.Windows.Forms.TreeView.OnAfterCheck'/>, <see cref='System.Windows.Forms.TreeView.OnAfterCollapse'/>, <see cref='System.Windows.Forms.TreeView.OnAfterExpand'/>, or <see cref='System.Windows.Forms.TreeView.OnAfterSelect'/>
    ///       event of a <see cref='System.Windows.Forms.TreeView'/>
    ///       .
    ///    </para>
    /// </devdoc>
    public delegate void TreeViewEventHandler(object sender, TreeViewEventArgs e);
}
