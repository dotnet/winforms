// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies a value indicating whether the text appears from right to
    ///  left, as when using Hebrew or Arabic fonts.
    /// </summary>
    public enum RightToLeft
    {
        /// <summary>
        ///  The text reads from left to right. This is the default.
        /// </summary>
        No = 0,

        /// <summary>
        ///  The text reads from right to left.
        /// </summary>
        Yes = 1,

        /// <summary>
        ///  The direction the text appears in is inherited from the parent control.
        /// </summary>
        Inherit = 2
    }
}
