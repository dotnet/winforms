// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;
using static System.Windows.Forms.Generators.ProjectFileReader;

namespace System.Windows.Forms.Generators.Tests
{
    public partial class ProjectFileReaderTests
    {
        public class FontConverterTest
        {
            private static readonly char s_separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

            public static TheoryData<string, string, float, /*GraphicsUnit*/int, /*FontStyle*/int> TestConvertFormData()
                => new()
                {
                    { $"Courier New", "Courier New", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                    { $"Courier New{s_separator} 11", "Courier New", 11f, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                    { $"Arial{s_separator} 11px", "Arial", 11f, (int)GraphicsUnit.Pixel, (int)FontStyle.Regular },
                    { $"Courier New{s_separator} 11 px", "Courier New", 11f, (int)GraphicsUnit.Pixel, (int)FontStyle.Regular },
                    { $"Courier New{s_separator} 11 px{s_separator} style=Regular", "Courier New", 11f, (int)GraphicsUnit.Pixel, (int)FontStyle.Regular },
                    { $"Courier New{s_separator} style=Bold", "Courier New", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Bold },
                    { $"Courier New{s_separator} 11 px{s_separator} style=Bold{s_separator} Italic", "Courier New", 11f, (int)GraphicsUnit.Pixel, (int)(FontStyle.Bold | FontStyle.Italic) },
                    { $"Courier New{s_separator} 11 px{s_separator} style=Regular, Italic", "Courier New", 11f, (int)GraphicsUnit.Pixel, (int)(FontStyle.Regular | FontStyle.Italic) },
                    { $"Courier New{s_separator} 11 px{s_separator} style=Bold{s_separator} Italic{s_separator} Strikeout", "Courier New", 11f, (int)GraphicsUnit.Pixel, (int)(FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout) },
                    { $"Arial{s_separator} 11 px{s_separator} style=Bold, Italic, Strikeout", "Arial", 11f, (int)GraphicsUnit.Pixel, (int)(FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout) },
                    { $"arIAL{s_separator} 10{s_separator} style=bold", "arIAL", 10f, (int)GraphicsUnit.Point, (int)FontStyle.Bold },
                    { $"Arial{s_separator} 10{s_separator}", "Arial", 10f, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                    { $"Arial{s_separator}", "Arial", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                    { $"Arial{s_separator} 10{s_separator} style=12", "Arial", 10f, (int)GraphicsUnit.Point, (int)(FontStyle.Underline | FontStyle.Strikeout) },
                    { $"Courier New{s_separator} Style=Bold", "Courier New", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Bold }, // FullFramework style keyword is case sensitive.
                    { $"{s_separator} 10{s_separator} style=bold", "", 10f, (int)GraphicsUnit.Point, (int)FontStyle.Bold },

                    // NOTE: in .NET runtime these tests will result in FontName='', but the implementation relies on GDI+, which we don't have...
                    { $"11px{s_separator} Style=Bold", $"11px", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Bold },
                    { $"11px", "11px", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                    { $"Style=Bold", "Style=Bold", PropertyDefaultValue.FontSize, (int)GraphicsUnit.Point, (int)FontStyle.Regular },
                };

            [Theory]
            [MemberData(nameof(TestConvertFormData))]
            internal void TestConvertFrom(string input, string expectedName, float expectedSize, GraphicsUnit expectedUnits, FontStyle expectedFontStyle)
            {
                FontDescriptor font = FontConverter.ConvertFrom(input)!;

                Assert.Equal(expectedName, font.Name);
                Assert.Equal(expectedSize, font.Size);
                Assert.Equal(expectedUnits, font.Unit);
                Assert.Equal(expectedFontStyle, font.Style);
            }

            public static TheoryData<string> ArgumentExceptionFontConverterData()
                => new()
                {
                    { $"Courier New{s_separator} 11 px{s_separator} type=Bold{s_separator} Italic" },
                    { $"Courier New{s_separator} {s_separator} Style=Bold" },
                    { $"Courier New{s_separator} 11{s_separator} Style=" },
                    { $"Courier New{s_separator} 11{s_separator} Style=RandomEnum" },
                    { $"Arial{s_separator} 10{s_separator} style=bold{s_separator}" },
                    { $"Arial{s_separator} 10{s_separator} style=null" },
                    { $"Arial{s_separator} 10{s_separator} style=abc#" },
                    { $"Arial{s_separator} 10{s_separator} style=##" },
                    { $"Arial{s_separator} 10display{s_separator} style=bold" },
                    { $"Arial{s_separator} 10style{s_separator} style=bold" },
                };

            [Theory]
            [MemberData(nameof(ArgumentExceptionFontConverterData))]
            public void InvalidInputThrowsArgumentException(string input)
            {
                Assert.Throws<ArgumentException>(() => FontConverter.ConvertFrom(input));
            }

            public static TheoryData<string, string> InvalidEnumArgumentExceptionFontConverterData()
                => new()
                {
                    { $"Arial{s_separator} 10{s_separator} style=56", "style" },
                    { $"Arial{s_separator} 10{s_separator} style=-1", "style" },
                };

            [Theory]
            [MemberData(nameof(InvalidEnumArgumentExceptionFontConverterData))]
            public void InvalidInputThrowsInvalidEnumArgumentException(string input, string paramName)
            {
                Assert.Throws<InvalidEnumArgumentException>(paramName, () => FontConverter.ConvertFrom(input));
            }

            [Fact]
            public void EmptyStringInput()
            {
                FontDescriptor? font = FontConverter.ConvertFrom(string.Empty);
                Assert.Null(font);
            }
        }
    }
}
