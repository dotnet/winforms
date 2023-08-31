// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDropDownItemDesignerTest
{
    [WinFormsFact]
    public void ToolStripDropDownItemDesignerTest_AssociatedComponentsTest()
    {
        ToolStripDropDownItemDesigner toolStripDropDownItemDesigner = new();
        ToolStripMenuItem toolStripDropDown = new();

        toolStripDropDownItemDesigner.Initialize(toolStripDropDown);

        Assert.Empty(toolStripDropDownItemDesigner.AssociatedComponents);

        toolStripDropDown.DropDownItems.Add("123");

        Assert.Equal(1, toolStripDropDownItemDesigner.AssociatedComponents.Count);
    }
}
