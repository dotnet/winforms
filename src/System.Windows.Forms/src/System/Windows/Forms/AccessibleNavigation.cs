// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies values for navigating between accessible objects.
    /// </devdoc>
    public enum AccessibleNavigation
    {
        /// <devdoc>
        /// Navigation to a sibling object located below the starting object.
        /// </devdoc>
        Down = 0x2,

        /// <devdoc>
        /// Navigation to the first child of the object.
        /// </devdoc>
        FirstChild = 0x7,

        /// <devdoc>
        /// Navigation to the last child of the object
        /// </devdoc>
        LastChild = 0x8,

        /// <devdoc>
        /// Navigation to the sibling object located to the left of the
        /// starting object.
        /// </devdoc>
        Left = 0x3,

        /// <devdoc>
        /// Navigation to the next logical object, generally from a sibling
        /// object to the starting object.
        /// </devdoc>
        Next = 0x5,
        
        /// <devdoc>
        /// Navigation to the previous logical object, generally from a sibling
        /// object to the starting object.
        /// </devdoc>
        Previous = 0x6,
        
        /// <devdoc>
        /// Navigation to the sibling object located to the right of the
        /// starting object.
        /// </devdoc>
        Right = 0x4,
        
        /// <devdoc>
        /// Navigation to a sibling object located above the starting object.
        /// </devdoc>
        Up = 0x1,
    }
}
