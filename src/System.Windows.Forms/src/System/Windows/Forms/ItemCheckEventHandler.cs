// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;


    /// <include file='doc\ItemCheckEventHandler.uex' path='docs/doc[@for="ItemCheckEventHandler"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the method that will
    ///       handle the <see langword='ItemCheck'/> event of a
    ///    <see cref='System.Windows.Forms.CheckedListBox'/> or 
    ///    <see cref='System.Windows.Forms.ListView'/>.
    ///       
    ///    </para>
    /// </devdoc>
    public delegate void ItemCheckEventHandler(object sender, ItemCheckEventArgs e);
}
