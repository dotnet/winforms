// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the return value for HITTEST on treeview.
    /// </devdoc>
    [Flags]
    [ComVisible(true)]
    public enum TreeViewHitTestLocations
    {
        /// <summary>
        /// No Information.
        /// </devdoc>
        None = NativeMethods.TVHT_NOWHERE,

        /// <summary>
        /// On Image.
        /// </devdoc>
        Image = NativeMethods.TVHT_ONITEMICON,

        /// <summary>
        /// On Label.
        /// </devdoc>
        Label = NativeMethods.TVHT_ONITEMLABEL,

        /// <summary>
        /// Indent.
        /// </devdoc>
        Indent = NativeMethods.TVHT_ONITEMINDENT,

        /// <summary>
        /// AboveClientArea.
        /// </devdoc>
        AboveClientArea =  NativeMethods.TVHT_ABOVE,

        /// <summary>
        /// BelowClientArea.
        /// </devdoc>
        BelowClientArea = NativeMethods.TVHT_BELOW,

        /// <summary>
        /// LeftOfClientArea.
        /// </devdoc>
        LeftOfClientArea = NativeMethods.TVHT_TOLEFT,

        /// <summary>
        /// RightOfClientArea.
        /// </devdoc>
        RightOfClientArea = NativeMethods.TVHT_TORIGHT,

        /// <summary>
        /// RightOfNode.
        /// </devdoc>
        RightOfLabel =   NativeMethods.TVHT_ONITEMRIGHT,

        /// <summary>
        /// StateImage.
        /// </devdoc>
        StateImage = NativeMethods.TVHT_ONITEMSTATEICON,

        /// <summary>
        ///      PlusMinus.
        /// </devdoc>
        PlusMinus = NativeMethods.TVHT_ONITEMBUTTON,
    }
}
