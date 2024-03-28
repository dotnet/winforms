// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Drawing;

/// <summary>
///  Specifies the unit of measure for the given data.
/// </summary>
[TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public enum GraphicsUnit
{
    /// <summary>
    ///  Specifies the world unit as the unit of measure.
    /// </summary>
    World = Unit.UnitWorld,

    /// <summary>
    ///  Specifies 1/75 inch as the unit of measure.
    /// </summary>
    Display = Unit.UnitDisplay,

    /// <summary>
    ///  Specifies a device pixel as the unit of measure.
    /// </summary>
    Pixel = Unit.UnitPixel,

    /// <summary>
    ///  Specifies a printer's point (1/72 inch) as the unit of measure.
    /// </summary>
    Point = Unit.UnitPoint,

    /// <summary>
    ///  Specifies the inch as the unit of measure.
    /// </summary>
    Inch = Unit.UnitInch,

    /// <summary>
    ///  Specifies the document unit (1/300 inch) as the unit of measure.
    /// </summary>
    Document = Unit.UnitDocument,

    /// <summary>
    ///  Specifies the millimeter as the unit of measure.
    /// </summary>
    Millimeter = Unit.UnitMillimeter
}
