// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DrawItemEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, SystemColors.WindowText, SystemColors.Window };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, SystemColors.WindowText, SystemColors.Window };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, SystemColors.WindowText, SystemColors.Window };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_TestData))]
        public void DrawItemEventArgs_Ctor_Graphics_Font_Rectangle_Int_DrawItemState(Font font, Rectangle rect, int index, DrawItemState state, Color expectedForeColor, Color expectedBackColor)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawItemEventArgs(graphics, font, rect, index, state);
                Assert.Equal(graphics, e.Graphics);
                Assert.Equal(font, e.Font);
                Assert.Equal(rect, e.Bounds);
                Assert.Equal(index, e.Index);
                Assert.Equal(state, e.State);
                Assert.Equal(expectedForeColor, e.ForeColor);
                Assert.Equal(expectedBackColor, e.BackColor);
            }
        }

        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), Color.Empty, Color.Empty };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, Color.Red, Color.Blue };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, Color.Red, Color.Blue };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, Color.Red, Color.Blue };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, Color.Red, Color.Blue };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color_TestData))]
        public void DrawItemEventArgs_Ctor_Graphics_Font_Rectangle_Int_DrawItemState_Color_Color(Font font, Rectangle rect, int index, DrawItemState state, Color foreColor, Color backColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

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
        public void DrawItemEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DrawItemEventArgs(null, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 0, DrawItemState.None));
            Assert.Throws<ArgumentNullException>("graphics", () => new DrawItemEventArgs(null, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 0, DrawItemState.None, Color.Red, Color.Blue));
        }

        public static IEnumerable<object[]> Draw_TestData()
        {
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.None };
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.Selected };
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.Focus };
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.Focus | DrawItemState.Selected };
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.Focus | DrawItemState.NoFocusRect };
            yield return new object[] { new Rectangle(1, 2, 3, 4), DrawItemState.Focus | DrawItemState.NoFocusRect | DrawItemState.Selected };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.None };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.Selected };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.Focus };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.Focus | DrawItemState.Selected };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.Focus | DrawItemState.NoFocusRect };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), DrawItemState.Focus | DrawItemState.NoFocusRect | DrawItemState.Selected };
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawItemEventArgs_DrawBackground_Invoke_Success(Rectangle bounds, DrawItemState state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawItemEventArgs(graphics, SystemFonts.DefaultFont, bounds, -1, state);
                e.DrawBackground();
            }
        }

        [Theory]
        [MemberData(nameof(Draw_TestData))]
        public void DrawItemEventArgs_DrawFocusRectangle_Invoke_Success(Rectangle bounds, DrawItemState state)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DrawItemEventArgs(graphics, SystemFonts.DefaultFont, bounds, -1, state);
                e.DrawFocusRectangle();
            }
        }
    }
}
