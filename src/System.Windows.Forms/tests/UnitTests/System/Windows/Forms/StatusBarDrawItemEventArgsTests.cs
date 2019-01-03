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
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), null };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, new StatusBarPanel() };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, new StatusBarPanel() };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, new StatusBarPanel() };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, new StatusBarPanel() };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_TestData))]
        public void Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state, StatusBarPanel panel)
        {
            var e = new StatusBarDrawItemEventArgs(graphics, font, rect, index, state, panel);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(font, e.Font);
            Assert.Equal(rect, e.Bounds);
            Assert.Equal(index, e.Index);
            Assert.Equal(state, e.State);
            Assert.Equal(panel, e.Panel);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : SystemColors.WindowText, e.ForeColor);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : SystemColors.Window, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, -2, (DrawItemState)(DrawItemState.None - 1), null, Color.Empty, Color.Empty };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), -1, DrawItemState.None, new StatusBarPanel(), Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(-1, 2, -3, -4), 0, DrawItemState.Selected, new StatusBarPanel(), Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus, new StatusBarPanel(), Color.Red, Color.Blue };
            yield return new object[] { graphics, SystemFonts.DefaultFont, new Rectangle(1, 2, 3, 4), 1, DrawItemState.Focus | DrawItemState.NoFocusRect, new StatusBarPanel(), Color.Red, Color.Blue };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color_TestData))]
        public void Ctor_Graphics_Font_Rectangle_Int_DrawItemState_StatusBarPanel_Color_Color(Graphics graphics, Font font, Rectangle rect, int index, DrawItemState state, StatusBarPanel panel, Color foreColor, Color backColor)
        {
            var e = new StatusBarDrawItemEventArgs(graphics, font, rect, index, state, panel, foreColor, backColor);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(font, e.Font);
            Assert.Equal(rect, e.Bounds);
            Assert.Equal(index, e.Index);
            Assert.Equal(state, e.State);
            Assert.Equal(panel, e.Panel);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : foreColor, e.ForeColor);
            Assert.Equal((state & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : backColor, e.BackColor);
        }
    }
}
