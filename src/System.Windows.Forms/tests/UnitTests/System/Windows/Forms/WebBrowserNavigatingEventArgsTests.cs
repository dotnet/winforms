// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class WebBrowserNavigatingEventArgsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("targetFrameName")]
        public void Ctor_Uri(string targetFrameName)
        {
            var url = new Uri("http://google.com");
            var e = new WebBrowserNavigatingEventArgs(url, targetFrameName);
            Assert.Equal(url, e.Url);
            Assert.Equal(targetFrameName, e.TargetFrameName);
        }
    }
}
