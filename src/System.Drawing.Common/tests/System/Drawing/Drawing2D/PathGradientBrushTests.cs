// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
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

public class PathGradientBrushTests
{
    private readonly Point[] _defaultIntPoints = [new(1, 2), new(20, 30)];
    private readonly PointF[] _defaultFloatPoints = [new(1, 2), new(20, 30)];
    private readonly RectangleF _defaultRectangle = new(1, 2, 19, 28);

    [Fact]
    public void Ctor_Points_ReturnsExpected()
    {
        using PathGradientBrush bi = new(_defaultIntPoints);
        using PathGradientBrush bf = new(_defaultFloatPoints);
        AssertDefaults(bi);
        Assert.Equal(WrapMode.Clamp, bi.WrapMode);
        AssertDefaults(bf);
        Assert.Equal(WrapMode.Clamp, bf.WrapMode);
    }

    public static IEnumerable<object[]> WrapMode_TestData()
    {
        yield return new object[] { WrapMode.Clamp };
        yield return new object[] { WrapMode.Tile };
        yield return new object[] { WrapMode.TileFlipX };
        yield return new object[] { WrapMode.TileFlipXY };
        yield return new object[] { WrapMode.TileFlipY };
    }

    [Theory]
    [MemberData(nameof(WrapMode_TestData))]
    public void Ctor_PointsWrapMode_ReturnsExpected(WrapMode wrapMode)
    {
        using PathGradientBrush brushInt = new(_defaultIntPoints, wrapMode);
        using PathGradientBrush brushFloat = new(_defaultFloatPoints, wrapMode);
        AssertDefaults(brushInt);
        Assert.Equal(wrapMode, brushInt.WrapMode);
        AssertDefaults(brushFloat);
        Assert.Equal(wrapMode, brushFloat.WrapMode);
    }

    [Fact]
    public void Ctor_PointsNull_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("points", () => new PathGradientBrush((Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => new PathGradientBrush((PointF[])null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Ctor_PointsLengthLessThenTwo_ThrowsArgumentException(int pointsLength)
    {
        Assert.Throws<ArgumentException>(() => new PathGradientBrush(new Point[pointsLength]));
        Assert.Throws<ArgumentException>(() => new PathGradientBrush(new Point[pointsLength], WrapMode.Clamp));
        Assert.Throws<ArgumentException>(() => new PathGradientBrush(new PointF[pointsLength]));
        Assert.Throws<ArgumentException>(() => new PathGradientBrush(new PointF[pointsLength], WrapMode.Clamp));
    }

    [Fact]
    public void Ctor_InvalidWrapMode_ThrowsInvalidEnumArgumentException()
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            new PathGradientBrush(_defaultIntPoints, (WrapMode)int.MaxValue));

        Assert.ThrowsAny<ArgumentException>(() =>
            new PathGradientBrush(_defaultFloatPoints, (WrapMode)int.MaxValue));
    }

    [Fact]
    public void Ctor_Path_ReturnsExpected()
    {
        using GraphicsPath path = new(_defaultFloatPoints, [0, 1]);
        using PathGradientBrush brush = new(path);
        AssertDefaults(brush);
        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void Ctor_Path_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("path", () => new PathGradientBrush((GraphicsPath)null));
    }

    [Fact]
    public void Ctor_PathWithLessThenTwoPoints_ThrowsOutOfMemoryException()
    {
        using GraphicsPath path = new();
        Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(path));
        path.AddLines(new PointF[] { new(1, 1) });
        Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(path));
    }

