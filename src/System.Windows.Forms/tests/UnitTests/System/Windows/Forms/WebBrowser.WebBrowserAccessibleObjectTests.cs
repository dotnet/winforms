// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.WebBrowser;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class WebBrowser_WebBrowserAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "ToolStripContainer1")]
        public void WebBrowserAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using WebBrowser webBrowser = new WebBrowser
            {
                Name = expected.ToString(),
                AccessibleName = expected.ToString()
            };

            WebBrowserAccessibleObject accessibleObject = (WebBrowserAccessibleObject)webBrowser.AccessibilityObject;
            object value = accessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(webBrowser.IsHandleCreated);
        }

        [WinFormsFact]
        public void WebBrowserAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfControlHasNoFocus()
        {
            using WebBrowser webBrowser = new();

            WebBrowserAccessibleObject accessibleObject = (WebBrowserAccessibleObject)webBrowser.AccessibilityObject;
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.False(value);
            Assert.False(webBrowser.IsHandleCreated);
        }
    }
}
