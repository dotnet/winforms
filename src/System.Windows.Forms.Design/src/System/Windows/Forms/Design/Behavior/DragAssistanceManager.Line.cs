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
        public int x1, y1, x2, y2;
        private LineType _lineType;
        private PaddingLineType _paddingLineType;
        private Rectangle _originalBounds;

        public LineType LineType
        {
            get => _lineType;
            set => _lineType = value;
        }

        public Rectangle OriginalBounds
        {
            get => _originalBounds;
            set => _originalBounds = value;
        }

        public PaddingLineType PaddingLineType
        {
            get => _paddingLineType;
            set => _paddingLineType = value;
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            _lineType = LineType.Standard;
        }

        private Line(int x1, int y1, int x2, int y2, LineType type)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            _lineType = type;
        }

        public static Line[]? GetDiffs(Line l1, Line l2)
        {
            // x's align
            if (l1.x1 == l1.x2 && l1.x1 == l2.x1)
            {
                return [
                    new(l1.x1, Math.Min(l1.y1, l2.y1), l1.x1, Math.Max(l1.y1, l2.y1)),
                    new(l1.x1, Math.Min(l1.y2, l2.y2), l1.x1, Math.Max(l1.y2, l2.y2))
                ];
            }

            // y's align
            if (l1.y1 == l1.y2 && l1.y1 == l2.y1)
            {
                return [
                    new(Math.Min(l1.x1, l2.x1), l1.y1, Math.Max(l1.x1, l2.x1), l1.y1),
                    new(Math.Min(l1.x2, l2.x2), l1.y1, Math.Max(l1.x2, l2.x2), l1.y1)
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
            if ((l1.LineType != LineType.Standard) && (l1.LineType != LineType.Baseline))
            {
                return null;
            }

            // 2 overlapping vertical lines
            if ((l1.x1 == l1.x2) && (l2.x1 == l2.x2) && (l1.x1 == l2.x1))
            {
                return new Line(l1.x1, Math.Min(l1.y1, l2.y1), l1.x2, Math.Max(l1.y2, l2.y2), l1.LineType);
            }

            // 2 overlapping horizontal lines
            if ((l1.y1 == l1.y2) && (l2.y1 == l2.y2) && (l1.y1 == l2.y2))
            {
                return new Line(Math.Min(l1.x1, l2.x1), l1.y1, Math.Max(l1.x2, l2.x2), l1.y2, l1.LineType);
            }

            return null;
        }

        public override string ToString()
        {
            return $"Line, type = {_lineType}, dims =({x1}, {y1})->({x2}, {y2})";
        }
    }
}
