// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

public class ContextMenuStripTests
{
    [WinFormsFact]
    public void ContextMenuStrip_Constructor()
    {
        using ContextMenuStrip cms = new();

        Assert.NotNull(cms);
    }

    [WinFormsFact]
    public void ContextMenuStrip_ConstructorIContainer()
    {
        IContainer nullContainer = null;
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer.Setup(x => x.Add(It.IsAny<ContextMenuStrip>())).Verifiable();

        // act & assert
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new ContextMenuStrip(nullContainer));
        Assert.Equal("container", ex.ParamName);

        using ContextMenuStrip cms = new(mockContainer.Object);
        Assert.NotNull(cms);
        mockContainer.Verify(x => x.Add(cms));
    }
}
