// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Specifies identifiers to indicate the display modes used by <see cref='ByteViewer'/>.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        ///  Indicates using Hexadecimal format.
        /// </summary>
        Hexdump = 1,

        /// <summary>
        ///  Indicates using ANSI format.
        /// </summary>
        Ansi = 2,

        /// <summary>
        ///  Indicates using Unicode format.
        /// </summary>
        Unicode = 3,

        /// <summary>
        ///  Indicates using automatic format selection.
        /// </summary>
        Auto = 4,
    }
}
