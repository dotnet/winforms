// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListBox_ListBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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

            object actual = itemAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

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

            object actual = itemAccessibleObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId);

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

            object actual = itemAccessibleObject.GetPropertyValue(UiaCore.UIA.BoundingRectanglePropertyId);

            Assert.Equal(itemAccessibleObject.BoundingRectangle, actual);
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

            object actual = itemAccessibleObject.GetPropertyValue(UiaCore.UIA.HelpTextPropertyId);

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

            bool actual = (bool)itemAccessibleObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId);

            Assert.False(actual);
            Assert.False(listBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsValuePatternAvailablePropertyId))]
        public void ListBoxItemAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using ListBox listBox = new();
            listBox.Items.Add(item: "testItem");
            ListBox.ListBoxAccessibleObject listBoxAccessibleObject = new(listBox);
            ListBox.ListBoxItemAccessibleObject accessibleObject = (ListBox.ListBoxItemAccessibleObject)listBoxAccessibleObject.GetChild(0);

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId) ?? false);
            Assert.False(listBox.IsHandleCreated);
        }
    }
}
