// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging.Effects.Tests;

public class EffectsTests
{
    [Fact]
    public void SepiaEffect_Apply()
    {
        using Bitmap bitmap = new(10, 10);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.FillRectangle(Brushes.White, new(default, bitmap.Size));
        using SepiaEffect effect = new();
        bitmap.ApplyEffect(effect);

        // We don't really need to check all effect results, just seeing that something changed for one is probably
        // enough until we look at providing alternative implementations.
        bitmap.GetPixel(0, 0).Should().Be(Color.FromArgb(255, 255, 255, 239));
    }

    [Theory]
    [InlineData(CurveChannel.All, 0)]
    [InlineData(CurveChannel.Red, 0)]
    [InlineData(CurveChannel.Green, 0)]
    [InlineData(CurveChannel.Blue, 0)]
    [InlineData(CurveChannel.All, 254)]
    [InlineData(CurveChannel.Red, 254)]
    [InlineData(CurveChannel.Green, 254)]
    [InlineData(CurveChannel.Blue, 254)]
    public void BlackSaturationEffect_Apply(CurveChannel channel, int blackPoint)
    {
        using Bitmap bitmap = new(10, 10);
        using BlackSaturationCurveEffect effect = new(channel, blackPoint);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(255)]
    public void BlackSaturationEffect_Apply_Invalid(int blackPoint)
    {
        Action action = () => _ = new BlackSaturationCurveEffect(CurveChannel.All, blackPoint);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(CurveChannel.All, 1)]
    [InlineData(CurveChannel.Red, 1)]
    [InlineData(CurveChannel.Green, 1)]
    [InlineData(CurveChannel.Blue, 1)]
    [InlineData(CurveChannel.All, 255)]
    [InlineData(CurveChannel.Red, 255)]
    [InlineData(CurveChannel.Green, 255)]
    [InlineData(CurveChannel.Blue, 255)]
    public void WhiteSaturationEffect_Apply(CurveChannel channel, int whitePoint)
    {
        using Bitmap bitmap = new(10, 10);
        using WhiteSaturationCurveEffect effect = new(channel, whitePoint);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(256)]
    public void WhiteSaturationEffect_Apply_Invalid(int whitePoint)
    {
        Action action = () => _ = new WhiteSaturationCurveEffect(CurveChannel.All, whitePoint);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0.0f, true)]
    [InlineData(0.0f, false)]
    [InlineData(256.0f, true)]
    [InlineData(256.0f, false)]
    public void BlurEffect_Apply(float radius, bool expandEdge)
    {
        using Bitmap bitmap = new(10, 10);
        using BlurEffect effect = new(radius, expandEdge);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-0.1f, true)]
    [InlineData(-0.1f, false)]
    [InlineData(257.0f, true)]
    [InlineData(257.0f, false)]
    public void BlurEffect_Apply_Invalid(float radius, bool expandEdge)
    {
        Action action = () => _ = new BlurEffect(radius, expandEdge);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-255, 0)]
    [InlineData(255, 0)]
    [InlineData(0, -100)]
    [InlineData(0, 100)]
    [InlineData(255, 100)]
    [InlineData(-255, -100)]
    public void BrightnessContrastEffect_Apply(int brightnessLevel, int contrastLevel)
    {
        using Bitmap bitmap = new(10, 10);
        using BrightnessContrastEffect effect = new(brightnessLevel, contrastLevel);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-256, 0)]
    [InlineData(256, 0)]
    [InlineData(0, -101)]
    [InlineData(0, 101)]
    public void BrightnessContrastEffect_Apply_Invalid(int brightnessLevel, int contrastLevel)
    {
        Action action = () => _ = new BrightnessContrastEffect(brightnessLevel, contrastLevel);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(-100, 0, 0)]
    [InlineData(100, 0, 0)]
    [InlineData(0, -100, 0)]
    [InlineData(0, 100, 0)]
    [InlineData(0, 0, -100)]
    [InlineData(0, 0, 100)]
    public void ColorBalanceEffect_Apply(int cyanRed, int magentaGreen, int yellowBlue)
    {
        using Bitmap bitmap = new(10, 10);
        using ColorBalanceEffect effect = new(cyanRed, magentaGreen, yellowBlue);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101, 0, 0)]
    [InlineData(101, 0, 0)]
    [InlineData(0, -101, 0)]
    [InlineData(0, 101, 0)]
    [InlineData(0, 0, -101)]
    [InlineData(0, 0, 101)]
    public void ColorBalanceEffect_Apply_Invalid(int cyanRed, int magentaGreen, int yellowBlue)
    {
        Action action = () => _ = new ColorBalanceEffect(cyanRed, magentaGreen, yellowBlue);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(100, 0, 0)]
    [InlineData(0, -100, 0)]
    [InlineData(0, 100, 0)]
    [InlineData(0, 0, 100)]
    public void LevelsEffect_Apply(int highlight, int midtone, int shadow)
    {
        using Bitmap bitmap = new(10, 10);
        using LevelsEffect effect = new(highlight, midtone, shadow);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-1, 0, 0)]
    [InlineData(101, 0, 0)]
    [InlineData(0, -101, 0)]
    [InlineData(0, 101, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, 101)]
    public void LevelsEffect_Apply_Invalid(int highlight, int midtone, int shadow)
    {
        Action action = () => _ = new LevelsEffect(highlight, midtone, shadow);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    public void ColorLookupTableEffect_Apply(byte tableValue)
    {
        using Bitmap bitmap = new(10, 10);

        Span<byte> buffer = stackalloc byte[256];
        buffer.Fill(tableValue);
        using ColorLookupTableEffect effect = new(buffer, buffer, buffer, buffer);

        effect.AlphaLookupTable.Length.Should().Be(256);
        effect.AlphaLookupTable.Span[0].Should().Be(tableValue);
        effect.AlphaLookupTable.Span[255].Should().Be(tableValue);
        effect.BlueLookupTable.Length.Should().Be(256);
        effect.BlueLookupTable.Span[0].Should().Be(tableValue);
        effect.BlueLookupTable.Span[255].Should().Be(tableValue);
        effect.GreenLookupTable.Length.Should().Be(256);
        effect.GreenLookupTable.Span[0].Should().Be(tableValue);
        effect.GreenLookupTable.Span[255].Should().Be(tableValue);
        effect.RedLookupTable.Length.Should().Be(256);
        effect.RedLookupTable.Span[0].Should().Be(tableValue);
        effect.RedLookupTable.Span[255].Should().Be(tableValue);

        bitmap.ApplyEffect(effect);

        // The final values will be padded with zeros
        using ColorLookupTableEffect effect2 = new(buffer[..100], buffer[..1], buffer, buffer);
        bitmap.ApplyEffect(effect);

        effect2.RedLookupTable.Length.Should().Be(256);
        effect2.RedLookupTable.Span[0].Should().Be(tableValue);
        effect2.RedLookupTable.Span[255].Should().Be(0);
        effect2.GreenLookupTable.Length.Should().Be(256);
        effect2.GreenLookupTable.Span[0].Should().Be(tableValue);
        effect2.GreenLookupTable.Span[255].Should().Be(0);
        effect2.BlueLookupTable.Length.Should().Be(256);
        effect2.BlueLookupTable.Span[0].Should().Be(tableValue);
        effect2.BlueLookupTable.Span[255].Should().Be(tableValue);
        effect2.AlphaLookupTable.Length.Should().Be(256);
        effect2.AlphaLookupTable.Span[0].Should().Be(tableValue);
        effect2.AlphaLookupTable.Span[255].Should().Be(tableValue);
    }

    [Fact]
    public void ColorLookupTableEffect_Apply_Invalid()
    {
        Action action = () =>
        {
            Span<byte> buffer = stackalloc byte[257];
            _ = new ColorLookupTableEffect(buffer, buffer, buffer, buffer);
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(1.0f)]
    [InlineData(2.0f)]
    [InlineData(260.0f)]
    [InlineData(float.NaN)]
    public void ColorMatrixEffect_Apply(float value)
    {
        using Bitmap bitmap = new(10, 10);
        Span<float> buffer = stackalloc float[25];
        buffer.Fill(value);
        using ColorMatrixEffect effect = new(new ColorMatrix(buffer));
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void ContrastEffect_Apply(int contrast)
    {
        using Bitmap bitmap = new(10, 10);
        using ContrastCurveEffect effect = new(CurveChannel.All, contrast);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void ContrastEffect_Apply_Invalid(int contrast)
    {
        Action action = () => _ = new ContrastCurveEffect(CurveChannel.All, contrast);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-255)]
    [InlineData(255)]
    public void DensityEffect_Apply(int density)
    {
        using Bitmap bitmap = new(10, 10);
        using DensityCurveEffect effect = new(CurveChannel.All, density);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-257)]
    [InlineData(257)]
    public void DensityEffect_Apply_Invalid(int density)
    {
        Action action = () => _ = new DensityCurveEffect(CurveChannel.All, density);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-255)]
    [InlineData(255)]
    public void ExposureEffect_Apply(int exposure)
    {
        using Bitmap bitmap = new(10, 10);
        using ExposureCurveEffect effect = new(CurveChannel.All, exposure);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-257)]
    [InlineData(257)]
    public void ExposureEffect_Apply_Invalid(int exposure)
    {
        Action action = () => _ = new ExposureCurveEffect(CurveChannel.All, exposure);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void HighlightEffect_Apply(int highlight)
    {
        using Bitmap bitmap = new(10, 10);
        using HighlightCurveEffect effect = new(CurveChannel.All, highlight);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void HighlightEffect_Apply_Invalid(int highlight)
    {
        Action action = () => _ = new HighlightCurveEffect(CurveChannel.All, highlight);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void MidtoneEffect_Apply(int midtone)
    {
        using Bitmap bitmap = new(10, 10);
        using MidtoneCurveEffect effect = new(CurveChannel.All, midtone);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void MidtoneEffect_Apply_Invalid(int midtone)
    {
        Action action = () => _ = new MidtoneCurveEffect(CurveChannel.All, midtone);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void ShadowEffect_Apply(int shadow)
    {
        using Bitmap bitmap = new(10, 10);
        using ShadowCurveEffect effect = new(CurveChannel.All, shadow);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void ShadowEffect_Apply_Invalid(int shadow)
    {
        Action action = () => _ = new ShadowCurveEffect(CurveChannel.All, shadow);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0.0f, 0.0f)]
    [InlineData(256.0f, 100.0f)]
    public void SharpenEffect_Apply(float radius, float amount)
    {
        using Bitmap bitmap = new(10, 10);
        using SharpenEffect effect = new(radius, amount);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-0.1f, 0.0f)]
    [InlineData(257.0f, 0.0f)]
    [InlineData(0.0f, -0.1f)]
    [InlineData(0.0f, 101.0f)]
    public void SharpenEffect_Apply_Invalid(float radius, float amount)
    {
        Action action = () => _ = new SharpenEffect(radius, amount);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(360, -100)]
    [InlineData(360, 100)]
    public void TintEffect_Hue_Apply(int hue, int amount)
    {
        using Bitmap bitmap = new(10, 10);
        using TintEffect effect = new(hue, amount);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-361, 0)]
    [InlineData(361, 0)]
    [InlineData(0, -101)]
    [InlineData(0, 101)]
    public void TintEffect_Hue_Apply_Invalid(int hue, int amount)
    {
        Action action = () => _ = new TintEffect(hue, amount);
        action.Should().Throw<ArgumentException>();
    }
}
