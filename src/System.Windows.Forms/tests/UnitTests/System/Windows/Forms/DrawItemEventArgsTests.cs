// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DrawItemEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1) };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_TestData))]
        public void Ctor_Graphics_Font_Rectangle_Int_DrawItemState(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state)
        {
            var e = new DrawItemEventArgs(graphics, font, rect, index, state);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(font, e.Font);
            Assert.Equal(rect, e.Bounds);
            Assert.Equal(index, e.Index);
            Assert.Equal(state, e.State);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : SystemColors.WindowText, e.ForeColor);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : SystemColors.Window, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), Color.Empty, Color.Empty };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, Color.Red, Color.Blue };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color_TestData))]
        public void Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
        {
            var e = new DrawItemEventArgs(graphics, font, rect, index, state, foreColor, backColor);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(font, e.Font);
            Assert.Equal(rect, e.Bounds);
            Assert.Equal(index, e.Index);
            Assert.Equal(state, e.State);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : foreColor, e.ForeColor);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : backColor, e.BackColor);
        }

        [Fact]
        public void DrawBackground_HasGraphics_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawItemEventArgs(graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None);
                e.DrawBackground();
            }
        }

        [Fact]
        public void DrawBackground_NullGraphics_ThrowsNullReferenceException()
        {
            var e = new DrawItemEventArgs(null, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None);
            Assert.Throws<NullReferenceException>(() => e.DrawBackground());
        }

        [Fact]
        public void DrawFocusRectangle_HasGraphics_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawItemEventArgs(graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.Focus);
                e.DrawFocusRectangle();
            }
        }

        [Theory]
        [InlineData(DrawItemState.None)]
        [InlineData(DrawItemState.Focus | DrawItemState.NoFocusRect)]
        public void DrawFocusRectangle_NullGraphics_Nop(DrawItemState state)
        {
            var e = new DrawItemEventArgs(null, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, state);
            e.DrawFocusRectangle();
        }

        [Fact]
        public void DrawFocusRectangle_NullGraphics_ThrowsArgumentNullException()
        {
            var e = new DrawItemEventArgs(null, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.Focus);
            Assert.Throws<ArgumentNullException>("graphics", () => e.DrawFocusRectangle());
        }
    }
}
