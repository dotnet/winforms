// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SplitterTest
    {
        [Fact]
        public void SplitterTest_Constructor()
        {
            var s = new Splitter();

            Assert.NotNull(s);
            Assert.False(s.TabStop);
            Assert.Equal(25, s.MinSize);
            Assert.Equal(25, s.MinExtra);
            Assert.Equal(DockStyle.Left, s.Dock);
        }
    }
}
