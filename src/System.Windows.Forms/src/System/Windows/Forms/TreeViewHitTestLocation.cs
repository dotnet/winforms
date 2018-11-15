// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the
    ///       return value for HITTEST on treeview.
    ///    </para>
    /// </devdoc>
    [
    Flags,
    System.Runtime.InteropServices.ComVisible(true)
    ]
    public enum TreeViewHitTestLocations {

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No Information.
        ///    </para>
        /// </devdoc>
        None = NativeMethods.TVHT_NOWHERE,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.Image"]/*' />
        /// <devdoc>
        ///    <para>
        ///       On Image.
        ///    </para>
        /// </devdoc>
        Image = NativeMethods.TVHT_ONITEMICON,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.Label"]/*' />
        /// <devdoc>
        ///    <para>
        ///       On Label.
        ///    </para>
        /// </devdoc>
        Label = NativeMethods.TVHT_ONITEMLABEL,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.Indent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indent.
        ///    </para>
        /// </devdoc>
        Indent = NativeMethods.TVHT_ONITEMINDENT,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.AboveClientArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       AboveClientArea.
        ///    </para>
        /// </devdoc>
        AboveClientArea =  NativeMethods.TVHT_ABOVE,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.BelowClientArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       BelowClientArea.
        ///    </para>
        /// </devdoc>
        BelowClientArea = NativeMethods.TVHT_BELOW,

        /// <include file='doc\TreeViewHitTestInfo.uex' path='docs/doc[@for="TreeViewHitTestInfo.LeftOfClientArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       LeftOfClientArea.
        ///    </para>
        /// </devdoc>
        LeftOfClientArea = NativeMethods.TVHT_TOLEFT,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.RightOfClientArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       RightOfClientArea.
        ///    </para>
        /// </devdoc>
        RightOfClientArea = NativeMethods.TVHT_TORIGHT,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.RightOfNode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       RightOfNode.
        ///    </para>
        /// </devdoc>
        RightOfLabel =   NativeMethods.TVHT_ONITEMRIGHT,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.StateImage"]/*' />
        /// <devdoc>
        ///    <para>
        ///       StateImage.
        ///    </para>
        /// </devdoc>
        StateImage = NativeMethods.TVHT_ONITEMSTATEICON,

        /// <include file='doc\TreeViewHitTestLocations.uex' path='docs/doc[@for="TreeViewHitTestLocations.PlusMinus"]/*' />
        /// <devdoc>
        ///    <para>
        ///      PlusMinus.
        ///    </para>
        /// </devdoc>
        PlusMinus = NativeMethods.TVHT_ONITEMBUTTON,
    }
}

