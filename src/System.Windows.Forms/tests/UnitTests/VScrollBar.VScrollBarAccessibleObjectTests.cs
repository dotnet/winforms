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
    public class VScrollBar_VScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void VScrollBarAccessibleObject_ctor_ThrowsException_IfVScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new VScrollBar.VScrollBarAccessibleObject(null));
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_Ctor_Default()
        {
            using var vertScrollBar = new VScrollBar();
            AccessibleObject accessibleObject = vertScrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.ScrollBar, accessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(vertScrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void VScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var vertScrollBar = new VScrollBar();
            VScrollBar.VScrollBarAccessibleObject accessibleObject
                = Assert.IsType<VScrollBar.VScrollBarAccessibleObject>(vertScrollBar.AccessibilityObject);
            Assert.Equal("Vertical", accessibleObject.Name);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(vertScrollBar.IsHandleCreated);
        }
    }
}
