// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects.Tests;

[RequiresPreviewFeatures]
public class EffectsTests
{
    [Fact]
    public void SepiaEffect_Apply()
    {
        using Bitmap bitmap = new(10, 10);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.FillRectangle(Brushes.White, new(default, bitmap.Size));
        using var effect = ColorMatrixEffect.SepiaEffect();
        bitmap.ApplyEffect(effect);

        // We don't really need to check all effect results, just seeing that something changed for one is probably
        // enough until we look at providing alternative implementations.
        bitmap.GetPixel(0,0).Should().Be(Color.FromArgb(255, 255, 255, 239));
    }

    [Theory]
    [InlineData(CurveChannel.CurveChannelAll, 0)]
    [InlineData(CurveChannel.CurveChannelRed, 0)]
    [InlineData(CurveChannel.CurveChannelGreen, 0)]
    [InlineData(CurveChannel.CurveChannelBlue, 0)]
    [InlineData(CurveChannel.CurveChannelAll, 254)]
    [InlineData(CurveChannel.CurveChannelRed, 254)]
    [InlineData(CurveChannel.CurveChannelGreen, 254)]
    [InlineData(CurveChannel.CurveChannelBlue, 254)]
    public void BlackSaturationEffect_Apply(CurveChannel channel, int blackPoint)
    {
        using Bitmap bitmap = new(10, 10);
        using BlackSaturationEffect effect = new(channel, blackPoint);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(255)]
    public void BlackSaturationEffect_Apply_Invalid(int blackPoint)
    {
        Action action = () => _ = new BlackSaturationEffect(CurveChannel.CurveChannelAll, blackPoint);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(CurveChannel.CurveChannelAll, 1)]
    [InlineData(CurveChannel.CurveChannelRed, 1)]
    [InlineData(CurveChannel.CurveChannelGreen, 1)]
    [InlineData(CurveChannel.CurveChannelBlue, 1)]
    [InlineData(CurveChannel.CurveChannelAll, 255)]
    [InlineData(CurveChannel.CurveChannelRed, 255)]
    [InlineData(CurveChannel.CurveChannelGreen, 255)]
    [InlineData(CurveChannel.CurveChannelBlue, 255)]
    public void WhiteSaturationEffect_Apply(CurveChannel channel, int whitePoint)
    {
        using Bitmap bitmap = new(10, 10);
        using WhiteSaturationEffect effect = new(channel, whitePoint);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(256)]
    public void WhiteSaturationEffect_Apply_Invalid(int whitePoint)
    {
        Action action = () => _ = new WhiteSaturationEffect(CurveChannel.CurveChannelAll, whitePoint);
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
        bitmap.ApplyEffect(effect);

        // The final values will be padded with zeros
        using ColorLookupTableEffect effect2 = new(buffer[..100], buffer[..1], buffer, buffer);
        bitmap.ApplyEffect(effect);
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
        using ContrastEffect effect = new(CurveChannel.CurveChannelAll, contrast);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void ContrastEffect_Apply_Invalid(int contrast)
    {
        Action action = () => _ = new ContrastEffect(CurveChannel.CurveChannelAll, contrast);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-255)]
    [InlineData(255)]
    public void DensityEffect_Apply(int density)
    {
        using Bitmap bitmap = new(10, 10);
        using DensityEffect effect = new(CurveChannel.CurveChannelAll, density);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-257)]
    [InlineData(257)]
    public void DensityEffect_Apply_Invalid(int density)
    {
        Action action = () => _ = new DensityEffect(CurveChannel.CurveChannelAll, density);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-255)]
    [InlineData(255)]
    public void ExposureEffect_Apply(int exposure)
    {
        using Bitmap bitmap = new(10, 10);
        using ExposureEffect effect = new(CurveChannel.CurveChannelAll, exposure);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-257)]
    [InlineData(257)]
    public void ExposureEffect_Apply_Invalid(int exposure)
    {
        Action action = () => _ = new ExposureEffect(CurveChannel.CurveChannelAll, exposure);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void HighlightEffect_Apply(int highlight)
    {
        using Bitmap bitmap = new(10, 10);
        using HighlightEffect effect = new(CurveChannel.CurveChannelAll, highlight);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void HighlightEffect_Apply_Invalid(int highlight)
    {
        Action action = () => _ = new HighlightEffect(CurveChannel.CurveChannelAll, highlight);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void MidtoneEffect_Apply(int midtone)
    {
        using Bitmap bitmap = new(10, 10);
        using MidtoneEffect effect = new(CurveChannel.CurveChannelAll, midtone);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void MidtoneEffect_Apply_Invalid(int midtone)
    {
        Action action = () => _ = new MidtoneEffect(CurveChannel.CurveChannelAll, midtone);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(100)]
    public void ShadowEffect_Apply(int shadow)
    {
        using Bitmap bitmap = new(10, 10);
        using ShadowEffect effect = new(CurveChannel.CurveChannelAll, shadow);
        bitmap.ApplyEffect(effect);
    }

    [Theory]
    [InlineData(-101)]
    [InlineData(101)]
    public void ShadowEffect_Apply_Invalid(int shadow)
    {
        Action action = () => _ = new ShadowEffect(CurveChannel.CurveChannelAll, shadow);
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
