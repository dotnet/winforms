// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how a form window is displayed.
    /// </summary>
    public enum FormWindowState
    {
        /// <summary>
        ///  A default sized window.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  A minimized window.
        /// </summary>
        Minimized = 1,

        /// <summary>
        ///  A maximized window.
        /// </summary>
        Maximized = 2,
    }
}
