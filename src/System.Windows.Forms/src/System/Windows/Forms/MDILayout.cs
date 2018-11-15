// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\MDILayout.uex' path='docs/doc[@for="MdiLayout"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the layout of multiple document interface (MDI) child windows in an MDI parent window.
    ///    </para>
    /// </devdoc>
    public enum MdiLayout {

        /// <include file='doc\MDILayout.uex' path='docs/doc[@for="MdiLayout.Cascade"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All MDI child windows are cascaded within the client
        ///       region of
        ///       the MDI parent form.
        ///    </para>
        /// </devdoc>
        Cascade         = 0,
        /// <include file='doc\MDILayout.uex' path='docs/doc[@for="MdiLayout.TileHorizontal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All MDI child windows are tiled horizontally within the client region of the MDI parent form.
        ///    </para>
        /// </devdoc>
        TileHorizontal = 1,
        /// <include file='doc\MDILayout.uex' path='docs/doc[@for="MdiLayout.TileVertical"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All MDI child windows are tiled vertically within the client region of the MDI parent form.
        ///    </para>
        /// </devdoc>
        TileVertical   = 2,
        /// <include file='doc\MDILayout.uex' path='docs/doc[@for="MdiLayout.ArrangeIcons"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All MDI child icons are arranged within the client region of the MDI parent form.
        ///       An application sets this layout to arrange all minimized MDI child windows (in the bottom of the client area). 
        ///       It does not affect child windows that are not minimized. 
        ///    </para>
        /// </devdoc>
        ArrangeIcons   = 3,

    }
}

