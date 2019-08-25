// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class StatusBarDrawItemEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), null, SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, new StatusBarPanel(), SystemColors.WindowText, SystemColors.Window };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, new StatusBarPanel(), SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, new StatusBarPanel(), SystemColors.WindowText, SystemColors.Window };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, new StatusBarPanel(), SystemColors.WindowText, SystemColors.Window };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_TestData))]
        public void StatusBarDrawItemEventArgs_Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel(Font font, Rectangle rect, int index, DrawItemState state, StatusBarPanel panel, Color expectedForeColor, Color expectedBackColor)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new StatusBarDrawItemEventArgs(graphics, font, rect, index, state, panel);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(font, e.Font);
                Assert.Equal(rect, e.Bounds);
                Assert.Equal(index, e.Index);
                Assert.Equal(state, e.State);
                Assert.Same(panel, e.Panel);
                Assert.Equal(expectedForeColor, e.ForeColor);
                Assert.Equal(expectedBackColor, e.BackColor);
            }
        }

        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), null, Color.Empty, Color.Empty, SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, new StatusBarPanel(), Color.Red, Color.Blue, Color.Red, Color.Blue };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, new StatusBarPanel(), Color.Red, Color.Blue, SystemColors.HighlightText, SystemColors.Highlight };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, new StatusBarPanel(), Color.Red, Color.Blue, Color.Red, Color.Blue };
            yield return new object[] { SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, new StatusBarPanel(), Color.Red, Color.Blue, Color.Red, Color.Blue };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color_TestData))]
        public void StatusBarDrawItemEventArgs_Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color(Font font, Rectangle rect, int index, DrawItemState state, StatusBarPanel panel, Color foreColor, Color backColor, Color expectedForeColor, Color expectedBackColor)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new StatusBarDrawItemEventArgs(graphics, font, rect, index, state, panel, foreColor, backColor);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(font, e.Font);
                Assert.Equal(rect, e.Bounds);
                Assert.Equal(index, e.Index);
                Assert.Equal(state, e.State);
                Assert.Same(panel, e.Panel);
                Assert.Equal(expectedForeColor, e.ForeColor);
                Assert.Equal(expectedBackColor, e.BackColor);
            }
        }
    }
}
