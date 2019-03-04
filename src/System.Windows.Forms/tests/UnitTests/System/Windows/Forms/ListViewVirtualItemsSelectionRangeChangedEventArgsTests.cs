// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewVirtualItemsSelectionRangeChangedEventArgsTests
    {
        [Theory]
        [InlineData(-2, -2, true)]
        [InlineData(-1, -1, false)]
        [InlineData(0, 0, true)]
        [InlineData(1, 2, true)]
        [InlineData(1, 1, false)]
        public void Ctor_Int_Int_Bool(int startIndex, int endIndex, bool isSelected)
        {
            var e = new ListViewVirtualItemsSelectionRangeChangedEventArgs(startIndex, endIndex, isSelected);
            Assert.Equal(startIndex, e.StartIndex);
            Assert.Equal(endIndex, e.EndIndex);
            Assert.Equal(isSelected, e.IsSelected);
        }

        [Fact]
        public void Ctor_StartIndexGreaterThanEndIndex_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(null, () => new ListViewVirtualItemsSelectionRangeChangedEventArgs(1, 0, false));
        }
    }
}
