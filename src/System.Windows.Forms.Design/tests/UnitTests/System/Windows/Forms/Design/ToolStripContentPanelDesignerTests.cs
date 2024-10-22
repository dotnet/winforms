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
        _toolStripContentPanelDesigner = new ToolStripContentPanelDesigner();
        _toolStripContentPanel = new ToolStripContentPanel();
        _toolStripContentPanelDesigner.Initialize(_toolStripContentPanel);
    }

    [Fact]
    public void SnapLines_ShouldReturnNonNullList()
    {
        var snapLines = _toolStripContentPanelDesigner.SnapLines;

        snapLines.Should().NotBeNull();
    }

    [Fact]
    public void SnapLines_ShouldCallAddPaddingSnapLines()
    {
        var paddingFilters = new[] { SnapLine.PaddingLeft, SnapLine.PaddingRight, SnapLine.PaddingTop, SnapLine.PaddingBottom };

        var snapLines = _toolStripContentPanelDesigner.SnapLines.Cast<SnapLine>().ToList();

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
        PanelDesigner parentDesigner = new();

        bool result = _toolStripContentPanelDesigner.CanBeParentedTo(parentDesigner);

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _toolStripContentPanelDesigner.Dispose();
        _toolStripContentPanel.Dispose();
    }
}
