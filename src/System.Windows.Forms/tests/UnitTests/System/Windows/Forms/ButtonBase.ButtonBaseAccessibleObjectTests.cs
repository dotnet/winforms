// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ButtonBase_ButtonBaseAccessibleObjectTests
    {
        [WinFormsFact]
        public void ButtonBaseAccessibleObject_Ctor_NullControl_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ButtonBase.ButtonBaseAccessibleObject(null));
        }

        [WinFormsFact]
        public void ButtonBaseAccessibleObject_Ctor_InvalidTypeControl_ThrowsArgumentException()
        {
            using var textBox = new TextBox();
            Assert.Throws<ArgumentException>(() => new ButtonBase.ButtonBaseAccessibleObject(textBox));
        }

        [WinFormsTheory]
        [InlineData(FlatStyle.Flat, true, true, AccessibleStates.Focusable | AccessibleStates.Pressed)]
        [InlineData(FlatStyle.Flat, false, true, AccessibleStates.None)]
        [InlineData(FlatStyle.Flat, true, false, AccessibleStates.Focusable)]
        [InlineData(FlatStyle.Flat, false, false, AccessibleStates.None)]
        [InlineData(FlatStyle.Popup, true, true, AccessibleStates.Focusable | AccessibleStates.Pressed)]
        [InlineData(FlatStyle.Popup, false, true, AccessibleStates.None)]
        [InlineData(FlatStyle.Popup, true, false, AccessibleStates.Focusable)]
        [InlineData(FlatStyle.Popup, false, false, AccessibleStates.None)]
        [InlineData(FlatStyle.Standard, true, true, AccessibleStates.Focusable | AccessibleStates.Pressed)]
        [InlineData(FlatStyle.Standard, false, true, AccessibleStates.None)]
        [InlineData(FlatStyle.Standard, true, false, AccessibleStates.Focusable)]
        [InlineData(FlatStyle.Standard, false, false, AccessibleStates.None)]
        [InlineData(FlatStyle.System, true, true, AccessibleStates.Focusable)]
        [InlineData(FlatStyle.System, false, true, AccessibleStates.None)]
        [InlineData(FlatStyle.System, true, false, AccessibleStates.Focusable)]
        [InlineData(FlatStyle.System, false, false, AccessibleStates.None)]
        public void ButtonBaseAccessibleObject_State_is_correct(FlatStyle flatStyle, bool createControl, bool mouseIsDown, AccessibleStates expectedAccessibleState)
        {
            using var button = new SubButtonBase()
            {
                FlatStyle = flatStyle
            };

            if (createControl)
            {
                button.CreateControl();
            }

            Assert.Equal(createControl, button.IsHandleCreated);

            if (mouseIsDown)
            {
                button.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }

            var buttonBaseAccessibleObject = new ButtonBase.ButtonBaseAccessibleObject(button);

            Assert.Equal(expectedAccessibleState, buttonBaseAccessibleObject.State);
            Assert.Equal(createControl, button.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, AccessibleRole.Client)]
        [InlineData(true, false, AccessibleRole.HelpBalloon)]
        [InlineData(false, true, AccessibleRole.None)]
        [InlineData(false, false, AccessibleRole.HelpBalloon)]
        public void ButtonBase_CreateAccessibilityInstance_InvokeWithRole_ReturnsExpected(bool createControl, bool defaultRole, AccessibleRole expectedAccessibleRole)
        {
            using var control = new SubButtonBase();

            if (!defaultRole)
            {
                control.AccessibleRole = AccessibleRole.HelpBalloon;
            }

            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);

            ButtonBase.ButtonBaseAccessibleObject instance = Assert.IsType<ButtonBase.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());

            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(expectedAccessibleRole, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void ButtonBase_CreateAccessibilityInstance_InvokeWithDefaultRole_ReturnsExpected_ForAllFlatStyles_IfControlIsCreated(FlatStyle flatStyle)
        {
            using var control = new SubButtonBase()
            {
                FlatStyle = flatStyle
            };
            control.CreateControl();
            ButtonBase.ButtonBaseAccessibleObject instance = Assert.IsType<ButtonBase.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.Client, instance.Role);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void ButtonBase_CreateAccessibilityInstance_InvokeWithDefaultRole_ReturnsNone_ForAllFlatStyles_IfControlIsNotCreated(FlatStyle flatStyle)
        {
            using var control = new SubButtonBase()
            {
                FlatStyle = flatStyle
            };

            Assert.False(control.IsHandleCreated);

            ButtonBase.ButtonBaseAccessibleObject instance = Assert.IsType<ButtonBase.ButtonBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.None, instance.Role);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ButtonBase_CreateAccessibilityInstance_InvokeDoDefaultAction_CallsOnClick(bool createControl)
        {
            using var control = new SubButtonBase();

            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);

            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            var buttonBaseAccessibleObject = new ButtonBase.ButtonBaseAccessibleObject(control);
            buttonBaseAccessibleObject.DoDefaultAction();

            Assert.Equal(createControl ? 1 : 0, callCount);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ButtonBase_CreateAccessibilityInstance_InvokeIButtonControlDoDefaultAction_CallsOnClick(bool createControl, int expectedCallCount)
        {
            using var control = new SubButtonBase();

            if (createControl)
            {
                control.CreateControl();
            }

            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            int performClickCallCount = 0;
            control.PerformClickAction = () => performClickCallCount++;
            var buttonBaseAccessibleObject = new ButtonBase.ButtonBaseAccessibleObject(control);

            buttonBaseAccessibleObject.DoDefaultAction();

            Assert.Equal(expectedCallCount, callCount);
            Assert.Equal(0, performClickCallCount);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        private class SubButtonBase : ButtonBase
        {
            public Action PerformClickAction { get; set; }

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public void PerformClick() => PerformClickAction();
        }
    }
}
