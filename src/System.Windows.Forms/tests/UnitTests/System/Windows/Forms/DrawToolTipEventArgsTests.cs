// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawToolTipEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, null, Rectangle.Empty, null, Color.Empty, Color.Empty, null };
            yield return new object[] { graphics, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "", Color.Red, Color.Blue, SystemFonts.DefaultFont };
            yield return new object[] { graphics, new SubWin32Window(), new Button(), new Rectangle(-1, -2, -3, -4), "toolTipText", Color.Red, Color.Blue, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font_TestData))]
        public void Ctor_Graphics_IWin32Window_Control_Rectangle_String_Color_Color_Font(Graphics graphics, IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
        {
            var e = new DrawToolTipEventArgs(graphics, associatedWindow, associatedControl, bounds, toolTipText, backColor, foreColor, font);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(associatedWindow, e.AssociatedWindow);
            Assert.Equal(associatedControl, e.AssociatedControl);
            Assert.Equal(bounds, e.Bounds);
            Assert.Equal(toolTipText, e.ToolTipText);
            Assert.Equal(font, e.Font);
        }

        [Fact]
        public void DrawBackground_Invoke_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont);
                e.DrawBackground();
            }
        }

        [Fact]
        public void DrawBackground_NullGraphics_ThrowsNullReferenceException()
        {
            var e = new DrawToolTipEventArgs(null, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont);
            Assert.Throws<NullReferenceException>(() => e.DrawBackground());
        }

        public static IEnumerable<object[]> DrawText_TestData()
        {
            yield return new object[] { "tooltipText", SystemFonts.DefaultFont };
            yield return new object[] { "tooltipText", null };
            yield return new object[] { "", SystemFonts.DefaultFont };
            yield return new object[] { null, SystemFonts.DefaultFont };
        }

        [Theory]
        [MemberData(nameof(DrawText_TestData))]
        public void DrawText_HasGraphicsWithoutFlags_Success(string tooltipText, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), tooltipText, Color.Red, Color.Blue, font);
                e.DrawText();
            }
        }

        [Theory]
        [MemberData(nameof(DrawText_TestData))]
        public void DrawText_HasGraphicsWithFlags_Success(string tooltipText, Font font)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), tooltipText, Color.Red, Color.Blue, font);
                e.DrawText(TextFormatFlags.Bottom);
            }
        }

        [Fact]
        public void DrawText_NullGraphics_ThrowsArgumentNullException()
        {
            var e = new DrawToolTipEventArgs(null, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont);
            Assert.Throws<ArgumentNullException>("dc", () => e.DrawText());
            Assert.Throws<ArgumentNullException>("dc", () => e.DrawText(TextFormatFlags.Left));
        }

        [Fact]
        public void DrawBorder_Invoke_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawToolTipEventArgs(graphics, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont);
                e.DrawBorder();
            }
        }

        [Fact]
        public void DrawBorder_NullGraphics_ThrowsArgumentNullException()
        {
            var e = new DrawToolTipEventArgs(null, new SubWin32Window(), new Button(), new Rectangle(1, 2, 3, 4), "tooltipText", Color.Red, Color.Blue, SystemFonts.DefaultFont);
            Assert.Throws<ArgumentNullException>("graphics", () => e.DrawBorder());
        }

        private class SubWin32Window : IWin32Window
        {
            public IntPtr Handle { get; }
        }
    }
}
