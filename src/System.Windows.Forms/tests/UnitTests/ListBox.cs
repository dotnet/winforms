// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListBoxTests
    {
        [Fact]
        public void ListBox_Constructor()
        {
            var lb = new ListBox();

            Assert.NotNull(lb);
            Assert.Equal(0, lb.Location.X);
            Assert.Equal(0, lb.Location.Y);
            Assert.Equal(120, lb.Width);
            Assert.Equal(96, lb.Height);
        }
    }
}
