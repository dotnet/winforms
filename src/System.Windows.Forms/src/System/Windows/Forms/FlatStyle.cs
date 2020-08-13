// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the style of control to display.
    /// </summary>
    public enum FlatStyle
    {
        /// <summary>
        ///  The control appears flat.
        /// </summary>
        Flat,

        /// <summary>
        ///  A control appears flat until the mouse pointer moves over it, at
        ///  which point it appears three-dimensional.
        /// </summary>
        Popup,

        /// <summary>
        ///  The control appears three-dimensional.
        /// </summary>
        Standard,

        /// <summary>
        ///  The control appears three-dimensional.
        /// </summary>
        System,
    }
}
