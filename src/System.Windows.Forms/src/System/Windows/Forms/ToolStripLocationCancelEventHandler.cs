// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;

    /// <include file='doc\ToolStripLocationCancelEventHandler.uex' path='docs/doc[@for="ToolStripLocationCancelEventHandler"]/*' />
    /// <devdoc>
    ///    <para>Represents the method that will handle the event raised when canceling an
    ///       OnLocationChanging event for ToolStrips.</para>
    /// </devdoc>
    internal delegate void ToolStripLocationCancelEventHandler(object sender, ToolStripLocationCancelEventArgs e);
}

