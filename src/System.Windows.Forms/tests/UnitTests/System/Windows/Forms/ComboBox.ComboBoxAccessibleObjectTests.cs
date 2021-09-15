// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            using ComboBox control = new ComboBox();
            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);
            ComboBox.ComboBoxAccessibleObject accessibleObject = new ComboBox.ComboBoxAccessibleObject(control);
            Assert.Equal(createControl, control.IsHandleCreated);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState_IfControlIsCreated(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox control = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };
            control.CreateControl();
            AccessibleObject accessibleObject = control.AccessibilityObject;

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
            using ComboBox control = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            AccessibleObject accessibleObject = control.AccessibilityObject;
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

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnNull_IfHandleNotCreated(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            AccessibleObject accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Null(firstChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test text")]
        [InlineData(null)]
        public void ComboBoxEditAccessibleObject_NameNotNull(string name)
        {
            using ComboBox control = new ComboBox();
            control.AccessibleName = name;
            control.CreateControl(false);
            object editAccessibleName = control.ChildEditAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.NotNull(editAccessibleName);
        }

        [WinFormsFact]
        public void ComboBoxEditAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            const string name = "Test text";
            using ComboBox comboBox = new();
            comboBox.AccessibleName = name;
            comboBox.CreateControl(false);
            object actual = comboBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            Assert.Equal(name, actual);
            Assert.True(comboBox.IsHandleCreated);
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
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(
            ComboBoxStyle comboBoxStyle,
            bool createControl,
            bool droppedDown,
            bool childListDisplayed)
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
            AccessibleObject expectedLastChild = comboBoxStyle == ComboBoxStyle.Simple
                ? comboBox.ChildEditAccessibleObject
                : GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider;

            Assert.NotNull(lastChild);
            Assert.Equal(expectedLastChild, lastChild);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown)]
        [InlineData(ComboBoxStyle.DropDownList)]
        [InlineData(ComboBoxStyle.Simple)]
        public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnNull_IfHandleNotCreated(ComboBoxStyle comboBoxStyle)
        {
            using ComboBox comboBox = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            AccessibleObject accessibleObject = comboBox.AccessibilityObject;
            UiaCore.IRawElementProviderFragment lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.Null(lastChild);
            Assert.False(comboBox.IsHandleCreated);
        }

        private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
        {
            return comboBox.AccessibilityObject as ComboBox.ComboBoxAccessibleObject;
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
        {
            using ComboBox control = new ComboBox();
            // AccessibleRole is not set = Default

            object actual = control.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ComboBoxControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ComboBox comboBox = new ComboBox();
            comboBox.AccessibleRole = role;

            object actual = comboBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxEditAccessibleObject_GetPropertyValue_NativeWindowHandle_ReturnsExpected()
        {
            using ComboBox comboBox = new();
            comboBox.CreateControl(false);
            object actual = comboBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.NativeWindowHandlePropertyId);

            Assert.True(comboBox.IsHandleCreated);
            Assert.Equal(comboBox.InternalHandle, actual);
        }

        [WinFormsTheory]
        [InlineData(true, ((int)UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UiaCore.UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(true, ((int)UiaCore.UIA.IsValuePatternAvailablePropertyId))]
        public void ComboBoxAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using ComboBox comboBox = new();
            comboBox.CreateControl(false);
            ComboBox.ComboBoxAccessibleObject accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UiaCore.UIA)propertyId) ?? false);
            Assert.True(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_DefaultAction_IfAccessibleDefaultActionDescriptionIsNotNull_ReturnsAccessibleDefaultActionDescription()
        {
            using ComboBox comboBox = new() { AccessibleDefaultActionDescription = "Test" };

            Assert.Equal(comboBox.AccessibleDefaultActionDescription, comboBox.AccessibilityObject.DefaultAction);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsNotCreated_ReturnsNull()
        {
            using ComboBox comboBox = new();

            Assert.Empty(comboBox.AccessibilityObject.DefaultAction);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreatedAndDropDownStyleIsSimple_ReturnsNull()
        {
            using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.Simple };
            comboBox.CreateControl();

            Assert.True(comboBox.IsHandleCreated);
            Assert.Empty(comboBox.AccessibilityObject.DefaultAction);
        }

        public static IEnumerable<object[]> ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected_TestData()
        {
            yield return new object[] { ComboBoxStyle.DropDown, false, SR.AccessibleActionExpand };
            yield return new object[] { ComboBoxStyle.DropDown, true, SR.AccessibleActionCollapse };
            yield return new object[] { ComboBoxStyle.DropDownList, false, SR.AccessibleActionExpand };
            yield return new object[] { ComboBoxStyle.DropDownList, true, SR.AccessibleActionCollapse };
        }

        [WinFormsTheory]
        [MemberData(nameof(ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected_TestData))]
        public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected(ComboBoxStyle style, bool droppedDown, string expectedAction)
        {
            using ComboBox comboBox = new() { DropDownStyle = style };
            comboBox.CreateControl();
            comboBox.DroppedDown = droppedDown;

            Assert.True(comboBox.IsHandleCreated);
            Assert.Equal(expectedAction, comboBox.AccessibilityObject.DefaultAction);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsNotCreated_DoesNotExpand()
        {
            using ComboBox comboBox = new();

            Assert.False(comboBox.DroppedDown);

            comboBox.AccessibilityObject.DoDefaultAction();

            Assert.False(comboBox.DroppedDown);
            Assert.False(comboBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsCreatedAndDropDownStyleIsSimple_DoesNotCollapse()
        {
            using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.Simple };
            comboBox.CreateControl();

            Assert.True(comboBox.IsHandleCreated);
            Assert.True(comboBox.DroppedDown);

            comboBox.AccessibilityObject.DoDefaultAction();

            Assert.True(comboBox.DroppedDown);
        }

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, false, true)]
        [InlineData(ComboBoxStyle.DropDown, true, false)]
        [InlineData(ComboBoxStyle.DropDownList, false, true)]
        [InlineData(ComboBoxStyle.DropDownList, true, false)]
        public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsCreated_DoesExpected(ComboBoxStyle style, bool droppedDown, bool expectedDroppedDown)
        {
            using ComboBox comboBox = new() { DropDownStyle = style };
            comboBox.CreateControl();
            comboBox.DroppedDown = droppedDown;

            Assert.True(comboBox.IsHandleCreated);

            comboBox.AccessibilityObject.DoDefaultAction();

            Assert.Equal(expectedDroppedDown, comboBox.DroppedDown);
        }
    }
}
