// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ContextMenuStripActionListTests : IDisposable
{
    private readonly Mock<IDesignerHost> _mockDesignerHost;
    private readonly Mock<ISelectionService> _mockSelectionService;
    private readonly Mock<IComponentChangeService> _mockComponentChangeService;
    private readonly ToolStripDropDownMenu _toolStripDropDownMenu;
    private readonly ToolStripDropDown _toolStripDropDown;
    private readonly ToolStripDropDownDesigner _designer;
    private readonly ContextMenuStripActionList _actionList;

    public ContextMenuStripActionListTests()
    {
        _mockDesignerHost = new();
        _mockSelectionService = new();
        _mockComponentChangeService = new();
        _toolStripDropDownMenu = new();
        _toolStripDropDown = new();
        InitializeMocks();

        _designer = new();
        _designer.Initialize(_toolStripDropDownMenu);
        _actionList = new(_designer);

        void InitializeMocks()
        {
            Mock<ISite> mockSite = new();
            mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
            mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
            mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);
            _toolStripDropDownMenu.Site = mockSite.Object;

            _mockDesignerHost.Setup(d => d.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);
        }
    }

    public void Dispose()
    {
        _toolStripDropDownMenu.Dispose();
        _toolStripDropDown.Dispose();
        _designer.Dispose();
    }

    [Fact]
    public void Constructor_InitializesFields()
    {
        _actionList.Should().NotBeNull();
        _actionList.Should().BeOfType<ContextMenuStripActionList>();

        ToolStripDropDown toolStripDropDownValue = (ToolStripDropDown)_actionList.TestAccessor().Dynamic._toolStripDropDown;
        ((ToolStripDropDownMenu)toolStripDropDownValue).Should().Be(_toolStripDropDownMenu);
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
    [BoolData]
    public void ShowImageMargin_GetSet_ReturnsExpected(bool value)
    {
        _actionList.ShowImageMargin.Should().BeTrue();

        _actionList.ShowImageMargin = value;
        _actionList.ShowImageMargin.Should().Be(value);
    }

    [Theory]
    [BoolData]
    public void ShowCheckMargin_GetSet_ReturnsExpected(bool value)
    {
        _actionList.ShowCheckMargin.Should().BeFalse();

        _actionList.ShowCheckMargin = value;
        _actionList.ShowCheckMargin.Should().Be(value);
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

    [Fact]
    public void GetSortedActionItems_AlwaysReturnsRenderMode()
    {
        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Select(item => item.MemberName)
           .Should().Contain(nameof(ContextMenuStripActionList.RenderMode));

        items.Select(item => item.DisplayName)
            .Should().Contain(SR.ToolStripActionList_RenderMode);

        items.Select(item => item.Description)
            .Should().Contain(SR.ToolStripActionList_RenderModeDesc);
    }

    [Fact]
    public void GetSortedActionItems_WithToolStripDropDownMenu_IncludesShowImageMarginAndShowCheckMargin()
    {
        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Select(item => item.MemberName)
            .Should().Contain(nameof(ContextMenuStripActionList.RenderMode))
            .And.Contain(nameof(ContextMenuStripActionList.ShowImageMargin))
            .And.Contain(nameof(ContextMenuStripActionList.ShowCheckMargin));

        items.Select(item => item.DisplayName)
            .Should().Contain(SR.ContextMenuStripActionList_ShowImageMargin)
            .And.Contain(SR.ContextMenuStripActionList_ShowCheckMargin);
    }

    [Fact]
    public void GetSortedActionItems_WithToolStripDropDown_OnlyIncludesRenderMode()
    {
        _toolStripDropDown.Site = _toolStripDropDownMenu.Site;
        _designer.Initialize(_toolStripDropDown);
        _actionList.TestAccessor().Dynamic._toolStripDropDown = _toolStripDropDown;

        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Select(item => item.MemberName)
            .Should().Contain(nameof(ContextMenuStripActionList.RenderMode))
            .And.NotContain(nameof(ContextMenuStripActionList.ShowImageMargin))
            .And.NotContain(nameof(ContextMenuStripActionList.ShowCheckMargin));
    }
}
