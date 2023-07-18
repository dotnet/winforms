// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerTests
{
    [WinFormsFact]
    public void ToolStripDesigner_AssociatedComponentsTest()
    {
        DesignSurfaceExt.DesignSurfaceExt surface = new();
        surface.CreateRootComponent<Form>(Size.Empty);
        using ToolStrip toolStrip = surface.CreateControl<ToolStrip>(Size.Empty, Point.Empty);
        using ToolStripDesigner toolStripDesigner = new();

        toolStripDesigner.Initialize(toolStrip);

        Assert.Empty(toolStripDesigner.AssociatedComponents);

        toolStrip.Items.Add("123");
        toolStrip.Items.Add("abc");

        Assert.Equal(2, toolStripDesigner.AssociatedComponents.Count);
    }
}
