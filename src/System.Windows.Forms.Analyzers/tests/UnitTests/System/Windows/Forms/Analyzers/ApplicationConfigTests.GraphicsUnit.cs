// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Analyzers.Tests;

public partial class ApplicationConfigTests
{
    // Copied from https://github.com/dotnet/runtime/blob/00ee1c18715723e62484c9bc8a14f517455fc3b3/src/libraries/System.Drawing.Common/tests/System/Drawing/FontConverterTests.cs#L203
    public class GraphicsUnitTests
    {
        [Fact]
        public void GetStandardValuesTest()
        {
            var values = Enum.GetValues(typeof(GraphicsUnit));
            Assert.Equal(7, values.Length); // The values of Graphics unit: World, Display, Pixel, Point, Inch, Document, Millimeter.
        }

        [Theory]
        [InlineData("Display", GraphicsUnit.Display)]
        [InlineData("Document", GraphicsUnit.Document)]
        [InlineData("Inch", GraphicsUnit.Inch)]
        [InlineData("Millimeter", GraphicsUnit.Millimeter)]
        [InlineData("Pixel", GraphicsUnit.Pixel)]
        [InlineData("Point", GraphicsUnit.Point)]
        [InlineData("World", GraphicsUnit.World)]
        internal void CanConvertFrom(string input, GraphicsUnit expected)
        {
            GraphicsUnit value = Enum.Parse<GraphicsUnit>(input);
            Assert.Equal(expected, value);
        }
    }
}
