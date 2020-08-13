// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class CacheVirtualItemsEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void Ctor_Int_Int(int startIndex, int endIndex)
        {
            var e = new CacheVirtualItemsEventArgs(startIndex, endIndex);
            Assert.Equal(startIndex, e.StartIndex);
            Assert.Equal(endIndex, e.EndIndex);
        }
    }
}
