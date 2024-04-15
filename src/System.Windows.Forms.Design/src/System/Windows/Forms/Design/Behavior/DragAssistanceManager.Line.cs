// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

internal sealed partial class DragAssistanceManager
{
    /// <summary>
    ///  Our 'line' class - used to manage two points and calculate the difference between any two lines.
    /// </summary>
    internal class Line
    {
        public int X1 { get; set; }

        public int Y1 { get; set; }

        public int X2 { get; set; }

        public int Y2 { get; set; }

        public LineType LineType { get; set; }

        public Rectangle OriginalBounds { get; set; }

        public PaddingLineType PaddingLineType { get; set; }

        public Line(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            LineType = LineType.Standard;
        }

        private Line(int x1, int y1, int x2, int y2, LineType type)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            LineType = type;
        }

        public static Line[]? GetDiffs(Line l1, Line l2)
        {
            // x's align
            if (l1.X1 == l1.X2 && l1.X1 == l2.X1)
            {
                return [
                    new(l1.X1, Math.Min(l1.Y1, l2.Y1), l1.X1, Math.Max(l1.Y1, l2.Y1)),
                    new(l1.X1, Math.Min(l1.Y2, l2.Y2), l1.X1, Math.Max(l1.Y2, l2.Y2))
                ];
            }

            // y's align
            if (l1.Y1 == l1.Y2 && l1.Y1 == l2.Y1)
            {
                return [
                    new(Math.Min(l1.X1, l2.X1), l1.Y1, Math.Max(l1.X1, l2.X1), l1.Y1),
                    new(Math.Min(l1.X2, l2.X2), l1.Y1, Math.Max(l1.X2, l2.X2), l1.Y1)
                ];
            }

            return null;
        }

        public static Line? Overlap(Line l1, Line l2)
        {
            // Need to be the same type
            if (l1.LineType != l2.LineType)
            {
                return null;
            }

            // only makes sense to do this for Standard and Baseline
            if (l1.LineType is not LineType.Standard and not LineType.Baseline)
            {
                return null;
            }

            // 2 overlapping vertical lines
            if ((l1.X1 == l1.X2) && (l2.X1 == l2.X2) && (l1.X1 == l2.X1))
            {
                return new Line(l1.X1, Math.Min(l1.Y1, l2.Y1), l1.X2, Math.Max(l1.Y2, l2.Y2), l1.LineType);
            }

            // 2 overlapping horizontal lines
            if ((l1.Y1 == l1.Y2) && (l2.Y1 == l2.Y2) && (l1.Y1 == l2.Y2))
            {
                return new Line(Math.Min(l1.X1, l2.X1), l1.Y1, Math.Max(l1.X2, l2.X2), l1.Y2, l1.LineType);
            }

            return null;
        }

        public override string ToString() =>
            $"Line, type = {LineType}, dims =({X1}, {Y1})->({X2}, {Y2})";
    }
}
