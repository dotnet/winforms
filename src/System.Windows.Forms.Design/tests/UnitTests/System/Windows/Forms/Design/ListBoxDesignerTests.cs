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

        designer.IntegralHeight.Should().Be(true);

        bool value = false;
        designer.IntegralHeight = value;

        designer.IntegralHeight.Should().Be(value);
        listBox.IntegralHeight.Should().Be(value);
    }

    public static IEnumerable<object?[]> SetDockStyle_TestData()
    {
        yield return new object[] { DockStyle.Fill, false };
        yield return new object[] { DockStyle.Left, false };
        yield return new object[] { DockStyle.Right, false };
        yield return new object[] { DockStyle.Bottom };
        yield return new object[] { DockStyle.Top };
    }

    [Theory]
    [MemberData(nameof(SetDockStyle_TestData))]
    public void IntegralHeight_WhenDockStyleIsFillLeftOrRight_ShouldSetShadowPropertyAndNotListBoxIntegralHeight(DockStyle dockStyle, bool expected = true)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        designer.IntegralHeight.Should().Be(true);

        listBox.Dock = dockStyle;
        designer.IntegralHeight = expected;

        designer.IntegralHeight.Should().Be(expected);
        if (expected is false)
        {
            // When DockStyle is Fill/Left/Right, should set ShadowProperty and not set ListBox's IntegralHeight.
            listBox.IntegralHeight.Should().NotBe(expected);
        }
        else
        {
            listBox.IntegralHeight.Should().Be(expected);
        }
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
    [MemberData(nameof(SetDockStyle_TestData))]
    public void Dock_Set_WhenDockStyleIsFillLeftOrRight_ShouldSetListBoxDockAndSetListBoxIntegralHeightToFalse(DockStyle dockStyle, bool expected = true)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);
        designer.Dock = dockStyle;

        listBox.Dock.Should().Be(dockStyle);
        if (expected is false)
        {
            // When DockStyle is Fill/Left/Right, should set ListBox's Dock and set ListBox's IntegralHeight to False.
            listBox.IntegralHeight.Should().BeFalse();
        }
        else
        {
            listBox.IntegralHeight.Should().BeTrue();
        }
    }

    [Theory]
    [MemberData(nameof(SetDockStyle_TestData))]
    public void Dock_Set_WhenDockStyleNotFillLeftOrRight_ShouldSetListBoxDockAndRestoreListBoxIntegralHeight(DockStyle dockStyle, bool expected = true)
    {
        using ListBoxDesigner designer = new();
        using ListBox listBox = new();
        designer.Initialize(listBox);

        listBox.IntegralHeight.Should().BeTrue();

        listBox.IntegralHeight = false;
        designer.IntegralHeight = true;
        designer.Dock = dockStyle;

        listBox.Dock.Should().Be(dockStyle);
        if (expected is false)
        {
            // When DockStyle not Fill/Left/Right, whould set listBox's Dock and restore ListBox's IntegralHeight.
            listBox.IntegralHeight.Should().BeFalse();
        }
        else
        {
            listBox.IntegralHeight.Should().BeTrue();
        }
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
