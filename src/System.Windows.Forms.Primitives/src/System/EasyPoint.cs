// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System
{
    /// <summary>
    ///  Simple immutable, intermediary, struct that allows creating point data via tuple: (12, 6).
    /// </summary>
    internal readonly struct EasyPoint
    {
        public int X { get; }
        public int Y { get; }
        public EasyPoint(int x, int y) => (X, Y) = (x, y);
        public EasyPoint((int X, int Y) point) => (X, Y) = point;
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
        public static implicit operator Point(in EasyPoint point) => new Point(point.X, point.Y);
        public static implicit operator EasyPoint(Point point) => new EasyPoint(point.X, point.Y);
        public static implicit operator EasyPoint(in (int X, int Y) point) => new EasyPoint(point);
    }
}