    [Fact]
    public void Clone_ReturnsExpected()
    {
        using GraphicsPath path = new(_defaultFloatPoints, [0, 1]);
        using PathGradientBrush brush = new(path);
        using PathGradientBrush clone = Assert.IsType<PathGradientBrush>(brush.Clone());
        AssertDefaults(clone);
        Assert.Equal(WrapMode.Clamp, clone.WrapMode);
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.Clone);
    }

    [Fact]
    public void CenterColor_ReturnsExpected()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.Equal(Color.Black.ToArgb(), brush.CenterColor.ToArgb());
        brush.CenterColor = Color.Blue;
        Assert.Equal(Color.Blue.ToArgb(), brush.CenterColor.ToArgb());
        brush.CenterColor = Color.Transparent;
        Assert.Equal(Color.Transparent.ToArgb(), brush.CenterColor.ToArgb());
    }

    [Fact]
    public void CenterColor_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.CenterColor = Color.Blue);
    }

    [Fact]
    public void SurroundColors_ReturnsExpected()
    {
        Color[] expectedColors = [Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0)];
        Color[] sameColors = [Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 255, 255, 0)];
        Color[] expectedSameColors = [Color.FromArgb(255, 255, 255, 0)];

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SurroundColors = expectedColors;
        Assert.Equal(expectedColors, brush.SurroundColors);
        brush.SurroundColors = sameColors;
        Assert.Equal(expectedSameColors, brush.SurroundColors);
    }

    [Fact]
    public void SurroundColors_CannotChange()
    {
        Color[] colors = [Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0)];
        Color[] defaultColors = [Color.FromArgb(255, 255, 255, 255)];

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SurroundColors.ToList().AddRange(colors);
        Assert.Equal(defaultColors, brush.SurroundColors);
        brush.SurroundColors[0] = Color.FromArgb(255, 0, 0, 255);
        Assert.NotEqual(Color.FromArgb(255, 0, 0, 255), brush.SurroundColors[0]);
        Assert.Equal(defaultColors, brush.SurroundColors);
    }

    [Fact]
    public void SurroundColors_Disposed_ThrowsArgumentException()
    {
        Color[] colors = [Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0)];
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.SurroundColors = colors);
    }

    public static IEnumerable<object[]> SurroundColors_InvalidColorsLength_TestData()
    {
        yield return new object[] { new Point[2] { new(1, 1), new(2, 2) }, Array.Empty<Color>() };
        yield return new object[] { new Point[2] { new(1, 1), new(2, 2) }, new Color[3] };
    }

    [Theory]
    [MemberData(nameof(SurroundColors_InvalidColorsLength_TestData))]
    public void SurroundColors_InvalidColorsLength_ThrowsArgumentException(Point[] points, Color[] colors)
    {
        using PathGradientBrush brush = new(points);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.SurroundColors = colors);
    }

    [Fact]
    public void SurroundColors_Null_ThrowsArgumentNullException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentNullException>(() => brush.SurroundColors = null);
    }

    [Fact]
    public void CenterPoint_ReturnsExpected()
    {
        PointF centralPoint = new(float.MaxValue, float.MinValue);
        PointF defaultCentralPoint = new(10.5f, 16f);

        using PathGradientBrush brush = new(_defaultFloatPoints, WrapMode.TileFlipXY);
        Assert.Equal(defaultCentralPoint, brush.CenterPoint);
        brush.CenterPoint = centralPoint;
        Assert.Equal(centralPoint, brush.CenterPoint);

        centralPoint.X = float.NaN;
        centralPoint.Y = float.NegativeInfinity;
        brush.CenterPoint = centralPoint;
        Assert.Equal(float.NaN, brush.CenterPoint.X);
        Assert.Equal(float.NegativeInfinity, brush.CenterPoint.Y);
    }

    [Fact]
    public void CenterPoint_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.CenterPoint);
    }

    public static IEnumerable<object[]> Blend_FactorsPositions_TestData()
    {
        yield return new object[] { new float[1] { 1 }, new float[1] { 0 } };
        yield return new object[] { new float[2] { 1, 1 }, new float[2] { 0, 1 } };
        yield return new object[] { new float[3] { 1, 0, 1 }, new float[3] { 0, 3, 1 } };
    }

    [Theory]
    [MemberData(nameof(Blend_FactorsPositions_TestData))]
    public void Blend_ReturnsExpected(float[] factors, float[] positions)
    {
        int expectedSize = factors.Length;

        using PathGradientBrush brush = new(_defaultFloatPoints, WrapMode.TileFlipXY);
        brush.Blend = new Blend { Factors = factors, Positions = positions };
        Assert.Equal(factors, brush.Blend.Factors);
        Assert.Equal(expectedSize, brush.Blend.Positions.Length);
        if (expectedSize == positions.Length && expectedSize != 1)
        {
            Assert.Equal(factors, brush.Blend.Factors);
            Assert.Equal(positions, brush.Blend.Positions);
        }
        else
        {
            Assert.Equal(factors, brush.Blend.Factors);
            Assert.Single(brush.Blend.Positions);
        }
    }

    [Fact]
    public void Blend_CannotChange()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints, WrapMode.TileFlipXY);
        brush.Blend.Factors = [];
        Assert.Single(brush.Blend.Factors);
        brush.Blend.Factors = new float[2];
        Assert.Single(brush.Blend.Factors);
        brush.Blend.Positions = [];
        Assert.Single(brush.Blend.Positions);
        brush.Blend.Positions = new float[2];
        Assert.Single(brush.Blend.Positions);
    }

    [Fact]
    public void Blend_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend);
    }

    public static IEnumerable<object[]> Blend_InvalidFactorsPositions_TestData()
    {
        yield return new object[] { new Blend() { Factors = [], Positions = [] } };
        yield return new object[] { new Blend() { Factors = new float[2], Positions = [1, 1] } };
        yield return new object[] { new Blend() { Factors = new float[2], Positions = [0, 5] } };
        yield return new object[] { new Blend() { Factors = new float[3], Positions = [0, 1, 5] } };
        yield return new object[] { new Blend() { Factors = new float[3], Positions = [1, 1, 1] } };
    }

    [Theory]
    [MemberData(nameof(Blend_InvalidFactorsPositions_TestData))]
    public void Blend_InvalidFactorPositions_ThrowsArgumentException(Blend blend)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend = blend);
    }

    [Fact]
    public void Blend_InvalidFactorPositionsLengthMismatch_ThrowsArgumentException()
    {
        Blend invalidBlend = new() { Factors = new float[2], Positions = new float[1] };

        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>("value", null, () => brush.Blend = invalidBlend);
    }

    [Fact]
    public void Blend_Null_ThrowsArgumentNullException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.Throws<ArgumentNullException>(() => brush.Blend = null);
        Assert.Throws<ArgumentNullException>(() => brush.Blend = new Blend() { Factors = null, Positions = null });
        Assert.Throws<ArgumentNullException>(() => brush.Blend = new Blend() { Factors = null, Positions = [] });
    }

    [Fact]
    public void Blend_NullBlendProperites_ThrowsArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException, ArgumentNullException>("value", "source", () =>
            brush.Blend = new Blend() { Factors = [], Positions = null });
    }

    [Theory]
    [InlineData(1f)]
    [InlineData(0f)]
    [InlineData(0.5f)]
    public void SetSigmaBellShape_Focus_Success(float focus)
    {
        float defaultScale = 1f;

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SetSigmaBellShape(focus);
        Assert.True(brush.Transform.IsIdentity);
        if (focus == 0f)
        {
            Assert.Equal(focus, brush.Blend.Positions[0]);
            Assert.Equal(defaultScale, brush.Blend.Factors[0]);
            Assert.Equal(1f, brush.Blend.Positions[^1]);
            Assert.Equal(0f, brush.Blend.Factors[^1]);
        }
        else if (focus == 1f)
        {
            Assert.Equal(0f, brush.Blend.Positions[0]);
            Assert.Equal(0f, brush.Blend.Factors[0]);
            Assert.Equal(focus, brush.Blend.Positions[^1]);
            Assert.Equal(defaultScale, brush.Blend.Factors[^1]);
        }
        else
        {
            Assert.Equal(0f, brush.Blend.Positions[0]);
            Assert.Equal(0f, brush.Blend.Factors[0]);
            Assert.Equal(1f, brush.Blend.Positions[^1]);
            Assert.Equal(0f, brush.Blend.Factors[^1]);
        }
    }

    [Theory]
    [InlineData(1f)]
    [InlineData(0f)]
    [InlineData(0.5f)]
    public void SetSigmaBellShape_FocusScale_Success(float focus)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SetSigmaBellShape(focus);
        Assert.True(brush.Transform.IsIdentity);
        if (focus == 0f)
        {
            Assert.Equal(256, brush.Blend.Positions.Length);
            Assert.Equal(256, brush.Blend.Factors.Length);
            Assert.Equal(focus, brush.Blend.Positions[0]);
            Assert.Equal(1f, brush.Blend.Factors[0]);
            Assert.Equal(1f, brush.Blend.Positions[^1]);
            Assert.Equal(0f, brush.Blend.Factors[^1]);
        }
        else if (focus == 1f)
        {
            Assert.Equal(256, brush.Blend.Positions.Length);
            Assert.Equal(256, brush.Blend.Factors.Length);
            Assert.Equal(0f, brush.Blend.Positions[0]);
            Assert.Equal(0f, brush.Blend.Factors[0]);
            Assert.Equal(focus, brush.Blend.Positions[^1]);
            Assert.Equal(1f, brush.Blend.Factors[^1]);
        }
        else
        {
            Assert.Equal(511, brush.Blend.Positions.Length);
            Assert.Equal(511, brush.Blend.Factors.Length);
            Assert.Equal(0f, brush.Blend.Positions[0]);
            Assert.Equal(0f, brush.Blend.Factors[0]);
            Assert.Equal(focus, brush.Blend.Positions[255]);
            Assert.Equal(1f, brush.Blend.Factors[255]);
            Assert.Equal(1f, brush.Blend.Positions[^1]);
            Assert.Equal(0f, brush.Blend.Factors[^1]);
        }
    }

    [Fact]
    public void SetSigmaBellShape_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetSigmaBellShape(1f));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetSigmaBellShape(1f, 1f));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.1f)]
    public void SetSigmaBellShape_InvalidFocus_ThrowsArgumentException(float focus)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(focus));
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(focus, 1f));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.1f)]
    public void SetSigmaBellShape_InvalidScale_ThrowsArgumentException(float scale)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(1f, scale));
    }

    [Theory]
    [InlineData(1f)]
    [InlineData(0f)]
    [InlineData(0.5f)]
    public void SetBlendTriangularShape_Focus_Success(float focus)
    {
        float defaultScale = 1f;

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SetBlendTriangularShape(focus);
        Assert.True(brush.Transform.IsIdentity);
        if (focus == 0f)
        {
            Assert.Equal([defaultScale, 0f], brush.Blend.Factors);
            Assert.Equal([focus, 1f], brush.Blend.Positions);
        }
        else if (focus == 1f)
        {
            Assert.Equal([0f, defaultScale], brush.Blend.Factors);
            Assert.Equal([0f, focus], brush.Blend.Positions);
        }
        else
        {
            Assert.Equal([0f, defaultScale, 0f], brush.Blend.Factors);
            Assert.Equal([0f, focus, 1f], brush.Blend.Positions);
        }
    }

    [Theory]
    [InlineData(1f)]
    [InlineData(0f)]
    [InlineData(0.5f)]
    public void SetBlendTriangularShape_FocusScale_Success(float focus)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.SetBlendTriangularShape(focus);
        Assert.True(brush.Transform.IsIdentity);
        Assert.True(brush.Transform.IsIdentity);
        if (focus == 0f)
        {
            Assert.Equal([1f, 0f], brush.Blend.Factors);
            Assert.Equal([focus, 1f], brush.Blend.Positions);
        }
        else if (focus == 1f)
        {
            Assert.Equal([0f, 1f], brush.Blend.Factors);
            Assert.Equal([0f, focus], brush.Blend.Positions);
        }
        else
        {
            Assert.Equal([0f, 1f, 0f], brush.Blend.Factors);
            Assert.Equal([0f, focus, 1f], brush.Blend.Positions);
        }
    }

    [Fact]
    public void SetBlendTriangularShape_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetBlendTriangularShape(1f));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetBlendTriangularShape(1f, 1f));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.1f)]
    public void SetBlendTriangularShape_InvalidFocus_ThrowsArgumentException(float focus)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(focus));
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(focus, 1f));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.1f)]
    public void SetBlendTriangularShape_InvalidScale_ThrowsArgumentException(float scale)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(1f, scale));
    }

    [Fact]
    public void InterpolationColors_ReturnsExpected()
    {
        Color[] expectedColors = [Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0)];
        float[] expectedPositions = [0, 1];
        Color[] sameColors = [Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 255, 255, 0)];

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.InterpolationColors = new ColorBlend() { Colors = expectedColors, Positions = expectedPositions };
        Assert.Equal(expectedColors, brush.InterpolationColors.Colors);
        Assert.Equal(expectedPositions, brush.InterpolationColors.Positions);

        brush.InterpolationColors = new ColorBlend() { Colors = sameColors, Positions = expectedPositions };
        Assert.Equal(sameColors, brush.InterpolationColors.Colors);
        Assert.Equal(expectedPositions, brush.InterpolationColors.Positions);
    }

    [Fact]
    public void InterpolationColors_CannotChange()
    {
        Color[] colors = [Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 255, 0, 0)];
        Color[] defaultColors = [Color.Empty];

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.InterpolationColors.Colors.ToList().AddRange(colors);
        Assert.Equal(defaultColors, brush.InterpolationColors.Colors);
        brush.InterpolationColors.Colors = colors;
        Assert.Equal(defaultColors, brush.InterpolationColors.Colors);
        brush.InterpolationColors.Colors[0] = Color.Pink;
        Assert.NotEqual(Color.Pink, brush.InterpolationColors.Colors[0]);
        Assert.Equal(defaultColors, brush.InterpolationColors.Colors);
        brush.InterpolationColors.Positions = [];
        Assert.Single(brush.InterpolationColors.Positions);
        brush.InterpolationColors.Positions = new float[2];
        Assert.Single(brush.InterpolationColors.Positions);
    }

    [Fact]
    public void InterpolationColors_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.InterpolationColors);
    }

    [Fact]
    public void InterpolationColors_Null_ThrowsArgumentNullException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.Throws<ArgumentNullException>(() => brush.InterpolationColors = null);
    }

    [Fact]
    public void InterpolationColors_NullColors_ThrowsNullReferenceException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.Throws<NullReferenceException>(() =>
            brush.InterpolationColors = new ColorBlend() { Colors = null, Positions = null });

        Assert.Throws<NullReferenceException>(() =>
            brush.InterpolationColors = new ColorBlend() { Colors = null, Positions = new float[2] });
    }

    [Fact]
    public void InterpolationColors_NullPoints_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException, ArgumentNullException>("value", "source", () =>
            brush.InterpolationColors = new ColorBlend() { Colors = new Color[1], Positions = null });
    }

    [Fact]
    public void InterpolationColors_Empty_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.InterpolationColors = new ColorBlend());
    }

    [Fact]
    public void InterpolationColors_EmptyColors_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () =>
            brush.InterpolationColors = new ColorBlend() { Colors = [], Positions = [] });
    }

    [Fact]
    public void InterpolationColors_PointsLengthGreaterThenColorsLength_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException, ArgumentException>("value", null, () =>
            brush.InterpolationColors = new ColorBlend() { Colors = new Color[1], Positions = new float[2] });
    }

    [Fact]
    public void InterpolationColors_ColorsLengthGreaterThenPointsLength_ThrowsArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.Throws<ArgumentException>(() =>
            brush.InterpolationColors = new ColorBlend() { Colors = new Color[2], Positions = new float[1] });
    }

    [Fact]
    public void Transform_ReturnsExpected()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix defaultMatrix = new(1, 0, 0, 1, 0, 0);
        using Matrix matrix = new(1, 0, 0, 1, 1, 1);
        Assert.Equal(defaultMatrix, brush.Transform);
        brush.Transform = matrix;
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void Transform_EmptyMatrix_ReturnsExpected()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new();
        brush.Transform = matrix;
        Assert.True(brush.Transform.IsIdentity);
    }

    [Fact]
    public void Transform_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform);
    }

    [Fact]
    public void Transform_Null_ArgumentNullException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentNullException>("value", "matrix", () => brush.Transform = null);
    }

    [Fact]
    public void Transform_NonInvertible_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix nonInvertible = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform = nonInvertible);
    }

    [Fact]
    public void ResetTransform_Success()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix defaultMatrix = new(1, 0, 0, 1, 0, 0);
        using Matrix matrix = new(1, 0, 0, 1, 1, 1);
        Assert.Equal(defaultMatrix, brush.Transform);
        brush.Transform = matrix;
        Assert.Equal(matrix, brush.Transform);
        brush.ResetTransform();
        Assert.Equal(defaultMatrix, brush.Transform);
    }

    [Fact]
    public void ResetTransform_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.ResetTransform);
    }

    [Fact]
    public void MultiplyTransform_Matrix_Success()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix defaultMatrix = new(1, 0, 0, 1, 0, 0);
        using Matrix matrix = new(1, 0, 0, 1, 1, 1);
        defaultMatrix.Multiply(matrix, MatrixOrder.Prepend);
        brush.MultiplyTransform(matrix);
        Assert.Equal(defaultMatrix, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Append)]
    [InlineData(MatrixOrder.Prepend)]
    public void MultiplyTransform_MatrixMatrixOrder_Success(MatrixOrder matrixOrder)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix defaultMatrix = new(1, 0, 0, 1, 0, 0);
        using Matrix matrix = new(1, 0, 0, 1, 1, 1);
        defaultMatrix.Multiply(matrix, matrixOrder);
        brush.MultiplyTransform(matrix, matrixOrder);
        Assert.Equal(defaultMatrix, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_Disposed_ThrowsArgumentException()
    {
        using Matrix matrix = new(1, 0, 0, 1, 1, 1);
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix, MatrixOrder.Append));
    }

    [Fact]
    public void MultiplyTransform_NullMatrix_ThrowsArgumentNullException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null));
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null, MatrixOrder.Append));
    }

    [Fact]
    public void MultiplyTransform_DisposedMatrix_Nop()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix transform = brush.Transform;
        Matrix matrix = new();
        matrix.Dispose();

        brush.MultiplyTransform(matrix);
        brush.MultiplyTransform(matrix, MatrixOrder.Append);

        Assert.Equal(transform, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_InvalidMatrixOrder_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 1, 1, 1, 1, 1);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix, (MatrixOrder)int.MinValue));
    }

    [Fact]
    public void MultiplyTransform_NonInvertible_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix nonInvertible = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(nonInvertible));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(nonInvertible, MatrixOrder.Append));
    }

    [Fact]
    public void TranslateTransform_Offset_Success()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Translate(20f, 30f, MatrixOrder.Prepend);
        brush.TranslateTransform(20f, 30f);
        Assert.Equal(matrix, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Append)]
    [InlineData(MatrixOrder.Prepend)]
    public void TranslateTransform_OffsetMatrixOrder_Success(MatrixOrder matrixOrder)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Translate(20f, 30f, matrixOrder);
        brush.TranslateTransform(20f, 30f, matrixOrder);
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void TranslateTransform_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(20f, 30f, MatrixOrder.Append));
    }

    [Fact]
    public void TranslateTransform_InvalidMatrixOrder_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(20f, 30f, (MatrixOrder)int.MinValue));
    }

    [Fact]
    public void ScaleTransform_Scale_Success()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Scale(2, 4, MatrixOrder.Prepend);
        brush.ScaleTransform(2, 4);
        Assert.Equal(matrix, brush.Transform);

        matrix.Scale(0.5f, 0.25f, MatrixOrder.Prepend);
        brush.ScaleTransform(0.5f, 0.25f);
        Assert.True(brush.Transform.IsIdentity);

        matrix.Scale(float.MaxValue, float.MinValue, MatrixOrder.Prepend);
        brush.ScaleTransform(float.MaxValue, float.MinValue);
        Assert.Equal(matrix, brush.Transform);

        matrix.Scale(float.MinValue, float.MaxValue, MatrixOrder.Prepend);
        brush.ScaleTransform(float.MinValue, float.MaxValue);
        Assert.Equal(matrix, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Append)]
    [InlineData(MatrixOrder.Prepend)]
    public void ScaleTransform_ScaleMatrixOrder_Success(MatrixOrder matrixOrder)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Scale(0.25f, 2, matrixOrder);
        brush.ScaleTransform(0.25f, 2, matrixOrder);
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void ScaleTransform_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(0.25f, 2, MatrixOrder.Append));
    }

    [Fact]
    public void ScaleTransform_InvalidMatrixOrder_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(1, 1, (MatrixOrder)int.MinValue));
    }

    [Fact]
    public void RotateTransform_Angle_Success()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Rotate(90, MatrixOrder.Prepend);
        brush.RotateTransform(90);
        Assert.Equal(matrix, brush.Transform);

        brush.RotateTransform(270);
        Assert.True(brush.Transform.IsIdentity);
    }

    [Theory]
    [InlineData(MatrixOrder.Append)]
    [InlineData(MatrixOrder.Prepend)]
    public void RotateTransform_AngleMatrixOrder_Success(MatrixOrder matrixOrder)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        using Matrix matrix = new(1, 0, 0, 1, 0, 0);
        matrix.Rotate(45, matrixOrder);
        brush.RotateTransform(45, matrixOrder);
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void RotateTransform_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(45, MatrixOrder.Append));
    }

    [Fact]
    public void RotateTransform_InvalidMatrixOrder_ArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(45, (MatrixOrder)int.MinValue));
    }

    [Fact]
    public void FocusScales_ReturnsExpected()
    {
        PointF point = new(2.5f, 3.4f);

        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.FocusScales = point;
        Assert.Equal(point, brush.FocusScales);
    }

    [Fact]
    public void FocusScales_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.FocusScales);
    }

    [Theory]
    [MemberData(nameof(WrapMode_TestData))]
    public void WrapMode_ReturnsExpected(WrapMode wrapMode)
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        brush.WrapMode = wrapMode;
        Assert.Equal(wrapMode, brush.WrapMode);
    }

    [Fact]
    public void WrapMode_Disposed_ThrowsArgumentException()
    {
        PathGradientBrush brush = new(_defaultFloatPoints);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode);
    }

    [Fact]
    public void WrapMode_Invalid_InvalidEnumArgumentException()
    {
        using PathGradientBrush brush = new(_defaultFloatPoints);
        Assert.ThrowsAny<ArgumentException>(() => brush.WrapMode = (WrapMode)int.MinValue);
    }

    private void AssertDefaults(PathGradientBrush brush)
    {
        Assert.Equal(_defaultRectangle, brush.Rectangle);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);
        Assert.Equal(new PointF(10.5f, 16f), brush.CenterPoint);
        Assert.Equal(new Color[] { Color.Empty }, brush.InterpolationColors.Colors);
        Assert.Equal(new Color[] { Color.FromArgb(255, 255, 255, 255) }, brush.SurroundColors);
        Assert.Equal([0], brush.InterpolationColors.Positions);
        Assert.True(brush.Transform.IsIdentity);
        Assert.True(brush.FocusScales.IsEmpty);
    }
}
