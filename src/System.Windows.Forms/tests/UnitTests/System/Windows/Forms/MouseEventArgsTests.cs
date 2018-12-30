// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MouseEventArgsTests
    {
        [Theory]
        [InlineData(MouseButtons.Left, 1, 2, 3, 4)]
        [InlineData((MouseButtons)1, 0, 0, 0, 0)]
        [InlineData((MouseButtons)3, -1, -1, -1, -2)]
        public void Ctor_MouseButtons_Int_Int_Int_Int(MouseButtons button, int clicks, int x, int y, int delta)
        {
            var e = new MouseEventArgs(button, clicks, x, y, delta);
            Assert.Equal(button, e.Button);
            Assert.Equal(clicks, e.Clicks);
            Assert.Equal(x, e.X);
            Assert.Equal(y, e.Y);
            Assert.Equal(delta, e.Delta);
            Assert.Equal(new Point(x, y), e.Location);
        }
    }
}
