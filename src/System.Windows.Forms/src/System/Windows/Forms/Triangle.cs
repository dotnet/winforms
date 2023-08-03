// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This class fully encapsulates the painting logic for a triangle.  (Used by DataGrid)
/// </summary>
internal static partial class Triangle
{
    private const double TRI_HEIGHT_RATIO = 2.5;
    private const double TRI_WIDTH_RATIO = 0.8;

    public static void Paint(
        Graphics g,
        Rectangle bounds,
        TriangleDirection dir,
        Brush backBr,
        Pen backPen1,
        Pen backPen2,
        Pen backPen3,
        bool opaque)
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

    private static Point[] BuildTrianglePoints(
        TriangleDirection dir,
        Rectangle bounds)
    {
        Point[] points = new Point[3];

        int upDownWidth = (int)(bounds.Width * TRI_WIDTH_RATIO);
        if (upDownWidth % 2 == 1)
        {
            upDownWidth++;
        }

        int upDownHeight = (int)Math.Ceiling((upDownWidth / 2) * TRI_HEIGHT_RATIO);

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
                    points[0] = new Point(0, upDownHeight);
                    points[1] = new Point(upDownWidth, upDownHeight);
                    points[2] = new Point(upDownWidth / 2, 0);
                }

                break;
            case TriangleDirection.Down:
                {
                    points[0] = new Point(0, 0);
                    points[1] = new Point(upDownWidth, 0);
                    points[2] = new Point(upDownWidth / 2, upDownHeight);
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
                              bounds.X + (bounds.Width - upDownHeight) / 2,
                              bounds.Y + (bounds.Height - upDownWidth) / 2);
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
