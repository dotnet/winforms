// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\DragAction.uex' path='docs/doc[@for="DragAction"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how and if a drag-and-drop operation should continue.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum DragAction {
        /// <include file='doc\DragAction.uex' path='docs/doc[@for="DragAction.Continue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The operation will continue.
        ///    </para>
        /// </devdoc>
        Continue = 0,
        /// <include file='doc\DragAction.uex' path='docs/doc[@for="DragAction.Drop"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The operation will stop with a drop.
        ///    </para>
        /// </devdoc>
        Drop = 1,
        /// <include file='doc\DragAction.uex' path='docs/doc[@for="DragAction.Cancel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The operation is canceled with no
        ///       drop message.
        ///       
        ///    </para>
        /// </devdoc>
        Cancel = 2,

    }
}
