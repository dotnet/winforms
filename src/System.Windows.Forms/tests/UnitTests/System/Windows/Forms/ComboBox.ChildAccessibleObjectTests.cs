// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ChildAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> ChildTextEditAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_TestData()
        {
            foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
            {
                foreach (bool createControl in new[] { true, false })
                {
                    foreach (bool droppedDown in new[] { true, false })
                    {
                        bool childListDisplayed = droppedDown || comboBoxStyle == ComboBoxStyle.Simple;
                        yield return new object[] { comboBoxStyle, createControl, droppedDown, childListDisplayed };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ChildTextEditAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_TestData))]
        public void ChildTextEditAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown, bool childListDisplayed)
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
            AccessibleObject previousItem = GetTextEditAccessibleObject(comboBox).FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

            Assert.Equal(!childListDisplayed, previousItem is null);
            Assert.Equal(childListDisplayed, previousItem == comboBox.ChildListAccessibleObject);
            Assert.True(comboBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> ComboBoxChildAccessibleObject_FragmentNavigate_TestData()
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
        [MemberData(nameof(ComboBoxChildAccessibleObject_FragmentNavigate_TestData))]
        public void ChildTextEditAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject nextItem = GetTextEditAccessibleObject(comboBox).FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;
            AccessibleObject expectedItem = comboBoxStyle != ComboBoxStyle.Simple ? GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider : null;

            Assert.Equal(expectedItem, nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxChildAccessibleObject_FragmentNavigate_TestData))]
        public void ChildListAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject previousItem = comboBox.ChildListAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;
            AccessibleObject expectedItem = comboBoxStyle == ComboBoxStyle.Simple ? comboBox.ChildListAccessibleObject : null;

            Assert.Equal(expectedItem, previousItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxChildAccessibleObject_FragmentNavigate_TestData))]
        public void ChildListAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject nextItem = comboBox.ChildListAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

            Assert.Equal(GetTextEditAccessibleObject(comboBox), nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

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
        public void DropDownButtonUiaProvider_FragmentNavigate_NextSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject nextItem = GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider.FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

            Assert.Null(nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DropDownButtonUiaProvider_FragmentNavigate_TestData))]
        public void DropDownButtonUiaProvider_FragmentNavigate_PreviousSibling_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject nextItem = GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling) as AccessibleObject;

            Assert.Equal(GetTextEditAccessibleObject(comboBox), nextItem);
            Assert.True(comboBox.IsHandleCreated);
        }

        private AccessibleObject GetTextEditAccessibleObject(ComboBox comboBox)
        {
            return comboBox.DropDownStyle == ComboBoxStyle.DropDownList
                ? comboBox.ChildTextAccessibleObject
                : comboBox.ChildEditAccessibleObject;
        }

        private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
        {
            return comboBox.AccessibilityObject as ComboBox.ComboBoxAccessibleObject;
        }
    }
}
