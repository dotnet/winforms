// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the return value for HITTEST on treeview.
    /// </summary>
    [Flags]
    public enum TreeViewHitTestLocations
    {
        /// <summary>
        ///  No Information.
        /// </summary>
        None = (int)ComCtl32.TVHT.NOWHERE,

        /// <summary>
        ///  On Image.
        /// </summary>
        Image = (int)ComCtl32.TVHT.ONITEMICON,

        /// <summary>
        ///  On Label.
        /// </summary>
        Label = (int)ComCtl32.TVHT.ONITEMLABEL,

        /// <summary>
        ///  Indent.
        /// </summary>
        Indent = (int)ComCtl32.TVHT.ONITEMINDENT,

        /// <summary>
        ///  AboveClientArea.
        /// </summary>
        AboveClientArea = (int)ComCtl32.TVHT.ABOVE,

        /// <summary>
        ///  BelowClientArea.
        /// </summary>
        BelowClientArea = (int)ComCtl32.TVHT.BELOW,

        /// <summary>
        ///  LeftOfClientArea.
        /// </summary>
        LeftOfClientArea = (int)ComCtl32.TVHT.TOLEFT,

        /// <summary>
        ///  RightOfClientArea.
        /// </summary>
        RightOfClientArea = (int)ComCtl32.TVHT.TORIGHT,

        /// <summary>
        ///  RightOfNode.
        /// </summary>
        RightOfLabel = (int)ComCtl32.TVHT.ONITEMRIGHT,

        /// <summary>
        ///  StateImage.
        /// </summary>
        StateImage = (int)ComCtl32.TVHT.ONITEMSTATEICON,

        /// <summary>
        ///  PlusMinus.
        /// </summary>
        PlusMinus = (int)ComCtl32.TVHT.ONITEMBUTTON,
    }
}
