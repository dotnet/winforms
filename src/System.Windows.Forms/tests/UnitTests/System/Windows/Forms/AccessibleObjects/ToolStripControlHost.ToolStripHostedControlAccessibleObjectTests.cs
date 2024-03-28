// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripControlHost_ToolStripHostedControlAccessibleObjectTests
{
    public static IEnumerable<object[]> ToolStripItemAccessibleObject_TestData()
    {
        return ReflectionHelper.GetPublicNotAbstractClasses<ToolStripControlHost>().Select(type => new object[] { type });
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripItemAccessibleObject_TestData))]
    public void ToolStripHostedControlAccessibleObject_GetPropertyValue_IsOffscreenPropertyId_ReturnExpected(Type type)
    {
        using ToolStrip toolStrip = new();
        toolStrip.CreateControl();
        using ToolStripControlHost item = ReflectionHelper.InvokePublicConstructor<ToolStripControlHost>(type);
        item.Size = new Size(0, 0);
        toolStrip.Items.Add(item);

        Assert.True(GetIsOffscreenPropertyValue(item.AccessibilityObject));
        Assert.True(GetIsOffscreenPropertyValue(item.Control.AccessibilityObject));
    }

    private bool GetIsOffscreenPropertyValue(AccessibleObject accessibleObject) =>
        (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId) || (accessibleObject.Bounds.Width > 0 && accessibleObject.Bounds.Height > 0);
}
