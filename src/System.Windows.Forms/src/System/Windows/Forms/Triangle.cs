// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Diagnostics;
    using System;
    using System.Drawing;

    /// <summary>
    ///  This class fully encapsulates the painting logic for a triangle.  (Used by DataGrid)
    /// </summary>
    internal static class Triangle
    {
        private const double TRI_HEIGHT_RATIO = 2.5;
        private const double TRI_WIDTH_RATIO = 0.8;

        /* Commenting this overload out until someone actually needs it again...
        public static void Paint(Graphics g, Rectangle bounds, TriangleDirection dir, Brush backBr, Pen backPen) {
            Paint(g, bounds, dir, backBr, backPen, true);
        }
        */

        /* Commenting this overload out until someone actually needs it again...
        public static void Paint(Graphics g, Rectangle bounds, TriangleDirection dir,
                                  Brush backBr, Pen backPen, bool opaque) {
            // build an equilateral triangle centered on the midpoint of the rect.
            Point[] points = BuildTrianglePoints(dir, bounds);

            if (opaque)
                g.FillPolygon(backBr, points);
            g.DrawPolygon(backPen, points);
        }
        */

        public static void Paint(Graphics g, Rectangle bounds, TriangleDirection dir, Brush backBr,
                                 Pen backPen1, Pen backPen2, Pen backPen3, bool opaque)
        {
            // build an equilateral triangle centered on the midpoint of the rect.
            Point[] points = BuildTrianglePoints(dir, bounds);

            if (opaque)
            {
                g.FillPolygon(backBr, points);
            }

            g.DrawLine(backPen1, points[0], points[1]);
            g.DrawLine(backPen2, points[1], points[2]);
            g.DrawLine(backPen3, points[2], points[0]);
        }

        private static Point[] BuildTrianglePoints(TriangleDirection dir,
                                                    Rectangle bounds)
        {
            Point[] points = new Point[3];

            int updnWidth = (int)(bounds.Width * TRI_WIDTH_RATIO);
            if (updnWidth % 2 == 1)
            {
                updnWidth++;
            }

            int updnHeight = (int)Math.Ceiling((updnWidth / 2) * TRI_HEIGHT_RATIO);

            int lrWidth = (int)(bounds.Height * TRI_WIDTH_RATIO);
            if (lrWidth % 2 == 0)
            {
                lrWidth++;
            }

            int lrHeight = (int)Math.Ceiling((lrWidth / 2) * TRI_HEIGHT_RATIO);

            switch (dir)
            {
                case TriangleDirection.Up:
                    {
                        points[0] = new Point(0, updnHeight);
                        points[1] = new Point(updnWidth, updnHeight);
                        points[2] = new Point(updnWidth / 2, 0);
                    }
                    break;
                case TriangleDirection.Down:
                    {
                        points[0] = new Point(0, 0);
                        points[1] = new Point(updnWidth, 0);
                        points[2] = new Point(updnWidth / 2, updnHeight);
                    }
                    break;
                case TriangleDirection.Left:
                    {
                        points[0] = new Point(lrWidth, 0);
                        points[1] = new Point(lrWidth, lrHeight);
                        points[2] = new Point(0, lrHeight / 2);
                    }
                    break;
                case TriangleDirection.Right:
                    {
                        points[0] = new Point(0, 0);
                        points[1] = new Point(0, lrHeight);
                        points[2] = new Point(lrWidth, lrHeight / 2);
                    }
                    break;
                default:
                    Debug.Fail("Wrong triangle enum");
                    break;
            }

            // we need to center our triangles into the bounds given.
            // NOTE: On the up/down case, the offsets are different!
            switch (dir)
            {
                case TriangleDirection.Up:
                case TriangleDirection.Down:
                    OffsetPoints(points,
                                  bounds.X + (bounds.Width - updnHeight) / 2,
                                  bounds.Y + (bounds.Height - updnWidth) / 2);
                    break;
                case TriangleDirection.Left:
                case TriangleDirection.Right:
                    OffsetPoints(points,
                                  bounds.X + (bounds.Width - lrWidth) / 2,
                                  bounds.Y + (bounds.Height - lrHeight) / 2);
                    break;
            }
            return points;
        }

        private static void OffsetPoints(Point[] points, int xOffset, int yOffset)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X += xOffset;
                points[i].Y += yOffset;
            }
        }
    }

    internal enum TriangleDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
