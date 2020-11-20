// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
    }
}
