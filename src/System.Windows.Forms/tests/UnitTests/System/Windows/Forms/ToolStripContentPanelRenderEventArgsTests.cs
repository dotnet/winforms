// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripContentPanelRenderEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStripContentPanel_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null };
            yield return new object[] { graphics, new ToolStripContentPanel() };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStripContentPanel_TestData))]
        public void Ctor_Graphics_ToolStripContentPanel(Graphics g, ToolStripContentPanel contentPanel)
        {
            var e = new ToolStripContentPanelRenderEventArgs(g, contentPanel);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(contentPanel, e.ToolStripContentPanel);
            Assert.False(e.Handled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handled_Set_GetReturnsExpected(bool value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new ToolStripContentPanelRenderEventArgs(graphics, new ToolStripContentPanel())
                {
                    Handled = value
                };
                Assert.Equal(value, e.Handled);
            }
        }
    }
}
