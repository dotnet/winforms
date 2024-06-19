// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Xunit;
using Xunit.Abstractions;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.Analyzers.Tests;

public partial class ApplicationConfigTests
{
    public class FontDescriptorTests
    {
        private readonly ITestOutputHelper _output;

        public FontDescriptorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void FontDescriptor_ctor()
        {
            FontDescriptor descriptor = new(
                fontName: "fontName",
                emSize: 10f,
                style: FontStyle.Bold | FontStyle.Italic,
                unit: GraphicsUnit.Point);

            Assert.Equal("fontName", descriptor.Name);
            Assert.Equal(10f, descriptor.Size);
            Assert.Equal(FontStyle.Bold | FontStyle.Italic, descriptor.Style);
            Assert.Equal(GraphicsUnit.Point, descriptor.Unit);
        }

        [Theory]
        [InlineData("", "new global::System.Drawing.Font(global::System.Windows.Forms.Control.DefaultFont.FontFamily, 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData(" ", "new global::System.Drawing.Font(global::System.Windows.Forms.Control.DefaultFont.FontFamily, 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("\t", "new global::System.Drawing.Font(global::System.Windows.Forms.Control.DefaultFont.FontFamily, 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("fontName", "new global::System.Drawing.Font(new global::System.Drawing.FontFamily(\"fontName\"), 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("\"fontName\"", "new global::System.Drawing.Font(new global::System.Drawing.FontFamily(\"fontName\"), 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("Name with \tspaces", "new global::System.Drawing.Font(new global::System.Drawing.FontFamily(\"Name with spaces\"), 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("Name with 'quotes'", "new global::System.Drawing.Font(new global::System.Drawing.FontFamily(\"Name with quotes\"), 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        [InlineData("Name with \r\n lines", "new global::System.Drawing.Font(new global::System.Drawing.FontFamily(\"Name with  lines\"), 10f, (global::System.Drawing.FontStyle)3, (global::System.Drawing.GraphicsUnit)3)")]
        public void FontDescriptor_ToString(string fontName, string expected)
        {
            FontDescriptor descriptor = new(
                fontName: fontName,
                emSize: 10f,
                style: FontStyle.Bold | FontStyle.Italic,
                unit: GraphicsUnit.Point);

            _output.WriteLine(descriptor.ToString());
            Assert.Equal(expected, descriptor.ToString());
        }

        [Theory]
        [InlineData("ar-SA")]
        [InlineData("en-US")]
        [InlineData("es-ES")]
        [InlineData("fr-FR")]
        [InlineData("hi-IN")]
        [InlineData("ja-JP")]
        [InlineData("ru-RU")]
        [InlineData("tr-TR")]
        [InlineData("zh-CN")]
        public void FontDescriptor_ToString_culture_agnostic(string cultureName)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            FontDescriptor descriptor = new(
                fontName: "Microsoft Sans Serif",
                emSize: 8.25f,
                style: FontStyle.Bold | FontStyle.Italic,
                unit: GraphicsUnit.Point);

            _output.WriteLine(descriptor.ToString());

            Assert.Equal(
                expected: "new global::System.Drawing.Font(" +
                          "new global::System.Drawing.FontFamily(\"Microsoft Sans Serif\"), " +
                          "8.25f, (global::System.Drawing.FontStyle)3, " +
                          "(global::System.Drawing.GraphicsUnit)3)",
                actual: descriptor.ToString());
        }
    }
}
