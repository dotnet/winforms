// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        public enum ExpandCollapseState
        {
            /// <summary>
            ///  No children are showing
            /// </summary>
            Collapsed,

            /// <summary>
            ///  All children are showing
            /// </summary>
            Expanded,

            /// <summary>
            ///  Not all children are showing
            /// </summary>
            PartiallyExpanded,

            /// <summary>
            ///  Does not expand or collapse
            /// </summary>
            LeafNode
        }
    }
}
