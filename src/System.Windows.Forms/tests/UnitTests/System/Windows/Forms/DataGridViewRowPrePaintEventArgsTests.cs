// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowPrePaintEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false };
            yield return new object[] { new DataGridView(), graphics, new Rectangle(-1, -2, -3, -4), new Rectangle(-1, -2, -3, -4), -1, DataGridViewElementStates.Displayed, "", new DataGridViewCellStyle(), true, false };
            yield return new object[] { new DataGridView(), graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 0, (DataGridViewElementStates)7, "errorText", new DataGridViewCellStyle(), true, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool_TestData))]
        public void Ctor_DataGridView_Graphics_Rectangle_Rectangle_Int_DataGridViewElementStates_String_DataGridViewCellStyle_Bool_Bool(DataGridView dataGridView, Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, string errorText, DataGridViewCellStyle inheritedRowStyle, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, clipBounds, rowBounds, rowIndex, rowState, errorText, inheritedRowStyle, isFirstDisplayedRow, isLastVisibleRow);
            Assert.Equal(graphics, e.Graphics);
            Assert.Equal(clipBounds, e.ClipBounds);
            Assert.Equal(rowBounds, e.RowBounds);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(rowState, e.State);
            Assert.Equal(errorText, e.ErrorText);
            Assert.Equal(inheritedRowStyle, e.InheritedRowStyle);
            Assert.Equal(isFirstDisplayedRow, e.IsFirstDisplayedRow);
            Assert.Equal(isLastVisibleRow, e.IsLastVisibleRow);
            Assert.False(e.Handled);
            Assert.Equal(DataGridViewPaintParts.All, e.PaintParts);
        }

        [Fact]
        public void Ctor_NullDataGridView_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("dataGridView", () => new DataGridViewRowPrePaintEventArgs(null, graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false));
            }
        }

        [Fact]
        public void Ctor_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => new DataGridViewRowPrePaintEventArgs(new DataGridView(), null, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false));
        }

        [Fact]
        public void Ctor_NullInheritedRowStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("inheritedRowStyle", () => new DataGridViewRowPrePaintEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, null, false, false));
            }
        }

        public static IEnumerable<object[]> ClipBounds_TestData()
        {
            yield return new object[] { Rectangle.Empty };
            yield return new object[] { new Rectangle(1, 2, 3, 4) };
            yield return new object[] { new Rectangle(-1, -2, -3, -4) };
        }

        [Theory]
        [MemberData(nameof(ClipBounds_TestData))]
        public void ClipBounds_Set_GetReturnsExpected(Rectangle value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DataGridViewRowPrePaintEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false)
                {
                    ClipBounds = value
                };
                Assert.Equal(value, e.ClipBounds);
            }
        }

        [Theory]
        [InlineData(DataGridViewPaintParts.None)]
        [InlineData(DataGridViewPaintParts.All)]
        public void PaintParts_Set_GetReturnsExpected(DataGridViewPaintParts value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DataGridViewRowPrePaintEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false)
                {
                    PaintParts = value
                };
                Assert.Equal(value, e.PaintParts);
            }
        }

        [Theory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void PaintParts_SetInvalidValue_ThrowsArgumentException(DataGridViewPaintParts value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new DataGridViewRowPrePaintEventArgs(new DataGridView(), graphics, Rectangle.Empty, Rectangle.Empty, -2, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<ArgumentException>("value", () => e.PaintParts = value);
            }
        }

        [Fact]
        public void DrawFocus_ValidRowIndex_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.DrawFocus(new Rectangle(1, 2, 3, 4), true);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void DrawFocus_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<InvalidOperationException>(() => e.DrawFocus(new Rectangle(1, 2, 3, 4), true));
            }
        }

        [Fact]
        public void PaintCells_ValidRowIndex_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.PaintCells(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void PaintCells_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<InvalidOperationException>(() => e.PaintCells(new Rectangle(1, 2, 3, 4), DataGridViewPaintParts.None));
            }
        }

        [Fact]
        public void PaintCellsBackground_ValidRowIndex_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.PaintCellsBackground(new Rectangle(1, 2, 3, 4), true);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void PaintCellsBackground_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<InvalidOperationException>(() => e.PaintCellsBackground(new Rectangle(1, 2, 3, 4), true));
            }
        }

        [Fact]
        public void PaintCellsContent_ValidRowIndex_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.PaintCellsContent(new Rectangle(1, 2, 3, 4));
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void PaintCellsContent_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<InvalidOperationException>(() => e.PaintCellsContent(new Rectangle(1, 2, 3, 4)));
            }
        }

        [Fact]
        public void PaintHeader_ValidRowIndexDataGridViewPaintParts_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.PaintHeader(DataGridViewPaintParts.None);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PaintHeader_ValidRowIndexBool_Success(bool paintSelectionBackground)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                e.PaintHeader(paintSelectionBackground);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void PaintHeader_InvalidRowIndex_ThrowsInvalidOperationException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView();
                dataGridView.Columns.Add("name", "text");
                var e = new DataGridViewRowPrePaintEventArgs(dataGridView, graphics, Rectangle.Empty, Rectangle.Empty, rowIndex, DataGridViewElementStates.Displayed, null, new DataGridViewCellStyle(), false, false);
                Assert.Throws<InvalidOperationException>(() => e.PaintHeader(DataGridViewPaintParts.None));
                Assert.Throws<InvalidOperationException>(() => e.PaintHeader(true));
            }
        }
    }
}
