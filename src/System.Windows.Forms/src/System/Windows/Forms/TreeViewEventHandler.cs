// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a method that will handle the <see cref='TreeView.OnAfterCheck'/>,
    ///  <see cref='TreeView.OnAfterCollapse'/>, <see cref='TreeView.OnAfterExpand'/>,
    ///  or <see cref='TreeView.OnAfterSelect'/> event.
    /// </summary>
    public delegate void TreeViewEventHandler(object sender, TreeViewEventArgs e);
}
