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
    }

    [Fact]
    public void Constructor_InitializesFields()
    {
        _actionList.Should().NotBeNull();
        _actionList.Should().BeOfType<ContextMenuStripActionList>();

        var toolStripDropDownValue = _actionList.TestAccessor().Dynamic._toolStripDropDown;
        ((ToolStripDropDownMenu)toolStripDropDownValue).Should().Be(_toolStripDropDownMenu);
    }

    [Fact]
    public void AutoShow_GetSet_ReturnsExpected()
    {
        _actionList.AutoShow.Should().BeFalse();

        _actionList.AutoShow = true;
        _actionList.AutoShow.Should().BeTrue();
    }

    [Fact]
    public void ShowImageMargin_GetSet_ReturnsExpected()
    {
        _actionList.ShowImageMargin = true;
        _actionList.ShowImageMargin.Should().BeTrue();

        _actionList.ShowImageMargin = false;
        _actionList.ShowImageMargin.Should().BeFalse();
    }

    [Fact]
    public void ShowCheckMargin_GetSet_ReturnsExpected()
    {
        _actionList.ShowCheckMargin = true;
        _actionList.ShowCheckMargin.Should().BeTrue();

        _actionList.ShowCheckMargin = false;
        _actionList.ShowCheckMargin.Should().BeFalse();
    }

    [Fact]
    public void RenderMode_GetSet_ReturnsExpected()
    {
        _actionList.RenderMode = ToolStripRenderMode.System;
        _actionList.RenderMode.Should().Be(ToolStripRenderMode.System);

        _actionList.RenderMode = ToolStripRenderMode.Professional;
        _actionList.RenderMode.Should().Be(ToolStripRenderMode.Professional);
    }

    [Fact]
    public void GetSortedActionItems_AlwaysReturnsRenderMode()
    {
        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Should().Contain(item => item.MemberName == "RenderMode");
        items.Should().Contain(item => item.DisplayName == SR.ToolStripActionList_RenderMode);
        items.Should().Contain(item => item.Description == SR.ToolStripActionList_RenderModeDesc);
    }

    [Fact]
    public void GetSortedActionItems_WithToolStripDropDownMenu_IncludesShowImageMarginAndShowCheckMargin()
    {
        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Should().Contain(item => item.MemberName == "RenderMode");
        items.Should().Contain(item => item.MemberName == "ShowImageMargin");
        items.Should().Contain(item => item.MemberName == "ShowCheckMargin");

        items.Should().Contain(item => item.DisplayName == SR.ContextMenuStripActionList_ShowImageMargin);
        items.Should().Contain(item => item.DisplayName == SR.ContextMenuStripActionList_ShowCheckMargin);
    }

    [Fact]
    public void GetSortedActionItems_WithToolStripDropDown_OnlyIncludesRenderMode()
    {
        _toolStripDropDown.Site = _toolStripDropDownMenu.Site;
        _designer.Initialize(_toolStripDropDown);
        _actionList.TestAccessor().Dynamic._toolStripDropDown = _toolStripDropDown;

        var items = _actionList.GetSortedActionItems().Cast<DesignerActionPropertyItem>().ToList();

        items.Should().Contain(item => item.MemberName == "RenderMode");
        items.Should().NotContain(item => item.MemberName == "ShowImageMargin");
        items.Should().NotContain(item => item.MemberName == "ShowCheckMargin");
    }
}
