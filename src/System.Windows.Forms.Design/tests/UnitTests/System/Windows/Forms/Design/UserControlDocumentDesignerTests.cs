// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class UserControlDocumentDesignerTests
{
    [Fact]
    public void Constructor_ShouldSetAutoResizeHandlesToTrue()
    {
        UserControlDocumentDesigner userControl = new();

        userControl.AutoResizeHandles.Should().BeTrue();
    }
}
