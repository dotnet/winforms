// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ScrollBar_ScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ScrollBarAccessibleObject_ctor_ThrowsException_IfScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ScrollBar.ScrollBarAccessibleObject(null));
        }

        [WinFormsFact]
        public void ScrollBarAccessibleObject_Ctor_Default()
        {
            using var scrollBar = new SubScrollBar();
            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.ScrollBar, accessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected()
        {
            using var scrollBar = new SubScrollBar();
            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.True(accessibleObject.IsPatternSupported(UIA.ValuePatternId));
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.ScrollBarControlTypeId)]
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.IsValuePatternAvailablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "AutomId")]
        public void ScrollBarAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var scrollBar = new SubScrollBar
            {
                AccessibleName = "TestName",
                Name = "AutomId"
            };

            Assert.False(scrollBar.IsHandleCreated);
            var scrollBarAccessibleObject = new ScrollBar.ScrollBarAccessibleObject(scrollBar);
            object value = scrollBarAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(scrollBar.IsHandleCreated);
        }

        private class SubScrollBar : ScrollBar
        {
            public SubScrollBar() : base()
            {
            }
        }
    }
}
