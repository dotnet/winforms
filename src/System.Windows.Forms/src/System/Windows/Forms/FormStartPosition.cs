// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the initial position of a form.
    /// </summary>
    [ComVisible(true)]
    public enum FormStartPosition
    {
        /// <summary>
        ///  The location and size of the form will determine its starting position.
        /// </summary>
        Manual = 0,

        /// <summary>
        ///  The form is centered on the current display, and has the dimensions
        ///  specified in the form's size.
        /// </summary>
        CenterScreen = 1,

        /// <summary>
        ///  The form is positioned at the Windows default location and has the
        ///  dimensions specified in the form's size.
        /// </summary>
        WindowsDefaultLocation = 2,

        /// <summary>
        ///  The form is positioned at the Windows default location and has the
        ///  bounds determined by Windows default.
        /// </summary>
        WindowsDefaultBounds = 3,

        /// <summary>
        ///  The form is centered within the bounds of its parent form.
        /// </summary>
        CenterParent = 4
    }
}
