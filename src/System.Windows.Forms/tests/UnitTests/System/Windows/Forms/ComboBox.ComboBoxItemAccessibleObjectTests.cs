﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using static System.Windows.Forms.ComboBox;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_Get_Not_ThrowsException()
        {
            using (new NoAssertContext())
            {
                using var control = new ComboBox();

                var item1 = new HashNotImplementedObject();
                var item2 = new HashNotImplementedObject();
                var item3 = new HashNotImplementedObject();

                control.Items.AddRange(new[] { item1, item2, item3 });

                var comboBoxAccessibleObject = (ComboBox.ComboBoxAccessibleObject)control.AccessibilityObject;

                var exceptionThrown = false;

                try
                {
                    var item1AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item1];
                    var item2AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item2];
                    var item3AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item3];
                }
                catch
                {
                    exceptionThrown = true;
                }

                Assert.False(exceptionThrown, "Getting accessible object for ComboBox item has thrown an exception.");
            }
        }

        public class HashNotImplementedObject
        {
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_DataBoundAccessibleName()
        {
            using (new NoAssertContext())
            {
                // Regression test for https://github.com/dotnet/winforms/issues/3549
                using var control = new ComboBox()
                {
                    DataSource = TestDataSources.GetPersons(),
                    DisplayMember = TestDataSources.PersonDisplayMember
                };

                ComboBox.ComboBoxAccessibleObject accessibleObject = Assert.IsType<ComboBox.ComboBoxAccessibleObject>(control.AccessibilityObject);

                foreach (Person person in TestDataSources.GetPersons())
                {
                    var item = accessibleObject.ItemAccessibleObjects[person];
                    AccessibleObject itemAccessibleObject = Assert.IsType<ComboBox.ComboBoxItemAccessibleObject>(item);
                    Assert.Equal(person.Name, itemAccessibleObject.Name);
                }
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_IsPatternSupported_ReturnsTrue_ForScrollItemPattern()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.Items.AddRange(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBox.CreateControl();

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;

            // Check that all items support ScrollItemPattern
            foreach (string item in comboBox.Items)
            {
                ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];

                Assert.True(itemAccessibleObject.IsPatternSupported(UiaCore.UIA.ScrollItemPatternId));
            }
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_GetPropertyValue_ScrollItemPattern_IsAvailable()
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.Items.AddRange(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBox.CreateControl();

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;

            // Check that all items support ScrollItemPattern
            foreach (string item in comboBox.Items)
            {
                ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];

                Assert.True((bool)itemAccessibleObject.GetPropertyValue(UiaCore.UIA.IsScrollItemPatternAvailablePropertyId));
            }
        }

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_ScrollIntoView_DoNothing_IfControlIsNotEnabled_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                yield return new object[] { comboBoxStyle };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxItemAccessibleObject_ScrollIntoView_DoNothing_IfControlIsNotEnabled_TestData))]
        public void ComboBoxItemAccessibleObject_ScrollIntoView_DoNothing_IfControlIsNotEnabled(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.IntegralHeight = false;
            comboBox.DropDownStyle = comboBoxStyle;
            comboBox.Enabled = false;
            comboBox.Items.AddRange(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBox.CreateControl();

            if (comboBoxStyle == ComboBoxStyle.Simple)
            {
                comboBox.Size = new Size(100, 132);
            }
            else
            {
                comboBox.DropDownHeight = 107;
                comboBox.DroppedDown = true;
            }

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;
            object item = comboBox.Items[10];
            ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];

            itemAccessibleObject.ScrollIntoView();

            int actual = unchecked((int)(long)User32.SendMessageW(comboBox, (User32.WM)User32.CB.GETTOPINDEX));

            Assert.Equal(0, actual); // ScrollIntoView didn't scroll to the tested item because the combobox is disabled
        }

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_ScrollIntoView_TestData()
        {
            foreach (bool scrollingDown in new[] { true, false })
            {
                foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
                {
                    int itemsCount = 41;

                    for (int index = 0; index < itemsCount; index++)
                    {
                        yield return new object[] { comboBoxStyle, scrollingDown, index, itemsCount };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxItemAccessibleObject_ScrollIntoView_TestData))]
        public void ComboBoxItemAccessibleObject_ScrollIntoView_DoExpected(ComboBoxStyle comboBoxStyle, bool scrollingDown, int itemIndex, int itemsCount)
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.IntegralHeight = false;
            comboBox.DropDownStyle = comboBoxStyle;
            comboBox.CreateControl();

            for (int i = 0; i < itemsCount; i++)
            {
                comboBox.Items.Add(i);
            }

            if (comboBoxStyle == ComboBoxStyle.Simple)
            {
                comboBox.Size = new Size(100, 132);
            }
            else
            {
                comboBox.DropDownHeight = 107;
                comboBox.DroppedDown = true;
            }

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;
            object item = comboBox.Items[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];

            int expected;
            Rectangle dropDownRect = comboBox.ChildListAccessibleObject.Bounds;
            int visibleItemsCount = (int)Math.Ceiling((double)dropDownRect.Height / comboBox.ItemHeight);

            // Get an index of the first item that is visible if dropdown is scrolled to the bottom
            int lastFirstVisible = itemsCount - visibleItemsCount;

            if (scrollingDown)
            {
                if (dropDownRect.IntersectsWith(itemAccessibleObject.Bounds))
                {
                    // ScrollIntoView method shouldn't scroll to the item because it is already visible
                    expected = 0;
                }
                else
                {
                    //  ScrollIntoView method should scroll to the item or
                    //  the first item that is visible if dropdown is scrolled to the bottom
                    expected = itemIndex > lastFirstVisible ? lastFirstVisible : itemIndex;
                }
            }
            else
            {
                // Scroll to the bottom and test the method when scrolling up
                User32.SendMessageW(comboBox, (User32.WM)User32.CB.SETTOPINDEX, (IntPtr)(itemsCount - 1));

                if (dropDownRect.IntersectsWith(itemAccessibleObject.Bounds))
                {
                    // ScrollIntoView method shouldn't scroll to the item because it is already visible
                    expected = lastFirstVisible;
                }
                else
                {
                    // ScrollIntoView method should scroll to the item
                    expected = itemIndex;
                }
            }

            itemAccessibleObject.ScrollIntoView();

            int actual = unchecked((int)(long)User32.SendMessageW(comboBox, (User32.WM)User32.CB.GETTOPINDEX));

            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForVisibleItems_IfComboBoxIsScrollable_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                // The tested combobox contains 11 items
                for (int index = 0; index < 11; index++)
                {
                    int y = index * 15;
                    int initialYPosition = comboBoxStyle == ComboBoxStyle.Simple ? 56 : 55;
                    int x = comboBoxStyle == ComboBoxStyle.Simple ? 10 : 9;
                    Point point = new Point(x, y + initialYPosition);
                    yield return new object[] { comboBoxStyle, index, point };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForVisibleItems_IfComboBoxIsScrollable_TestData))]
        public void ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForVisibleItems_IfComboBoxIsScrollable(ComboBoxStyle comboBoxStyle, int itemIndex, Point expectedPosition)
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.IntegralHeight = false;
            comboBox.DropDownStyle = comboBoxStyle;
            comboBox.Items.AddRange(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBox.CreateControl();

            if (comboBoxStyle == ComboBoxStyle.Simple)
            {
                comboBox.Size = new Size(100, 132);
            }
            else
            {
                comboBox.Size = new Size(100, comboBox.Size.Height);
                comboBox.DropDownHeight = 105;
                comboBox.DroppedDown = true;
            }

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;
            object item = comboBox.Items[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];
            Rectangle actual = itemAccessibleObject.Bounds;
            Rectangle dropdownRect = comboBox.ChildListAccessibleObject.Bounds;
            int itemWidth = comboBoxStyle == ComboBoxStyle.Simple ? 79 : 81;

            Assert.Equal(expectedPosition.X, actual.X);
            Assert.Equal(expectedPosition.Y, actual.Y);
            Assert.Equal(itemWidth, actual.Width); // All items are the same width
            Assert.Equal(15, actual.Height); // All items are the same height
        }

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForDifferentHeightItems_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                // The tested combobox contains 11 items
                for (int index = 0; index < 11; index++)
                {
                    int height = DifferentHeightComboBox.GetCustomItemHeight(index);
                    int width = comboBoxStyle == ComboBoxStyle.Simple ? 96 : 81;
                    int x = comboBoxStyle == ComboBoxStyle.Simple ? 10 : 9;
                    int y = comboBoxStyle == ComboBoxStyle.Simple ? 57 : 56;

                    for (int i = 0; i < index; i++)
                    {
                        y += DifferentHeightComboBox.GetCustomItemHeight(i); // Calculate the sum of heights of all items before the current
                    }

                    yield return new object[] { comboBoxStyle, index, new Rectangle(x, y, width, height) };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForDifferentHeightItems_TestData))]
        public void ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_ForDifferentHeightItems(ComboBoxStyle comboBoxStyle, int itemIndex, Rectangle expectedRect)
        {
            using DifferentHeightComboBox comboBox = new DifferentHeightComboBox();
            comboBox.DropDownStyle = comboBoxStyle;
            comboBox.Items.AddRange(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBox.CreateControl();

            if (comboBoxStyle == ComboBoxStyle.Simple)
            {
                comboBox.Size = new Size(100, 400);
            }
            else
            {
                comboBox.Size = new Size(100, comboBox.Size.Height);
                comboBox.DropDownHeight = 400;
                comboBox.DroppedDown = true;
            }

            ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBoxAccessibleObject)comboBox.AccessibilityObject;
            ComboBoxItemAccessibleObjectCollection itemsCollection = comboBoxAccessibleObject.ItemAccessibleObjects;
            object item = comboBox.Items[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = (ComboBoxItemAccessibleObject)comboBoxAccessibleObject.ItemAccessibleObjects[item];
            Rectangle actual = itemAccessibleObject.Bounds;
            Rectangle dropdownRect = comboBox.ChildListAccessibleObject.Bounds;

            Assert.Equal(expectedRect, actual);
        }

        private class DifferentHeightComboBox : ComboBox
        {
            public DifferentHeightComboBox() : base()
            {
                DrawMode = DrawMode.OwnerDrawVariable;
            }

            public static int GetCustomItemHeight(int index) => 15 + (index % 5) * 5;

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                e.ItemHeight = GetCustomItemHeight(e.Index);
                base.OnMeasureItem(e);
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index < 0 || e.Index >= Items.Count)
                {
                    return;
                }

                e.DrawBackground();
                using var brush = new SolidBrush(e.ForeColor);
                e.Graphics.DrawString(
                    Items[e.Index].ToString(),
                    e.Font,
                    brush,
                    e.Bounds);
                e.DrawFocusRectangle();
                base.OnDrawItem(e);
            }
        }
    }
}
