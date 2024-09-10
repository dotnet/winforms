// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using FluentAssertions;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ListBoxItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListBoxItemAccessibleObject_Bounds_BeforeAndAfterHandleCreation()
        {
            using Form form = new();
            using ListBox listBox = new() { Parent = form };
            listBox.Items.Add("Test Item");

            var itemAccessibleObject = listBox.AccessibilityObject.GetChild(0).Should().BeOfType<ListBox.ListBoxItemAccessibleObject>().Which;

            var boundsBeforeHandleCreation = itemAccessibleObject.Bounds;
            boundsBeforeHandleCreation.Should().Be(Rectangle.Empty);

            form.Show();

            listBox.Height = 200;
            listBox.Width = 100;

            var boundsAfterHandleCreation = itemAccessibleObject.Bounds;
            boundsAfterHandleCreation.Should().NotBe(Rectangle.Empty);
            boundsAfterHandleCreation.Width.Should().BeLessOrEqualTo(listBox.Width);
            boundsAfterHandleCreation.Height.Should().BeLessOrEqualTo(listBox.Height);
        }

        [WinFormsFact]
        public void ListBoxItemAccessibleObject_DefaultAction_VariesByContext()
        {
            using Form formDoubleClick = new();
            using ListBox listBoxDoubleClick = new() { Items = { "Item 1" }, Parent = formDoubleClick };
            formDoubleClick.Show();

            var itemAccessibleObjectDoubleClick = listBoxDoubleClick.AccessibilityObject.GetChild(0).Should().BeOfType<ListBox.ListBoxItemAccessibleObject>().Which;
            itemAccessibleObjectDoubleClick.DefaultAction.Should().Be("Double Click");

            using ListBox listBoxNullAction = new ListBox { Items = { "Item 2" } };
            var itemAccessibleObjectNullAction = listBoxNullAction.AccessibilityObject.GetChild(0).Should().BeOfType<ListBox.ListBoxItemAccessibleObject>().Which;
            itemAccessibleObjectNullAction.DefaultAction.Should().BeNull();
        }

        [WinFormsFact]
        public void TestDoDefaultAction_HandleCreatedAndNotCreated()
        {
            using Form form = new();
            using ListBox listBox = new() { Parent = form };
            listBox.Items.Add("Test Item");

            var itemAccessibleObject = listBox.AccessibilityObject.GetChild(0).Should().BeOfType<ListBox.ListBoxItemAccessibleObject>().Which;
            itemAccessibleObject.DoDefaultAction();

            listBox.Focused.Should().BeFalse();

            form.Show();

            listBox.IsHandleCreated.Should().BeTrue();

            itemAccessibleObject.DoDefaultAction();

            listBox.Focused.Should().BeTrue();
        }
    }
}
