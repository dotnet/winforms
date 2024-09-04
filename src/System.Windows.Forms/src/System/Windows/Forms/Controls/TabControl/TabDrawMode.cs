// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The TabStrip and TabControl both support ownerdraw functionality, but
///  only one type, in which you can paint the tabs individually. This
///  enumeration contains the valid values for it's drawMode property.
/// </summary>
public enum TabDrawMode
{
    /// <summary>
    ///  All the items in the control are painted by the system and are of the
    ///  same size
    /// </summary>
    Normal = 0,

    /// <summary>
    ///  The user paints the items in the control manually
    /// </summary>
    OwnerDrawFixed = 1,
}
