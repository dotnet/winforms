// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellPaintingEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_Int_DataGridViewElementStates_Object_Object_String_DataGridViewCellStyle_DataGridViewAdvancedBorderStyle_DataGridViewPaintParts_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, -2, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All };
            yield return new object[] { new DataGridView(), graphics, new Rectangle(-1, -2, -3, -4), new Rectangle(-1, -2, -3, -4), -1, -1, DataGridViewElementStates.Displayed, "value", "formattedValue", "", new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle(), DataGridViewPaintParts.All };
            yield return new object[] { new DataGridView(), graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 0, 1, (DataGridViewElementStates)7, "value", "formattedValue", "errorText", new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle(), DataGridViewPaintParts.All };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_Int_DataGridViewElementStates_Object_Object_String_DataGridViewCellStyle_DataGridViewAdvancedBorderStyle_DataGridViewPaintParts_TestData))]
        public void DataGridViewCellPaintingEventArgs_Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_Int_DataGridViewElementStates_Object_Object_String_DataGridViewCellStyle_DataGridViewAdvancedBorderStyle_DataGridViewPaintParts(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, int columnIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, clipBounds, cellBounds, rowIndex, columnIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(clipBounds, e.ClipBounds);
            Assert.Equal(cellBounds, e.CellBounds);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(cellState, e.State);
            Assert.Equal(value, e.Value);
            Assert.Equal(formattedValue, e.FormattedValue);
            Assert.Equal(errorText, e.ErrorText);
            Assert.Equal(cellStyle, e.CellStyle);
            Assert.Equal(advancedBorderStyle, e.AdvancedBorderStyle);
            Assert.Equal(paintParts, e.PaintParts);
            Assert.False(e.Handled);
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_Ctor_NullDataGridView_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("dataGridView", () => new DataGridViewCellPaintingEventArgs(null, graphics, Rectangle.Empty, Rectangle.Empty, -2, -2, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All));
            }
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DataGridViewCellPaintingEventArgs(new DataGridView(), null, Rectangle.Empty, Rectangle.Empty, -2, -2, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_Ctor_NullCellStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("cellStyle", () => new DataGridViewCellPaintingEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, -2, DataGridViewElementStates.Displayed, null, null, null, null, null, DataGridViewPaintParts.All));
            }
        }

        [Theory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void DataGridViewCellPaintingEventArgs_Ctor_InvalidPaintParts_ThrowsArgumentException(DataGridViewPaintParts paintParts)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentException>("paintParts", () => new DataGridViewCellPaintingEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, -2, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, paintParts));
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public void DataGridViewCellPaintingEventArgs_Paint_ValidRowAndColumnIndex_Success(int rowIndex, int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle(), DataGridViewPaintParts.All);
                e.Paint(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None);
            }
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_Paint_NullAdvancedBorderStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<ArgumentNullException>("advancedBorderStyle", () => e.Paint(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_Paint_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.Paint(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_Paint_InvalidColumnIndex_ThrowsInvalidOperationException(int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.Paint(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None));
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public void DataGridViewCellPaintingEventArgs_PaintBackground_ValidRowAndColumnIndex_Success(int rowIndex, int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle(), DataGridViewPaintParts.All);
                e.PaintBackground(new Rectangle(1, 2, 3, 4), true);
            }
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_PaintBackground_NullAdvancedBorderStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<ArgumentNullException>("advancedBorderStyle", () => e.PaintBackground(new Rectangle(1, 2, 3, 4), true));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_PaintBackground_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.PaintBackground(new Rectangle(1, 2, 3, 4), true));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_PaintBackground_InvalidColumnIndex_ThrowsInvalidOperationException(int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.PaintBackground(new Rectangle(1, 2, 3, 4), true));
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public void DataGridViewCellPaintingEventArgs_PaintContent_ValidRowAndColumnIndex_Success(int rowIndex, int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle(), DataGridViewPaintParts.All);
                e.PaintContent(new Rectangle(1, 2, 3, 4));
            }
        }

        [Fact]
        public void DataGridViewCellPaintingEventArgs_PaintContent_NullAdvancedBorderStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<ArgumentNullException>("advancedBorderStyle", () => e.PaintContent(new Rectangle(1, 2, 3, 4)));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_PaintContent_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, 0, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.PaintContent(new Rectangle(1, 2, 3, 4)));
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCellPaintingEventArgs_PaintContent_InvalidColumnIndex_ThrowsInvalidOperationException(int columnIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewCellPaintingEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, columnIndex, DataGridViewElementStates.Displayed, null, null, null, new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
                Assert.Throws<InvalidOperationException>(() => e.PaintContent(new Rectangle(1, 2, 3, 4)));
            }
        }
    }
}
