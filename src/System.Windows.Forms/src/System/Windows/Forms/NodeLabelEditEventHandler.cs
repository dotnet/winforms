// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;


    /// <include file='doc\NodeLabelEditEventHandler.uex' path='docs/doc[@for="NodeLabelEditEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the method that will handle the <see cref='System.Windows.Forms.TreeView.OnBeforeLabelEdit'/> or <see cref='System.Windows.Forms.TreeView.OnAfterLabelEdit'/>
    ///       
    ///       event of a <see cref='System.Windows.Forms.TreeView'/>.
    ///       
    ///    </para>
    /// </devdoc>
    public delegate void NodeLabelEditEventHandler(object sender, NodeLabelEditEventArgs e);
}
