// Licensed to the .NET Foundation under one or more agreements.
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
    public class ComboBox_ComboBoxItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void ComboBoxItemAccessibleObject_Get_Not_ThrowsException()
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
                var item1AccessibleObject= comboBoxAccessibleObject.ItemAccessibleObjects[item1];
                var item2AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item2];
                var item3AccessibleObject = comboBoxAccessibleObject.ItemAccessibleObjects[item3];
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.False(exceptionThrown, "Getting accessible object for ComboBox item has thrown an exception.");
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
            // Regression test for https://github.com/dotnet/winforms/issues/3584

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

                Assert.True(itemAccessibleObject.IsPatternSupported(NativeMethods.UIA_ScrollItemPatternId));
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

                Assert.True((bool)itemAccessibleObject.GetPropertyValue(NativeMethods.UIA_IsScrollItemPatternAvailablePropertyId));
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
                comboBox.Size = new Drawing.Size(100, 132);
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

            int actual = unchecked((int)(long)User32.SendMessageW(comboBox, NativeMethods.CB_GETTOPINDEX));

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
                comboBox.Size = new Drawing.Size(100, 132);
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
                User32.SendMessageW(comboBox, NativeMethods.CB_SETTOPINDEX, (IntPtr)(itemsCount - 1));

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

            int actual = unchecked((int)(long)User32.SendMessageW(comboBox, NativeMethods.CB_GETTOPINDEX));

            Assert.Equal(expected, actual);
        }
    }
}
