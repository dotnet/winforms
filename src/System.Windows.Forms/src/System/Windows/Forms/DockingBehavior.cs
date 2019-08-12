// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies whether any characters in the current selection have the
    ///  style or attribute.
    /// </summary>
    public enum DockingBehavior
    {
        /// <summary>
        ///  Some but not all characters.
        /// </summary>
        Never = 0,

        /// <summary>
        ///  No characters.
        /// </summary>
        Ask = 1,

        /// <summary>
        ///  All characters.
        /// </summary>
        AutoDock = 2
    }
}
