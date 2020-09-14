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

        [WinFormsTheory]
        [InlineData(ComboBoxStyle.DropDown, true)]
        [InlineData(ComboBoxStyle.DropDownList, true)]
        [InlineData(ComboBoxStyle.DropDown, false)]
        [InlineData(ComboBoxStyle.DropDownList, false)]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_NotNull(ComboBoxStyle comboBoxStyle, bool createControl)
        {
            using var control = new ComboBox
            {
                DropDownStyle = comboBoxStyle
            };

            if (createControl)
            {
                control.CreateControl();
            }

            var accessibleObject = control.AccessibilityObject;

            UiaCore.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(createControl, control.IsHandleCreated);
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
    }
}
