// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public partial class ControlTests
    {
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnAutoSizeChanged_Invoke_CallsAutoSizeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.AutoSizeChanged += handler;
            control.OnAutoSizeChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.AutoSizeChanged -= handler;
            control.OnAutoSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_OnBackColorChanged_InvokeInDisposing_DoesNotCallBackColorChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackColorChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnBackColorChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithChildren_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BackColorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BackColorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithChildrenWithBackColor_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                BackColor = Color.Yellow
            };
            using var child2 = new Control
            {
                BackColor = Color.YellowGreen
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BackColorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BackColorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_Invoke_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackgroundImageChanged += handler;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithHandle_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.BackgroundImageChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_OnBackgroundImageChanged_InvokeInDisposing_DoesNotCallBackgroundImageChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackgroundImageChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnBackgroundImageChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithChildren_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackgroundImageChanged += handler;
            child1.BackgroundImageChanged += childHandler1;
            child2.BackgroundImageChanged += childHandler2;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            child1.BackgroundImageChanged -= childHandler1;
            child2.BackgroundImageChanged -= childHandler2;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithChildrenWithBackgroundImage_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                BackgroundImage = new Bitmap(10, 10)
            };
            using var child2 = new Control
            {
                BackgroundImage = new Bitmap(10, 10)
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackgroundImageChanged += handler;
            child1.BackgroundImageChanged += childHandler1;
            child2.BackgroundImageChanged += childHandler2;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            child1.BackgroundImageChanged -= childHandler1;
            child2.BackgroundImageChanged -= childHandler2;
            control.OnBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_Invoke_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackgroundImageLayoutChanged += handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithHandle_CallsBackgroundImageLayoutChangedAndInvalidated(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.BackgroundImageLayoutChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithChildren_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BackgroundImageLayoutChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BackgroundImageLayoutChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackgroundImageLayoutChanged += handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithChildrenWithBackgroundImageLayout_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                BackgroundImageLayout = ImageLayout.Center
            };
            using var child2 = new Control
            {
                BackgroundImageLayout = ImageLayout.Center
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BackgroundImageLayoutChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BackgroundImageLayoutChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BackgroundImageLayoutChanged += handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsFact]
        public void Control_OnBackgroundImageLayoutChanged_InvokeInDisposing_DoesNotCallBackgroundImageLayoutChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackgroundImageLayoutChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnBackgroundImageLayoutChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_Invoke_CallsBindingContextChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BindingContextChanged += handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_InvokeWithChildren_CallsBindingContextChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BindingContextChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BindingContextChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BindingContextChanged += handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_InvokeWithChildrenWithBindingContext_CallsBindingContextChanged(EventArgs eventArgs)
        {
            var childContext1 = new BindingContext();
            var childContext2 = new BindingContext();
            using var child1 = new Control
            {
                BindingContext = childContext1
            };
            using var child2 = new Control
            {
                BindingContext = childContext2
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.BindingContextChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.BindingContextChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.BindingContextChanged += handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCausesValidationChanged_Invoke_CallsCausesValidationChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CausesValidationChanged += handler;
            control.OnCausesValidationChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.OnCausesValidationChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> UICuesEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new UICuesEventArgs(UICues.None) };
            yield return new object[] { new UICuesEventArgs(UICues.Changed) };
        }

        [WinFormsTheory]
        [MemberData(nameof(UICuesEventArgs_TestData))]
        public void Control_OnChangeUICues_Invoke_CallsChangeUICues(UICuesEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            UICuesEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ChangeUICues += handler;
            control.OnChangeUICues(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ChangeUICues -= handler;
            control.OnChangeUICues(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Click += handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Click -= handler;
            control.OnClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClientSizeChanged_Invoke_CallsClientSizeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int sizeChangedCallCount = 0;
            EventHandler sizeChangedHandler = (sender, e) => sizeChangedCallCount++;
            int resizeCallCount = 0;
            EventHandler resizeHandler = (sender, e) => resizeCallCount++;

            // Call with handler.
            control.ClientSizeChanged += handler;
            control.SizeChanged += sizeChangedHandler;
            control.Resize += resizeHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ClientSizeChanged -= handler;
            control.SizeChanged += sizeChangedHandler;
            control.Resize += resizeHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClientSizeChanged_InvokeWithHandle_CallsClientSizeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int sizeChangedCallCount = 0;
            EventHandler sizeChangedHandler = (sender, e) => sizeChangedCallCount++;
            int resizeCallCount = 0;
            EventHandler resizeHandler = (sender, e) => resizeCallCount++;
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.ClientSizeChanged += handler;
            control.SizeChanged += sizeChangedHandler;
            control.Resize += resizeHandler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.ClientSizeChanged -= handler;
            control.SizeChanged -= sizeChangedHandler;
            control.Resize -= resizeHandler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClientSizeChanged_InvokeWithResizeRedraw_CallsClientSizeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, true);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int sizeChangedCallCount = 0;
            EventHandler sizeChangedHandler = (sender, e) => sizeChangedCallCount++;
            int resizeCallCount = 0;
            EventHandler resizeHandler = (sender, e) => resizeCallCount++;
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.ClientSizeChanged += handler;
            control.SizeChanged += sizeChangedHandler;
            control.Resize += resizeHandler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.ClientSizeChanged -= handler;
            control.SizeChanged -= sizeChangedHandler;
            control.Resize -= resizeHandler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnClientSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnContextMenuStripChanged_Invoke_CallsContextMenuStripChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ContextMenuStripChanged += handler;
            control.OnContextMenuStripChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ContextMenuStripChanged -= handler;
            control.OnContextMenuStripChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ControlEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ControlEventArgs(null) };
            yield return new object[] { new ControlEventArgs(new Control()) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlEventArgs_TestData))]
        public void Control_OnControlAdded_Invoke_CallsControlAdded(ControlEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            ControlEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ControlAdded += handler;
            control.OnControlAdded(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ControlAdded -= handler;
            control.OnControlAdded(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlEventArgs_TestData))]
        public void Control_OnControlRemoved_Invoke_CallsControlRemoved(ControlEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            ControlEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ControlRemoved += handler;
            control.OnControlRemoved(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ControlRemoved -= handler;
            control.OnControlRemoved(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void Control_OnCreateControl_Invoke_Nop()
        {
            using var control = new SubControl();
            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_Invoke_CallsCursorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CursorChanged += handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_InvokeWithChildren_CallsCursorChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.CursorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.CursorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.CursorChanged += handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_InvokeWithChildrenWithCursor_CallsCursorChanged(EventArgs eventArgs)
        {
            var childCursor1 = new Cursor((IntPtr)1);
            var childCursor2 = new Cursor((IntPtr)1);
            using var child1 = new Control
            {
                Cursor = childCursor1
            };
            using var child2 = new Control
            {
                Cursor = childCursor2
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.CursorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child1.CursorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.CursorChanged += handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDockChanged_Invoke_CallsDockChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DockChanged += handler;
            control.OnDockChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.OnDockChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            using var control = new SubControl();
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

            // Remove handler.
            control.DoubleClick -= handler;
            control.OnDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDpiChangedBeforeParent_Invoke_CallsDpiChangedAfterParent(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DpiChangedAfterParent += handler;
            control.OnDpiChangedAfterParent(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DpiChangedAfterParent -= handler;
            control.OnDpiChangedAfterParent(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDpiChangedBeforeParent_Invoke_CallsDpiChangedBeforeParent(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DpiChangedBeforeParent += handler;
            control.OnDpiChangedBeforeParent(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DpiChangedBeforeParent -= handler;
            control.OnDpiChangedBeforeParent(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> DragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DragEventArgs(null, 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragDrop_Invoke_CallsDragDrop(DragEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DragDrop += handler;
            control.OnDragDrop(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DragDrop -= handler;
            control.OnDragDrop(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragEnter_Invoke_CallsDragEnter(DragEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DragEnter += handler;
            control.OnDragEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DragEnter -= handler;
            control.OnDragEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDragLeave_Invoke_CallsDragLeave(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DragLeave += handler;
            control.OnDragLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DragLeave -= handler;
            control.OnDragLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragOver_Invoke_CallsDragOver(DragEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.DragOver += handler;
            control.OnDragOver(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.DragOver -= handler;
            control.OnDragOver(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.EnabledChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithHandle_CallsEnabledChangedCallsInvalidated(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.EnabledChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithHandleNoUserPaint_CallsEnabledChangedDoesNotCallInvalidated(EventArgs eventArgs)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, false);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.EnabledChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithChildren_CallsEnabledChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.EnabledChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            child2.EnabledChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithChildrenDisabled_CallsEnabledChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                Enabled = false
            };
            using var child2 = new Control
            {
                Enabled = false
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.EnabledChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            child2.EnabledChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubControl();
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

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithFontHeight_CallsFontChanged(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                FontHeight = 10
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
            Assert.Equal(1, callCount);
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnFontChanged_WithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, null };
                yield return new object[] { userPaint, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnFontChanged_WithHandle_TestData))]
        public void Control_OnFontChanged_InvokeWithHandle_CallsFontChangedAndInvalidated(bool userPaint, EventArgs eventArgs)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
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
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(Control.DefaultFont.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithChildren_CallsFontChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.FontChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.FontChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithChildrenWithFont_CallsFontChanged(EventArgs eventArgs)
        {
            using var childFont1 = new Font("Arial", 1);
            using var childFont2 = new Font("Arial", 2);
            using var child1 = new Control
            {
                Font = childFont1
            };
            using var child2 = new Control
            {
                Font = childFont2
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.FontChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.FontChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.FontChanged += handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsFact]
        public void Control_OnFontChanged_InvokeInDisposing_DoesNotCallFontChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.FontChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnFontChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ForeColorChanged += handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithHandle_CallsForeColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.ForeColorChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_OnForeColorChanged_InvokeInDisposing_DoesNotCallForeColorChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.ForeColorChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnForeColorChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithChildren_CallsForeColorChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.ForeColorChanged += handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithChildrenWithForeColor_CallsForeColorChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                ForeColor = Color.Yellow
            };
            using var child2 = new Control
            {
                ForeColor = Color.YellowGreen
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.ForeColorChanged += handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        public static IEnumerable<object[]> GiveFeedbackEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new GiveFeedbackEventArgs(DragDropEffects.None, true) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GiveFeedbackEventArgs_TestData))]
        public void Control_OnGiveFeedback_Invoke_CallsGiveFeedback(GiveFeedbackEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            GiveFeedbackEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.GiveFeedback += handler;
            control.OnGiveFeedback(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.GiveFeedback -= handler;
            control.OnGiveFeedback(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.GotFocus += handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.GotFocus -= handler;
            control.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleCreated_WithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, null };
                yield return new object[] { userPaint, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_WithHandle_TestData))]
        public void Control_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(bool userPaint, EventArgs eventArgs)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleCreated_Region_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { testData[0], new Region() };
                yield return new object[] { testData[0], new Region(new Rectangle(1, 2, 3, 4)) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_Region_TestData))]
        public void Control_OnHandleCreated_InvokeWithRegion_CallsHandleCreated(EventArgs eventArgs, Region region)
        {
            using var control = new SubControl
            {
                Region = region
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Same(region, control.Region);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Same(region, control.Region);
        }

        public static IEnumerable<object[]> OnHandleCreated_Text_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { testData[0], null, string.Empty };
                yield return new object[] { testData[0], string.Empty, string.Empty };
                yield return new object[] { testData[0], "text", "text" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleCreated_Text_TestData))]
        public void Control_OnHandleCreated_InvokeWithText_CallsHandleCreated(EventArgs eventArgs, string text, string expectedText)
        {
            using var control = new SubControl
            {
                Text = text
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_InvokeWithHandleAllowDrop_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                AllowDrop = true
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.True(control.AllowDrop);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.True(control.AllowDrop);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleDestroyed_Region_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { testData[0], new Region() };
                yield return new object[] { testData[0], new Region(new Rectangle(1, 2, 3, 4)) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleDestroyed_Region_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithRegion_CallsHandleDestroyed(EventArgs eventArgs, Region region)
        {
            using var control = new SubControl
            {
                Region = region
            };
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
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Same(region, control.Region);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Same(region, control.Region);
        }

        public static IEnumerable<object[]> OnHandleDestroyed_Text_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { testData[0], null, string.Empty };
                yield return new object[] { testData[0], string.Empty, string.Empty };
                yield return new object[] { testData[0], "text", "text" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleDestroyed_Text_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithText_CallsHandleDestroyed(EventArgs eventArgs, string text, string expectedText)
        {
            using var control = new SubControl
            {
                Text = text
            };
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
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeAllowDrop_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                AllowDrop = true
            };
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
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.AllowDrop);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.AllowDrop);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleDestroyed_Region_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleWithRegion_CallsHandleDestroyed(EventArgs eventArgs, Region region)
        {
            using var control = new SubControl
            {
                Region = region
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Same(region, control.Region);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Same(region, control.Region);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleDestroyed_Text_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleWithText_CallsHandleDestroyed(EventArgs eventArgs, string text, string expectedText)
        {
            using var control = new SubControl
            {
                Text = text
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleAllowDrop_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                AllowDrop = true
            };
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
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.True(control.AllowDrop);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.True(control.AllowDrop);
        }

        public static IEnumerable<object[]> HelpEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new HelpEventArgs(new Point(1, 2)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(HelpEventArgs_TestData))]
        public void Control_OnHelpRequested_Invoke_CallsHelpRequested(HelpEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            HelpEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HelpRequested += handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.HelpRequested -= handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(HelpEventArgs_TestData))]
        public void Control_OnHelpRequested_InvokeWithParent_CallsHelpRequested(HelpEventArgs eventArgs)
        {
            var parent = new Control();
            int parentCallCount = 0;
            HelpEventHandler parentHandler = (sender, e) => parentCallCount++;
            parent.HelpRequested += parentHandler;

            using var control = new SubControl
            {
                Parent = parent
            };
            int callCount = 0;
            HelpEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HelpRequested += handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentCallCount);

            // Remove handler.
            control.HelpRequested -= handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentCallCount);
        }

        [WinFormsFact]
        public void Control_OnHelpRequested_InvokeWithHandler_SetsHandled()
        {
            var eventArgs = new HelpEventArgs(new Point(1, 2));
            using var control = new SubControl();
            int callCount = 0;
            HelpEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HelpRequested += handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(eventArgs.Handled);

            // Remove handler.
            eventArgs.Handled = false;
            control.HelpRequested -= handler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(eventArgs.Handled);
        }

        [WinFormsFact]
        public void Control_OnHelpRequested_InvokeWithParentHandler_SetsHandled()
        {
            var parent = new Control();
            int parentCallCount = 0;
            HelpEventHandler parentHandler = (sender, e) => parentCallCount++;
            parent.HelpRequested += parentHandler;

            var eventArgs = new HelpEventArgs(new Point(1, 2));
            using var control = new SubControl
            {
                Parent = parent
            };

            // Call with handler.
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, parentCallCount);
            Assert.True(eventArgs.Handled);

            // Remove handler.
            eventArgs.Handled = false;
            parent.HelpRequested -= parentHandler;
            control.OnHelpRequested(eventArgs);
            Assert.Equal(1, parentCallCount);
            Assert.False(eventArgs.Handled);
        }

        [WinFormsFact]
        public void Control_OnHelpRequested_InvokeWithoutHandler_DoesNotSetHandled()
        {
            var eventArgs = new HelpEventArgs(new Point(1, 2));
            using var control = new SubControl();
            control.OnHelpRequested(eventArgs);
            Assert.False(eventArgs.Handled);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnImeModeChanged_Invoke_CallsImeModeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ImeModeChanged += handler;
            control.OnImeModeChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.OnImeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> InvalidateEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new InvalidateEventArgs(new Rectangle(1, 2, 3, 4)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidateEventArgs_TestData))]
        public void Control_OnInvalidated_Invoke_CallsInvalidated(InvalidateEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            InvalidateEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Invalidated += handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Invalidated -= handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnInvalidated_WithChildren_TestData()
        {
            yield return new object[] { true, Color.Empty, null };
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(Rectangle.Empty) };
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)) };
            yield return new object[] { true, Color.Red, null };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(Rectangle.Empty) };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)) };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), null };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(Rectangle.Empty) };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)) };

            yield return new object[] { false, Color.Empty, null };
            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(Rectangle.Empty) };
            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)) };
            yield return new object[] { false, Color.Red, null };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(Rectangle.Empty) };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnInvalidated_WithChildren_TestData))]
        public void Control_OnInvalidated_InvokeWithChildren_CallsInvalidated(bool supportsTransparentBackgroundColor, Color backColor, InvalidateEventArgs eventArgs)
        {
            using var child1 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var child2 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            child1.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            child1.BackColor = backColor;
            child2.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            child2.BackColor = backColor;

            int callCount = 0;
            InvalidateEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Invalidated += handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Remove handler.
            control.Invalidated -= handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidateEventArgs_TestData))]
        public void Control_OnInvalidated_InvokeWithHandle_CallsInvalidated(InvalidateEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            InvalidateEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Invalidated += handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Invalidated -= handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnInvalidated_WithChildrenWithHandle_TestData()
        {
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(Rectangle.Empty), 0 };
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)), 0 };
            yield return new object[] { true, Color.Empty, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)), 0 };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(Rectangle.Empty), 0 };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)), 0 };
            yield return new object[] { true, Color.Red, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)), 0 };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(Rectangle.Empty), 0 };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)), 0 };
            yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)), 1 };

            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(Rectangle.Empty), 0 };
            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)), 0 };
            yield return new object[] { false, Color.Empty, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)), 0 };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(Rectangle.Empty), 0 };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(new Rectangle(100, 200, 300, 400)), 0 };
            yield return new object[] { false, Color.Red, new InvalidateEventArgs(new Rectangle(1, 2, 300, 400)), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnInvalidated_WithChildrenWithHandle_TestData))]
        public void Control_OnInvalidated_InvokeWithChildrenWithHandle_CallsInvalidated(bool supportsTransparentBackgroundColor, Color backColor, InvalidateEventArgs eventArgs, int expectedChildInvalidatedCallCount)
        {
            using var child1 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var child2 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            child1.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            child1.BackColor = backColor;
            child2.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            child2.BackColor = backColor;

            int callCount = 0;
            InvalidateEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int invalidatedCallCount1 = 0;
            child1.Invalidated += (sender, e) => invalidatedCallCount1++;
            int styleChangedCallCount1 = 0;
            child1.StyleChanged += (sender, e) => styleChangedCallCount1++;
            int createdCallCount1 = 0;
            child1.HandleCreated += (sender, e) => createdCallCount1++;
            int invalidatedCallCount2 = 0;
            child2.Invalidated += (sender, e) => invalidatedCallCount2++;
            int styleChangedCallCount2 = 0;
            child2.StyleChanged += (sender, e) => styleChangedCallCount2++;
            int createdCallCount2 = 0;
            child2.HandleCreated += (sender, e) => createdCallCount2++;

            // Call with handler.
            control.Invalidated += handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.Equal(expectedChildInvalidatedCallCount, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(child2.IsHandleCreated);
            Assert.Equal(expectedChildInvalidatedCallCount, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);

            // Remove handler.
            control.Invalidated -= handler;
            control.OnInvalidated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.Equal(expectedChildInvalidatedCallCount * 2, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(child2.IsHandleCreated);
            Assert.Equal(expectedChildInvalidatedCallCount * 2, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
        }

        [WinFormsFact]
        public void Control_OnInvalidated_NullEventArgsWithChildren_ThrowsNullReferenceException()
        {
            using var child1 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var child2 = new SubControl
            {
                ClientSize = new Size(10, 20)
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);
            child1.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            child1.BackColor = Color.FromArgb(200, 50, 100, 150);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            Assert.Throws<NullReferenceException>(() => control.OnInvalidated(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void Control_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyPress += handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyUp += handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void Control_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubControl();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void Control_OnLayout_InvokeWithHandle_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
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
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Layout -= handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void Control_OnLayout_InvokeWithParent_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnLeave_Invoke_CallsLeave(KeyEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Leave += handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Leave -= handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_Invoke_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_InvokeWithHandle_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        public static IEnumerable<object[]> OnLocationChanged_HandleWithTransparentBackColor_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetEventArgsTheoryData())
            {
                yield return new object[] { true, testData[0], 1 };
                yield return new object[] { false, testData[0], 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnLocationChanged_InvokeWithHandleWithTransparentBackColor_CallsLocationChangedAndMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                locationChangedCallCount++;
            };
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.LocationChanged += locationChangedHandler;
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnLocationChanged(eventArgs);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLostFocus_Invoke_CallsLostFocus(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.LostFocus += handler;
            control.OnLostFocus(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.LostFocus -= handler;
            control.OnLostFocus(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMarginChanged_Invoke_CallsMarginChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MarginChanged += handler;
            control.OnMarginChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MarginChanged -= handler;
            control.OnMarginChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseCaptureChanged_Invoke_CallsMouseCaptureChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseCaptureChanged += handler;
            control.OnMouseCaptureChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseCaptureChanged -= handler;
            control.OnMouseCaptureChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseClick += handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseClick -= handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseDown += handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseEnter += handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseHover_Invoke_CallsMouseHover(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseHover += handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseHover -= handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseLeave += handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseMove += handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseMove -= handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseUp += handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseWheel += handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.MouseWheel -= handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_OnMouseWheel_InvokeHandledMouseEventArgs_DoesNotSetHandled()
        {
            using var control = new SubControl();
            var eventArgs = new HandledMouseEventArgs(MouseButtons.Left, 1, 2, 3, 4);
            int callCount = 0;
            MouseEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                Assert.False(eventArgs.Handled);
                callCount++;
            };
            control.MouseWheel += handler;

            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(eventArgs.Handled);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_Invoke_CallsMove(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };

            // Call with handler.
            control.Move += moveHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_InvokeWithHandle_CallsMove(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnMove_InvokeWithHandleWithTransparentBackColor_CallsMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                moveCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Move += moveHandler;
            control.Invalidated += invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Remove handler.
            control.Move -= moveHandler;
            control.Invalidated -= invalidatedHandler;
            control.OnMove(eventArgs);
            Assert.Equal(1, moveCallCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnPaddingChanged_Invoke_CallsPaddingChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.PaddingChanged += handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnPaddingChanged_WithHandle_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaddingChanged_WithHandle_TestData))]
        public void Control_OnPaddingChanged_InvokeWithHandle_CallsPaddingChanged(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.PaddingChanged += handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void Control_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnPaintBackground_TestData()
        {
            foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb), new Bitmap(10, 10, PixelFormat.Format32bppArgb) })
            {
                foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                {
                    yield return new object[] { true, Color.Empty, backgroundImage, backgroundImageLayout };
                    yield return new object[] { true, Color.Red, backgroundImage, backgroundImageLayout };
                    yield return new object[] { true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout };
                    yield return new object[] { true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout };
                    yield return new object[] { false, Color.Empty, backgroundImage, backgroundImageLayout };
                    yield return new object[] { false, Color.Red, backgroundImage, backgroundImageLayout };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_TestData))]
        public void Control_OnPaintBackground_Invoke_Success(bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
            control.BackColor = backColor;
            control.BackgroundImage = backgroundImage;
            control.BackgroundImageLayout = backgroundImageLayout;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnPaintBackground_WithParent_TestData()
        {
            var control = new Control
            {
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            var tabPage = new TabPage
            {
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            foreach (Control parent in new Control[] { control, tabPage })
            {
                foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb) })
                {
                    foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                    {
                        yield return new object[] { parent, true, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { parent, true, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { parent, true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout, 1 };
                        yield return new object[] { parent, true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout, 1 };
                        yield return new object[] { parent, false, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                        yield return new object[] { parent, false, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                    }
                }

                yield return new object[] { parent, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { parent, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { parent, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                yield return new object[] { parent, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
                yield return new object[] { parent, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
                yield return new object[] { parent, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };

                yield return new object[] { parent, true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { parent, true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { parent, true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
                yield return new object[] { parent, true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
                yield return new object[] { parent, false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
                yield return new object[] { parent, false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_WithParent_TestData))]
        public void Control_OnPaintBackground_InvokeWithParent_CallsPaint(Control parent, bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout, int expectedPaintCallCount)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 10, 20),
                Parent = parent
            };
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
            control.BackColor = backColor;
            control.BackgroundImage = backgroundImage;
            control.BackgroundImageLayout = backgroundImageLayout;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int parentCallCount = 0;
            PaintEventHandler parentHandler = (sender, e) =>
            {
                Assert.Same(parent, sender);
                Assert.NotSame(graphics, e.Graphics);
                Assert.Equal(new Rectangle(1, 2, 0, 0), e.ClipRectangle);
                parentCallCount++;
            };

            // Call with handler.
            control.Paint += handler;
            parent.Paint += parentHandler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(expectedPaintCallCount, parentCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Paint -= handler;
            parent.Paint -= parentHandler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(expectedPaintCallCount, parentCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_TestData))]
        public void Control_OnPaintBackground_InvokeWithHandle_Success(bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
            control.BackColor = backColor;
            control.BackgroundImage = backgroundImage;
            control.BackgroundImageLayout = backgroundImageLayout;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Paint += handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnPaintBackground_WithParentWithHandle_TestData()
        {
            foreach (Image backgroundImage in new Image[] { null, new Bitmap(10, 10, PixelFormat.Format32bppRgb) })
            {
                foreach (ImageLayout backgroundImageLayout in Enum.GetValues(typeof(ImageLayout)))
                {
                    yield return new object[] { true, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                    yield return new object[] { true, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                    yield return new object[] { true, Color.FromArgb(100, 50, 100, 150), backgroundImage, backgroundImageLayout, 1 };
                    yield return new object[] { true, Color.FromArgb(0, 50, 100, 150), backgroundImage, backgroundImageLayout, 1 };
                    yield return new object[] { false, Color.Empty, backgroundImage, backgroundImageLayout, 0 };
                    yield return new object[] { false, Color.Red, backgroundImage, backgroundImageLayout, 0 };
                }
            }

            yield return new object[] { true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
            yield return new object[] { true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
            yield return new object[] { true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
            yield return new object[] { true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 1 };
            yield return new object[] { false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };
            yield return new object[] { false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, 0 };

            yield return new object[] { true, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
            yield return new object[] { true, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
            yield return new object[] { true, Color.FromArgb(100, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
            yield return new object[] { true, Color.FromArgb(0, 50, 100, 150), new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 2 };
            yield return new object[] { false, Color.Empty, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
            yield return new object[] { false, Color.Red, new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaintBackground_WithParentWithHandle_TestData))]
        public void Control_OnPaintBackground_InvokeWithParentWithHandle_CallsPaint(bool supportsTransparentBackColor, Color backColor, Image backgroundImage, ImageLayout backgroundImageLayout, int expectedPaintCallCount)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            using var parent = new Control
            {
                Bounds = new Rectangle(1, 2, 30, 40)
            };
            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 10, 20),
                Parent = parent
            };
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackColor);
            control.BackColor = backColor;
            control.BackgroundImage = backgroundImage;
            control.BackgroundImageLayout = backgroundImageLayout;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int parentCallCount = 0;
            PaintEventHandler parentHandler = (sender, e) =>
            {
                Assert.Same(parent, sender);
                Assert.NotSame(graphics, e.Graphics);
                Assert.Equal(new Rectangle(1, 2, 10, 20), e.ClipRectangle);
                parentCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Paint += handler;
            parent.Paint += parentHandler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(expectedPaintCallCount, parentCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Paint -= handler;
            parent.Paint -= parentHandler;
            control.OnPaintBackground(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(expectedPaintCallCount, parentCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_OnPaintBackground_NullEventArgs_ThrowsArgumentNullException()
        {
            using var control = new SubControl();
            Assert.Throws<ArgumentNullException>(() => control.OnPaintBackground(null));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnParentBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnParentBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackColorChanged_InvokeWithBackColor_DoesNotCallBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                BackColor = Color.Red
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnParentBackColorChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnParentBackColorChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackgroundImageChanged_Invoke_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackgroundImageChanged += handler;
            control.OnParentBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.OnParentBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackgroundImageChanged_InvokeWithBackgroundImage_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var image = new Bitmap(10, 10);
            using var control = new SubControl
            {
                BackgroundImage = image
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackgroundImageChanged += handler;
            control.OnParentBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.OnParentBackgroundImageChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBindingContextChanged_Invoke_CallsBindingContextChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BindingContextChanged += handler;
            control.OnParentBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.OnParentBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBindingContextChanged_InvokeWithBindingContext_DoesNotCallBindingContextChanged(EventArgs eventArgs)
        {
            var context = new BindingContext();
            using var control = new SubControl
            {
                BindingContext = context
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BindingContextChanged += handler;
            control.OnParentBindingContextChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.OnParentBindingContextChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentCursorChanged_Invoke_CallsCursorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CursorChanged += handler;
            control.OnParentCursorChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.OnParentCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentCursorChanged_InvokeWithCursor_DoesNotCallCursorChanged(EventArgs eventArgs)
        {
            var cursor = new Cursor((IntPtr)1);
            using var control = new SubControl
            {
                Cursor = cursor
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.CursorChanged += handler;
            control.OnParentCursorChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.OnParentCursorChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnParentEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnParentEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentEnabledChanged_InvokeDisabled_DoesNotCallEnabledChanged(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                Enabled = false
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnParentEnabledChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnParentEnabledChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.FontChanged += handler;
            control.OnParentFontChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnParentFontChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentFontChanged_InvokeWithFont_DoesNotCallFontChanged(EventArgs eventArgs)
        {
            var font = new Font("Arial", 8.25f);
            using var control = new SubControl
            {
                Font = font
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
            control.OnParentFontChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.OnParentFontChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ForeColorChanged += handler;
            control.OnParentForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.OnParentForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentForeColorChanged_InvokeWithForeColor_DoesNotCallForeColorChanged(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                ForeColor = Color.Red
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ForeColorChanged += handler;
            control.OnParentForeColorChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.OnParentForeColorChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnParentVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnParentVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentVisibleChanged_InvokeDisabled_DoesNotCallVisibleChanged(EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                Visible = false
            };

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnParentVisibleChanged(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnParentVisibleChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> OnPrint_WithoutHandle_TestData()
        {
            foreach (bool opaque in new bool[] { true, false })
            {
                yield return new object[] { true, opaque, 1, false };
                yield return new object[] { false, opaque, 0, true };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPrint_WithoutHandle_TestData))]
        public void Control_OnPrint_InvokeWithoutHandle_Success(bool userPaint, bool opaque, int expectedPaintCallCount, bool expectedIsHandleCreated)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);
            control.OnPrint(eventArgs);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_OnPrint_InvokeExceptionThrownInOnPaintWithoutHandle_Success()
        {
            using var control = new SubControl();
            int paintCallCount = 0;
            control.Paint += (sender, e) =>
            {
                paintCallCount++;
                throw new DivideByZeroException();
            };

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);
            Assert.Throws<DivideByZeroException>(() => control.OnPrint(eventArgs));
            Assert.Equal(1, paintCallCount);
            Assert.False(control.IsHandleCreated);

            // Print again.
            control.OnPrint(eventArgs);
            Assert.Equal(1, paintCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnPrint_WithHandle_TestData()
        {
            foreach (bool opaque in new bool[] { true, false })
            {
                yield return new object[] { true, opaque, 1 };
                yield return new object[] { false, opaque, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPrint_WithHandle_TestData))]
        public void Control_OnPrint_InvokeWithHandle_Success(bool userPaint, bool opaque, int expectedPaintCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);
            control.OnPrint(eventArgs);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_OnPrint_InvokeExceptionThrownInOnPaintWithHandle_Success()
        {
            using var control = new SubControl();
            int paintCallCount = 0;
            control.Paint += (sender, e) =>
            {
                paintCallCount++;
                throw new DivideByZeroException();
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);
            Assert.Throws<DivideByZeroException>(() => control.OnPrint(eventArgs));
            Assert.Equal(1, paintCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Print again.
            control.OnPrint(eventArgs);
            Assert.Equal(1, paintCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_OnPrint_InvokeSetTextInPrint_CachesText()
        {
            using var control = new SubControl();

            int printCallCount = 0;
            control.Paint += (sender, e) =>
            {
                string longString = new string('a', 65536);
                control.Text = longString;
                Assert.Equal(longString, control.Text);
                printCallCount++;
            };

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Empty(control.Text);

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);
            control.OnPrint(eventArgs);
            Assert.Equal(1, printCallCount);
            Assert.Empty(control.Text);
        }

        [WinFormsFact]
        public void Control_OnPrint_InvokeNullE_ThrowsArgumentNullException()
        {
            using var control = new SubControl();
            Assert.Throws<ArgumentNullException>("e", () => control.OnPrint(null));
        }

        public static IEnumerable<object[]> QueryContinueDragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new QueryContinueDragEventArgs(0, true, DragAction.Drop) };
        }

        [WinFormsTheory]
        [MemberData(nameof(QueryContinueDragEventArgs_TestData))]
        public void Control_OnQueryContinueDrag_Invoke_CallsQueryContinueDrag(QueryContinueDragEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            QueryContinueDragEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.QueryContinueDrag += handler;
            control.OnQueryContinueDrag(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.QueryContinueDrag -= handler;
            control.OnQueryContinueDrag(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRegionChanged_Invoke_CallsRegionChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.RegionChanged += handler;
            control.OnRegionChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.RegionChanged -= handler;
            control.OnRegionChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnResize_Invoke_CallsResize(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnResize_WithHandle_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnResize_WithHandle_TestData))]
        public void Control_OnResize_InvokeWithHandle_CallsResize(bool resizeRedraw, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRightToLeftChanged_Invoke_CallsRightToLeftChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RightToLeftChanged += handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRightToLeftChanged_InvokeWithHandle_CallsRightToLeftChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RightToLeftChanged += handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(1, createdCallCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(2, createdCallCount);
        }

        [WinFormsFact]
        public void Control_OnRightToLeftChanged_InvokeInDisposing_DoesNotCallRightToLeftChanged()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.RightToLeftChanged += (sender, e) => callCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.OnRightToLeftChanged(EventArgs.Empty);
                Assert.Equal(0, callCount);
                Assert.Equal(0, createdCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRightToLeftChanged_InvokeWithChildren_CallsRightToLeftChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control
            {
                RightToLeft = RightToLeft.Inherit
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.RightToLeftChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.RightToLeftChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.RightToLeftChanged += handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRightToLeftChanged_InvokeWithChildrenWithRightToLeft_CallsRightToLeftChanged(EventArgs eventArgs)
        {
            using var child1 = new Control
            {
                RightToLeft = RightToLeft.Yes
            };
            using var child2 = new Control
            {
                RightToLeft = RightToLeft.No
            };
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.RightToLeftChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                child1CallCount++;
            };
            child2.RightToLeftChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                child2CallCount++;
            };

            // Call with handler.
            control.RightToLeftChanged += handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSizeChanged_Invoke_CallsSizeChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SizeChanged += handler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.SizeChanged -= handler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSizeChanged_Invoke_CallsSizeChangedAndResize(EventArgs eventArgs)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int resizeCallCount = 0;
            EventHandler resizeHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                resizeCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.SizeChanged += handler;
            control.Resize += resizeHandler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.SizeChanged -= handler;
            control.Resize -= resizeHandler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSizeChanged_InvokeWithResizeRedraw_CallsSizeChangedAndResizeAndInvalidate(EventArgs eventArgs)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, true);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int resizeCallCount = 0;
            EventHandler resizeHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                resizeCallCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.SizeChanged += handler;
            control.Resize += resizeHandler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.SizeChanged -= handler;
            control.Resize -= resizeHandler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnSizeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, resizeCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnStyleChanged_Invoke_CallsStyleChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.StyleChanged += handler;
            control.OnStyleChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.StyleChanged -= handler;
            control.OnStyleChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSystemColorsChanged_Invoke_CallsSystemColorsChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SystemColorsChanged += handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.SystemColorsChanged -= handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSystemColorsChanged_InvokeWithHandle_CallsSystemColorsChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
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
            control.SystemColorsChanged += handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.SystemColorsChanged -= handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnSystemColorsChanged_InvokeWithChildren_CallsSystemColorsChanged(EventArgs eventArgs)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            child1.SystemColorsChanged += (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1CallCount++;
            };
            child2.SystemColorsChanged += (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2CallCount++;
            };

            // Call with handler.
            control.SystemColorsChanged += handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Remove handler.
            control.SystemColorsChanged -= handler;
            control.OnSystemColorsChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnTabIndexChanged_Invoke_CallsTabIndexChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TabIndexChanged += handler;
            control.OnTabIndexChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.TabIndexChanged -= handler;
            control.OnTabIndexChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnTabStopChanged_Invoke_CallsTabStopChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TabStopChanged += handler;
            control.OnTabStopChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.OnTabStopChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnTextChanged_Invoke_CallsTextChanged(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.TextChanged += handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnValidated_Invoke_CallsValidated(EventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Validated += handler;
            control.OnValidated(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Validated -= handler;
            control.OnValidated(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnValidating_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new CancelEventArgs() };
            yield return new object[] { new CancelEventArgs(true) };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnValidating_TestData))]
        public void Control_OnValidating_Invoke_CallsValidating(CancelEventArgs eventArgs)
        {
            using var control = new SubControl();
            int callCount = 0;
            CancelEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Validating += handler;
            control.OnValidating(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Validating -= handler;
            control.OnValidating(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnVisibleChanged_TestData()
        {
            foreach (bool visible in new bool[] { true, false })
            {
                yield return new object[] { visible, null };
                yield return new object[] { visible, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_TestData))]
        public void Control_OnVisibleChanged_Invoke_CallsVisibleChanged(bool visible, EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                Visible = visible
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_TestData))]
        public void Control_OnVisibleChanged_InvokeWithParent_CallsVisibleChanged(bool visible, EventArgs eventArgs)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent,
                Visible = visible
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(visible, control.IsHandleCreated);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(visible, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnVisibleChanged_WithHandle_TestData()
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (bool userPaint in new bool[] { true, false })
                {
                    yield return new object[] { visible, userPaint, null };
                    yield return new object[] { visible, userPaint, new EventArgs() };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_WithHandle_TestData))]
        public void Control_OnVisibleChanged_InvokeWithHandle_CallsVisibleChangedCallsInvalidated(bool visible, bool userPaint, EventArgs eventArgs)
        {
            using var control = new SubControl
            {
                Visible = visible
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
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
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_WithHandle_TestData))]
        public void Control_OnVisibleChanged_InvokeWithParentWithHandle_CallsVisibleChanged(bool visible, bool userPaint, EventArgs eventArgs)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent,
                Visible = visible
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
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
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> OnVisibleChanged_WithChildren_TestData()
        {
            yield return new object[] { true, null, 1 };
            yield return new object[] { true, new EventArgs(), 1 };
            yield return new object[] { false, null, 0 };
            yield return new object[] { false, new EventArgs(), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_WithChildren_TestData))]
        public void Control_OnVisibleChanged_InvokeWithChildren_CallsVisibleChanged(bool visible, EventArgs eventArgs, int expectedChildCallCount)
        {
            using var grandchild1 = new Control();
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new SubControl
            {
                Visible = visible
            };
            child1.Controls.Add(grandchild1);
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int grandchildCallCount1 = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            void grandchildHandler1(object sender, EventArgs e)
            {
                Assert.Same(grandchild1, sender);
                Assert.Same(eventArgs, e);
                grandchildCallCount1++;
            };
            grandchild1.VisibleChanged += grandchildHandler1;
            void childHandler1(object sender, EventArgs e)
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            child1.VisibleChanged += childHandler1;
            void childHandler2(object sender, EventArgs e)
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };
            child2.VisibleChanged += childHandler2;

            try
            {
                // Call with handler.
                control.VisibleChanged += handler;
                control.OnVisibleChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(expectedChildCallCount, childCallCount1);
                Assert.Equal(expectedChildCallCount, childCallCount2);
                Assert.Equal(expectedChildCallCount, grandchildCallCount1);

                // Remove handler.
                control.VisibleChanged -= handler;
                control.OnVisibleChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(expectedChildCallCount * 2, childCallCount1);
                Assert.Equal(expectedChildCallCount * 2, childCallCount2);
                Assert.Equal(expectedChildCallCount * 2, grandchildCallCount1);
            }
            finally
            {
                grandchild1.VisibleChanged -= grandchildHandler1;
                child1.VisibleChanged -= childHandler1;
                child2.VisibleChanged -= childHandler2;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnVisibleChanged_TestData))]
        public void Control_OnVisibleChanged_InvokeWithChildrenNotVisible_CallsVisibleChanged(bool visible, EventArgs eventArgs)
        {
            using var grandchild1 = new Control();
            using var child1 = new Control
            {
                Visible = false
            };
            using var child2 = new Control
            {
                Visible = false
            };
            using var control = new SubControl
            {
                Visible = visible
            };
            child1.Controls.Add(grandchild1);
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int grandchildCallCount1 = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            grandchild1.VisibleChanged += (sender, e) => grandchildCallCount1++;
            child1.VisibleChanged += (sender, e) => childCallCount1++;
            child2.VisibleChanged += (sender, e) => childCallCount2++;

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
            Assert.Equal(0, grandchildCallCount1);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
            Assert.Equal(0, grandchildCallCount1);
        }
    }
}
