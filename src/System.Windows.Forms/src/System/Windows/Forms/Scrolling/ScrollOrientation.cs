// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="ScrollBar.Scroll"/>
///  event. This enumeration gives the orientation of the scroll that took place
/// </summary>
public enum ScrollOrientation
{
    /// <summary>
    ///  Denotes that horizontal scrolling took place.
    /// </summary>
    HorizontalScroll,

    /// <summary>
    ///  Denotes that vertical scrolling took place.
    /// </summary>
    VerticalScroll
}
