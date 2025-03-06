// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using FluentAssertions;

namespace System.Windows.Forms.Tests
{
    public class ListBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListBoxAccessibleObject_State_ShouldBeExpected()
        {
            using Form form = new();
            using ListBox listBox = new();
            using TextBox textBox = new();
            form.Controls.Add(listBox);
            form.Controls.Add(textBox);

            form.Show();
            listBox.CreateControl();

            textBox.Focus();
            listBox.Focused.Should().BeFalse();
            listBox.AccessibilityObject.State.Should().Be(AccessibleStates.Focusable);

            listBox.Focus();
            listBox.Focused.Should().BeTrue();
            listBox.AccessibilityObject.State.Should().Be(AccessibleStates.Focused | AccessibleStates.Focusable);
        }

        [WinFormsFact]
        public void TestHitTest_PointInsideListBoxBoundsButOutsideItems_ReturnsSelf()
        {
            using Form form = new();
            using ListBox listBox = new() { Parent = form, Items = { "Item 1", "Item 2" } };
            listBox.CreateControl();
            form.Show();
            Point testPoint = listBox.PointToScreen(new Point(0, listBox.ClientRectangle.Height - 1));

            var result = listBox.AccessibilityObject.HitTest(testPoint.X, testPoint.Y);

            result.Should().Be(listBox.AccessibilityObject);
        }

        [WinFormsFact]
        public void TestHitTest_PointInsideChildBounds_ReturnsChild()
        {
            using Form form = new();
            using ListBox listBox = new() { Parent = form, Items = { "Item 1", "Item 2" } };
            listBox.CreateControl();
            form.Show();
            listBox.SelectedIndex = 0;
            var itemBounds = listBox.GetItemRectangle(0);
            Point testPoint = listBox.PointToScreen(new Point(itemBounds.Left + 1, itemBounds.Top + 1));

            var result = listBox.AccessibilityObject.HitTest(testPoint.X, testPoint.Y);

            result.Should().Be(listBox.AccessibilityObject.GetChild(0));
        }
    }
}
