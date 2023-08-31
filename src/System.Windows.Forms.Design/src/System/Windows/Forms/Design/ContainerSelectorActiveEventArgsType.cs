// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Specifies IDs for containers of certain event types.
/// </summary>
internal enum ContainerSelectorActiveEventArgsType
{
    /// <summary>
    ///  Indicates the container of the active event was the contextmenu.
    /// </summary>
    Contextmenu = 1,
    /// <summary>
    ///  Indicates the container of the active event was the mouse.
    /// </summary>
    Mouse = 2,
}
