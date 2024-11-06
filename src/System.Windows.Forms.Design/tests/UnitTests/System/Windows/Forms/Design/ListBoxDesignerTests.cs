// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class ListBoxDesignerTests
{
    [Fact]
    public void IntegralHeight_WhenDockStyleNotFillLeftOrRight_ShouldSetShadowPropertyAndListBoxIntegralHeight()
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        designer.IntegralHeight.Should().BeTrue();

        bool value = false;
        designer.IntegralHeight = value;

        designer.IntegralHeight.Should().Be(value);
        listBox.IntegralHeight.Should().Be(value);
    }

    public static TheoryData<object[]> SetDockStyle_TheoryData() => new()
    {
        new object[] { DockStyle.Fill, false },
        new object[] { DockStyle.Left, false },
        new object[] { DockStyle.Right, false },
        new object[] { DockStyle.Bottom, true },
        new object[] { DockStyle.Top, true }
    };

    [Theory]
    [MemberData(nameof(SetDockStyle_TheoryData))]
    public void IntegralHeight_WhenDockStyleIsFillLeftOrRight_ShouldSetShadowPropertyAndNotListBoxIntegralHeight(object[] value)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        designer.IntegralHeight.Should().BeTrue();

        DockStyle dockStyle = (DockStyle)value[0];
        bool integralHeight = (bool)value[1];
        listBox.Dock = dockStyle;
        designer.IntegralHeight = integralHeight;

        // When DockStyle is Fill/Left/Right, should set ShadowProperty and not set ListBox's IntegralHeight.
        designer.IntegralHeight.Should().Be(integralHeight);
        listBox.IntegralHeight.Should().BeTrue();
        listBox.IsHandleCreated.Should().BeFalse();
    }

    [Fact]
    public void Dock_Get_ShouldReturnListBoxDock()
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        designer.Dock.Should().Be(listBox.Dock);
    }

    [Theory]
    [MemberData(nameof(SetDockStyle_TheoryData))]
    public void Dock_Set_WhenDockStyleIsFillLeftOrRight_ShouldSetListBoxDockAndSetListBoxIntegralHeightToFalse(object[] value)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        DockStyle dockStyle = (DockStyle)value[0];
        bool expected = (bool)value[1];
        designer.Dock = dockStyle;

        // When DockStyle is Fill/Left/Right, should set ListBox's Dock and set ListBox's IntegralHeight to False.
        listBox.Dock.Should().Be(dockStyle);
        listBox.IntegralHeight.Should().Be(expected);
        listBox.IsHandleCreated.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(SetDockStyle_TheoryData))]
    public void Dock_Set_WhenDockStyleNotFillLeftOrRight_ShouldSetListBoxDockAndRestoreListBoxIntegralHeight(object[] value)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        listBox.IntegralHeight.Should().BeTrue();

        listBox.IntegralHeight = false;
        designer.IntegralHeight = true;

        DockStyle dockStyle = (DockStyle)value[0];
        bool expected = (bool)value[1];
        designer.Dock = dockStyle;

        // When DockStyle not Fill/Left/Right, should set listBox's Dock and restore ListBox's IntegralHeight.
        listBox.Dock.Should().Be(dockStyle);
        listBox.IntegralHeight.Should().Be(expected);
        listBox.IsHandleCreated.Should().BeFalse();
    }

    [Fact]
    public void Initialize_ShouldModifyProperties()
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        listBox.Should().NotBeNull();
        designer.AutoResizeHandles.Should().BeTrue();
        listBox.IntegralHeight.Should().Be(designer.IntegralHeight);
    }

    [Fact]
    public void ActionLists_WithCheckedListBoxListBox_ShouldSetValue()
    {
        using ListBoxDesigner designer = new();
        using CheckedListBox listBox = new();
        designer.Initialize(listBox);

        var actionList = designer.ActionLists;
        actionList.Should().NotBeNull();
        actionList[0]!.GetType().Name.Should().Be(nameof(ListControlUnboundActionList));
    }

    [Fact]
    public void InitializeNewComponent_WithCheckedListBox_ShouldSetFormattingEnabledToTrue()
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new CheckedListBox();
        designer.Initialize(listBox);
        listBox.FormattingEnabled.Should().BeFalse();

        designer.InitializeNewComponent(new Dictionary<int, int>());

        listBox.FormattingEnabled.Should().BeTrue();
    }
}
