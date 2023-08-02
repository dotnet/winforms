﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Drawing.Text;
using System.Globalization;
using static System.Drawing.FontConverter;

namespace System.ComponentModel.TypeConverterTests;

public class FontNameConverterTest
{
    [Fact]
    public void TestConvertFrom()
    {
        FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
        // returns "Times" under Linux and "Times New Roman" under Windows
        if (PlatformDetection.IsWindows)
        {
            Assert.Equal("Times New Roman", converter.ConvertFrom("Times") as string);
        }
        else
        {
            Assert.Equal("Times", converter.ConvertFrom("Times") as string);
        }
        Assert.True(converter.GetStandardValuesSupported(), "standard values supported");
        Assert.False(converter.GetStandardValuesExclusive(), "standard values exclusive");
    }

    [Fact]
    public void ExTestConvertFrom_ThrowsNotSupportedException()
    {
        FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(1));
    }
}

public class FontConverterTest
{
    public static char s_separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

    [Theory]
    [MemberData(nameof(TestConvertFormData))]
    public void TestConvertFrom(string input, string expectedName, float expectedSize, GraphicsUnit expectedUnits, FontStyle expectedFontStyle)
    {
        FontConverter converter = new FontConverter();
        Font font = (Font)converter.ConvertFrom(input);

        // Unix fonts
        Assert.Equal(expectedName, font.Name);
        Assert.Equal(expectedSize, font.Size);
        Assert.Equal(expectedUnits, font.Unit);
        Assert.Equal(expectedFontStyle, font.Style);
    }

    [Theory]
    [MemberData(nameof(ArgumentExceptionFontConverterData))]
    public void InvalidInputThrowsArgumentException(string input, string paramName, string netfxParamName)
    {
        FontConverter converter = new FontConverter();
        AssertExtensions.Throws<ArgumentException>(paramName, netfxParamName, () => converter.ConvertFrom(input));
    }

    [Theory]
    [MemberData(nameof(InvalidEnumArgumentExceptionFontConverterData))]
    public void InvalidInputThrowsInvalidEnumArgumentException(string input, string paramName)
    {
        FontConverter converter = new FontConverter();
        Assert.Throws<InvalidEnumArgumentException>(paramName, () => converter.ConvertFrom(input));
    }

