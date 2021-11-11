﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Windows.Forms.Analyzers
{
    internal partial class ApplicationConfig
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
                    ? "Control.DefaultFont.FontFamily"
                    : $"new FontFamily(\"{name}\")";

                return $"new Font({fontFamily}, {Size.ToString(CultureInfo.InvariantCulture)}f, (FontStyle){(int)Style}, (GraphicsUnit){(int)Unit})";
            }
        }
    }
}
