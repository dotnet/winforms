// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SplitContainerTests
    {
        [Fact]
        public void SplitContainer_Constructor()
        {
            var sc = new SplitContainer();

            Assert.NotNull(sc);
            Assert.NotNull(sc.Panel1);
            Assert.Equal(sc, sc.Panel1.Owner);
            Assert.NotNull(sc.Panel2);
            Assert.Equal(sc, sc.Panel2.Owner);
            Assert.False(sc.SplitterRectangle.IsEmpty);
        }
    }
}
