// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ColumnWidthChangingEventArgsTests
    {
        [Theory]
        [InlineData(-1, -1, true)]
        [InlineData(0, 0, false)]
        [InlineData(1, 2, true)]
        public void Ctor_Int_Int_Bool(int columnIndex, int newWidth, bool cancel)
        {
            var e = new ColumnWidthChangingEventArgs(columnIndex, newWidth, cancel);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(newWidth, e.NewWidth);
            Assert.Equal(cancel, e.Cancel);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int columnIndex, int newWidth)
        {
            var e = new ColumnWidthChangingEventArgs(columnIndex, newWidth);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(newWidth, e.NewWidth);
            Assert.False(e.Cancel);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void NewWidth_Set_GetReturnsExpected(int value)
        {
            var e = new ColumnWidthChangingEventArgs(2, 3)
            {
                NewWidth = value
            };
            Assert.Equal(value, e.NewWidth);
        }
    }
}
