// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class SplitterEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(-1, -1, -1, -1)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 2, 3, 4)]
        public void Ctor_Int_Int_Int_Int(int x, int y, int splitX, int splitY)
        {
            var e = new SplitterEventArgs(x, y, splitX, splitY);
            Assert.Equal(x, e.X);
            Assert.Equal(y, e.Y);
            Assert.Equal(splitX, e.SplitX);
            Assert.Equal(splitY, e.SplitY);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void SplitX_Set_GetReturnsExpected(int value)
        {
            var e = new SplitterEventArgs(1, 2, 3, 4)
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
            var e = new SplitterEventArgs(1, 2, 3, 4)
            {
                SplitY = value
            };
            Assert.Equal(value, e.SplitY);
        }
    }
}
