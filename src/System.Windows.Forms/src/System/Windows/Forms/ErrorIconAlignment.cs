// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Describes the set of locations that an error icon can appear in
    /// relation to the control with the error.
    /// </devdoc>
    public enum ErrorIconAlignment
    {
        /// <summary>
        /// The icon appears aligned with the top of the control, and to the
        /// left of the control.
        /// </devdoc>
        TopLeft,

        /// <summary>
        /// The icon appears aligned with the top of the control, and to the
        /// right of the control.
        /// </devdoc>
        TopRight,

        /// <summary>
        /// The icon appears aligned with the middle of the control, and the
        /// left of the control.
        /// </devdoc>
        MiddleLeft,

        /// <summary>
        /// The icon appears aligned with the middle of the control, and the
        /// right of the control.
        /// </devdoc>
        MiddleRight,

        /// <summary>
        /// The icon appears aligned with the bottom of the control, and the
        /// left of the control.
        /// </devdoc>
        BottomLeft,

        /// <summary>
        /// The icon appears aligned with the bottom of the control, and the
        /// right of the control.
        /// </devdoc>
        BottomRight
    }
}
