// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class VScrollBar_VScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void VScrollBarAccessibleObject_ctor_ThrowsException_IfVScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new VScrollBar.VScrollBarAccessibleObject(null));
        }

        [WinFormsTheory(Skip = "Crash with an unexpected result. See: https://github.com/dotnet/winforms/issues/3856")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3856")]
        [InlineData(true, AccessibleRole.ScrollBar)]
        [InlineData(false, AccessibleRole.None)]
        public void VScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
        {
            using var scrollBar = new VScrollBar();

            if (createControl)
            {
                scrollBar.CreateControl();
            }

            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(accessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var scrollBar = new VScrollBar();
            VScrollBar.VScrollBarAccessibleObject accessibleObject
                = Assert.IsType<VScrollBar.VScrollBarAccessibleObject>(scrollBar.AccessibilityObject);
            Assert.Equal("Vertical", accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_ControlType_IsScrollBar_IfAccessibleRoleIsDefault()
        {
            using VScrollBar scrollBar = new VScrollBar();
            // AccessibleRole is not set = Default

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ScrollBarControlTypeId, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }
    }
}
