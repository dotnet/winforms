// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\MenuMerge.uex' path='docs/doc[@for="MenuMerge"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the behavior of a <see cref='System.Windows.Forms.MenuItem'/> when it is merged with items in another menu.
    ///    </para>
    /// </devdoc>
    public enum MenuMerge {

        /// <include file='doc\MenuMerge.uex' path='docs/doc[@for="MenuMerge.Add"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The <see cref='System.Windows.Forms.MenuItem'/> is added to
        ///       the existing <see cref='System.Windows.Forms.MenuItem'/> objects in a merged menu.
        ///    </para>
        /// </devdoc>
        Add        = 0,

        /// <include file='doc\MenuMerge.uex' path='docs/doc[@for="MenuMerge.Replace"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The <see cref='System.Windows.Forms.MenuItem'/> replaces the
        ///       existing <see cref='System.Windows.Forms.MenuItem'/>
        ///       at the same position in a
        ///       merged menu.
        ///
        ///    </para>
        /// </devdoc>
        Replace    = 1,

        /// <include file='doc\MenuMerge.uex' path='docs/doc[@for="MenuMerge.MergeItems"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Subitems of this <see cref='System.Windows.Forms.MenuItem'/> are merged with
        ///       those of existing <see cref='System.Windows.Forms.MenuItem'/> objects
        ///       at the same position in a merged menu.
        ///    </para>
        /// </devdoc>
        MergeItems = 2,

        /// <include file='doc\MenuMerge.uex' path='docs/doc[@for="MenuMerge.Remove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The <see cref='System.Windows.Forms.MenuItem'/> is not included in a merged menu.
        ///    </para>
        /// </devdoc>
        Remove     = 3,

    }
}
