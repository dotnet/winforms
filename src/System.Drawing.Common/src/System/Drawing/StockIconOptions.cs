// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Shell;

namespace System.Drawing;

#if NET8_0_OR_GREATER
/// <summary>
///  Options for use with <see cref="SystemIcons.GetStockIcon(StockIconId, StockIconOptions)"/>.
/// </summary>
[Flags]
public enum StockIconOptions
{
    /// <summary>
    ///  Use the defaults, which is to retrieve a large version of the icon (as defined by the current system
    ///  metrics).
    /// </summary>
    Default = 0,

    /// <summary>
    ///  Retrieve the small version of the icon (as defined by the current system metrics).
    /// </summary>
    SmallIcon = (int)SHGSI_FLAGS.SHGSI_SMALLICON,

    /// <summary>
    ///  Retrieve the shell icon size of the icon.
    /// </summary>
    ShellIconSize = (int)SHGSI_FLAGS.SHGSI_SHELLICONSIZE,

    /// <summary>
    ///  Adds a link overlay onto the icon.
    /// </summary>
    LinkOverlay = (int)SHGSI_FLAGS.SHGSI_LINKOVERLAY,

    /// <summary>
    ///  Blends the icon with the system highlight color.
    /// </summary>
    Selected = (int)SHGSI_FLAGS.SHGSI_SELECTED,
}
#endif
