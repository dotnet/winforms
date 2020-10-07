// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.LinkLabel;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class LinkLabel_LinkLabelAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void LinkLabelAccessibleObject_Ctor_Default()
        {
            using LinkLabel linkLabel = new LinkLabel();
            LinkLabelAccessibleObject accessibleObject = new LinkLabelAccessibleObject(linkLabel);

            Assert.Equal(linkLabel, accessibleObject.Owner);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_ControlType_IsNull()
        {
            using LinkLabel linkLabel = new LinkLabel();
            object actual = linkLabel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Null(actual);
            Assert.False(linkLabel.IsHandleCreated);
        }

        [WinFormsFact]
        public void LinkLabelAccessibleObject_Role_IsStaticText_ByDefault()
        {
            using LinkLabel linkLabel = new LinkLabel();
            // AccessibleRole is not set = Default

            AccessibleRole actual = linkLabel.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StaticText, actual);
            Assert.False(linkLabel.IsHandleCreated);
        }
    }
}
