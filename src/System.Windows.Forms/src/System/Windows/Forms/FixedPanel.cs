// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the Fixed Panel in the SplitContainer Control.
    /// </summary>
    public enum FixedPanel
    {
        /// <summary>
        ///  No panel is fixed. Resize causes the Resize of both the panels.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Panel1 is Fixed. The resize will increase the size of second panel.
        /// </summary>
        Panel1 = 1,

        /// <summary>
        ///  Panel2 is Fixed. The resize will increase the size of first panel.
        /// </summary>
        Panel2 = 2,
    }
}
