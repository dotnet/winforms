// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class SplitContainerDesignerTests
{
    [WinFormsFact]
    public void SplitContainerDesigner_AssociatedComponentsTest()
    {
        DesignSurfaceExt.DesignSurfaceExt surface = new();
        surface.CreateRootComponent<Form>(Size.Empty);
        using SplitContainer splitContainer = surface.CreateControl<SplitContainer>(Size.Empty, Point.Empty);
        using SplitContainerDesigner splitContainerDesigner = (SplitContainerDesigner)surface.CreateDesigner(splitContainer, false);
        splitContainerDesigner.Initialize(splitContainer);

        Assert.Empty(splitContainerDesigner.AssociatedComponents);

        using Control control = new();
        control.Parent = splitContainer.Panel1;

        Assert.Equal(1, splitContainerDesigner.AssociatedComponents.Count);
    }
}
