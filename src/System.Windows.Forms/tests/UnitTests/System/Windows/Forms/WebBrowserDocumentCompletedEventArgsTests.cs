// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class WebBrowserDocumentCompletedEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void Ctor_Uri()
        {
            var url = new Uri("http://google.com");
            var e = new WebBrowserDocumentCompletedEventArgs(url);
            Assert.Equal(url, e.Url);
        }
    }
}
