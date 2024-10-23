// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class SplitterDesignerTests
{
    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue()
    {
        using SplitterDesigner designer = new();

        designer.AutoResizeHandles.Should().BeTrue();
    }
}
