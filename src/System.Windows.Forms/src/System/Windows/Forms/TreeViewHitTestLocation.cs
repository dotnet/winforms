// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

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

        /// <devdoc>
        ///    <para>
        ///       No Information.
        ///    </para>
        /// </devdoc>
        None = NativeMethods.TVHT_NOWHERE,

        /// <devdoc>
        ///    <para>
        ///       On Image.
        ///    </para>
        /// </devdoc>
        Image = NativeMethods.TVHT_ONITEMICON,

        /// <devdoc>
        ///    <para>
        ///       On Label.
        ///    </para>
        /// </devdoc>
        Label = NativeMethods.TVHT_ONITEMLABEL,

        /// <devdoc>
        ///    <para>
        ///       Indent.
        ///    </para>
        /// </devdoc>
        Indent = NativeMethods.TVHT_ONITEMINDENT,

        /// <devdoc>
        ///    <para>
        ///       AboveClientArea.
        ///    </para>
        /// </devdoc>
        AboveClientArea =  NativeMethods.TVHT_ABOVE,

        /// <devdoc>
        ///    <para>
        ///       BelowClientArea.
        ///    </para>
        /// </devdoc>
        BelowClientArea = NativeMethods.TVHT_BELOW,

        /// <devdoc>
        ///    <para>
        ///       LeftOfClientArea.
        ///    </para>
        /// </devdoc>
        LeftOfClientArea = NativeMethods.TVHT_TOLEFT,

        /// <devdoc>
        ///    <para>
        ///       RightOfClientArea.
        ///    </para>
        /// </devdoc>
        RightOfClientArea = NativeMethods.TVHT_TORIGHT,

        /// <devdoc>
        ///    <para>
        ///       RightOfNode.
        ///    </para>
        /// </devdoc>
        RightOfLabel =   NativeMethods.TVHT_ONITEMRIGHT,

        /// <devdoc>
        ///    <para>
        ///       StateImage.
        ///    </para>
        /// </devdoc>
        StateImage = NativeMethods.TVHT_ONITEMSTATEICON,

        /// <devdoc>
        ///    <para>
        ///      PlusMinus.
        ///    </para>
        /// </devdoc>
        PlusMinus = NativeMethods.TVHT_ONITEMBUTTON,
    }
}

