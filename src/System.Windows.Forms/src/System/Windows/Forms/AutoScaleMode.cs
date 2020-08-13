// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the auto scaling mode used by a container control.
    /// </summary>
    public enum AutoScaleMode
    {
        /// <summary>
        ///  AutoScale is turned off.
        /// </summary>
        None,

        /// <summary>
        ///  Controls scale according to the dimensions of the font they are using.
        /// </summary>
        Font,

        /// <summary>
        ///  Controls scale according to the display Dpi.
        /// </summary>
        Dpi,

        /// <summary>
        ///  Controls scale according to their parent's scaling mode.
        ///  If there is no parent, this behaves as if AutoScaleMode.None were set.
        /// </summary>
        Inherit
    }
}
