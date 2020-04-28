// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ToolStripAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripAccessibleObject_Ctor_Default()
        {
            using ToolStrip toolStrip = new ToolStrip();

            var accessibleObject = new ToolStrip.ToolStripAccessibleObject(toolStrip);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.ToolBar, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ThumbButton()
        {
            using ToolStrip toolStrip = new ToolStrip();
            var accessibleObject = toolStrip.AccessibilityObject;

            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(UiaCore.UIA.ThumbControlTypeId, firstChild.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }
    }
}
