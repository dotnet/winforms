// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies values for navigating between accessible objects.
    /// </summary>
    public enum AccessibleNavigation
    {
        /// <summary>
        ///  Navigation to a sibling object located below the starting object.
        /// </summary>
        Down = 0x2,

        /// <summary>
        ///  Navigation to the first child of the object.
        /// </summary>
        FirstChild = 0x7,

        /// <summary>
        ///  Navigation to the last child of the object
        /// </summary>
        LastChild = 0x8,

        /// <summary>
        ///  Navigation to the sibling object located to the left of the
        ///  starting object.
        /// </summary>
        Left = 0x3,

        /// <summary>
        ///  Navigation to the next logical object, generally from the starting
        ///  object to a sibling object.
        /// </summary>
        Next = 0x5,

        /// <summary>
        ///  Navigation to the previous logical object, generally from a sibling
        ///  object to the starting object.
        /// </summary>
        Previous = 0x6,

        /// <summary>
        ///  Navigation to the sibling object located to the right of the
        ///  starting object.
        /// </summary>
        Right = 0x4,

        /// <summary>
        ///  Navigation to a sibling object located above the starting object.
        /// </summary>
        Up = 0x1,
    }
}
