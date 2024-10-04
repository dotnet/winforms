﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripComboBoxTests : IDisposable
{
    private readonly ToolStripComboBox _toolStripComboBox;

    public ToolStripComboBoxTests()
    {
        _toolStripComboBox = new();
    }

    public void Dispose()
    {
        _toolStripComboBox.Dispose();
    }

    [WinFormsFact]
    public void ToolStripComboBox_ConstructorWithName_SetsName()
    {
        string expectedName = "TestComboBox";
        _toolStripComboBox.Name = expectedName;
        _toolStripComboBox.Name.Should().Be(expectedName);
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownHeight_SetAndGet()
    {
        _toolStripComboBox.DropDownHeight.Should().Be(106);

        _toolStripComboBox.DropDownHeight = 200;
        _toolStripComboBox.DropDownHeight.Should().Be(200);
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownWidth_SetAndGet()
    {
        _toolStripComboBox.DropDownWidth = 200;
        _toolStripComboBox.DropDownWidth.Should().Be(200);
    }

    [WinFormsFact]
    public void ToolStripComboBox_DroppedDown_SetAndGet()
    {
        _toolStripComboBox.DroppedDown.Should().BeFalse();

        _toolStripComboBox.DroppedDown = true;

        _toolStripComboBox.DroppedDown = false;
        _toolStripComboBox.DroppedDown.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripComboBox_MaxLength_SetAndGet()
    {
        _toolStripComboBox.MaxLength = 10;
        _toolStripComboBox.MaxLength.Should().Be(10);
    }

    [WinFormsFact]
    public void ToolStripComboBox_IntegralHeight_SetAndGet()
    {
        _toolStripComboBox.IntegralHeight.Should().BeTrue();

        _toolStripComboBox.IntegralHeight = false;
        _toolStripComboBox.IntegralHeight.Should().BeFalse();

        _toolStripComboBox.IntegralHeight = true;
        _toolStripComboBox.IntegralHeight.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_MaxDropDownItems_SetAndGet()
    {
        _toolStripComboBox.MaxDropDownItems = 20;
        _toolStripComboBox.MaxDropDownItems.Should().Be(20);
    }

    [WinFormsFact]
    public void ToolStripComboBox_Sorted_SetAndGet()
    {
        _toolStripComboBox.Sorted.Should().BeFalse();

        _toolStripComboBox.Sorted = true;
        _toolStripComboBox.Sorted.Should().BeTrue();

        _toolStripComboBox.Sorted = false;
        _toolStripComboBox.Sorted.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripComboBox_ConstructorWithControl_ThrowsNotSupportedException()
    {
        using Control control = new();
        new Action(() => new ToolStripComboBox(control)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void ToolStripComboBox_AutoCompleteCustomSource_SetAndGet()
    {
        AutoCompleteStringCollection source = new(){ "Item1", "Item2" };
        _toolStripComboBox.AutoCompleteCustomSource = source;
        _toolStripComboBox.AutoCompleteCustomSource.Should().BeEquivalentTo(source);
    }

    [WinFormsTheory]
    [InlineData(AutoCompleteMode.Suggest)]
    [InlineData(AutoCompleteMode.Append)]
    [InlineData(AutoCompleteMode.SuggestAppend)]
    [InlineData(AutoCompleteMode.None)]
    public void ToolStripComboBox_AutoCompleteMode_SetAndGet(AutoCompleteMode mode)
    {
        _toolStripComboBox.AutoCompleteMode.Should().Be(AutoCompleteMode.None);

        _toolStripComboBox.AutoCompleteMode = mode;
        _toolStripComboBox.AutoCompleteMode.Should().Be(mode);
    }

    [WinFormsTheory]
    [InlineData(AutoCompleteSource.None)]
    [InlineData(AutoCompleteSource.AllSystemSources)]
    [InlineData(AutoCompleteSource.AllUrl)]
    [InlineData(AutoCompleteSource.CustomSource)]
    [InlineData(AutoCompleteSource.FileSystem)]
    [InlineData(AutoCompleteSource.FileSystemDirectories)]
    [InlineData(AutoCompleteSource.HistoryList)]
    [InlineData(AutoCompleteSource.RecentlyUsedList)]
    [InlineData(AutoCompleteSource.ListItems)]
    public void ToolStripComboBox_AutoCompleteSource_SetAndGet(AutoCompleteSource source)
    {
        _toolStripComboBox.AutoCompleteSource = source;
        _toolStripComboBox.AutoCompleteSource.Should().Be(source);
    }

    [WinFormsFact]
    public void ToolStripComboBox_BackgroundImage_SetAndGet()
    {
        using Bitmap image = new(10, 10);
        _toolStripComboBox.BackgroundImage = image;
        _toolStripComboBox.BackgroundImage.Should().Be(image);
    }

    [WinFormsTheory]
    [InlineData(ImageLayout.None)]
    [InlineData(ImageLayout.Tile)]
    [InlineData(ImageLayout.Center)]
    [InlineData(ImageLayout.Stretch)]
    [InlineData(ImageLayout.Zoom)]
    public void ToolStripComboBox_BackgroundImageLayout_SetAndGet(ImageLayout layout)
    {
        _toolStripComboBox.BackgroundImageLayout = layout;
        _toolStripComboBox.BackgroundImageLayout.Should().Be(layout);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.Simple)]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    public void ToolStripComboBox_DropDownStyle_SetAndGet(ComboBoxStyle style)
    {
        _toolStripComboBox.DropDownStyle = style;
        _toolStripComboBox.DropDownStyle.Should().Be(style);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.System)]
    public void ToolStripComboBox_FlatStyle_SetAndGet(FlatStyle style)
    {
        _toolStripComboBox.FlatStyle = style;
        _toolStripComboBox.FlatStyle.Should().Be(style);
    }

    [WinFormsFact]
    public void ToolStripComboBox_Items_AddAndGet()
    {
        string[] items = ["Item1", "Item2"];
        _toolStripComboBox.Items.AddRange(items);
        _toolStripComboBox.Items.Cast<string>().Should().Contain(items);
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectedIndex_SetAndGet()
    {
        _toolStripComboBox.SelectedIndex.Should().Be(-1);

        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectedIndex.Should().Be(1);

        _toolStripComboBox.SelectedIndex = 0;
        _toolStripComboBox.SelectedIndex.Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectedItem_SetAndGet()
    {
        _toolStripComboBox.SelectedItem.Should().BeNull();

        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedItem = "Item2";
        _toolStripComboBox.SelectedItem.Should().Be("Item2");
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectionLength_SetAndGet()
    {
        _toolStripComboBox.SelectionLength.Should().Be(0);

        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectionLength = 5;
        _toolStripComboBox.SelectionLength.Should().Be(5);
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectionStart_SetAndGet()
    {
        _toolStripComboBox.SelectionStart.Should().Be(0);

        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectionStart = 1;
        _toolStripComboBox.SelectionStart.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDown_EventRaised()
    {
        _toolStripComboBox.ComboBox.DroppedDown.Should().BeFalse();

        bool eventRaised = false;
        _toolStripComboBox.DropDown += (sender, e) => eventRaised = true;
        _toolStripComboBox.ComboBox.DroppedDown = true;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownClosed_EventRaised()
    {
        _toolStripComboBox.ComboBox.DroppedDown.Should().BeFalse();

        bool eventRaised = false;
        _toolStripComboBox.DropDownClosed += (sender, e) => eventRaised = true;
        _toolStripComboBox.ComboBox.DroppedDown = true;
        _toolStripComboBox.ComboBox.DroppedDown = false;
        eventRaised.Should().BeTrue();

        _toolStripComboBox.ComboBox.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownStyleChanged_EventRaised()
    {
        _toolStripComboBox.DropDownStyle.Should().Be(ComboBoxStyle.DropDown);

        bool eventRaised = false;
        _toolStripComboBox.DropDownStyleChanged += (sender, e) => eventRaised = true;
        _toolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        eventRaised.Should().BeTrue();

        _toolStripComboBox.ComboBox.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectedIndexChanged_EventRaised()
    {
        bool eventRaised = false;
        _toolStripComboBox.SelectedIndexChanged += (sender, e) => eventRaised = true;
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_TextUpdate_EventRaised()
    {
        bool eventRaised = false;
        _toolStripComboBox.TextUpdate += (sender, e) => eventRaised = true;

        _toolStripComboBox.ComboBox.Text = "NewText";
        _toolStripComboBox.ComboBox.TestAccessor().Dynamic.OnTextUpdate(EventArgs.Empty);

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_WrappedMethods_InvokeSuccessfully()
    {
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");

        _toolStripComboBox.BeginUpdate();
        _toolStripComboBox.EndUpdate();

        int index = _toolStripComboBox.FindString("Item1");
        index.Should().Be(0);

        index = _toolStripComboBox.FindString("Item2", 1);
        index.Should().Be(1);

        index = _toolStripComboBox.FindStringExact("Item1");
        index.Should().Be(0);

        index = _toolStripComboBox.FindStringExact("Item2", 1);
        index.Should().Be(1);

        int height = _toolStripComboBox.GetItemHeight(0);
        height.Should().BeGreaterThan(0);

        _toolStripComboBox.ComboBox.Text = "Item1";
        _toolStripComboBox.ComboBox.Select(0, 5);
        _toolStripComboBox.SelectedText.Should().Be("Item1");

        _toolStripComboBox.SelectAll();
        _toolStripComboBox.SelectionLength.Should().Be(_toolStripComboBox.Text.Length);
    }
}
