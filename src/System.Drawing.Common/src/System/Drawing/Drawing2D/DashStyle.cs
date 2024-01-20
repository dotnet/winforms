// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum DashStyle
{
    Solid = GdiPlus.DashStyle.DashStyleSolid,
    Dash = GdiPlus.DashStyle.DashStyleDash,
    Dot = GdiPlus.DashStyle.DashStyleDot,
    DashDot = GdiPlus.DashStyle.DashStyleDashDot,
    DashDotDot = GdiPlus.DashStyle.DashStyleDashDotDot,
    Custom = GdiPlus.DashStyle.DashStyleCustom
}
