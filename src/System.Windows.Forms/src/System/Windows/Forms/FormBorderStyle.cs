// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the border styles for a form.
    /// </devdoc>
    [ComVisible(true)]
    public enum FormBorderStyle
    {
        /// <devdoc>
        /// No border.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// A fixed, single line border.
        /// </devdoc>
        FixedSingle = 1,

        /// <devdoc>
        /// A fixed, three-dimensional border.
        /// </devdoc>
        Fixed3D = 2,

        /// <devdoc>
        /// A thick, fixed dialog-style border.
        /// </devdoc>
        FixedDialog = 3,

        /// <devdoc>
        /// A resizable border.
        /// </devdoc>
        Sizable = 4,

        /// <devdoc>
        /// A tool window border that is not resizable.
        /// </devdoc>
        FixedToolWindow = 5,

        /// <devdoc>
        /// A resizable tool window border.
        /// </devdoc>
        SizableToolWindow = 6,
    }
}
