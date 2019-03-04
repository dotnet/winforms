// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PaintEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_Rectangle_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { graphics, Rectangle.Empty };
            yield return new object[] { graphics, new Rectangle(1, 2, 3, 4) };
            yield return new object[] { graphics, new Rectangle(-1, -2, -3, -4) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Rectangle_TestData))]
        public void Ctor_Graphics_Rectangle(Graphics graphics, Rectangle clipRect)
        {
            var e = new PaintEventArgs(graphics, clipRect);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(clipRect, e.ClipRectangle);
        }

        [Fact]
        public void Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new PaintEventArgs(null, new Rectangle(1, 2, 3, 4)));
        }

        [Fact]
        public void Dispose_Invoke_Success()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            var e = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));
            e.Dispose();
            e.Dispose();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Dispose_InvokeDisposing_Success(bool disposing)
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);
            var e = new SubPaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));
            e.DisposeEntry(disposing);
            e.DisposeEntry(disposing);
        }

        private class SubPaintEventArgs : PaintEventArgs
        {
            public SubPaintEventArgs(Graphics graphics, Rectangle clipRect) : base(graphics, clipRect)
            {
            }

            public void DisposeEntry(bool disposing) => Dispose(disposing);
        }
    }
}
