// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public sealed class ToolStripActionListTests : IDisposable
{
    private readonly Mock<IDesignerHost> _mockDesignerHost;
    private readonly Mock<ISelectionService> _mockSelectionService;
    private readonly Mock<IComponentChangeService> _mockComponentChangeService;
    private readonly ToolStrip _toolStrip;
    private readonly ToolStripDesigner _designer;
    private readonly ToolStripActionList _actionList;

    public ToolStripActionListTests()
    {
        _mockDesignerHost = new();
        _mockSelectionService = new();
        _mockComponentChangeService = new();
        _toolStrip = new();
        InitializeMocks();

        _designer = new();
        _designer.Initialize(_toolStrip);
        _actionList = new(_designer);

        void InitializeMocks()
        {
            Mock<ISite> mockSite = new();
            mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
            mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
            mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);

            _toolStrip.Site = mockSite.Object;

            _mockDesignerHost.Setup(d => d.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);
        }
    }

    public void Dispose()
    {
        _toolStrip.Dispose();
        _designer.Dispose();
    }

    [Fact]
    public void Constructor_InitializesFields()
    {
        _actionList.Should().NotBeNull();
        _actionList.Should().BeOfType<ToolStripActionList>();
        _actionList.Component.Should().Be(_toolStrip);
    }

    [Theory]
    [BoolData]
    public void AutoShow_GetSet_ReturnsExpected(bool value)
    {
        _actionList.AutoShow.Should().BeFalse();

        _actionList.AutoShow = value;
        _actionList.AutoShow.Should().Be(value);
    }

    [Theory]
    [EnumData<DockStyle>]
    public void Dock_GetSet_ReturnsExpected(DockStyle dockStyle)
    {
        _actionList.Dock.Should().Be(DockStyle.Top);

        _actionList.Dock = dockStyle;
        _actionList.Dock.Should().Be(dockStyle);
    }

    [Theory]
    [EnumData<ToolStripRenderMode>]
    public void RenderMode_GetSet_ReturnsExpected(ToolStripRenderMode renderMode)
    {
        if (renderMode == ToolStripRenderMode.Custom)
        {
            _actionList.Invoking(a => a.RenderMode = renderMode)
             .Should().Throw<NotSupportedException>();
        }
        else
        {
            _actionList.RenderMode = renderMode;
            _actionList.RenderMode.Should().Be(renderMode);
        }
    }

    [Theory]
    [EnumData<ToolStripGripStyle>]
    public void GripStyle_GetSet_ReturnsExpected(ToolStripGripStyle gripStyle)
    {
        _actionList.GripStyle.Should().Be(ToolStripGripStyle.Visible);

        _actionList.GripStyle = gripStyle;
        _actionList.GripStyle.Should().Be(gripStyle);
    }

    [Fact]
    public void GetSortedActionItems_ShouldIncludeExpectedItems_WhenConditionsAreMet()
    {
        var (methodItems, propertyItems) = GetSortedActionItems();

        methodItems.Should().ContainSingle(action => action.MemberName == "InvokeEmbedVerb");
        methodItems.Should().ContainSingle(action => action.MemberName == "InvokeInsertStandardItemsVerb");
        propertyItems.Should().ContainSingle(property => property.MemberName == "RenderMode");
        propertyItems.Should().ContainSingle(property => property.MemberName == "Dock");
        propertyItems.Should().ContainSingle(property => property.MemberName == "GripStyle");
    }

    [Fact]
    public void GetSortedActionItems_ShouldNotIncludeEmbedVerb_WhenIsReadOnly()
    {
        TypeDescriptor.AddAttributes(_toolStrip, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        TypeDescriptor.Refresh(_toolStrip);

        var (methodItems, _) = GetSortedActionItems();

        methodItems.Should().NotContain(action => action.MemberName == "InvokeEmbedVerb");
    }

    [Fact]
    public void GetSortedActionItems_ShouldIncludeRenderModeAndInsertStandardItemsVerb_WhenCanAddItems()
    {
        var (methodItems, propertyItems) = GetSortedActionItems();

        methodItems.Should().ContainSingle(action => action.MemberName == "InvokeInsertStandardItemsVerb");
        propertyItems.Should().ContainSingle(property => property.MemberName == "RenderMode");
    }

    [Fact]
    public void GetSortedActionItems_ShouldIncludeDock_WhenParentIsNotToolStripPanel()
    {
        _toolStrip.Parent = new Form();

        var (_, propertyItems) = GetSortedActionItems();

        propertyItems.Should().ContainSingle(property => property.MemberName == "Dock");
    }

    [Fact]
    public void GetSortedActionItems_ShouldNotIncludeGripStyle_WhenToolStripIsStatusStrip()
    {
        var statusStrip = new StatusStrip
        {
            Site = _toolStrip.Site
        };
        _designer.Initialize(statusStrip);
        _actionList.TestAccessor().Dynamic._toolStrip = statusStrip;

        var (_, propertyItems) = GetSortedActionItems();

        propertyItems.Should().NotContain(property => property.MemberName == "GripStyle");
    }

    private (List<DesignerActionMethodItem> methodItems, List<DesignerActionPropertyItem> propertyItems) GetSortedActionItems()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();
        List<DesignerActionItem> itemList = items.Cast<DesignerActionItem>().ToList();
        List<DesignerActionMethodItem> methodItems = itemList.OfType<DesignerActionMethodItem>().ToList();
        List<DesignerActionPropertyItem> propertyItems = itemList.OfType<DesignerActionPropertyItem>().ToList();
        return (methodItems, propertyItems);
    }
}
