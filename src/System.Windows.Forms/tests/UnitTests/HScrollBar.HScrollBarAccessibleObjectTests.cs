// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class HScrollBar_HScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HScrollBarAccessibleObject_ctor_ThrowsException_IfHScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HScrollBar.HScrollBarAccessibleObject(null));
        }

        [WinFormsFact]
        public void HScrollBarAccessibleObject_Ctor_Default()
        {
            using var horScrollBar = new HScrollBar();
            AccessibleObject accessibleObject = horScrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.ScrollBar, accessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(horScrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void HScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var horScrollBar = new HScrollBar();
            HScrollBar.HScrollBarAccessibleObject accessibleObject =
                Assert.IsType<HScrollBar.HScrollBarAccessibleObject>(horScrollBar.AccessibilityObject);
            Assert.Equal("Horizontal", accessibleObject.Name);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(horScrollBar.IsHandleCreated);
        }
    }
}
