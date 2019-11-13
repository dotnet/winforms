// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.BackColorChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [Fact]
        public void Control_OnBackColorChanged_InvokeInDisposing_DoesNotCallBackColorChanged()
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithChildren_CallsBackColorChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.BackColorChanged += handler;
            child1.BackColorChanged += childHandler1;
            child2.BackColorChanged += childHandler2;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            child1.BackColorChanged -= childHandler1;
            child2.BackColorChanged -= childHandler2;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackColorChanged_InvokeWithChildrenWithBackColor_CallsBackColorChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                BackColor = Color.Yellow
            };
            var child2 = new Control
            {
                BackColor = Color.YellowGreen
            };
            var control = new SubControl();
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
            control.BackColorChanged += handler;
            child1.BackColorChanged += childHandler1;
            child2.BackColorChanged += childHandler2;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            child1.BackColorChanged -= childHandler1;
            child2.BackColorChanged -= childHandler2;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_Invoke_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithHandle_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Fact]
        public void Control_OnBackgroundImageChanged_InvokeInDisposing_DoesNotCallBackgroundImageChanged()
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithChildren_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageChanged_InvokeWithChildrenWithBackgroundImage_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                BackgroundImage = new Bitmap(10, 10)
            };
            var child2 = new Control
            {
                BackgroundImage = new Bitmap(10, 10)
            };
            var control = new SubControl();
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
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_Invoke_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithHandle_CallsBackgroundImageLayoutChangedAndInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithChildren_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.BackgroundImageLayoutChanged += handler;
            child1.BackgroundImageLayoutChanged += childHandler1;
            child2.BackgroundImageLayoutChanged += childHandler2;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            child1.BackgroundImageLayoutChanged -= childHandler1;
            child2.BackgroundImageLayoutChanged -= childHandler2;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBackgroundImageLayoutChanged_InvokeWithChildrenWithBackgroundImageLayout_CallsBackgroundImageLayoutChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                BackgroundImageLayout = ImageLayout.Center
            };
            var child2 = new Control
            {
                BackgroundImageLayout = ImageLayout.Center
            };
            var control = new SubControl();
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
            control.BackgroundImageLayoutChanged += handler;
            child1.BackgroundImageLayoutChanged += childHandler1;
            child2.BackgroundImageLayoutChanged += childHandler2;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            child1.BackgroundImageLayoutChanged -= childHandler1;
            child2.BackgroundImageLayoutChanged -= childHandler2;
            control.OnBackgroundImageLayoutChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Fact]
        public void Control_OnBackgroundImageLayoutChanged_InvokeInDisposing_DoesNotCallBackgroundImageLayoutChanged()
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_Invoke_CallsBindingContextChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_InvokeWithChildren_CallsBindingContextChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.BindingContextChanged += handler;
            child1.BindingContextChanged += childHandler1;
            child2.BindingContextChanged += childHandler2;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            child1.BindingContextChanged -= childHandler1;
            child2.BindingContextChanged -= childHandler2;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnBindingContextChanged_InvokeWithChildrenWithBindingContext_CallsBindingContextChanged(EventArgs eventArgs)
        {
            var childContext1 = new BindingContext();
            var childContext2 = new BindingContext();
            var child1 = new Control
            {
                BindingContext = childContext1
            };
            var child2 = new Control
            {
                BindingContext = childContext2
            };
            var control = new SubControl();
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
            control.BindingContextChanged += handler;
            child1.BindingContextChanged += childHandler1;
            child2.BindingContextChanged += childHandler2;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            child1.BindingContextChanged -= childHandler1;
            child2.BindingContextChanged -= childHandler2;
            control.OnBindingContextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCausesValidationChanged_Invoke_CallsCausesValidationChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
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

        [Theory]
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnClientSizeChanged_InvokeWithResizeRedraw_CallsClientSizeChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnContextMenuStripChanged_Invoke_CallsContextMenuStripChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Fact]
        public void Control_OnCreateControl_Invoke_Nop()
        {
            var control = new SubControl();
            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            control.OnCreateControl();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_Invoke_CallsCursorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_InvokeWithChildren_CallsCursorChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.CursorChanged += handler;
            child1.CursorChanged += childHandler1;
            child2.CursorChanged += childHandler2;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            child1.CursorChanged -= childHandler1;
            child2.CursorChanged -= childHandler2;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnCursorChanged_InvokeWithChildrenWithCursor_CallsCursorChanged(EventArgs eventArgs)
        {
            var childCursor1 = new Cursor((IntPtr)1);
            var childCursor2 = new Cursor((IntPtr)1);
            var child1 = new Control
            {
                Cursor = childCursor1
            };
            var child2 = new Control
            {
                Cursor = childCursor2
            };
            var control = new SubControl();
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
            control.CursorChanged += handler;
            child1.CursorChanged += childHandler1;
            child2.CursorChanged += childHandler2;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            child1.CursorChanged -= childHandler1;
            child2.CursorChanged -= childHandler2;
            control.OnCursorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        public static IEnumerable<object[]> DragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DragEventArgs(null, 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move) };
        }

        [Theory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragDrop_Invoke_CallsDragDrop(DragEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragEnter_Invoke_CallsDragEnter(DragEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnDragLeave_Invoke_CallsDragLeave(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void Control_OnDragOver_Invoke_CallsDragOver(DragEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithHandle_CallsEnabledChangedCallsInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithHandleNoUserPaint_CallsEnabledChangedDoesNotCallInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithChildren_CallsEnabledChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            child1.EnabledChanged += childHandler1;
            child2.EnabledChanged += childHandler2;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            child1.EnabledChanged -= childHandler1;
            child2.EnabledChanged -= childHandler2;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnabledChanged_InvokeWithChildrenDisabled_CallsEnabledChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                Enabled = false
            };
            var child2 = new Control
            {
                Enabled = false
            };
            var control = new SubControl();
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
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            child1.EnabledChanged += childHandler1;
            child2.EnabledChanged += childHandler2;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            child1.EnabledChanged -= childHandler1;
            child2.EnabledChanged -= childHandler2;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithFontHeight_CallsFontChanged(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithHandle_CallsFontChangedAndInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.FontChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.StyleChanged += styleChangedHandler;
            control.HandleCreated += createdHandler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.FontChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.StyleChanged -= styleChangedHandler;
            control.HandleCreated -= createdHandler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithHandleNoUserPaint_CallsFontChangedAndInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            int styleChangedCallCount = 0;
            EventHandler styleChangedHandler = (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            EventHandler createdHandler = (sender, e) => createdCallCount++;

            // Call with handler.
            control.FontChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.FontChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithChildren_CallsFontChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.FontChanged += handler;
            child1.FontChanged += childHandler1;
            child2.FontChanged += childHandler2;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.FontChanged -= handler;
            child1.FontChanged -= childHandler1;
            child2.FontChanged -= childHandler2;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnFontChanged_InvokeWithChildrenWithFont_CallsFontChanged(EventArgs eventArgs)
        {
            var childFont1 = new Font("Arial", 1);
            var childFont2 = new Font("Arial", 2);
            var child1 = new Control
            {
                Font = childFont1
            };
            var child2 = new Control
            {
                Font = childFont2
            };
            var control = new SubControl();
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
            control.FontChanged += handler;
            child1.FontChanged += childHandler1;
            child2.FontChanged += childHandler2;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.FontChanged -= handler;
            child1.FontChanged -= childHandler1;
            child2.FontChanged -= childHandler2;
            control.OnFontChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithHandle_CallsForeColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Fact]
        public void Control_OnForeColorChanged_InvokeInDisposing_DoesNotCallForeColorChanged()
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithChildren_CallsForeColorChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            control.ForeColorChanged += handler;
            child1.ForeColorChanged += childHandler1;
            child2.ForeColorChanged += childHandler2;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            child1.ForeColorChanged -= childHandler1;
            child2.ForeColorChanged -= childHandler2;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnForeColorChanged_InvokeWithChildrenWithForeColor_CallsForeColorChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                ForeColor = Color.Yellow
            };
            var child2 = new Control
            {
                ForeColor = Color.YellowGreen
            };
            var control = new SubControl();
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
            control.ForeColorChanged += handler;
            child1.ForeColorChanged += childHandler1;
            child2.ForeColorChanged += childHandler2;
            control.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            child1.ForeColorChanged -= childHandler1;
            child2.ForeColorChanged -= childHandler2;
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

        [Theory]
        [MemberData(nameof(GiveFeedbackEventArgs_TestData))]
        public void Control_OnGiveFeedback_Invoke_CallsGiveFeedback(GiveFeedbackEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.GetStyle(ControlStyles.UserPaint));

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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_InvokeWithHandleNoUserPaint_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(OnHandleCreated_Region_TestData))]
        public void Control_OnHandleCreated_InvokeWithRegion_CallsHandleCreated(EventArgs eventArgs, Region region)
        {
            var control = new SubControl
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

        [Theory]
        [MemberData(nameof(OnHandleCreated_Text_TestData))]
        public void Control_OnHandleCreated_InvokeWithText_CallsHandleCreated(EventArgs eventArgs, string text, string expectedText)
        {
            var control = new SubControl
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

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleCreated_InvokeWithHandleAllowDrop_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(OnHandleDestroyed_Region_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithRegion_CallsHandleDestroyed(EventArgs eventArgs, Region region)
        {
            var control = new SubControl
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

        [Theory]
        [MemberData(nameof(OnHandleDestroyed_Text_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithText_CallsHandleDestroyed(EventArgs eventArgs, string text, string expectedText)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeAllowDrop_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(OnHandleDestroyed_Region_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleWithRegion_CallsHandleDestroyed(EventArgs eventArgs, Region region)
        {
            var control = new SubControl
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

        [Theory]
        [MemberData(nameof(OnHandleDestroyed_Text_TestData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleWithText_CallsHandleDestroyed(EventArgs eventArgs, string text, string expectedText)
        {
            var control = new SubControl
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

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnHandleDestroyed_InvokeWithHandleAllowDrop_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [MemberData(nameof(HelpEventArgs_TestData))]
        public void Control_OnHelpRequested_Invoke_CallsHelpRequested(HelpEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(HelpEventArgs_TestData))]
        public void Control_OnHelpRequested_InvokeWithParent_CallsHelpRequested(HelpEventArgs eventArgs)
        {
            var parent = new Control();
            int parentCallCount = 0;
            HelpEventHandler parentHandler = (sender, e) => parentCallCount++;
            parent.HelpRequested += parentHandler;

            var control = new SubControl
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

        [Fact]
        public void Control_OnHelpRequested_InvokeWithHandler_SetsHandled()
        {
            var eventArgs = new HelpEventArgs(new Point(1, 2));
            var control = new SubControl();
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

        [Fact]
        public void Control_OnHelpRequested_InvokeWithParentHandler_SetsHandled()
        {
            var parent = new Control();
            int parentCallCount = 0;
            HelpEventHandler parentHandler = (sender, e) => parentCallCount++;
            parent.HelpRequested += parentHandler;

            var eventArgs = new HelpEventArgs(new Point(1, 2));
            var control = new SubControl
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

        [Fact]
        public void Control_OnHelpRequested_InvokeWithoutHandler_DoesNotSetHandled()
        {
            var eventArgs = new HelpEventArgs(new Point(1, 2));
            var control = new SubControl();
            control.OnHelpRequested(eventArgs);
            Assert.False(eventArgs.Handled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnImeModeChanged_Invoke_CallsImeModeChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void Control_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void Control_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
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
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_Invoke_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnLocationChanged_InvokeWithHandle_CallsLocationChangedAndMove(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnLocationChanged_InvokeWithHandleWithTransparentBackColor_CallsLocationChangedAndMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseClick_Invoke_CallsMouseClick(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseClick -= handler;
            control.OnMouseClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseDoubleClick -= handler;
            control.OnMouseDoubleClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseDown_Invoke_CallsMouseDown(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseDown -= handler;
            control.OnMouseDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseEnter_Invoke_CallsMouseEnter(EventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseEnter -= handler;
            control.OnMouseEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseHover_Invoke_CallsMouseHover(EventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseHover -= handler;
            control.OnMouseHover(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMouseLeave_Invoke_CallsMouseLeave(EventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseLeave -= handler;
            control.OnMouseLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseMove_Invoke_CallsMouseMove(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseMove -= handler;
            control.OnMouseMove(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseUp_Invoke_CallsMouseUp(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseUp -= handler;
            control.OnMouseUp(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void Control_OnMouseWheel_Invoke_CallsMouseWheel(MouseEventArgs eventArgs)
        {
            var control = new SubControl();
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

            // Remove handler.
            control.MouseWheel -= handler;
            control.OnMouseWheel(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Control_OnMouseWheel_InvokeHandledMouseEventArgs_DoesNotSetHandled()
        {
            var control = new SubControl();
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
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_Invoke_CallsMove(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnMove_InvokeWithHandle_CallsMove(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [MemberData(nameof(OnLocationChanged_HandleWithTransparentBackColor_TestData))]
        public void Control_OnMove_InvokeWithHandleWithTransparentBackColor_CallsMoveAndInvalidated(bool supportsTransparentBackgroundColor, EventArgs eventArgs, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
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

        [Theory]
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

        [Theory]
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void Control_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackColorChanged_InvokeWithBackColor_DoesNotCallBackColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackgroundImageChanged_Invoke_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBackgroundImageChanged_InvokeWithBackgroundImage_CallsBackgroundImageChanged(EventArgs eventArgs)
        {
            var image = new Bitmap(10, 10);
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBindingContextChanged_Invoke_CallsBindingContextChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentBindingContextChanged_InvokeWithBindingContext_DoesNotCallBindingContextChanged(EventArgs eventArgs)
        {
            var context = new BindingContext();
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentCursorChanged_Invoke_CallsCursorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentCursorChanged_InvokeWithCursor_DoesNotCallCursorChanged(EventArgs eventArgs)
        {
            var cursor = new Cursor((IntPtr)1);
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentEnabledChanged_InvokeDisabled_DoesNotCallEnabledChanged(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentFontChanged_Invoke_CallsFontChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentFontChanged_InvokeWithFont_DoesNotCallFontChanged(EventArgs eventArgs)
        {
            var font = new Font("Arial", 8.25f);
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentForeColorChanged_InvokeWithForeColor_DoesNotCallForeColorChanged(EventArgs eventArgs)
        {
            var control = new SubControl
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnParentVisibleChanged_InvokeDisabled_DoesNotCallVisibleChanged(EventArgs eventArgs)
        {
            var control = new SubControl
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


        public static IEnumerable<object[]> QueryContinueDragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new QueryContinueDragEventArgs(0, true, DragAction.Drop) };
        }

        [Theory]
        [MemberData(nameof(QueryContinueDragEventArgs_TestData))]
        public void Control_OnQueryContinueDrag_Invoke_CallsQueryContinueDrag(QueryContinueDragEventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnRegionChanged_Invoke_CallsRegionChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
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
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Theory]
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

        [Theory]
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

        [Theory]
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnStyleChanged_Invoke_CallsStyleChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnTextChanged_Invoke_CallsTextChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.VisibleChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_InvokeWithHandle_CallsVisibleChangedCallsInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.VisibleChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_InvokeWithHandleNoUserPaint_CallsVisibleChangedDoesNotCallInvalidated(EventArgs eventArgs)
        {
            var control = new SubControl();
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
            control.VisibleChanged += handler;
            control.Invalidated += invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_InvokeWithChildren_CallsVisibleChanged(EventArgs eventArgs)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubControl();
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
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            child1.VisibleChanged += childHandler1;
            child2.VisibleChanged += childHandler2;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.VisibleChanged -= handler;
            child1.VisibleChanged -= childHandler1;
            child2.VisibleChanged -= childHandler2;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void Control_OnVisibleChanged_InvokeWithChildrenNotVisible_CallsVisibleChanged(EventArgs eventArgs)
        {
            var child1 = new Control
            {
                Visible = false
            };
            var child2 = new Control
            {
                Visible = false
            };
            var control = new SubControl();
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
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(eventArgs, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(eventArgs, e);
                childCallCount2++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            child1.VisibleChanged += childHandler1;
            child2.VisibleChanged += childHandler2;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.VisibleChanged -= handler;
            child1.VisibleChanged -= childHandler1;
            child2.VisibleChanged -= childHandler2;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }
    }
}
