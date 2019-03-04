// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class VScrollBarTests
    {
        [Fact]
        public void VScrollBar_Constructor()
        {
            var vsb = new VScrollBar();

            Assert.NotNull(vsb);
            Assert.False(vsb.TabStop);
        }
    }
}
