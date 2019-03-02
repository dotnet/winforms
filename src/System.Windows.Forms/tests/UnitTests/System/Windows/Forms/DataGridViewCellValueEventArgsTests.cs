// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellValueEventArgsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int columnIndex, int rowIndex)
        {
            var e = new DataGridViewCellValueEventArgs(columnIndex, rowIndex);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Null(e.Value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException(int columnIndex)
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellValueEventArgs(columnIndex, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellValueEventArgs(0, rowIndex));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void Value_Set_GetReturnsExpected(object value)
        {
            var e = new DataGridViewCellValueEventArgs(1, 2)
            {
                Value = value
            };
            Assert.Equal(value, e.Value);
        }
    }
}
