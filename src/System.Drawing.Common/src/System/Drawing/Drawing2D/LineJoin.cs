// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum LineJoin
{
    Miter = GdiPlus.LineJoin.LineJoinMiter,
    Bevel = GdiPlus.LineJoin.LineJoinBevel,
    Round = GdiPlus.LineJoin.LineJoinRound,
    MiterClipped = GdiPlus.LineJoin.LineJoinMiterClipped
}
