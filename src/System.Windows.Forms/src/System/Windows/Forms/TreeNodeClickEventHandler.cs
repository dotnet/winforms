// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;


    /// <include file='doc\TreeNodeClickEventHandler.uex' path='docs/doc[@for="TreeNodeClickEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the method that will
    ///       handle the <see cref='System.Windows.Forms.TreeView.OnNodeClick'/>, <see cref='System.Windows.Forms.TreeView.OnNodeClick'/>, <see cref='System.Windows.Forms.TreeView.OnNodeClick'/>, or <see cref='System.Windows.Forms.TreeView.BeforeSelect'/> event of a <see cref='System.Windows.Forms.TreeView'/>
    ///       .
    ///       
    ///    </para>
    /// </devdoc>
    public delegate void TreeNodeMouseClickEventHandler(object sender, TreeNodeMouseClickEventArgs e);
}
