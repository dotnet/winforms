// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxChildListUiaProviderTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> ChildListAccessibleObject_FragmentNavigate_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
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
        [MemberData(nameof(ChildListAccessibleObject_FragmentNavigate_TestData))]
        public void ChildListAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(
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
            AccessibleObject previousItem = comboBox.ChildListAccessibleObject
                .FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

            AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.Simple
                ? comboBox.ChildListAccessibleObject
                : null;

            Assert.Equal(expectedItem, previousItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ChildListAccessibleObject_FragmentNavigate_TestData))]
        public void ChildListAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(
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
            AccessibleObject nextItem = comboBox.ChildListAccessibleObject
                .FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

            AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.DropDownList
                ? comboBox.ChildTextAccessibleObject
                : comboBox.ChildEditAccessibleObject;

            Assert.Equal(expectedItem, nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                yield return new object[] { comboBoxStyle };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable_TestData))]
        public void ChildListAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfComboBoxIsScrollable(ComboBoxStyle comboBoxStyle)
        {
            const int expectedWidth = 100;

            using ComboBox comboBox = new()
            {
                DropDownStyle = comboBoxStyle,
                IntegralHeight = false
            };
            comboBox.Items.AddRange(Enumerable.Range(0, 11).Cast<object>().ToArray());
            comboBox.CreateControl();

            if (comboBoxStyle == ComboBoxStyle.Simple)
            {
                comboBox.Size = new Size(expectedWidth, 150);
            }
            else
            {
                comboBox.Size = new Size(expectedWidth, comboBox.Size.Height);
                comboBox.DropDownHeight = 120;
                comboBox.DroppedDown = true;
            }

            UiaCore.IRawElementProviderFragment childListUiaProvider = comboBox.ChildListAccessibleObject;
            UiaCore.UiaRect actual = childListUiaProvider.BoundingRectangle;

            Assert.Equal(expectedWidth, actual.width);
        }
    }
}
