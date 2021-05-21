// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class PaintEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Rectangle_TestData()
        {
            yield return new object[] { Rectangle.Empty };
            yield return new object[] { new Rectangle(1, 2, 3, 4) };
            yield return new object[] { new Rectangle(-1, -2, -3, -4) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Rectangle_TestData))]
        public void Ctor_Graphics_Rectangle(Rectangle clipRect)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            using var e = new PaintEventArgs(graphics, clipRect);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(clipRect, e.ClipRectangle);
        }

        [Fact]
        public void Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new PaintEventArgs((Graphics)null, new Rectangle(1, 2, 3, 4)));
        }

        [Fact]
        public void Dispose_Invoke_Success()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));
            e.Dispose();
            e.Dispose();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Dispose_InvokeDisposing_Success(bool disposing)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var e = new SubPaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));
            e.DisposeEntry(disposing);
            e.DisposeEntry(disposing);
        }

        [Fact]
        public void GraphicsIdentity()
        {
            // https://github.com/dotnet/winforms/issues/3910
            using var hdc = GdiCache.GetScreenHdc();
            using PaintEventArgs args = new PaintEventArgs(hdc, default);
            Graphics g1 = args.Graphics;
            Graphics g2 = args.Graphics;
            Assert.Same(g1, g2);
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
