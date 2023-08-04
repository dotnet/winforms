// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public class ToolStripMenuItemDesignerTest
{
    [WinFormsFact]
    public void ToolStripMenuItemDesignerTest_AssociatedComponentsTest()
    {
        ToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripDropDown = new();
        toolStripMenuItemDesigner.Initialize(toolStripDropDown);

        Assert.Empty(toolStripMenuItemDesigner.AssociatedComponents);

        toolStripDropDown.DropDownItems.Add("123");

        Assert.Equal(1, toolStripMenuItemDesigner.AssociatedComponents.Count);
    }
}
