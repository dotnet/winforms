// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Numerics;

namespace System;

internal static class GdiExtensions
{
    public static string ToSystemColorString(this COLORREF colorRef)
        => SystemCOLORs.ToSystemColorString(colorRef);

    public static Point TransformPoint(in this Matrix3x2 transform, Point point)
    {
        if (transform.IsIdentity)
        {
            return point;
        }

        float y = point.Y;
        float x = point.X;

        float xadd = y * transform.M21 + transform.M31;
        float yadd = x * transform.M12 + transform.M32;
        x *= transform.M11;
        x += xadd;
        y *= transform.M22;
        y += yadd;

        return new Point((int)x, (int)y);
    }
}
