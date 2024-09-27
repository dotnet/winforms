// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripSplitButtonTests : IDisposable
{
    private readonly ToolStripSplitButton _toolStripSplitButton;

    public ToolStripSplitButtonTests()
    {
        _toolStripSplitButton = new("Test");
    }

    public void Dispose() => _toolStripSplitButton.Dispose();

    public static TheoryData<ToolStripItem?> ToolStripItem_Set_TestData()
    {
        TheoryData<ToolStripItem?> data = new();
        data.Add(null);
        data.Add(new SubToolStripItem());
        return data;
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripItem_Set_TestData))]
    public void ToolStripSplitButton_DefaultItem_Set_GetReturnsExpected(ToolStripItem? value)
    {
        _toolStripSplitButton.DefaultItem = value;
        _toolStripSplitButton.DefaultItem.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DefaultItem_SetWithHandler_CallsOnDefaultItemChanged()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().Be(_toolStripSplitButton);
            e.Should().Be(EventArgs.Empty);
            callCount++;
        };
        _toolStripSplitButton.DefaultItemChanged += handler;

        // Set non-null.
        using SubToolStripItem item1 = new();
        _toolStripSplitButton.DefaultItem = item1;
        _toolStripSplitButton.DefaultItem.Should().Be(item1);
        callCount.Should().Be(1);

        // Set same.
        _toolStripSplitButton.DefaultItem = item1;
        _toolStripSplitButton.DefaultItem.Should().Be(item1);
        callCount.Should().Be(1);

        // Set different.
        using SubToolStripItem item2 = new();
        _toolStripSplitButton.DefaultItem = item2;
        _toolStripSplitButton.DefaultItem.Should().Be(item2);
        callCount.Should().Be(2);

        // Set null.
        _toolStripSplitButton.DefaultItem = null;
        _toolStripSplitButton.DefaultItem.Should().BeNull();
        callCount.Should().Be(3);

        // Remove handler.
        _toolStripSplitButton.DefaultItemChanged -= handler;
        _toolStripSplitButton.DefaultItem = item1;
        _toolStripSplitButton.DefaultItem.Should().Be(item1);
        callCount.Should().Be(3);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String()
    {
        _toolStripSplitButton.Text.Should().Be("Test");

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_ImageAndNullImage()
    {
        using Bitmap image = new(10, 10);
        _toolStripSplitButton.Image = image;
        _toolStripSplitButton.Image.Should().Be(image);

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);

        _toolStripSplitButton.Image = null;
        _toolStripSplitButton.Image.Should().BeNull();
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String_Image()
    {
        using Bitmap image = new(10, 10);
        _toolStripSplitButton.Image = image;

        _toolStripSplitButton.Text.Should().Be("Test");
        _toolStripSplitButton.Image.Should().Be(image);

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String_NullImage()
    {
        _toolStripSplitButton.Text.Should().Be("Test");
        _toolStripSplitButton.Image.Should().BeNull();

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String_Image_EventHandler()
    {
        bool clickInvoked = false;
        EventHandler onClick = (sender, e) => clickInvoked = true;

        using Bitmap image = new(10, 10);
        _toolStripSplitButton.Image = image;
        _toolStripSplitButton.Click += onClick;

        _toolStripSplitButton.Text.Should().Be("Test");
        _toolStripSplitButton.Image.Should().Be(image);

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);

        _toolStripSplitButton.PerformClick();
        clickInvoked.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String_Image_EventHandler_Name()
    {
        bool clickInvoked = false;
        EventHandler onClick = (sender, e) => clickInvoked = true;

        using Bitmap image = new(10, 10);
        _toolStripSplitButton.Image = image;
        _toolStripSplitButton.Click += onClick;
        _toolStripSplitButton.Name = "TestButton";

        _toolStripSplitButton.Text.Should().Be("Test");
        _toolStripSplitButton.Image.Should().Be(image);
        _toolStripSplitButton.Name.Should().Be("TestButton");

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);

        _toolStripSplitButton.PerformClick();
        clickInvoked.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_Ctor_String_Image_DropDownItems()
    {
        using Bitmap image = new(10, 10);
        using ToolStripMenuItem item1 = new("Item1");
        using ToolStripMenuItem item2 = new("Item2");

        _toolStripSplitButton.Image = image;
        _toolStripSplitButton.DropDownItems.AddRange(new[] { item1, item2 });

        _toolStripSplitButton.Text.Should().Be("Test");
        _toolStripSplitButton.Image.Should().Be(image);
        _toolStripSplitButton.DropDownItems.Cast<ToolStripItem>().Should().ContainInOrder(new[] { item1, item2 });

        int defaultDropDownButtonWidth = _toolStripSplitButton.TestAccessor().Dynamic.DefaultDropDownButtonWidth;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(defaultDropDownButtonWidth);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_AutoToolTip_DefaultValue()
    {
        _toolStripSplitButton.AutoToolTip.Should().BeTrue();
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripSplitButton_AutoToolTip_Set_GetReturnsExpected(bool value)
    {
        _toolStripSplitButton.AutoToolTip.Should().BeTrue();

        _toolStripSplitButton.AutoToolTip = value;
        _toolStripSplitButton.AutoToolTip.Should().Be(value);

        _toolStripSplitButton.AutoToolTip = !value;
        _toolStripSplitButton.AutoToolTip.Should().Be(!value);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonBounds_ReturnsExpected()
    {
        Rectangle expectedBounds = _toolStripSplitButton.TestAccessor().Dynamic.SplitButtonButton.Bounds;
        _toolStripSplitButton.ButtonBounds.Should().Be(expectedBounds);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonPressed_ReturnsExpected()
    {
        bool expectedPressed = _toolStripSplitButton.TestAccessor().Dynamic.SplitButtonButton.Pressed;
        _toolStripSplitButton.ButtonPressed.Should().Be(expectedPressed);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonSelected_ReturnsExpected()
    {
        bool expectedSelected = _toolStripSplitButton.TestAccessor().Dynamic.SplitButtonButton.Selected;
        _toolStripSplitButton.ButtonSelected.Should().Be(expectedSelected);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonClick_EventTriggered()
    {
        bool eventTriggered = false;

        EventHandler handler = (sender, e) =>
        {
            eventTriggered = true;
        };

        _toolStripSplitButton.ButtonClick += handler;

        _toolStripSplitButton.PerformButtonClick();

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonClick_EventHandlerCalledExpectedTimes()
    {
        int callCount = 0;

        EventHandler handler = (sender, e) =>
        {
            callCount++;
        };

        _toolStripSplitButton.ButtonClick += handler;

        _toolStripSplitButton.PerformButtonClick();
        _toolStripSplitButton.PerformButtonClick();

        callCount.Should().Be(2);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonClick_EventHandlerRemoved()
    {
        int callCount = 0;

        EventHandler handler = (sender, e) =>
        {
            callCount++;
        };

        _toolStripSplitButton.ButtonClick += handler;
        _toolStripSplitButton.PerformButtonClick();
        _toolStripSplitButton.ButtonClick -= handler;
        _toolStripSplitButton.PerformButtonClick();

        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonDoubleClick_EventTriggered()
    {
        bool eventTriggered = false;
        _toolStripSplitButton.ButtonDoubleClick += (sender, e) => eventTriggered = true;

        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonDoubleClick_EventHandlerCalledExpectedTimes()
    {
        int callCount = 0;
        _toolStripSplitButton.ButtonDoubleClick += (sender, e) => callCount++;

        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);
        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);

        callCount.Should().Be(2);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ButtonDoubleClick_EventHandlerRemoved()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        _toolStripSplitButton.ButtonDoubleClick += handler;
        _toolStripSplitButton.ButtonDoubleClick -= handler;

        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);

        callCount.Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DropDownButtonArea_ReturnsExpected()
    {
        Rectangle expectedBounds = _toolStripSplitButton.DropDownButtonBounds;

        _toolStripSplitButton.DropDownButtonArea.Should().Be(expectedBounds);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DropDownButtonSelected_DefaultValue()
    {
        _toolStripSplitButton.DropDownButtonSelected.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DropDownButtonSelected_Selected()
    {
        _toolStripSplitButton.Select();
        _toolStripSplitButton.DropDownButtonSelected.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void ToolStripSplitButton_DropDownButtonWidth_Set_GetReturnsExpected(int value)
    {
        _toolStripSplitButton.DropDownButtonWidth = value;
        _toolStripSplitButton.DropDownButtonWidth.Should().Be(value);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DropDownButtonWidth_SetInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => _toolStripSplitButton.DropDownButtonWidth = -1;
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_SplitterBounds_DefaultValue()
    {
        _toolStripSplitButton.SplitterBounds.Should().Be(Rectangle.Empty);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_OnButtonClick_TriggersButtonClickEvent()
    {
        bool eventTriggered = false;
        _toolStripSplitButton.ButtonClick += (sender, e) => eventTriggered = true;

        _toolStripSplitButton.TestAccessor().Dynamic.OnButtonClick(EventArgs.Empty);

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_OnButtonClick_RoutesClickToDefaultItem()
    {
        using ToolStripButton defaultItem = new();
        bool eventTriggered = false;
        defaultItem.Click += (sender, e) => eventTriggered = true;
        _toolStripSplitButton.DefaultItem = defaultItem;

        _toolStripSplitButton.TestAccessor().Dynamic.OnButtonClick(EventArgs.Empty);

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_OnButtonDoubleClick_TriggersButtonDoubleClickEvent()
    {
        bool eventTriggered = false;
        _toolStripSplitButton.ButtonDoubleClick += (sender, e) => eventTriggered = true;

        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_OnButtonDoubleClick_RoutesDoubleClickToDefaultItem()
    {
        using ToolStripButton defaultItem = new();
        bool eventTriggered = false;
        defaultItem.DoubleClick += (sender, e) => eventTriggered = true;
        _toolStripSplitButton.DefaultItem = defaultItem;

        _toolStripSplitButton.OnButtonDoubleClick(EventArgs.Empty);

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_PerformButtonClick_TriggersButtonClickEvent()
    {
        bool eventTriggered = false;
        _toolStripSplitButton.ButtonClick += (sender, e) => eventTriggered = true;

        _toolStripSplitButton.PerformButtonClick();

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_PerformButtonClick_RoutesClickToDefaultItem()
    {
        using ToolStripButton defaultItem = new();
        bool eventTriggered = false;
        defaultItem.Click += (sender, e) => eventTriggered = true;
        _toolStripSplitButton.DefaultItem = defaultItem;

        _toolStripSplitButton.PerformButtonClick();

        eventTriggered.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ResetDropDownButtonWidth_SetsToDefault()
    {
        _toolStripSplitButton.DropDownButtonWidth = 20;

        _toolStripSplitButton.ResetDropDownButtonWidth();

        _toolStripSplitButton.DropDownButtonWidth.Should().Be(11);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ResetDropDownButtonWidth_AlreadyDefault_NoChange()
    {
        _toolStripSplitButton.DropDownButtonWidth = 11;

        _toolStripSplitButton.ResetDropDownButtonWidth();

        _toolStripSplitButton.DropDownButtonWidth.Should().Be(11);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ShouldSerializeDropDownButtonWidth_NotDefault_ReturnsTrue()
    {
        _toolStripSplitButton.DropDownButtonWidth = 20;

        bool result = _toolStripSplitButton.ShouldSerializeDropDownButtonWidth();

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripSplitButton_ShouldSerializeDropDownButtonWidth_Default_ReturnsFalse()
    {
        _toolStripSplitButton.DropDownButtonWidth = 11;

        bool result = _toolStripSplitButton.ShouldSerializeDropDownButtonWidth();

        result.Should().BeFalse();
    }

    private class SubToolStripItem : ToolStripItem
    {
        public SubToolStripItem() : base()
        {
        }
    }
}
