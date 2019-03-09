﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how a control anchors to the edges of its container.
    /// </devdoc>
    [Editor("System.Windows.Forms.Design.AnchorEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
    [Flags]
    public enum AnchorStyles
    {
        /// <devdoc>
        /// The control is anchored to the top edge of its container.
        /// </devdoc>
        Top = 0x01,

        /// <devdoc>
        /// The control is anchored to the bottom edge of its container.
        /// </devdoc>
        Bottom = 0x02,

        /// <devdoc>
        /// The control is anchored to the left edge of its container.
        /// </devdoc>
        Left = 0x04,

        /// <devdoc>
        /// The control is anchored to the right edge of its container.
        /// </devdoc>
        Right = 0x08,

        /// <devdoc>
        /// The control is not anchored to any edges of its container.
        /// </devdoc>
        None = 0,
    }
}
