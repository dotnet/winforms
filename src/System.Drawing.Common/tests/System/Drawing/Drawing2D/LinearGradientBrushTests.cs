// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D.Tests;

public class LinearGradientBrushTests
{
    public static IEnumerable<object[]> Ctor_Point_TestData()
    {
        yield return new object[] { new Point(0, 0), new Point(2, 2), Color.Empty, Color.Empty, new RectangleF(0, 0, 2, 2) };
        yield return new object[] { new Point(1, 0), new Point(0, 0), Color.Empty, Color.Red, new RectangleF(0, -0.5f, 1, 1) };
        yield return new object[] { new Point(1, 2), new Point(4, 6), Color.Plum, Color.Red, new RectangleF(1, 2, 3, 4) };
        yield return new object[] { new Point(1, 2), new Point(4, 6), Color.Red, Color.Red, new RectangleF(1, 2, 3, 4) };
        yield return new object[] { new Point(-1, -2), new Point(4, 6), Color.Red, Color.Plum, new RectangleF(-1, -2, 5, 8) };
        yield return new object[] { new Point(-4, -6), new Point(1, 2), Color.Black, Color.Wheat, new RectangleF(-4, -6, 5, 8) };
        yield return new object[] { new Point(4, 6), new Point(-1, -2), Color.Black, Color.Wheat, new RectangleF(-1, -2, 5, 8) };
        yield return new object[] { new Point(4, 6), new Point(1, 2), Color.Black, Color.Wheat, new RectangleF(1, 2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(Ctor_Point_TestData))]
    public void Ctor_PointF_PointF_Color_Color(Point point1, Point point2, Color color1, Color color2, RectangleF expectedRectangle)
    {
        using LinearGradientBrush brush = new((PointF)point1, point2, color1, color2);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(expectedRectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.False(brush.Transform.IsIdentity);
    }

    [Fact]
    public void Ctor_PointF_PointF_Color_Color_FloatRanges()
    {
        using LinearGradientBrush brush = new(new PointF(float.NaN, float.NaN), new PointF(float.PositiveInfinity, float.NegativeInfinity), Color.Plum, Color.Red);
        Assert.Equal(float.PositiveInfinity, brush.Rectangle.X);
        Assert.Equal(float.NegativeInfinity, brush.Rectangle.Y);
        Assert.Equal(float.NaN, brush.Rectangle.Width);
        Assert.Equal(float.NaN, brush.Rectangle.Height);
    }

    [Theory]
    [MemberData(nameof(Ctor_Point_TestData))]
    public void Ctor_Point_Point_Color_Color(Point point1, Point point2, Color color1, Color color2, RectangleF expectedRectangle)
    {
        using LinearGradientBrush brush = new(point1, point2, color1, color2);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(expectedRectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.False(brush.Transform.IsIdentity);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public void Ctor_EqualPoints_ThrowsOutOfMemoryException(int x, int y)
    {
        Assert.Throws<OutOfMemoryException>(() => new LinearGradientBrush(new Point(x, y), new Point(x, y), Color.Fuchsia, Color.GhostWhite));
        Assert.Throws<OutOfMemoryException>(() => new LinearGradientBrush(new PointF(x, y), new PointF(x, y), Color.Fuchsia, Color.GhostWhite));
    }

    public static IEnumerable<object[]> Ctor_Rectangle_LinearGradientMode_TestData()
    {
        yield return new object[] { new Rectangle(0, 0, 1, 2), Color.Empty, Color.Red, LinearGradientMode.BackwardDiagonal };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, LinearGradientMode.ForwardDiagonal };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), Color.Red, Color.Red, LinearGradientMode.Horizontal };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Plum, LinearGradientMode.Vertical };
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_LinearGradientMode_TestData))]
    public void Ctor_Rectangle_Color_Color_LinearGradientMode(Rectangle rectangle, Color color1, Color color2, LinearGradientMode linearGradientMode)
    {
        using LinearGradientBrush brush = new(rectangle, color1, color2, linearGradientMode);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal(linearGradientMode == LinearGradientMode.Horizontal, brush.Transform.IsIdentity);
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_LinearGradientMode_TestData))]
    public void Ctor_RectangleF_Color_Color_LinearGradientMode(Rectangle rectangle, Color color1, Color color2, LinearGradientMode linearGradientMode)
    {
        using LinearGradientBrush brush = new((RectangleF)rectangle, color1, color2, linearGradientMode);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal(linearGradientMode == LinearGradientMode.Horizontal, brush.Transform.IsIdentity);
    }

    public static IEnumerable<object[]> Ctor_Rectangle_Angle_TestData()
    {
        yield return new object[] { new Rectangle(0, 0, 1, 2), Color.Empty, Color.Red, 90 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 180 };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), Color.Red, Color.Red, 0 };
        yield return new object[] { new Rectangle(-1, -2, -3, -4), Color.Red, Color.Red, 360 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Plum, 90 };
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Angle_TestData))]
    public void Ctor_Rectangle_Color_Color_Angle(Rectangle rectangle, Color color1, Color color2, float angle)
    {
        using LinearGradientBrush brush = new(rectangle, color1, color2, angle);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal((angle % 360) == 0, brush.Transform.IsIdentity);
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Angle_TestData))]
    public void Ctor_RectangleF_Color_Color_Angle(Rectangle rectangle, Color color1, Color color2, float angle)
    {
        using LinearGradientBrush brush = new((RectangleF)rectangle, color1, color2, angle);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal((angle % 360) == 0, brush.Transform.IsIdentity);
    }

    public static IEnumerable<object[]> Ctor_Rectangle_Angle_IsAngleScalable_TestData()
    {
        foreach (object[] testData in Ctor_Rectangle_Angle_TestData())
        {
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], true };
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], false };
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Angle_IsAngleScalable_TestData))]
    public void Ctor_Rectangle_Color_Color_Angle_IsAngleScalable(Rectangle rectangle, Color color1, Color color2, float angle, bool isAngleScalable)
    {
        using LinearGradientBrush brush = new(rectangle, color1, color2, angle, isAngleScalable);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal((angle % 360) == 0, brush.Transform.IsIdentity);
    }

    [Theory]
    [MemberData(nameof(Ctor_Rectangle_Angle_IsAngleScalable_TestData))]
    public void Ctor_RectangleF_Color_Color_Angle_IsAngleScalable(Rectangle rectangle, Color color1, Color color2, float angle, bool isAngleScalable)
    {
        using LinearGradientBrush brush = new((RectangleF)rectangle, color1, color2, angle, isAngleScalable);
        Assert.Equal([1], brush.Blend.Factors);
        Assert.Single(brush.Blend.Positions);

        Assert.False(brush.GammaCorrection);
        _ = brush.InterpolationColors;
        Assert.Equal(new Color[] { Color.FromArgb(color1.ToArgb()), Color.FromArgb(color2.ToArgb()) }, brush.LinearColors);
        Assert.Equal(rectangle, brush.Rectangle);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);

        Assert.Equal((angle % 360) == 0, brush.Transform.IsIdentity);
    }

    [Fact]
    public void Ctor_ZeroWidth_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 0, 4), Color.Empty, Color.Empty, 0f));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 0, 4), Color.Empty, Color.Empty, 0f));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 0, 4), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 0, 4), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 0, 4), Color.Empty, Color.Empty, 0, true));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 0, 4), Color.Empty, Color.Empty, 0, true));
    }

    [Fact]
    public void Ctor_ZeroHeight_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 3, 0), Color.Empty, Color.Empty, 0f));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 3, 0), Color.Empty, Color.Empty, 0f));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 3, 0), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 3, 0), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new Rectangle(1, 2, 3, 0), Color.Empty, Color.Empty, 0, true));
        AssertExtensions.Throws<ArgumentException>(null, () => new LinearGradientBrush(new RectangleF(1, 2, 3, 0), Color.Empty, Color.Empty, 0, true));
    }

    [Theory]
    [InlineData(LinearGradientMode.Horizontal - 1)]
    [InlineData(LinearGradientMode.BackwardDiagonal + 1)]
    public void Ctor_InvalidLinearGradientMode_ThrowsEnumArgumentException(LinearGradientMode linearGradientMode)
    {
        Assert.ThrowsAny<ArgumentException>(() => new LinearGradientBrush(new Rectangle(1, 2, 3, 4), Color.Empty, Color.Empty, linearGradientMode));
        Assert.ThrowsAny<ArgumentException>(() => new LinearGradientBrush(new RectangleF(1, 2, 3, 4), Color.Empty, Color.Empty, linearGradientMode));
    }

    [Fact]
    public void Clone_Brush_ReturnsClone()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        LinearGradientBrush clone = Assert.IsType<LinearGradientBrush>(brush.Clone());

        Assert.NotSame(clone, brush);
        Assert.Equal(brush.Blend.Factors, clone.Blend.Factors);
        Assert.Equal(brush.Blend.Positions.Length, clone.Blend.Positions.Length);
        Assert.Equal(brush.LinearColors, clone.LinearColors);
        Assert.Equal(brush.Rectangle, clone.Rectangle);
        Assert.Equal(brush.Transform, clone.Transform);
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.Clone);
    }

    [Fact]
    public void Blend_GetWithInterpolationColorsSet_ReturnsNull()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        ColorBlend blend = new()
        {
            Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
            Positions = [0, 10, 1]
        };

        brush.InterpolationColors = blend;
        Assert.Null(brush.Blend);
    }

    [Theory]
    [InlineData(new float[] { 1 }, new float[] { 1 })]
    [InlineData(new float[] { 0 }, new float[] { 0 })]
    [InlineData(new float[] { float.MaxValue }, new float[] { float.MaxValue })]
    [InlineData(new float[] { float.MinValue }, new float[] { float.MinValue })]
    [InlineData(new float[] { 0.5f, 0.5f }, new float[] { 0, 1 })]
    [InlineData(new float[] { 0.4f, 0.3f, 0.2f }, new float[] { 0, 0.5f, 1 })]
    [InlineData(new float[] { -1 }, new float[] { -1 })]
    [InlineData(new float[] { float.NaN }, new float[] { float.NaN })]
    public void Blend_Set_Success(float[] factors, float[] positions)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Blend blend = new()
        {
            Factors = factors,
            Positions = positions
        };
        brush.Blend = blend;

        Assert.Equal(blend.Factors, brush.Blend.Factors);
        Assert.Equal(factors.Length, brush.Blend.Positions.Length);
    }

    [Theory]
    [InlineData(new float[] { 1, 2 }, new float[] { 1, 2 })]
    [InlineData(new float[] { 1, 2 }, new float[] { 1, 1 })]
    [InlineData(new float[] { 1, 2 }, new float[] { 1, 0 })]
    [InlineData(new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 })]
    public void Blend_InvalidBlend_ThrowsArgumentException(float[] factors, float[] positions)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Blend blend = new()
        {
            Factors = factors,
            Positions = positions
        };
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend = blend);
    }

    [Fact]
    public void Blend_SetNullBlend_ThrowsArgumentNullException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<ArgumentNullException>(() => brush.Blend = null);
    }

    [Fact]
    public void Blend_SetNullBlendFactors_ThrowsArgumentNullException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<ArgumentNullException>(() => brush.Blend = new Blend { Factors = null });
    }

    [Fact]
    public void Blend_SetNullBlendPositions_ThrowsArgumentException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException, ArgumentNullException>("value", "source", () => brush.Blend = new Blend { Factors = new float[2], Positions = null });
    }

    [Fact]
    public void Blend_SetFactorsLengthGreaterThanPositionsLength_ThrowsArgumentOutOfRangeException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>("value", null, () => brush.Blend = new Blend { Factors = new float[2], Positions = new float[1] });
    }

    [Fact]
    public void Blend_SetInvalidBlendFactorsLength_ThrowsArgumentException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend = new Blend { Factors = [], Positions = [] });
    }

    [Fact]
    public void Blend_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Blend = new Blend());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GammaCorrection_Set_GetReturnsExpected(bool gammaCorrection)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true) { GammaCorrection = gammaCorrection };
        Assert.Equal(gammaCorrection, brush.GammaCorrection);
    }

    [Fact]
    public void GammaCorrection_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.GammaCorrection);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.GammaCorrection = true);
    }

    [Fact]
    public void InterpolationColors_SetValid_GetReturnsExpected()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        ColorBlend blend = new()
        {
            Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
            Positions = [0, 10, 1]
        };

        brush.InterpolationColors = blend;
        Assert.Equal(blend.Colors.Select(c => Color.FromArgb(c.ToArgb())), brush.InterpolationColors.Colors);
        Assert.Equal(blend.Positions, brush.InterpolationColors.Positions);
    }

    [Fact]
    public void InterpolationColors_SetWithExistingInterpolationColors_OverwritesInterpolationColors()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true)
        {
            InterpolationColors = new ColorBlend
            {
                Colors = [Color.Wheat, Color.Yellow],
                Positions = [0, 1]
            }
        };
        ColorBlend blend = new()
        {
            Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
            Positions = [0, 0.5f, 1f]
        };
        brush.InterpolationColors = blend;
        Assert.Equal(blend.Colors.Select(c => Color.FromArgb(c.ToArgb())), brush.InterpolationColors.Colors);
        Assert.Equal(blend.Positions, brush.InterpolationColors.Positions);
    }

    [Fact]
    public void InterpolationColors_SetNullBlend_ThrowsArgumentNullException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentNullException>("value", () => brush.InterpolationColors = null);
    }

    [Fact]
    public void InterpolationColors_SetBlendWithNullColors_ThrowsNullReferenceException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<NullReferenceException>(() => brush.InterpolationColors = new ColorBlend { Colors = null });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void InterpolationColors_SetBlendWithTooFewColors_ThrowsArgumentException(int colorsLength)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(
            colorsLength == 0 ? "value" : null,
            () => brush.InterpolationColors = new ColorBlend { Colors = new Color[colorsLength] });
    }

    [Fact]
    public void InterpolationColors_SetNullBlendPositions_ThrowsNullReferenceException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<ArgumentException>(() => brush.InterpolationColors = new ColorBlend { Colors = new Color[2], Positions = null });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void InterpolationColors_SetInvalidBlendPositionsLength_ThrowsArgumentException(int positionsLength)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>("value", () => brush.InterpolationColors = new ColorBlend
        {
            Colors = new Color[2],
            Positions = new float[positionsLength]
        });
    }

    [Theory]
    [InlineData(new float[] { 1, 0, 1 })]
    [InlineData(new float[] { 0, 0, 0 })]
    public void InterpolationColors_InvalidPositions_ThrowsArgumentException(float[] positions)
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.InterpolationColors = new ColorBlend
        {
            Colors = new Color[positions.Length],
            Positions = positions
        });
    }

    [Fact]
    public void InterpolationColors_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true)
        {
            InterpolationColors = new ColorBlend
            {
                Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
                Positions = [0, 0.5f, 1]
            }
        };
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.InterpolationColors);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.InterpolationColors = new ColorBlend
        {
            Colors = new Color[2],
            Positions = [0f, 1f]
        });
    }

    [Fact]
    public void InterpolationColors_SetBlendTriangularShape_ReturnsEmptyInterpolationColors()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true)
        {
            InterpolationColors = new ColorBlend
            {
                Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
                Positions = [0, 0.5f, 1]
            }
        };

        Assert.NotNull(brush.InterpolationColors);
        brush.SetBlendTriangularShape(0.5f);
        brush.InterpolationColors.Colors.Should().HaveCount(1);
        brush.InterpolationColors.Positions.Should().HaveCount(1);
        brush.InterpolationColors.Colors[0].IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void InterpolationColors_SetBlend_ReturnsEmptyInterpolationColors()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true)
        {
            InterpolationColors = new ColorBlend
            {
                Colors = [Color.Red, Color.PeachPuff, Color.PowderBlue],
                Positions = [0, 0.5f, 1]
            }
        };
        Assert.NotNull(brush.InterpolationColors);

        brush.Blend = new Blend
        {
            Factors = new float[1],
            Positions = new float[1]
        };

        brush.InterpolationColors.Colors.Should().HaveCount(1);
        brush.InterpolationColors.Positions.Should().HaveCount(1);
        brush.InterpolationColors.Colors[0].IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void LinearColors_SetValid_GetReturnsExpected()
    {
        Color[] colors = [Color.Red, Color.Blue, Color.AntiqueWhite];
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true) { LinearColors = colors };
        Assert.Equal(colors.Take(2).Select(c => Color.FromArgb(c.ToArgb())), brush.LinearColors);
    }

    [Fact]
    public void LinearColors_SetNull_ThrowsNullReferenceException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<NullReferenceException>(() => brush.LinearColors = null);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void LinearColors_SetInvalidLength_ThrowsIndexOutOfRangeException(int length)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.Throws<IndexOutOfRangeException>(() => brush.LinearColors = new Color[length]);
    }

    [Fact]
    public void LinearColors_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.LinearColors);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.LinearColors = [Color.Red, Color.Wheat]);
    }

    [Fact]
    public void Rectangle_GetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Rectangle);
    }

    [Fact]
    public void Transform_SetValid_GetReturnsExpected()
    {
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true) { Transform = transform };
        Assert.Equal(transform, brush.Transform);
    }

    [Fact]
    public void Transform_SetNull_ThrowsArgumentNullException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentNullException>("value", "matrix", () => brush.Transform = null);
    }

    [Fact]
    public void Transform_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform = new Matrix());
    }

    [Theory]
    [InlineData(WrapMode.Tile)]
    [InlineData(WrapMode.TileFlipX)]
    [InlineData(WrapMode.TileFlipXY)]
    [InlineData(WrapMode.TileFlipY)]
    public void WrapMode_SetValid_GetReturnsExpected(WrapMode wrapMode)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true) { WrapMode = wrapMode };
        Assert.Equal(wrapMode, brush.WrapMode);
    }

    [Theory]
    [InlineData(WrapMode.Tile - 1)]
    [InlineData(WrapMode.Clamp + 1)]
    public void WrapMode_SetInvalid_ThrowsInvalidEnumArgumentException(WrapMode wrapMode)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.ThrowsAny<ArgumentException>(() => brush.WrapMode = wrapMode);
    }

    [Fact]
    public void WrapMode_Clamp_ThrowsArgumentException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode = WrapMode.Clamp);
    }

    [Fact]
    public void WrapMode_GetSetDisposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode = WrapMode.TileFlipX);
    }

    [Fact]
    public void ResetTransform_Invoke_SetsTransformToIdentity()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Assert.False(brush.Transform.IsIdentity);

        brush.ResetTransform();
        Assert.True(brush.Transform.IsIdentity);
    }

    [Fact]
    public void ResetTransform_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.ResetTransform);
    }

    [Fact]
    public void MultiplyTransform_NoOrder_Success()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Multiply(matrix);

        brush.MultiplyTransform(matrix);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend)]
    [InlineData(MatrixOrder.Append)]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void MultiplyTransform_Order_Success(MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        Matrix expectedTransform = brush.Transform;

        if (order is MatrixOrder.Append or MatrixOrder.Prepend)
        {
            expectedTransform.Multiply(matrix, order);
        }
        else
        {
            // Invalid MatrixOrder is interpreted as MatrixOrder.Append.
            expectedTransform.Multiply(matrix, MatrixOrder.Append);
        }

        brush.MultiplyTransform(matrix, order);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_NullMatrix_ThrowsArgumentNullException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null));
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null, MatrixOrder.Append));
    }

    [Fact]
    public void MultiplyTransform_DisposedMatrix_Nop()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        using Matrix transform = brush.Transform;
        Matrix matrix = new();
        matrix.Dispose();

        brush.MultiplyTransform(matrix);
        brush.MultiplyTransform(matrix, MatrixOrder.Append);

        Assert.Equal(transform, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_NonInvertibleMatrix_ThrowsArgumentException()
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        using Matrix matrix = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix, MatrixOrder.Append));
    }

    [Fact]
    public void MultiplyTransform_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(new Matrix()));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(new Matrix(), MatrixOrder.Prepend));
    }

    [Theory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void TranslateTransform_NoOrder_Success(float dx, float dy)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Translate(dx, dy);

        brush.TranslateTransform(dx, dy);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(1, 1, MatrixOrder.Prepend)]
    [InlineData(1, 1, MatrixOrder.Append)]
    [InlineData(0, 0, MatrixOrder.Prepend)]
    [InlineData(0, 0, MatrixOrder.Append)]
    [InlineData(-1, -1, MatrixOrder.Prepend)]
    [InlineData(-1, -1, MatrixOrder.Append)]
    public void TranslateTransform_Order_Success(float dx, float dy, MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Translate(dx, dy, order);

        brush.TranslateTransform(dx, dy, order);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void TranslateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(0, 0, order));
    }

    [Fact]
    public void TranslateTransform_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(0, 0));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(0, 0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void ScaleTransform_NoOrder_Success(float sx, float sy)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Scale(sx, sy);

        brush.ScaleTransform(sx, sy);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(1, 1, MatrixOrder.Prepend)]
    [InlineData(1, 1, MatrixOrder.Append)]
    [InlineData(0, 0, MatrixOrder.Prepend)]
    [InlineData(0, 0, MatrixOrder.Append)]
    [InlineData(-1, -1, MatrixOrder.Prepend)]
    [InlineData(-1, -1, MatrixOrder.Append)]
    public void ScaleTransform_Order_Success(float sx, float sy, MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Scale(sx, sy, order);

        brush.ScaleTransform(sx, sy, order);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void ScaleTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(0, 0, order));
    }

    [Fact]
    public void ScaleTransform_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(0, 0));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(0, 0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(360)]
    public void RotateTransform_NoOrder_Success(float angle)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Rotate(angle);

        brush.RotateTransform(angle);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(1, MatrixOrder.Prepend)]
    [InlineData(1, MatrixOrder.Append)]
    [InlineData(0, MatrixOrder.Prepend)]
    [InlineData(360, MatrixOrder.Append)]
    [InlineData(-1, MatrixOrder.Prepend)]
    [InlineData(-1, MatrixOrder.Append)]
    public void RotateTransform_Order_Success(float angle, MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix expectedTransform = brush.Transform;
        expectedTransform.Rotate(angle, order);

        brush.RotateTransform(angle, order);
        Assert.Equal(expectedTransform, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void RotateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(0, order));
    }

    [Fact]
    public void RotateTransform_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(0));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(1)]
    [InlineData(float.NaN)]
    public void SetSigmalBellShape(float focus)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.SetSigmaBellShape(focus);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void SetSigmalBellShape_InvalidFocus_ThrowsArgumentException(float focus)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(focus));
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(focus, 1));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void SetSigmalBellShape_InvalidScale_ThrowsArgumentException(float scale)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetSigmaBellShape(0.1f, scale));
    }

    [Fact]
    public void SetSigmalBellShape_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetSigmaBellShape(0));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetSigmaBellShape(0, 1));
    }

    [Theory]
    [InlineData(0, new float[] { 1, 0 }, new float[] { 0, 1 })]
    [InlineData(0.5, new float[] { 0, 1, 0 }, new float[] { 0, 0.5f, 1 })]
    [InlineData(1, new float[] { 0, 1 }, new float[] { 0, 1 })]
    public void SetBlendTriangularShape_Success(float focus, float[] expectedFactors, float[] expectedPositions)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 0, true);
        brush.SetBlendTriangularShape(focus);

        Assert.Equal(expectedFactors, brush.Blend.Factors);
        Assert.Equal(expectedPositions, brush.Blend.Positions);
    }

    [Theory]
    [InlineData(0, 1, new float[] { 1, 0 }, new float[] { 0, 1 })]
    [InlineData(0.5, 0, new float[] { 0, 0, 0 }, new float[] { 0, 0.5f, 1 })]
    [InlineData(0.5, 1, new float[] { 0, 1, 0 }, new float[] { 0, 0.5f, 1 })]
    [InlineData(1, 0.5, new float[] { 0, 0.5f }, new float[] { 0, 1 })]
    public void SetBlendTriangularShape_Scale_Success(float focus, float scale, float[] expectedFactors, float[] expectedPositions)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 0, true);
        brush.SetBlendTriangularShape(focus, scale);

        Assert.Equal(expectedFactors, brush.Blend.Factors);
        Assert.Equal(expectedPositions, brush.Blend.Positions);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void SetBlendTriangularShape_InvalidFocus_ThrowsArgumentException(float focus)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(focus));
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(focus, 1));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void SetBlendTriangularShape_InvalidScale_ThrowsArgumentException(float scale)
    {
        using LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        AssertExtensions.Throws<ArgumentException>(null, null, () => brush.SetBlendTriangularShape(0.1f, scale));
    }

    [Fact]
    public void SetBlendTriangularShape_Disposed_ThrowsArgumentException()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetBlendTriangularShape(0));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.SetBlendTriangularShape(0, 1));
    }

    [Fact]
    public void Dispose_MultipleTimes_Success()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        brush.Dispose();
        brush.Dispose();
    }
}
