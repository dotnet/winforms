// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class ListBoxDesignerTests
{
    [Fact]
    public void IntegralHeight_WhenDockStyleNotFillLeftOrRight_ShouldSetShadowPropertyAndListBoxIntegralHeight()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        bool value = false;
        designer.IntegralHeight = value;

        designer.IntegralHeight.Should().Be(value);
        listBox.IntegralHeight.Should().Be(value);
    }

    [Fact]
    public void IntegralHeight_WhenDockStyleIsFillLeftOrRight_ShouldSetShadowPropertyAndNotListBoxIntegralHeight()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        bool value = false;
        listBox.Dock = DockStyle.Fill;
        designer.IntegralHeight = value;

        designer.IntegralHeight.Should().Be(value);
        listBox.IntegralHeight.Should().NotBe(value);
    }

    [Fact]
    public void Dock_Get_ShouldReturnListBoxDock()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        var expected = listBox.Dock;
        var result = designer.Dock;

        result.Should().Be(expected);
    }

    [Fact]
    public void Dock_Set_WhenDockStyleIsFillLeftOrRight_ShouldSetListBoxDockAndSetListBoxIntegralHeightToFalse()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        var value = DockStyle.Fill;
        designer.Dock = value;

        listBox.Dock.Should().Be(value);
        listBox.IntegralHeight.Should().BeFalse();
    }

    [Fact]
    public void Dock_Set_WhenDockStyleNotFillLeftOrRight_ShouldSetListBoxDockAndRestoreListBoxIntegralHeight()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        listBox.IntegralHeight = false;
        designer.IntegralHeight = true;
        var value = DockStyle.Top;

        designer.Dock = value;

        listBox.Dock.Should().Be(value);
        listBox.IntegralHeight.Should().BeTrue();
    }

    [Fact]
    public void Initialize_ShouldModifyProperties()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new();
        designer.Initialize(listBox);

        listBox.Should().NotBeNull();
        designer.AutoResizeHandles.Should().BeTrue();
        listBox.IntegralHeight.Should().Be(designer.IntegralHeight);
    }

    [Fact]
    public void ActionLists_WithCheckedListBoxListBox_ShouldSetValue()
    {
        ListBoxDesigner designer = new();
        CheckedListBox listBox = new();
        designer.Initialize(listBox);

        var actionList = designer.ActionLists;
        actionList.Should().NotBeNull();
        actionList[0]!.GetType().Name.Should().Be(nameof(ListControlUnboundActionList));
    }

    [Fact]
    public void InitializeNewComponent_WithCheckedListBox_ShouldSetFormattingEnabledToTrue()
    {
        ListBoxDesigner designer = new();
        ListBox listBox = new CheckedListBox();
        designer.Initialize(listBox);
        designer.InitializeNewComponent(new Dictionary<int, int>());

        listBox.FormattingEnabled.Should().Be(true);
    }
}
