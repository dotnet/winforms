﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the bounds of the control to use when defining a control's size
    /// and position.
    /// </devdoc>
    [Flags]
    public enum BoundsSpecified
    {
        /// <summary>
        /// Specifies the left edge of the control is defined.
        /// </devdoc>
        X = 0x1,

        /// <summary>
        /// Specifies the top edge of the control of the control is defined.
        /// </devdoc>
        Y = 0x2,

        /// <summary>
        /// Specifies the width of the control is defined.
        /// </devdoc>
        Width = 0x4,

        /// <summary>
        /// Specifies the height of the control is defined.
        /// </devdoc>
        Height = 0x8,

        /// <summary>
        /// Both <see langword='X'/> and <see langword='Y'/> coordinates of the
        /// control are defined.
        /// </devdoc>
        Location = X | Y,

        /// <summary>
        /// Both <see cref='System.Windows.Forms.Control.Width'/> and <see cref='System.Windows.Forms.Control.Height'/>
        /// property values of the control are defined.
        /// </devdoc>
        Size = Width | Height,

        /// <summary>
        /// Both <see cref='System.Windows.Forms.Control.Location'/> and <see cref='System.Windows.Forms.Control.Size'/>
        /// property values are defined.
        /// </devdoc>
        All = Location | Size,

        /// <summary>
        /// No bounds are specified.
        /// </devdoc>
        None = 0,
    }
}
