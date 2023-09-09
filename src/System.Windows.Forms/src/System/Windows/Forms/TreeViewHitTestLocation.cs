// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the return value for HITTEST on treeview.
/// </summary>
[Flags]
public enum TreeViewHitTestLocations
{
    /// <summary>
    ///  No Information.
    /// </summary>
    None = (int)TVHITTESTINFO_FLAGS.TVHT_NOWHERE,

    /// <summary>
    ///  On Image.
    /// </summary>
    Image = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMICON,

    /// <summary>
    ///  On Label.
    /// </summary>
    Label = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMLABEL,

    /// <summary>
    ///  Indent.
    /// </summary>
    Indent = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMINDENT,

    /// <summary>
    ///  AboveClientArea.
    /// </summary>
    AboveClientArea = (int)TVHITTESTINFO_FLAGS.TVHT_ABOVE,

    /// <summary>
    ///  BelowClientArea.
    /// </summary>
    BelowClientArea = (int)TVHITTESTINFO_FLAGS.TVHT_BELOW,

    /// <summary>
    ///  LeftOfClientArea.
    /// </summary>
    LeftOfClientArea = (int)TVHITTESTINFO_FLAGS.TVHT_TOLEFT,

    /// <summary>
    ///  RightOfClientArea.
    /// </summary>
    RightOfClientArea = (int)TVHITTESTINFO_FLAGS.TVHT_TORIGHT,

    /// <summary>
    ///  RightOfNode.
    /// </summary>
    RightOfLabel = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMRIGHT,

    /// <summary>
    ///  StateImage.
    /// </summary>
    StateImage = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMSTATEICON,

    /// <summary>
    ///  PlusMinus.
    /// </summary>
    PlusMinus = (int)TVHITTESTINFO_FLAGS.TVHT_ONITEMBUTTON,
}
