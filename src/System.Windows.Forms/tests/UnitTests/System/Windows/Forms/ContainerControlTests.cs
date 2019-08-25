// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ContainerControlTests
    {
        [Fact]
        public void ContainerControl_Ctor_Default()
        {
            var control = new SubContainerControl();
            Assert.Null(control.ActiveControl);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
            Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.NotNull(control.BindingContext);
            Assert.Same(control.BindingContext, control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanProcessMnemonic());
            Assert.True(control.CausesValidation);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Null(control.Container);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(0, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(0, control.Width);
        }

        [Fact]
        public void ContainerControl_ActiveContainerContainerControl_Set_GetReturnsExpected()
        {
            var control = new ContainerControl();
            var child = new Control();
            var grandchild = new Control();
            control.Controls.Add(child);
            child.Controls.Add(grandchild);

            control.ActiveControl = child;
            Assert.Same(child, control.ActiveControl);

            // Set same.
            control.ActiveControl = child;
            Assert.Same(child, control.ActiveControl);

            // Set grandchild.
            control.ActiveControl = grandchild;
            Assert.Same(grandchild, control.ActiveControl);

            // Set null.
            control.ActiveControl = null;
            Assert.Null(control.ActiveControl);
        }

        [Fact]
        public void ContainerControl_ActiveContainerContainerControl_SetInvalid_ThrowsArgumentException()
        {
            var control = new ContainerControl();
            Assert.Throws<ArgumentException>("value", () => control.ActiveControl = control);
            Assert.Throws<ArgumentException>("value", () => control.ActiveControl = new Control());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
        public void AutoScaleDimensions_Set_GetReturnsExpected(Size value)
        {
            var control = new ContainerControl
            {
                AutoScaleDimensions = value
            };
            Assert.Equal(value, control.AutoScaleDimensions);

            // Set same.
            control.AutoScaleDimensions = value;
            Assert.Equal(value, control.AutoScaleDimensions);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
        public void AutoScaleDimensions_SetWithChildren_GetReturnsExpected(Size value)
        {
            var child = new Control();
            var control = new ContainerControl();
            control.Controls.Add(child);

            control.AutoScaleDimensions = value;
            Assert.Equal(value, control.AutoScaleDimensions);

            // Set same.
            control.AutoScaleDimensions = value;
            Assert.Equal(value, control.AutoScaleDimensions);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoPositives)]
        public void AutoScaleDimensions_SetInvalid_ThrowsArgumentOutOfRangeException(Size value)
        {
            var control = new ContainerControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.AutoScaleDimensions = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoScaleMode))]
        public void AutoScaleMode_Set_GetReturnsExpected(AutoScaleMode value)
        {
            var control = new ContainerControl
            {
                AutoScaleMode = value
            };
            Assert.Equal(value, control.AutoScaleMode);

            // Set same.
            control.AutoScaleMode = value;
            Assert.Equal(value, control.AutoScaleMode);
        }

        public static IEnumerable<object[]> AutoScaleMode_SetDifferent_TestData()
        {
            yield return new object[] { AutoScaleMode.None, SizeF.Empty };
            yield return new object[] { AutoScaleMode.Dpi, new SizeF(96, 96) };
            yield return new object[] { AutoScaleMode.Inherit, SizeF.Empty };
        }

        [Theory]
        [MemberData(nameof(AutoScaleMode_SetDifferent_TestData))]
        public void AutoScaleMode_SetDifferent_ResetsAutoScaleDimensions(AutoScaleMode value, SizeF expectedAutoScaleDimensions)
        {
            var control = new ContainerControl
            {
                AutoScaleDimensions = new SizeF(1, 2),
                AutoScaleMode = AutoScaleMode.Font
            };

            control.AutoScaleMode = value;
            Assert.Equal(value, control.AutoScaleMode);
            Assert.Equal(expectedAutoScaleDimensions, control.AutoScaleDimensions);
            Assert.Equal(expectedAutoScaleDimensions, control.CurrentAutoScaleDimensions);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoScaleMode))]
        public void AutoScaleMode_SetInvalid_ThrowsInvalidEnumArgumentException(AutoScaleMode value)
        {
            var control = new ContainerControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoScaleMode = value);
        }

        public static IEnumerable<object[]> AutoValidate_Set_TestData()
        {
            yield return new object[] { AutoValidate.Disable, AutoValidate.Disable };
            yield return new object[] { AutoValidate.EnablePreventFocusChange, AutoValidate.EnablePreventFocusChange };
            yield return new object[] { AutoValidate.EnableAllowFocusChange, AutoValidate.EnableAllowFocusChange };
            yield return new object[] { AutoValidate.Inherit, AutoValidate.EnablePreventFocusChange };
        }

        [Theory]
        [MemberData(nameof(AutoValidate_Set_TestData))]
        public void AutoValidate_Set_GetReturnsExpected(AutoValidate value, AutoValidate expected)
        {
            var control = new ContainerControl
            {
                AutoValidate = value
            };
            Assert.Equal(expected, control.AutoValidate);

            // Set same.
            control.AutoValidate = value;
            Assert.Equal(expected, control.AutoValidate);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoValidate))]
        public void AutoValidate_SetInvalid_ThrowsInvalidEnumArgumentException(AutoValidate value)
        {
            var control = new ContainerControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.AutoValidate = value);
        }

        [Fact]
        public void ContainerControl_AutoValidate_SetWithHandler_CallsAutoValidateChanged()
        {
            var control = new ContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoValidateChanged += handler;

            // Set different.
            control.AutoValidate = AutoValidate.EnablePreventFocusChange;
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoValidate = AutoValidate.EnablePreventFocusChange;
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoValidate = AutoValidate.EnableAllowFocusChange;
            Assert.Equal(AutoValidate.EnableAllowFocusChange, control.AutoValidate);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoValidateChanged -= handler;
            control.AutoValidate = AutoValidate.Disable;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void ContainerControl_BindingContext_Set_GetReturnsExpected()
        {
            var value = new BindingContext();
            var control = new ContainerControl
            {
                BindingContext = value
            };
            Assert.Same(value, control.BindingContext);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);

            // Set null.
            control.BindingContext = null;
            Assert.NotNull(control.BindingContext);
        }

        [Fact]
        public void ContainerControl_BindingContext_SetWithHandler_CallsBindingContextChanged()
        {
            var control = new ContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var value1 = new BindingContext();
            control.BindingContext = value1;
            Assert.Same(value1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = value1;
            Assert.Same(value1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var value2 = new BindingContext();
            control.BindingContext = value2;
            Assert.Equal(value2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Remove handler.
            var value3 = new BindingContext();
            control.BindingContextChanged -= handler;
            control.BindingContext = value3;
            Assert.Same(value3, control.BindingContext);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Font_Set_GetReturnsExpected(Font value)
        {
            var control = new ContainerControl
            {
                Font = value
            };
            Assert.Same(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Same(value ?? Control.DefaultFont, control.Font);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Font_SetWithAutoScaleModeFont_GetReturnsExpected(Font value)
        {
            var control = new SubContainerControl
            {
                AutoScaleMode = AutoScaleMode.Font
            };

            control.Font = value;
            Assert.Same(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(new Size(1, 1), control.AutoScaleFactor);

            // Set same.
            control.Font = value;
            Assert.Same(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
        }

        [Fact]
        public void ContainerContainerControl_Font_SetWithHandler_CallsFontChanged()
        {
            var control = new ContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Same(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoScaleMode))]
        public void PerformAutoScale_InvokeWithoutChildren_Success(AutoScaleMode autoScaleMode)
        {
            var control = new SubContainerControl
            {
                AutoScaleMode = autoScaleMode
            };
            control.PerformAutoScale();
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoScaleMode))]
        public void PerformAutoScale_InvokeWithChildren_Success(AutoScaleMode autoScaleMode)
        {
            var child = new Control();
            var control = new SubContainerControl
            {
                AutoScaleMode = autoScaleMode
            };
            control.Controls.Add(child);
            control.PerformAutoScale();
        }

        [Fact]
        public void ContainerControl_CreateContainerContainerControl_Invoke_CallsBindingContextChanged()
        {
            var control = new ContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var value1 = new BindingContext();
            control.BindingContext = value1;
            Assert.Same(value1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = value1;
            Assert.Same(value1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var value2 = new BindingContext();
            control.BindingContext = value2;
            Assert.Equal(value2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Remove handler.
            var value3 = new BindingContext();
            control.BindingContextChanged -= handler;
            control.BindingContext = value3;
            Assert.Same(value3, control.BindingContext);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void ContainerControl_Dispose_Invoke_ResetsActiveControl()
        {
            var control = new ContainerControl();
            var child = new Control();
            control.Controls.Add(child);
            control.ActiveControl = child;

            control.Dispose();
            Assert.Null(control.ActiveControl);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContainerControl_OnAutoValidateChanged_Invoke_CallsAutoValidateChanged(EventArgs eventArgs)
        {
            var control = new SubContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            control.AutoValidateChanged += handler;
            control.OnAutoValidateChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           control.AutoValidateChanged -= handler;
           control.OnAutoValidateChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContainerControl_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            var control = new SubContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           control.FontChanged -= handler;
           control.OnFontChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContainerControl_OnFontChanged_InvokeWithAutoScaleModeFont_CallsFontChanged(EventArgs eventArgs)
        {
            var control = new SubContainerControl
            {
                AutoScaleMode = AutoScaleMode.Font
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           control.FontChanged -= handler;
           control.OnFontChanged(eventArgs);
            Assert.Equal(new Size(1, 1), control.AutoScaleFactor);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void ContainerControl_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            var control = new SubContainerControl();
            int callCount = 0;
            LayoutEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            control.Layout += handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           control.Layout -= handler;
           control.OnLayout(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContainerControl_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
        {
            var control = new SubContainerControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ParentChanged += handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ContainerControl_UpdateDefaultButton_Invoke_Nop()
        {
            var control = new SubContainerControl();
            control.UpdateDefaultButton();
            control.UpdateDefaultButton();
        }

        public class SubContainerControl : ContainerControl
        {
            public new SizeF AutoScaleFactor => base.AutoScaleFactor;

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

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void UpdateDefaultButton() => base.UpdateDefaultButton();

            public new void OnAutoValidateChanged(EventArgs e) => base.OnAutoValidateChanged(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);
        }
    }
}
