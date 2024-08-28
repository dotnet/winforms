// Licensed to the .NET Foundation under one or more agreements.
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

    [WinFormsTheory]
    [InlineData("TestComboBox", 150, 200, true, 10, false, 20, true)]
    public void ToolStripComboBox_Properties_SetAndGet(string expectedName, int dropDownHeight, int dropDownWidth, bool droppedDown,int maxLength, bool integralHeight,int maxDropDownItems, bool sorted)
    {
        _toolStripComboBox.Name = expectedName;
        _toolStripComboBox.Name.Should().Be(expectedName);

        _toolStripComboBox.DropDownHeight = dropDownHeight;
        _toolStripComboBox.DropDownHeight.Should().Be(dropDownHeight);

        _toolStripComboBox.DropDownWidth = dropDownWidth;
        _toolStripComboBox.DropDownWidth.Should().Be(dropDownWidth);

        _toolStripComboBox.DroppedDown = droppedDown;
        _toolStripComboBox.DroppedDown.Should().Be(droppedDown);

        _toolStripComboBox.MaxLength = maxLength;
        _toolStripComboBox.MaxLength.Should().Be(maxLength);

        _toolStripComboBox.IntegralHeight = integralHeight;
        _toolStripComboBox.IntegralHeight.Should().Be(integralHeight);

        _toolStripComboBox.MaxDropDownItems = maxDropDownItems;
        _toolStripComboBox.MaxDropDownItems.Should().Be(maxDropDownItems);

        _toolStripComboBox.Sorted = sorted;
        _toolStripComboBox.Sorted.Should().Be(sorted);
    }

    [WinFormsFact]
    public void ToolStripComboBox_ConstructorWithControl_ThrowsNotSupportedException()
    {
        Control control = new();

        try
        {
            new Action(() => new ToolStripComboBox(control)).Should().Throw<NotSupportedException>();
        }
        finally
        {
            control?.Dispose();
        }
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
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.Items.Cast<string>().Should().Contain(new[] { "Item1", "Item2" });
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectedIndex_SetAndGet()
    {
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectedIndex.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectedItem_SetAndGet()
    {
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedItem = "Item2";
        _toolStripComboBox.SelectedItem.Should().Be("Item2");
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectionLength_SetAndGet()
    {
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectionLength = 5;
        _toolStripComboBox.SelectionLength.Should().Be(5);
    }

    [WinFormsFact]
    public void ToolStripComboBox_SelectionStart_SetAndGet()
    {
        _toolStripComboBox.Items.Add("Item1");
        _toolStripComboBox.Items.Add("Item2");
        _toolStripComboBox.SelectedIndex = 1;
        _toolStripComboBox.SelectionStart = 1;
        _toolStripComboBox.SelectionStart.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDown_EventRaised()
    {
        bool eventRaised = false;
        _toolStripComboBox.DropDown += (sender, e) => eventRaised = true;
        _toolStripComboBox.ComboBox.DroppedDown = true;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownClosed_EventRaised()
    {
        bool eventRaised = false;
        _toolStripComboBox.DropDownClosed += (sender, e) => eventRaised = true;
        _toolStripComboBox.ComboBox.DroppedDown = true;
        _toolStripComboBox.ComboBox.DroppedDown = false;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripComboBox_DropDownStyleChanged_EventRaised()
    {
        bool eventRaised = false;
        _toolStripComboBox.DropDownStyleChanged += (sender, e) => eventRaised = true;
        _toolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        eventRaised.Should().BeTrue();
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

        var accessor = _toolStripComboBox.ComboBox.TestAccessor();
        accessor.Dynamic.OnTextUpdate(EventArgs.Empty);

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
