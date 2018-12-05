// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    /// <devdoc>
    ///    <para>
    ///       Specifies the mode to start the computer
    ///       in.
    ///    </para>
    /// </devdoc>
    public enum BootMode {
        /// <devdoc>
        ///    <para>
        ///       Starts the computer in standard mode.
        ///    </para>
        /// </devdoc>
        Normal = 0,
        /// <devdoc>
        ///    <para>
        ///       Starts the computer by using only the basic
        ///       files and
        ///       drivers.
        ///    </para>
        /// </devdoc>
        FailSafe = 1,
        /// <devdoc>
        ///    <para>
        ///       Starts the computer by using the basic files, drivers and
        ///       the services and drivers
        ///       necessary to start networking.
        ///    </para>
        /// </devdoc>
        FailSafeWithNetwork = 2,
    }
}

