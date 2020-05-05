// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        /// <summary>
        ///  Directions for navigation the UIAutomation tree
        /// </summary>
        public enum NavigateDirection
        {
            /// <summary>
            ///  Navigate to parent
            /// </summary>
            Parent,
            /// <summary>
            ///  Navigate to next sibling
            /// </summary>
            NextSibling,
            /// <summary>
            ///  Navigate to previous sibling
            /// </summary>
            PreviousSibling,
            /// <summary>
            ///  Navigate to first child
            /// </summary>
            FirstChild,
            /// <summary>
            ///  Navigate to last child
            /// </summary>
            LastChild,
        }
    }
}
