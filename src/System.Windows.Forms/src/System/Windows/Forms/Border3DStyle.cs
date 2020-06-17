// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the style of a three-dimensional border.
    /// </summary>
    public enum Border3DStyle
    {
        /// <summary>
        ///  The border is drawn outside the specified rectangle, preserving the
        ///  dimensions of the rectangle for drawing.
        /// </summary>
        Adjust = (int)User32.BF.ADJUST,

        /// <summary>
        ///  The border has a raised outer edge and a sunken inner edge.
        /// </summary>
        Bump = (int)User32.EDGE.BUMP,

        /// <summary>
        ///  The border has a with a sunken inner edge and a raised outer edge.
        /// </summary>
        Etched = (int)User32.EDGE.ETCHED,

        /// <summary>
        ///  The border has a with no three-dimensional effects.
        /// </summary>
        Flat = (int)User32.BF.FLAT | (int)User32.EDGE.SUNKEN,

        /// <summary>
        ///  The border has a with raised inner and outer edges.
        /// </summary>
        Raised = (int)User32.EDGE.RAISED,

        /// <summary>
        ///  The border has a with a raised inner edge and no outer edge.
        /// </summary>
        RaisedInner = (int)User32.EDGE.RAISEDINNER,

        /// <summary>
        ///  The border has a with a raised outer edge and no inner edge.
        /// </summary>
        RaisedOuter = (int)User32.EDGE.RAISEDOUTER,

        /// <summary>
        ///  The border has a with sunken inner and outer edges.
        /// </summary>
        Sunken = (int)User32.EDGE.SUNKEN,

        /// <summary>
        ///  The border has a with a sunken inner edge and no outer edge.
        /// </summary>
        SunkenInner = (int)User32.EDGE.SUNKENINNER,

        /// <summary>
        ///  The border has a with a sunken outer edge and no inner edge.
        /// </summary>
        SunkenOuter = (int)User32.EDGE.SUNKENOUTER,
    }
}
