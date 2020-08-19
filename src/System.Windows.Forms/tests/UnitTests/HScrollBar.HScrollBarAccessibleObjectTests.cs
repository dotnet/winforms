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

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ScrollBar)]
        [InlineData(false, AccessibleRole.None)]
        public void HScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
        {
            using var horScrollBar = new HScrollBar();

            if (createControl)
            {
                horScrollBar.CreateControl();
            }

            AccessibleObject accessibleObject = horScrollBar.AccessibilityObject;
            Assert.NotNull(accessibleObject);
            Assert.Equal(accessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, horScrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void HScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var horScrollBar = new HScrollBar();
            HScrollBar.HScrollBarAccessibleObject accessibleObject =
                Assert.IsType<HScrollBar.HScrollBarAccessibleObject>(horScrollBar.AccessibilityObject);
            Assert.Equal("Horizontal", accessibleObject.Name);
            Assert.False(horScrollBar.IsHandleCreated);
        }
    }
}
