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
        public void Ctor_Default()
        {
            var control = new SubContainerControl();
            Assert.Null(control.ActiveControl);
            Assert.False(control.AutoScroll);
            Assert.Equal(SizeF.Empty, control.AutoScaleDimensions);
            Assert.Equal(new SizeF(1, 1), control.AutoScaleFactor);
            Assert.Equal(AutoScaleMode.Inherit, control.AutoScaleMode);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.Equal(AutoValidate.EnablePreventFocusChange, control.AutoValidate);
            Assert.NotNull(control.BindingContext);
            Assert.Same(control.BindingContext, control.BindingContext);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.True(control.CanProcessMnemonic());
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(SizeF.Empty, control.CurrentAutoScaleDimensions);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.ParentForm);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.True(control.Visible);
        }

        [Fact]
        public void ActiveControl_Set_GetReturnsExpected()
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
        public void ActiveControl_SetInvalid_ThrowsArgumentException()
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
        public void AutoValidate_SetWithHandler_CallsAutoValidateChanged()
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
        public void BindingContext_Set_GetReturnsExpected()
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
        public void BindingContext_SetWithHandler_CallsBindingContextChanged()
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
        public void Font_SetWithAutoScaleFont_GetReturnsExpected(Font value)
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
        public void CreateControl_Invoke_CallsBindingContextChanged()
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
        public void Dispose_Invoke_ResetsActiveControl()
        {
            var control = new ContainerControl();
            var child = new Control();
            control.Controls.Add(child);
            control.ActiveControl = child;

            control.Dispose();
            Assert.Null(control.ActiveControl);
        }

        private class SubContainerControl : ContainerControl
        {
            public new SizeF AutoScaleFactor => base.AutoScaleFactor;

            public new bool CanEnableIme => base.CanEnableIme;

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
        }
    }
}
