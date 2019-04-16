// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the mode to start the computer in.
    /// </devdoc>
    public enum BootMode
    {
        /// <devdoc>
        /// Starts the computer in standard mode.
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        /// Starts the computer by using only the basic files and drivers.
        /// </devdoc>
        FailSafe = 1,

        /// <devdoc>
        /// Starts the computer by using the basic files, drivers and the services
        /// and drivers necessary to start networking.
        /// </devdoc>
        FailSafeWithNetwork = 2,
    }
}
