
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the HighDpi mode.
    /// </summary>
    public enum HighDpiMode
    {
        /// <summary>
        /// The window does not scale for DPI changes and always assumes a scale factor of 100%.
        /// </summary>
        DpiUnaware,

        /// <summary>
        /// The window will query for the DPI of the primary monitor once and use this for the process on all monitors.
        /// </summary>
        SystemAware,

        /// <summary>
        /// The Window checks for DPI when it's created and adjusts scale factor when the DPI changes.
        /// </summary>
        PerMonitor,

        /// <summary>
        /// Similar to PerMonitor, but enables Child window DPI change notification, improved scaling of comctl32 controls and dialog scaling.
        /// </summary>
        PerMonitorV2,

        /// <summary>
        /// Similar to DpiUnaware, but improves the quality of GDI/GDI+ based content.
        /// </summary>
        DpiUnawareGdiScaled
    }
}
