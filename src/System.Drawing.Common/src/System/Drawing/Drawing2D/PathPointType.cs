// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum PathPointType
{
    /// <summary>
    ///  Indicates that the point is the start of a figure.
    /// </summary>
    Start = GdiPlus.PathPointType.PathPointTypeStart,

    /// <summary>
    ///  Indicates that the point is an endpoint of a line.
    /// </summary>
    Line = GdiPlus.PathPointType.PathPointTypeLine,

    /// <summary>
    ///  Indicates that the point is an endpoint or a control point of a cubic Bézier spline.
    /// </summary>
    Bezier = GdiPlus.PathPointType.PathPointTypeBezier,

    /// <summary>
    ///  Masks all bits except for the three low-order bits, which indicate the point type.
    /// </summary>
    PathTypeMask = GdiPlus.PathPointType.PathPointTypePathTypeMask,

    /// <summary>
    ///  Not used.
    /// </summary>
    DashMode = GdiPlus.PathPointType.PathPointTypeDashMode,

    /// <summary>
    ///  Specifies that the point is a marker.
    /// </summary>
    PathMarker = GdiPlus.PathPointType.PathPointTypePathMarker,

    /// <summary>
    ///  Specifies that the point is the last point in a closed subpath (figure).
    /// </summary>
    CloseSubpath = GdiPlus.PathPointType.PathPointTypeCloseSubpath,

    /// <inheritdoc cref="Bezier"/>
    Bezier3 = GdiPlus.PathPointType.PathPointTypeBezier3
}
