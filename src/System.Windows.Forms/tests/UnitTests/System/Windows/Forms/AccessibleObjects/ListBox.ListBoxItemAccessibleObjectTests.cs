// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListBox_ListBoxItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ListBoxItemAccessibleObject_DataBoundAccessibleName()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/3706

        using Form form = new()
        {
            BindingContext = []
        };

        using ListBox control = new()
        {
            Parent = form,
            DisplayMember = TestDataSources.PersonDisplayMember,
            DataSource = TestDataSources.GetPersons()
        };

        ListBox.ListBoxAccessibleObject accessibleObject =
            Assert.IsType<ListBox.ListBoxAccessibleObject>(control.AccessibilityObject);

        List<Person> persons = TestDataSources.GetPersons();
        Assert.Equal(persons.Count, accessibleObject.GetChildCount());

        for (int i = 0; i < persons.Count; i++)
        {
            Person person = persons[i];
            AccessibleObject itemAccessibleObject = accessibleObject.GetChild(i);

            Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);
            Assert.Equal(person.Name, itemAccessibleObject.Name);
        }

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        VARIANT actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);
        if (itemAccessibleObject.Name is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(itemAccessibleObject.Name, ((BSTR)actual).ToStringAndFree());
        }

        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        using VARIANT actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(itemAccessibleObject.RuntimeId, actual.ToObject());
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxItemAccessibleObject_GetPropertyValue_BoundingRectangle_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        using VARIANT actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId);
        double[] actualArray = (double[])actual.ToObject();
        Rectangle actualRectangle = new((int)actualArray[0], (int)actualArray[1], (int)actualArray[2], (int)actualArray[3]);
        Assert.Equal(itemAccessibleObject.BoundingRectangle, actualRectangle);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_HelpText_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        string actual = ((BSTR)itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree();

        Assert.Equal(itemAccessibleObject.Help ?? string.Empty, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        bool actual = (bool)itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.False(actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void ListBoxItemAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject listBoxAccessibleObject = new(listBox);
        ListBox.ListBoxItemAccessibleObject accessibleObject = (ListBox.ListBoxItemAccessibleObject)listBoxAccessibleObject.GetChild(0);
        VARIANT result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);
        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(listBox.IsHandleCreated);
    }

#nullable enable

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_VerifyProperties()
    {
        using ListBox listBox = new();
        listBox.Items.Add("Item 1");

        var accessibleObject = listBox.AccessibilityObject;
        var itemAccessibleObject = accessibleObject.GetChild(0).Should().BeOfType<ListBox.ListBoxItemAccessibleObject>().Which;

        itemAccessibleObject.Bounds.Should().Be(Rectangle.Empty);

        listBox.CreateControl();
        listBox.IsHandleCreated.Should().BeTrue();

        itemAccessibleObject.Role.Should().Be(AccessibleRole.ListItem);
        itemAccessibleObject.Name.Should().Be("Item 1");
        itemAccessibleObject.DefaultAction.Should().NotBeNull();
    }

    [WinFormsTheory]
    [InlineData(AccessibleSelection.AddSelection)]
    [InlineData(AccessibleSelection.RemoveSelection)]
    [InlineData(AccessibleSelection.TakeFocus)]
    [InlineData((AccessibleSelection)int.MaxValue)]
    public void Select_WithVariousFlags_ShouldNotThrow(AccessibleSelection flags)
    {
        using ListBox listBox = new();
        ItemArray.Entry itemEntry = new ItemArray.Entry("Test Item");
        ListBox.ListBoxAccessibleObject listBoxAccessibleObject = new(listBox);
        ListBox.ListBoxItemAccessibleObject listBoxItemAccessibleObject = new(listBox, itemEntry, listBoxAccessibleObject);

        Action action = () => listBoxItemAccessibleObject.Select(flags);

        action.Should().NotThrow();
    }

#nullable disable
}
