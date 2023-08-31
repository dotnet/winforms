// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

internal static partial class Interop
{
    public struct POINTS
    {
        public short x;
        public short y;

        public override string ToString() => $"{{X={x} Y={y}}}";

        public static implicit operator Point(POINTS point) => new(point.x, point.y);
    }
}
