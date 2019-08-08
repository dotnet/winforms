// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the style of a three-dimensional border.
    /// </summary>
    [ComVisible(true)]
    public enum Border3DStyle
    {
        /// <summary>
        ///  The border is drawn outside the specified rectangle, preserving the
        ///  dimensions of the rectangle for drawing.
        /// </summary>
        Adjust = NativeMethods.BF_ADJUST,

        /// <summary>
        ///  The border has a raised outer edge and a sunken inner edge.
        /// </summary>
        Bump = NativeMethods.EDGE_BUMP,

        /// <summary>
        ///  The border has a with a sunken inner edge and a raised outer edge.
        /// </summary>
        Etched = NativeMethods.EDGE_ETCHED,

        /// <summary>
        ///  The border has a with no three-dimensional effects.
        /// </summary>
        Flat = NativeMethods.BF_FLAT | NativeMethods.EDGE_SUNKEN,

        /// <summary>
        ///  The border has a with raised inner and outer edges.
        /// </summary>
        Raised = NativeMethods.EDGE_RAISED,

        /// <summary>
        ///  The border has a with a raised inner edge and no outer edge.
        /// </summary>
        RaisedInner = NativeMethods.BDR_RAISEDINNER,

        /// <summary>
        ///  The border has a with a raised outer edge and no inner edge.
        /// </summary>
        RaisedOuter = NativeMethods.BDR_RAISEDOUTER,

        /// <summary>
        ///  The border has a with sunken inner and outer edges.
        /// </summary>
        Sunken = NativeMethods.EDGE_SUNKEN,

        /// <summary>
        ///  The border has a with a sunken inner edge and no outer edge.
        /// </summary>
        SunkenInner = NativeMethods.BDR_SUNKENINNER,

        /// <summary>
        ///  The border has a with a sunken outer edge and no inner edge.
        /// </summary>
        SunkenOuter = NativeMethods.BDR_SUNKENOUTER,
    }
}
