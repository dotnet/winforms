// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ChangeToolStripParentVerbTests : IDisposable
{
    private readonly ToolStripDesigner _designer = new();
    private readonly ToolStrip _toolStrip;

    public ChangeToolStripParentVerbTests()
    {
        _toolStrip = MockToolStrip();
        _designer.Initialize(_toolStrip);
    }

    public void Dispose()
    {
        _toolStrip?.Dispose();
        _designer.Dispose();
    }

    private ToolStrip MockToolStrip()
    {
        Mock<ISite> mockSite = new();
        Mock<ISelectionService> mockSelectionService = new();
        mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);

        Mock<IDesignerHost> mockDesignerHost = new();
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);

        Mock<IComponentChangeService> mockComponentChangeService = new();
        mockDesignerHost
            .Setup(dh => dh.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        ToolStrip toolStrip = new() { Site = mockSite.Object };

        return toolStrip;
    }

    [Fact]
    public void Constructor_WithNullDesigner_Throws()
    {
        Action action = () => new ChangeToolStripParentVerb(null);

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void ChangeParent_DeepestPossibleTest()
    {
        var changeToolStripParentVerb = new ChangeToolStripParentVerb(_designer);

        changeToolStripParentVerb.ChangeParent();

        throw new NotImplementedException();
    }

    [Fact]
    public void ChangeParent_WithNullRootDesigner_DoesNotChangesParent()
    {
        var changeToolStripParentVerb = new ChangeToolStripParentVerb(_designer);

        changeToolStripParentVerb.ChangeParent();

        throw new NotImplementedException();
    }
}
