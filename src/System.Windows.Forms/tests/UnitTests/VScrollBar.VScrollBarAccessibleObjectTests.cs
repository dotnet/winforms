// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

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
            using var vertScrollBar = new VScrollBar();

            if (createControl)
            {
                vertScrollBar.CreateControl();
            }

            AccessibleObject accessibleObject = vertScrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(accessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, vertScrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var vertScrollBar = new VScrollBar();
            VScrollBar.VScrollBarAccessibleObject accessibleObject
                = Assert.IsType<VScrollBar.VScrollBarAccessibleObject>(vertScrollBar.AccessibilityObject);
            Assert.Equal("Vertical", accessibleObject.Name);
            Assert.False(vertScrollBar.IsHandleCreated);
        }
    }
}
