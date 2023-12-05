// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the style of a three-dimensional border.
/// </summary>
public enum Border3DStyle
{
    /// <summary>
    ///  The border is drawn outside the specified rectangle, preserving the
    ///  dimensions of the rectangle for drawing.
    /// </summary>
    Adjust = (int)DRAW_EDGE_FLAGS.BF_ADJUST,

    /// <summary>
    ///  The border has a raised outer edge and a sunken inner edge.
    /// </summary>
    Bump = (int)DRAWEDGE_FLAGS.EDGE_BUMP,

    /// <summary>
    ///  The border has a with a sunken inner edge and a raised outer edge.
    /// </summary>
    Etched = (int)DRAWEDGE_FLAGS.EDGE_ETCHED,

    /// <summary>
    ///  The border has a with no three-dimensional effects.
    /// </summary>
    Flat = (int)DRAW_EDGE_FLAGS.BF_FLAT | (int)DRAWEDGE_FLAGS.EDGE_SUNKEN,

    /// <summary>
    ///  The border has a with raised inner and outer edges.
    /// </summary>
    Raised = (int)DRAWEDGE_FLAGS.EDGE_RAISED,

    /// <summary>
    ///  The border has a with a raised inner edge and no outer edge.
    /// </summary>
    RaisedInner = (int)DRAWEDGE_FLAGS.BDR_RAISEDINNER,

    /// <summary>
    ///  The border has a with a raised outer edge and no inner edge.
    /// </summary>
    RaisedOuter = (int)DRAWEDGE_FLAGS.BDR_RAISEDOUTER,

    /// <summary>
    ///  The border has a with sunken inner and outer edges.
    /// </summary>
    Sunken = (int)DRAWEDGE_FLAGS.EDGE_SUNKEN,

    /// <summary>
    ///  The border has a with a sunken inner edge and no outer edge.
    /// </summary>
    SunkenInner = (int)DRAWEDGE_FLAGS.BDR_SUNKENINNER,

    /// <summary>
    ///  The border has a with a sunken outer edge and no inner edge.
    /// </summary>
    SunkenOuter = (int)DRAWEDGE_FLAGS.BDR_SUNKENOUTER,
}
