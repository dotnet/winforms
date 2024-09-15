// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class TabPageDesignerTests
{
    private (TabPageDesigner designer, TabPage tabPage) InitializeDesignerWithTabPage()
    {
        TabPage tabPage = new();
        TabPageDesigner designer = new();
        designer.Initialize(tabPage);
        return (designer, tabPage);
    }

    [Fact]
    public void CanBeParentedTo_WithNonTabControlParentDesigner_ReturnsFalse()
    {
        Mock<IDesigner> mockDesigner = new();
        var (designer, tabPage) = InitializeDesignerWithTabPage();
        using (designer)
        using (tabPage)
        {
            bool result = designer.CanBeParentedTo(mockDesigner.Object);
            result.Should().BeFalse();
        }
    }

    [Fact]
    public void CanBeParentedTo_WithTabControlParentDesigner_ReturnsTrue()
    {
        Mock<TabControl> mockTabControl = new();
        Mock<IDesigner> mockDesigner = new();
        mockDesigner.Setup(d => d.Component).Returns(mockTabControl.Object);
        var (designer, tabPage) = InitializeDesignerWithTabPage();
        using (designer)
        using (tabPage)
        {
            bool result = designer.CanBeParentedTo(mockDesigner.Object);
            result.Should().BeTrue();
        }
    }

    [Fact]
    public void SelectionRules_WithTabControlParent_ReturnsCorrectSelectionRules()
    {
        using TabControl tabControl = new();
        using TabPage tabPage = new();
        tabControl.TabPages.Add(tabPage);
        using TabPageDesigner designer = new();
        designer.Initialize(tabControl);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = designer.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }
}
