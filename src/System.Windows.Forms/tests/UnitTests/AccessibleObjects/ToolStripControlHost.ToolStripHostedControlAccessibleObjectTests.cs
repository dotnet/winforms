// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ToolStripHostedControlAccessibleObject : IClassFixture<ThreadExceptionFixture>
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

            AccessibleObject toolStripItemAccessibleObject = item.AccessibilityObject;
            AccessibleObject toolStripItemControlAccessibleObject = item.Control.AccessibilityObject;

            Assert.True((bool)toolStripItemAccessibleObject.GetPropertyValue(UIA.IsOffscreenPropertyId)
                || (toolStripItemAccessibleObject.Bounds.Width > 0 && toolStripItemAccessibleObject.Bounds.Height > 0));

            Assert.True((bool)toolStripItemControlAccessibleObject.GetPropertyValue(UIA.IsOffscreenPropertyId)
                || (toolStripItemControlAccessibleObject.Bounds.Width > 0 && toolStripItemControlAccessibleObject.Bounds.Height > 0));
        }
    }
}
