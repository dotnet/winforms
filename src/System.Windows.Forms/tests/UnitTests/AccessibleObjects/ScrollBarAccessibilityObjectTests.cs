// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ScrollBarAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            var horScrollBar = new HScrollBar();
            HScrollBar.HScrollBarAccessibleObject accessibleObject = Assert.IsType<HScrollBar.HScrollBarAccessibleObject>(horScrollBar.AccessibilityObject);
            Assert.Equal("Horizontal", accessibleObject.Name);
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            var vertScrollBar = new VScrollBar();
            VScrollBar.VScrollBarAccessibleObject accessibleObject = Assert.IsType<VScrollBar.VScrollBarAccessibleObject>(vertScrollBar.AccessibilityObject);
            Assert.Equal("Vertical", accessibleObject.Name);
        }
    }
}
