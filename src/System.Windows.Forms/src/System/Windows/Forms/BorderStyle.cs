// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the border style for a control or form.
    /// </devdoc>
    [ComVisible(true)]
    public enum BorderStyle
    {
        /// <devdoc>
        /// No border.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// A single-line border.
        /// </devdoc>
        FixedSingle = 1,

        /// <devdoc>
        /// A three-dimensional border.
        /// </devdoc>
        Fixed3D = 2,
    }
}
