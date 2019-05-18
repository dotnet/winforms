// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the border styles for a form.
    /// </devdoc>
    [ComVisible(true)]
    public enum FormBorderStyle
    {
        /// <summary>
        /// No border.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// A fixed, single line border.
        /// </devdoc>
        FixedSingle = 1,

        /// <summary>
        /// A fixed, three-dimensional border.
        /// </devdoc>
        Fixed3D = 2,

        /// <summary>
        /// A thick, fixed dialog-style border.
        /// </devdoc>
        FixedDialog = 3,

        /// <summary>
        /// A resizable border.
        /// </devdoc>
        Sizable = 4,

        /// <summary>
        /// A tool window border that is not resizable.
        /// </devdoc>
        FixedToolWindow = 5,

        /// <summary>
        /// A resizable tool window border.
        /// </devdoc>
        SizableToolWindow = 6,
    }
}
