// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    public enum ThemedScrollbarMode
    {
        /// <summary>
        ///  The window and all of its children will have themed scrollbars
        /// </summary>
        All = 1,

        /// <summary>
        ///  The window and all of its children will be un-themed
        /// </summary>
        None = 2,

        /// <summary>
        ///  The window will have themed scrollbars but all of its children will be un-themed
        /// </summary>
        OnlyTopLevel = 3
    };
}
