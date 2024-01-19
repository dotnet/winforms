// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum LineCap
{
    Flat = GdiPlus.LineCap.LineCapFlat,
    Square = GdiPlus.LineCap.LineCapSquare,
    Round = GdiPlus.LineCap.LineCapRound,
    Triangle = GdiPlus.LineCap.LineCapTriangle,
    NoAnchor = GdiPlus.LineCap.LineCapNoAnchor,                 // corresponds to flat cap
    SquareAnchor = GdiPlus.LineCap.LineCapSquareAnchor,         // corresponds to square cap
    RoundAnchor = GdiPlus.LineCap.LineCapRoundAnchor,           // corresponds to round cap
    DiamondAnchor = GdiPlus.LineCap.LineCapDiamondAnchor,       // corresponds to triangle cap
    ArrowAnchor = GdiPlus.LineCap.LineCapArrowAnchor,           // no correspondence
    Custom = GdiPlus.LineCap.LineCapCustom,                     // custom cap
    AnchorMask = GdiPlus.LineCap.LineCapAnchorMask              // mask to check for anchor or not.
}
