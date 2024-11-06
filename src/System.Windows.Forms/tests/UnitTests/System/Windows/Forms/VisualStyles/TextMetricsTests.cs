// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles.Tests;

// NB: doesn't require thread affinity
public class TextMetricsTests
{
    [Fact]
    public void TextMetrics_Ctor_Default()
    {
        TextMetrics metric = default;
        Assert.Equal(0, metric.Ascent);
        Assert.Equal(0, metric.AverageCharWidth);
        Assert.Equal('\0', metric.BreakChar);
        Assert.Equal(TextMetricsCharacterSet.Ansi, metric.CharSet);
        Assert.Equal('\0', metric.DefaultChar);
        Assert.Equal(0, metric.Descent);
        Assert.Equal(0, metric.DigitizedAspectX);
        Assert.Equal(0, metric.DigitizedAspectY);
        Assert.Equal(0, metric.ExternalLeading);
        Assert.Equal('\0', metric.FirstChar);
        Assert.Equal(0, metric.Height);
        Assert.Equal(0, metric.InternalLeading);
        Assert.False(metric.Italic);
        Assert.Equal(0, metric.MaxCharWidth);
        Assert.Equal(0, metric.Overhang);
        Assert.Equal((TextMetricsPitchAndFamilyValues)0, metric.PitchAndFamily);
        Assert.False(metric.StruckOut);
        Assert.False(metric.Underlined);
        Assert.Equal(0, metric.Weight);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_Ascent_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            Ascent = value
        };
        Assert.Equal(value, metric.Ascent);

        // Set same.
        metric.Ascent = value;
        Assert.Equal(value, metric.Ascent);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_AverageCharWidth_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            AverageCharWidth = value
        };
        Assert.Equal(value, metric.AverageCharWidth);

        // Set same.
        metric.AverageCharWidth = value;
        Assert.Equal(value, metric.AverageCharWidth);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCharTheoryData))]
    public void TextMetrics_BreakChar_Set_GetReturnsExpected(char value)
    {
        TextMetrics metric = new()
        {
            BreakChar = value
        };
        Assert.Equal(value, metric.BreakChar);

        // Set same.
        metric.BreakChar = value;
        Assert.Equal(value, metric.BreakChar);
    }

    [Theory]
    [EnumData<TextMetricsCharacterSet>]
    [InvalidEnumData<TextMetricsCharacterSet>]
    public void TextMetrics_CharSet_Set_GetReturnsExpected(TextMetricsCharacterSet value)
    {
        TextMetrics metric = new()
        {
            CharSet = value
        };
        Assert.Equal(value, metric.CharSet);

        // Set same.
        metric.CharSet = value;
        Assert.Equal(value, metric.CharSet);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCharTheoryData))]
    public void TextMetrics_DefaultChar_Set_GetReturnsExpected(char value)
    {
        TextMetrics metric = new()
        {
            DefaultChar = value
        };
        Assert.Equal(value, metric.DefaultChar);

        // Set same.
        metric.DefaultChar = value;
        Assert.Equal(value, metric.DefaultChar);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_Descent_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            Descent = value
        };
        Assert.Equal(value, metric.Descent);

        // Set same.
        metric.Descent = value;
        Assert.Equal(value, metric.Descent);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_DigitizedAspectX_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            DigitizedAspectX = value
        };
        Assert.Equal(value, metric.DigitizedAspectX);

        // Set same.
        metric.DigitizedAspectX = value;
        Assert.Equal(value, metric.DigitizedAspectX);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_DigitizedAspectY_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            DigitizedAspectY = value
        };
        Assert.Equal(value, metric.DigitizedAspectY);

        // Set same.
        metric.DigitizedAspectY = value;
        Assert.Equal(value, metric.DigitizedAspectY);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_ExternalLeading_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            ExternalLeading = value
        };
        Assert.Equal(value, metric.ExternalLeading);

        // Set same.
        metric.ExternalLeading = value;
        Assert.Equal(value, metric.ExternalLeading);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCharTheoryData))]
    public void TextMetrics_FirstChar_Set_GetReturnsExpected(char value)
    {
        TextMetrics metric = new()
        {
            FirstChar = value
        };
        Assert.Equal(value, metric.FirstChar);

        // Set same.
        metric.FirstChar = value;
        Assert.Equal(value, metric.FirstChar);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_Height_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            Height = value
        };
        Assert.Equal(value, metric.Height);

        // Set same.
        metric.Height = value;
        Assert.Equal(value, metric.Height);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_InternalLeading_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            InternalLeading = value
        };
        Assert.Equal(value, metric.InternalLeading);

        // Set same.
        metric.InternalLeading = value;
        Assert.Equal(value, metric.InternalLeading);
    }

    [Theory]
    [BoolData]
    public void TextMetrics_Italic_Set_GetReturnsExpected(bool value)
    {
        TextMetrics metric = new()
        {
            Italic = value
        };
        Assert.Equal(value, metric.Italic);

        // Set same.
        metric.Italic = value;
        Assert.Equal(value, metric.Italic);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetCharTheoryData))]
    public void TextMetrics_LastChar_Set_GetReturnsExpected(char value)
    {
        TextMetrics metric = new()
        {
            LastChar = value
        };
        Assert.Equal(value, metric.LastChar);

        // Set same.
        metric.LastChar = value;
        Assert.Equal(value, metric.LastChar);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_MaxCharWidth_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            MaxCharWidth = value
        };
        Assert.Equal(value, metric.MaxCharWidth);

        // Set same.
        metric.MaxCharWidth = value;
        Assert.Equal(value, metric.MaxCharWidth);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_Overhang_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            Overhang = value
        };
        Assert.Equal(value, metric.Overhang);

        // Set same.
        metric.Overhang = value;
        Assert.Equal(value, metric.Overhang);
    }

    [Theory]
    [EnumData<TextMetricsPitchAndFamilyValues>]
    [InvalidEnumData<TextMetricsPitchAndFamilyValues>]
    public void TextMetrics_PitchAndFamily_GetReturnsExpected(TextMetricsPitchAndFamilyValues value)
    {
        TextMetrics metric = new()
        {
            PitchAndFamily = value
        };
        Assert.Equal(value, metric.PitchAndFamily);

        // Set same.
        metric.PitchAndFamily = value;
        Assert.Equal(value, metric.PitchAndFamily);
    }

    [Theory]
    [BoolData]
    public void TextMetrics_StruckOut_Set_GetReturnsExpected(bool value)
    {
        TextMetrics metric = new()
        {
            StruckOut = value
        };
        Assert.Equal(value, metric.StruckOut);

        // Set same.
        metric.StruckOut = value;
        Assert.Equal(value, metric.StruckOut);
    }

    [Theory]
    [BoolData]
    public void TextMetrics_Underlined_Set_GetReturnsExpected(bool value)
    {
        TextMetrics metric = new()
        {
            Underlined = value
        };
        Assert.Equal(value, metric.Underlined);

        // Set same.
        metric.Underlined = value;
        Assert.Equal(value, metric.Underlined);
    }

    [Theory]
    [IntegerData<int>]
    public void TextMetrics_Weight_Set_GetReturnsExpected(int value)
    {
        TextMetrics metric = new()
        {
            Weight = value
        };
        Assert.Equal(value, metric.Weight);

        // Set same.
        metric.Weight = value;
        Assert.Equal(value, metric.Weight);
    }
}
