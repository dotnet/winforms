// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class FormTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Form_Ctor_Default()
        {
            using var form = new Form();
            Assert.False(form.Active);
            Assert.Null(form.ActiveMdiChild);
            Assert.False(form.AllowTransparency);
#pragma warning disable 0618
            Assert.True(form.AutoScale);
#pragma warning restore 0618
            Assert.Equal(AutoScaleMode.Inherit, form.AutoScaleMode);
            Assert.False(form.AutoScroll);
            Assert.False(form.AutoSize);
            Assert.False(form.IsRestrictedWindow);
            Assert.True(form.TopLevel);
            Assert.False(form.Visible);
        }

        [WinFormsFact]
        public void Form_AcceptButtonGetSet()
        {
            using var form = new Form();
            var mock = new Mock<IButtonControl>(MockBehavior.Strict);
            mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

            form.AcceptButton = mock.Object;

            Assert.Equal(mock.Object, form.AcceptButton);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_Active_Set_GetReturnsExpected(bool value)
        {
            using var form = new Form
            {
                Active = value
            };
            Assert.Equal(value, form.Active);

            // Set same.
            form.Active = value;
            Assert.Equal(value, form.Active);
        }

        // non deterministic, commenting out for now
        /*[Fact]
        public void Form_ActiveFormNotSetActive()
        {
            var form = new Form();

            form.Visible = true;
            form.Active = false;

            Assert.NotNull(Form.ActiveForm);
            Assert.Equal(form, Form.ActiveForm);
            Assert.False(Form.ActiveForm.Active);
        }*/

        [WinFormsFact]
        public void Form_ActiveMdiChildInternalGetSet()
        {
            using var form = new Form();
            using var child = new Form();

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChildInternal);
            Assert.Equal(child, form.ActiveMdiChildInternal);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSet()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = true,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSetChildNotVisible()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = false,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [WinFormsFact]
        public void Form_ActiveMdiChildGetSetChildNotEnabled()
        {
            using var form = new Form();
            using var child = new Form
            {
                Visible = true,
                Enabled = false
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AllowTransparency_Set_GetReturnsExpected(bool value)
        {
            using var control = new Form
            {
                AllowTransparency = value
            };
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AllowTransparency_SetWithOpacityAndTransparencyKey_TestData()
        {
            yield return new object[] { true, Color.Red, 0.5 };
            yield return new object[] { false, Color.Empty, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AllowTransparency_SetWithOpacityAndTransparencyKey_TestData))]
        public void Form_AllowTransparency_SetWithOpacityAndTransparencyKey_GetReturnsExpected(bool value, Color expectedTransparencyKey, float expectedOpacity)
        {
            using var control = new Form
            {
                Opacity = 0.5,
                TransparencyKey = Color.Red,
                AllowTransparency = value
            };
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
            Assert.Equal(expectedOpacity, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
            Assert.Equal(expectedOpacity, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AllowTransparency_SetNotTopLevel_GetReturnsExpected(bool value)
        {
            using var control = new Form
            {
                TopLevel = false,
                AllowTransparency = value
            };
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Form_AllowTransparency_SetWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_TestData()
        {
            yield return new object[] { true, Color.Red, 0.5, 0 };
            yield return new object[] { false, Color.Empty, 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_TestData))]
        public void Form_AllowTransparency_SetWithOpacityAndTransparencyKeyWithHandle_GetReturnsExpected(bool value, Color expectedTransparencyKey, float expectedOpacity, int expectedStyleChangedCallCount)
        {
            using var control = new Form
            {
                Opacity = 0.5,
                TransparencyKey = Color.Red
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
            Assert.Equal(expectedOpacity, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(expectedTransparencyKey, control.TransparencyKey);
            Assert.Equal(expectedOpacity, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Form_AllowTransparency_SetNotTopLevelWithHandle_GetReturnsExpected(bool value, int expectedStyleChangedCallCount)
        {
            using var control = new Form
            {
                TopLevel = false
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowTransparency = value;
            Assert.Equal(value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowTransparency = !value;
            Assert.Equal(!value, control.AllowTransparency);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(1, control.Opacity);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

#pragma warning disable 0618

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoScale_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoScale = value
            };
            Assert.Equal(value, form.AutoScale);

            // Set same.
            form.AutoScale = value;
            Assert.Equal(value, form.AutoScale);
        }

#pragma warning restore 0618

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoScaleMode))]
        public void Form_AutoScaleMode_Set_GetReturnsExpected(AutoScaleMode value)
        {
            var form = new Form
            {
                AutoScaleMode = value
            };
            Assert.Equal(value, form.AutoScaleMode);

            // Set same.
            form.AutoScaleMode = value;
            Assert.Equal(value, form.AutoScaleMode);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoScroll_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoScroll = value
            };
            Assert.Equal(value, form.AutoScroll);

            // Set same.
            form.AutoScroll = value;
            Assert.Equal(value, form.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AutoSizeSet_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AutoSize = value
            };
            Assert.Equal(value, form.AutoSize);

            // Set same.
            form.AutoSize = value;
            Assert.Equal(value, form.AutoSize);
        }

        public static IEnumerable<object[]> Opacity_Set_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, 1.1, 1.0, allowTransparency, allowTransparency };
                yield return new object[] { allowTransparency, 1.0, 1.0, allowTransparency, allowTransparency };
                yield return new object[] { allowTransparency, 0.5, 0.5, true, false };
                yield return new object[] { allowTransparency, 0, 0, true, false };
                yield return new object[] { allowTransparency, -0.1, 0, true, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_Set_TestData))]
        public void Form_Opacity_Set_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, bool expectedHandleCreated)
        {
            using var control = new Form
            {
                AllowTransparency = allowTransparency,
                Opacity = value
            };
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);

            // Set same.
            control.Opacity = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Opacity_SetWithTransparencyKey_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, 1.1, 1.0 };
                yield return new object[] { allowTransparency, 1.0, 1.0 };
                yield return new object[] { allowTransparency, 0.5, 0.5 };
                yield return new object[] { allowTransparency, 0, 0 };
                yield return new object[] { allowTransparency, -0.1, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_SetWithTransparencyKey_TestData))]
        public void Form_Opacity_SetWithTransparencyKey_GetReturnsExpected(bool allowTransparency, double value, double expected)
        {
            using var control = new Form
            {
                AllowTransparency = allowTransparency,
                TransparencyKey = Color.Red,
                Opacity = value
            };
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Red, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Red, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_Set_TestData))]
        public void Form_Opacity_SetTopLevel_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, bool expectedHandleCreated)
        {
            using var control = new Form
            {
                TopLevel = false,
                AllowTransparency = allowTransparency,
                Opacity = value
            };
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);

            // Set same.
            control.Opacity = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Opacity_SetNotTransparentWithHandle_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, 1.1, 1.0 };
                yield return new object[] { allowTransparency, 1.0, 1.0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_SetNotTransparentWithHandle_TestData))]
        public void Form_Opacity_SetNotTransparentWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AllowTransparency = allowTransparency;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Opacity = value;
            Assert.Equal(allowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Opacity = value;
            Assert.Equal(allowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0.5, 0.5, 0)]
        [InlineData(0, 0, 0)]
        [InlineData(-0.1, 0, 0)]
        public void Form_Opacity_SetTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected(float value, float expected, int expectedStyleChangedCallCount)
        {
            using var control = new Form
            {
                AllowTransparency = true
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Opacity_SetTransparentWithHandle_TestData()
        {
            yield return new object[] { true, 0.5, 0.5, 0 };
            yield return new object[] { true, 0, 0, 0 };
            yield return new object[] { true, -0.1, -0, 0 };

            yield return new object[] { false, 0.5, 0.5, 1 };
            yield return new object[] { false, 0, 0, 1 };
            yield return new object[] { false, -0.1, -0, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_SetTransparentWithHandle_TestData))]
        public void Form_Opacity_SetTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected(bool allowTransparency, float value, float expected, int expectedStyleChangedCallCount)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AllowTransparency = allowTransparency;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Opacity_SetWithTransparencyKeyWithHandle_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, 1.1, 1.0 };
                yield return new object[] { allowTransparency, 1.0, 1.0 };
                yield return new object[] { allowTransparency, 0.5, 0.5 };
                yield return new object[] { allowTransparency, 0, 0 };
                yield return new object[] { allowTransparency, -0.1, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_SetWithTransparencyKeyWithHandle_TestData))]
        public void Form_Opacity_SetWithTransparencyKeyWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AllowTransparency = allowTransparency;
            control.TransparencyKey = Color.Red;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Red, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Opacity = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Red, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Opacity_SetTopLevelWithHandle_TestData()
        {
            yield return new object[] { true, 1.1, 1.0, true, 0 };
            yield return new object[] { true, 1.0, 1.0, true, 0 };
            yield return new object[] { true, 0.5, 0.5, true, 0 };
            yield return new object[] { true, 0, 0, true, 0 };
            yield return new object[] { true, -0.1, 0, true, 0 };

            yield return new object[] { false, 1.1, 1.0, false, 0 };
            yield return new object[] { false, 1.0, 1.0, false, 0 };
            yield return new object[] { false, 0.5, 0.5, true, 1 };
            yield return new object[] { false, 0, 0, true, 1 };
            yield return new object[] { false, -0.1, 0, true, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Opacity_SetTopLevelWithHandle_TestData))]
        public void Form_Opacity_SetTopLevelWithHandle_GetReturnsExpected(bool allowTransparency, double value, double expected, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
        {
            using var control = new Form
            {
                TopLevel = false,
                AllowTransparency = allowTransparency
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Opacity = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Opacity = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(expected, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
            yield return new object[] { new Form() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Form_Parent_Set_GetReturnsExpected(Control value)
        {
            using var control = new Form
            {
                TopLevel = false,
                Parent = value
            };
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Form_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            using var oldParent = new Control();
            using var control = new Form
            {
                TopLevel = false,
                Parent = oldParent
            };

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Parent_SetMdiChild_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_SetMdiChild_TestData))]
        public void Form_Parent_SetMdiChild_GetReturnsExpected(Control value)
        {
            using var oldParent = new Form
            {
                IsMdiContainer = true
            };
            using var control = new Form
            {
                MdiParent = oldParent,
                Parent = value
            };

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Form_Parent_SetNonNull_AddsToControls()
        {
            using var parent = new Control();
            using var control = new Form
            {
                TopLevel = false,
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Form_Parent_SetWithHandle_GetReturnsExpected(Control value)
        {
            using var control = new Form
            {
                TopLevel = false
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Null(control.MdiParent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Form_Parent_SetWithHandler_CallsParentChanged()
        {
            using var parent = new Control();
            using var control = new Form
            {
                TopLevel = false
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void Form_Parent_SetSame_ThrowsArgumentException()
        {
            using var control = new Form
            {
                TopLevel = false
            };
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [WinFormsFact]
        public void Form_Parent_SetTopLevel_ThrowsArgumentException()
        {
            using var control = new Form();
            using var parent = new Control();
            Assert.Throws<ArgumentException>(null, () => control.Parent = parent);
            Assert.Null(control.Parent);
        }

        [WinFormsFact]
        public void Form_Parent_SetFormWithMdiParent_ThrowsArgumentException()
        {
            using var oldParent = new Form
            {
                IsMdiContainer = true
            };
            using var control = new Form
            {
                MdiParent = oldParent
            };
            using var parent = new Form();
            Assert.Throws<ArgumentException>("value", () => control.Parent = parent);
            Assert.NotNull(control.Parent);
            Assert.Same(oldParent, control.MdiParent);
        }

        public static IEnumerable<object[]> TransparencyKey_Set_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, Color.Empty, allowTransparency };
                yield return new object[] { allowTransparency, Color.Red, true };
                yield return new object[] { allowTransparency, Color.Transparent, true };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_Set_TestData))]
        public void Form_TransparencyKey_Set_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency)
        {
            using var control = new Form
            {
                AllowTransparency = allowTransparency,
                TransparencyKey = value
            };
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> TransparencyKey_SetWithCustomOldValue_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, Color.Empty };
                yield return new object[] { allowTransparency, Color.Red };
                yield return new object[] { allowTransparency, Color.Transparent };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithCustomOldValue_TestData))]
        public void Form_TransparencyKey_SetWithCustomOldValue_GetReturnsExpected(bool allowTransparency, Color value)
        {
            using var control = new Form
            {
                AllowTransparency = allowTransparency,
                TransparencyKey = Color.Yellow
            };

            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> TransparencyKey_SetWithOpacity_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, Color.Empty };
                yield return new object[] { allowTransparency, Color.Red };
                yield return new object[] { allowTransparency, Color.Transparent };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithOpacity_TestData))]
        public void Form_TransparencyKey_SetWithOpacity_GetReturnsExpected(bool allowTransparency, Color value)
        {
            using var control = new Form
            {
                AllowTransparency = allowTransparency,
                Opacity = 0.5,
                TransparencyKey = value
            };
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> TransparencyKey_SetMdiContainer_TestData()
        {
            foreach (bool allowTransparency in new bool[] { true, false })
            {
                yield return new object[] { allowTransparency, Color.Empty };
                yield return new object[] { allowTransparency, Color.Red };
                yield return new object[] { allowTransparency, Color.Transparent };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetMdiContainer_TestData))]
        public void Form_TransparencyKey_SetMdiContainer_Nop(bool allowTransparency, Color value)
        {
            using var control = new Form
            {
                IsMdiContainer = true,
                AllowTransparency = allowTransparency,
                TransparencyKey = value
            };
            Assert.Equal(allowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            Assert.Equal(allowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_Set_TestData))]
        public void Form_TransparencyKey_SetNotTopLevel_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency)
        {
            using var control = new Form
            {
                TopLevel = true,
                AllowTransparency = allowTransparency,
                TransparencyKey = value
            };
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Form_TransparencyKey_SetNotTransparentWithHandleSetNotAllowTransparency_GetReturnsExpected()
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = Color.Empty;
            Assert.False(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = Color.Empty;
            Assert.False(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> TransparencyKey_SetWithHandleSetNotAllowTransparency_TestData()
        {
            yield return new object[] { Color.Red, true, 2 };
            yield return new object[] { Color.Transparent, true, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithHandleSetNotAllowTransparency_TestData))]
        public void Form_TransparencyKey_SetTransparentWithHandleSetNotAllowTransparency_GetReturnsExpected(Color value, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Form_TransparencyKey_SetNotTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected()
        {
            using var control = new Form
            {
                AllowTransparency = true
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> TransparencyKey_SetWithHandleSetAllowTransparency_TestData()
        {
            yield return new object[] { Color.Red };
            yield return new object[] { Color.Transparent };
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
        public void Form_TransparencyKey_SetTransparentWithHandleSetAllowTransparencyBefore_GetReturnsExpected(Color value)
        {
            using var control = new Form
            {
                AllowTransparency = true
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Form_TransparencyKey_SetNotTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected()
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AllowTransparency = true;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
        public void Form_TransparencyKey_SetTransparentWithHandleSetAllowTransparencyAfter_GetReturnsExpected(Color value)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AllowTransparency = true;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Form_TransparencyKey_SetNotTransparentWithHandleSetOpacityBefore_GetReturnsExpected()
        {
            using var control = new Form
            {
                Opacity = 0.5
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
        public void Form_TransparencyKey_SetTransparentWithHandleSetOpacityBefore_GetReturnsExpected(Color value)
        {
            using var control = new Form
            {
                Opacity = 0.5
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Form_TransparencyKey_SetNotTransparentWithHandleSetOpacityAfter_GetReturnsExpected()
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.Opacity = 0.5;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = Color.Empty;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(Color.Empty, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetWithHandleSetAllowTransparency_TestData))]
        public void Form_TransparencyKey_SetTransparentWithHandleSetOpacityAfter_GetReturnsExpected(Color value)
        {
            using var control = new Form();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.Opacity = 0.5;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.True(control.AllowTransparency);
            Assert.Equal(0.5, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> TransparencyKey_SetNotTopLevelWithHandle_TestData()
        {
            yield return new object[] { true, Color.Empty, true, 1 };
            yield return new object[] { true, Color.Red, true, 0 };
            yield return new object[] { true, Color.Transparent, true, 0 };

            yield return new object[] { false, Color.Empty, false, 0 };
            yield return new object[] { false, Color.Red, true, 2 };
            yield return new object[] { false, Color.Transparent, true, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(TransparencyKey_SetNotTopLevelWithHandle_TestData))]
        public void Form_TransparencyKey_SetNotTopLevelWithHandle_GetReturnsExpected(bool allowTransparency, Color value, bool expectedAllowTransparency, int expectedStyleChangedCallCount)
        {
            using var control = new Form
            {
                TopLevel = false,
                AllowTransparency = allowTransparency
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TransparencyKey = value;
            Assert.Equal(expectedAllowTransparency, control.AllowTransparency);
            Assert.Equal(1, control.Opacity);
            Assert.Equal(value, control.TransparencyKey);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Visible_Set_TestData()
        {
            foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
            {
                yield return new object[] { dialogResult, true };
                yield return new object[] { dialogResult, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void Form_Visible_Set_GetReturnsExpected(DialogResult dialogResult, bool value)
        {
            using var control = new Form
            {
                DialogResult = dialogResult,
                Visible = value
            };
            Assert.Equal(value, control.Visible);
            Assert.Equal(value, control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(value, control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Visible_SetMdiChild_TestData()
        {
            foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
            {
                yield return new object[] { dialogResult, true, true, true };
                yield return new object[] { dialogResult, true, false, false };
                yield return new object[] { dialogResult, false, true, false };
                yield return new object[] { dialogResult, false, false, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_SetMdiChild_TestData))]
        public void Form_Visible_SetMdiChild_GetReturnsExpected(DialogResult dialogResult, bool parentVisible, bool value, bool expected)
        {
            using var parent = new Form
            {
                IsMdiContainer = true,
                Visible = parentVisible
            };
            using var control = new Form
            {
                MdiParent = parent,
                DialogResult = dialogResult,
                Visible = value
            };
            Assert.Equal(expected, control.Visible);
            Assert.Equal(expected, control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(expected, control.Visible);
            Assert.Equal(expected, control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!expected && parentVisible, control.Visible);
            Assert.Equal(!expected && parentVisible, control.IsHandleCreated);

            // Set same.
            control.Visible = !value;
            Assert.Equal(!expected && parentVisible, control.Visible);
            Assert.Equal(!expected && parentVisible, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void Form_Visible_SetWithHandle_GetReturnsExpected(DialogResult dialogResult, bool value)
        {
            using var control = new Form
            {
                DialogResult = dialogResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DialogResult))]
        public void Form_Visible_SetTrueMdiChildVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult)
        {
            using var parent = new Form
            {
                IsMdiContainer = true,
                Visible = true
            };
            using var control = new Form
            {
                MdiParent = parent,
                DialogResult = dialogResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Visible = true;
            Assert.Equal(dialogResult == DialogResult.OK || dialogResult == DialogResult.None, control.Visible);
            Assert.Equal(dialogResult == DialogResult.OK || dialogResult == DialogResult.None, control.IsHandleCreated);

            // Set same.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.True(control.IsHandleCreated);

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Visible = true;
            Assert.Equal(dialogResult != DialogResult.OK, control.Visible);
            Assert.Equal(dialogResult != DialogResult.OK, control.IsHandleCreated);

            // Set same.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DialogResult))]
        public void Form_Visible_SetFalseMdiChildVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult)
        {
            using var parent = new Form
            {
                IsMdiContainer = true,
                Visible = true
            };
            using var control = new Form
            {
                MdiParent = parent,
                DialogResult = dialogResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            control.Visible = true;
            Assert.Equal(dialogResult == DialogResult.None, control.Visible);
            Assert.Equal(dialogResult == DialogResult.None, control.IsHandleCreated);

            // Set same.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.True(control.IsHandleCreated);

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void Form_Visible_SetMdiChildNotVisibleWithHandle_GetReturnsExpected(DialogResult dialogResult, bool value)
        {
            using var parent = new Form
            {
                IsMdiContainer = true
            };
            using var control = new Form
            {
                MdiParent = parent,
                DialogResult = dialogResult,
                Visible = value
            };
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = !value;
            Assert.False(control.Visible);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Visible_SetWithHandler_TestData()
        {
            foreach (DialogResult dialogResult in Enum.GetValues(typeof(DialogResult)))
            {
                yield return new object[] { dialogResult, true, 1 };
                yield return new object[] { dialogResult, false, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_SetWithHandler_TestData))]
        public void Form_Visible_SetTrueWithHandler_CallsVisibleChanged(DialogResult dialogResult, bool value, int expectedLoadCallCount)
        {
            using var control = new Form
            {
                DialogResult = dialogResult
            };
            int loadCallCount = 0;
            control.Load += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                loadCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount, callCount);
            Assert.Equal(expectedLoadCallCount, loadCallCount);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount, callCount);
            Assert.Equal(expectedLoadCallCount, loadCallCount);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);

            // Set same.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);
        }

        [WinFormsFact]
        public void Form_Visible_SetWithHandlerMdiChildVisible_CallsVisibleChanged()
        {
            using var parent = new Form
            {
                IsMdiContainer = true,
                Visible = true
            };
            using var control = new Form
            {
                MdiParent = parent
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(3, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(3, callCount);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(4, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(4, callCount);
        }

        [WinFormsFact]
        public void Form_Visible_SetWithHandlerMdiChildNotVisible_CallsVisibleChanged()
        {
            using var parent = new Form
            {
                IsMdiContainer = true
            };
            using var control = new Form
            {
                MdiParent = parent
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = true;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = true;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);

            // Set different.
            control.Visible = true;
            Assert.False(control.Visible);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_SetWithHandler_TestData))]
        public void Form_Visible_SetTrueWithHandlerWithHandle_CallsVisibleChanged(DialogResult dialogResult, bool value, int expectedLoadCallCount)
        {
            using var control = new Form
            {
                DialogResult = dialogResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int loadCallCount = 0;
            control.Load += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                loadCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount, callCount);
            Assert.Equal(expectedLoadCallCount, loadCallCount);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount, callCount);
            Assert.Equal(expectedLoadCallCount, loadCallCount);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);

            // Set same.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.Equal(expectedLoadCallCount + 1, callCount);
            Assert.Equal(1, loadCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_Visible_SetWithHandlerWithHandle_CallsVisibleChanged(bool initialVisible)
        {
            using var control = new Form
            {
                Visible = initialVisible
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
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = !initialVisible;
            Assert.Equal(!initialVisible, control.Visible);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Visible = !initialVisible;
            Assert.Equal(!initialVisible, control.Visible);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Visible = initialVisible;
            Assert.Equal(initialVisible, control.Visible);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = !initialVisible;
            Assert.Equal(!initialVisible, control.Visible);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Form_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubForm();
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
        public void Form_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubForm();
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
        public void Form_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubForm();
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
        public void Form_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubForm();
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

        private class SubForm : Form
        {
            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);
        }
    }
}
