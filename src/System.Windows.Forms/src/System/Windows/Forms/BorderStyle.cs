// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the border style for a control or form.
    /// </summary>
    [ComVisible(true)]
    public enum BorderStyle
    {
        /// <summary>
        ///  No border.
        /// </summary>
        None = 0,

        /// <summary>
        ///  A single-line border.
        /// </summary>
        FixedSingle = 1,

        /// <summary>
        ///  A three-dimensional border.
        /// </summary>
        Fixed3D = 2,
    }
}
