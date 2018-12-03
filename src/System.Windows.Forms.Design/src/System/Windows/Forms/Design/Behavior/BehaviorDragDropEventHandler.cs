// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     Delegate used by the BehaviorService to send BeginDrag and EndDrag
    ///     events.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public delegate void BehaviorDragDropEventHandler(object sender, BehaviorDragDropEventArgs e);
}
