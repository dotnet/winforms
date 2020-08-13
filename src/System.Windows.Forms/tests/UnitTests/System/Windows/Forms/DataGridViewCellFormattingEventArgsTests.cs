// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewCellFormattingEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Int_Int_Object_Type_DataGridViewCellStyle_TestData()
        {
            yield return new object[] { -1, -1, null, null, null };
            yield return new object[] { 0, 0, "value", typeof(string), new DataGridViewCellStyle() };
            yield return new object[] { 1, 2, "value", typeof(string), new DataGridViewCellStyle() };
        }

        [Theory]
        [MemberData(nameof(Ctor_Int_Int_Object_Type_DataGridViewCellStyle_TestData))]
        public void Ctor_Int_Int_Object_Type_DataGridViewCellStyle(int columnIndex, int rowIndex, object value, Type desiredType, DataGridViewCellStyle cellStyle)
        {
            var e = new DataGridViewCellFormattingEventArgs(columnIndex, rowIndex, value, desiredType, cellStyle);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(value, e.Value);
            Assert.Equal(desiredType, e.DesiredType);
            Assert.Equal(cellStyle, e.CellStyle);
            Assert.False(e.FormattingApplied);
        }

        [Fact]
        public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewCellFormattingEventArgs(-2, 0, "value", typeof(string), null));
        }

        [Fact]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewCellFormattingEventArgs(0, -2, "value", typeof(string), null));
        }

        public static IEnumerable<object[]> CellStyle_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewCellStyle() };
        }

        [Theory]
        [MemberData(nameof(CellStyle_TestData))]
        public void CellStyle_Set_GetReturnsExpected(DataGridViewCellStyle value)
        {
            var e = new DataGridViewCellFormattingEventArgs(1, 2, "value", typeof(string), new DataGridViewCellStyle())
            {
                CellStyle = value
            };
            Assert.Equal(value, e.CellStyle);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FormattingApplied_Set_GetReturnsExpected(bool value)
        {
            var e = new DataGridViewCellFormattingEventArgs(1, 2, "value", typeof(string), new DataGridViewCellStyle())
            {
                FormattingApplied = value
            };
            Assert.Equal(value, e.FormattingApplied);
        }
    }
}
