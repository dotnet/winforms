// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.WebBrowser;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class WebBrowser_WebBrowserAccessibleObjectTests
{
    [WinFormsFact]
    public void WebBrowserAccessibleObject_Ctor_Default()
    {
        using WebBrowser webBrowser = new();
        WebBrowserAccessibleObject accessibleObject = (WebBrowserAccessibleObject)webBrowser.AccessibilityObject;

        Assert.Equal(webBrowser, accessibleObject.Owner);
        Assert.False(webBrowser.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "ToolStripContainer1")]
    public void WebBrowserAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, string expected)
    {
        using WebBrowser webBrowser = new WebBrowser
        {
            Name = expected.ToString(),
            AccessibleName = expected.ToString()
        };

        WebBrowserAccessibleObject accessibleObject = (WebBrowserAccessibleObject)webBrowser.AccessibilityObject;
        string value = ((BSTR)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID)).ToStringAndFree();

        Assert.Equal(expected, value);
        Assert.False(webBrowser.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowserAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfControlHasNoFocus()
    {
        using WebBrowser webBrowser = new();

        WebBrowserAccessibleObject accessibleObject = (WebBrowserAccessibleObject)webBrowser.AccessibilityObject;
        bool value = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.False(value);
        Assert.False(webBrowser.IsHandleCreated);
    }
}
