// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how a form window is displayed.
    /// </devdoc>
    [ComVisible(true)]
    public enum FormWindowState
    {
        /// <devdoc>
        /// A default sized window.
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        /// A minimized window.
        /// </devdoc>
        Minimized = 1,

        /// <devdoc>
        /// A maximized window.
        /// </devdoc>
        Maximized = 2,
    }
}
