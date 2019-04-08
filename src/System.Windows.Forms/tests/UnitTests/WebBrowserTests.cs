// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class WebBrowserTests
    {
        // WebBrowser needs to be run in STA
        [StaFact]
        public void WebBrowser_Constructor()
        {
            var wb = new WebBrowser();

            Assert.NotNull(wb);
            Assert.True(wb.AllowNavigation);
        }
    }
}
