// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using FluentAssertions;
using Moq;
using static System.Windows.Forms.PropertyGrid;

namespace System.Windows.Forms.UITests;

public class PropertyGridTests : IDisposable
{
    private readonly PropertyGrid _propertyGrid;
    private readonly Form _form;

    public PropertyGridTests()
    {
        _propertyGrid = new();
        _form = new() { Controls = { _propertyGrid } };
        _propertyGrid.CreateControl();
        _propertyGrid.SelectedObject = _form;
        _form.Show();
    }

    public void Dispose()
    {
        _form.Dispose();
        _propertyGrid.Dispose();
    }

    [WinFormsFact]
    public void PropertyGrid_Focused_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.Focused.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_ToolbarVisible_GetSet_ReturnsExpected()
    {
        _propertyGrid.ToolbarVisible.Should().BeTrue();

        _propertyGrid.ToolbarVisible = false;
        _propertyGrid.ToolbarVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_HelpVisible_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpVisible.Should().BeTrue();

        _propertyGrid.HelpVisible = false;
        _propertyGrid.HelpVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsVisibleIfAvailable_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsVisibleIfAvailable.Should().BeTrue();

        _propertyGrid.CommandsVisibleIfAvailable = false;
        _propertyGrid.CommandsVisibleIfAvailable.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsVisible_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.CommandsVisible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CanShowCommands_Get_ShouldBeFalseByDefault()
    {
        _propertyGrid.CanShowCommands.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_CanShowVisualStyleGlyphs_GetSet_ReturnsExpected()
    {
        _propertyGrid.CanShowVisualStyleGlyphs.Should().BeTrue();

        _propertyGrid.CanShowVisualStyleGlyphs = false;
        _propertyGrid.CanShowVisualStyleGlyphs.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_AutoScroll_GetSet_ReturnsExpected()
    {
        _propertyGrid.AutoScroll.Should().BeFalse();

        _propertyGrid.AutoScroll = true;
        _propertyGrid.AutoScroll.Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_RefreshEvent_Raised_ShouldNotThrowException()
    {
        Action act = _propertyGrid.Refresh;
        act.Should().NotThrow();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsBorderColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsBorderColor.Should().Be(SystemColors.ControlDark);

        _propertyGrid.CommandsBorderColor = Color.Red;
        _propertyGrid.CommandsBorderColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_HelpBorderColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpBorderColor.Should().Be(SystemColors.ControlDark);

        _propertyGrid.HelpBorderColor = Color.Blue;
        _propertyGrid.HelpBorderColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedItemWithFocusBackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedItemWithFocusBackColor.Should().Be(SystemColors.Highlight);

        _propertyGrid.SelectedItemWithFocusBackColor = Color.Green;
        _propertyGrid.SelectedItemWithFocusBackColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedItemWithFocusForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedItemWithFocusForeColor.Should().Be(SystemColors.HighlightText);

        _propertyGrid.SelectedItemWithFocusForeColor = Color.Red;
        _propertyGrid.SelectedItemWithFocusForeColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_DisabledItemForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.DisabledItemForeColor.Should().Be(SystemColors.GrayText);

        _propertyGrid.DisabledItemForeColor = Color.Blue;
        _propertyGrid.DisabledItemForeColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_CategorySplitterColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CategorySplitterColor.Should().Be(SystemColors.Control);

        _propertyGrid.CategorySplitterColor = Color.Green;
        _propertyGrid.CategorySplitterColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_ForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.ForeColor.Should().Be(SystemColors.ControlText);

        _propertyGrid.ForeColor = Color.Red;
        _propertyGrid.ForeColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_BackgroundImageLayout_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackgroundImageLayout.Should().Be(ImageLayout.Tile);

        _propertyGrid.BackgroundImageLayout = ImageLayout.Center;
        _propertyGrid.BackgroundImageLayout.Should().Be(ImageLayout.Center);
    }

    [WinFormsFact]
    public void PropertyGrid_BackgroundImage_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackgroundImage.Should().BeNull();

        using Image newImage = new Bitmap(100, 100);
        _propertyGrid.BackgroundImage = newImage;
        _propertyGrid.BackgroundImage.Should().Be(newImage);
    }

    [WinFormsFact]
    public void PropertyGrid_BackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.BackColor.Should().Be(SystemColors.Control);

        _propertyGrid.BackColor = Color.Blue;
        _propertyGrid.BackColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_BrowsableAttributes_GetSet_ReturnsExpected()
    {
        _propertyGrid.BrowsableAttributes[0].Should().Be(BrowsableAttribute.Default);

        AttributeCollection newAttributes = new(new BrowsableAttribute(true));
        _propertyGrid.BrowsableAttributes = newAttributes;
        _propertyGrid.BrowsableAttributes.Contains(newAttributes[0]).Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsBackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsBackColor.Should().Be(SystemColors.Control);

        _propertyGrid.CommandsBackColor = Color.Green;
        _propertyGrid.CommandsBackColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsForeColor.Should().Be(SystemColors.ControlText);

        _propertyGrid.CommandsForeColor = Color.Red;
        _propertyGrid.CommandsForeColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_ContextMenuDefaultLocation_GetSet_ReturnsExpected()
    {
        _propertyGrid.ContextMenuDefaultLocation.Should().NotBeNull();

        Point oldLocation = _propertyGrid.ContextMenuDefaultLocation;
        Point newLocation = new(10, 20);

        _propertyGrid.Location = newLocation;
        _propertyGrid.ContextMenuDefaultLocation.X.Should().Be(oldLocation.X + newLocation.X);
        _propertyGrid.ContextMenuDefaultLocation.Y.Should().Be(oldLocation.Y + newLocation.Y);
    }

    [WinFormsFact]
    public void PropertyGrid_Controls_GetSet_ReturnsExpected()
    {
        _propertyGrid.Controls.Should().NotBeNull();

        using TextBox textBox = new();
        _propertyGrid.Controls.Add(textBox);
        _propertyGrid.Controls.Contains(textBox).Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_HelpBackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpBackColor.Should().Be(SystemColors.Control);

        _propertyGrid.HelpBackColor = Color.Blue;
        _propertyGrid.HelpBackColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_HelpForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.HelpForeColor.Should().Be(SystemColors.ControlText);

        _propertyGrid.HelpForeColor = Color.Green;
        _propertyGrid.HelpForeColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_LineColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.LineColor.Should().Be(SystemColors.InactiveBorder);

        _propertyGrid.LineColor = Color.Red;
        _propertyGrid.LineColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_PropertySort_GetSet_ReturnsExpected()
    {
        _propertyGrid.PropertySort.Should().Be(PropertySort.CategorizedAlphabetical);

        _propertyGrid.PropertySort = PropertySort.Alphabetical;
        _propertyGrid.PropertySort.Should().Be(PropertySort.Alphabetical);
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedObject_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedObject.Should().Be(_form);

        using TextBox textBox1 = new();
        _propertyGrid.SelectedObject = textBox1;
        _propertyGrid.SelectedObject.Should().Be(textBox1);

        _propertyGrid.SelectedObject = null;
        _propertyGrid.SelectedObject.Should().BeNull();
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedObjects_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedObjects.Should().Contain(_form);

        using Button button1 = new();
        using TextBox textBox1 = new();
        using ComboBox comboBox1 = new();
        object[] objects = [button1, textBox1, comboBox1];

        _propertyGrid.SelectedObjects = objects;
        _propertyGrid.SelectedObjects.Should().Contain(objects);

        _propertyGrid.SelectedObjects = [];
        _propertyGrid.SelectedObjects.Should().BeEmpty();
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedObjects_Set_ThrowExpectedException()
    {
        object[] objects = [null!];
        Action act = () => _propertyGrid.SelectedObjects = objects;
        act.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedTab_Get_ShouldNotBeNullByDefault()
    {
        _propertyGrid.SelectedTab.Should().NotBeNull();
    }

    [WinFormsFact]
    public void PropertyGrid_LargeButtons_GetSet_ReturnsExpected()
    {
        _propertyGrid.LargeButtons.Should().BeFalse();

        _propertyGrid.LargeButtons = true;
        _propertyGrid.LargeButtons.Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_ViewBackColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.ViewBackColor.Should().Be(SystemColors.Window);

        _propertyGrid.ViewBackColor = Color.Red;
        _propertyGrid.ViewBackColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void PropertyGrid_ViewBorderColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.ViewBorderColor.Should().Be(SystemColors.ControlDark);

        _propertyGrid.ViewBorderColor = Color.Blue;
        _propertyGrid.ViewBorderColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void PropertyGrid_ViewForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.ViewForeColor.Should().Be(SystemColors.WindowText);

        _propertyGrid.ViewForeColor = Color.Green;
        _propertyGrid.ViewForeColor.Should().Be(Color.Green);
    }

    [WinFormsFact]
    public void PropertyGrid_CategoryForeColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CategoryForeColor.Should().Be(SystemColors.ControlText);

        _propertyGrid.CategoryForeColor = SystemColors.MenuHighlight;
        _propertyGrid.CategoryForeColor.Should().Be(SystemColors.MenuHighlight);
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsLinkColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsLinkColor.A.Should().Be(255);
        _propertyGrid.CommandsLinkColor.R.Should().Be(0);
        _propertyGrid.CommandsLinkColor.G.Should().Be(0);
        _propertyGrid.CommandsLinkColor.B.Should().Be(255);

        _propertyGrid.CommandsLinkColor = Color.Silver;
        _propertyGrid.CommandsLinkColor.Should().Be(Color.Silver);
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsActiveLinkColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsActiveLinkColor.Should().Be(Color.Red);

        _propertyGrid.CommandsActiveLinkColor = SystemColors.ControlDark;
        _propertyGrid.CommandsActiveLinkColor.Should().Be(SystemColors.ControlDark);
    }

    [WinFormsFact]
    public void PropertyGrid_CommandsDisabledLinkColor_GetSet_ReturnsExpected()
    {
        _propertyGrid.CommandsDisabledLinkColor.A.Should().Be(255);
        _propertyGrid.CommandsDisabledLinkColor.R.Should().Be(133);
        _propertyGrid.CommandsDisabledLinkColor.G.Should().Be(133);
        _propertyGrid.CommandsDisabledLinkColor.B.Should().Be(133);

        _propertyGrid.CommandsDisabledLinkColor = Color.Orange;
        _propertyGrid.CommandsDisabledLinkColor.Should().Be(Color.Orange);
    }

    [WinFormsFact]
    public void PropertyGrid_ResetSelectedPropertyEvent_Raised_Success()
    {
        using TextBox textBox = new();
        _propertyGrid.SelectedObject = textBox;

        PropertyDescriptor? propertyDescriptor = TypeDescriptor.GetProperties(textBox)["Text"];
        propertyDescriptor.Should().NotBeNull();

        string originalValue = propertyDescriptor?.GetValue(textBox) as string ?? string.Empty;
        originalValue.Should().NotBeNull();

        textBox.Text = "New TextBox Text";

        _propertyGrid.ResetSelectedProperty();
        propertyDescriptor!.GetValue(textBox).Should().Be(originalValue);
    }

    [WinFormsFact]
    public void PropertyGrid_HorizontalScroll_Get_ShouldReturnExpected()
    {
        HScrollProperties horizontalScroll = _propertyGrid.HorizontalScroll;
        horizontalScroll.Visible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_VerticalScroll_Get_ShouldReturnExpected()
    {
        VScrollProperties verticalScroll = _propertyGrid.VerticalScroll;
        verticalScroll.Visible.Should().BeFalse();
    }

    [WinFormsFact]
    public void PropertyGrid_UseCompatibleTextRendering_GetSet_ReturnsExpected()
    {
        _propertyGrid.UseCompatibleTextRendering.Should().BeTrue();

        _propertyGrid.UseCompatibleTextRendering = false;
        _propertyGrid.UseCompatibleTextRendering.Should().BeFalse();

        _propertyGrid.UseCompatibleTextRendering = true;
        _propertyGrid.UseCompatibleTextRendering.Should().BeTrue();
    }

    [WinFormsFact]
    public void PropertyGrid_Site_GetSet_ReturnsExpected()
    {
        _propertyGrid.Site.Should().BeNull();

        Mock<ISite> mockSite = new();
        _propertyGrid.Site = mockSite.Object;
        _propertyGrid.Site.Should().Be(mockSite.Object);
    }

    [WinFormsFact]
    public void PropertyGrid_PropertyTabs_Get_ReturnsExpected()
    {
        _propertyGrid.PropertyTabs.Should().NotBeNull();

        PropertyTabCollection propertyTabCollection = _propertyGrid.PropertyTabs;
        PropertyGrid propertyGrid = propertyTabCollection.TestAccessor().Dynamic._ownerPropertyGrid;
        propertyGrid.Should().Be(_propertyGrid);
    }

    [WinFormsFact]
    public void PropertyGrid_RefreshTabsEvent_Raised_Success()
    {
        PropertyTabCollection initialTabs = _propertyGrid.PropertyTabs;
        int initialTabCount = initialTabs.Count;

        _propertyGrid.RefreshTabs(PropertyTabScope.Component);
        PropertyTabCollection refreshedTabs = _propertyGrid.PropertyTabs;
        int refreshedTabCount = refreshedTabs.Count;

        refreshedTabCount.Should().Be(initialTabCount);
        for (int i = 0; i < initialTabCount; i++)
        {
            refreshedTabs[i].Should().BeSameAs(initialTabs[i]);
        }
    }

    [WinFormsFact]
    public void PropertyGrid_SelectedGridItem_GetSet_ReturnsExpected()
    {
        _propertyGrid.SelectedGridItem.Should().NotBeNull();
        _propertyGrid.SelectedGridItem!.Label.Should().Be("Text");

        _propertyGrid.SelectedObject = _propertyGrid;
        _propertyGrid.SelectedGridItem.Label.Should().Be("Accessibility");

        GridItem gridItem = _propertyGrid.SelectedGridItem.GridItems[0];
        _propertyGrid.SelectedGridItem = gridItem;
        _propertyGrid.SelectedGridItem.Should().Be(gridItem);
    }
}
