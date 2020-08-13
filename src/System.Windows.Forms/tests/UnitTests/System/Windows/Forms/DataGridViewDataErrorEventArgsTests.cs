// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewDataErrorEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_Exception_Int_Int_DataGridViewDataErrorContexts_TestData()
        {
            yield return new object[] { null, -1, -1, (DataGridViewDataErrorContexts)3 };
            yield return new object[] { new Exception(), 0, 0, DataGridViewDataErrorContexts.Formatting };
            yield return new object[] { new Exception(), 1, 2, DataGridViewDataErrorContexts.Formatting };
        }

        [Theory]
        [MemberData(nameof(Ctor_Exception_Int_Int_DataGridViewDataErrorContexts_TestData))]
        public void Ctor_Exception_Int_Int_DataGridViewDataErrorContexts(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context)
        {
            var e = new DataGridViewDataErrorEventArgs(exception, columnIndex, rowIndex, context);
            Assert.Equal(exception, e.Exception);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(context, e.Context);
            Assert.False(e.Cancel);
            Assert.False(e.ThrowException);
        }

        [Fact]
        public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewDataErrorEventArgs(null, -2, 0, DataGridViewDataErrorContexts.Formatting));
        }

        [Fact]
        public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewDataErrorEventArgs(null, 0, -2, DataGridViewDataErrorContexts.Formatting));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ThrowException_SetFalseWithException_GetReturnsExpected(bool value)
        {
            var e = new DataGridViewDataErrorEventArgs(new Exception(), 1, 2, DataGridViewDataErrorContexts.Formatting) { ThrowException = value };
            Assert.Equal(value, e.ThrowException);
        }

        [Fact]
        public void ThrowException_SetFalseWithoutException_GetReturnsExpected()
        {
            var e = new DataGridViewDataErrorEventArgs(null, 1, 2, DataGridViewDataErrorContexts.Formatting) { ThrowException = false };
            Assert.False(e.ThrowException);
        }

        [Fact]
        public void ThrowException_SetTrueWithoutException_ThrowsArgumentException()
        {
            var e = new DataGridViewDataErrorEventArgs(null, 1, 2, DataGridViewDataErrorContexts.Formatting);
            Assert.Throws<ArgumentException>(null, () => e.ThrowException = true);
        }
    }
}
