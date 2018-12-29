// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class WebBrowserNavigatedEventArgsTests
    {
        [Fact]
        public void Ctor_Uri()
        {
            var url = new Uri("http://google.com");
            var e = new WebBrowserNavigatedEventArgs(url);
            Assert.Equal(url, e.Url);
        }

        [Fact]
        public void Url_GetNull_ThrowsNullReferenceException()
        {
            var e = new WebBrowserNavigatedEventArgs(null);
            Assert.Throws<NullReferenceException>(() => e.Url);
        }

        [Fact]
        public void Url_GetNull_ThrowsInvalidOperationException()
        {
            var e = new WebBrowserNavigatedEventArgs(new Uri("/relative", UriKind.Relative));
            Assert.Throws<InvalidOperationException>(() => e.Url);
        }
    }
}
