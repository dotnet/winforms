// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;


    /// <include file='doc\KeyEventHandler.uex' path='docs/doc[@for="KeyEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a method that will handle the <see cref='System.Windows.Forms.Control.KeyUp'/> or <see cref='System.Windows.Forms.Control.KeyDown'/> event of a <see cref='System.Windows.Forms.Control'/>.
    ///    </para>
    /// </devdoc>
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

}
