// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class ComponentDocumentDesignerTests
{
    [Fact]
    public void Designer_ThrowsNotImplementedException()
    {
        ComponentDocumentDesigner designer = new();
        using Timer timer = new();
        Action action = () => designer.Initialize(timer);
        action.Should().Throw<NotImplementedException>();
    }
}
