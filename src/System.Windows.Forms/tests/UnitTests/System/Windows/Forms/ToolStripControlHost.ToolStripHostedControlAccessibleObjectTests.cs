// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ToolStripControlHost_ToolStripHostedControlAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> ToolStripItemAccessibleObject_TestData()
        {
            return ReflectionHelper.GetPublicNotAbstractClasses<ToolStripControlHost>().Select(type => new object[] { type });
        }

        [WinFormsTheory]
        [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
        public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_ReturnExpected(Type type)
        {
            using var toolStrip = new ToolStrip();
            toolStrip.CreateControl();
            using ToolStripControlHost item = ReflectionHelper.InvokePublicConstructor<ToolStripControlHost>(type);
            item.Size = new Size(0, 0);
            toolStrip.Items.Add(item);

            Assert.True(GetIsOffscreenPropertyValue(item.AccessibilityObject));
            Assert.True(GetIsOffscreenPropertyValue(item.Control.AccessibilityObject));
        }

        private bool GetIsOffscreenPropertyValue(AccessibleObject accessibleObject) =>
            (bool)accessibleObject.GetPropertyValue(UIA.IsOffscreenPropertyId) || (accessibleObject.Bounds.Width > 0 && accessibleObject.Bounds.Height > 0);
    }
}
