﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using static System.Windows.Forms.ComboBox;
using static System.Windows.Forms.ComboBox.ObjectCollection;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        private const AccessibleStates InvisibleItemState = AccessibleStates.Invisible | AccessibleStates.Offscreen | AccessibleStates.Focusable | AccessibleStates.Selectable;
        private const AccessibleStates VisibleItemState = AccessibleStates.Selected | AccessibleStates.Focusable | AccessibleStates.Selectable;

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_Get_Not_ThrowsException()
        {
            using (new NoAssertContext())
            {
                using ComboBox control = new ComboBox();

                HashNotImplementedObject item1 = new();
                HashNotImplementedObject item2 = new();
                HashNotImplementedObject item3 = new();

                control.Items.AddRange(new[] { item1, item2, item3 });

                ComboBox.ComboBoxAccessibleObject comboBoxAccessibleObject = (ComboBox.ComboBoxAccessibleObject)control.AccessibilityObject;

                bool exceptionThrown = false;

                try
                {
                    ComboBox.ComboBoxItemAccessibleObject item1AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[0]);
                    ComboBox.ComboBoxItemAccessibleObject item2AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[1]);
                    ComboBox.ComboBoxItemAccessibleObject item3AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(control.Items.InnerList[2]);
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
                using ComboBox control = new ComboBox()
                {
                    DataSource = TestDataSources.GetPersons(),
                    DisplayMember = TestDataSources.PersonDisplayMember
                };

                ComboBox.ComboBoxAccessibleObject accessibleObject = Assert.IsType<ComboBox.ComboBoxAccessibleObject>(control.AccessibilityObject);

                foreach (Person person in TestDataSources.GetPersons())
                {
                    ComboBox.ComboBoxItemAccessibleObject item = accessibleObject.ItemAccessibleObjects.GetComboBoxItemAccessibleObject(new Entry(person));
                    AccessibleObject itemAccessibleObject = Assert.IsType<ComboBox.ComboBoxItemAccessibleObject>(item);
                    Assert.Equal(person.Name, itemAccessibleObject.Name);
                }
            }
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxItemAccessibleObject_SeveralSameItems_FragmentNavigate_NextSibling_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            comboBox.Items.AddRange(new[] { "aaa", "aaa", "aaa" });
            comboBox.CreateControl();

            ComboBoxItemAccessibleObjectCollection itemAccessibleObjects = ((ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject).ItemAccessibleObjects;

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("aaa", comboBoxItem1.Name);
            Assert.Equal(comboBoxItem1, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[0]));

            // FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling) should return accessible object for second "aaa" item
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            Assert.NotEqual(comboBoxItem1, comboBoxItem2);
            Assert.Equal("aaa", comboBoxItem2.Name);
            Assert.Equal(comboBoxItem2, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[1]));

            // FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling) should return accessible object for third "aaa" item
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            Assert.NotEqual(comboBoxItem3, comboBoxItem2);
            Assert.NotEqual(comboBoxItem3, comboBoxItem1);
            Assert.Equal("aaa", comboBoxItem3.Name);
            Assert.Equal(comboBoxItem3, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[2]));

            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxItemAccessibleObject_SeveralSameItems_FragmentNavigate_PreviousSibling_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            comboBox.Items.AddRange(new[] { "aaa", "aaa", "aaa" });
            comboBox.CreateControl();

            ComboBoxItemAccessibleObjectCollection itemAccessibleObjects = ((ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject).ItemAccessibleObjects;

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild);

            Assert.Equal("aaa", comboBoxItem3.Name);
            Assert.Equal(comboBoxItem3, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[2]));

            // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return accessible object for second "aaa" item
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem3
                .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);

            Assert.NotEqual(comboBoxItem2, comboBoxItem3);
            Assert.Equal("aaa", comboBoxItem2.Name);
            Assert.Equal(comboBoxItem2, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[1]));

            // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return accessible object for first "aaa" item
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);
            Assert.NotEqual(comboBoxItem1, comboBoxItem2);
            Assert.NotEqual(comboBoxItem1, comboBoxItem3);
            Assert.Equal("aaa", comboBoxItem1.Name);
            Assert.Equal(comboBoxItem1, itemAccessibleObjects.GetComboBoxItemAccessibleObject(comboBox.Items.InnerList[0]));

            // FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling) should return null for first "aaa" item
            ComboBox.ComboBoxItemAccessibleObject comboBoxItemPrevious = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                .FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling);
            Assert.Null(comboBoxItemPrevious);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxItemAccessibleObject_FragmentNavigate_Parent_ReturnListAccessibleObject(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBox(comboBoxStyle);
            AccessibleObject childListAccessibleObject = comboBox.ChildListAccessibleObject;
            AccessibleObject comboBoxItem = (AccessibleObject)childListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);

            Assert.Equal(childListAccessibleObject, comboBoxItem.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxItemAccessibleObject_FragmentNavigate_Child_ReturnNull(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBox(comboBoxStyle);
            AccessibleObject childListAccessibleObject = comboBox.ChildListAccessibleObject;
            AccessibleObject comboBoxItem = (AccessibleObject)childListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);

            Assert.Null(comboBoxItem.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(comboBoxItem.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
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
            foreach (Entry itemEntry in comboBox.Items.InnerList)
            {
                ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);

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
            foreach (Entry itemEntry in comboBox.Items.InnerList)
            {
                ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);

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
            Entry itemEntry = comboBox.Items.InnerList[10];
            ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);

            itemAccessibleObject.ScrollIntoView();

            int actual = unchecked((int)(long)User32.SendMessageW(comboBox, (User32.WM)User32.CB.GETTOPINDEX));

            Assert.Equal(0, actual); // ScrollIntoView didn't scroll to the tested item because the combobox is disabled
        }

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_ScrollIntoView_EnsureVisible_TestData()
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
        [MemberData(nameof(ComboBoxItemAccessibleObject_ScrollIntoView_EnsureVisible_TestData))]
        public void ComboBoxItemAccessibleObject_ScrollIntoView_EnsureVisible(ComboBoxStyle comboBoxStyle, bool scrollingDown, int itemIndex, int itemsCount)
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
            Entry itemEntry = comboBox.Items.InnerList[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);

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

        public static IEnumerable<object[]> ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_IfComboBoxIsScrollable_TestData()
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
        [MemberData(nameof(ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_IfComboBoxIsScrollable_TestData))]
        public void ComboBoxItemAccessibleObject_Bounds_ReturnsCorrect_IfComboBoxIsScrollable(ComboBoxStyle comboBoxStyle, int itemIndex, Point expectedPosition)
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
            Entry itemEntry = comboBox.Items.InnerList[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);
            Rectangle actual = itemAccessibleObject.Bounds;
            Rectangle dropdownRect = comboBox.ChildListAccessibleObject.Bounds;

            // We get items rectangles from Windows
            // It returns to us different expected widths values depending on a combobox drop-down style
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

                    // We get items rectangles from Windows
                    // It returns to us different expected bounds values depending on a combobox drop-down style
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
            Entry itemEntry = comboBox.Items.InnerList[itemIndex];
            ComboBoxItemAccessibleObject itemAccessibleObject = itemsCollection.GetComboBoxItemAccessibleObject(itemEntry);
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

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_MaxDropDownItems_State_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBoxWithMaxItems(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            Assert.Equal(comboBoxItem1.State, InvisibleItemState); // comboBoxItem1 above the visible area
            Assert.Equal(comboBoxItem2.State, VisibleItemState); // comboBoxItem2 in the visible area
            Assert.Equal(comboBoxItem3.State, InvisibleItemState); // comboBoxItem3 below the visible area
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_DropDownHeight_State_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBoxWithMaxHeight(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            Assert.Equal(comboBoxItem1.State, InvisibleItemState); // comboBoxItem1 above the visible area
            Assert.Equal(comboBoxItem2.State, VisibleItemState); // comboBoxItem2 in the visible area
            Assert.Equal(comboBoxItem3.State, InvisibleItemState); // comboBoxItem3 below the visible area
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_DropDownCollapsed_State_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBox(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            Assert.Equal(comboBoxItem1.State, InvisibleItemState);
            Assert.Equal(comboBoxItem2.State, InvisibleItemState);
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_State_ReturnExpected()
        {
            using ComboBox comboBox = GetComboBoxWithMaxSize(ComboBoxStyle.Simple);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            AccessibleStates itemState = AccessibleStates.Invisible | AccessibleStates.Selected | AccessibleStates.Focusable | AccessibleStates.Selectable;

            Assert.Equal(comboBoxItem1.State, InvisibleItemState); // comboBoxItem1 above the visible area
            Assert.Equal(comboBoxItem2.State, itemState); // comboBoxItem2 in the visible area
            Assert.Equal(comboBoxItem3.State, InvisibleItemState); // comboBoxItem3 below the visible area
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_MaxDropDownItems_GetOffScreenProperty_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBoxWithMaxItems(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            AssertExtensions.True(comboBoxItem1, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem1 above the visible area
            AssertExtensions.False(comboBoxItem2, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem2 in the visible area
            AssertExtensions.True(comboBoxItem3, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem3 below the visible area
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_DropDownHeight_GetOffScreenProperty_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBoxWithMaxHeight(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            AssertExtensions.True(comboBoxItem1, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem1 above the visible area
            AssertExtensions.False(comboBoxItem2, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem2 in the visible area
            AssertExtensions.True(comboBoxItem3, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem3 below the visible area
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void ComboBoxItemAccessibleObject_DropDownCollapsed_GetOffScreenProperty_ReturnExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = GetComboBox(comboBoxStyle);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            AssertExtensions.True(comboBoxItem1, UiaCore.UIA.IsOffscreenPropertyId);
            AssertExtensions.True(comboBoxItem2, UiaCore.UIA.IsOffscreenPropertyId);
        }

        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_GetOffScreenProperty_ReturnExpected()
        {
            using ComboBox comboBox = GetComboBoxWithMaxSize(ComboBoxStyle.Simple);

            ComboBox.ComboBoxItemAccessibleObject comboBoxItem1 = (ComboBox.ComboBoxItemAccessibleObject)comboBox
                    .ChildListAccessibleObject.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem2 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem1
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);
            ComboBox.ComboBoxItemAccessibleObject comboBoxItem3 = (ComboBox.ComboBoxItemAccessibleObject)comboBoxItem2
                    .FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling);

            AssertExtensions.True(comboBoxItem1, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem1 above the visible area
            AssertExtensions.False(comboBoxItem2, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem2 in the visible area
            AssertExtensions.True(comboBoxItem3, UiaCore.UIA.IsOffscreenPropertyId); // comboBoxItem3 below the visible area
        }

        private ComboBox GetComboBox(ComboBoxStyle comboBoxStyle)
        {
            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            comboBox.Items.Add(1);
            comboBox.Items.Add(2);
            comboBox.Items.Add(3);
            comboBox.CreateControl();
            return comboBox;
        }

        private ComboBox GetComboBoxWithMaxItems(ComboBoxStyle comboBoxStyle)
        {
            ComboBox comboBox = GetComboBox(comboBoxStyle);
            comboBox.IntegralHeight = false;
            comboBox.MaxDropDownItems = 1;
            comboBox.SelectedIndex = 1;
            comboBox.DroppedDown = true;
            return comboBox;
        }

        private ComboBox GetComboBoxWithMaxHeight(ComboBoxStyle comboBoxStyle)
        {
            ComboBox comboBox = GetComboBox(comboBoxStyle);
            comboBox.DropDownHeight = 10;
            comboBox.SelectedIndex = 1;
            comboBox.DroppedDown = true;
            return comboBox;
        }

        private ComboBox GetComboBoxWithMaxSize(ComboBoxStyle comboBoxStyle)
        {
            ComboBox comboBox = GetComboBox(comboBoxStyle);
            comboBox.Size = new Size(100, 50);
            comboBox.SelectedIndex = 1;
            return comboBox;
        }
    }
}
