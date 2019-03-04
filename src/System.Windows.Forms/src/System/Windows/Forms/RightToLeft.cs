// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies a value indicating whether the text appears from right to
    /// left, as when using Hebrew or Arabic fonts.
    /// </devdoc>
    public enum RightToLeft
    {
        /// <devdoc>
        /// The text reads from left to right. This is the default.
        /// </devdoc>
        No = 0,

        /// <devdoc>
        /// The text reads from right to left.
        /// </devdoc>
        Yes = 1,

        /// <devdoc>
        /// The direction the text appears in is inherited from the parent control.
        /// </devdoc>
        Inherit = 2
    }
}
