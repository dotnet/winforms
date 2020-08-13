// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        /// <summary>
        ///  Logical structure change flags
        /// </summary>
        public enum StructureChangeType
        {
            /// <summary>
            ///  Logical child added
            /// </summary>
            ChildAdded,

            /// <summary>
            ///  Logical child removed
            /// </summary>
            ChildRemoved,

            /// <summary>
            ///  Logical children invalidated
            /// </summary>
            ChildrenInvalidated,

            /// <summary>
            ///  Logical children were bulk added
            /// </summary>
            ChildrenBulkAdded,

            /// <summary>
            ///  Logical children were bulk removed
            /// </summary>
            ChildrenBulkRemoved,
            /// <summary>
            ///  The order of the children below their parent has changed
            /// </summary>
            ChildrenReordered,
        }
    }
}
