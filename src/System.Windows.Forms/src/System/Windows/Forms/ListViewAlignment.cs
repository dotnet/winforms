// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\ListViewAlignment.uex' path='docs/doc[@for="ListViewAlignment"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how items align in the <see cref='System.Windows.Forms.ListView'/>.
    ///    </para>
    /// </devdoc>
    public enum ListViewAlignment {

        /// <include file='doc\ListViewAlignment.uex' path='docs/doc[@for="ListViewAlignment.Default"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When the user moves an
        ///       item, it remains where it is dropped.
        ///    </para>
        /// </devdoc>
        Default = NativeMethods.LVA_DEFAULT,

        /// <include file='doc\ListViewAlignment.uex' path='docs/doc[@for="ListViewAlignment.Top"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Items are aligned to the top of the <see cref='System.Windows.Forms.ListView'/> control.
        ///    </para>
        /// </devdoc>
        Top = NativeMethods.LVA_ALIGNTOP,

        /// <include file='doc\ListViewAlignment.uex' path='docs/doc[@for="ListViewAlignment.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Items are aligned to the left of the <see cref='System.Windows.Forms.ListView'/> control.
        ///    </para>
        /// </devdoc>
        Left = NativeMethods.LVA_ALIGNLEFT,

        /// <include file='doc\ListViewAlignment.uex' path='docs/doc[@for="ListViewAlignment.SnapToGrid"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Items
        ///       are aligned to an invisible grid in the control.
        ///       When the user moves an item, it moves to the
        ///       closest juncture in the grid.
        ///    </para>
        /// </devdoc>
        SnapToGrid = NativeMethods.LVA_SNAPTOGRID,
    }
}

