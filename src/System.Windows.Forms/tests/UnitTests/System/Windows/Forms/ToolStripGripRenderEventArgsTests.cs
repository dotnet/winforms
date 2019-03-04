// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripGripRenderEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, new ToolStrip() };
            yield return new object[] { graphics, new ToolStrip() };
            yield return new object[]
            {
                graphics, new ToolStrip
                {
                    LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow,
                    GripStyle = ToolStripGripStyle.Visible
                }
            };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStrip_TestData))]
        public void Ctor_Graphics_ToolStrip(Graphics g, ToolStrip toolStrip)
        {
            var e = new ToolStripGripRenderEventArgs(g, toolStrip);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(toolStrip, e.ToolStrip);
            Assert.Equal(new Rectangle(Point.Empty, toolStrip.Size), e.AffectedBounds);
            Assert.Equal(toolStrip.GripRectangle, e.GripBounds);
            Assert.Equal(toolStrip.GripDisplayStyle, e.GripDisplayStyle);
            Assert.Equal(toolStrip.GripStyle, e.GripStyle);
        }
    }
}
