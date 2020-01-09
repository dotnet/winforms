// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how a control anchors to the edges of its container.
    /// </summary>
    [Editor("System.Windows.Forms.Design.AnchorEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    [Flags]
    public enum AnchorStyles
    {
        /// <summary>
        ///  The control is anchored to the top edge of its container.
        /// </summary>
        Top = 0x01,

        /// <summary>
        ///  The control is anchored to the bottom edge of its container.
        /// </summary>
        Bottom = 0x02,

        /// <summary>
        ///  The control is anchored to the left edge of its container.
        /// </summary>
        Left = 0x04,

        /// <summary>
        ///  The control is anchored to the right edge of its container.
        /// </summary>
        Right = 0x08,

        /// <summary>
        ///  The control is not anchored to any edges of its container.
        /// </summary>
        None = 0,
    }
}
