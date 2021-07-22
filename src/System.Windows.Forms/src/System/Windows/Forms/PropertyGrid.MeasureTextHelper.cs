// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        /// <summary>
        ///  Helper class to support rendering text using either GDI or GDI+.
        /// </summary>
        internal static class MeasureTextHelper
        {
            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font)
                => MeasureTextSimple(owner, g, text, font, new SizeF(0, 0));

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, int width)
                => MeasureText(owner, g, text, font, new SizeF(width, 999999));

            public static SizeF MeasureTextSimple(PropertyGrid owner, Graphics g, string text, Font font, SizeF size)
            {
                SizeF bindingSize;
                if (owner.UseCompatibleTextRendering)
                {
                    bindingSize = g.MeasureString(text, font, size);
                }
                else
                {
                    bindingSize = TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), GetTextRendererFlags());
                }

                return bindingSize;
            }

            public static SizeF MeasureText(PropertyGrid owner, Graphics g, string text, Font font, SizeF size)
            {
                SizeF bindingSize;
                if (owner.UseCompatibleTextRendering)
                {
                    bindingSize = g.MeasureString(text, font, size);
                }
                else
                {
                    TextFormatFlags flags =
                        GetTextRendererFlags() |
                        TextFormatFlags.LeftAndRightPadding |
                        TextFormatFlags.WordBreak |
                        TextFormatFlags.NoFullWidthCharacterBreak;

                    bindingSize = (SizeF)TextRenderer.MeasureText(g, text, font, Size.Ceiling(size), flags);
                }

                return bindingSize;
            }

            public static TextFormatFlags GetTextRendererFlags()
                => TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform;
        }
    }
}
