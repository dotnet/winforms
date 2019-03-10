﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the style of a three-dimensional border.
    /// </devdoc>
    [ComVisible(true)]
    public enum Border3DStyle
    {
        /// <devdoc>
        /// The border is drawn outside the specified rectangle, preserving the
        /// dimensions of the rectangle for drawing.
        /// </devdoc>
        Adjust = NativeMethods.BF_ADJUST,

        /// <devdoc>
        /// The border has a raised outer edge and a sunken inner edge.
        /// </devdoc>
        Bump = NativeMethods.EDGE_BUMP,

        /// <devdoc>
        /// The border has a with a sunken inner edge and a raised outer edge.
        /// </devdoc>
        Etched = NativeMethods.EDGE_ETCHED,

        /// <devdoc>
        /// The border has a with no three-dimensional effects.
        /// </devdoc>
        Flat = NativeMethods.BF_FLAT | NativeMethods.EDGE_SUNKEN,

        /// <devdoc>
        /// The border has a with raised inner and outer edges.
        /// </devdoc>
        Raised = NativeMethods.EDGE_RAISED,

        /// <devdoc>
        /// The border has a with a raised inner edge and no outer edge.
        /// </devdoc>
        RaisedInner = NativeMethods.BDR_RAISEDINNER,

        /// <devdoc>
        /// The border has a with a raised outer edge and no inner edge.
        /// </devdoc>
        RaisedOuter = NativeMethods.BDR_RAISEDOUTER,

        /// <devdoc>
        /// The border has a with sunken inner and outer edges.
        /// </devdoc>
        Sunken = NativeMethods.EDGE_SUNKEN,

        /// <devdoc>
        /// The border has a with a sunken inner edge and no outer edge.
        /// </devdoc>
        SunkenInner = NativeMethods.BDR_SUNKENINNER,

        /// <devdoc>
        /// The border has a with a sunken outer edge and no inner edge.
        /// </devdoc>
        SunkenOuter = NativeMethods.BDR_SUNKENOUTER,
    }
}
