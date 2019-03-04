// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellContextMenuStripNeededEventArgsTests
    {
        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int columnIndex, int rowIndex)
        {
            var e = new DataGridViewCellContextMenuStripNeededEventArgs(columnIndex, rowIndex);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Null(e.ContextMenuStrip);
        }

        [Fact]
        public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellContextMenuStripNeededEventArgs(-2, 0));
        }

        [Fact]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellContextMenuStripNeededEventArgs(0, -2));
        }

        public static IEnumerable<object[]> ContextMenuStrip_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [Theory]
        [MemberData(nameof(ContextMenuStrip_TestData))]
        public void ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
        {
            var e = new DataGridViewCellContextMenuStripNeededEventArgs(1, 2)
            {
                ContextMenuStrip = value
            };
            Assert.Equal(value, e.ContextMenuStrip);
        }
    }
}
