// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Moq;
using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class FormTests
    {
        [Fact]
        public void Form_Ctor_Default()
        {
            var form = new Form();
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

        [Fact]
        public void Form_AcceptButtonGetSet()
        {
            var form = new Form();
            var mock = new Mock<IButtonControl>(MockBehavior.Strict);
            mock.Setup(x => x.NotifyDefault(It.IsAny<bool>()));

            form.AcceptButton = mock.Object;

            Assert.Equal(mock.Object, form.AcceptButton);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_Active_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
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

        [Fact]
        public void Form_ActiveMdiChildInternalGetSet()
        {
            var form = new Form();
            var child = new Form();

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChildInternal);
            Assert.Equal(child, form.ActiveMdiChildInternal);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSet()
        {
            var form = new Form();
            var child = new Form
            {
                Visible = true,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.NotNull(form.ActiveMdiChild);
            Assert.Equal(child, form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotVisible()
        {
            var form = new Form();
            var child = new Form
            {
                Visible = false,
                Enabled = true
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Fact]
        public void Form_ActiveMdiChildGetSetChildNotEnabled()
        {
            var form = new Form();
            var child = new Form
            {
                Visible = true,
                Enabled = false
            };

            form.ActiveMdiChildInternal = child;

            Assert.Null(form.ActiveMdiChild);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Form_AllowTransparency_Set_GetReturnsExpected(bool value)
        {
            var form = new Form
            {
                AllowTransparency = value
            };
            Assert.Equal(value, form.AllowTransparency);

            // Set same.
            form.AllowTransparency = value;
            Assert.Equal(value, form.AllowTransparency);
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
    }
}
