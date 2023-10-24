// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Automation;
using System.Windows.Forms.IntegrationTests.Common;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListBox_ListBoxItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ListBoxItemAccessibleObject_DataBoundAccessibleName()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/3706

        using var form = new Form
        {
            BindingContext = new BindingContext()
        };

        using var control = new ListBox
        {
            Parent = form,
            DisplayMember = TestDataSources.PersonDisplayMember,
            DataSource = TestDataSources.GetPersons()
        };

        ListBox.ListBoxAccessibleObject accessibleObject =
            Assert.IsType<ListBox.ListBoxAccessibleObject>(control.AccessibilityObject);

        Collections.Generic.List<Person> persons = TestDataSources.GetPersons();
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

        object actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal(itemAccessibleObject.Name, actual);
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

        object actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(itemAccessibleObject.RuntimeId, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_BoundingRectangle_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.Add(item: "testItem");
        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
        AccessibleObject itemAccessibleObject = accessibleObject.GetChild(0);

        Assert.IsType<ListBox.ListBoxItemAccessibleObject>(itemAccessibleObject);

        object actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId);
        using SafeArrayScope<double> rectArray = UiaTextProvider.BoundingRectangleAsArray(itemAccessibleObject.BoundingRectangle);
        Assert.Equal(((VARIANT)rectArray).ToObject(), actual);
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

        object actual = itemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId);

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

        Assert.Equal(expected, accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId) ?? false);
        Assert.False(listBox.IsHandleCreated);
    }
}
