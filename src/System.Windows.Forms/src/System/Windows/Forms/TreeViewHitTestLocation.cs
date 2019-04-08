// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the return value for HITTEST on treeview.
    /// </devdoc>
    [Flags]
    [ComVisible(true)]
    public enum TreeViewHitTestLocations
    {
        /// <devdoc>
        /// No Information.
        /// </devdoc>
        None = NativeMethods.TVHT_NOWHERE,

        /// <devdoc>
        /// On Image.
        /// </devdoc>
        Image = NativeMethods.TVHT_ONITEMICON,

        /// <devdoc>
        /// On Label.
        /// </devdoc>
        Label = NativeMethods.TVHT_ONITEMLABEL,

        /// <devdoc>
        /// Indent.
        /// </devdoc>
        Indent = NativeMethods.TVHT_ONITEMINDENT,

        /// <devdoc>
        /// AboveClientArea.
        /// </devdoc>
        AboveClientArea =  NativeMethods.TVHT_ABOVE,

        /// <devdoc>
        /// BelowClientArea.
        /// </devdoc>
        BelowClientArea = NativeMethods.TVHT_BELOW,

        /// <devdoc>
        /// LeftOfClientArea.
        /// </devdoc>
        LeftOfClientArea = NativeMethods.TVHT_TOLEFT,

        /// <devdoc>
        /// RightOfClientArea.
        /// </devdoc>
        RightOfClientArea = NativeMethods.TVHT_TORIGHT,

        /// <devdoc>
        /// RightOfNode.
        /// </devdoc>
        RightOfLabel =   NativeMethods.TVHT_ONITEMRIGHT,

        /// <devdoc>
        /// StateImage.
        /// </devdoc>
        StateImage = NativeMethods.TVHT_ONITEMSTATEICON,

        /// <devdoc>
        ///      PlusMinus.
        /// </devdoc>
        PlusMinus = NativeMethods.TVHT_ONITEMBUTTON,
    }
}
