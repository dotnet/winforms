// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class LinkClickedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("linkText")]
        public void Ctor_String(string linkText)
        {
            var e = new LinkClickedEventArgs(linkText);
            Assert.Equal(linkText, e.LinkText);
        }
    }
}
