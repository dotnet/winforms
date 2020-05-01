// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.InteropTests
{
    public class WebBrowserSiteBaseInteropTests : InteropTestBase
    {
        [WinFormsFact]
        public void WebBrowserSiteBase_RunInteropTests()
        {
            var browser = new SubWebBrowser();
            WebBrowserSiteBase site = browser.CreateWebBrowserSiteBase();
            AssertSuccess(Test_WebBrowserSiteBase(site));
        }

        [DllImport(NativeTests, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern string Test_WebBrowserSiteBase([MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        private class SubWebBrowser : WebBrowser
        {
            public new WebBrowserSiteBase CreateWebBrowserSiteBase() => base.CreateWebBrowserSiteBase();
        }
    }
}
