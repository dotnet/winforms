// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutCellPaintEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_Rectangle_Rectangle_Int_Int_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { graphics, Rectangle.Empty, Rectangle.Empty, -2, -2 };
            yield return new object[] { graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), -1, -1 };
            yield return new object[] { graphics, new Rectangle(-1, -2, -3, -4), new Rectangle(-1, -2, -3, -4), 0, 0 };
            yield return new object[] { graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, 2 };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_Rectangle_Rectangle_Int_Int_TestData))]
        public void Ctor_Graphics_Rectangle_Rectangle_Int_Int(Graphics graphics, Rectangle clipRectangle, Rectangle cellBounds, int column, int row)
        {
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
            Assert.Throws<ArgumentNullException>("graphics", () => new TableLayoutCellPaintEventArgs(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, 2));
        }
    }
}
