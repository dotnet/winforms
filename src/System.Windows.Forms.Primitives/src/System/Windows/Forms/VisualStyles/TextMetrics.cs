// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

public struct TextMetrics
{
    public int Height { get; set; }

    public int Ascent { get; set; }

    public int Descent { get; set; }

    public int InternalLeading { get; set; }

    public int ExternalLeading { get; set; }

    public int AverageCharWidth { get; set; }

    public int MaxCharWidth { get; set; }

    public int Weight { get; set; }

    public int Overhang { get; set; }

    public int DigitizedAspectX { get; set; }

    public int DigitizedAspectY { get; set; }

    public char FirstChar { get; set; }

    public char LastChar { get; set; }

    public char DefaultChar { get; set; }

    public char BreakChar { get; set; }

    public bool Italic { get; set; }

    public bool Underlined { get; set; }

    public bool StruckOut { get; set; }

    public TextMetricsPitchAndFamilyValues PitchAndFamily { get; set; }

    public TextMetricsCharacterSet CharSet { get; set; }

    internal static TextMetrics FromTEXTMETRICW(TEXTMETRICW metrics) => new()
    {
        Height = metrics.tmHeight,
        Ascent = metrics.tmAscent,
        Descent = metrics.tmDescent,
        InternalLeading = metrics.tmInternalLeading,
        ExternalLeading = metrics.tmExternalLeading,
        AverageCharWidth = metrics.tmAveCharWidth,
        MaxCharWidth = metrics.tmMaxCharWidth,
        Weight = metrics.tmWeight,
        Overhang = metrics.tmOverhang,
        DigitizedAspectX = metrics.tmDigitizedAspectX,
        DigitizedAspectY = metrics.tmDigitizedAspectY,
        FirstChar = metrics.tmFirstChar,
        LastChar = metrics.tmLastChar,
        DefaultChar = metrics.tmDefaultChar,
        BreakChar = metrics.tmBreakChar,
        Italic = metrics.tmItalic != 0,
        Underlined = metrics.tmUnderlined != 0,
        StruckOut = metrics.tmStruckOut != 0,
        PitchAndFamily = (TextMetricsPitchAndFamilyValues)metrics.tmPitchAndFamily,
        CharSet = (TextMetricsCharacterSet)metrics.tmCharSet
    };
}
