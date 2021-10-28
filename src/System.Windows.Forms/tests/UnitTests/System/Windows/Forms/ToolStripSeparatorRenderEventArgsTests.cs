// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSeparatorRenderEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSeparatorRenderEventArgs_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ToolStripSeparatorRenderEventArgs(null, null, true));
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Bool_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            yield return new object[] { graphics, null, true };
            yield return new object[] { graphics, new ToolStripSeparator(), false };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_Bool_TestData))]
        public void Ctor_Graphics_ToolStripItem_Bool(Graphics g, ToolStripSeparator separator, bool vertical)
        {
            var e = new ToolStripSeparatorRenderEventArgs(g, separator, vertical);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(separator, e.Item);
            Assert.Equal(vertical, e.Vertical);
        }
    }
}
