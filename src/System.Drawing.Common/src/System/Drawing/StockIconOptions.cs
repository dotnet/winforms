// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    Default         = 0x000000000,

    /// <summary>
    ///  Retrieve the small version of the icon (as defined by the current system metrics).
    /// </summary>
    SmallIcon       = 0x000000001,

    /// <summary>
    ///  Retrieve the shell icon size of the icon.
    /// </summary>
    ShellIconSize   = 0x000000004,

    /// <summary>
    ///  Adds a link overlay onto the icon.
    /// </summary>
    LinkOverlay     = 0x000008000,

    /// <summary>
    ///  Blends the icon with the system highlight color.
    /// </summary>
    Selected        = 0x000010000,
}
#endif
