// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripArrowRenderEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Rectangle_Color_ArrowDirection_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, Rectangle.Empty, Color.Empty, (ArrowDirection)(ArrowDirection.Down + 1) };
            yield return new object[] { graphics, new ToolStripButton(), new Rectangle(1, 2, 3, 4), Color.Blue, ArrowDirection.Down };
            yield return new object[] { graphics, new ToolStripButton(), new Rectangle(-1, -2, -3, -4), Color.Blue, ArrowDirection.Down };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_Rectangle_Color_ArrowDirection_TestData))]
        public void Ctor_Graphics_ToolStripItem_Rectangle_Color_ArrowDirection(Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
        {
            var e = new ToolStripArrowRenderEventArgs(g, toolStripItem, arrowRectangle, arrowColor, arrowDirection);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(arrowRectangle, e.ArrowRectangle);
            Assert.Equal(toolStripItem, e.Item);
            Assert.Equal(arrowColor, e.ArrowColor);
            Assert.Equal(arrowDirection, e.Direction);
        }

        public static IEnumerable<object[]> ArrowRectangle_TestData()
        {
            yield return new object[] { Rectangle.Empty };
            yield return new object[] { new Rectangle(1, 2, 3, 4) };
            yield return new object[] { new Rectangle(-1, -2, -3, -4) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ArrowRectangle_TestData))]
        public void ArrowRectangle_Set_GetReturnsExpected(Rectangle value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                using var button = new ToolStripButton();
                var e = new ToolStripArrowRenderEventArgs(graphics, button, new Rectangle(1, 2, 3, 4), Color.Blue, ArrowDirection.Down)
                {
                    ArrowRectangle = value
                };
                Assert.Equal(value, e.ArrowRectangle);
            }
        }

        public static IEnumerable<object[]> ArrowColor_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(ArrowColor_TestData))]
        public void ArrowColor_Set_GetReturnsExpected(Color value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                using var button = new ToolStripButton();
                var e = new ToolStripArrowRenderEventArgs(graphics, button, new Rectangle(1, 2, 3, 4), Color.Blue, ArrowDirection.Down)
                {
                    ArrowColor = value
                };
                Assert.Equal(value, e.ArrowColor);
            }
        }

        [WinFormsTheory]
        [InlineData((ArrowDirection)(ArrowDirection.Down + 1))]
        [InlineData(ArrowDirection.Up)]
        public void Direction_Set_GetReturnsExpected(ArrowDirection value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                using var button = new ToolStripButton();
                var e = new ToolStripArrowRenderEventArgs(graphics, button, new Rectangle(1, 2, 3, 4), Color.Blue, ArrowDirection.Down)
                {
                    Direction = value
                };
                Assert.Equal(value, e.Direction);
            }
        }
    }
}
