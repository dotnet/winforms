// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;


    /// <include file='doc\TreeViewCancelEventHandler.uex' path='docs/doc[@for="TreeViewCancelEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the method that will
    ///       handle the <see cref='System.Windows.Forms.TreeView.OnBeforeCheck'/>, <see cref='System.Windows.Forms.TreeView.OnBeforeCollapse'/>, <see cref='System.Windows.Forms.TreeView.BeforeExpand'/>, or <see cref='System.Windows.Forms.TreeView.BeforeSelect'/> event of a <see cref='System.Windows.Forms.TreeView'/>
    ///       .
    ///       
    ///    </para>
    /// </devdoc>
    public delegate void TreeViewCancelEventHandler(object sender, TreeViewCancelEventArgs e);
}
