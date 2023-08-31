// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Windows.Forms.Analyzers;

internal partial record ApplicationConfig
{
    public class FontDescriptor
    {
        public FontDescriptor(string fontName, float emSize, FontStyle style, GraphicsUnit unit)
        {
            Name = fontName;
            Size = emSize;
            Style = style;
            Unit = unit;
        }

        public string Name { get; set; }
        public float Size { get; }
        public FontStyle Style { get; }
        public GraphicsUnit Unit { get; }

        public override string ToString()
        {
            // Sanitize the name - remove anything but alpha-numeric and spaces.
            string name = Regex.Replace(Name, @"[^\w\d ]", string.Empty);

            string fontFamily = string.IsNullOrWhiteSpace(name)
                ? "global::System.Windows.Forms.Control.DefaultFont.FontFamily"
                : $"new global::System.Drawing.FontFamily(\"{name}\")";

            return $"new global::System.Drawing.Font({fontFamily}, {Size.ToString(CultureInfo.InvariantCulture)}f, (global::System.Drawing.FontStyle){(int)Style}, (global::System.Drawing.GraphicsUnit){(int)Unit})";
        }
    }
}
