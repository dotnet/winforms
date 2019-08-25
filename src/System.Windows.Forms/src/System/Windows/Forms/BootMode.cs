// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the mode to start the computer in.
    /// </summary>
    public enum BootMode
    {
        /// <summary>
        ///  Starts the computer in standard mode.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  Starts the computer by using only the basic files and drivers.
        /// </summary>
        FailSafe = 1,

        /// <summary>
        ///  Starts the computer by using the basic files, drivers and the services
        ///  and drivers necessary to start networking.
        /// </summary>
        FailSafeWithNetwork = 2,
    }
}
