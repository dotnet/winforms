// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copyright (C) 2006-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Drawing.Drawing2D.Tests;

public class GraphicsPathTests
{
    private const float Pi4 = (float)(Math.PI / 4);
    private const float Delta = 0.0003f;

    [Fact]
    public void Ctor_Default_Success()
    {
        using GraphicsPath gp = new();
        Assert.Equal(FillMode.Alternate, gp.FillMode);
        AssertEmptyGraphicsPath(gp);
    }

    [Fact]
    public void Ctor_FillMode_Success()
    {
        using GraphicsPath gpa = new(FillMode.Alternate);
        using GraphicsPath gpw = new(FillMode.Winding);
        Assert.Equal(FillMode.Alternate, gpa.FillMode);
        AssertEmptyGraphicsPath(gpa);
        Assert.Equal(FillMode.Winding, gpw.FillMode);
        AssertEmptyGraphicsPath(gpw);
    }

    [Fact]
    public void Ctor_SamePoints_Success()
    {
        byte[] types = [0, 1, 1, 1, 1, 1];
        Point[] points = [
            new(1, 1), new(1, 1), new(1, 1),
            new(1, 1), new(1, 1), new(1, 1),
        ];

        PointF[] fPoints = [
            new(1f, 1f), new(1f, 1f), new(1f, 1f),
            new(1f, 1f), new(1f, 1f), new(1f, 1f),
        ];

        using GraphicsPath gp = new(points, types);
        using GraphicsPath gpf = new(fPoints, types);
        Assert.Equal(FillMode.Alternate, gp.FillMode);
        Assert.Equal(6, gp.PointCount);
        Assert.Equal(FillMode.Alternate, gpf.FillMode);
        Assert.Equal(6, gpf.PointCount);
        types[0] = 1;
        Assert.Equal(FillMode.Alternate, gp.FillMode);
        Assert.Equal(6, gp.PointCount);
        Assert.Equal(FillMode.Alternate, gpf.FillMode);
        Assert.Equal(6, gpf.PointCount);
    }

