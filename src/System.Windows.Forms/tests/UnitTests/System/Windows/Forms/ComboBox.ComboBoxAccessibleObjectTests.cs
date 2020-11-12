// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ComboBox)]
        [InlineData(false, AccessibleRole.None)]
        public void ComboBoxAccessibleObject_Ctor_Default(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using var control = new ComboBox();
            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);
            var accessibleObject = new ComboBox.ComboBoxAccessibleObject(control);
            Assert.Equal(createControl, control.IsHandleCreated);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState_IfControlIsCreated(ComboBoxStyle comboBoxStyle)
        {
            using var control = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };
            control.CreateControl();
            var accessibleObject = control.AccessibilityObject;

            accessibleObject.Expand();
            Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.Equal(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

            accessibleObject.Collapse();
            Assert.Equal(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState_IfControlIsNotCreated(ComboBoxStyle comboBoxStyle)
        {
            using var control = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };
            var accessibleObject = control.AccessibilityObject;
            Assert.False(control.IsHandleCreated);

            accessibleObject.Expand();
            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

            accessibleObject.Collapse();
            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_DropDown_ReturnExpected_IfHandleNotCreated()
        {
            using var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown
            };

            var accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Equal(GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider, firstChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_DropDownList_ReturnExpected_IfHandleNotCreated()
        {
            using var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Equal(comboBox.ChildTextAccessibleObject, firstChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_Simple_ReturnExpected_IfHandleNotCreated()
        {
            using var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.Simple
            };

            var accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Null(firstChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test text")]
        [InlineData(null)]
        public void ComboBoxEditAccessibleObject_NameNotNull(string name)
        {
            using var control = new ComboBox();
            control.AccessibleName = name;
            control.CreateControl(false);
            object editAccessibleName = control.ChildEditAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.NotNull(editAccessibleName);
        }

        public static IEnumerable<object[]> ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_TestData()
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
        [MemberData(nameof(ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_TestData))]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown, bool childListDisplayed)
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
            AccessibleObject firstChild = comboBox.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;

            Assert.NotNull(firstChild);
            Assert.Equal(childListDisplayed, firstChild == comboBox.ChildListAccessibleObject);
            Assert.True(comboBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_TestData()
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
        [MemberData(nameof(ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_TestData))]
        public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
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
            AccessibleObject lastChild = comboBox.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            AccessibleObject expectedLastChild = comboBoxStyle == ComboBoxStyle.Simple ? comboBox.ChildEditAccessibleObject : GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider;

            Assert.NotNull(lastChild);
            Assert.Equal(expectedLastChild, lastChild);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnExpected_IfHandleNotCreated(ComboBoxStyle comboBoxStyle)
        {
            using var comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            var accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.Equal(GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider, lastChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_Simple_ReturnExpected_IfHandleNotCreated()
        {
            using var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.Simple
            };

            var accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Null(firstChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
        {
            return comboBox.AccessibilityObject as ComboBox.ComboBoxAccessibleObject;
        }
    }
}
