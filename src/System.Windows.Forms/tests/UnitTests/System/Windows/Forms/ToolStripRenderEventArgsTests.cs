// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripRenderEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, new ToolStrip(), SystemColors.Control };
            yield return new object[] { graphics, new ToolStrip(), SystemColors.Control };
            yield return new object[] { graphics, new ToolStrip() { BackColor = Color.Red }, Color.Red };
            yield return new object[] { graphics, new ToolStripDropDown(), SystemColors.Menu };
            yield return new object[] { graphics, new MenuStrip(), SystemColors.MenuBar };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStrip_TestData))]
        public void Ctor_Graphics_ToolStrip(Graphics g, ToolStrip toolStrip, Color expectedBackColor)
        {
            var e = new ToolStripRenderEventArgs(g, toolStrip);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(toolStrip, e.ToolStrip);
            Assert.Equal(new Rectangle(Point.Empty, toolStrip.Size), e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_Rectangle_Color_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, new ToolStrip(), Rectangle.Empty, Color.Empty, SystemColors.Control };
            yield return new object[] { graphics, new ToolStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Control };
            yield return new object[] { graphics, new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Empty, Color.Red };
            yield return new object[] { graphics, new ToolStripDropDown(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Menu };
            yield return new object[] { graphics, new MenuStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.MenuBar };
            yield return new object[] { graphics, new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Blue, Color.Blue };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStrip_Rectangle_Color_TestData))]
        public void Ctor_Graphics_ToolStrip_Rectangle_Color(Graphics g, ToolStrip toolStrip, Rectangle affectedBounds, Color backColor, Color expectedBackColor)
        {
            var e = new ToolStripRenderEventArgs(g, toolStrip, affectedBounds, backColor);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(toolStrip, e.ToolStrip);
            Assert.Equal(affectedBounds, e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
        }

        [Fact]
        public void Ctor_NullToolStrip_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<NullReferenceException>(() => new ToolStripRenderEventArgs(graphics, null));
            }
        }

        [Fact]
        public void BackColor_GetWithNullToolStripNonEmptyColor_Nop()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new ToolStripRenderEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), Color.Red);
                Assert.Equal(Color.Red, e.BackColor);
            }
        }

        [Fact]
        public void BackColor_GetWithNullToolStripEmptyColor_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new ToolStripRenderEventArgs(graphics, null, new Rectangle(1, 2, 3, 4), Color.Empty);
                Assert.Throws<NullReferenceException>(() => e.BackColor);
            }
        }

        public static IEnumerable<object[]> ConnectedArea_Empty_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
            yield return new object[] { new ToolStripDropDown() };
            yield return new object[] { new ToolStripOverflow(new ToolStripButton()) };
            yield return new object[] { new ToolStripOverflow(new ToolStripDropDownButton()) };
        
            var ownedDropDownItem = new ToolStripDropDownButton();
            var owner = new ToolStrip();
            owner.Items.Add(ownedDropDownItem);
            yield return new object[] { new ToolStripOverflow(ownedDropDownItem) };
            yield return new object[] { new ToolStripOverflow(owner.OverflowButton) };
        }

        [Theory]
        [MemberData(nameof(ConnectedArea_Empty_TestData))]
        public void ConnectedArea_Get_ReturnsEmpty(ToolStrip toolStrip)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new ToolStripRenderEventArgs(graphics, toolStrip, new Rectangle(1, 2, 3, 4), Color.Empty);
                Assert.Equal(Rectangle.Empty, e.ConnectedArea);
            }
        }
    }
}
