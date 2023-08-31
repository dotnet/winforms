// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static partial class WebBrowserHelper
{
    // Enumeration of the different states of the ActiveX control
    internal enum AXState
    {
        Passive = 0,        // Not loaded
        Loaded = 1,         // Loaded, but no server   [ocx created]
        Running = 2,        // Server running, invisible [depersisted]
        InPlaceActive = 4,  // Server in-place active [visible]
        UIActive = 8        // Used only by WebBrowserSiteBase
    }
}
