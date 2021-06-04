// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxChildDropDownButtonUiaProviderTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> DropDownButtonUiaProvider_FragmentNavigate_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                // ComboBox with "ComboBoxStyle.Simple" DropDownStyle has no DropDownButtonUiaProvider
                if (comboBoxStyle == ComboBoxStyle.Simple)
                {
                    continue;
                }

                foreach (bool createControl in new[] { true, false })
                {
                    foreach (bool droppedDown in new[] { true, false })
                    {
                        yield return new object[] { comboBoxStyle, createControl, droppedDown };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DropDownButtonUiaProvider_FragmentNavigate_TestData))]
        public void DropDownButtonUiaProvider_FragmentNavigate_NextSibling_ReturnsExpected(
            ComboBoxStyle comboBoxStyle,
            bool createControl,
            bool droppedDown)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.DroppedDown = droppedDown;
            AccessibleObject nextItem = GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider
                .FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

            Assert.Null(nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DropDownButtonUiaProvider_FragmentNavigate_TestData))]
        public void DropDownButtonUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected(
            ComboBoxStyle comboBoxStyle,
            bool createControl,
            bool droppedDown)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            if (createControl)
            {
                comboBox.CreateControl();
            }

            comboBox.DroppedDown = droppedDown;
            AccessibleObject previousItem = GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider
                .FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

            AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.DropDownList
                ? comboBox.ChildTextAccessibleObject
                : comboBox.ChildEditAccessibleObject;

            Assert.Equal(expectedItem, previousItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.Simple)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void DropDownButtonUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected_IfHandleIsNotCreated(
            ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            AccessibleObject previousItem = GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider
                .FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

            Assert.Null(previousItem);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.DropDown)]
        public void DropDownButtonUiaProvider_Name_ReturnsExpected(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            Assert.Equal(SR.ComboboxDropDownButtonOpenName, GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider.Name);

            // Open the dropdown list
            comboBox.DroppedDown = true;

            Assert.Equal(SR.ComboboxDropDownButtonCloseName, GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider.Name);
        }

        private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
        {
            return comboBox.AccessibilityObject as ComboBox.ComboBoxAccessibleObject;
        }
    }
}
