// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.CSharp.Generators.ApplicationConfiguration;

internal static partial class ProjectFileReader
{
    // Copied from https://github.com/dotnet/runtime/blob/00ee1c18715723e62484c9bc8a14f517455fc3b3/src/libraries/System.Drawing.Common/src/System/Drawing/FontConverter.cs
    internal static class FontConverter
    {
        private const string StylePrefix = "style=";
        private static readonly CultureInfo s_culture = CultureInfo.InvariantCulture;

        public static FontDescriptor? ConvertFrom(string font)
        {
            font = font!.Trim();

            // Expected string format: "name[, size[, units[, style=style1[, style2[...]]]]]"
            // Example using 'vi-VN' culture: "Microsoft Sans Serif, 8,25pt, style=Italic, Bold"
            if (font.Length == 0)
            {
                return null;
            }

            char separator = s_culture.TextInfo.ListSeparator[0]; // For vi-VN: ','
            string fontName = font; // start with the assumption that only the font name was provided.
            string? style = null;
            string? sizeStr;
            float fontSize = PropertyDefaultValue.FontSize;
            FontStyle fontStyle = FontStyle.Regular;
            GraphicsUnit units = GraphicsUnit.Point;

            // Get the index of the first separator (would indicate the end of the name in the string).
            int nameIndex = font.IndexOf(separator);

            if (nameIndex < 0)
            {
                return new FontDescriptor(fontName, fontSize, fontStyle, units);
            }

            // Some parameters are provided in addition to name.
            fontName = font.Substring(0, nameIndex);

            if (nameIndex < font.Length - 1)
            {
                // Get the style index (if any). The size is a bit problematic because it can be formatted differently
                // depending on the culture, we'll parse it last.
                int styleIndex = s_culture.CompareInfo.IndexOf(font, StylePrefix, CompareOptions.IgnoreCase);

                if (styleIndex != -1)
                {
                    // style found.
                    style = font.Substring(styleIndex, font.Length - styleIndex);

                    // Get the mid-substring containing the size information.
                    sizeStr = font.Substring(nameIndex + 1, styleIndex - nameIndex - 1);
                }
                else
                {
                    // no style.
                    sizeStr = font.Substring(nameIndex + 1);
                }

                // Parse size.
                (string? size, string? unit) unitTokens = ParseSizeTokens(sizeStr, separator);

                if (unitTokens.size is not null)
                {
                    try
                    {
                        fontSize = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(null, s_culture, unitTokens.size);
                    }
                    catch
                    {
                        // Exception from converter is too generic.
                        throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                  font,
                                                                  $"name{separator} size[units[{separator} style=style1[{separator} style2{separator} ...]]]"));
                    }
                }

                if (unitTokens.unit is not null)
                {
                    // ParseGraphicsUnits throws an ArgumentException if format is invalid.
                    units = ParseGraphicsUnits(unitTokens.unit);
                }

                if (style is not null)
                {
                    // Parse FontStyle
                    style = style.Substring(6); // style string always starts with style=
                    string[] styleTokens = style.Split(separator);

                    for (int tokenCount = 0; tokenCount < styleTokens.Length; tokenCount++)
                    {
                        string styleText = styleTokens[tokenCount];
                        styleText = styleText.Trim();

                        fontStyle |= (FontStyle)Enum.Parse(typeof(FontStyle), styleText, true);

                        // Enum.IsDefined doesn't do what we want on flags enums...
                        FontStyle validBits = FontStyle.Regular | FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;
                        if ((fontStyle | validBits) != validBits)
                        {
                            throw new InvalidEnumArgumentException(nameof(style), (int)fontStyle, typeof(FontStyle));
                        }
                    }
                }
            }

            return new FontDescriptor(fontName, fontSize, fontStyle, units);
        }

        private static GraphicsUnit ParseGraphicsUnits(string units) =>
            units switch
            {
                // Display unit is not supported
                // https://github.com/dotnet/runtime/blob/01b7e73cd378145264a7cb7a09365b41ed42b240/src/libraries/System.Drawing.Common/src/System/Drawing/FontConverter.cs#L446-L463
                // "display" => GraphicsUnit.Display,
                "doc" => GraphicsUnit.Document,
                "pt" => GraphicsUnit.Point,
                "in" => GraphicsUnit.Inch,
                "mm" => GraphicsUnit.Millimeter,
                "px" => GraphicsUnit.Pixel,
                "world" => GraphicsUnit.World,
                _ => throw new ArgumentException(string.Format(SR.InvalidArgumentValueFontConverter, units), nameof(units)),
            };

        private static (string?, string?) ParseSizeTokens(string text, char separator)
        {
            string? size = null;
            string? units = null;

            text = text.Trim();

            int length = text.Length;
            int splitPoint;

            if (length > 0)
            {
                // text is expected to have a format like " 8,25pt, ". Leading and trailing spaces (trimmed above),
                // last comma, unit and decimal value may not appear.  We need to make it ####.##CC
                for (splitPoint = 0; splitPoint < length; splitPoint++)
                {
                    if (char.IsLetter(text[splitPoint]))
                    {
                        break;
                    }
                }

                char[] trimChars = [separator, ' '];

                if (splitPoint > 0)
                {
                    size = text.Substring(0, splitPoint);
                    // Trimming spaces between size and units.
                    size = size.Trim(trimChars);
                }

                if (splitPoint < length)
                {
                    units = text.Substring(splitPoint);
                    units = units.TrimEnd(trimChars);
                }
            }

            return (size, units);
        }
    }
}
