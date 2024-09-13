// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class PanelDesignerTests
{
    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue()
    {
        using Panel panel = new();
        using PanelDesigner designer = new();
        designer.Initialize(panel);

        designer.AutoResizeHandles.Should().BeTrue();
    }
}
