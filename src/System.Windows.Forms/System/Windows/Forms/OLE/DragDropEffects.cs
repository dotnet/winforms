// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

[Flags]
public enum DragDropEffects
{
    /// <summary>
    ///  The drop target does not accept the data.
    /// </summary>
    None = (int)DROPEFFECT.DROPEFFECT_NONE,

    /// <summary>
    ///  The data is copied to the drop target.
    /// </summary>
    Copy = (int)DROPEFFECT.DROPEFFECT_COPY,

    /// <summary>
    ///  The data from the drag source is moved to the drop target.
    /// </summary>
    Move = (int)DROPEFFECT.DROPEFFECT_MOVE,

    /// <summary>
    ///  The data from the drag source is linked to the drop target.
    /// </summary>
    Link = (int)DROPEFFECT.DROPEFFECT_LINK,

    /// <summary>
    ///  Scrolling is about to start or is currently occurring in the drop target.
    /// </summary>
    Scroll = unchecked((int)DROPEFFECT.DROPEFFECT_SCROLL),

    /// <summary>
    ///  The data is copied, removed from the drag source, and scrolled in the
    ///  drop target. NOTE: Link is intentionally not present in All.
    /// </summary>
    All = Copy | Move | Scroll,
}
