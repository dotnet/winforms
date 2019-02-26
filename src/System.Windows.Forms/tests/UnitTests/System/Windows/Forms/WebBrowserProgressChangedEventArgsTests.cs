// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class WebBrowserProgressChangedEventArgsTests
    {
        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void Ctor_Long_Long(long currentProgress, long maximumProgress)
        {
            var e = new WebBrowserProgressChangedEventArgs(currentProgress, maximumProgress);
            Assert.Equal(currentProgress, e.CurrentProgress);
            Assert.Equal(maximumProgress, e.MaximumProgress);
        }
    }
}
