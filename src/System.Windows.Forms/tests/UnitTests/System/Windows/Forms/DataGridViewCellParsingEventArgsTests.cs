// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellParsingEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Int_Int_Object_Type_DataGridViewCellStyle_TestData()
        {
            yield return new object[] { -2, -2, null, null, null };
            yield return new object[] { -1, -1, null, null, null };
            yield return new object[] { 0, 0, "value", typeof(string), new DataGridViewCellStyle() };
            yield return new object[] { 1, 2, "value", typeof(string), new DataGridViewCellStyle() };
        }

        [Theory]
        [MemberData(nameof(Ctor_Int_Int_Object_Type_DataGridViewCellStyle_TestData))]
        public void Ctor_Int_Int_Object_Type_DataGridViewCellStyle(int rowIndex, int columnIndex, object value, Type desiredType, DataGridViewCellStyle inheritedCellStyle)
        {
            var e = new DataGridViewCellParsingEventArgs(rowIndex, columnIndex, value, desiredType, inheritedCellStyle);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(value, e.Value);
            Assert.Equal(desiredType, e.DesiredType);
            Assert.Equal(inheritedCellStyle, e.InheritedCellStyle);
            Assert.False(e.ParsingApplied);
        }

        public static IEnumerable<object[]> InheritedCellStyle_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewCellStyle() };
        }

        [Theory]
        [MemberData(nameof(InheritedCellStyle_TestData))]
        public void InheritedCellStyle_Set_GetReturnsExpected(DataGridViewCellStyle value)
        {
            var e = new DataGridViewCellParsingEventArgs(1, 2, "value", typeof(string), new DataGridViewCellStyle())
            {
                InheritedCellStyle = value
            };
            Assert.Equal(value, e.InheritedCellStyle);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParsingApplied_Set_GetReturnsExpected(bool value)
        {
            var e = new DataGridViewCellParsingEventArgs(1, 2, "value", typeof(string), new DataGridViewCellStyle())
            {
                ParsingApplied = value
            };
            Assert.Equal(value, e.ParsingApplied);
        }
    }
}
