// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripRenderEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_ToolStrip_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, Rectangle.Empty, SystemColors.Control };
            yield return new object[] { new ToolStrip(), new Rectangle(0, 0, 100, 25), SystemColors.Control };
            yield return new object[] { new ToolStrip(), new Rectangle(0, 0, 100, 25), SystemColors.Control };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ToolStrip_TestData))]
        public void ToolStripRenderEventArgs_Ctor_ToolStrip(ToolStrip toolStrip, Rectangle expectedAffectedBounds, Color expectedBackColor)
        {
            var e = new ToolStripRenderEventArgs(null, toolStrip);

            Assert.Null(e.Graphics);
            Assert.Same(toolStrip, e.ToolStrip);
            Assert.Equal(expectedAffectedBounds, e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_TestData()
        {
            yield return new object[] { new ToolStrip(), new Rectangle(0, 0, 100, 25), SystemColors.Control };
            yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(0, 0, 100, 25), Color.Red };
            yield return new object[] { new ToolStripDropDown(), new Rectangle(0, 0, 100, 25), SystemColors.Menu };
            yield return new object[] { new MenuStrip(), new Rectangle(0, 0, 200, 24), SystemColors.MenuBar };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Graphics_ToolStrip_TestData))]
        public void ToolStripRenderEventArgs_Ctor_Graphics_ToolStrip(ToolStrip toolStrip, Rectangle expectedAffectedBounds, Color expectedBackColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            var e = new ToolStripRenderEventArgs(graphics, toolStrip);

            Assert.Same(graphics, e.Graphics);
            Assert.Same(toolStrip, e.ToolStrip);
            Assert.Equal(expectedAffectedBounds, e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_ToolStrip_Rectangle_Color_TestData()
        {
            yield return new object[] { null, Rectangle.Empty, Color.Empty, SystemColors.Control };
            yield return new object[] { null, Rectangle.Empty, Color.Red, Color.Red };
            yield return new object[] { new ToolStrip(), Rectangle.Empty, Color.Empty, SystemColors.Control };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_ToolStrip_Rectangle_Color_TestData))]
        public void ToolStripRenderEventArgs_Ctor_null_ToolStrip_Rectangle_Color(ToolStrip toolStrip, Rectangle affectedBounds, Color backColor, Color expectedBackColor)
        {
            var e = new ToolStripRenderEventArgs(null, toolStrip, affectedBounds, backColor);

            Assert.Null(e.Graphics);
            Assert.Same(toolStrip, e.ToolStrip);
            Assert.Equal(affectedBounds, e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_Rectangle_Color_TestData()
        {
            yield return new object[] { new ToolStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Control };
            yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Empty, Color.Red };
            yield return new object[] { new ToolStripDropDown(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.Menu };
            yield return new object[] { new MenuStrip(), new Rectangle(1, 2, 3, 4), Color.Empty, SystemColors.MenuBar };
            yield return new object[] { new ToolStrip() { BackColor = Color.Red }, new Rectangle(1, 2, 3, 4), Color.Blue, Color.Blue };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Graphics_ToolStrip_Rectangle_Color_TestData))]
        public void ToolStripRenderEventArgs_Ctor_Graphics_ToolStrip_Rectangle_Color(ToolStrip toolStrip, Rectangle affectedBounds, Color backColor, Color expectedBackColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            var e = new ToolStripRenderEventArgs(graphics, toolStrip, affectedBounds, backColor);

            Assert.Same(graphics, e.Graphics);
            Assert.Same(toolStrip, e.ToolStrip);
            Assert.Equal(affectedBounds, e.AffectedBounds);
            Assert.Equal(expectedBackColor, e.BackColor);
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

        [WinFormsTheory]
        [MemberData(nameof(ConnectedArea_Empty_TestData))]
        public void ToolStripRenderEventArgs_ConnectedArea_Get_ReturnsEmpty(ToolStrip toolStrip)
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
