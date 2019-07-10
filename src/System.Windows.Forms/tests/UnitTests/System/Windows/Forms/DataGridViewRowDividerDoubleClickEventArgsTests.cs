// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowDividerDoubleClickEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Int_HandledMouseEventArgs_TestData()
        {
            yield return new object[] { -1, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4, true) };
            yield return new object[] { 0, new HandledMouseEventArgs((MouseButtons)1, 0, 0, 0, 0, true) };
            yield return new object[] { 1, new HandledMouseEventArgs((MouseButtons)3, -1, -1, -1, -2, false) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Int_HandledMouseEventArgs_TestData))]
        public void DataGridViewRowDividerDoubleClickEventArgs_Ctor_Int_HandledMouseEventArgs(int rowIndex, HandledMouseEventArgs mouseE)
        {
            var e = new DataGridViewRowDividerDoubleClickEventArgs(rowIndex, mouseE);
            Assert.Equal(rowIndex, e.RowIndex);
            Assert.Equal(mouseE.Button, e.Button);
            Assert.Equal(mouseE.Clicks, e.Clicks);
            Assert.Equal(mouseE.X, e.X);
            Assert.Equal(mouseE.Y, e.Y);
            Assert.Equal(mouseE.Delta, e.Delta);
            Assert.Equal(mouseE.Handled, e.Handled);
        }

        [Fact]
        public void DataGridViewRowDividerDoubleClickEventArgs_Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewRowDividerDoubleClickEventArgs(-2, null));
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewRowDividerDoubleClickEventArgs(-2, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4, true)));
        }

        [Fact]
        public void DataGridViewRowDividerDoubleClickEventArgs_Ctor_NullE_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("e", () => new DataGridViewRowDividerDoubleClickEventArgs(1, null));
        }
    }
}
