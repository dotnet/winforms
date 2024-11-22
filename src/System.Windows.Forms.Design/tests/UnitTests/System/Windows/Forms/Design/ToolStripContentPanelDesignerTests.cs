// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design.Tests;

public sealed class ToolStripContentPanelDesignerTests : IDisposable
{
    private readonly ToolStripContentPanelDesigner _toolStripContentPanelDesigner;
    private readonly ToolStripContentPanel _toolStripContentPanel;

    public ToolStripContentPanelDesignerTests()
    {
        _toolStripContentPanelDesigner = new();
        _toolStripContentPanel = new();
        _toolStripContentPanelDesigner.Initialize(_toolStripContentPanel);
    }

    public void Dispose()
    {
        _toolStripContentPanelDesigner.Dispose();
        _toolStripContentPanel.Dispose();
    }

    [Fact]
    public void SnapLines_ShouldReturnNonNullList()
    {
        IList<SnapLine> snapLines = _toolStripContentPanelDesigner.SnapLines.Cast<SnapLine>().ToList();

        snapLines.Should().NotBeNull();
    }

    [Fact]
    public void SnapLines_ShouldCallAddPaddingSnapLines()
    {
        string?[] paddingFilters = [SnapLine.PaddingLeft, SnapLine.PaddingRight, SnapLine.PaddingTop, SnapLine.PaddingBottom];

        List<SnapLine> snapLines = _toolStripContentPanelDesigner.SnapLines.Cast<SnapLine>().ToList();

        bool containsPaddingSnapLines = snapLines.Any(snapLine => paddingFilters.Contains(snapLine.Filter));

        containsPaddingSnapLines.Should().BeTrue();
    }

    [Fact]
    public void CanBeParentedTo_ShouldReturnFalse_WhenParentDesignerIsNull()
    {
        bool result = _toolStripContentPanelDesigner.CanBeParentedTo(parentDesigner: null!);

        result.Should().BeFalse();
    }

    [Fact]
    public void CanBeParentedTo_ShouldReturnFalse_WhenParentDesignerIsNotNull()
    {
        using PanelDesigner parentDesigner = new();
        using Panel panel = new();
        parentDesigner.Initialize(panel);

        bool result = _toolStripContentPanelDesigner.CanBeParentedTo(parentDesigner);

        result.Should().BeFalse();
    }
}
