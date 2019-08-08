// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the return value for HITTEST on treeview.
    /// </summary>
    [Flags]
    [ComVisible(true)]
    public enum TreeViewHitTestLocations
    {
        /// <summary>
        ///  No Information.
        /// </summary>
        None = NativeMethods.TVHT_NOWHERE,

        /// <summary>
        ///  On Image.
        /// </summary>
        Image = NativeMethods.TVHT_ONITEMICON,

        /// <summary>
        ///  On Label.
        /// </summary>
        Label = NativeMethods.TVHT_ONITEMLABEL,

        /// <summary>
        ///  Indent.
        /// </summary>
        Indent = NativeMethods.TVHT_ONITEMINDENT,

        /// <summary>
        ///  AboveClientArea.
        /// </summary>
        AboveClientArea = NativeMethods.TVHT_ABOVE,

        /// <summary>
        ///  BelowClientArea.
        /// </summary>
        BelowClientArea = NativeMethods.TVHT_BELOW,

        /// <summary>
        ///  LeftOfClientArea.
        /// </summary>
        LeftOfClientArea = NativeMethods.TVHT_TOLEFT,

        /// <summary>
        ///  RightOfClientArea.
        /// </summary>
        RightOfClientArea = NativeMethods.TVHT_TORIGHT,

        /// <summary>
        ///  RightOfNode.
        /// </summary>
        RightOfLabel = NativeMethods.TVHT_ONITEMRIGHT,

        /// <summary>
        ///  StateImage.
        /// </summary>
        StateImage = NativeMethods.TVHT_ONITEMSTATEICON,

        /// <summary>
        ///  PlusMinus.
        /// </summary>
        PlusMinus = NativeMethods.TVHT_ONITEMBUTTON,
    }
}
