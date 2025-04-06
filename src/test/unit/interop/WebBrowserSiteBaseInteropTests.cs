// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.InteropTests;

public class WebBrowserSiteBaseInteropTests : InteropTestBase
{
    [WinFormsFact]
    public void WebBrowserSiteBase_RunInteropTests()
    {
        SubWebBrowser browser = new();
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
