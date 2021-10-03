// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ToolStripControlHost;

namespace System.Windows.Forms.Tests
{
    public class ToolStripControlHost_ToolStripControlHostAccessibleObjectTests
    {
        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_Ctor_OwnerToolStripControlHostCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ToolStripControlHostAccessibleObject(null));
        }

        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_Ctor_Default()
        {
            using Control control = new();
            using ToolStripControlHost toolStrip = new(control);
            var accessibleObject = (ToolStripControlHostAccessibleObject)toolStrip.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(toolStrip, accessibleObject.Owner);
            Assert.False(toolStrip.Control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_DefaultAction_ReturnsExpected()
        {
            using Control control = new();
            using ToolStripControlHost toolStrip = new(control);
            var accessibleObject = (ToolStripControlHostAccessibleObject)toolStrip.AccessibilityObject;

            Assert.Equal(string.Empty, accessibleObject.DefaultAction);
            Assert.False(toolStrip.Control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_Role_ReturnsExpected()
        {
            AccessibleRole testRole = AccessibleRole.Cell;
            using Control control = new();
            using ToolStripControlHost toolStrip = new(control);
            var accessibleObject = (ToolStripControlHostAccessibleObject)toolStrip.AccessibilityObject;

            control.AccessibleRole = testRole;

            Assert.Equal(testRole, accessibleObject.Role);
            Assert.False(toolStrip.Control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHostAccessibleObject_FragmentNavigate_ReturnsExpected()
        {
            using Control control = new();
            using ToolStripControlHost toolStrip = new(control);
            var accessibleObject = (ToolStripControlHostAccessibleObject)toolStrip.AccessibilityObject;

            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
            Assert.False(toolStrip.Control.IsHandleCreated);
        }
    }
}
