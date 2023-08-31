// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Analyzers.Tests;

public partial class ApplicationConfigTests
{
    public class FontStyleTests
    {
        [Fact]
        public void GetStandardValuesTest()
        {
            var values = Enum.GetValues(typeof(FontStyle));
            Assert.Equal(5, values.Length); // The values of Graphics unit: Regular, Bold, Italic, Underline, Strikeout.
        }

        [Theory]
        [InlineData("Bold", FontStyle.Bold)]
        [InlineData("Italic", FontStyle.Italic)]
        [InlineData("Regular", FontStyle.Regular)]
        [InlineData("Strikeout", FontStyle.Strikeout)]
        [InlineData("Underline", FontStyle.Underline)]
        internal void CanConvertFrom(string input, FontStyle expected)
        {
            FontStyle value = Enum.Parse<FontStyle>(input);
            Assert.Equal(expected, value);
        }
    }
}
