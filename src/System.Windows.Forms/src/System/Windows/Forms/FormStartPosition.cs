// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the initial position of a form.
    /// </devdoc>
    [ComVisible(true)]
    public enum FormStartPosition
    {
        /// <devdoc>
        /// The location and size of the form will determine its starting position.
        /// </devdoc>
        Manual = 0,

        /// <devdoc>
        /// The form is centered on the current display, and has the dimensions
        /// specified in the form's size.
        /// </devdoc>
        CenterScreen = 1,

        /// <devdoc>
        /// The form is positioned at the Windows default location and has the
        /// dimensions specified in the form's size.
        /// </devdoc>
        WindowsDefaultLocation = 2,

        /// <devdoc>
        /// The form is positioned at the Windows default location and has the
        /// bounds determined by Windows default.
        /// </devdoc>
        WindowsDefaultBounds = 3,

        /// <devdoc>
        /// The form is centered within the bounds of its parent form.
        /// </devdoc>
        CenterParent = 4
    }
}