    [Fact]
    public void EmptyStringInput()
    {
        FontConverter converter = new FontConverter();
        Font font = (Font)converter.ConvertFrom(string.Empty);
        Assert.Null(font);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotBuiltWithAggressiveTrimming))]
    public void GetFontPropsSorted()
    {
        // The order provided since .NET Framework
        string[] expectedPropNames = new[]
        {
            nameof(Font.Name),
            nameof(Font.Size),
            nameof(Font.Unit),
            nameof(Font.Bold),
            nameof(Font.GdiCharSet),
            nameof(Font.GdiVerticalFont),
            nameof(Font.Italic),
            nameof(Font.Strikeout),
            nameof(Font.Underline),
        };

        FontConverter converter = new FontConverter();
        Font font = new($"Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point);

        PropertyDescriptorCollection props = converter.GetProperties(font);
        string[] propNames = new string[props.Count];

        int index = 0;
        foreach (PropertyDescriptor prop in props)
        {
            propNames[index++] = prop.DisplayName;
        }

        Assert.True(propNames.SequenceEqual(expectedPropNames));
    }

    [Theory]
    [MemberData(nameof(InstanceDescriptorTestData))]
    public void ConvertToInstanceDescriptor(Font font, int expectedArguments)
    {
        try
        {
            FontConverter converter = new();
            InstanceDescriptor descriptor = (InstanceDescriptor)converter.ConvertTo(font, typeof(InstanceDescriptor));
            Assert.Equal(expectedArguments, descriptor.Arguments.Count);
            using Font newFont = (Font)descriptor.Invoke();
            Assert.Equal(font.Name, newFont.Name);
            Assert.Equal(font.Size, newFont.Size);
            Assert.Equal(font.Style, newFont.Style);
            Assert.Equal(font.Unit, newFont.Unit);
            Assert.Equal(font.GdiCharSet, newFont.GdiCharSet);
            Assert.Equal(font.GdiVerticalFont, newFont.GdiVerticalFont);
        }
        finally
        {
            font.Dispose();
        }
    }

    public static TheoryData<Font, int> InstanceDescriptorTestData => new()
    {
        { new Font("Arial", 12.0f), 2 },
        { new Font("Arial", 12.0f, FontStyle.Regular), 2 },
        { new Font("Courier", 8.0f, FontStyle.Italic), 3 },
        { new Font("Courier", 1.0f, FontStyle.Regular, GraphicsUnit.Point), 2 },
        { new Font("Courier", 1.0f, FontStyle.Regular, GraphicsUnit.Inch), 4 },
        { new Font("Courier", 1.0f, FontStyle.Regular, GraphicsUnit.Pixel, gdiCharSet: 2 /* SYMBOL_CHARSET */), 5 },
        { new Font("Courier", 1.0f, FontStyle.Regular, GraphicsUnit.Point, gdiCharSet: 1 /* DEFAULT_CHARSET */), 2 },
        { new Font("Courier", 1.0f, FontStyle.Regular, GraphicsUnit.Pixel, gdiCharSet: 1 /* DEFAULT_CHARSET */, gdiVerticalFont: true), 6 },
    };

    public static TheoryData<string, string, float, GraphicsUnit, FontStyle> TestConvertFormData()
    {
        var data =
            new TheoryData<string, string, float, GraphicsUnit, FontStyle>()
            {
                { $"Courier New", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Courier New{s_separator} 11", "Courier New", 11f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{s_separator} 11px", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{s_separator} 11 px", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{s_separator} 11 px{s_separator} style=Regular", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{s_separator} style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold },
                { $"Courier New{s_separator} 11 px{s_separator} style=Bold{s_separator} Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic },
                { $"Courier New{s_separator} 11 px{s_separator} style=Regular, Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular | FontStyle.Italic },
                { $"Courier New{s_separator} 11 px{s_separator} style=Bold{s_separator} Italic{s_separator} Strikeout", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { $"Arial{s_separator} 11 px{s_separator} style=Bold, Italic, Strikeout", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { $"11px", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"arIAL{s_separator} 10{s_separator} style=bold", "Arial", 10f, GraphicsUnit.Point, FontStyle.Bold },
                { $"Arial{s_separator} 10{s_separator}", "Arial", 10f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{s_separator}", "Arial", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{s_separator} 10{s_separator} style=12", "Arial", 10f, GraphicsUnit.Point, FontStyle.Underline | FontStyle.Strikeout },
                { $"Courier New{s_separator} Style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold }, // FullFramework style keyword is case sensitive.
                { $"11px{s_separator} Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Bold}
            };

        // FullFramework disregards all arguments if the font name is an empty string.
        // Empty string is not an installed font on Windows 7, windows 8 and some versions of windows 10.
        if (EmptyFontPresent)
        {
            data.Add($"{s_separator} 10{s_separator} style=bold", "", 10f, GraphicsUnit.Point, FontStyle.Bold);
        }
        else
        {
            data.Add($"{s_separator} 10{s_separator} style=bold", "Microsoft Sans Serif", 10f, GraphicsUnit.Point, FontStyle.Bold);
        }

        return data;
    }

    private static bool EmptyFontPresent
    {
        get
        {
            using (var installedFonts = new InstalledFontCollection())
            {
                return installedFonts.Families.Select(t => t.Name).Contains(string.Empty);
            }
        }
    }

    public static TheoryData<string, string, string> ArgumentExceptionFontConverterData() => new()
    {
        { $"Courier New{s_separator} 11 px{s_separator} type=Bold{s_separator} Italic", "units", null },
        { $"Courier New{s_separator} {s_separator} Style=Bold", "value", null },
        { $"Courier New{s_separator} 11{s_separator} Style=", "value", null },
        { $"Courier New{s_separator} 11{s_separator} Style=RandomEnum", null, null },
        { $"Arial{s_separator} 10{s_separator} style=bold{s_separator}", "value", null },
        { $"Arial{s_separator} 10{s_separator} style=null", null, null },
        { $"Arial{s_separator} 10{s_separator} style=abc#", null, null },
        { $"Arial{s_separator} 10{s_separator} style=##", null, null },
        { $"Arial{s_separator} 10display{s_separator} style=bold", null, null },
        { $"Arial{s_separator} 10style{s_separator} style=bold", "units", null },
    };

    public static TheoryData<string, string> InvalidEnumArgumentExceptionFontConverterData() => new()
    {
        { $"Arial{s_separator} 10{s_separator} style=56", "style" },
        { $"Arial{s_separator} 10{s_separator} style=-1", "style" },
    };
}

public class FontUnitConverterTest
{
    [Fact]
    public void GetStandardValuesTest()
    {
        FontUnitConverter converter = new FontUnitConverter();
        var values = converter.GetStandardValues();
        Assert.Equal(6, values.Count); // The six supported values of Graphics unit: World, Pixel, Point, Inch, Document, Millimeter.

        foreach (var item in values)
        {
            Assert.NotEqual(GraphicsUnit.Display, (GraphicsUnit)item);
        }
    }

    [Theory]
    [InlineData("Display", GraphicsUnit.Display)]
    [InlineData("Document", GraphicsUnit.Document)]
    [InlineData("Inch", GraphicsUnit.Inch)]
    [InlineData("Millimeter", GraphicsUnit.Millimeter)]
    [InlineData("Pixel", GraphicsUnit.Pixel)]
    [InlineData("Point", GraphicsUnit.Point)]
    [InlineData("World", GraphicsUnit.World)]
    public void CanConvertFrom(string input, GraphicsUnit expected)
    {
        FontUnitConverter converter = new FontUnitConverter();
        GraphicsUnit value = (GraphicsUnit)converter.ConvertFrom(input);
        Assert.Equal(expected, value);
    }
}
