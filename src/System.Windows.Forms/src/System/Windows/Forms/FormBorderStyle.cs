// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the border styles for a form.
    /// </summary>
    public enum FormBorderStyle
    {
        /// <summary>
        ///  No border.
        /// </summary>
        None = 0,

        /// <summary>
        ///  A fixed, single line border.
        /// </summary>
        FixedSingle = 1,

        /// <summary>
        ///  A fixed, three-dimensional border.
        /// </summary>
        Fixed3D = 2,

        /// <summary>
        ///  A thick, fixed dialog-style border.
        /// </summary>
        FixedDialog = 3,

        /// <summary>
        ///  A resizable border.
        /// </summary>
        Sizable = 4,

        /// <summary>
        ///  A tool window border that is not resizable.
        /// </summary>
        FixedToolWindow = 5,

        /// <summary>
        ///  A resizable tool window border.
        /// </summary>
        SizableToolWindow = 6,
    }
}
