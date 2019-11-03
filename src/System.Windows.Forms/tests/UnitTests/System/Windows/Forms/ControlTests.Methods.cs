// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public partial class ControlTests
    {
        public static IEnumerable<object[]> AccessibilityNotifyClients_AccessibleEvents_Int_TestData()
        {
            yield return new object[] { AccessibleEvents.DescriptionChange, int.MinValue };
            yield return new object[] { AccessibleEvents.DescriptionChange, -1 };
            yield return new object[] { AccessibleEvents.DescriptionChange, 0 };
            yield return new object[] { AccessibleEvents.DescriptionChange, 1 };
            yield return new object[] { AccessibleEvents.DescriptionChange, int.MaxValue };
            yield return new object[] { (AccessibleEvents)0, int.MinValue };
            yield return new object[] { (AccessibleEvents)0, -1 };
            yield return new object[] { (AccessibleEvents)0, 0 };
            yield return new object[] { (AccessibleEvents)0, 1 };
            yield return new object[] { (AccessibleEvents)0, int.MaxValue };
            yield return new object[] { (AccessibleEvents)int.MaxValue, int.MinValue };
            yield return new object[] { (AccessibleEvents)int.MaxValue, -1 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, 0 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, 1 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, int.MaxValue };
        }

        [Theory]
        [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_TestData))]
        public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntWithoutHandle_Nop(AccessibleEvents accEvent, int childID)
        {
            var control = new SubControl();
            control.AccessibilityNotifyClients(accEvent, childID);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.AccessibilityNotifyClients(accEvent, childID);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_TestData))]
        public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntWithHandle_Success(AccessibleEvents accEvent, int childID)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.AccessibilityNotifyClients(accEvent, childID);
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.AccessibilityNotifyClients(accEvent, childID);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AccessibilityNotifyClients_AccessibleEvents_Int_Int_TestData()
        {
            yield return new object[] { AccessibleEvents.DescriptionChange, int.MaxValue, int.MinValue };
            yield return new object[] { AccessibleEvents.DescriptionChange, 1, -1 };
            yield return new object[] { AccessibleEvents.DescriptionChange, 0, 0 };
            yield return new object[] { AccessibleEvents.DescriptionChange, -1, 1 };
            yield return new object[] { AccessibleEvents.DescriptionChange, int.MinValue, int.MaxValue };
            yield return new object[] { (AccessibleEvents)0, int.MaxValue, int.MinValue };
            yield return new object[] { (AccessibleEvents)0, 1, -1 };
            yield return new object[] { (AccessibleEvents)0, 0, 0 };
            yield return new object[] { (AccessibleEvents)0, -1, 1 };
            yield return new object[] { (AccessibleEvents)0, int.MinValue, int.MaxValue };
            yield return new object[] { (AccessibleEvents)int.MaxValue, int.MaxValue, int.MinValue };
            yield return new object[] { (AccessibleEvents)int.MaxValue, 1, -1 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, 0, 0 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, 1, 1 };
            yield return new object[] { (AccessibleEvents)int.MaxValue, int.MinValue, int.MaxValue };
        }

        [Theory]
        [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_Int_TestData))]
        public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntIntWithoutHandle_Nop(AccessibleEvents accEvent, int objectID, int childID)
        {
            var control = new SubControl();
            control.AccessibilityNotifyClients(accEvent, objectID, childID);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.AccessibilityNotifyClients(accEvent, objectID, childID);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_Int_TestData))]
        public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntIntWithHandle_Success(AccessibleEvents accEvent, int objectID, int childID)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.AccessibilityNotifyClients(accEvent, objectID, childID);
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.AccessibilityNotifyClients(accEvent, objectID, childID);
            Assert.True(control.IsHandleCreated);
        }

        [Fact]
        public void Control_ControlAddedAndRemoved()
        {
            bool wasAdded = false;
            bool wasRemoved = false;
            var cont = new Control();
            cont.ControlAdded += (sender, args) => wasAdded = true;
            cont.ControlRemoved += (sender, args) => wasRemoved = true;
            var child = new Control();

            cont.Controls.Add(child);
            cont.Controls.Remove(child);

            Assert.True(wasAdded);
            Assert.True(wasRemoved);
        }

        [Fact]
        public void Control_CreateControl_Invoke_Success()
        {
            var control = new SubControl();
            Assert.True(control.GetStyle(ControlStyles.UserPaint));

            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            IntPtr handle1 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle1);

            // Call again.
            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            IntPtr handle2 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.Equal(handle1, handle2);
        }

        [Fact]
        public void Control_CreateControl_InvokeNoUserPaint_Success()
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, false);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        public static IEnumerable<object[]> CreateControl_Region_TestData()
        {
            yield return new object[] { new Region() };
            yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
        }

        [Theory]
        [MemberData(nameof(CreateControl_Region_TestData))]
        public void Control_CreateControl_InvokeWithRegion_Success(Region region)
        {
            var control = new SubControl
            {
                Region = region
            };

            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Same(region, control.Region);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_CreateControl_InvokeWithText_Success(string text, string expectedText)
        {
            var control = new SubControl
            {
                Text = text
            };

            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expectedText, control.Text);
        }

        [StaFact]
        public void Control_CreateControl_InvokeAllowDrop_Success()
        {
            var control = new SubControl
            {
                AllowDrop = true
            };

            control.CreateControl();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.AllowDrop);
        }

        [Fact]
        public void Control_CreateControl_InvokeWithParent_Success()
        {
            var parent = new Control();
            var control = new SubControl
            {
                Parent = parent
            };
            control.CreateControl();
            Assert.False(parent.Created);
            Assert.False(parent.IsHandleCreated);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateControl_InvokeWithChildren_Success()
        {
            var parent = new SubControl();
            var control = new SubControl
            {
                Parent = parent
            };
            parent.CreateControl();
            Assert.True(parent.Created);
            Assert.True(parent.IsHandleCreated);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateControl_InvokeWithHandler_CallsHandleCreated()
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.HandleCreated += handler;

            control.CreateControl();
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateControl_InvokeDisposed_ThrowsObjectDisposedException()
        {
            var control = new SubControl();
            control.Dispose();
            Assert.Throws<ObjectDisposedException>(() => control.CreateControl());
        }

        [WinFormsFact]
        public void Control_CreateAccessibilityInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubControl();
            Control.ControlAccessibleObject accessibleObject = Assert.IsType<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.Same(control, accessibleObject.Owner);
            Assert.NotSame(accessibleObject, control.CreateAccessibilityInstance());
        }

        [WinFormsFact]
        public void Control_CreateControlsInstance_Invoke_ReturnsExpected()
        {
            using var control = new SubControl();
            Control.ControlCollection controls = Assert.IsType<Control.ControlCollection>(control.CreateControlsInstance());
            Assert.Empty(controls);
            Assert.Same(control, controls.Owner);
            Assert.False(controls.IsReadOnly);
            Assert.NotSame(controls, control.CreateControlsInstance());
        }

        [Fact]
        public void Control_CreateHandle_Invoke_Success()
        {
            var control = new SubControl();
            Assert.True(control.GetStyle(ControlStyles.UserPaint));

            control.CreateHandle();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateHandle_InvokeNoUserPaint_Success()
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, false);
            Assert.False(control.GetStyle(ControlStyles.UserPaint));

            control.CreateHandle();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        public static IEnumerable<object[]> CreateHandle_Region_TestData()
        {
            yield return new object[] { new Region() };
            yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
        }

        [Theory]
        [MemberData(nameof(CreateHandle_Region_TestData))]
        public void Control_CreateHandle_InvokeWithRegion_Success(Region region)
        {
            var control = new SubControl
            {
                Region = region
            };

            control.CreateHandle();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Same(region, control.Region);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_CreateHandle_InvokeWithText_Success(string text, string expectedText)
        {
            var control = new SubControl
            {
                Text = text
            };

            control.CreateHandle();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expectedText, control.Text);
        }

        [StaFact]
        public void Control_CreateHandle_InvokeAllowDrop_Success()
        {
            var control = new SubControl
            {
                AllowDrop = true
            };

            control.CreateHandle();
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(control.AllowDrop);
        }

        [Fact]
        public void Control_CreateHandle_InvokeWithParent_Success()
        {
            var parent = new Control();
            var control = new SubControl
            {
                Parent = parent
            };
            control.CreateHandle();
            Assert.False(parent.Created);
            Assert.False(parent.IsHandleCreated);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateHandle_InvokeWithChildren_Success()
        {
            var parent = new SubControl();
            var control = new SubControl
            {
                Parent = parent
            };
            parent.CreateHandle();
            Assert.True(parent.Created);
            Assert.True(parent.IsHandleCreated);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateHandle_InvokeWithHandler_CallsHandleCreated()
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.HandleCreated += handler;

            control.CreateHandle();
            Assert.Equal(1, callCount);
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        [Fact]
        public void Control_CreateHandle_InvokeDisposed_ThrowsObjectDisposedException()
        {
            var control = new SubControl();
            control.Dispose();
            Assert.Throws<ObjectDisposedException>(() => control.CreateHandle());
        }

        [WinFormsFact]
        public void Control_CreateHandle_InvokeAlreadyCreated_ThrowsInvalidOperationException()
        {
            var control = new SubControl();
            control.CreateHandle();
            Assert.Throws<InvalidOperationException>(() => control.CreateHandle());
        }

        [Fact]
        public void Control_Contains()
        {
            var cont = new Control();
            var child = new Control();
            cont.Controls.Add(child);

            // act and assert
            Assert.True(cont.Contains(child));
        }

        [Fact]
        public void Control_ContainsGrandchild()
        {
            var cont = new Control();
            var child = new Control();
            var grandchild = new Control();
            cont.Controls.Add(child);
            child.Controls.Add(grandchild);

            // act and assert
            Assert.True(cont.Contains(grandchild));
        }

        [Fact]
        public void Control_ContainsNot()
        {
            var cont = new Control();
            var child = new Control();

            // act and assert
            Assert.False(cont.Contains(child));
        }

        [Fact]
        public void Control_DestroyHandle_InvokeWithHandle_Success()
        {
            var control = new SubControl();
            IntPtr handle1 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle1);

            control.DestroyHandle();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            IntPtr handle2 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle2, handle1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_CreateControl_InvokeWithHandleWithText_Success(string text, string expectedText)
        {
            var control = new SubControl
            {
                Text = text
            };

            IntPtr handle1 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.Equal(expectedText, control.Text);

            control.DestroyHandle();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedText, control.Text);

            IntPtr handle2 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle2, handle1);
            Assert.Equal(expectedText, control.Text);
        }

        public static IEnumerable<object[]> DestroyHandle_Region_TestData()
        {
            yield return new object[] { new Region() };
            yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
        }

        [Theory]
        [MemberData(nameof(CreateHandle_Region_TestData))]
        public void Control_DestroyHandle_InvokeWithRegion_Success(Region region)
        {
            var control = new SubControl
            {
                Region = region
            };

            IntPtr handle1 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.Same(region, control.Region);

            control.DestroyHandle();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.Same(region, control.Region);

            IntPtr handle2 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle2, handle1);
            Assert.Same(region, control.Region);
        }

        [StaFact]
        public void Control_DestroyHandle_InvokeWithHandleAllowDrop_Success()
        {
            var control = new SubControl
            {
                AllowDrop = true
            };

            IntPtr handle1 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.True(control.AllowDrop);

            control.DestroyHandle();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.AllowDrop);

            IntPtr handle2 = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle2, handle1);
            Assert.True(control.AllowDrop);
        }

        [Fact]
        public void Control_DestroyHandle_InvokeWithoutHandle_Nop()
        {
            var control = new SubControl();
            control.DestroyHandle();
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void Control_DestroyHandle_InvokeWithHandler_CallsHandleDestroyed()
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.HandleDestroyed += handler;

            control.DestroyHandle();
            Assert.Equal(0, callCount);

            IntPtr handle = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle);

            control.DestroyHandle();
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            control.DestroyHandle();
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            handle = control.Handle;
            Assert.True(control.Created);
            Assert.True(control.IsHandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle);

            control.HandleDestroyed -= handler;
            control.DestroyHandle();
            Assert.Equal(1, callCount);
            Assert.False(control.Created);
            Assert.False(control.IsHandleCreated);
        }

        /// <summary>
        ///  Data for the DoDragDrop test
        /// </summary>
        public static TheoryData<DragDropEffects> DoDragDropData =>
            CommonTestHelper.GetEnumTheoryData<DragDropEffects>();

        [Theory]
        [MemberData(nameof(DoDragDropData))]
        public void Control_DoDragDrop(DragDropEffects expected)
        {
            var cont = new Control();
            var mock = new Mock<IDataObject>(MockBehavior.Strict);

            DragDropEffects ret = cont.DoDragDrop(mock.Object, expected);

            Assert.Equal(DragDropEffects.None, ret);
        }

        [Fact]
        public void Control_FindFormWithParent_ReturnsForm()
        {
            var control = new Control();
            var form = new Form();
            control.Parent = form;
            Assert.Equal(form, control.FindForm());
        }

        [Fact]
        public void Control_FindFormWithoutParent_ReturnsNull()
        {
            var control = new Control();
            Assert.Null(control.FindForm());
        }

        [WinFormsFact]
        public void Control_FromChildHandle_InvokeControlHandle_ReturnsExpected()
        {
            using var control = new SubControl();
            IntPtr handle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Same(control, Control.FromChildHandle(handle));

            // Get when destroyed.
            control.DestroyHandle();
            Assert.Null(Control.FromChildHandle(handle));
        }

        [WinFormsFact]
        public void Control_FromChildHandle_InvokeNativeWindowHandle_ReturnsExpected()
        {
            var window = new NativeWindow();
            window.CreateHandle(new CreateParams());
            IntPtr handle = window.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Null(Control.FromChildHandle(handle));

            // Get when destroyed.
            window.DestroyHandle();
            Assert.Null(Control.FromChildHandle(handle));
        }

        [WinFormsFact]
        public void Control_FromChildHandle_InvokeChildHandle_ReturnsExpected()
        {
            using var parent = new SubControl();
            using var control = new SubControl
            {
                Parent = parent
            };
            IntPtr parentHandle = parent.Handle;
            Assert.NotEqual(IntPtr.Zero, parentHandle);
            Assert.True(control.IsHandleCreated);

            var window = new NativeWindow();
            window.CreateHandle(control.CreateParams);
            Assert.Same(parent, Control.FromChildHandle(window.Handle));

            // Get when destroyed.
            window.DestroyHandle();
            Assert.Null(Control.FromChildHandle(window.Handle));
        }

        [WinFormsFact]
        public void Control_FromChildHandle_InvokeNoSuchControl_ReturnsNull()
        {
            Assert.Null(Control.FromChildHandle(IntPtr.Zero));
            Assert.Null(Control.FromChildHandle((IntPtr)1));
        }

        [WinFormsFact]
        public void Control_FromHandle_InvokeControlHandle_ReturnsExpected()
        {
            using var control = new SubControl();
            IntPtr handle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Same(control, Control.FromHandle(handle));

            // Get when destroyed.
            control.DestroyHandle();
            Assert.Null(Control.FromHandle(handle));
        }

        [WinFormsFact]
        public void Control_FromHandle_InvokeNativeWindowHandle_ReturnsExpected()
        {
            var window = new NativeWindow();
            window.CreateHandle(new CreateParams());
            IntPtr handle = window.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Null(Control.FromHandle(handle));

            // Get when destroyed.
            window.DestroyHandle();
            Assert.Null(Control.FromHandle(handle));
        }

        [WinFormsFact]
        public void Control_FromHandle_InvokeChildHandle_ReturnsExpected()
        {
            using var parent = new SubControl();
            using var control = new SubControl
            {
                Parent = parent
            };
            IntPtr parentHandle = parent.Handle;
            Assert.NotEqual(IntPtr.Zero, parentHandle);
            Assert.True(control.IsHandleCreated);

            var window = new NativeWindow();
            window.CreateHandle(control.CreateParams);
            Assert.Null(Control.FromHandle(window.Handle));

            // Get when destroyed.
            window.DestroyHandle();
            Assert.Null(Control.FromHandle(window.Handle));
        }

        [WinFormsFact]
        public void Control_FromHandle_InvokeNoSuchControl_ReturnsNull()
        {
            Assert.Null(Control.FromHandle(IntPtr.Zero));
            Assert.Null(Control.FromHandle((IntPtr)1));
        }

        // TODO: create a focus test that returns true when a handle has been created
        [Fact]
        public void Control_FocusHandleNotCreated()
        {
            var cont = new Control();

            var ret = cont.Focus();

            Assert.False(ret);
        }

        [WinFormsFact]
        public void Control_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubControl();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());

            // Call again to tets caching.
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        /// <summary>
        ///  Data for the GetChildAtPointNull test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointNullData =>
            CommonTestHelper.GetEnumTheoryData<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointNullData))]
        public void Control_GetChildAtPointNull(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            Control ret = cont.GetChildAtPoint(new Point(5, 5), skip);

            Assert.Null(ret);
        }

        /// <summary>
        ///  Data for the GetChildAtPointInvalid test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointInvalidData))]
        public void Control_GetChildAtPointInvalid(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            // act & assert
            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.GetChildAtPoint(new Point(5, 5), skip));
            Assert.Equal("skipValue", ex.ParamName);
        }

        [WinFormsFact]
        public void Control_GetContainerControl_GetWithParent_ReturnsNull()
        {
            using var grandparent = new Control();
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Null(control.GetContainerControl());
            Assert.Null(parent.GetContainerControl());
            Assert.Null(grandparent.GetContainerControl());
        }

        [WinFormsFact]
        public void Control_GetContainerControl_GetWithContainerControlParent_ReturnsExpected()
        {
            using var greatGrandparent = new ContainerControl();
            using var grandparent = new ContainerControl
            {
                Parent = greatGrandparent
            };
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Same(grandparent, control.GetContainerControl());
            Assert.Same(grandparent, parent.GetContainerControl());
            Assert.Same(grandparent, grandparent.GetContainerControl());
        }

        [WinFormsFact]
        public void Control_GetContainerControl_GetWithSplitContainerParent_ReturnsExpected()
        {
            using var greatGrandparent = new ContainerControl();
            using var grandparent = new SplitContainer
            {
                Parent = greatGrandparent
            };
            using var control = new Control
            {
                Parent = grandparent.Panel1
            };
            Assert.Same(grandparent, control.GetContainerControl());
            Assert.Same(grandparent, grandparent.Panel1.GetContainerControl());
            Assert.Same(greatGrandparent, grandparent.GetContainerControl());
        }

        [WinFormsFact]
        public void Control_GetContainerControl_GetWithInvalidContainerControlParent_ReturnsExpected()
        {
            using var greatGrandparent = new ContainerControl();
            using var grandparent = new SubContainerControl
            {
                Parent = greatGrandparent
            };
            grandparent.SetStyle(ControlStyles.ContainerControl, false);
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Same(greatGrandparent, control.GetContainerControl());
            Assert.Same(greatGrandparent, parent.GetContainerControl());
            Assert.Same(greatGrandparent, grandparent.GetContainerControl());
        }

        [WinFormsFact]
        public void Control_GetContainerControl_GetWithFakeContainerControlParent_ReturnsExpected()
        {
            using var greatGrandparent = new ContainerControl();
            using var grandparent = new SubControl
            {
                Parent = greatGrandparent
            };
            grandparent.SetStyle(ControlStyles.ContainerControl, true);
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Same(greatGrandparent, control.GetContainerControl());
            Assert.Same(greatGrandparent, parent.GetContainerControl());
            Assert.Same(greatGrandparent, grandparent.GetContainerControl());
        }

        private class SubContainerControl : ContainerControl
        {
            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }

        [Fact]
        public void Control_GetNextControl()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(second, cont.GetNextControl(first, true));
        }

        [Fact]
        public void Control_GetNextControlReverse()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, cont.GetNextControl(second, false));
        }

        [Fact]
        public void Control_GetNextControlNoNext()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(third, true));
        }

        [Fact]
        public void Control_GetNextControlNoNextReverse()
        {
            var cont = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(first, false));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void Control_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubControl();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void Control_GetTopLevel_Invoke_ReturnsFalse()
        {
            using var control = new SubControl();
            Assert.False(control.GetTopLevel());

            // Call again to test caching.
            Assert.False(control.GetTopLevel());
        }

        [Fact]
        public void Control_Hide_Invoke_SetsInvisible()
        {
            var control = new Control
            {
                Visible = true
            };
            control.Hide();
            Assert.False(control.Visible);

            // Hide again.
            control.Hide();
            Assert.False(control.Visible);
        }

        [Fact]
        public void Control_Hide_InvokeWithHandler_CallsVisibleChanged()
        {
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Call again.
            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = true;
            control.Hide();
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        public void Control_IsInputChar_InvokeWithoutHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubControl();
            Assert.Equal(expected, control.IsInputChar((char)keyData));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        public void Control_IsInputChar_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.IsInputChar((char)keyData));
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> IsInputKey_TestData()
        {
            yield return new object[] { Keys.Tab, false };
            yield return new object[] { Keys.Up, false };
            yield return new object[] { Keys.Down, false };
            yield return new object[] { Keys.Left, false };
            yield return new object[] { Keys.Right, false };
            yield return new object[] { Keys.Return, false };
            yield return new object[] { Keys.Escape, false };
            yield return new object[] { Keys.A, false };
            yield return new object[] { Keys.C, false };
            yield return new object[] { Keys.Insert, false };
            yield return new object[] { Keys.Space, false };
            yield return new object[] { Keys.Home, false };
            yield return new object[] { Keys.End, false }; ;
            yield return new object[] { Keys.Back, false };
            yield return new object[] { Keys.Next, false };
            yield return new object[] { Keys.Prior, false };
            yield return new object[] { Keys.Delete, false };
            yield return new object[] { Keys.D0, false };
            yield return new object[] { Keys.NumPad0, false };
            yield return new object[] { Keys.F1, false };
            yield return new object[] { Keys.F2, false };
            yield return new object[] { Keys.F3, false };
            yield return new object[] { Keys.F4, false };
            yield return new object[] { Keys.F10, false };
            yield return new object[] { Keys.RButton, false };
            yield return new object[] { Keys.PageUp, false };
            yield return new object[] { Keys.PageDown, false };
            yield return new object[] { Keys.Menu, false };
            yield return new object[] { Keys.None, false };

            yield return new object[] { Keys.Control | Keys.Tab, false };
            yield return new object[] { Keys.Control | Keys.Up, false };
            yield return new object[] { Keys.Control | Keys.Down, false };
            yield return new object[] { Keys.Control | Keys.Left, false };
            yield return new object[] { Keys.Control | Keys.Right, false };
            yield return new object[] { Keys.Control | Keys.Return, false };
            yield return new object[] { Keys.Control | Keys.Escape, false };
            yield return new object[] { Keys.Control | Keys.A, false };
            yield return new object[] { Keys.Control | Keys.C, false };
            yield return new object[] { Keys.Control | Keys.Insert, false };
            yield return new object[] { Keys.Control | Keys.Space, false };
            yield return new object[] { Keys.Control | Keys.Home, false };
            yield return new object[] { Keys.Control | Keys.End, false }; ;
            yield return new object[] { Keys.Control | Keys.Back, false };
            yield return new object[] { Keys.Control | Keys.Next, false };
            yield return new object[] { Keys.Control | Keys.Prior, false };
            yield return new object[] { Keys.Control | Keys.Delete, false };
            yield return new object[] { Keys.Control | Keys.D0, false };
            yield return new object[] { Keys.Control | Keys.NumPad0, false };
            yield return new object[] { Keys.Control | Keys.F1, false };
            yield return new object[] { Keys.Control | Keys.F2, false };
            yield return new object[] { Keys.Control | Keys.F3, false };
            yield return new object[] { Keys.Control | Keys.F4, false };
            yield return new object[] { Keys.Control | Keys.F10, false };
            yield return new object[] { Keys.Control | Keys.RButton, false };
            yield return new object[] { Keys.Control | Keys.PageUp, false };
            yield return new object[] { Keys.Control | Keys.PageDown, false };
            yield return new object[] { Keys.Control | Keys.Menu, false };
            yield return new object[] { Keys.Control | Keys.None, false };

            yield return new object[] { Keys.Alt | Keys.Tab, false };
            yield return new object[] { Keys.Alt | Keys.Up, false };
            yield return new object[] { Keys.Alt | Keys.Down, false };
            yield return new object[] { Keys.Alt | Keys.Left, false };
            yield return new object[] { Keys.Alt | Keys.Right, false };
            yield return new object[] { Keys.Alt | Keys.Return, false };
            yield return new object[] { Keys.Alt | Keys.Escape, false };
            yield return new object[] { Keys.Alt | Keys.A, false };
            yield return new object[] { Keys.Alt | Keys.C, false };
            yield return new object[] { Keys.Alt | Keys.Insert, false };
            yield return new object[] { Keys.Alt | Keys.Space, false };
            yield return new object[] { Keys.Alt | Keys.Home, false };
            yield return new object[] { Keys.Alt | Keys.End, false }; ;
            yield return new object[] { Keys.Alt | Keys.Back, false };
            yield return new object[] { Keys.Alt | Keys.Next, false };
            yield return new object[] { Keys.Alt | Keys.Prior, false };
            yield return new object[] { Keys.Alt | Keys.Delete, false };
            yield return new object[] { Keys.Alt | Keys.D0, false };
            yield return new object[] { Keys.Alt | Keys.NumPad0, false };
            yield return new object[] { Keys.Alt | Keys.F1, false };
            yield return new object[] { Keys.Alt | Keys.F2, false };
            yield return new object[] { Keys.Alt | Keys.F3, false };
            yield return new object[] { Keys.Alt | Keys.F4, false };
            yield return new object[] { Keys.Alt | Keys.F10, false };
            yield return new object[] { Keys.Alt | Keys.RButton, false };
            yield return new object[] { Keys.Alt | Keys.PageUp, false };
            yield return new object[] { Keys.Alt | Keys.PageDown, false };
            yield return new object[] { Keys.Alt | Keys.Menu, false };
            yield return new object[] { Keys.Alt | Keys.None, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        public void Control_IsInputKey_InvokeWithoutHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubControl();
            Assert.Equal(expected, control.IsInputKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(IsInputKey_TestData))]
        public void Control_IsInputKey_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.IsInputKey(keyData));
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> PreProcessMessage_TestData()
        {
            yield return new object[] { 0, Keys.None, false };
            yield return new object[] { 0, Keys.A, false };
            yield return new object[] { 0, Keys.Tab, false };
            yield return new object[] { 0, Keys.Menu, false };
            yield return new object[] { 0, Keys.F10, false };

            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.Tab, true };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.Menu, true };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.F10, true };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.Tab, true };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.Menu, true };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.F10, true };

            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.Tab, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.Menu, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.F10, false };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.Tab, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.Menu, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.F10, false };

            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.None, true };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.A, true };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.Tab, true };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.Menu, true };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.F10, true };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.Tab, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.Menu, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.F10, false };

            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.Tab, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.Menu, false };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.F10, false };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.None, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.A, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.Tab, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.Menu, false };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.F10, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(PreProcessMessage_TestData))]
        public void Control_PreProcessMessage_Invoke_ReturnsExpected(int windowMsg, Keys keys, bool expectedIsHandleCreated)
        {
            using var control = new SubControl();
            var msg = new Message
            {
                Msg = windowMsg,
                WParam = (IntPtr)keys
            };
            Assert.False(control.PreProcessMessage(ref msg));
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(PreProcessMessage_TestData))]
        public void Control_PreProcessMessage_InvokeWithParent_ReturnsExpected(int windowMsg, Keys keys, bool expectedIsHandleCreated)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            var msg = new Message
            {
                Msg = windowMsg,
                WParam = (IntPtr)keys
            };
            Assert.False(control.PreProcessMessage(ref msg));
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
        }

        public static IEnumerable<object[]> PreProcessMessage_CustomProcessCmdKeyParent_TestData()
        {
            yield return new object[] { 0, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
            yield return new object[] { 0, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 0 };

            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.None, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.None, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.None, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.None, false, false, false, false, false, false, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.A, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.A, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.A, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, Keys.A, false, false, false, false, false, false, 1, 1, 1, 0, 0 };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.None, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.None, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.None, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.None, false, false, false, false, false, false, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.A, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.A, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.A, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, Keys.A, false, false, false, false, false, false, 1, 1, 1, 0, 0 };

            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.None, false, false, false, true, false, false, 0, 0, 0, 1, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.None, false, false, false, true, true, false, 0, 0, 0, 1, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.None, false, false, false, false, true, true, 0, 0, 0, 1, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.None, false, false, false, false, false, false, 0, 0, 0, 1, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.A, false, false, false, true, false, false, 0, 0, 0, 1, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.A, false, false, false, true, true, false, 0, 0, 0, 1, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.A, false, false, false, false, true, true, 0, 0, 0, 1, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_CHAR, Keys.A, false, false, false, false, false, false, 0, 0, 0, 1, 1 };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.None, false, false, false, true, false, false, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.None, false, false, false, true, true, true, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.None, false, false, false, false, true, true, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.A, false, false, false, true, false, false, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.A, false, false, false, true, true, true, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.A, false, false, false, false, true, true, 0, 0, 0, 0, 1 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 1 };

            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 0 };

            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
            yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(PreProcessMessage_CustomProcessCmdKeyParent_TestData))]
        public void Control_PreProcessMessage_InvokeWithCustomParent_ReturnsExpected(int windowMsg, Keys keys, bool processCmdKeyResult, bool isInputKeyResult, bool processDialogKeyResult, bool isInputCharResult, bool processDialogCharResult, bool expectedResult, int expectedProcessCmdKeyCallCount, int expectedIsInputKeyCallCount, int expectedProcessDialogKeyCallCount, int expectedIsInputCharCallCount, int expectedProcessDialogCharCallCount)
        {
            int processCmdKeyCallCount = 0;
            bool processCmdKeyAction(Message actualM, Keys actualKeyData)
            {
                Assert.Equal((IntPtr)keys, actualM.WParam);
                Assert.Equal(keys, actualKeyData);
                processCmdKeyCallCount++;
                return processCmdKeyResult;
            };
            int isInputKeyCallCount = 0;
            bool isInputKeyAction(Keys actualKeyData)
            {
                Assert.Equal(keys, actualKeyData);
                isInputKeyCallCount++;
                return isInputKeyResult;
            };
            int processDialogKeyCallCount = 0;
            bool processDialogKeyAction(Keys actualKeyData)
            {
                Assert.Equal(keys, actualKeyData);
                processDialogKeyCallCount++;
                return processDialogKeyResult;
            };
            int isInputCharCallCount = 0;
            bool isInputCharAction(char actualCharCode)
            {
                Assert.Equal((char)keys, actualCharCode);
                isInputCharCallCount++;
                return isInputCharResult;
            };
            int processDialogCharCallCount = 0;
            bool processDialogCharAction(char actualCharCode)
            {
                Assert.Equal((char)keys, actualCharCode);
                processDialogCharCallCount++;
                return processDialogCharResult;
            };
            using var parent = new CustomProcessControl
            {
                ProcessCmdKeyAction = processCmdKeyAction,
                ProcessDialogKeyAction = processDialogKeyAction,
                ProcessDialogCharAction = processDialogCharAction
            };
            using var control = new CustomIsInputControl
            {
                Parent = parent,
                IsInputKeyAction = isInputKeyAction,
                IsInputCharAction = isInputCharAction
            };
            var msg = new Message
            {
                Msg = windowMsg,
                WParam = (IntPtr)keys
            };
            Assert.Equal(expectedResult, control.PreProcessMessage(ref msg));
            Assert.Equal(expectedProcessCmdKeyCallCount, processCmdKeyCallCount);
            Assert.Equal(expectedIsInputKeyCallCount, isInputKeyCallCount);
            Assert.Equal(expectedProcessDialogKeyCallCount, processDialogKeyCallCount);
            Assert.Equal(expectedIsInputCharCallCount, isInputCharCallCount);
            Assert.Equal(expectedProcessDialogCharCallCount, processDialogCharCallCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomIsInputControl : Control
        {
            public Func<char, bool> IsInputCharAction { get; set; }

            protected override bool IsInputChar(char charCode) => IsInputCharAction(charCode);

            public Func<Keys, bool> IsInputKeyAction { get; set; }

            protected override bool IsInputKey(Keys keyData) => IsInputKeyAction(keyData);
        }


        private class CustomProcessControl : Control
        {
            public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

            public Func<char, bool> ProcessDialogCharAction { get; set; }

            protected override bool ProcessDialogChar(char charCode) => ProcessDialogCharAction(charCode);

            public Func<Keys, bool> ProcessDialogKeyAction { get; set; }

            protected override bool ProcessDialogKey(Keys keyData) => ProcessDialogKeyAction(keyData);

            public Func<Message, bool> ProcessKeyPreviewAction { get; set; }

            protected override bool ProcessKeyPreview(ref Message m) => ProcessKeyPreviewAction(m);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void Control_ProcessCmdKey_InvokeWithoutParent_ReturnsFalse(Keys keyData)
        {
            using var control = new SubControl();
            var m = new Message();
            Assert.False(control.ProcessCmdKey(ref m, keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void Control_ProcessCmdKey_InvokeWithContextMenu_ReturnsFalse(Keys keyData)
        {
            using var menu = new ContextMenu();
            using var control = new SubControl
            {
                ContextMenu = menu
            };
            var msg = new Message();
            Assert.False(control.ProcessCmdKey(ref msg, keyData));
            Assert.Same(control, menu.SourceControl);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void Control_ProcessCmdKey_InvokeWithCustomContextMenu_ReturnsExpected(Keys keyData, bool result)
        {
            using var control = new SubControl();
            var msg = new Message
            {
                Msg = 1
            };
            int callCount = 0;
            using var contextMenu = new CustomProcessCmdKeyContextMenu();
            bool action(Message actualMsg, Keys actualKeyData, Control actualControl)
            {
                Assert.Equal(1, actualMsg.Msg);
                Assert.Equal(keyData, actualKeyData);
                Assert.Same(control, actualControl);
                Assert.Null(contextMenu.SourceControl);
                callCount++;
                return result;
            }
            contextMenu.ProcessCmdKeyAction = action;
            control.ContextMenu = contextMenu;

            Assert.Equal(result, control.ProcessCmdKey(ref msg, keyData));
            Assert.Null(contextMenu.SourceControl);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void Control_ProcessCmdKey_InvokeWithParent_ReturnsFalse(Keys keyData)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            var msg = new Message();
            Assert.False(control.ProcessCmdKey(ref msg, keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void Control_ProcessCmdKey_InvokeWithCustomParent_ReturnsExpected(Keys keyData, bool result)
        {
            using var control = new SubControl();
            var msg = new Message
            {
                Msg = 1
            };
            int callCount = 0;
            bool action(Message actualMsg, Keys actualKeyData)
            {
                Assert.Equal(1, actualMsg.Msg);
                Assert.Equal(keyData, actualKeyData);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessCmdKeyAction = action
            };
            control.Parent = parent;

            Assert.Equal(result, control.ProcessCmdKey(ref msg, keyData));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true, true, 0, true)]
        [InlineData(Keys.A, true, false, 0, true)]
        [InlineData(Keys.A, false, true, 1, true)]
        [InlineData(Keys.A, false, false, 1, false)]
        public void Control_ProcessCmdKey_InvokeWithCustomContextMenuAndParent_ReturnsExpected(Keys keyData, bool contextMenuResult, bool parentResult, int expectedParentCallCount, bool expectedResult)
        {
            using var control = new SubControl();
            var msg = new Message
            {
                Msg = 1
            };
            using var contextMenu = new CustomProcessCmdKeyContextMenu();
            int contextMenuCallCount = 0;
            bool contextMenuAction(Message actualMsg, Keys actualKeyData, Control actualControl)
            {
                Assert.Equal(1, actualMsg.Msg);
                Assert.Equal(keyData, actualKeyData);
                Assert.Same(control, actualControl);
                Assert.Null(contextMenu.SourceControl);
                contextMenuCallCount++;
                return contextMenuResult;
            }
            contextMenu.ProcessCmdKeyAction = contextMenuAction;
            control.ContextMenu = contextMenu;
            int parentCallCount = 0;
            bool parentAction(Message actualMsg, Keys actualKeyData)
            {
                Assert.Equal(1, actualMsg.Msg);
                Assert.Equal(keyData, actualKeyData);
                parentCallCount++;
                return parentResult;
            }
            using var parent = new CustomProcessControl
            {
                ProcessCmdKeyAction = parentAction
            };
            control.Parent = parent;

            Assert.Equal(expectedResult, control.ProcessCmdKey(ref msg, keyData));
            Assert.Null(contextMenu.SourceControl);
            Assert.Equal(1, contextMenuCallCount);
            Assert.Equal(expectedParentCallCount, parentCallCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomProcessCmdKeyContextMenu : ContextMenu
        {
            public Func<Message, Keys, Control, bool> ProcessCmdKeyAction { get; set; }

            protected internal override bool ProcessCmdKey(ref Message msg, Keys keyData, Control control) => ProcessCmdKeyAction(msg, keyData, control);
        }

        [WinFormsTheory]
        [InlineData('a')]
        public void Control_ProcessDialogChar_InvokeWithoutParent_ReturnsFalse(char charCode)
        {
            using var control = new SubControl();
            Assert.False(control.ProcessDialogChar(charCode));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData('a')]
        public void Control_ProcessDialogChar_InvokeWithParent_ReturnsFalse(char charCode)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            Assert.False(control.ProcessDialogChar(charCode));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData('a', true)]
        [InlineData('a', false)]
        public void Control_ProcessDialogChar_InvokeWithCustomParent_ReturnsExpected(char charCode, bool result)
        {
            int callCount = 0;
            bool action(char actualKeyData)
            {
                Assert.Equal(charCode, actualKeyData);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessDialogCharAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            Assert.Equal(result, control.ProcessDialogChar(charCode));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void Control_ProcessDialogKey_InvokeWithoutParent_ReturnsFalse(Keys keyData)
        {
            using var control = new SubControl();
            Assert.False(control.ProcessDialogKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A)]
        public void Control_ProcessDialogKey_InvokeWithParent_ReturnsFalse(Keys keyData)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            Assert.False(control.ProcessDialogKey(keyData));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void Control_ProcessDialogKey_InvokeWithCustomParent_ReturnsExpected(Keys keyData, bool result)
        {
            int callCount = 0;
            bool action(Keys actualKeyData)
            {
                Assert.Equal(keyData, actualKeyData);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessDialogKeyAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            Assert.Equal(result, control.ProcessDialogKey(keyData));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ProcessKeyEventArgs_TestData()
        {
            foreach (bool handled in new bool[] { true })
            {
                yield return new object[] { (int)User32.WindowMessage.WM_CHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
                yield return new object[] { (int)User32.WindowMessage.WM_CHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
                yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
                yield return new object[] { (int)User32.WindowMessage.WM_SYSCHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
                yield return new object[] { (int)User32.WindowMessage.WM_IME_CHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
                yield return new object[] { (int)User32.WindowMessage.WM_IME_CHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
                yield return new object[] { (int)User32.WindowMessage.WM_KEYDOWN, '2', handled, 0, 1, 0, (IntPtr)2 };
                yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYDOWN, '2', handled, 0, 1, 0, (IntPtr)2 };
                yield return new object[] { (int)User32.WindowMessage.WM_KEYUP, '2', handled, 0, 0, 1, (IntPtr)2 };
                yield return new object[] { (int)User32.WindowMessage.WM_SYSKEYUP, '2', handled, 0, 0, 1, (IntPtr)2 };
                yield return new object[] { 0, '2', handled, 0, 0, 1, (IntPtr)2 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyEventArgs_InvokeWithoutParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var control = new SubControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyEventArgs_InvokeWithParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyEventArgs_InvokeWithCustomParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            int callCount = 0;
            bool action(Message m)
            {
                callCount++;
                return true;
            }
            using var parent = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyEventArgs(ref m));
            Assert.Equal(0, callCount);
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)User32.WindowMessage.WM_CHAR)]
        [InlineData((int)User32.WindowMessage.WM_SYSCHAR)]
        public void Control_ProcessKeyEventArgs_InvokeCharAfterImeChar_Success(int msg)
        {
            using var control = new SubControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(0, e.KeyChar);
                e.Handled = true;
                keyPressCallCount++;
            };
            var charM = new Message
            {
                Msg = msg
            };
            var imeM = new Message
            {
                Msg = (int)User32.WindowMessage.WM_IME_CHAR
            };

            // Char.
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(2, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(2, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Ime, Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(4, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(5, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(6, keyPressCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)User32.WindowMessage.WM_KEYDOWN)]
        [InlineData((int)User32.WindowMessage.WM_SYSKEYDOWN)]
        [InlineData((int)User32.WindowMessage.WM_KEYUP)]
        [InlineData((int)User32.WindowMessage.WM_SYSKEYUP)]
        public void Control_ProcessKeyEventArgs_InvokeNonCharAfterImeChar_Success(int msg)
        {
            using var control = new SubControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                e.Handled = true;
                keyPressCallCount++;
            };
            int keyCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                e.Handled = true;
                keyCallCount++;
            };
            control.KeyUp += (sender, e) =>
            {
                e.Handled = true;
                keyCallCount++;
            };
            var charM = new Message
            {
                Msg = msg
            };
            var imeM = new Message
            {
                Msg = (int)User32.WindowMessage.WM_IME_CHAR
            };

            // Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(0, keyPressCallCount);
            Assert.Equal(1, keyCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(1, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(2, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(1, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);

            // Ime, Ime, Non-Char.
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(2, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref imeM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(3, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(4, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(5, keyCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.ProcessKeyEventArgs(ref charM));
            Assert.Equal(3, keyPressCallCount);
            Assert.Equal(6, keyCallCount);
            Assert.False(control.IsHandleCreated);
        }

        private class CustomProcessKeyEventArgsControl : Control
        {
            public Func<Message, bool> ProcessKeyEventArgsAction { get; set; }

            protected override bool ProcessKeyEventArgs(ref Message m) => ProcessKeyEventArgsAction(m);

            public new bool ProcessKeyMessage(ref Message m) => base.ProcessKeyMessage(ref m);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyMessage_InvokeWithoutParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var control = new SubControl();
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyMessage(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyMessage_InvokeWithParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyMessage(ref m));
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ProcessKeyEventArgs_TestData))]
        public void Control_ProcessKeyMessage_InvokeWithCustomParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
        {
            int callCount = 0;
            bool action(Message m)
            {
                callCount++;
                return true;
            }
            using var parent = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            int keyPressCallCount = 0;
            control.KeyPress += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyChar);
                e.KeyChar = newChar;
                e.Handled = handled;
                keyPressCallCount++;
            };
            int keyDownCallCount = 0;
            control.KeyDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyDownCallCount++;
            };
            int keyUpCallCount = 0;
            control.KeyUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(2, e.KeyValue);
                e.Handled = handled;
                keyUpCallCount++;
            };
            var m = new Message
            {
                Msg = msg,
                WParam = (IntPtr)2
            };
            Assert.Equal(handled, control.ProcessKeyMessage(ref m));
            Assert.Equal(0, callCount);
            Assert.Equal(expectedKeyPressCallCount, keyPressCallCount);
            Assert.Equal(expectedKeyDownCallCount, keyDownCallCount);
            Assert.Equal(expectedKeyUpCallCount, keyUpCallCount);
            Assert.Equal(expectedWParam, m.WParam);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_ProcessKeyMessage_InvokeWithCustomParentProcessKeyPreview_ReturnsExpected(bool result)
        {
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessKeyPreviewAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(result, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_ProcessKeyMessage_InvokeWithCustomProcessKeyEventArgs_ReturnsExpected(bool result)
        {
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var control = new CustomProcessKeyEventArgsControl
            {
                ProcessKeyEventArgsAction = action
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(result, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0, true)]
        [InlineData(true, false, 0, true)]
        [InlineData(false, true, 1, true)]
        [InlineData(false, false, 1, false)]
        public void Control_ProcessKeyMessage_InvokeWithCustomParentProcessKeyPreviewCustomProcessKeyEventArgs_ReturnsExpected(bool parentResult, bool result, int expectedCallCount, bool expectedResult)
        {
            int parentCallCount = 0;
            bool parentAction(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                parentCallCount++;
                return parentResult;
            }
            using var parent = new CustomProcessControl
            {
                ProcessKeyPreviewAction = parentAction
            };
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var control = new CustomProcessKeyEventArgsControl
            {
                Parent = parent,
                ProcessKeyEventArgsAction = action
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(expectedResult, control.ProcessKeyMessage(ref m));
            Assert.Equal(1, parentCallCount);
            Assert.Equal(expectedCallCount, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_ProcessKeyPreview_InvokeWithoutParent_ReturnsFalse()
        {
            using var control = new SubControl();
            var m = new Message();
            Assert.False(control.ProcessKeyPreview(ref m));
        }

        [WinFormsFact]
        public void Control_ProcessKeyPreview_InvokeWithParent_ReturnsFalse()
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            var m = new Message();
            Assert.False(control.ProcessKeyPreview(ref m));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_ProcessKeyPreview_InvokeWithCustomParent_ReturnsExpected(bool result)
        {
            int callCount = 0;
            bool action(Message actualM)
            {
                Assert.Equal(1, actualM.Msg);
                callCount++;
                return result;
            }
            using var parent = new CustomProcessControl
            {
                ProcessKeyPreviewAction = action
            };
            using var control = new SubControl
            {
                Parent = parent
            };
            var m = new Message
            {
                Msg = 1
            };
            Assert.Equal(result, control.ProcessKeyPreview(ref m));
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData('a')]
        [InlineData(char.MinValue)]
        public void Control_ProcessMnemonic_Invoke_ReturnsFalse(char charCode)
        {
            using var control = new SubControl();
            Assert.False(control.ProcessMnemonic(charCode));
            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void Control_RecreateHandle_InvokeWithHandle_Success()
        {
            var control = new SubControl();
            IntPtr handle1 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle1);
            Assert.True(control.IsHandleCreated);

            control.RecreateHandle();
            IntPtr handle2 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle2);
            Assert.NotEqual(handle1, handle2);
            Assert.True(control.IsHandleCreated);

            // Invoke again.
            control.RecreateHandle();
            IntPtr handle3 = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle3);
            Assert.NotEqual(handle2, handle3);
            Assert.True(control.IsHandleCreated);
        }

        [Fact]
        public void Control_RecreateHandle_InvokeWithoutHandle_Nop()
        {
            var control = new SubControl();
            control.RecreateHandle();
            Assert.False(control.IsHandleCreated);

            // Invoke again.
            control.RecreateHandle();
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-1, -2)]
        public void Control_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
        {
            var control = new SubControl();
            control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        }

        [WinFormsFact]
        public void Control_ResetBackColor_Invoke_Success()
        {
            using var control = new Control();

            // Reset without value.
            control.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, control.BackColor);

            // Reset with value.
            control.BackColor = Color.Black;
            control.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, control.BackColor);

            // Reset again.
            control.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
        }

        [WinFormsFact]
        public void Control_ResetBindings_Invoke_Success()
        {
            using var control = new SubControl();

            // Reset without value.
            control.ResetBindings();
            Assert.Empty(control.DataBindings);

            // Reset with value.
            control.DataBindings.Add(new Binding("Text", new object(), "member"));
            control.ResetBindings();
            Assert.Empty(control.DataBindings);

            // Reset again.
            control.ResetBindings();
            Assert.Empty(control.DataBindings);
        }

        [WinFormsFact]
        public void Control_ResetCursor_Invoke_Success()
        {
            using var control = new SubControl();
            using var cursor = new Cursor((IntPtr)1);

            // Reset without value.
            control.ResetCursor();
            Assert.Equal(control.DefaultCursor, control.Cursor);

            // Reset with value.
            control.Cursor = cursor;
            control.ResetCursor();
            Assert.Equal(control.DefaultCursor, control.Cursor);

            // Reset again.
            control.ResetCursor();
            Assert.Equal(control.DefaultCursor, control.Cursor);
        }

        [WinFormsFact]
        public void Control_ResetFont_Invoke_Success()
        {
            using var control = new Control();
            using var font = new Font("Arial", 8.25f);

            // Reset without value.
            control.ResetFont();
            Assert.Equal(Control.DefaultFont, control.Font);

            // Reset with value.
            control.Font = font;
            control.ResetFont();
            Assert.Equal(Control.DefaultFont, control.Font);

            // Reset again.
            control.ResetFont();
            Assert.Equal(Control.DefaultFont, control.Font);
        }

        [WinFormsFact]
        public void Control_ResetForeColor_Invoke_Success()
        {
            using var control = new Control();

            // Reset without value.
            control.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);

            // Reset with value.
            control.ForeColor = Color.Black;
            control.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);

            // Reset again.
            control.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        }

        [WinFormsFact]
        public void Control_ResetImeMode_Invoke_Success()
        {
            using var control = new SubControl();

            // Reset without value.
            control.ResetImeMode();
            Assert.Equal(ImeMode.NoControl, control.ImeMode);

            // Reset with value.
            control.ImeMode = ImeMode.On;
            control.ResetImeMode();
            Assert.Equal(ImeMode.NoControl, control.ImeMode);

            // Reset again.
            control.ResetImeMode();
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
        }

        [WinFormsFact]
        public void Control_ResetMouseEventArgs_InvokeWithoutHandle_Success()
        {
            using var control = new SubControl();
            control.ResetMouseEventArgs();
            Assert.False(control.IsHandleCreated);

            // Invoke again.
            control.ResetMouseEventArgs();
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_ResetMouseEventArgs_InvokeWithHandle_Success()
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ResetMouseEventArgs();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.ResetMouseEventArgs();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_ResetRightToLeft_Invoke_Success()
        {
            using var control = new SubControl();

            // Reset without value.
            control.ResetRightToLeft();
            Assert.Equal(RightToLeft.No, control.RightToLeft);

            // Reset with value.
            control.RightToLeft = RightToLeft.Yes;
            control.ResetRightToLeft();
            Assert.Equal(RightToLeft.No, control.RightToLeft);

            // Reset again.
            control.ResetRightToLeft();
            Assert.Equal(RightToLeft.No, control.RightToLeft);
        }

        [WinFormsFact]
        public void Control_ResetText_Invoke_Success()
        {
            using var control = new SubControl();

            // Reset without value.
            control.ResetText();
            Assert.Empty(control.Text);

            // Reset with value.
            control.Text = "Text";
            control.ResetText();
            Assert.Empty(control.Text);

            // Reset again.
            control.ResetText();
            Assert.Empty(control.Text);
        }

        public static IEnumerable<object[]> SetAutoSizeMode_TestData()
        {
            yield return new object[] { AutoSizeMode.GrowOnly, AutoSizeMode.GrowOnly };
            yield return new object[] { AutoSizeMode.GrowAndShrink, AutoSizeMode.GrowAndShrink };
            yield return new object[] { AutoSizeMode.GrowAndShrink - 1, AutoSizeMode.GrowOnly };
            yield return new object[] { AutoSizeMode.GrowOnly + 1, AutoSizeMode.GrowOnly };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetAutoSizeMode_TestData))]
        public void Control_SetAutoSizeMode_Invoke_GetAutoSizeModeReturnsExpected(AutoSizeMode mode, AutoSizeMode expected)
        {
            using var control = new SubControl();
            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetAutoSizeMode_TestData))]
        public void Control_SetAutoSizeMode_InvoekWithParent_GetAutoSizeModeReturnsExpected(AutoSizeMode mode, AutoSizeMode expected)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            parent.Layout += (sender, e) => layoutCallCount++;

            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetAutoSizeMode_TestData))]
        public void Control_SetAutoSizeMode_InvokeWithHandle_GetAutoSizeModeReturnsExpected(AutoSizeMode mode, AutoSizeMode expected)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetAutoSizeMode(mode);
            Assert.Equal(expected, control.GetAutoSizeMode());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_Set_TestData))]
        public void Control_SetClientSizeCore_Invoke_GetReturnsExpected(Size value, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_SetWithCustomStyle_TestData))]
        public void Control_SetClientSizeCore_InvokeWithCustomStyle_GetReturnsExpected(Size value, Size expectedSize)
        {
            using var control = new BorderedControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_SetWithHandle_TestData))]
        public void Control_SetClientSizeCore_InvokeWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, Size expectedSize, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetClientSizeCore(value.Width, value.Height);
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_SetClientSizeCore_InvokeWithHandler_CallsClientSizeChanged()
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            int sizeChangedCallCount = 0;
            EventHandler sizeChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += handler;
            control.SizeChanged += sizeChangedHandler;

            control.SetClientSizeCore(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(2, callCount);
            Assert.Equal(1, sizeChangedCallCount);

            // Set same.
            control.SetClientSizeCore(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(3, callCount);
            Assert.Equal(1, sizeChangedCallCount);

            // Set different.
            control.SetClientSizeCore(11, 11);
            Assert.Equal(new Size(11, 11), control.ClientSize);
            Assert.Equal(5, callCount);
            Assert.Equal(2, sizeChangedCallCount);

            // Remove handler.
            control.ClientSizeChanged -= handler;
            control.SizeChanged -= sizeChangedHandler;
            control.SetClientSizeCore(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(5, callCount);
            Assert.Equal(2, sizeChangedCallCount);
        }

        public static IEnumerable<object[]> SetStyle_TestData()
        {
            yield return new object[] { ControlStyles.UserPaint, true, true };
            yield return new object[] { ControlStyles.UserPaint, false, false };
            yield return new object[] { (ControlStyles)0, true, true };
            yield return new object[] { (ControlStyles)0, false, true };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetStyle_TestData))]
        public void Control_SetStyle_Invoke_GetStyleReturnsExpected(ControlStyles flag, bool value, bool expected)
        {
            var control = new SubControl();
            control.SetStyle(flag, value);
            Assert.Equal(expected, control.GetStyle(flag));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetStyle(flag, value);
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsTheory]
        [MemberData(nameof(SetStyle_TestData))]
        public void Control_SetStyle_InvokeWithHandle_GetStyleReturnsExpected(ControlStyles flag, bool value, bool expected)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetStyle(flag, value);
            Assert.Equal(expected, control.GetStyle(flag));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetStyle(flag, value);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, true, 1, 1, 1)]
        [InlineData(true, false, false, 0, 0, 1)]
        [InlineData(false, true, false, 1, 0, 0)]
        [InlineData(false, false, false, 0, 0, 0)]
        public void Control_SetTopLevel_Invoke_GetTopLevelReturnsExpected(bool visible, bool value, bool expectedHandleCreated1, int expectedStyleChangedCallCount1, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
        {
            using var control = new SubControl
            {
                Visible = visible
            };
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetTopLevel(value);
            Assert.Equal(value, control.GetTopLevel());
            Assert.Equal(expectedHandleCreated1, control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set same.
            control.SetTopLevel(value);
            Assert.Equal(value, control.GetTopLevel());
            Assert.Equal(expectedHandleCreated1, control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount1, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);

            // Set different.
            control.SetTopLevel(!value);
            Assert.Equal(!value, control.GetTopLevel());
            Assert.Equal(visible, control.IsHandleCreated);
            Assert.Equal(expectedCreatedCallCount1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount1 + 1, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, true, 1)]
        [InlineData(false, false, 0)]
        public void Control_SetTopLevel_InvokeWithHandle_GetTopLevelReturnsExpected(bool visible, bool value, int expectedStyleChangedCallCount)
        {
            using var control = new SubControl
            {
                Visible = visible
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetTopLevel(value);
            Assert.Equal(value, control.GetTopLevel());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetTopLevel(value);
            Assert.Equal(value, control.GetTopLevel());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.SetTopLevel(!value);
            Assert.Equal(!value, control.GetTopLevel());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount + 1, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount + 1, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_SetTopLevel_InvokeWithParent_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            Assert.Throws<ArgumentException>("value", () => control.SetTopLevel(true));
            control.SetTopLevel(false);
            Assert.False(control.GetTopLevel());
        }

        public static IEnumerable<object[]> SizeFromClientSize_TestData()
        {
            yield return new object[] { Size.Empty };
            yield return new object[] { new Size(1, 2) };
            yield return new object[] { new Size(-1, -2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(SizeFromClientSize_TestData))]
        public void Control_SizeFromClientSize_Invoke_ReturnsExpected(Size clientSize)
        {
            using var control = new SubControl();
            Assert.Equal(clientSize, control.SizeFromClientSize(clientSize));
            Assert.False(control.IsHandleCreated);

            // Call again to test caching behavior.
            Assert.Equal(clientSize, control.SizeFromClientSize(clientSize));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SizeFromClientSize_TestData))]
        public void Control_SizeFromClientSize_InvokeWithStyles_ReturnsExpected(Size clientSize)
        {
            using var control = new BorderedControl();
            var expected = new Size(clientSize.Width + 4, clientSize.Height + 4);
            Assert.Equal(expected, control.SizeFromClientSize(clientSize));
            Assert.False(control.IsHandleCreated);

            // Call again to test caching behavior.
            Assert.Equal(expected, control.SizeFromClientSize(clientSize));
            Assert.False(control.IsHandleCreated);
        }

        private class BorderedControl : Control
        {
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style |= (int)User32.WS.BORDER;
                    cp.ExStyle |= (int)User32.WS_EX.STATICEDGE;
                    return cp;
                }
            }

            public new void SetClientSizeCore(int width, int height) => base.SetClientSizeCore(width, height);

            public new Size SizeFromClientSize(Size clientSize) => base.SizeFromClientSize(clientSize);

            public new void UpdateBounds() => base.UpdateBounds();

            public new void UpdateBounds(int x, int y, int width, int height) => base.UpdateBounds(x, y, width, height);

            public new void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight) => base.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
        }

        [WinFormsTheory]
        [MemberData(nameof(SizeFromClientSize_TestData))]
        public void Control_SizeFromClientSize_InvokeWithHandle_ReturnsExpected(Size clientSize)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(clientSize, control.SizeFromClientSize(clientSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again to test caching behavior.
            Assert.Equal(clientSize, control.SizeFromClientSize(clientSize));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void Control_ToString_Invoke_ReturnsExpected()
        {
            var control = new Control();
            Assert.Equal("System.Windows.Forms.Control", control.ToString());
        }

        [WinFormsFact]
        public void Control_UpdateBounds_Invoke_Success()
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int resizeCallCount = 0;
            control.Resize += (sender, e) => resizeCallCount++;
            int sizeChangedCallCount = 0;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            int clientSizeChangedCallCount = 0;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeTopLevel_Success()
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int resizeCallCount = 0;
            control.Resize += (sender, e) => resizeCallCount++;
            int sizeChangedCallCount = 0;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            int clientSizeChangedCallCount = 0;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
            control.SetTopLevel(true);
            Assert.True(control.IsHandleCreated);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UpdateBounds();
            Assert.True(control.ClientSize.Width > 0);
            Assert.True(control.ClientSize.Height >= 0);
            Assert.Equal(0, control.ClientRectangle.X);
            Assert.Equal(0, control.ClientRectangle.Y);
            Assert.True(control.ClientRectangle.Width > 0);
            Assert.True(control.ClientRectangle.Height >= 0);
            Assert.Equal(0, control.DisplayRectangle.X);
            Assert.Equal(0, control.DisplayRectangle.Y);
            Assert.True(control.DisplayRectangle.Width > 0);
            Assert.True(control.DisplayRectangle.Height >= 0);
            Assert.True(control.Size.Width > 0);
            Assert.True(control.Size.Height > 0);
            Assert.Equal(0, control.Left);
            Assert.True(control.Right > 0);
            Assert.Equal(0, control.Top);
            Assert.True(control.Bottom >= 0);
            Assert.True(control.Width > 0);
            Assert.True(control.Height >= 0);
            Assert.Equal(0, control.Bounds.X);
            Assert.Equal(0, control.Bounds.Y);
            Assert.True(control.Bounds.Width > 0);
            Assert.True(control.Bounds.Height >= 0);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.UpdateBounds();
            Assert.True(control.ClientSize.Width > 0);
            Assert.True(control.ClientSize.Height >= 0);
            Assert.Equal(0, control.ClientRectangle.X);
            Assert.Equal(0, control.ClientRectangle.Y);
            Assert.True(control.ClientRectangle.Width > 0);
            Assert.True(control.ClientRectangle.Height >= 0);
            Assert.Equal(0, control.DisplayRectangle.X);
            Assert.Equal(0, control.DisplayRectangle.Y);
            Assert.True(control.DisplayRectangle.Width > 0);
            Assert.True(control.DisplayRectangle.Height >= 0);
            Assert.True(control.Size.Width > 0);
            Assert.True(control.Size.Height >= 0);
            Assert.Equal(0, control.Left);
            Assert.True(control.Right > 0);
            Assert.Equal(0, control.Top);
            Assert.True(control.Bottom >= 0);
            Assert.True(control.Width > 0);
            Assert.True(control.Height >= 0);
            Assert.Equal(0, control.Bounds.X);
            Assert.Equal(0, control.Bounds.Y);
            Assert.True(control.Bounds.Width > 0);
            Assert.True(control.Bounds.Height >= 0);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeWithBounds_Success()
        {
            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 3, 4)
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeWithParentWithBounds_Success()
        {
            using var parent = new SubControl
            {
                Bounds = new Rectangle(10, 20, 30, 40)
            };
            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 3, 4),
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeWithHandle_Success()
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeWithBoundsWithHandle_Success()
        {
            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 3, 4)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int resizeCallCount = 0;
            control.Resize += (sender, e) => resizeCallCount++;
            int sizeChangedCallCount = 0;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            int clientSizeChangedCallCount = 0;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UpdateBounds();
            Assert.Equal(new Size(3, 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.DisplayRectangle);
            Assert.Equal(new Size(3, 4), control.Size);
            Assert.Equal(1, control.Left);
            Assert.Equal(4, control.Right);
            Assert.Equal(2, control.Top);
            Assert.Equal(6, control.Bottom);
            Assert.Equal(3, control.Width);
            Assert.Equal(4, control.Height);
            Assert.Equal(new Rectangle(1, 2, 3, 4), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(new Size(3, 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.DisplayRectangle);
            Assert.Equal(new Size(3, 4), control.Size);
            Assert.Equal(1, control.Left);
            Assert.Equal(4, control.Right);
            Assert.Equal(2, control.Top);
            Assert.Equal(6, control.Bottom);
            Assert.Equal(3, control.Width);
            Assert.Equal(4, control.Height);
            Assert.Equal(new Rectangle(1, 2, 3, 4), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_UpdateBounds_InvokeWithParentWithBoundsWithHandle_Success()
        {
            using var parent = new SubControl
            {
                Bounds = new Rectangle(10, 20, 30, 40)
            };
            using var control = new SubControl
            {
                Bounds = new Rectangle(1, 2, 3, 4),
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int resizeCallCount = 0;
            control.Resize += (sender, e) => resizeCallCount++;
            int sizeChangedCallCount = 0;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            int clientSizeChangedCallCount = 0;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.UpdateBounds();
            Assert.Equal(new Size(3, 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.DisplayRectangle);
            Assert.Equal(new Size(3, 4), control.Size);
            Assert.Equal(1, control.Left);
            Assert.Equal(4, control.Right);
            Assert.Equal(2, control.Top);
            Assert.Equal(6, control.Bottom);
            Assert.Equal(3, control.Width);
            Assert.Equal(4, control.Height);
            Assert.Equal(new Rectangle(1, 2, 3, 4), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.UpdateBounds();
            Assert.Equal(new Size(3, 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 3, 4), control.DisplayRectangle);
            Assert.Equal(new Size(3, 4), control.Size);
            Assert.Equal(1, control.Left);
            Assert.Equal(4, control.Right);
            Assert.Equal(2, control.Top);
            Assert.Equal(6, control.Bottom);
            Assert.Equal(3, control.Width);
            Assert.Equal(4, control.Height);
            Assert.Equal(new Rectangle(1, 2, 3, 4), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { 0, 0, 0, 0, 0 };
            yield return new object[] { -1, -2, -3, -4, 1 };
            yield return new object[] { 1, 0, 0, 0, 0 };
            yield return new object[] { 0, 2, 0, 0, 0 };
            yield return new object[] { 1, 2, 0, 0, 0 };
            yield return new object[] { 0, 0, 1, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, 1 };
            yield return new object[] { 0, 0, 1, 2, 1 };
            yield return new object[] { 1, 2, 30, 40, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_TestData))]
        public void UpdateBounds_InvokeIntIntIntInt_Success(int x, int y, int width, int height, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_WithCustomStyle_TestData()
        {
            yield return new object[] { 0, 0, 0, 0, -4, -4 };
            yield return new object[] { -1, -2, -3, -4, -7, -8 };
            yield return new object[] { 1, 0, 0, 0, -4, -4 };
            yield return new object[] { 0, 2, 0, 0, -4, -4 };
            yield return new object[] { 1, 2, 0, 0, -4, -4 };
            yield return new object[] { 0, 0, 1, 0, -3, -4 };
            yield return new object[] { 0, 0, 0, 2, -4, -2 };
            yield return new object[] { 0, 0, 1, 2, -3, -2 };
            yield return new object[] { 1, 2, 30, 40, 26, 36 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_WithCustomStyle_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntWithCustomStyle_Success(int x, int y, int width, int height, int expectedClientWidth, int expectedClientHeight)
        {
            using var control = new BorderedControl();
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.Equal(1, resizeCallCount);
            Assert.Equal(1, sizeChangedCallCount);
            Assert.Equal(1, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntWithParent_Success(int x, int y, int width, int height, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount - 1, parentLayoutCallCount);
                Assert.Equal(layoutCallCount - 1, parentLayoutCallCount);
                Assert.Equal(sizeChangedCallCount - 1, parentLayoutCallCount);
                Assert.Equal(clientSizeChangedCallCount - 1, parentLayoutCallCount);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_WithHandle_TestData()
        {
            yield return new object[] { true, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, -1, -2, -3, -4, 1, 1 };
            yield return new object[] { true, 1, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 0, 2, 0, 0, 0, 0 };
            yield return new object[] { true, 1, 2, 0, 0, 0, 0 };
            yield return new object[] { true, 0, 0, 1, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 2, 1, 1 };
            yield return new object[] { true, 0, 0, 1, 2, 1, 1 };
            yield return new object[] { true, 1, 2, 30, 40, 1, 1 };

            yield return new object[] { false, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, -1, -2, -3, -4, 1, 0 };
            yield return new object[] { false, 1, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 2, 0, 0, 0, 0 };
            yield return new object[] { false, 1, 2, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 0, 1, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 2, 1, 0 };
            yield return new object[] { false, 0, 0, 1, 2, 1, 0 };
            yield return new object[] { false, 1, 2, 30, 40, 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_WithHandle_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_WithHandle_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount - 1, parentLayoutCallCount);
                Assert.Equal(layoutCallCount - 1, parentLayoutCallCount);
                Assert.Equal(sizeChangedCallCount - 1, parentLayoutCallCount);
                Assert.Equal(clientSizeChangedCallCount - 1, parentLayoutCallCount);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.UpdateBounds(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { -1, -2, -3, -4, -5, -6, 1 };
            yield return new object[] { 1, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { 0, 2, 0, 0, 0, 0, 0 };
            yield return new object[] { 1, 2, 0, 0, 0, 0, 0 };
            yield return new object[] { 0, 0, 1, 0, 0, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, 0, 0, 1 };
            yield return new object[] { 0, 0, 1, 2, 0, 0, 1 };
            yield return new object[] { 0, 0, 0, 0, 1, 0, 1 };
            yield return new object[] { 0, 0, 0, 0, 0, 2, 1 };
            yield return new object[] { 0, 0, 0, 0, 1, 2, 1 };
            yield return new object[] { 1, 2, 0, 0, 3, 4, 1 };
            yield return new object[] { 1, 2, 30, 40, 30, 40, 1 };
            yield return new object[] { 1, 2, 30, 40, 20, 30, 1 };
            yield return new object[] { 1, 2, 30, 40, 50, 60, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_Int_Int_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntIntInt_Success(int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_Int_Int_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntIntIntWithParent_Success(int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount - 1, parentLayoutCallCount);
                Assert.Equal(layoutCallCount - 1, parentLayoutCallCount);
                Assert.Equal(sizeChangedCallCount - 1, parentLayoutCallCount);
                Assert.Equal(clientSizeChangedCallCount - 1, parentLayoutCallCount);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_Int_Int_WithHandle_TestData()
        {
            yield return new object[] { true, 0, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, -1, -2, -3, -4, -5, -6, 1, 1 };
            yield return new object[] { true, 1, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 0, 2, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 1, 2, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 0, 0, 1, 0, 0, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 2, 0, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 1, 2, 0, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 0, 1, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 0, 0, 2, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 0, 1, 2, 1, 1 };
            yield return new object[] { true, 1, 2, 0, 0, 30, 40, 1, 1 };
            yield return new object[] { true, 1, 2, 30, 40, 30, 40, 1, 1 };
            yield return new object[] { true, 1, 2, 30, 40, 20, 30, 1, 1 };
            yield return new object[] { true, 1, 2, 30, 40, 50, 60, 1, 1 };

            yield return new object[] { false, 0, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, -1, -2, -3, -4, -5, -6, 1, 0 };
            yield return new object[] { false, 1, 0, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 2, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 1, 2, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 0, 0, 1, 0, 0, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 2, 0, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 1, 2, 0, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 0, 1, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 0, 0, 2, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 0, 1, 2, 1, 0 };
            yield return new object[] { false, 1, 2, 0, 0, 30, 40, 1, 0 };
            yield return new object[] { false, 1, 2, 30, 40, 30, 40, 1, 0 };
            yield return new object[] { false, 1, 2, 30, 40, 20, 30, 1, 0 };
            yield return new object[] { false, 1, 2, 30, 40, 50, 60, 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_Int_Int_WithHandle_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntIntIntWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_Int_Int_WithHandle_TestData))]
        public void UpdateBounds_InvokeIntIntIntIntIntIntWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount - 1, parentLayoutCallCount);
                Assert.Equal(layoutCallCount - 1, parentLayoutCallCount);
                Assert.Equal(sizeChangedCallCount - 1, parentLayoutCallCount);
                Assert.Equal(clientSizeChangedCallCount - 1, parentLayoutCallCount);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
            Assert.Equal(new Size(clientWidth, clientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, clientWidth, clientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        [Fact]
        public void Control_UpdateStyles_InvokeWithoutHandle_Success()
        {
            var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.StyleChanged += handler;
            control.UpdateStyles();
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.StyleChanged -= handler;
            control.UpdateStyles();
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void Control_UpdateStyles_InvokeWithHandle_Success()
        {
            var control = new SubControl();
            IntPtr handle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            control.StyleChanged += handler;
            control.UpdateStyles();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(handle, control.Handle);

            // Remove handler.
            control.StyleChanged -= handler;
            control.UpdateStyles();
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(handle, control.Handle);
        }
    }
}
