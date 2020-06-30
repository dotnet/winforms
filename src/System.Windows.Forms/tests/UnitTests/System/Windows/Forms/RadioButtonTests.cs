﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Layout;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.UiaCore;
using static Interop.User32;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class RadioButtonTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void RadioButton_Ctor_Default()
        {
            using var control = new SubRadioButton();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(Appearance.Normal, control.Appearance);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.True(control.AutoCheck);
            Assert.False(control.AutoEllipsis);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(24, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(ContentAlignment.MiddleLeft, control.CheckAlign);
            Assert.False(control.Checked);
            Assert.Equal(new Size(104, 24), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(104, 24), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.NotNull(control.FlatAppearance);
            Assert.Same(control.FlatAppearance, control.FlatAppearance);
            Assert.Equal(FlatStyle.Standard, control.FlatStyle);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(24, control.Height);
            Assert.Null(control.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
            Assert.Equal(-1, control.ImageIndex);
            Assert.Empty(control.ImageKey);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.Disable, control.ImeMode);
            Assert.Equal(ImeMode.Disable, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsDefault);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(new Size(14, 13), control.PreferredSize);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(104, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(104, 24), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ContentAlignment.MiddleLeft, control.TextAlign);
            Assert.Equal(TextImageRelation.Overlay, control.TextImageRelation);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.UseCompatibleTextRendering);
            Assert.True(control.UseMnemonic);
            Assert.True(control.UseVisualStyleBackColor);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(104, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButton_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubRadioButton();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(24, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x5600000B, createParams.Style);
            Assert.Equal(104, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0x5600000B)]
        [InlineData(false, 0x5600000B)]
        public void RadioButton_CreateParams_GetUserPaint_ReturnsExpected(bool userPaint, int expectedStyle)
        {
            using var control = new SubRadioButton();
            control.SetStyle(ControlStyles.UserPaint, userPaint);

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(24, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(104, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RadioRadioButton_TabStop_Set_GetReturnsExpected(bool value)
        {
            using var control = new RadioButton
            {
                TabStop = value
            };
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RadioRadioButton_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new RadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RadioRadioButton_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            using var control = new RadioButton
            {
                TabStop = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabStopChanged += handler;

            // Set different.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabStop = true;
            Assert.True(control.TabStop);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> TextAlign_Set_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
                {
                    foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
                    {
                        yield return new object[] { autoSize, flatStyle, value };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TextAlign_Set_TestData))]
        public void RadioRadioButton_TextAlign_Set_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value)
        {
            using var control = new SubRadioButton
            {
                AutoSize = autoSize,
                FlatStyle = flatStyle
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> TextAlign_SetWithHandle_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                foreach (ContentAlignment value in Enum.GetValues(typeof(ContentAlignment)))
                {
                    int expectedCallCount = value == ContentAlignment.MiddleLeft ? 0 : 1;
                    yield return new object[] { autoSize, FlatStyle.Flat, value, expectedCallCount, 0 };
                    yield return new object[] { autoSize, FlatStyle.Popup, value, expectedCallCount, 0 };
                    yield return new object[] { autoSize, FlatStyle.Standard, value, expectedCallCount, 0 };
                    yield return new object[] { autoSize, FlatStyle.System, value, expectedCallCount, expectedCallCount };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TextAlign_SetWithHandle_TestData))]
        public void RadioRadioButton_TextAlign_SetWithHandle_GetReturnsExpected(bool autoSize, FlatStyle flatStyle, ContentAlignment value, int expectedInvalidatedCallCount, int expectedStyleChangedCallCount)
        {
            using var control = new SubRadioButton
            {
                AutoSize = autoSize,
                FlatStyle = flatStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TextAlign = value;
            Assert.Equal(value, control.TextAlign);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        public void RadioRadioButton_TextAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var control = new SubRadioButton();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextAlign = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void RadioButton_CreateAccessibilityInstance_Invoke_ReturnsExpected(FlatStyle flatStyle)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            RadioButton.RadioButtonAccessibleObject instance = Assert.IsType<RadioButton.RadioButtonAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.NotEmpty(instance.DefaultAction);
            Assert.Equal(AccessibleStates.Focusable, instance.State);
            Assert.Equal(AccessibleRole.RadioButton, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void RadioButton_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected(FlatStyle flatStyle)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle,
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            RadioButton.RadioButtonAccessibleObject instance = Assert.IsType<RadioButton.RadioButtonAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.NotEmpty(instance.DefaultAction);
            Assert.Equal(AccessibleStates.Focusable, instance.State);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsTheory]
        [InlineData(FlatStyle.Flat, "")]
        [InlineData(FlatStyle.Popup, "")]
        [InlineData(FlatStyle.Standard, "")]
        [InlineData(FlatStyle.System, "")]
        [InlineData(FlatStyle.Flat, "Description")]
        [InlineData(FlatStyle.Popup, "Description")]
        [InlineData(FlatStyle.Standard, "Description")]
        [InlineData(FlatStyle.System, "Description")]
        public void RadioButton_CreateAccessibilityInstance_InvokeWithCustomDefaultActionDescription_ReturnsExpected(FlatStyle flatStyle, string defaultActionDescription)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle,
                AccessibleDefaultActionDescription = defaultActionDescription
            };
            RadioButton.RadioButtonAccessibleObject instance = Assert.IsType<RadioButton.RadioButtonAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(defaultActionDescription, instance.DefaultAction);
            Assert.Equal(AccessibleStates.Focusable, instance.State);
            Assert.Equal(AccessibleRole.RadioButton, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void RadioButton_CreateAccessibilityInstance_InvokeChecked_ReturnsExpected(FlatStyle flatStyle)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle,
                Checked = true
            };
            RadioButton.RadioButtonAccessibleObject instance = Assert.IsType<RadioButton.RadioButtonAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.NotEmpty(instance.DefaultAction);
            Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Checked, instance.State);
            Assert.Equal(AccessibleRole.RadioButton, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlatStyle))]
        public void RadioButton_CreateAccessibilityInstance_InvokeDoDefaultAction_CallsOnClick(FlatStyle flatStyle)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            control.Click += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            RadioButton.RadioButtonAccessibleObject instance = Assert.IsType<RadioButton.RadioButtonAccessibleObject>(control.CreateAccessibilityInstance());
            instance.DoDefaultAction();
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void RadioButton_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubRadioButton();
            Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, true)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, true)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, true)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void RadioButton_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubRadioButton();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void RadioButton_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubRadioButton();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnCheckedChanged_Invoke_CallsCheckedChanged(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CheckedChanged += handler;
            control.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.CheckedChanged -= handler;
            control.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnCheckedChanged_InvokeWithHandle_CallsCheckedChanged(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CheckedChanged += handler;
            control.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.CheckedChanged -= handler;
            control.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnClick_TestData()
        {
            foreach (bool autoCheck in new bool[] { true, false })
            {
                yield return new object[] { autoCheck, null };
                yield return new object[] { autoCheck, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnClick_TestData))]
        public void RadioButton_OnClick_Invoke_CallsClick(bool autoCheck, EventArgs eventArgs)
        {
            using var control = new SubRadioButton
            {
                AutoCheck = autoCheck
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(autoCheck, control.Checked);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(autoCheck, control.Checked);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.Equal(autoCheck, control.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(autoCheck, control.Checked);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.Equal(autoCheck, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnClick_WithHandle_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnClick_WithHandle_TestData))]
        public void RadioButton_OnClick_InvokeWithHandle_CallsClick(bool autoCheck, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubRadioButton
            {
                AutoCheck = autoCheck
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.Equal(autoCheck, control.Checked);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(autoCheck, control.Checked);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(autoCheck, control.Checked);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DoubleClick += handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.DoubleClick -= handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnEnter_InvokeWithHandle_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.IsHandleCreated);
            Assert.True(invalidatedCallCount >= 0);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.IsHandleCreated);
            Assert.True(invalidatedCallCount >= 0);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RadioButton_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void RadioButton_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDoubleClick += handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseDoubleClick -= handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseUp_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_TestData))]
        public void RadioButton_OnMouseUp_Invoke_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseUp_MouseDown_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                bool expectedIsHandleCreated = flatStyle != FlatStyle.System;
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), expectedIsHandleCreated };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4), expectedIsHandleCreated };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4), false };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4), false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_MouseDown_TestData))]
        public void RadioButton_OnMouseUp_InvokeMouseDown_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs, bool expectedIsHandleCreated)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_TestData))]
        public void RadioButton_OnMouseUp_InvokeWithHandle_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnMouseUp_WithHandle_TestData()
        {
            foreach (FlatStyle flatStyle in Enum.GetValues(typeof(FlatStyle)))
            {
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new MouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.None, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Middle, 1, 2, 3, 4) };
                yield return new object[] { flatStyle, new HandledMouseEventArgs(MouseButtons.Right, 1, 2, 3, 4) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseUp_WithHandle_TestData))]
        public void RadioButton_OnMouseUp_InvokeMouseDownWithHandle_CallsMouseUp(FlatStyle flatStyle, MouseEventArgs eventArgs)
        {
            using var control = new SubRadioButton
            {
                FlatStyle = flatStyle
            };
            control.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            int mouseClickCallCount = 0;
            control.MouseClick += (sender, e) => mouseClickCallCount++;

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, mouseClickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RadioButton_OnMouseUp_NullE_ThrowsNullReferenceException()
        {
            using var control = new SubRadioButton();
            Assert.Throws<NullReferenceException>(() => control.OnMouseUp(null));
        }

        [WinFormsFact]
        public void RadioButton_PerformClick_Invoke_CallsClick()
        {
            using var control = new SubRadioButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButton_PerformClick_InvokeWithHandle_CallsClick()
        {
            using var control = new SubRadioButton();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Click -= handler;
            control.PerformClick();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a', false)]
        [InlineData(true, "", char.MinValue, false)]
        [InlineData(true, "&a", 'a', true)]
        [InlineData(true, "&a", 'b', false)]
        [InlineData(true, "&&a", 'a', false)]
        [InlineData(true, "&", 'a', false)]
        [InlineData(true, "text", 'a', false)]
        [InlineData(false, "", 'a', false)]
        [InlineData(false, "", char.MinValue, false)]
        [InlineData(false, "&a", 'a', false)]
        [InlineData(false, "&a", 'b', false)]
        [InlineData(false, "&&a", 'a', false)]
        [InlineData(false, "&", 'a', false)]
        [InlineData(false, "text", 'a', false)]
        public void RadioButton_ProcessMnemonic_Invoke_ReturnsExpected(bool useMnemonic, string text, char charCode, bool expected)
        {
            using var control = new SubRadioButton
            {
                UseMnemonic = useMnemonic,
                Text = text
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            Assert.Equal(expected, control.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a')]
        [InlineData(true, "", char.MinValue)]
        [InlineData(true, "&a", 'a')]
        [InlineData(true, "&a", 'b')]
        [InlineData(true, "&&a", 'a')]
        [InlineData(true, "&", 'a')]
        [InlineData(true, "text", 'a')]
        [InlineData(false, "", 'a')]
        [InlineData(false, "", char.MinValue)]
        [InlineData(false, "&a", 'a')]
        [InlineData(false, "&a", 'b')]
        [InlineData(false, "&&a", 'a')]
        [InlineData(false, "&", 'a')]
        [InlineData(false, "text", 'a')]
        public void RadioButton_ProcessMnemonic_InvokeCantProcessMnemonic_ReturnsFalse(bool useMnemonic, string text, char charCode)
        {
            using var control = new SubRadioButton
            {
                UseMnemonic = useMnemonic,
                Text = text,
                Enabled = false
            };
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            Assert.False(control.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a', false)]
        [InlineData(true, "", char.MinValue, false)]
        [InlineData(true, "&a", 'a', true)]
        [InlineData(true, "&a", 'b', false)]
        [InlineData(true, "&&a", 'a', false)]
        [InlineData(true, "&", 'a', false)]
        [InlineData(true, "text", 'a', false)]
        [InlineData(false, "", 'a', false)]
        [InlineData(false, "", char.MinValue, false)]
        [InlineData(false, "&a", 'a', false)]
        [InlineData(false, "&a", 'b', false)]
        [InlineData(false, "&&a", 'a', false)]
        [InlineData(false, "&", 'a', false)]
        [InlineData(false, "text", 'a', false)]
        public void RadioButton_ProcessMnemonic_InvokeWithHandle_ReturnsExpected(bool useMnemonic, string text, char charCode, bool expected)
        {
            using var control = new SubRadioButton
            {
                UseMnemonic = useMnemonic,
                Text = text
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;

            Assert.Equal(expected, control.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, "", 'a')]
        [InlineData(true, "", char.MinValue)]
        [InlineData(true, "&a", 'a')]
        [InlineData(true, "&a", 'b')]
        [InlineData(true, "&&a", 'a')]
        [InlineData(true, "&", 'a')]
        [InlineData(true, "text", 'a')]
        [InlineData(false, "", 'a')]
        [InlineData(false, "", char.MinValue)]
        [InlineData(false, "&a", 'a')]
        [InlineData(false, "&a", 'b')]
        [InlineData(false, "&&a", 'a')]
        [InlineData(false, "&", 'a')]
        [InlineData(false, "text", 'a')]
        public void RadioButton_ProcessMnemonic_InvokeCantProcessMnemonicWithHandle_ReturnsFalse(bool useMnemonic, string text, char charCode)
        {
            using var control = new SubRadioButton
            {
                UseMnemonic = useMnemonic,
                Text = text,
                Enabled = false
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int clickCallCount = 0;
            control.Click += (sender, e) => clickCallCount++;
            Assert.False(control.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RadioButton_RaiseAutomationEvent_Invoke_Success()
        {
            using var radioButton = new TestRadioButton();
            Assert.False(radioButton.IsHandleCreated);

            var accessibleObject = (SubRadioButtonAccessibleObject)radioButton.AccessibilityObject;
            Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);

            radioButton.PerformClick();

            Assert.Equal(1, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(1, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(radioButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-1, -2)]
        public void RadioButton_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
        {
            using var control = new SubRadioButton();
            control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RadioButton_ToString_Invoke_ReturnsExpected()
        {
            using var control = new RadioButton();
            Assert.Equal("System.Windows.Forms.RadioButton, Checked: False", control.ToString());
        }

        [WinFormsFact]
        public void RadioButton_ToString_InvokeShortText_ReturnsExpected()
        {
            using var control = new RadioButton
            {
                Text = "Text"
            };
            Assert.Equal("System.Windows.Forms.RadioButton, Checked: False", control.ToString());
        }

        private class SubRadioButton : RadioButton
        {
            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool IsDefault
            {
                get => base.IsDefault;
                set => base.IsDefault = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnCheckedChanged(EventArgs e) => base.OnCheckedChanged(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(MouseEventArgs eventargs) => base.OnMouseDown(eventargs);

            public new void OnMouseUp(MouseEventArgs eventargs) => base.OnMouseUp(eventargs);

            public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

            public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }

        private class TestRadioButton : RadioButton
        {
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new SubRadioButtonAccessibleObject(this);
            }
        }

        private class SubRadioButtonAccessibleObject : RadioButton.RadioButtonAccessibleObject
        {
            public SubRadioButtonAccessibleObject(RadioButton owner) : base(owner)
            {
                RaiseAutomationEventCallsCount = 0;
                RaiseAutomationPropertyChangedEventCallsCount = 0;
            }

            public int RaiseAutomationEventCallsCount { get; private set; }

            public int RaiseAutomationPropertyChangedEventCallsCount { get; private set; }

            internal override bool RaiseAutomationEvent(UIA eventId)
            {
                RaiseAutomationEventCallsCount++;
                return base.RaiseAutomationEvent(eventId);
            }

            internal override bool RaiseAutomationPropertyChangedEvent(UIA propertyId, object oldValue, object newValue)
            {
                RaiseAutomationPropertyChangedEventCallsCount++;
                return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
            }
        }
    }
}
