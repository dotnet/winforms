// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ToolStripAccessibleObjectTests
    {
        [Fact]
        public void ToolStripAccessibleObject_Ctor_Default()
        {
            ToolStrip toolStrip = new ToolStrip();

            var accessibleObject = new ToolStrip.ToolStripAccessibleObject(toolStrip);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.ToolBar, accessibleObject.Role);
        }

        public static IEnumerable<object[]> ToolStripAccessibleObject_TestData()
        {
            ToolStrip toolStrip = new ToolStrip();
            yield return new object[] { toolStrip.AccessibilityObject };
        }

        [Theory]
        [MemberData(nameof(ToolStripAccessibleObject_TestData))]
        public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ThumbButton(AccessibleObject accessibleObject)
        {
            UnsafeNativeMethods.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UnsafeNativeMethods.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(NativeConstants.UIA_ThumbControlTypeId, firstChild.GetPropertyValue(NativeConstants.UIA_ControlTypePropertyId));
        }
    }
}
