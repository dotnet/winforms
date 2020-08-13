// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class TableLayoutCellPaintEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Rectangle_Rectangle_Int_Int_TestData()
        {
            yield return new object[] { Rectangle.Empty, Rectangle.Empty, -2, -2 };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), -1, -1 };
            yield return new object[] { new Rectangle(-1, -2, -3, -4), new Rectangle(-1, -2, -3, -4), 0, 0 };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, 2 };
        }

        [Theory]
        [MemberData(nameof(Ctor_Rectangle_Rectangle_Int_Int_TestData))]
        public void Ctor_Graphics_Rectangle_Rectangle_Int_Int(Rectangle clipRectangle, Rectangle cellBounds, int column, int row)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            var e = new TableLayoutCellPaintEventArgs(graphics, clipRectangle, cellBounds, column, row);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(clipRectangle, e.ClipRectangle);
            Assert.Equal(cellBounds, e.CellBounds);
            Assert.Equal(column, e.Column);
            Assert.Equal(row, e.Row);
        }

        [Fact]
        public void Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new TableLayoutCellPaintEventArgs(
                (Graphics)null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, 2));
        }
    }
}
