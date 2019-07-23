﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewColumnDividerDoubleClickEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Int_HandledMouseEventArgs_TestData()
        {
            yield return new object[] { -1, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4, true) };
            yield return new object[] { 0, new HandledMouseEventArgs((MouseButtons)1, 0, 0, 0, 0, true) };
            yield return new object[] { 1, new HandledMouseEventArgs((MouseButtons)3, -1, -1, -1, -2, false) };
        }

        [Theory]
        [MemberData(nameof(Ctor_Int_HandledMouseEventArgs_TestData))]
        public void DataGridViewColumnDividerDoubleClickEventArgs_Ctor_Int_HandledMouseEventArgs(int columnIndex, HandledMouseEventArgs mouseE)
        {
            var e = new DataGridViewColumnDividerDoubleClickEventArgs(columnIndex, mouseE);
            Assert.Equal(columnIndex, e.ColumnIndex);
            Assert.Equal(mouseE.Button, e.Button);
            Assert.Equal(mouseE.Clicks, e.Clicks);
            Assert.Equal(mouseE.X, e.X);
            Assert.Equal(mouseE.Y, e.Y);
            Assert.Equal(mouseE.Delta, e.Delta);
            Assert.Equal(mouseE.Handled, e.Handled);
        }

        [Fact]
        public void DataGridViewColumnDividerDoubleClickEventArgs_Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewColumnDividerDoubleClickEventArgs(-2, null));
            Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewColumnDividerDoubleClickEventArgs(-2, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4, true)));
        }

        [Fact]
        public void DataGridViewColumnDividerDoubleClickEventArgs_Ctor_NullE_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("e", () => new DataGridViewColumnDividerDoubleClickEventArgs(1, null));
        }
    }
}
