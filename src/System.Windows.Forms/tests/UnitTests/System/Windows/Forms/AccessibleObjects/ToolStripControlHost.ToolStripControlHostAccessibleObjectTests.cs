// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripControlHost;

namespace System.Windows.Forms.Tests.AccessibleObjects;

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

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(toolStrip.Control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripControlHostAccessibleObject_ReleaseUiaProvider_ToolStripControlHostControl()
    {
        using ToolStrip toolStrip = new();
        using Control control = new();
        using ToolStripControlHost toolStripControlHost = new(control);
        toolStrip.Items.Add(toolStripControlHost);
        toolStripControlHost.Parent = toolStrip;
        toolStrip.CreateControl();

        _ = toolStripControlHost.AccessibilityObject;
        _ = toolStripControlHost.Control.AccessibilityObject;

        Assert.True(toolStripControlHost.Control.IsAccessibilityObjectCreated);

        toolStripControlHost.ReleaseUiaProvider();

        Assert.False(toolStripControlHost.Control.IsAccessibilityObjectCreated);
    }
}
