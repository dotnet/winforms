// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SplitterCancelEventArgsTests
    {
        [Theory]
        [InlineData(-1, -1, -1, -1)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 2, 3, 4)]
        public void Ctor_Int_Int_Int_Int(int mouseCursorX, int mouseCursorY, int splitX, int splitY)
        {
            var e = new SplitterCancelEventArgs(mouseCursorX, mouseCursorY, splitX, splitY);
            Assert.Equal(mouseCursorX, e.MouseCursorX);
            Assert.Equal(mouseCursorY, e.MouseCursorY);
            Assert.Equal(splitX, e.SplitX);
            Assert.Equal(splitY, e.SplitY);
            Assert.False(e.Cancel);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SplitX_Set_GetReturnsExpected(int value)
        {
            var e = new SplitterCancelEventArgs(1, 2, 3, 4)
            {
                SplitX = value
            };
            Assert.Equal(value, e.SplitX);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SplitY_Set_GetReturnsExpected(int value)
        {
            var e = new SplitterCancelEventArgs(1, 2, 3, 4)
            {
                SplitY = value
            };
            Assert.Equal(value, e.SplitY);
        }
    }
}