    [Fact]
    public void Ctor_PointsNull_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("pts", () => new GraphicsPath((Point[])null, new byte[1]));
    }

    public static IEnumerable<object[]> AddCurve_PointsTypesLengthMismatch_TestData()
    {
        yield return new object[] { 1, 2 };
        yield return new object[] { 2, 1 };
    }

    [Theory]
    [MemberData(nameof(AddCurve_PointsTypesLengthMismatch_TestData))]
    public void Ctor_PointsTypesLengthMismatch_ThrowsArgumentException(int pointsLength, int typesLength)
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new GraphicsPath(new Point[pointsLength], new byte[typesLength]));
        AssertExtensions.Throws<ArgumentException>(null, () => new GraphicsPath(new PointF[pointsLength], new byte[typesLength]));
    }

    [Fact]
    public void Clone_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        Assert.Equal(FillMode.Alternate, clone.FillMode);
        AssertEmptyGraphicsPath(clone);
    }

    [Fact]
    public void Reset_Success()
    {
        using GraphicsPath gp = new();
        gp.Reset();

        Assert.Equal(FillMode.Alternate, gp.FillMode);
        AssertEmptyGraphicsPath(gp);
    }

    [Fact]
    public void GraphicsPath_FillModeChange()
    {
        using GraphicsPath gp = new();
        gp.FillMode = FillMode.Winding;
        Assert.Equal(FillMode.Winding, gp.FillMode);
    }

    [Theory]
    [InlineData(FillMode.Alternate - 1)]
    [InlineData(FillMode.Winding + 1)]
    public void GraphicsPath_InvalidFillMode_ThrowsInvalidEnumArgumentException(FillMode fillMode)
    {
        using GraphicsPath gp = new();
        Assert.ThrowsAny<ArgumentException>(() => gp.FillMode = fillMode);
    }

    [Fact]
    public void PathData_ReturnsExpected()
    {
        using GraphicsPath gp = new();
        Assert.Empty(gp.PathData.Points);
        Assert.Empty(gp.PathData.Types);
    }

    [Fact]
    public void PathData_CannotChange()
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(1, 1, 2, 2));
        Assert.Equal(1f, gp.PathData.Points[0].X);
        Assert.Equal(1f, gp.PathData.Points[0].Y);

        gp.PathData.Points[0] = new Point(0, 0);
        Assert.Equal(1f, gp.PathData.Points[0].X);
        Assert.Equal(1f, gp.PathData.Points[0].Y);
    }

    [Fact]
    public void PathPoints_CannotChange()
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(1, 1, 2, 2));
        Assert.Equal(1f, gp.PathPoints[0].X);
        Assert.Equal(1f, gp.PathPoints[0].Y);

        gp.PathPoints[0] = new Point(0, 0);
        Assert.Equal(1f, gp.PathPoints[0].X);
        Assert.Equal(1f, gp.PathPoints[0].Y);
    }

    [Fact]
    public void PathPoints_EmptyPath()
    {
        using GraphicsPath gp = new();
        gp.PathPoints.Should().BeEmpty();
    }

    [Fact]
    public void PathTypes_CannotChange()
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(1, 1, 2, 2));
        Assert.Equal(0, gp.PathTypes[0]);

        gp.PathTypes[0] = 1;
        Assert.Equal(0, gp.PathTypes[0]);
    }

    [Fact]
    public void PathTypes_EmptyPath_ReturnsEmptyArray()
    {
        using GraphicsPath gp = new();
        gp.PathTypes.Should().BeEmpty();
    }

    [Fact]
    public void GetLastPoint_ReturnsExpected()
    {
        byte[] types = [0, 1, 1];
        PointF[] points = [
            new(1f, 1f), new(2f, 2f), new(3f, 3f),
        ];

        using GraphicsPath gp = new(points, types);
        Assert.Equal(gp.GetLastPoint(), points[2]);
    }

    [Fact]
    public void AddLine_Success()
    {
        using GraphicsPath gpInt = new();
        using GraphicsPath gpFloat = new();
        using GraphicsPath gpPointsInt = new();
        using GraphicsPath gpfPointsloat = new();
        gpInt.AddLine(1, 1, 2, 2);
        // AssertLine() method expects line drawn between points with coordinates 1, 1 and 2, 2, here and below.
        AssertLine(gpInt);

        gpFloat.AddLine(1, 1, 2, 2);
        AssertLine(gpFloat);

        gpPointsInt.AddLine(new Point(1, 1), new Point(2, 2));
        AssertLine(gpPointsInt);

        gpfPointsloat.AddLine(new PointF(1, 1), new PointF(2, 2));
        AssertLine(gpfPointsloat);
    }

    [Fact]
    public void AddLine_SamePoints_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddLine(new Point(49, 157), new Point(75, 196));
        gpi.AddLine(new Point(75, 196), new Point(102, 209));
        Assert.Equal(3, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1 }, gpi.PathTypes);

        gpi.AddLine(new Point(102, 209), new Point(75, 196));
        Assert.Equal(4, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 1 }, gpi.PathTypes);

        gpf.AddLine(new PointF(49, 157), new PointF(75, 196));
        gpf.AddLine(new PointF(75, 196), new PointF(102, 209));
        Assert.Equal(3, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1 }, gpf.PathTypes);

        gpf.AddLine(new PointF(102, 209), new PointF(75, 196));
        Assert.Equal(4, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 1 }, gpf.PathTypes);
    }

    [Fact]
    public void AddLines_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddLines([new(1, 1), new(2, 2)]);
        AssertLine(gpi);

        gpf.AddLines(new PointF[] { new(1, 1), new(2, 2) });
        AssertLine(gpf);
    }

    [Fact]
    public void AddLines_SinglePoint_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddLines(new PointF[] { new(1, 1) });
        Assert.Equal(1, gpi.PointCount);
        Assert.Equal(0, gpi.PathTypes[0]);

        gpf.AddLines(new PointF[] { new(1, 1) });
        Assert.Equal(1, gpf.PointCount);
        Assert.Equal(0, gpf.PathTypes[0]);
    }

    [Fact]
    public void AddLines_SamePoint_Success()
    {
        Point[] intPoints = [
            new(49, 157), new(49, 157)
        ];

        PointF[] floatPoints = [
            new(49, 57), new(49, 57),
            new(49, 57), new(49, 57)
        ];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddLines(intPoints);
        Assert.Equal(2, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1 }, gpi.PathTypes);

        gpi.AddLines(intPoints);
        Assert.Equal(3, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1 }, gpi.PathTypes);

        gpi.AddLines(intPoints);
        Assert.Equal(4, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 1 }, gpi.PathTypes);

        gpf.AddLines(floatPoints);
        Assert.Equal(4, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 1 }, gpf.PathTypes);

        gpf.AddLines(floatPoints);
        Assert.Equal(7, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 1, 1, 1, 1 }, gpf.PathTypes);
    }

    [Fact]
    public void AddLines_PointsNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("points", () => new GraphicsPath().AddLines((Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => new GraphicsPath().AddLines((PointF[])null));
    }

    [Fact]
    public void AddLines_ZeroPoints_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>("points", null, () => new GraphicsPath().AddLines(Array.Empty<Point>()));
        AssertExtensions.Throws<ArgumentException>("points", null, () => new GraphicsPath().AddLines(Array.Empty<PointF>()));
    }

    [Fact]
    public void AddArc_Values_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddArc(1, 1, 2, 2, Pi4, Pi4);
        // AssertArc() method expects added Arc with parameters
        // x=1, y=1, width=2, height=2, startAngle=Pi4, seewpAngle=Pi4 here and below.
        AssertArc(gpi);

        gpf.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
        AssertArc(gpf);
    }

    [Fact]
    public void AddArc_Rectangle_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddArc(new Rectangle(1, 1, 2, 2), Pi4, Pi4);
        AssertArc(gpi);

        gpf.AddArc(new RectangleF(1, 1, 2, 2), Pi4, Pi4);
        AssertArc(gpf);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void AddArc_ZeroWidthHeight_ThrowsArgumentException(int width, int height)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddArc(1, 1, width, height, Pi4, Pi4));
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddArc(1.0f, 1.0f, width, height, Pi4, Pi4));
    }

    [Fact]
    public void AddBezier_Points_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddBezier(new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4));
        // AssertBezier() method expects added Bezier with points (1, 1), (2, 2), (3, 3), (4, 4), here and below.
        AssertBezier(gpi);

        gpf.AddBezier(new PointF(1, 1), new PointF(2, 2), new PointF(3, 3), new PointF(4, 4));
        AssertBezier(gpf);
    }

    [Fact]
    public void AddBezier_SamePoints_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath gpf = new();
        gp.AddBezier(new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0));
        Assert.Equal(4, gp.PointCount);
        Assert.Equal(new byte[] { 0, 3, 3, 3 }, gp.PathTypes);

        gp.AddBezier(new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0));
        Assert.Equal(7, gp.PointCount);
        Assert.Equal(new byte[] { 0, 3, 3, 3, 3, 3, 3 }, gp.PathTypes);

        gpf.AddBezier(new PointF(0, 0), new PointF(0, 0), new PointF(0, 0), new PointF(0, 0));
        Assert.Equal(4, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 3, 3, 3 }, gpf.PathTypes);

        gpf.AddBezier(new PointF(0, 0), new PointF(0, 0), new PointF(0, 0), new PointF(0, 0));
        Assert.Equal(7, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 3, 3, 3, 3, 3, 3 }, gpf.PathTypes);
    }

    [Fact]
    public void AddBezier_Values_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddBezier(1, 1, 2, 2, 3, 3, 4, 4);
        AssertBezier(gpi);

        gpf.AddBezier(1f, 1f, 2f, 2f, 3f, 3f, 4f, 4f);
        AssertBezier(gpf);
    }

    [Fact]
    public void AddBeziers_Points_Success()
    {
        PointF[] points = [
            new(1, 1), new(2, 2), new(3, 3), new(4, 4)
        ];

        using GraphicsPath gpf = new();
        gpf.AddBeziers(points);
        AssertBezier(gpf);
    }

    [Fact]
    public void AddBeziers_PointsNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("points", () => gp.AddBeziers((PointF[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => gp.AddBeziers((Point[])null));
    }

    public static IEnumerable<object[]> AddBeziers_InvalidFloatPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<PointF>() };
        yield return new object[] { new PointF[1] { new(1f, 1f) } };
        yield return new object[] { new PointF[2] { new(1f, 1f), new(2f, 2f) } };
        yield return new object[] { new PointF[3] { new(1f, 1f), new(2f, 2f), new(3f, 3f) } };
    }

    [Theory]
    [MemberData(nameof(AddBeziers_InvalidFloatPointsLength_TestData))]
    public void AddBeziers_InvalidFloatPointsLength_ThrowsArgumentException(PointF[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddBeziers(points));
    }

    [Fact]
    public void AddCurve_TwoPoints_Success()
    {
        Point[] intPoints = [new(1, 1), new(2, 2)];
        PointF[] floatPoints = [new(1, 1), new(2, 2)];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpf.AddCurve(floatPoints);
        // AssertCurve() method expects added Curve with points (1, 1), (2, 2), here and below.
        AssertCurve(gpf);

        gpi.AddCurve(intPoints);
        AssertCurve(gpi);
    }

    [Fact]
    public void AddCurve_TwoPointsWithTension_Success()
    {
        Point[] intPoints = [new(1, 1), new(2, 2)];
        PointF[] floatPoints = [new(1, 1), new(2, 2)];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddCurve(intPoints, 0.5f);
        AssertCurve(gpi);

        gpf.AddCurve(floatPoints, 0.5f);
        AssertCurve(gpf);
    }

    [Fact]
    public void AddCurve_SamePoints_Success()
    {
        Point[] intPoints = [new(1, 1), new(1, 1)];
        PointF[] floatPoints = [new(1, 1), new(1, 1)];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddCurve(intPoints);
        Assert.Equal(4, gpi.PointCount);
        gpi.AddCurve(intPoints);
        Assert.Equal(7, gpi.PointCount);

        gpf.AddCurve(floatPoints);
        Assert.Equal(4, gpf.PointCount);
        gpf.AddCurve(floatPoints);
        Assert.Equal(7, gpf.PointCount);
    }

    [Fact]
    public void AddCurve_LargeTension_Success()
    {
        Point[] intPoints = [new(1, 1), new(2, 2)];
        PointF[] floatPoints = [new(1, 1), new(2, 2)];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddCurve(intPoints, float.MaxValue);
        Assert.Equal(4, gpi.PointCount);

        gpf.AddCurve(floatPoints, float.MaxValue);
        Assert.Equal(4, gpf.PointCount);
    }

    [Fact]
    public void AddCurve_Success()
    {
        PointF[] points =
        [
            new PointF (37f, 185f),
            new PointF (99f, 185f),
            new PointF (161f, 159f),
            new PointF (223f, 185f),
            new PointF (285f, 54f),
        ];

        PointF[] expectedPoints =
        [
            new PointF (37f, 185f),
            new PointF (47.33333f, 185f),
            new PointF (78.3333f, 189.3333f),
            new PointF (99f, 185f),
            new PointF (119.6667f, 180.6667f),
            new PointF (140.3333f, 159f),
            new PointF (161f, 159f),
            new PointF (181.6667f, 159f),
            new PointF (202.3333f, 202.5f),
            new PointF (223f, 185f),
            new PointF (243.6667f, 167.5f),
            new PointF (274.6667f, 75.8333f),
            new PointF (285f, 54f),
        ];

        byte[] expectedTypes = [0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3];
        int[] pointsCount = [4, 7, 10, 13];
        using GraphicsPath gp = new();
        for (int i = 0; i < points.Length - 1; i++)
        {
            gp.AddCurve(points, i, 1, 0.5f);
            Assert.Equal(pointsCount[i], gp.PointCount);
        }

        gp.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        gp.PathTypes.Should().BeEquivalentTo(expectedTypes);
    }

    [Fact]
    public void AddCurve_PointsNull_ThrowsArgumentException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddCurve((PointF[])null));
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddCurve((Point[])null));
    }

    public static IEnumerable<object[]> AddCurve_InvalidFloatPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<PointF>() };
        yield return new object[] { new PointF[1] { new(1f, 1f) } };
    }

    [Theory]
    [MemberData(nameof(AddCurve_InvalidFloatPointsLength_TestData))]
    public void AddCurve_InvalidFloatPointsLength_ThrowsArgumentException(PointF[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(() => gp.AddCurve(points));
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(() => gp.AddCurve(points, 0, 2, 0.5f));
    }

    public static IEnumerable<object[]> AddCurve_InvalidPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<Point>() };
        yield return new object[] { new Point[1] { new(1, 1) } };
    }

    [Theory]
    [MemberData(nameof(AddCurve_InvalidPointsLength_TestData))]
    public void AddCurve_InvalidPointsLength_ThrowsArgumentException(Point[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(() => gp.AddCurve(points));
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(() => gp.AddCurve(points, 0, 2, 0.5f));
    }

    public static IEnumerable<object[]> AddCurve_InvalidSegment_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { -1 };
    }

    [Theory]
    [MemberData(nameof(AddCurve_InvalidSegment_TestData))]
    public void AddCurve_InvalidSegment_ThrowsArgumentException(int segment)
    {
        using GraphicsPath gp = new();
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(
            () => gp.AddCurve(new PointF[2] { new(1f, 1f), new(2f, 2f) }, 0, segment, 0.5f));

        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(
            () => gp.AddCurve([new(1, 1), new(2, 2)], 0, segment, 0.5f));
    }

    [Fact]
    public void AddCurve_OffsetTooLarge_ThrowsArgumentException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(
            () => gp.AddCurve(new PointF[3] { new(1f, 1f), new(0f, 20f), new(20f, 0f) }, 1, 2, 0.5f));

        AssertExtensions.ThrowsAny<ArgumentException, ArgumentOutOfRangeException>(
            () => gp.AddCurve([new(1, 1), new(0, 20), new(20, 0)], 1, 2, 0.5f));
    }

    [Fact]
    public void AddClosedCurve_Points_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddClosedCurve([new(1, 1), new(2, 2), new(3, 3)]);
        // AssertClosedCurve() method expects added ClosedCurve with points (1, 1), (2, 2), (3, 3), here and below.
        AssertClosedCurve(gpi);

        gpf.AddClosedCurve(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        AssertClosedCurve(gpf);
    }

    [Fact]
    public void AddClosedCurve_SamePoints_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddClosedCurve([new(1, 1), new(1, 1), new(1, 1)]);
        Assert.Equal(10, gpi.PointCount);
        gpi.AddClosedCurve([new(1, 1), new(1, 1), new(1, 1)]);
        Assert.Equal(20, gpi.PointCount);

        gpf.AddClosedCurve(new PointF[3] { new(1, 1), new(1, 1), new(1, 1) });
        Assert.Equal(10, gpf.PointCount);
        gpf.AddClosedCurve(new PointF[3] { new(1, 1), new(1, 1), new(1, 1) });
        Assert.Equal(20, gpf.PointCount);
    }

    [Fact]
    public void AddClosedCurve_Tension_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddClosedCurve([new(1, 1), new(2, 2), new(3, 3)], 0.5f);
        AssertClosedCurve(gpi);

        gpf.AddClosedCurve(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) }, 0.5f);
        AssertClosedCurve(gpf);
    }

    [Fact]
    public void AddClosedCurve_PointsNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("points", () => gp.AddClosedCurve((PointF[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => gp.AddClosedCurve((Point[])null));
    }

    public static IEnumerable<object[]> AddClosedCurve_InvalidPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<Point>() };
        yield return new object[] { new Point[1] { new(1, 1) } };
        yield return new object[] { new Point[2] { new(1, 1), new(2, 2) } };
    }

    [Theory]
    [MemberData(nameof(AddCurve_InvalidPointsLength_TestData))]
    public void AddClosedCurve_InvalidPointsLength_ThrowsArgumentException(Point[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddClosedCurve(points));
    }

    public static IEnumerable<object[]> AddClosedCurve_InvalidFloatPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<PointF>() };
        yield return new object[] { new PointF[1] { new(1f, 1f) } };
        yield return new object[] { new PointF[2] { new(1f, 1f), new(2f, 2f) } };
    }

    [Theory]
    [MemberData(nameof(AddClosedCurve_InvalidFloatPointsLength_TestData))]
    public void AddClosedCurve_InvalidFloatPointsLength_ThrowsArgumentException(PointF[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddClosedCurve(points));
    }

    [Fact]
    public void AddRectangle_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddRectangle(new Rectangle(1, 1, 2, 2));
        // AssertRectangle() method expects added Rectangle with parameters x=1, y=1, width=2, height=2, here and below.
        AssertRectangle(gpi);

        gpf.AddRectangle(new RectangleF(1, 1, 2, 2));
        AssertRectangle(gpf);
    }

    [Fact]
    public void AddRectangle_SameRectangles_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddRectangle(new Rectangle(1, 1, 1, 1));
        Assert.Equal(4, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 129 }, gpi.PathTypes);

        PointF endI = gpi.PathPoints[3];

        gpi.AddRectangle(new Rectangle((int)endI.X, (int)endI.Y, 1, 1));
        Assert.Equal(8, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 129, 0, 1, 1, 129 }, gpi.PathTypes);

        gpf.AddRectangle(new RectangleF(1, 1, 1, 1));
        Assert.Equal(4, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 129 }, gpf.PathTypes);
        Assert.Equal(129, gpf.PathTypes[3]);

        PointF endF = gpf.PathPoints[3];

        gpf.AddRectangle(new RectangleF(endF.X, endF.Y, 1, 1));
        Assert.Equal(8, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 1, 129, 0, 1, 1, 129 }, gpf.PathTypes);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 0)]
    [InlineData(0, 4)]
    public void AddRectangle_ZeroWidthHeight_Success(int width, int height)
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddRectangle(new Rectangle(1, 2, width, height));
        Assert.Empty(gpi.PathData.Points);

        gpf.AddRectangle(new RectangleF(1f, 2f, width, height));
        Assert.Empty(gpf.PathData.Points);
    }

    [Fact]
    public void AddRectangles_Success()
    {
        Rectangle[] rectInt = [new(1, 1, 2, 2), new(3, 3, 4, 4)];
        RectangleF[] rectFloat = [new(1, 1, 2, 2), new(3, 3, 4, 4)];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddRectangles(rectInt);
        Assert.Equal(8, gpi.PathPoints.Length);
        Assert.Equal(8, gpi.PathTypes.Length);
        Assert.Equal(8, gpi.PathData.Points.Length);

        gpf.AddRectangles(rectFloat);
        Assert.Equal(8, gpf.PathPoints.Length);
        Assert.Equal(8, gpf.PathTypes.Length);
        Assert.Equal(8, gpf.PathData.Points.Length);
    }

    [Fact]
    public void AddRectangles_SamePoints_Success()
    {
        Rectangle[] rectInt = [
            new(1, 1, 0, 0),
            new(1, 1, 2, 2),
            new(1, 1, 2, 2)
        ];

        RectangleF[] rectFloat = [
            new(1, 1, 0f, 0f),
            new(1, 1, 2, 2),
            new(1, 1, 2, 2)
        ];

        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddRectangles(rectInt);
        Assert.Equal(8, gpi.PathPoints.Length);
        Assert.Equal(8, gpi.PathTypes.Length);
        Assert.Equal(8, gpi.PathData.Points.Length);

        gpf.AddRectangles(rectFloat);
        Assert.Equal(8, gpf.PathPoints.Length);
        Assert.Equal(8, gpf.PathTypes.Length);
        Assert.Equal(8, gpf.PathData.Points.Length);
    }

    [Fact]
    public void AddRectangles_RectangleNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("rects", () => gp.AddRectangles((RectangleF[])null));
        AssertExtensions.Throws<ArgumentNullException>("rects", () => gp.AddRectangles((Rectangle[])null));
    }

    [Fact]
    public void AddEllipse_Rectangle_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddEllipse(new Rectangle(1, 1, 2, 2));
        // AssertEllipse() method expects added Ellipse with parameters x=1, y=1, width=2, height=2, here and below.
        AssertEllipse(gpi);

        gpf.AddEllipse(new RectangleF(1, 1, 2, 2));
        AssertEllipse(gpf);
    }

    [Fact]
    public void AddEllipse_Values_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddEllipse(1, 1, 2, 2);
        AssertEllipse(gpi);

        gpf.AddEllipse(1f, 1f, 2f, 2f);
        AssertEllipse(gpf);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(0, 2)]
    public void AddEllipse_ZeroWidthHeight_Success(int width, int height)
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddEllipse(1, 1, width, height);
        Assert.Equal(13, gpi.PathData.Points.Length);

        gpf.AddEllipse(1f, 2f, width, height);
        Assert.Equal(13, gpf.PathData.Points.Length);
    }

    [Fact]
    public void AddPie_Rectangle_Success()
    {
        using GraphicsPath gpi = new();
        gpi.AddPie(new Rectangle(1, 1, 2, 2), Pi4, Pi4);
        // AssertPie() method expects added Pie with parameters
        // x=1, y=1, width=2, height=2, startAngle=Pi4, seewpAngle=Pi4 here and below.
        AssertPie(gpi);
    }

    [Fact]
    public void AddPie_Values_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddPie(1, 1, 2, 2, Pi4, Pi4);
        AssertPie(gpi);

        gpf.AddPie(1f, 1f, 2f, 2f, Pi4, Pi4);
        AssertPie(gpf);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(0, 2)]
    public void AddPie_ZeroWidthHeight_ThrowsArgumentException(int width, int height)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddPie(1, 1, height, width, Pi4, Pi4));
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddPie(1f, 1f, height, width, Pi4, Pi4));
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddPie(new Rectangle(1, 1, height, width), Pi4, Pi4));
    }

    [Fact]
    public void AddPolygon_Points_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        // AssertPolygon() method expects added Polygon with points (1, 1), (2, 2), (3, 3), here and below.
        AssertPolygon(gpi);

        gpf.AddPolygon(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        AssertPolygon(gpf);
    }

    [Fact]
    public void AddPolygon_SamePoints_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        Assert.Equal(3, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129 }, gpi.PathTypes);

        gpi.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        Assert.Equal(6, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129 }, gpi.PathTypes);

        gpi.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        Assert.Equal(9, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129, 0, 1, 129 }, gpi.PathTypes);

        gpi.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        Assert.Equal(12, gpi.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129, 0, 1, 129, 0, 1, 129 }, gpi.PathTypes);

        gpf.AddPolygon(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        Assert.Equal(3, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129 }, gpf.PathTypes);

        gpf.AddPolygon(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        Assert.Equal(6, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129 }, gpf.PathTypes);

        gpf.AddPolygon(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        Assert.Equal(9, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129, 0, 1, 129 }, gpf.PathTypes);

        gpf.AddPolygon(new PointF[3] { new(1, 1), new(2, 2), new(3, 3) });
        Assert.Equal(12, gpf.PointCount);
        Assert.Equal(new byte[] { 0, 1, 129, 0, 1, 129, 0, 1, 129, 0, 1, 129 }, gpf.PathTypes);
    }

    [Fact]
    public void AddPolygon_PointsNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("points", () => new GraphicsPath().AddPolygon((Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => new GraphicsPath().AddPolygon((PointF[])null));
    }

    public static IEnumerable<object[]> AddPolygon_InvalidFloadPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<PointF>() };
        yield return new object[] { new PointF[1] { new(1f, 1f) } };
        yield return new object[] { new PointF[2] { new(1f, 1f), new(2f, 2f) } };
    }

    [Theory]
    [MemberData(nameof(AddPolygon_InvalidFloadPointsLength_TestData))]
    public void AddPolygon_InvalidFloadPointsLength_ThrowsArgumentException(PointF[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddPolygon(points));
    }

    public static IEnumerable<object[]> AddPolygon_InvalidPointsLength_TestData()
    {
        yield return new object[] { Array.Empty<Point>() };
        yield return new object[] { new Point[1] { new(1, 1) } };
        yield return new object[] { new Point[2] { new(1, 1), new(2, 2) } };
    }

    [Theory]
    [MemberData(nameof(AddPolygon_InvalidPointsLength_TestData))]
    public void AddPolygon_InvalidPointsLength_ThrowsArgumentException(Point[] points)
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => gp.AddPolygon(points));
    }

    [Fact]
    public void AddPath_Success()
    {
        using GraphicsPath inner = new();
        using GraphicsPath gp = new();
        inner.AddRectangle(new Rectangle(1, 1, 2, 2));
        gp.AddPath(inner, true);
        AssertRectangle(gp);
    }

    [Fact]
    public void AddPath_PathNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("addingPath", () => new GraphicsPath().AddPath(null, false));
    }

    [Fact]
    public void AddString_Point_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddString("mono", FontFamily.GenericMonospace, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpi.PointCount, 0);

        gpf.AddString("mono", FontFamily.GenericMonospace, 0, 10, new PointF(10f, 10f), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpf.PointCount, 0);
    }

    [Fact]
    public void AddString_Rectangle_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddString("mono", FontFamily.GenericMonospace, 0, 10, new Rectangle(10, 10, 10, 10), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpi.PointCount, 0);

        gpf.AddString("mono", FontFamily.GenericMonospace, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpf.PointCount, 0);
    }

    [Fact]
    public void AddString_NegativeSize_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddString("mono", FontFamily.GenericMonospace, 0, -10, new Point(10, 10), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpi.PointCount, 0);

        int gpiLengthOld = gpi.PathPoints.Length;
        gpi.AddString("mono", FontFamily.GenericMonospace, 0, -10, new Rectangle(10, 10, 10, 10), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpi.PointCount, gpiLengthOld);

        gpf.AddString("mono", FontFamily.GenericMonospace, 0, -10, new PointF(10f, 10f), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpf.PointCount, 0);

        int pgfLengthOld = gpf.PathPoints.Length;
        gpf.AddString("mono", FontFamily.GenericMonospace, 0, -10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault);
        AssertExtensions.GreaterThan(gpf.PointCount, pgfLengthOld);
    }

    [Fact]
    public void AddString_StringFormat_Success()
    {
        using GraphicsPath gp1 = new();
        using GraphicsPath gp2 = new();
        using GraphicsPath gp3 = new();
        gp1.AddString("mono", FontFamily.GenericMonospace, 0, 10, new RectangleF(10f, 10f, 10f, 10f), null);
        AssertExtensions.GreaterThan(gp1.PointCount, 0);

        gp2.AddString("mono", FontFamily.GenericMonospace, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault);
        Assert.Equal(gp1.PointCount, gp2.PointCount);

        gp3.AddString("mono", FontFamily.GenericMonospace, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericTypographic);
        Assert.NotEqual(gp1.PointCount, gp3.PointCount);
    }

    [Fact]
    public void AddString_EmptyString_Success()
    {
        using GraphicsPath gpi = new();
        using GraphicsPath gpf = new();
        gpi.AddString(string.Empty, FontFamily.GenericMonospace, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
        Assert.Equal(0, gpi.PointCount);

        gpi.AddString(string.Empty, FontFamily.GenericMonospace, 0, 10, new PointF(10f, 10f), StringFormat.GenericDefault);
        Assert.Equal(0, gpf.PointCount);
    }

    [Fact]
    public void AddString_StringNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        Assert.Throws<ArgumentNullException>(() =>
            gp.AddString(null, FontFamily.GenericMonospace, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
        Assert.Throws<ArgumentNullException>(() =>
            gp.AddString(null, FontFamily.GenericMonospace, 0, 10, new PointF(10f, 10f), StringFormat.GenericDefault));
        Assert.Throws<ArgumentNullException>(() =>
            gp.AddString(null, FontFamily.GenericMonospace, 0, 10, new Rectangle(10, 10, 10, 10), StringFormat.GenericDefault));
        Assert.Throws<ArgumentNullException>(() =>
            gp.AddString(null, FontFamily.GenericMonospace, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault));
    }

    [Fact]
    public void AddString_FontFamilyNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException, ArgumentException>("family", null, () =>
            new GraphicsPath().AddString("mono", null, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
    }

    [Fact]
    public void Transform_Success()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new(1f, 1f, 2f, 2f, 3f, 3f);
        gp.AddRectangle(new Rectangle(1, 1, 2, 2));
        AssertRectangle(gp);
        gp.Transform(matrix);
        Assert.Equal([1f, 1f, 2f, 2f, 3f, 3f], matrix.Elements);
        Assert.Equal(new RectangleF(6f, 6f, 6f, 6f), gp.GetBounds());
        Assert.Equal([new(6f, 6f), new(8f, 8f), new(12f, 12f), new(10f, 10f)], gp.PathPoints);
        Assert.Equal(new byte[] { 0, 1, 1, 129 }, gp.PathTypes);
    }

    [Fact]
    public void Transform_PathEmpty_Success()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new(1f, 1f, 2f, 2f, 3f, 3f);
        gp.Transform(matrix);
        Assert.Equal([1f, 1f, 2f, 2f, 3f, 3f], matrix.Elements);
        AssertEmptyGraphicsPath(gp);
    }

    [Fact]
    public void Transform_MatrixNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => gp.Transform(null));
    }

    [Fact]
    public void GetBounds_PathEmpty_ReturnsExpected()
    {
        using GraphicsPath gp = new();
        Assert.Equal(new RectangleF(0f, 0f, 0f, 0f), gp.GetBounds());
    }

    [Fact]
    public void GetBounds_Rectangle_ReturnsExpected()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new();
        RectangleF rectangle = new(1f, 1f, 2f, 2f);
        gp.AddRectangle(rectangle);
        Assert.Equal(rectangle, gp.GetBounds());
        Assert.Equal(rectangle, gp.GetBounds(null));
        Assert.Equal(rectangle, gp.GetBounds(matrix));
        Assert.Equal(rectangle, gp.GetBounds(null, null));
    }

    [Fact]
    public void GetBounds_Pie_ReturnsExpected()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new();
        Rectangle rectangle = new(10, 10, 100, 100);
        gp.AddPie(rectangle, 30, 45);
        gp.GetBounds().Should().BeApproximately(new(60f, 60f, 43.3f, 48.3f), precision: 0.1f);
    }

    [Fact]
    public void Flatten_Empty_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.Flatten();
        Assert.Equal(gp.PointCount, clone.PointCount);
    }

    [Fact]
    public void Flatten_MatrixNull_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.Flatten(null);
        Assert.Equal(gp.PointCount, clone.PointCount);
    }

    [Fact]
    public void Flatten_MatrixNullFloat_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.Flatten(null, 1f);
        Assert.Equal(gp.PointCount, clone.PointCount);
    }

    [Fact]
    public void Flatten_Arc_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddArc(0f, 0f, 100f, 100f, 30, 30);
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Bezier_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddBezier(0, 0, 100, 100, 30, 30, 60, 60);
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_ClosedCurve_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddClosedCurve(
        [
            new(0, 0), new(40, 20),
            new(20, 40), new(40, 40)
        ]);

        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Curve_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddCurve(
        [
            new(0, 0), new(40, 20),
            new(20, 40), new(40, 40)
        ]);

        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Ellipse_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddEllipse(10f, 10f, 100f, 100f);
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Line_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddLine(10f, 10f, 100f, 100f);
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Pie_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddPie(0, 0, 100, 100, 30, 30);
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Polygon_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddPolygon(
        [
            new(0, 0), new(10, 10),
            new(20, 20), new(40, 40)
        ]);

        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Flatten_Rectangle_Success()
    {
        using GraphicsPath gp = new();
        using GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone());
        gp.AddRectangle(new Rectangle(0, 0, 100, 100));
        gp.Flatten();
        AssertFlats(gp, clone);
    }

    [Fact]
    public void Warp_DestinationPointsNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("destPoints", () => gp.Warp(null, default));
    }

    [Fact]
    public void Warp_DestinationPointsZero_ThrowsArgumentException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentException>(null, () => new GraphicsPath().Warp([], default));
    }

    [Fact]
    public void Warp_PathEmpty_Success()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new();
        Assert.Equal(0, gp.PointCount);
        gp.Warp([new(0, 0)], new RectangleF(10, 20, 30, 40), matrix);
        Assert.Equal(0, gp.PointCount);
    }

    [Fact]
    public void Warp_WarpModeInvalid_Success()
    {
        using GraphicsPath gp = new();
        using Matrix matrix = new();
        gp.AddPolygon([new(5, 5), new(15, 5), new(10, 15)]);
        gp.Warp([new(0, 0)], new RectangleF(10, 20, 30, 40), matrix, (WarpMode)int.MinValue);
        Assert.Equal(0, gp.PointCount);
    }

    [Fact]
    public void Warp_RectangleEmpty_Success()
    {
        using GraphicsPath gp = new();
        gp.AddPolygon([new(5, 5), new(15, 5), new(10, 15)]);
        gp.Warp([new(0, 0)], default(Rectangle), null);
        AssertWrapNaN(gp);
    }

    [Fact]
    public void SetMarkers_EmptyPath_Success()
    {
        using GraphicsPath gp = new();
        gp.SetMarkers();
    }

    [Fact]
    public void SetMarkers_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(new Point(1, 1), new Point(2, 2));
        Assert.Equal(1, gp.PathTypes[1]);

        gp.SetMarkers();
        Assert.Equal(33, gp.PathTypes[1]);
    }

    [Fact]
    public void ClearMarkers_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(new Point(1, 1), new Point(2, 2));
        Assert.Equal(1, gp.PathTypes[1]);

        gp.SetMarkers();
        Assert.Equal(33, gp.PathTypes[1]);

        gp.ClearMarkers();
        Assert.Equal(1, gp.PathTypes[1]);
    }

    [Fact]
    public void ClearMarkers_EmptyPath_Success()
    {
        using GraphicsPath gp = new();
        gp.ClearMarkers();
    }

    [Fact]
    public void CloseFigure_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(new Point(1, 1), new Point(2, 2));
        Assert.Equal(1, gp.PathTypes[1]);

        gp.CloseFigure();
        Assert.Equal(129, gp.PathTypes[1]);
    }

    [Fact]
    public void CloseFigure_EmptyPath_Success()
    {
        using GraphicsPath gp = new();
        gp.CloseFigure();
    }

    [Fact]
    public void CloseAllFigures_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(new Point(1, 1), new Point(2, 2));
        gp.StartFigure();
        gp.AddLine(new Point(3, 3), new Point(4, 4));
        Assert.Equal(1, gp.PathTypes[1]);
        Assert.Equal(1, gp.PathTypes[3]);

        gp.CloseAllFigures();
        Assert.Equal(129, gp.PathTypes[1]);
        Assert.Equal(129, gp.PathTypes[3]);
    }

    [Fact]
    public void CloseAllFigures_EmptyPath_Success()
    {
        using GraphicsPath gp = new();
        gp.CloseAllFigures();
    }

    [Fact]
    public void StartClose_AddArc()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddArc(10, 10, 100, 100, 90, 180);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(3, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddBezier()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddBezier(10, 10, 100, 100, 20, 20, 200, 200);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(3, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddBeziers()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddBeziers(
        [
            new(10, 10), new(20, 10), new(20, 20),
            new(30, 20), new(40, 40), new(50, 40),
            new(50, 50)
        ]);

        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(3, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddClosedCurve()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddClosedCurve([new(1, 1), new(2, 2), new(3, 3)]);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(131, types[gp.PointCount - 3]);
        Assert.Equal(0, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddCurve()
    {
        using GraphicsPath path = new();
        path.AddLine(1, 1, 2, 2);
        path.AddCurve([new(1, 1), new(2, 2), new(3, 3)]);
        path.AddLine(10, 10, 20, 20);
        byte[] types = path.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(3, types[path.PointCount - 3]);
        Assert.Equal(1, types[path.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddEllipse()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddEllipse(10, 10, 100, 100);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(131, types[gp.PointCount - 3]);
        Assert.Equal(0, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddLine()
    {
        using GraphicsPath path = new();
        path.AddLine(1, 1, 2, 2);
        path.AddLine(5, 5, 10, 10);
        path.AddLine(10, 10, 20, 20);
        byte[] types = path.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(1, types[path.PointCount - 3]);
        Assert.Equal(1, types[path.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddLines()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddLines([new(10, 10), new(20, 10), new(20, 20), new(30, 20)]);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(1, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddPath_Connect()
    {
        using GraphicsPath gp = new();
        using GraphicsPath inner = new();
        inner.AddArc(10, 10, 100, 100, 90, 180);
        gp.AddLine(1, 1, 2, 2);
        gp.AddPath(inner, true);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(1, types[2]);
        Assert.Equal(3, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddPath_NoConnect()
    {
        using GraphicsPath inner = new();
        using GraphicsPath path = new();
        inner.AddArc(10, 10, 100, 100, 90, 180);
        path.AddLine(1, 1, 2, 2);
        path.AddPath(inner, false);
        path.AddLine(10, 10, 20, 20);
        byte[] types = path.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(3, types[path.PointCount - 3]);
        Assert.Equal(1, types[path.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddPie()
    {
        using GraphicsPath path = new();
        path.AddLine(1, 1, 2, 2);
        path.AddPie(10, 10, 10, 10, 90, 180);
        path.AddLine(10, 10, 20, 20);
        byte[] types = path.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);

        Assert.Equal(128, (types[path.PointCount - 3] & 128));
        Assert.Equal(0, types[path.PointCount - 2]);
        Assert.Equal(1, types[path.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddPolygon()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddPolygon([new(1, 1), new(2, 2), new(3, 3)]);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(129, types[gp.PointCount - 3]);
        Assert.Equal(0, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddRectangle()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddRectangle(new RectangleF(10, 10, 20, 20));
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(129, types[gp.PointCount - 3]);
        Assert.Equal(0, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddRectangles()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddRectangles(new RectangleF[2]
        {
            new(10, 10, 20, 20),
            new(20, 20, 10, 10)
        });

        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(129, types[gp.PointCount - 3]);
        Assert.Equal(0, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void StartClose_AddString()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 1, 2, 2);
        gp.AddString("mono", FontFamily.GenericMonospace, 0, 10, new Point(20, 20), StringFormat.GenericDefault);
        gp.AddLine(10, 10, 20, 20);
        byte[] types = gp.PathTypes;

        Assert.Equal(0, types[0]);
        Assert.Equal(0, types[2]);
        Assert.Equal(163, types[gp.PointCount - 3]);
        Assert.Equal(1, types[gp.PointCount - 2]);
        Assert.Equal(1, types[gp.PointCount - 1]);
    }

    [Fact]
    public void Widen_Pen_Success()
    {
        PointF[] expectedPoints =
        [
            new PointF(0.5f, 0.5f), new PointF(3.5f, 0.5f), new PointF(3.5f, 3.5f),
            new PointF(0.5f, 3.5f), new PointF(1.5f, 3.0f), new PointF(1.0f, 2.5f),
            new PointF(3.0f, 2.5f), new PointF(2.5f, 3.0f), new PointF(2.5f, 1.0f),
            new PointF(3.0f, 1.5f), new PointF(1.0f, 1.5f), new PointF(1.5f, 1.0f),
        ];

        byte[] expectedTypes = [0, 1, 1, 129, 0, 1, 1, 1, 1, 1, 1, 129];

        using GraphicsPath gp = new();
        using Pen pen = new(Color.Blue);
        gp.AddRectangle(new Rectangle(1, 1, 2, 2));
        gp.PointCount.Should().Be(4);
        gp.Widen(pen);
        gp.PointCount.Should().Be(12);
        gp.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        gp.PathTypes.Should().BeEquivalentTo(expectedTypes);
    }

    [Fact]
    public void Widen_EmptyPath_Success()
    {
        using GraphicsPath gp = new();
        using Pen pen = new(Color.Blue);
        Assert.Equal(0, gp.PointCount);
        gp.Widen(pen);
        Assert.Equal(0, gp.PointCount);
    }

    [Fact]
    public void Widen_PenNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.Widen(null));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.Widen(null, new Matrix()));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.Widen(null, new Matrix(), 0.67f));
    }

    [Fact]
    public void Widen_MatrixNull_Success()
    {
        using GraphicsPath gp = new();
        using Pen pen = new(Color.Blue);
        gp.AddPolygon([new(5, 5), new(15, 5), new(10, 15)]);
        gp.Widen(pen, null);
        Assert.Equal(9, gp.PointCount);
        AssertWiden3(gp);
    }

    [Fact]
    public void Widen_MatrixEmpty_Success()
    {
        using GraphicsPath gp = new();
        using Pen pen = new(Color.Blue);
        using Matrix matrix = new();
        gp.AddPolygon([new(5, 5), new(15, 5), new(10, 15)]);
        gp.Widen(pen, new Matrix());
        Assert.Equal(9, gp.PointCount);
        AssertWiden3(gp);
    }

    public static IEnumerable<object[]> Widen_PenSmallWidth_TestData()
    {
        yield return new object[] { new Rectangle(1, 1, 2, 2), 0f, new RectangleF(0.5f, 0.5f, 3.0f, 3.0f) };
        yield return new object[] { new Rectangle(1, 1, 2, 2), 0.5f, new RectangleF(0.5f, 0.5f, 3.0f, 3.0f) };
        yield return new object[] { new Rectangle(1, 1, 2, 2), 1.0f, new RectangleF(0.5f, 0.5f, 3.0f, 3.0f) };
        yield return new object[] { new Rectangle(1, 1, 2, 2), 1.1f, new RectangleF(0.45f, 0.45f, 3.10f, 3.10f) };
    }

    [Theory]
    [MemberData(nameof(Widen_PenSmallWidth_TestData))]
    public void Widen_Pen_SmallWidth_Success(
        Rectangle rectangle, float penWidth, RectangleF expectedBounds)
    {
        using GraphicsPath gp = new();
        using Pen pen = new(Color.Aqua, 0);
        using Matrix matrix = new();
        pen.Width = penWidth;
        gp.AddRectangle(rectangle);
        gp.Widen(pen);
        gp.GetBounds(null).Should().BeApproximately(expectedBounds, Delta);
        gp.GetBounds(matrix).Should().BeApproximately(expectedBounds, Delta);
    }

    [Fact]
    public void IsOutlineVisible_PenNull_ThrowsArgumentNullException()
    {
        using GraphicsPath gp = new();
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.IsOutlineVisible(1, 1, null));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.IsOutlineVisible(1.0f, 1.0f, null));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.IsOutlineVisible(default, null));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => gp.IsOutlineVisible(default(PointF), null));
    }

    [Fact]
    public void IsOutlineVisible_LineWithoutGraphics_ReturnsExpected()
    {
        AssertIsOutlineVisibleLine(null);
    }

    [Fact]
    public void IsOutlineVisible_LineInsideGraphics_ReturnsExpected()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        AssertIsOutlineVisibleLine(graphics);
    }

    [Fact]
    public void IsOutlineVisible_LineOutsideGraphics_ReturnsExpected()
    {
        using Bitmap bitmap = new(5, 5);
        using Graphics graphics = Graphics.FromImage(bitmap);
        AssertIsOutlineVisibleLine(graphics);
    }

    [Fact]
    public void IsOutlineVisible_LineWithGraphicsTransform_ReturnsExpected()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Matrix matrix = new(2, 0, 0, 2, 50, -50);
        graphics.Transform = matrix;
        AssertIsOutlineVisibleLine(graphics);
    }

    [Fact]
    public void IsOutlineVisible_LineWithGraphicsPageUnit_ReturnsExpected()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.PageUnit = GraphicsUnit.Millimeter;
        AssertIsOutlineVisibleLine(graphics);
    }

    [Fact]
    public void IsOutlineVisible_LineWithGraphicsPageScale_ReturnsExpected()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.PageScale = 2.0f;
        AssertIsOutlineVisibleLine(graphics);
    }

    [Fact]
    public void IsOutlineVisible_RectangleWithoutGraphics_ReturnsExpected()
    {
        AssertIsOutlineVisibleRectangle(null);
    }

    [Fact]
    public void IsVisible_RectangleWithoutGraphics_ReturnsExpected()
    {
        AssertIsVisibleRectangle(null);
    }

    [Fact]
    public void IsVisible_RectangleWithGraphics_ReturnsExpected()
    {
        using Bitmap bitmap = new(40, 40);
        using Graphics graphics = Graphics.FromImage(bitmap);
        AssertIsVisibleRectangle(graphics);
    }

    [Fact]
    public void IsVisible_EllipseWithoutGraphics_ReturnsExpected()
    {
        AssertIsVisibleEllipse(null);
    }

    [Fact]
    public void IsVisible_EllipseWithGraphics_ReturnsExpected()
    {
        using Bitmap bitmap = new(40, 40);
        using Graphics graphics = Graphics.FromImage(bitmap);
        AssertIsVisibleEllipse(graphics);
    }

    [Fact]
    public void Reverse_Arc_Success()
    {
        using GraphicsPath gp = new();
        gp.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Bezier_Success()
    {
        using GraphicsPath gp = new();
        gp.AddBezier(1, 2, 3, 4, 5, 6, 7, 8);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    public static IEnumerable<object[]> Reverse_TestData()
    {
        yield return new object[]
        {
            new Point[]
            {
                new(1, 2), new(3, 4), new(5, 6), new(7, 8),
                new(9, 10), new(11, 12), new(13, 14)
            }
        };
    }

    [Theory]
    [MemberData(nameof(Reverse_TestData))]
    public void Reverse_Beziers_Success(Point[] points)
    {
        using GraphicsPath gp = new();
        gp.AddBeziers(points);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Theory]
    [MemberData(nameof(Reverse_TestData))]
    public void Reverse_ClosedCurve_Success(Point[] points)
    {
        using GraphicsPath gp = new();
        gp.AddClosedCurve(points);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Theory]
    [MemberData(nameof(Reverse_TestData))]
    public void Reverse_Curve_Success(Point[] points)
    {
        using GraphicsPath gp = new();
        gp.AddCurve(points);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Ellipse_Success()
    {
        using GraphicsPath gp = new();
        gp.AddEllipse(1, 2, 3, 4);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Line_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 2, 3, 4);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_LineClosed_Success()
    {
        using GraphicsPath gp = new();
        gp.AddLine(1, 2, 3, 4);
        gp.CloseFigure();
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Theory]
    [MemberData(nameof(Reverse_TestData))]
    public void Reverse_Lines_Success(Point[] points)
    {
        using GraphicsPath gp = new();
        gp.AddLines(points);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Theory]
    [MemberData(nameof(Reverse_TestData))]
    public void Reverse_Polygon_Success(Point[] points)
    {
        using GraphicsPath gp = new();
        gp.AddPolygon(points);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Rectangle_Success()
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(1, 2, 3, 4));
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Rectangles_Success()
    {
        using GraphicsPath gp = new();
        Rectangle[] rects = [new(1, 2, 3, 4), new(5, 6, 7, 8)];
        gp.AddRectangles(rects);
        AssertReverse(gp, gp.PathPoints, gp.PathTypes);
    }

    [Fact]
    public void Reverse_Pie_Success()
    {
        using GraphicsPath gp = new();
        gp.AddPie(1, 2, 3, 4, 10, 20);
        byte[] expectedTypes = [0, 3, 3, 3, 129];
        AssertReverse(gp, gp.PathPoints, expectedTypes);
    }

    [Fact]
    public void Reverse_ArcLineInnerPath_Success()
    {
        using GraphicsPath inner = new();
        using GraphicsPath gp = new();
        inner.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
        inner.AddLine(1, 2, 3, 4);
        byte[] expectedTypes = [0, 1, 1, 3, 3, 3];
        gp.AddPath(inner, true);
        AssertReverse(gp, gp.PathPoints, expectedTypes);
    }

    [Fact]
    public void Reverse_EllipseRectangle_Success()
    {
        using GraphicsPath gp = new();
        gp.AddEllipse(50, 51, 50, 100);
        gp.AddRectangle(new Rectangle(200, 201, 60, 61));
        byte[] expectedTypes = [0, 1, 1, 129, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 131];
        AssertReverse(gp, gp.PathPoints, expectedTypes);
    }

    [Fact]
    public void Reverse_String_Success()
    {
        using GraphicsPath gp = new();
        gp.AddString("Mono::", FontFamily.GenericMonospace, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
        byte[] expectedTypes =
        [
                0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 129,
                0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 161,
                0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 129,
                0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 161,
                0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 131, 0, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 163, 0, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 3, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 161, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 131, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 163, 0, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1,
                1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 129
        ];

        AssertReverse(gp, gp.PathPoints, expectedTypes);
    }

    [Fact]
    public void Reverse_Marker_Success()
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(200, 201, 60, 61));
        gp.SetMarkers();
        byte[] expectedTypes = [0, 1, 1, 129];
        AssertReverse(gp, gp.PathPoints, expectedTypes);
    }

    [Fact]
    public void Reverse_SubpathMarker_Success()
    {
        using (GraphicsPath gp = new())
        {
            gp.AddLine(0, 1, 2, 3);
            gp.SetMarkers();
            gp.CloseFigure();
            gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
            gp.CloseFigure();
            byte[] expectedTypes = [0, 3, 3, 163, 0, 129];
            AssertReverse(gp, gp.PathPoints, expectedTypes);
        }

        using (GraphicsPath gp = new())
        {
            gp.AddLine(0, 1, 2, 3);
            gp.SetMarkers();
            gp.StartFigure();
            gp.AddLine(20, 21, 22, 23);
            gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
            byte[] expectedTypes = [0, 3, 3, 3, 1, 33, 0, 1];
            AssertReverse(gp, gp.PathPoints, expectedTypes);
        }
    }

    [Fact]
    public void Ctor_PointsTypes_Success()
    {
        int dX = 520;
        int dY = 320;
        Point[] expectedPoints = [
            new(dX-64, dY-24), new(dX-59, dY-34), new(dX-52, dY-54),
            new(dX-18, dY-66), new(dX-34, dY-47), new(dX-43, dY-27),
            new(dX-44, dY-8),
        ];

        byte[] expectedTypes =
        [
            (byte)PathPointType.Start, (byte)PathPointType.Bezier, (byte)PathPointType.Bezier,
            (byte)PathPointType.Bezier, (byte)PathPointType.Bezier, (byte)PathPointType.Bezier,
            (byte)PathPointType.Bezier
        ];

        using GraphicsPath path = new(expectedPoints, expectedTypes);
        Assert.Equal(7, path.PointCount);
        byte[] actualTypes = path.PathTypes;
        Assert.Equal(expectedTypes, actualTypes);
    }

    private static void AssertEmptyGraphicsPath(GraphicsPath gp)
    {
        Assert.Empty(gp.PathData.Points);
        Assert.Empty(gp.PathData.Types);
        Assert.Equal(0, gp.PointCount);
    }

    private static void AssertLine(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(1f, 1f), new PointF(2f, 2f)
        ];

        path.PathPoints.Should().HaveCount(2);
        path.PathTypes.Should().HaveCount(2);
        path.PathData.Points.Should().HaveCount(2);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 1f, 1f), precision: 0.000001f);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 1 });
    }

    private static void AssertArc(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(2.99990582f, 2.01370716f), new PointF(2.99984312f, 2.018276f),
            new PointF(2.99974918f, 2.02284455f), new PointF(2.999624f, 2.027412f),
        ];

        path.PathPoints.Should().HaveCount(4);
        path.PathTypes.Should().HaveCount(4);
        path.PathData.Points.Should().HaveCount(4);
        path.GetBounds().Should().BeApproximately(new(2.99962401f, 2.01370716f, 0f, 0.0137047768f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 3, 3, 3 });
    }

    private static void AssertBezier(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(1f, 1f), new PointF(2f, 2f),
            new PointF(3f, 3f), new PointF(4f, 4f),
        ];

        path.PointCount.Should().Be(4);
        path.PathPoints.Should().HaveCount(4);
        path.PathTypes.Should().HaveCount(4);
        path.PathData.Points.Should().HaveCount(4);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 3f, 3f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 3, 3, 3 });
    }

    private static void AssertCurve(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(1f, 1f), new PointF(1.16666663f, 1.16666663f),
            new PointF(1.83333325f, 1.83333325f), new PointF(2f, 2f)
        ];

        path.PathPoints.Should().HaveCount(4);
        path.PathTypes.Should().HaveCount(4);
        path.PathData.Points.Should().HaveCount(4);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 1f, 1f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 3, 3, 3 });
    }

    private static void AssertClosedCurve(GraphicsPath path)
    {
        path.PathPoints.Should().HaveCount(10);
        path.PathTypes.Should().HaveCount(10);
        path.PathData.Points.Should().HaveCount(10);
        path.GetBounds().Should().BeApproximately(new(0.8333333f, 0.8333333f, 2.33333278f, 2.33333278f), Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 3, 3, 3, 3, 3, 3, 3, 3, 131 });
    }

    private static void AssertRectangle(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(1f, 1f), new PointF(3f, 1f),
            new PointF(3f, 3f), new PointF(1f, 3f)
        ];

        path.PathPoints.Should().HaveCount(4);
        path.PathPoints.Should().HaveCount(4);
        path.PathTypes.Should().HaveCount(4);
        path.PathData.Points.Should().HaveCount(4);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 2f, 2f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 1, 1, 129 });
    }

    private static void AssertEllipse(GraphicsPath path)
    {
        path.PathPoints.Should().HaveCount(13);
        path.PathTypes.Should().HaveCount(13);
        path.PathData.Points.Should().HaveCount(13);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 2f, 2f), Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 131 });
    }

    private static void AssertPie(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(2f, 2f), new PointF(2.99990582f, 2.01370716f),
            new PointF(2.99984312f, 2.018276f), new PointF(2.99974918f, 2.02284455f),
            new PointF(2.999624f, 2.027412f)
        ];

        path.PathPoints.Should().HaveCount(5);
        path.PathTypes.Should().HaveCount(5);
        path.PathData.Points.Should().HaveCount(5);
        path.GetBounds().Should().BeApproximately(new(2f, 2f, 0.9999058f, 0.0274119377f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 1, 3, 3, 131 });
    }

    private static void AssertPolygon(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(1f, 1f),
            new PointF(2f, 2f),
            new PointF(3f, 3f)
        ];

        path.PathPoints.Should().HaveCount(3);
        path.PathTypes.Should().HaveCount(3);
        path.PathData.Points.Should().HaveCount(3);
        path.GetBounds().Should().BeApproximately(new(1f, 1f, 2f, 2f), Delta);
        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, Delta);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 1, 129 });
    }

    private static void AssertFlats(GraphicsPath flat, GraphicsPath original)
    {
        flat.PointCount.Should().BeGreaterThanOrEqualTo(original.PointCount);
        for (int i = 0; i < flat.PointCount; i++)
        {
            flat.PathTypes[i].Should().NotBe(3);
        }
    }

    private static void AssertWrapNaN(GraphicsPath path)
    {
        byte[] expectedTypes = [0, 1, 129];

        Assert.Equal(3, path.PointCount);
        Assert.Equal(float.NaN, path.PathPoints[0].X);
        Assert.Equal(float.NaN, path.PathPoints[0].Y);
        Assert.Equal(float.NaN, path.PathPoints[1].X);
        Assert.Equal(float.NaN, path.PathPoints[1].Y);
        Assert.Equal(float.NaN, path.PathPoints[2].X);
        Assert.Equal(float.NaN, path.PathPoints[2].Y);
        Assert.Equal(expectedTypes, path.PathTypes);
    }

    private static void AssertWiden3(GraphicsPath path)
    {
        PointF[] expectedPoints =
        [
            new PointF(4.2f, 4.5f), new PointF(15.8f, 4.5f),
            new PointF(10.0f, 16.1f), new PointF(10.4f, 14.8f),
            new PointF(9.6f, 14.8f), new PointF(14.6f, 4.8f),
            new PointF(15.0f, 5.5f), new PointF(5.0f, 5.5f),
            new PointF(5.4f, 4.8f)
        ];

        path.PathPoints.Should().BeApproximatelyEquivalentTo(expectedPoints, precision: 0.25f);
        path.PathTypes.Should().BeEquivalentTo(new byte[] { 0, 1, 129, 0, 1, 1, 1, 1, 129 });
    }

    private static void AssertIsOutlineVisibleLine(Graphics graphics)
    {
        using GraphicsPath gp = new();
        using Pen pen = new(Color.Red, 3.0f);
        gp.AddLine(10, 1, 14, 1);
        Assert.True(gp.IsOutlineVisible(10, 1, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(10, 2, pen, graphics));
        Assert.False(gp.IsOutlineVisible(10, 2, Pens.Red, graphics));

        Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, pen, graphics));
        Assert.False(gp.IsOutlineVisible(11.0f, 2.0f, Pens.Red, graphics));

        Point point = new(12, 2);
        Assert.False(gp.IsOutlineVisible(point, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(point, pen, graphics));

        point.Y = 1;
        Assert.True(gp.IsOutlineVisible(point, Pens.Red, graphics));

        PointF fPoint = new(13.0f, 2.0f);
        Assert.False(gp.IsOutlineVisible(fPoint, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(fPoint, pen, graphics));

        fPoint.Y = 1;
        Assert.True(gp.IsOutlineVisible(fPoint, Pens.Red, graphics));
    }

    private static void AssertIsOutlineVisibleRectangle(Graphics graphics)
    {
        using Pen pen = new(Color.Red, 3.0f);
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(10, 10, 20, 20));
        Assert.True(gp.IsOutlineVisible(10, 10, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(10, 11, pen, graphics));
        Assert.False(gp.IsOutlineVisible(11, 11, Pens.Red, graphics));

        Assert.True(gp.IsOutlineVisible(11.0f, 10.0f, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(11.0f, 11.0f, pen, graphics));
        Assert.False(gp.IsOutlineVisible(11.0f, 11.0f, Pens.Red, graphics));

        Point point = new(15, 10);
        Assert.True(gp.IsOutlineVisible(point, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(point, pen, graphics));

        point.Y = 15;
        Assert.False(gp.IsOutlineVisible(point, Pens.Red, graphics));

        PointF fPoint = new(29.0f, 29.0f);
        Assert.False(gp.IsOutlineVisible(fPoint, Pens.Red, graphics));
        Assert.True(gp.IsOutlineVisible(fPoint, pen, graphics));

        fPoint.Y = 31.0f;
        Assert.True(gp.IsOutlineVisible(fPoint, pen, graphics));
    }

    private static void AssertIsVisibleRectangle(Graphics graphics)
    {
        using GraphicsPath gp = new();
        gp.AddRectangle(new Rectangle(10, 10, 20, 20));
        Assert.False(gp.IsVisible(9, 9, graphics));
        Assert.True(gp.IsVisible(10, 10, graphics));
        Assert.True(gp.IsVisible(20, 20, graphics));
        Assert.True(gp.IsVisible(29, 29, graphics));
        Assert.False(gp.IsVisible(30, 29, graphics));
        Assert.False(gp.IsVisible(29, 30, graphics));
        Assert.False(gp.IsVisible(30, 30, graphics));
        Assert.False(gp.IsVisible(9.4f, 9.4f, graphics));
        Assert.True(gp.IsVisible(9.5f, 9.5f, graphics));
        Assert.True(gp.IsVisible(10f, 10f, graphics));
        Assert.True(gp.IsVisible(20f, 20f, graphics));
        Assert.True(gp.IsVisible(29.4f, 29.4f, graphics));
        Assert.False(gp.IsVisible(29.5f, 29.5f, graphics));
        Assert.False(gp.IsVisible(29.5f, 29.4f, graphics));
        Assert.False(gp.IsVisible(29.4f, 29.5f, graphics));
    }

    private static void AssertIsVisibleEllipse(Graphics graphics)
    {
        using GraphicsPath gp = new();
        gp.AddEllipse(new Rectangle(10, 10, 20, 20));
        Assert.False(gp.IsVisible(10, 10, graphics));
        Assert.True(gp.IsVisible(20, 20, graphics));
        Assert.False(gp.IsVisible(29, 29, graphics));
        Assert.False(gp.IsVisible(10f, 10f, graphics));
        Assert.True(gp.IsVisible(20f, 20f, graphics));
        Assert.False(gp.IsVisible(29.4f, 29.4f, graphics));
    }

    private static void AssertReverse(GraphicsPath gp, PointF[] expectedPoints, byte[] expectedTypes)
    {
        gp.Reverse();
        PointF[] reversedPoints = gp.PathPoints;
        byte[] reversedTypes = gp.PathTypes;

        int count = gp.PointCount;
        Assert.Equal(expectedPoints.Length, gp.PointCount);
        Assert.Equal(expectedTypes, gp.PathTypes);
        for (int i = 0; i < count; i++)
        {
            Assert.Equal(expectedPoints[i], reversedPoints[count - i - 1]);
            Assert.Equal(expectedTypes[i], reversedTypes[i]);
        }
    }

#if NET9_0_OR_GREATER
    [Fact]
    public void GraphicsPath_AddRoundedRectangle_Integer()
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(new Rectangle(10, 10, 20, 20), new(5, 5));
        path.PathPoints.Should().BeApproximatelyEquivalentTo(
            new PointF[]
            {
                new(27.499994f, 10),
                new(28.880707f, 9.999997f),
                new(29.999996f, 11.119284f),
                new(29.999998f, 12.499999f),
                new(29.999998f, 12.499999f),
                new(29.999998f, 12.5f),
                new(29.999998f, 12.5f),
                new(29.999998f, 27.499998f),
                new(29.999998f, 28.88071f),
                new(28.88071f, 29.999998f),
                new(27.5f, 29.999998f),
                new(12.500001f, 29.999998f),
                new(11.119289f, 30),
                new(10.000001f, 28.880713f),
                new(10f, 27.5f),
                new(9.999999f, 12.500001f),
                new(9.999998f, 11.119289f),
                new(11.119286f, 10.000001f),
                new(12.499997f, 9.999999f),
            },
            0.000001f);
    }

    [Fact]
    public void GraphicsPath_AddRoundedRectangle_Float()
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(new RectangleF(10, 10, 20, 20), new(5, 5));
        path.PathPoints.Should().BeApproximatelyEquivalentTo(
            new PointF[]
            {
                new(27.499994f, 10),
                new(28.880707f, 9.999997f),
                new(29.999996f, 11.119284f),
                new(29.999998f, 12.499999f),
                new(29.999998f, 12.499999f),
                new(29.999998f, 12.5f),
                new(29.999998f, 12.5f),
                new(29.999998f, 27.499998f),
                new(29.999998f, 28.88071f),
                new(28.88071f, 29.999998f),
                new(27.5f, 29.999998f),
                new(12.500001f, 29.999998f),
                new(11.119289f, 30),
                new(10.000001f, 28.880713f),
                new(10f, 27.5f),
                new(9.999999f, 12.500001f),
                new(9.999998f, 11.119289f),
                new(11.119286f, 10.000001f),
                new(12.499997f, 9.999999f),
            },
            0.000001f);
    }

#endif
}
