// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Layout;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

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

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_TestData))]
    public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntWithoutHandle_Nop(AccessibleEvents accEvent, int childID)
    {
        using SubControl control = new();
        control.AccessibilityNotifyClients(accEvent, childID);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.AccessibilityNotifyClients(accEvent, childID);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_TestData))]
    public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntWithHandle_Success(AccessibleEvents accEvent, int childID)
    {
        using SubControl control = new();
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

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_Int_TestData))]
    public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntIntWithoutHandle_Nop(AccessibleEvents accEvent, int objectID, int childID)
    {
        using SubControl control = new();
        control.AccessibilityNotifyClients(accEvent, objectID, childID);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.AccessibilityNotifyClients(accEvent, objectID, childID);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityNotifyClients_AccessibleEvents_Int_Int_TestData))]
    public void Control_AccessibilityNotifyClients_InvokeAccessibleEventsIntIntWithHandle_Success(AccessibleEvents accEvent, int objectID, int childID)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.AccessibilityNotifyClients(accEvent, objectID, childID);
        Assert.True(control.IsHandleCreated);

        // Call again.
        control.AccessibilityNotifyClients(accEvent, objectID, childID);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_BringToFront_InvokeWithoutHandleWithoutParent_Nop(bool topLevel)
    {
        using SubControl control = new();
        control.SetTopLevel(topLevel);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.BringToFront();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(topLevel, control.IsHandleCreated);

        // Call again.
        control.BringToFront();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(topLevel, control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_BringToFront_InvokeWithoutHandleWithParent_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child2.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child2, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_BringToFront_InvokeWithHandleWithParentWithoutHandle_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child2.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child2, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, child1.Handle);
        int invalidatedCallCount = 0;
        child1.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child1.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child1.HandleCreated += (sender, e) => createdCallCount++;

        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_BringToFront_InvokeWithHandleWithParentWithHandle_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child2.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child2, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        child1.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child1.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child1.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_BringToFront_InvokeWithoutHandleWithParentWithHandle_Success()
    {
        using Control parent = new();
        using SubControl child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child2.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child2, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        child1.DestroyHandle();
        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child2.BringToFront();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_BringToFront_InvokeWithHandleWithoutParent_Success(bool enabled, bool topLevel)
    {
        using SubControl control = new()
        {
            Enabled = enabled
        };
        control.SetTopLevel(topLevel);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BringToFront();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.BringToFront();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void Control_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createHandle, AccessibleRole expectedAccessibleRole)
    {
        using SubControl control = new();
        if (createHandle)
        {
            control.CreateHandle();
        }

        Assert.Equal(createHandle, control.IsHandleCreated);
        Control.ControlAccessibleObject instance = Assert.IsAssignableFrom<Control.ControlAccessibleObject>(control.CreateAccessibilityInstance());
        Assert.NotNull(instance);
        Assert.Same(control, instance.Owner);
        Assert.Equal(expectedAccessibleRole, instance.Role);
        Assert.NotSame(control.CreateAccessibilityInstance(), instance);
        Assert.NotSame(control.AccessibilityObject, instance);
        Assert.Equal(createHandle, control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_CreateControl_Invoke_Success()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_CreateControl_InvokeNoUserPaint_Success()
    {
        using SubControl control = new();
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

    [WinFormsTheory]
    [MemberData(nameof(CreateControl_Region_TestData))]
    public void Control_CreateControl_InvokeWithRegion_Success(Region region)
    {
        using SubControl control = new()
        {
            Region = region
        };

        control.CreateControl();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Same(region, control.Region);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_CreateControl_InvokeWithText_Success(string text, string expectedText)
    {
        using SubControl control = new()
        {
            Text = text
        };

        control.CreateControl();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expectedText, control.Text);
    }

    [WinFormsFact]
    public void Control_CreateControl_InvokeAllowDrop_Success()
    {
        using SubControl control = new()
        {
            AllowDrop = true
        };

        control.CreateControl();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.AllowDrop);
    }

    [WinFormsFact]
    public void Control_CreateControl_InvokeWithParent_Success()
    {
        using Control parent = new();
        using SubControl control = new()
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

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_CreateControl_InvokeWithHandleWithText_Success(string text, string expectedText)
    {
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_CreateControl_InvokeWithChildren_Success()
    {
        using SubControl parent = new();
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_CreateControl_InvokeNotVisible_Nop()
    {
        using Control control = new()
        {
            Visible = false
        };
        control.CreateControl();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_CreateControl_InvokeWithHandler_CallsHandleCreated()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_CreateControl_InvokeCantCreate_ThrowsWin32Exception()
    {
        using NoCreateControl control = new();
        Assert.Throws<Win32Exception>(control.CreateControl);
    }

    [WinFormsFact]
    public void Control_CreateControl_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using SubControl control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(control.CreateControl);
    }

    [WinFormsFact]
    public void Control_CreateControlsInstance_Invoke_ReturnsExpected()
    {
        using SubControl control = new();
        Control.ControlCollection controls = Assert.IsType<Control.ControlCollection>(control.CreateControlsInstance());
        Assert.Empty(controls);
        Assert.Same(control, controls.Owner);
        Assert.False(controls.IsReadOnly);
        Assert.NotSame(controls, control.CreateControlsInstance());
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_CreateHandle_Invoke_Success(bool resizeRedraw)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, resizeRedraw);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(0, layoutCallCount);
    }

    public static IEnumerable<object[]> CreateHandle_Region_TestData()
    {
        yield return new object[] { new Region() };
        yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateHandle_Region_TestData))]
    public void Control_CreateHandle_InvokeWithRegion_Success(Region region)
    {
        using SubControl control = new()
        {
            Region = region
        };

        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Same(region, control.Region);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_CreateHandle_InvokeWithText_Success(string text, string expectedText)
    {
        using SubControl control = new()
        {
            Text = text
        };

        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expectedText, control.Text);
    }

    [WinFormsFact]
    public void Control_CreateHandle_InvokeAllowDrop_Success()
    {
        using SubControl control = new()
        {
            AllowDrop = true
        };

        control.CreateHandle();
        Assert.True(control.Created);
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.AllowDrop);
    }

    [WinFormsFact]
    public void Control_CreateHandle_InvokeWithParent_Success()
    {
        using Control parent = new();
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_CreateHandle_InvokeWithChildren_Success()
    {
        using SubControl parent = new();
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_CreateHandle_InvokeNotVisible_Nop()
    {
        using SubControl control = new()
        {
            Visible = false
        };
        control.CreateHandle();
        Assert.True(control.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
    }

    [WinFormsFact]
    public void Control_CreateHandle_InvokeWithHandler_CallsHandleCreated()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_CreateHandle_InvokeCantCreate_ThrowsWin32Exception()
    {
        using NoCreateControl control = new();
        Assert.Throws<Win32Exception>(control.CreateHandle);
    }

    [WinFormsFact]
    public void Control_CreateHandle_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using SubControl control = new();
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(control.CreateHandle);
    }

    [WinFormsFact]
    public void Control_CreateHandle_InvokeAlreadyCreated_ThrowsInvalidOperationException()
    {
        using SubControl control = new();
        control.CreateHandle();
        Assert.Throws<InvalidOperationException>(control.CreateHandle);
    }

    [WinFormsFact]
    public void Control_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        using Control child1 = new()
        {
            Parent = control
        };
        using Control child2 = new()
        {
            Parent = control
        };
        using Control grandchild1 = new()
        {
            Parent = child1
        };
        using Control otherParent = new();
        using Control otherControl = new()
        {
            Parent = otherParent
        };
        using Control emptyControl = new();

        Assert.True(parent.Contains(control));
        Assert.True(parent.Contains(child1));
        Assert.True(parent.Contains(child2));
        Assert.True(parent.Contains(grandchild1));
        Assert.False(parent.Contains(parent));

        Assert.True(control.Contains(child1));
        Assert.True(control.Contains(child2));
        Assert.True(control.Contains(grandchild1));
        Assert.False(control.Contains(control));
        Assert.False(control.Contains(parent));

        Assert.False(control.Contains(emptyControl));
        Assert.False(control.Contains(otherParent));
        Assert.False(control.Contains(otherControl));
        Assert.False(control.Contains(null));
    }

    [WinFormsFact]
    public void Control_DestroyHandle_InvokeWithHandle_Success()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_DestroyHandle_InvokeWithHandleWithParent_Success()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);

        control.DestroyHandle();
        Assert.True(parent.IsHandleCreated);
        Assert.False(control.IsHandleCreated);
        Assert.Same(parent, control.Parent);
    }

    public static IEnumerable<object[]> DestroyHandle_Region_TestData()
    {
        yield return new object[] { new Region() };
        yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateHandle_Region_TestData))]
    public void Control_DestroyHandle_InvokeWithRegion_Success(Region region)
    {
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_DestroyHandle_InvokeWithHandleAllowDrop_Success()
    {
        using SubControl control = new()
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

    [WinFormsFact]
    public void Control_DestroyHandle_InvokeWithoutHandle_Nop()
    {
        using SubControl control = new();
        control.DestroyHandle();
        Assert.False(control.Created);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_DestroyHandle_InvokeWithHandler_CallsHandleDestroyed()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_Dispose_Invoke_Success()
    {
        using Control control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeWithParent_Success()
    {
        using Control parent = new();
        int controlRemovedCallCount = 0;
        parent.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        using Control control = new()
        {
            Parent = parent
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(1, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(1, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeWithChildren_Success()
    {
        using Control control = new();
        using Control child1 = new();
        using Control child2 = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;
        int child1CallCount = 0;
        child1.Disposed += (sender, e) => child1CallCount++;
        int child2CallCount = 0;
        child2.Disposed += (sender, e) => child2CallCount++;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.True(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.True(child2.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.True(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.True(child2.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeWithBindings_Success()
    {
        using Control control = new();
        control.DataBindings.Add(new Binding("Text", new object(), "member"));

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeWithHandle_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int destroyedCallCount = 0;
        control.HandleDestroyed += (sender, e) => destroyedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(1, destroyedCallCount);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(1, destroyedCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeInDisposing_Nop()
    {
        using SubControl control = new();

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.Dispose();
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposing_Success()
    {
        using SubControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeNotDisposing_Success()
    {
        using SubControl control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposingWithParent_Success()
    {
        using SubControl parent = new();
        int controlRemovedCallCount = 0;
        parent.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        using SubControl control = new()
        {
            Parent = parent
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(1, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(1, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeNotDisposingWithParent_Success()
    {
        using SubControl parent = new();
        int controlRemovedCallCount = 0;
        parent.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        using SubControl control = new()
        {
            Parent = parent
        };
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Same(parent, control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.False(control.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Same(parent, control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.False(control.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposingWithChildren_Success()
    {
        using SubControl control = new();
        using SubControl child1 = new();
        using SubControl child2 = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;
        int child1CallCount = 0;
        child1.Disposed += (sender, e) => child1CallCount++;
        int child2CallCount = 0;
        child2.Disposed += (sender, e) => child2CallCount++;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.True(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.True(child2.IsDisposed);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.True(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.True(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.True(child2.IsDisposed);
            Assert.Equal(2, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeNotDisposingWithChildren_Success()
    {
        using SubControl control = new();
        using SubControl child1 = new();
        using SubControl child2 = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);
        int controlRemovedCallCount = 0;
        control.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;
        int child1CallCount = 0;
        child1.Disposed += (sender, e) => child1CallCount++;
        int child2CallCount = 0;
        child2.Disposed += (sender, e) => child2CallCount++;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Equal(new Control[] { child1, child2 }, control.Controls.Cast<Control>());
            Assert.Empty(control.DataBindings);
            Assert.Same(control, child1.Parent);
            Assert.Same(control, child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.False(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.False(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.False(child2.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Equal(new Control[] { child1, child2 }, control.Controls.Cast<Control>());
            Assert.Empty(control.DataBindings);
            Assert.Same(control, child1.Parent);
            Assert.Same(control, child2.Parent);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(control.Disposing);
            Assert.False(control.IsDisposed);
            Assert.False(child1.Disposing);
            Assert.False(child1.IsDisposed);
            Assert.False(child2.Disposing);
            Assert.False(child2.IsDisposed);
            Assert.Equal(0, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposingWithBindings_Success()
    {
        using SubControl control = new();
        control.DataBindings.Add(new Binding("Text", new object(), "member"));

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeNotDisposingWithBindings_Success()
    {
        using SubControl control = new();
        Binding binding = new("Text", new object(), "member");
        control.DataBindings.Add(binding);

        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Same(binding, Assert.Single(control.DataBindings));
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Same(binding, Assert.Single(control.DataBindings));
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposingWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int destroyedCallCount = 0;
        control.HandleDestroyed += (sender, e) => destroyedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(1, destroyedCallCount);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(1, destroyedCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeNotDisposingWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int destroyedCallCount = 0;
        control.HandleDestroyed += (sender, e) => destroyedCallCount++;

        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, destroyedCallCount);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, destroyedCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void Control_Dispose_InvokeDisposingInDisposing_Nop()
    {
        using SubControl control = new();

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.Dispose(true);
            disposedCallCount++;
        };

        control.Dispose(true);
        Assert.Equal(1, disposedCallCount);
    }

    // This scenario throws an exception before accessing the native clipboard API,
    // so it does not touch the clipboard singleton and does not need to be placed in Sequential collection.
    [WinFormsFact]
    public void Control_DoDragDrop_NullData_ThrowsArgumentNullException()
    {
        using Control control = new();
        Action dragDrop = () => control.DoDragDrop(null, DragDropEffects.All);
        dragDrop.Should().Throw<ArgumentNullException>("data");
    }

    [WinFormsFact]
    public void Control_DoDragDropAsJson_NullData_ThrowsArgumentNullException()
    {
        using Control control = new();
        Action dragDrop = () => control.DoDragDropAsJson<string>(null, DragDropEffects.Copy);
        dragDrop.Should().Throw<ArgumentNullException>("data");
    }

    public static IEnumerable<object[]> DrawToBitmap_TestData()
    {
        yield return new object[] { new Rectangle(0, 0, 1, 1) };
        yield return new object[] { new Rectangle(0, 0, 10, 10) };
        yield return new object[] { new Rectangle(2, 3, 10, 15) };
        yield return new object[] { new Rectangle(2, 3, 15, 10) };
        yield return new object[] { new Rectangle(0, 0, 100, 150) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawToBitmap_TestData))]
    public void Control_DrawToBitmap_Invoke_Success(Rectangle targetBounds)
    {
        using SubControl control = new()
        {
            Width = 20,
            Height = 20,
        };
        using Bitmap bitmap = new(20, 20);
        control.DrawToBitmap(bitmap, targetBounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawToBitmap_TestData))]
    public void Control_DrawToBitmap_InvokeWithHandle_Success(Rectangle rectangle)
    {
        using SubControl control = new()
        {
            Width = 20,
            Height = 20,
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using Bitmap bitmap = new(20, 20);
        control.DrawToBitmap(bitmap, rectangle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_DrawToBitmap_NullBitmap_ThrowsArgumentNullException()
    {
        using SubControl control = new();
        Assert.Throws<ArgumentNullException>("bitmap", () => control.DrawToBitmap(null, new Rectangle(1, 2, 3, 4)));
    }

    [WinFormsTheory]
    [InlineData(-1, 0, 1, 2)]
    [InlineData(0, -1, 1, 2)]
    [InlineData(0, 0, -1, 2)]
    [InlineData(0, 0, 0, 2)]
    [InlineData(0, 0, 1, -1)]
    [InlineData(0, 0, 1, 0)]
    public void Control_DrawToBitmap_InvalidTargetBounds_ThrowsArgumentException(int x, int y, int width, int height)
    {
        using SubControl control = new()
        {
            Width = 20,
            Height = 20
        };
        using Bitmap bitmap = new(10, 10);
        Assert.Throws<ArgumentException>(() => control.DrawToBitmap(bitmap, new Rectangle(x, y, width, height)));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Control_DrawToBitmap_InvokeZeroWidth_ThrowsArgumentException(int width)
    {
        using SubControl control = new()
        {
            Width = width,
            Height = 20
        };
        using Bitmap bitmap = new(10, 10);
        Assert.Throws<ArgumentException>(() => control.DrawToBitmap(bitmap, new Rectangle(1, 2, 3, 4)));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Control_DrawToBitmap_InvokeZeroHeight_ThrowsArgumentException(int height)
    {
        using SubControl control = new()
        {
            Width = 20,
            Height = height
        };
        using Bitmap bitmap = new(10, 10);
        Assert.Throws<ArgumentException>(() => control.DrawToBitmap(bitmap, new Rectangle(1, 2, 3, 4)));
    }

    [WinFormsFact]
    public void Control_FindFormWithParent_ReturnsForm()
    {
        using Control control = new();
        Form form = new();
        control.Parent = form;
        Assert.Equal(form, control.FindForm());
    }

    [WinFormsFact]
    public void Control_FindFormWithoutParent_ReturnsNull()
    {
        using Control control = new();
        Assert.Null(control.FindForm());
    }

    [WinFormsFact]
    public void Control_FromChildHandle_InvokeControlHandle_ReturnsExpected()
    {
        using SubControl control = new();
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
        NativeWindow window = new();
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
        using SubControl parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        IntPtr parentHandle = parent.Handle;
        Assert.NotEqual(IntPtr.Zero, parentHandle);
        Assert.True(control.IsHandleCreated);

        NativeWindow window = new();
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
        Assert.Null(Control.FromChildHandle(1));
    }

    [WinFormsFact]
    public void Control_FromHandle_InvokeControlHandle_ReturnsExpected()
    {
        using SubControl control = new();
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
        NativeWindow window = new();
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
        using SubControl parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        IntPtr parentHandle = parent.Handle;
        Assert.NotEqual(IntPtr.Zero, parentHandle);
        Assert.True(control.IsHandleCreated);

        NativeWindow window = new();
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
        Assert.Null(Control.FromHandle(1));
    }

    // TODO: create a focus test that returns true when a handle has been created
    [WinFormsFact]
    public void Control_Focus_InvokeWithoutHandle_Nop()
    {
        using Control control = new();
        Assert.False(control.Focus());
        Assert.False(control.Focused);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());

        // Call again to test caching.
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [EnumData<GetChildAtPointSkip>]
    public void Control_GetChildAtPoint_Invoke_ReturnsExpected(GetChildAtPointSkip skipValue)
    {
        using Control control = new();
        Assert.Null(control.GetChildAtPoint(new Point(5, 5), skipValue));
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Null(control.GetChildAtPoint(new Point(5, 5), skipValue));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((GetChildAtPointSkip)(-1))]
    [InlineData((GetChildAtPointSkip)8)]
    public void Control_GetChildAtPoint_InvokeInvalidSkipValue_ThrowsInvalidEnumArgumentException(GetChildAtPointSkip skipValue)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("skipValue", () => control.GetChildAtPoint(new Point(5, 5), skipValue));
    }

    [WinFormsFact]
    public void Control_GetContainerControl_GetWithParent_ReturnsNull()
    {
        using Control grandparent = new();
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
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
        using ContainerControl greatGrandparent = new();
        using ContainerControl grandparent = new()
        {
            Parent = greatGrandparent
        };
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
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
        using ContainerControl greatGrandparent = new();
        using SplitContainer grandparent = new()
        {
            Parent = greatGrandparent
        };
        using Control control = new()
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
        using ContainerControl greatGrandparent = new();
        using SubContainerControl grandparent = new()
        {
            Parent = greatGrandparent
        };
        grandparent.SetStyle(ControlStyles.ContainerControl, false);
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
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
        using ContainerControl greatGrandparent = new();
        using SubControl grandparent = new()
        {
            Parent = greatGrandparent
        };
        grandparent.SetStyle(ControlStyles.ContainerControl, true);
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
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

    [WinFormsFact]
    public void Control_GetNextControl()
    {
        using Control cont = new();
        using Control first = new()
        {
            TabIndex = 0
        };
        using Control second = new()
        {
            TabIndex = 1
        };
        using Control third = new()
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

    [WinFormsFact]
    public void Control_GetNextControlReverse()
    {
        using Control cont = new();
        using Control first = new()
        {
            TabIndex = 0
        };
        using Control second = new()
        {
            TabIndex = 1
        };
        using Control third = new()
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

    [WinFormsFact]
    public void Control_GetNextControlNoNext()
    {
        using Control cont = new();
        using Control first = new()
        {
            TabIndex = 0
        };
        using Control second = new()
        {
            TabIndex = 1
        };
        using Control third = new()
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

    [WinFormsFact]
    public void Control_GetNextControlNoNextReverse()
    {
        using Control cont = new();
        using Control first = new()
        {
            TabIndex = 0
        };
        using Control second = new()
        {
            TabIndex = 1
        };
        using Control third = new()
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

    public static IEnumerable<object[]> GetPreferredSize_TestData()
    {
        yield return new object[] { Size.Empty };
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(10, 20) };
        yield return new object[] { new Size(30, 40) };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void Control_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
    {
        using Control control = new();
        Assert.Equal(Size.Empty, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(Size.Empty, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void Control_GetPreferredSize_InvokeWithBounds_ReturnsExpected(Size proposedSize)
    {
        using Control control = new()
        {
            Bounds = new Rectangle(1, 2, 30, 40)
        };
        Assert.Equal(new Size(30, 40), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(new Size(30, 40), control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetPreferredSize_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, new Size(30, 40), Size.Empty };
        yield return new object[] { new Size(10, 20), Size.Empty, new Size(30, 40), new Size(10, 20) };
        yield return new object[] { new Size(30, 40), Size.Empty, new Size(30, 40), new Size(30, 40) };
        yield return new object[] { new Size(31, 40), Size.Empty, new Size(30, 40), new Size(31, 40) };
        yield return new object[] { new Size(30, 41), Size.Empty, new Size(30, 40), new Size(30, 41) };
        yield return new object[] { new Size(40, 50), Size.Empty, new Size(30, 40), new Size(40, 50) };
        yield return new object[] { Size.Empty, new Size(20, 10), new Size(30, 40), Size.Empty };
        yield return new object[] { Size.Empty, new Size(30, 40), new Size(30, 40), Size.Empty };
        yield return new object[] { Size.Empty, new Size(31, 40), new Size(30, 40), Size.Empty };
        yield return new object[] { Size.Empty, new Size(30, 41), new Size(30, 40), Size.Empty };
        yield return new object[] { Size.Empty, new Size(40, 50), new Size(30, 40), Size.Empty };
        yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(30, 40), new Size(10, 20) };
        yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(int.MaxValue, int.MaxValue), new Size(10, 20) };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), new Size(10, 20) };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), new Size(10, 20) };
        yield return new object[] { new Size(30, 40), new Size(20, 30), new Size(30, 40), new Size(30, 40) };
        yield return new object[] { new Size(30, 40), new Size(40, 50), new Size(30, 40), new Size(30, 40) };
        yield return new object[] { new Size(40, 50), new Size(20, 30), new Size(30, 40), new Size(40, 50) };
        yield return new object[] { new Size(40, 50), new Size(40, 50), new Size(30, 40), new Size(40, 50) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_WithConstrainedSize_TestData))]
    public void Control_GetPreferredSize_InvokeWithConstrainedSize_ReturnsExpected(Size minimumSize, Size maximumSize, Size proposedSize, Size expected)
    {
        using Control control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize,
        };
        Assert.Equal(expected, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetPreferredSize(proposedSize));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 0) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 6, 12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 12) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, -6, -12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, -12) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_TestData))]
    public void Control_GetScaledBounds_Invoke_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubControl control = new();
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_WithStyles_TestData()
    {
        yield return new object[] { Rectangle.Empty, new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 4, 4) };
        yield return new object[] { Rectangle.Empty, new Size(0, 0), BoundsSpecified.X, Rectangle.Empty };
        yield return new object[] { Rectangle.Empty, new Size(0, 0), BoundsSpecified.Y, Rectangle.Empty };
        yield return new object[] { Rectangle.Empty, new Size(0, 0), BoundsSpecified.Width, new Rectangle(0, 0, 4, 0) };
        yield return new object[] { Rectangle.Empty, new Size(0, 0), BoundsSpecified.Height, new Rectangle(0, 0, 0, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 4, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 4, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 2, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 2, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_WithStyles_TestData))]
    public void Control_GetScaledBounds_InvokeWithStyles_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using BorderedControl control = new();
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_TestData))]
    public void Control_GetScaledBounds_InvokeWithSite_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(false);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubControl control = new()
        {
            Site = mockSite.Object
        };
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_InvalidDesignModeSite_TestData()
    {
        foreach (object[] testData in GetScaledBounds_TestData())
        {
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], null };
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], new() };

            Mock<IDesignerHost> mockNullDesignerHost = new(MockBehavior.Strict);
            mockNullDesignerHost
                .Setup(s => s.RootComponent)
                .Returns((IComponent)null);
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], mockNullDesignerHost.Object };

            Mock<IDesignerHost> mockUnknownDesignerHost = new(MockBehavior.Strict);
            mockUnknownDesignerHost
                .Setup(s => s.RootComponent)
                .Returns(new Control());
            yield return new object[] { testData[0], testData[1], testData[2], testData[3], mockUnknownDesignerHost.Object };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_InvalidDesignModeSite_TestData))]
    public void Control_GetScaledBounds_InvokeWithInvalidDesignModeSite_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected, object designerHost)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHost)
            .Verifiable();
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubControl control = new()
        {
            Site = mockSite.Object
        };
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_NoScaleLocation_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(1, 2, 0, 0) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 0) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(1, 2, 6, 12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 12) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(1, 2, -6, -12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, -12) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_NoScaleLocation_TestData))]
    public void Control_GetScaledBounds_InvokeWithValidDesignModeSite_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object)
            .Verifiable();
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubControl control = new()
        {
            Site = mockSite.Object
        };
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(control)
            .Verifiable();
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        mockDesignerHost.Verify(h => h.RootComponent, Times.Once());
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
        mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_NoScaleLocation_TestData))]
    public void Control_GetScaledBounds_InvokeTopLevel_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubControl control = new();
        control.SetTopLevel(true);

        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.True(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_FixedWidth_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 3, 0) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 0) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 3, 12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 12) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, 3, -12) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, -12) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_FixedWidth_TestData))]
    public void Control_GetScaledBounds_InvokeFixedWidth_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.FixedWidth, true);

        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_FixedHeight_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 0, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, -6, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_FixedHeight_TestData))]
    public void Control_GetScaledBounds_InvokeFixedHeight_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.FixedHeight, true);

        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> GetScaledBounds_FixedWidthAndHeight_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Rectangle.Empty, new Size(0, 0), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(1, 1), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(2, 3), specified, Rectangle.Empty };
            yield return new object[] { Rectangle.Empty, new Size(-2, -3), specified, Rectangle.Empty };
        }

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.All, new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.X, new Rectangle(0, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Y, new Rectangle(1, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(0, 0), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.All, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.X, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Y, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.All, new Rectangle(2, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.X, new Rectangle(2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Y, new Rectangle(1, 6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(2, 3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };

        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.All, new Rectangle(-2, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.X, new Rectangle(-2, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Y, new Rectangle(1, -6, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Width, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(-2, -3), BoundsSpecified.Height, new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_FixedWidthAndHeight_TestData))]
    public void Control_GetScaledBounds_InvokeFixedWidthAndHeight_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.FixedWidth, true);
        control.SetStyle(ControlStyles.FixedHeight, true);

        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetScaledBounds_FixedWidthAndHeight_TestData))]
    public void Control_GetScaledBounds_InvokeFixedWidthAndHeightWithStyles_ReturnsExpected(Rectangle bounds, SizeF factor, BoundsSpecified specified, Rectangle expected)
    {
        using BorderedControl control = new();
        control.SetStyle(ControlStyles.FixedWidth, true);
        control.SetStyle(ControlStyles.FixedHeight, true);

        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.GetScaledBounds(bounds, factor, specified));
        Assert.False(control.IsHandleCreated);
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
        using SubControl control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void Control_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.False(control.GetTopLevel());

        // Call again to test caching.
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void Control_Hide_Invoke_SetsInvisible()
    {
        using Control control = new()
        {
            Visible = true
        };
        control.Hide();
        Assert.False(control.Visible);

        // Hide again.
        control.Hide();
        Assert.False(control.Visible);
    }

    [WinFormsFact]
    public void Control_Hide_InvokeWithHandler_CallsVisibleChanged()
    {
        using Control control = new()
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

    [WinFormsFact]
    public void Control_InitLayout_Invoke_Success()
    {
        using SubControl control = new();
        control.InitLayout();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.InitLayout();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_InitLayout_InvokeMocked_Success()
    {
        using CustomLayoutEngineControl control = new();
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.InitLayout(control, BoundsSpecified.All))
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);

        control.InitLayout();
        mockLayoutEngine.Verify(e => e.InitLayout(control, BoundsSpecified.All), Times.Once());
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.InitLayout();
        mockLayoutEngine.Verify(e => e.InitLayout(control, BoundsSpecified.All), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_InitLayout_NullLayoutEngine_ThrowsNullReferenceException()
    {
        using CustomLayoutEngineControl control = new();
        control.SetLayoutEngine(null);
        Assert.Throws<NullReferenceException>(control.InitLayout);
    }

    private class CustomLayoutEngineControl : Control
    {
        private LayoutEngine _layoutEngine;

        public CustomLayoutEngineControl()
        {
            _layoutEngine = new Control().LayoutEngine;
        }

        public void SetLayoutEngine(LayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
        }

        public override LayoutEngine LayoutEngine => _layoutEngine;

        public new void InitLayout() => base.InitLayout();
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Invalidate_Invoke_Success(bool opaque)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate();
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate();
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Invalidate_InvokeWithHandle_Success(bool opaque)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(control.ClientRectangle, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Invalidate_InvokeWithChildrenWithHandle_Success(bool opaque)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(control.ClientRectangle, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_Invalidate_InvokeBool_Success(bool opaque, bool invalidateChildren)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate(invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate(invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_Invalidate_InvokeBoolWithHandle_Success(bool opaque, bool invalidateChildren)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(control.ClientRectangle, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate(invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate(invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_Invalidate_InvokeBoolWithChildrenWithHandle_Success(bool opaque, bool invalidateChildren)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(control.ClientRectangle, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate(invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate(invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Rectangle_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { opaque, Rectangle.Empty };
            yield return new object[] { opaque, new Rectangle(0, 0, 10, 20) };
            yield return new object[] { opaque, new Rectangle(1, 2, 3, 4) };
            yield return new object[] { opaque, new Rectangle(5, 10, 5, 10) };
            yield return new object[] { opaque, new Rectangle(100, 200, 300, 400) };
            yield return new object[] { opaque, new Rectangle(-100, -200, -300, -400) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void Control_Invalidate_InvokeRectangle_Success(bool opaque, Rectangle rc)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate(rc);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate(rc);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Rectangle_WithHandle_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { opaque, Rectangle.Empty, new Rectangle(0, 0, 10, 20) };
            yield return new object[] { opaque, new Rectangle(0, 0, 10, 20), new Rectangle(0, 0, 10, 20) };
            yield return new object[] { opaque, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4) };
            yield return new object[] { opaque, new Rectangle(5, 10, 5, 10), new Rectangle(5, 10, 5, 10) };
            yield return new object[] { opaque, new Rectangle(100, 200, 300, 400), new Rectangle(100, 200, 300, 400) };
            yield return new object[] { opaque, new Rectangle(-100, -200, -300, -400), new Rectangle(-100, -200, -300, -400) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRectangleWithHandle_Success(bool opaque, Rectangle rc, Rectangle expectedInvalidRect)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate(rc);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate(rc);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRectangleWithChildrenWithHandle_Success(bool opaque, Rectangle rc, Rectangle expectedInvalidRect)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate(rc);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate(rc);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Rectangle_Bool_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            foreach (bool invalidateChildren in new bool[] { true, false })
            {
                yield return new object[] { opaque, Rectangle.Empty, invalidateChildren };
                yield return new object[] { opaque, new Rectangle(0, 0, 10, 20), invalidateChildren };
                yield return new object[] { opaque, new Rectangle(1, 2, 3, 4), invalidateChildren };
                yield return new object[] { opaque, new Rectangle(5, 10, 5, 10), invalidateChildren };
                yield return new object[] { opaque, new Rectangle(100, 200, 300, 400), invalidateChildren };
                yield return new object[] { opaque, new Rectangle(-100, -200, -300, -400), invalidateChildren };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_Bool_TestData))]
    public void Control_Invalidate_InvokeRectangleBool_Success(bool opaque, Rectangle rc, bool invalidateChildren)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate(rc, invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate(rc, invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Rectangle_Bool_WithHandle_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            foreach (bool invalidateChildren in new bool[] { true, false })
            {
                yield return new object[] { opaque, Rectangle.Empty, invalidateChildren, new Rectangle(0, 0, 10, 20) };
                yield return new object[] { opaque, new Rectangle(0, 0, 10, 20), invalidateChildren, new Rectangle(0, 0, 10, 20) };
                yield return new object[] { opaque, new Rectangle(1, 2, 3, 4), invalidateChildren, new Rectangle(1, 2, 3, 4) };
                yield return new object[] { opaque, new Rectangle(5, 10, 5, 10), invalidateChildren, new Rectangle(5, 10, 5, 10) };
                yield return new object[] { opaque, new Rectangle(100, 200, 300, 400), invalidateChildren, new Rectangle(100, 200, 300, 400) };
                yield return new object[] { opaque, new Rectangle(-100, -200, -300, -400), invalidateChildren, new Rectangle(-100, -200, -300, -400) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_Bool_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRectangleBoolWithHandle_Success(bool opaque, Rectangle rc, bool invalidateChildren, Rectangle expectedInvalidRect)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate(rc, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate(rc, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_Bool_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRectangleBoolWithChildrenWithHandle_Success(bool opaque, Rectangle rc, bool invalidateChildren, Rectangle expectedInvalidRect)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate(rc, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate(rc, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Region_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { opaque, null };
            yield return new object[] { opaque, new Region(new Rectangle(0, 0, 10, 20)) };
            yield return new object[] { opaque, new Region(new Rectangle(1, 2, 3, 4)) };
            yield return new object[] { opaque, new Region(new Rectangle(5, 10, 5, 10)) };
            yield return new object[] { opaque, new Region(new Rectangle(100, 200, 300, 400)) };
            yield return new object[] { opaque, new Region(new Rectangle(-100, -200, -300, -400)) };
            yield return new object[] { opaque, new Region() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_TestData))]
    public void Control_Invalidate_InvokeRegion_Success(bool opaque, Region region)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate(region);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate(region);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Region_WithHandle_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { opaque, null, new Rectangle(0, 0, 10, 20) };
            yield return new object[] { opaque, new Region(new Rectangle(0, 0, 10, 20)), new Rectangle(0, 0, 10, 20) };
            yield return new object[] { opaque, new Region(new Rectangle(1, 2, 3, 4)), new Rectangle(1, 2, 3, 4) };
            yield return new object[] { opaque, new Region(new Rectangle(5, 10, 5, 10)), new Rectangle(5, 10, 5, 10) };
            yield return new object[] { opaque, new Region(new Rectangle(100, 200, 300, 400)), new Rectangle(100, 200, 300, 400) };
            yield return new object[] { opaque, new Region(new Rectangle(-100, -200, -300, -400)), new Rectangle(-100, -200, -300, -400) };
            yield return new object[] { opaque, new Region(), new Rectangle(-4194304, -4194304, 8388608, 8388608) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRegionWithHandle_Success(bool opaque, Region region, Rectangle expectedInvalidRect)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate(region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate(region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRegionWithChildrenWithHandle_Success(bool opaque, Region region, Rectangle expectedInvalidRect)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate(region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate(region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Region_Bool_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            foreach (bool invalidateChildren in new bool[] { true, false })
            {
                yield return new object[] { opaque, null, invalidateChildren };
                yield return new object[] { opaque, new Region(new Rectangle(0, 0, 10, 20)), invalidateChildren };
                yield return new object[] { opaque, new Region(new Rectangle(1, 2, 3, 4)), invalidateChildren };
                yield return new object[] { opaque, new Region(new Rectangle(5, 10, 5, 10)), invalidateChildren };
                yield return new object[] { opaque, new Region(new Rectangle(100, 200, 300, 400)), invalidateChildren };
                yield return new object[] { opaque, new Region(new Rectangle(-100, -200, -300, -400)), invalidateChildren };
                yield return new object[] { opaque, new Region(), invalidateChildren };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_Bool_TestData))]
    public void Control_Invalidate_InvokeRegionBool_Success(bool opaque, Region region, bool invalidateChildren)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.Opaque, opaque);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        control.Invalidate(region, invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);

        control.Invalidate(region, invalidateChildren);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Region_Bool_WithHandle_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            foreach (bool invalidateChildren in new bool[] { true, false })
            {
                yield return new object[] { opaque, null, invalidateChildren, new Rectangle(0, 0, 10, 20) };
                yield return new object[] { opaque, new Region(new Rectangle(0, 0, 10, 20)), invalidateChildren, new Rectangle(0, 0, 10, 20) };
                yield return new object[] { opaque, new Region(new Rectangle(1, 2, 3, 4)), invalidateChildren, new Rectangle(1, 2, 3, 4) };
                yield return new object[] { opaque, new Region(new Rectangle(5, 10, 5, 10)), invalidateChildren, new Rectangle(5, 10, 5, 10) };
                yield return new object[] { opaque, new Region(new Rectangle(100, 200, 300, 400)), invalidateChildren, new Rectangle(100, 200, 300, 400) };
                yield return new object[] { opaque, new Region(new Rectangle(-100, -200, -300, -400)), invalidateChildren, new Rectangle(-100, -200, -300, -400) };
                yield return new object[] { opaque, new Region(), invalidateChildren, new Rectangle(-4194304, -4194304, 8388608, 8388608) };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_Bool_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRegionBoolWithHandle_Success(bool opaque, Region region, bool invalidateChildren, Rectangle expectedInvalidRect)
    {
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Invalidate(region, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Invalidate(region, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Region_Bool_WithHandle_TestData))]
    public void Control_Invalidate_InvokeRegionBoolWithChildrenWithHandle_Success(bool opaque, Region region, bool invalidateChildren, Rectangle expectedInvalidRect)
    {
        using SubControl child = new()
        {
            Size = new Size(10, 20)
        };
        using SubControl control = new()
        {
            Size = new Size(10, 20)
        };
        control.SetStyle(ControlStyles.Opaque, opaque);
        control.Controls.Add(child);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedInvalidRect, e.InvalidRect);
            invalidatedCallCount++;
        };
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int childInvalidatedCallCount = 0;
        child.Invalidated += (sender, e) => childInvalidatedCallCount++;
        int childStyleChangedCallCount = 0;
        child.StyleChanged += (sender, e) => childStyleChangedCallCount++;
        int childCreatedCallCount = 0;
        child.HandleCreated += (sender, e) => childCreatedCallCount++;

        control.Invalidate(region, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);

        // Call again.
        control.Invalidate(region, invalidateChildren);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child.IsHandleCreated);
        Assert.Equal(0, childInvalidatedCallCount);
        Assert.Equal(0, childStyleChangedCallCount);
        Assert.Equal(0, childCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_InvokeDelegateSameThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action method = () =>
        {
            callCount++;
        };
        control.Invoke(method);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_InvokeDelegateThrowsExceptionSameThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action method = () =>
        {
            callCount++;
            throw new DivideByZeroException();
        };
        Assert.Throws<DivideByZeroException>(() => control.Invoke(method));
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public async Task Control_InvokeDelegateThrowsExceptionDifferentThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action method = () =>
        {
            callCount++;
            throw new DivideByZeroException();
        };
        await Task.Run(() =>
        {
            Assert.Throws<DivideByZeroException>(() => control.Invoke(method));
        });
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Invoke_Action_calls_correct_method()
    {
        using Control control = new();
        control.CreateControl();

        DivideByZeroException exception = Assert.Throws<DivideByZeroException>(() => control.Invoke(() => FaultingMethod()));

        /*
        Expecting something like the following.
        The first frame must be the this method, followed by MarshaledInvoke at previous location.

            at System.Windows.Forms.Tests.ControlTests.<Control_Invoke_Action_calls_correct_method>g__FaultingMethod|410_1() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3902
               at System.Windows.Forms.Tests.ControlTests.<>c.<Control_Invoke_Action_calls_correct_method>b__410_2() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3877
               at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6515
               at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6487
               at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6459
               at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6563
            --- End of stack trace from previous location ---
               at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
               at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
               at System.Windows.Forms.Control.Invoke(Action method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6376
               at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass410_0.<Control_Invoke_Action_calls_correct_method>b__0() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3877
               at Xunit.Assert.RecordException(Action testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 27
        */
        Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
        Assert.Contains(" System.Windows.Forms.Control.Invoke(Action method) ", exception.StackTrace);

        static void FaultingMethod()
        {
            throw new DivideByZeroException();
        }
    }

    [WinFormsFact]
    public void Control_Invoke_Delegate_Func_calls_correct_method()
    {
        using Control control = new();
        control.CreateControl();

        DivideByZeroException exception = Assert.Throws<DivideByZeroException>(() =>
        {
            int result = (int)control.Invoke((Delegate)(new Func<float>(() => FaultingMethod(10))));
        });

        /*
        Expecting something like the following.
        The first frame must be the this method, followed by MarshaledInvoke at previous location.

            at System.Windows.Forms.Tests.ControlTests.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>g__FaultingMethod|412_1() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3945
                at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6400
                at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6376
                at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6348
                at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6448
            --- End of stack trace from previous location ---
                at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6829
                at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6310
                at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6290
                at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass412_0.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>b__0() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3915
                at Xunit.Assert.RecordException(Func`1 testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 50
        */
        Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
        Assert.Contains(" System.Windows.Forms.Control.Invoke(Delegate method) ", exception.StackTrace);

        static int FaultingMethod(int a)
        {
            return a / 0;
        }
    }

    [WinFormsFact]
    public void Control_Invoke_Delegate_MethodInvoker_calls_correct_method()
    {
        using Control control = new();
        control.CreateControl();

        DivideByZeroException exception = Assert.Throws<DivideByZeroException>(() => control.Invoke((MethodInvoker)FaultingMethod));

        /*
        Expecting something like the following.
        The first frame must be the this method, followed by MarshaledInvoke at previous location.

            at System.Windows.Forms.Tests.ControlTests.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>g__FaultingMethod|412_1() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3945
                at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6400
                at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6376
                at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6348
                at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6448
            --- End of stack trace from previous location ---
                at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6829
                at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6310
                at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6290
                at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass412_0.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>b__0() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3915
                at Xunit.Assert.RecordException(Func`1 testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 50
        */
        Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
        Assert.Contains(" System.Windows.Forms.Control.Invoke(Delegate method) ", exception.StackTrace);

        static void FaultingMethod()
        {
            throw new DivideByZeroException();
        }
    }

    [WinFormsFact]
    public void Control_Invoke_Func_calls_correct_method()
    {
        using Control control = new();
        control.CreateControl();

        DivideByZeroException exception = Assert.Throws<DivideByZeroException>(() =>
        {
            int result = control.Invoke(() => FaultingMethod(10));
        });

        /*
        Expecting something like the following.
        The first frame must be the this method, followed by MarshaledInvoke at previous location.

            at System.Windows.Forms.Tests.ControlTests.<Control_Invoke_Func_calls_correct_method>g__FaultingMethod|412_1(Int32 a) in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3976
               at System.Windows.Forms.Tests.ControlTests.<>c.<Control_Invoke_Func_calls_correct_method>b__412_2() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3950
            --- End of stack trace from previous location ---
               at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
               at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
               at System.Windows.Forms.Control.Invoke[T](Func`1 method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6422
               at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass412_0.<Control_Invoke_Func_calls_correct_method>b__0() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3950
               at Xunit.Assert.RecordException(Action testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 27
        */
        Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
        Assert.Contains(" System.Windows.Forms.Control.Invoke[T](Func`1 method) ", exception.StackTrace);

        static int FaultingMethod(int a)
        {
            return a / 0;
        }
    }

    [WinFormsFact]
    public void Control_InvokeDelegateObjectSameThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action<int> method = (i) =>
        {
            Assert.Equal(1, i);
            callCount++;
        };
        control.Invoke(method, [1]);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_InvokeDelegateObjectThrowsExceptionSameThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action<int> method = (i) =>
        {
            Assert.Equal(1, i);
            callCount++;
            throw new DivideByZeroException();
        };
        Assert.Throws<DivideByZeroException>(() => control.Invoke(method, [1]));
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace()
    {
        using Control control = new();
        control.CreateControl();

        var exception = Assert.Throws<DivideByZeroException>(() => control.Invoke((MethodInvoker)FaultingMethod));

        /*

        Expecting something like the following.
        The first frame must be the this method, followed by MarshaledInvoke at previous location.

                at System.Windows.Forms.Tests.ControlTests.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>g__FaultingMethod|412_1() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3945
                       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6400
                       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6376
                       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6348
                       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6448
                    --- End of stack trace from previous location ---
                       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6829
                       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6310
                       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6290
                       at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass412_0.<Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace>b__0() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3915
                       at Xunit.Assert.RecordException(Func`1 testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 50

        */
        Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
        Assert.Contains(nameof(Control_InvokeDelegateObjectThrowsExceptionSameThread_VerifyStackTrace), exception.StackTrace);

        static void FaultingMethod()
        {
            throw new DivideByZeroException();
        }
    }

    [WinFormsFact]
    public async Task Control_InvokeDelegateObjectThrowsExceptionDifferentThread_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        Action<int> method = (param) =>
        {
            Assert.Equal(1, param);
            callCount++;
            throw new DivideByZeroException();
        };
        await Task.Run(() =>
        {
            Assert.Throws<DivideByZeroException>(() => control.Invoke(method, [1]));
        });
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public async Task Control_InvokeDelegateObjectThrowsExceptionDifferentThread_VerifyStackTrace()
    {
        using Control control = new();
        control.CreateControl();

        await Task.Run(() =>
        {
            var exception = Assert.Throws<DivideByZeroException>(() => control.Invoke((MethodInvoker)FaultingMethod));

            /*

            Expecting something like the following.
            The first frame must be the this method, followed by MarshaledInvoke at previous location.

                at System.Windows.Forms.Tests.ControlTests.<Control_InvokeDelegateObjectThrowsExceptionDifferentThread_VerifyStackTrace>g__FaultingMethod|413_1() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3969
                       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6400
                       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6367
                       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
                    --- End of stack trace from previous location ---
                       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
                       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6344
                       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6448
                    --- End of stack trace from previous location ---
                       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6829
                       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6310
                       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6290
                       at System.Windows.Forms.Tests.ControlTests.<>c__DisplayClass413_0.<Control_InvokeDelegateObjectThrowsExceptionDifferentThread_VerifyStackTrace>b__2() in ...\winforms\src\System.Windows.Forms\tests\UnitTests\System\Windows\Forms\ControlTests.Methods.cs:line 3947
                       at Xunit.Assert.RecordException(Func`1 testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 50

            */
            Assert.Contains(nameof(FaultingMethod), exception.StackTrace);
            Assert.Contains(nameof(Control_InvokeDelegateObjectThrowsExceptionDifferentThread_VerifyStackTrace), exception.StackTrace);
        });

        static void FaultingMethod()
        {
            throw new DivideByZeroException();
        }
    }

    [WinFormsFact]
    public void Control_Invoke_InvokeWithoutHandle_ThrowsInvalidOperationException()
    {
        using Control control = new();
        Action method = () => { };
        Assert.Throws<InvalidOperationException>(() => control.Invoke(method));
        Assert.Throws<InvalidOperationException>(() => control.Invoke(method, Array.Empty<object>()));
    }

    [WinFormsFact]
    public void Control_Invoke_InvokeInvalidParameters_ThrowsTargetParameterCountException()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Action<int> method = (i) => { };
        Assert.Throws<TargetParameterCountException>(() => control.Invoke(method));
        Assert.Throws<TargetParameterCountException>(() => control.Invoke(method, null));
        Assert.Throws<TargetParameterCountException>(() => control.Invoke(method, Array.Empty<object>()));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Control_InvokeGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
    {
        using Control otherControl = new();
        using SubControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int otherCallCount = 0;
        EventHandler otherHandler = (sender, e) =>
        {
            Assert.Same(otherControl, sender);
            Assert.Same(eventArgs, e);
            otherCallCount++;
        };

        // Call with handler.
        control.GotFocus += handler;
        otherControl.GotFocus += otherHandler;
        control.InvokeGotFocus(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);

        control.InvokeGotFocus(null, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);

        // Remove handler.
        control.GotFocus -= handler;
        otherControl.GotFocus -= otherHandler;
        control.InvokeGotFocus(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void Control_InvokeLostFocus_Invoke_CallsLostFocus(EventArgs eventArgs)
    {
        using SubControl otherControl = new();
        using SubControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int otherCallCount = 0;
        EventHandler otherHandler = (sender, e) =>
        {
            Assert.Same(otherControl, sender);
            Assert.Same(eventArgs, e);
            otherCallCount++;
        };

        // Call with handler.
        control.LostFocus += handler;
        otherControl.LostFocus += otherHandler;
        control.InvokeLostFocus(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);

        control.InvokeLostFocus(null, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);

        // Remove handler.
        control.LostFocus -= handler;
        otherControl.LostFocus -= otherHandler;
        control.InvokeLostFocus(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void Control_InvokePaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
    {
        using SubControl otherControl = new();
        using SubControl control = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) => callCount++;
        int otherCallCount = 0;
        PaintEventHandler otherHandler = (sender, e) =>
        {
            Assert.Same(otherControl, sender);
            Assert.Same(eventArgs, e);
            otherCallCount++;
        };

        // Call with handler.
        control.Paint += handler;
        otherControl.Paint += otherHandler;
        control.InvokePaint(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);

        // Remove handler.
        control.Paint -= handler;
        otherControl.Paint -= otherHandler;
        control.InvokePaint(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(1, otherCallCount);
    }

    [WinFormsFact]
    public void Control_InvokePaint_NullToInvoke_ThrowsNullReferenceException()
    {
        using SubControl control = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));
        Assert.Throws<NullReferenceException>(() => control.InvokePaint(null, eventArgs));
    }

    [WinFormsFact]
    public void Control_InvokePaintBackground_Invoke_CallsPaint()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));

        using Control otherControlParent = new();
        using SubControl otherControl = new()
        {
            Parent = otherControlParent
        };
        otherControl.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        otherControl.BackColor = Color.FromArgb(100, 50, 100, 150);
        using SubControl control = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) => callCount++;
        int otherCallCount = 0;
        PaintEventHandler otherHandler = (sender, e) => otherCallCount++;
        int otherParentCallCount = 0;
        PaintEventHandler otherParentHandler = (sender, e) =>
        {
            Assert.Same(otherControlParent, sender);
            Assert.NotSame(eventArgs, e);
            otherParentCallCount++;
        };

        // Call with handler.
        control.Paint += handler;
        otherControl.Paint += otherHandler;
        otherControlParent.Paint += otherParentHandler;
        control.InvokePaintBackground(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(0, otherCallCount);
        Assert.Equal(1, otherParentCallCount);

        // Remove handler.
        control.Paint -= handler;
        otherControl.Paint -= otherHandler;
        otherControlParent.Paint -= otherParentHandler;
        control.InvokePaintBackground(otherControl, eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(0, otherCallCount);
        Assert.Equal(1, otherParentCallCount);
    }

    [WinFormsFact]
    public void Control_InvokePaintBackground_NullToInvoke_ThrowsNullReferenceException()
    {
        using SubControl control = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using PaintEventArgs eventArgs = new(graphics, new Rectangle(1, 2, 3, 4));
        Assert.Throws<NullReferenceException>(() => control.InvokePaintBackground(null, eventArgs));
    }

    [WinFormsFact]
    public void Control_InvokePaintBackground_NullEventArgs_ThrowsArgumentNullException()
    {
        using Control otherControl = new();
        using SubControl control = new();
        Assert.Throws<ArgumentNullException>(() => control.InvokePaintBackground(otherControl, null));
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    public void Control_IsInputChar_InvokeWithoutHandle_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubControl control = new();
        Assert.Equal(expected, control.IsInputChar((char)keyData));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    public void Control_IsInputChar_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubControl control = new();
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
        yield return new object[] { Keys.End, false };
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
        yield return new object[] { Keys.Control | Keys.End, false };
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
        yield return new object[] { Keys.Alt | Keys.End, false };
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
        using SubControl control = new();
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(IsInputKey_TestData))]
    public void Control_IsInputKey_InvokeWithHandle_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.IsInputKey(keyData));
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> IsMnemonic_TestData()
    {
        yield return new object[] { '&', null, false };
        yield return new object[] { '&', "", false };
        yield return new object[] { '&', "text", false };
        yield return new object[] { '&', "&", false };
        yield return new object[] { 'a', null, false };
        yield return new object[] { 'a', "", false };
        yield return new object[] { 'a', "text", false };
        yield return new object[] { 'a', "a", false };
        yield return new object[] { 'a', "&", false };
        yield return new object[] { 'a', "&a", true };
        yield return new object[] { 'A', "&a", true };
        yield return new object[] { 'a', "&A", true };
        yield return new object[] { 'A', "&A", true };
        yield return new object[] { 'a', "&&a", false };
        yield return new object[] { 'a', "a&a", true };
        yield return new object[] { 'a', "a&ab", true };
        yield return new object[] { 'a', "a&b", false };
        yield return new object[] { 'a', "a&ba", false };
    }

    [WinFormsTheory]
    [MemberData(nameof(IsMnemonic_TestData))]
    public void Control_IsMnemonic_Invoke_ReturnsExpected(char charCode, string text, bool expected)
    {
        Assert.Equal(expected, Control.IsMnemonic(charCode, text));
    }

    public static IEnumerable<object[]> NotifyInvalidated_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(NotifyInvalidated_TestData))]
    public void Control_NotifyInvalidate_Invoke_CallsInvalidated(Rectangle invalidatedArea)
    {
        using SubControl control = new();
        int callCount = 0;
        InvalidateEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(invalidatedArea, e.InvalidRect);
            callCount++;
        };

        // Call with handler.
        control.Invalidated += handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Invalidated -= handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> NotifyInvalidate_WithChildren_TestData()
    {
        yield return new object[] { true, Color.Empty, null };
        yield return new object[] { true, Color.Empty, Rectangle.Empty };
        yield return new object[] { true, Color.Empty, new Rectangle(100, 200, 300, 400) };
        yield return new object[] { true, Color.Empty, new Rectangle(1, 2, 300, 400) };
        yield return new object[] { true, Color.Red, null };
        yield return new object[] { true, Color.Red, Rectangle.Empty };
        yield return new object[] { true, Color.Red, new Rectangle(100, 200, 300, 400) };
        yield return new object[] { true, Color.Red, new Rectangle(1, 2, 300, 400) };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), null };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), Rectangle.Empty };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new Rectangle(100, 200, 300, 400) };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new Rectangle(1, 2, 300, 400) };

        yield return new object[] { false, Color.Empty, null };
        yield return new object[] { false, Color.Empty, Rectangle.Empty };
        yield return new object[] { false, Color.Empty, new Rectangle(100, 200, 300, 400) };
        yield return new object[] { false, Color.Empty, new Rectangle(1, 2, 300, 400) };
        yield return new object[] { false, Color.Red, null };
        yield return new object[] { false, Color.Red, Rectangle.Empty };
        yield return new object[] { false, Color.Red, new Rectangle(100, 200, 300, 400) };
        yield return new object[] { false, Color.Red, new Rectangle(1, 2, 300, 400) };
    }

    [WinFormsTheory]
    [MemberData(nameof(NotifyInvalidate_WithChildren_TestData))]
    public void Control_NotifyInvalidate_InvokeWithChildren_CallsInvalidated(bool supportsTransparentBackgroundColor, Color backColor, Rectangle invalidatedArea)
    {
        using SubControl child1 = new()
        {
            ClientSize = new Size(10, 20)
        };
        using SubControl child2 = new()
        {
            ClientSize = new Size(10, 20)
        };
        using SubControl control = new();
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
            Assert.Equal(invalidatedArea, e.InvalidRect);
            callCount++;
        };

        // Call with handler.
        control.Invalidated += handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);

        // Remove handler.
        control.Invalidated -= handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(NotifyInvalidated_TestData))]
    public void Control_NotifyInvalidate_InvokeWithHandle_CallsInvalidated(Rectangle invalidatedArea)
    {
        using SubControl control = new();
        int callCount = 0;
        InvalidateEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(invalidatedArea, e.InvalidRect);
            callCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Call with handler.
        control.Invalidated += handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.Invalidated -= handler;
        control.NotifyInvalidate(invalidatedArea);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> NotifyInvalidate_WithChildrenWithHandle_TestData()
    {
        yield return new object[] { true, Color.Empty, Rectangle.Empty, 0 };
        yield return new object[] { true, Color.Empty, new Rectangle(100, 200, 300, 400), 0 };
        yield return new object[] { true, Color.Empty, new Rectangle(1, 2, 300, 400), 0 };
        yield return new object[] { true, Color.Red, Rectangle.Empty, 0 };
        yield return new object[] { true, Color.Red, new Rectangle(100, 200, 300, 400), 0 };
        yield return new object[] { true, Color.Red, new Rectangle(1, 2, 300, 400), 0 };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), Rectangle.Empty, 0 };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new Rectangle(100, 200, 300, 400), 0 };
        yield return new object[] { true, Color.FromArgb(200, 50, 100, 150), new Rectangle(1, 2, 300, 400), 1 };

        yield return new object[] { false, Color.Empty, Rectangle.Empty, 0 };
        yield return new object[] { false, Color.Empty, new Rectangle(100, 200, 300, 400), 0 };
        yield return new object[] { false, Color.Empty, new Rectangle(1, 2, 300, 400), 0 };
        yield return new object[] { false, Color.Red, Rectangle.Empty, 0 };
        yield return new object[] { false, Color.Red, new Rectangle(100, 200, 300, 400), 0 };
        yield return new object[] { false, Color.Red, new Rectangle(1, 2, 300, 400), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(NotifyInvalidate_WithChildrenWithHandle_TestData))]
    public void Control_NotifyInvalidate_InvokeWithChildrenWithHandle_CallsInvalidated(bool supportsTransparentBackgroundColor, Color backColor, Rectangle invalidatedArea, int expectedChildInvalidatedCallCount)
    {
        using SubControl child1 = new()
        {
            ClientSize = new Size(10, 20)
        };
        using SubControl child2 = new()
        {
            ClientSize = new Size(10, 20)
        };
        using SubControl control = new();
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
            Assert.Equal(invalidatedArea, e.InvalidRect);
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
        control.NotifyInvalidate(invalidatedArea);
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
        control.NotifyInvalidate(invalidatedArea);
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
    public void Control_PerformLayout_Invoke_Success()
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.PerformLayout();
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.PerformLayout();
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeCustomLayoutEngine_Success()
    {
        using CustomLayoutEngineControl control = new();
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.Layout(control, It.IsAny<LayoutEventArgs>()))
            .Returns(false)
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.PerformLayout();
        Assert.Equal(1, layoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Once());
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.PerformLayout();
        Assert.Equal(2, layoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeWithParent_Success()
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        parent.Layout += (sender, e) => parentLayoutCallCount++;

        control.PerformLayout();
        Assert.Equal(1, layoutCallCount);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.PerformLayout();
        Assert.Equal(2, layoutCallCount);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_PerformLayout_InvokeWithParentCustomLayoutEngine_Success(bool parentNeedsLayout, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using CustomLayoutEngineControl control = new()
        {
            Parent = parent
        };
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.Layout(control, It.IsAny<LayoutEventArgs>()))
            .Returns(parentNeedsLayout)
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("PreferredSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.PerformLayout();
        Assert.Equal(1, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Once());
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.PerformLayout();
        Assert.Equal(2, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_PerformLayout_InvokeSuspended_DoesNotCallLayout(bool performLayout, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout();
        Assert.Equal(0, layoutCallCount);

        // Resume.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        control.PerformLayout();
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_PerformLayout_InvokeSuspendedMultipleTimes_DoesNotCallLayout(bool performLayout, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Null(e.AffectedControl);
            Assert.Null(e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(new Control(), "OtherAffectedProperty");
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(new Control(), "OtherAffectedProperty");
        Assert.Equal(0, layoutCallCount);

        // Resume.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout();
        Assert.Equal(0, layoutCallCount);

        // Resume again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        control.PerformLayout();
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeSetTextInLayout_CachesText()
    {
        using Control control = new();

        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            string longString = new('a', 65536);
            control.Text = longString;
            Assert.Equal(longString, control.Text);
            layoutCallCount++;
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Empty(control.Text);
        control.PerformLayout();
        Assert.Empty(control.Text);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeInDisposing_DoesNotCallLayout()
    {
        using SubControl control = new();
        int callCount = 0;
        control.Layout += (sender, e) => callCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.PerformLayout();
            Assert.Equal(0, callCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    public static IEnumerable<object[]> PerformLayout_Control_String_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new Control(), string.Empty };
        yield return new object[] { new Control(), "AffectedProperty" };
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_TestData))]
    public void Control_PerformLayout_InvokeControlString_Success(Control affectedControl, string affectedProperty)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_TestData))]
    public void Control_PerformLayout_InvokeControlStringCustomLayoutEngine_Success(Control affectedControl, string affectedProperty)
    {
        using CustomLayoutEngineControl control = new();
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.Layout(control, It.IsAny<LayoutEventArgs>()))
            .Returns(false)
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(1, layoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Once());
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(2, layoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_TestData))]
    public void Control_PerformLayout_InvokeControlStringWithParent_Success(Control affectedControl, string affectedProperty)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        parent.Layout += (sender, e) => parentLayoutCallCount++;

        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(1, layoutCallCount);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(2, layoutCallCount);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> PerformLayout_Control_String_WithParent_TestData()
    {
        yield return new object[] { true, null, null, 1 };
        yield return new object[] { true, new Control(), string.Empty, 1 };
        yield return new object[] { true, new Control(), "AffectedProperty", 1 };

        yield return new object[] { false, null, null, 0 };
        yield return new object[] { false, new Control(), string.Empty, 0 };
        yield return new object[] { false, new Control(), "AffectedProperty", 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_WithParent_TestData))]
    public void Control_PerformLayout_InvokeControlStringWithParentCustomLayoutEngine_Success(bool parentNeedsLayout, Control affectedControl, string affectedProperty, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using CustomLayoutEngineControl control = new()
        {
            Parent = parent
        };
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.Layout(control, It.IsAny<LayoutEventArgs>()))
            .Returns(parentNeedsLayout)
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("PreferredSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(1, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Once());
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(2, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
        mockLayoutEngine.Verify(e => e.Layout(control, It.IsAny<LayoutEventArgs>()), Times.Exactly(2));
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> PerformLayout_Control_String_Suspended_TestData()
    {
        yield return new object[] { true, null, null, 1 };
        yield return new object[] { true, new Control(), string.Empty, 1 };
        yield return new object[] { true, new Control(), "AffectedProperty", 1 };

        yield return new object[] { false, null, null, 0 };
        yield return new object[] { false, new Control(), string.Empty, 0 };
        yield return new object[] { false, new Control(), "AffectedProperty", 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_Suspended_TestData))]
    public void Control_PerformLayout_InvokeControlStringSuspended_DoesNotCallLayout(bool performLayout, Control affectedControl, string affectedProperty, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);

        // Resume.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformLayout_Control_String_Suspended_TestData))]
    public void Control_PerformLayout_InvokeControlStringSuspendedMultipleTimes_DoesNotCallLayout(bool performLayout, Control affectedControl, string affectedProperty, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(new Control(), "OtherAffectedProperty");
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(new Control(), "OtherAffectedProperty");
        Assert.Equal(0, layoutCallCount);

        // Resume.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);

        // Resume again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeControlStringSetTextInLayout_CachesText()
    {
        using Control control = new();

        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            string longString = new('a', 65536);
            control.Text = longString;
            Assert.Equal(longString, control.Text);
            layoutCallCount++;
        };

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Empty(control.Text);
        control.PerformLayout(control, "AffectedProperty");
        Assert.Empty(control.Text);
    }

    [WinFormsFact]
    public void Control_PerformLayout_InvokeControlStringInDisposing_DoesNotCallLayout()
    {
        using SubControl control = new();
        int callCount = 0;
        control.Layout += (sender, e) => callCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.PerformLayout(new Control(), "AffectedProperty");
            Assert.Equal(0, callCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_PerformLayout_NullLayoutEngine_ThrowsNullReferenceException()
    {
        using CustomLayoutEngineControl control = new();
        control.SetLayoutEngine(null);
        Assert.Throws<NullReferenceException>(control.PerformLayout);
        Assert.Throws<NullReferenceException>(() => control.PerformLayout(new Control(), "AffectedProperty"));
    }

    public static IEnumerable<object[]> PreProcessMessage_TestData()
    {
        yield return new object[] { 0, Keys.None, false };
        yield return new object[] { 0, Keys.A, false };
        yield return new object[] { 0, Keys.Tab, false };
        yield return new object[] { 0, Keys.Menu, false };
        yield return new object[] { 0, Keys.F10, false };

        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.Tab, true };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.Menu, true };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.F10, true };

        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.Tab, true };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.Menu, true };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.F10, true };

        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.Tab, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.Menu, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.F10, false };

        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.Tab, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.Menu, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.F10, false };

        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.None, true };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.A, true };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.Tab, true };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.Menu, true };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.F10, true };

        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.Tab, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.Menu, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.F10, false };

        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.Tab, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.Menu, false };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.F10, false };

        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.None, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.A, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.Tab, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.Menu, false };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.F10, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(PreProcessMessage_TestData))]
    public void Control_PreProcessMessage_Invoke_ReturnsExpected(int windowMsg, Keys keys, bool expectedIsHandleCreated)
    {
        using SubControl control = new();
        Message msg = new()
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
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Message msg = new()
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

        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.None, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.None, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.None, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.None, false, false, false, false, false, false, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.A, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.A, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.A, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, Keys.A, false, false, false, false, false, false, 1, 1, 1, 0, 0 };

        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.None, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.None, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.None, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.None, false, false, false, false, false, false, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.A, true, false, false, false, false, true, 1, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.A, false, true, false, false, false, false, 1, 1, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.A, false, false, true, false, false, true, 1, 1, 1, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, Keys.A, false, false, false, false, false, false, 1, 1, 1, 0, 0 };

        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.None, false, false, false, true, false, false, 0, 0, 0, 1, 0 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.None, false, false, false, true, true, false, 0, 0, 0, 1, 0 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.None, false, false, false, false, true, true, 0, 0, 0, 1, 1 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.None, false, false, false, false, false, false, 0, 0, 0, 1, 1 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.A, false, false, false, true, false, false, 0, 0, 0, 1, 0 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.A, false, false, false, true, true, false, 0, 0, 0, 1, 0 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.A, false, false, false, false, true, true, 0, 0, 0, 1, 1 };
        yield return new object[] { (int)PInvokeCore.WM_CHAR, Keys.A, false, false, false, false, false, false, 0, 0, 0, 1, 1 };

        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.None, false, false, false, true, false, false, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.None, false, false, false, true, true, true, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.None, false, false, false, false, true, true, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.A, false, false, false, true, false, false, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.A, false, false, false, true, true, true, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.A, false, false, false, false, true, true, 0, 0, 0, 0, 1 };
        yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 1 };

        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_KEYUP, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 0 };

        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.None, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
        yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, Keys.A, false, false, false, false, false, false, 0, 0, 0, 0, 0 };
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
        }

        int isInputKeyCallCount = 0;
        bool isInputKeyAction(Keys actualKeyData)
        {
            Assert.Equal(keys, actualKeyData);
            isInputKeyCallCount++;
            return isInputKeyResult;
        }

        int processDialogKeyCallCount = 0;
        bool processDialogKeyAction(Keys actualKeyData)
        {
            Assert.Equal(keys, actualKeyData);
            processDialogKeyCallCount++;
            return processDialogKeyResult;
        }

        int isInputCharCallCount = 0;
        bool isInputCharAction(char actualCharCode)
        {
            Assert.Equal((char)keys, actualCharCode);
            isInputCharCallCount++;
            return isInputCharResult;
        }

        int processDialogCharCallCount = 0;
        bool processDialogCharAction(char actualCharCode)
        {
            Assert.Equal((char)keys, actualCharCode);
            processDialogCharCallCount++;
            return processDialogCharResult;
        }

        using CustomProcessControl parent = new()
        {
            ProcessCmdKeyAction = processCmdKeyAction,
            ProcessDialogKeyAction = processDialogKeyAction,
            ProcessDialogCharAction = processDialogCharAction
        };
        using CustomIsInputControl control = new()
        {
            Parent = parent,
            IsInputKeyAction = isInputKeyAction,
            IsInputCharAction = isInputCharAction
        };
        Message msg = new()
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
        using SubControl control = new();
        Message m = default;
        Assert.False(control.ProcessCmdKey(ref m, keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Keys.A)]
    public void Control_ProcessCmdKey_InvokeWithParent_ReturnsFalse(Keys keyData)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Message msg = default;
        Assert.False(control.ProcessCmdKey(ref msg, keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Keys.A, true)]
    [InlineData(Keys.A, false)]
    public void Control_ProcessCmdKey_InvokeWithCustomParent_ReturnsExpected(Keys keyData, bool result)
    {
        using SubControl control = new();
        Message msg = new()
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

        using CustomProcessControl parent = new()
        {
            ProcessCmdKeyAction = action
        };
        control.Parent = parent;

        Assert.Equal(result, control.ProcessCmdKey(ref msg, keyData));
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData('a')]
    public void Control_ProcessDialogChar_InvokeWithoutParent_ReturnsFalse(char charCode)
    {
        using SubControl control = new();
        Assert.False(control.ProcessDialogChar(charCode));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData('a')]
    public void Control_ProcessDialogChar_InvokeWithParent_ReturnsFalse(char charCode)
    {
        using Control parent = new();
        using SubControl control = new()
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

        using CustomProcessControl parent = new()
        {
            ProcessDialogCharAction = action
        };
        using SubControl control = new()
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
        using SubControl control = new();
        Assert.False(control.ProcessDialogKey(keyData));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(Keys.A)]
    public void Control_ProcessDialogKey_InvokeWithParent_ReturnsFalse(Keys keyData)
    {
        using Control parent = new();
        using SubControl control = new()
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

        using CustomProcessControl parent = new()
        {
            ProcessDialogKeyAction = action
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Equal(result, control.ProcessDialogKey(keyData));
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ProcessKeyEventArgs_TestData()
    {
        foreach (bool handled in new bool[] { true, false })
        {
            yield return new object[] { (int)PInvokeCore.WM_CHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
            yield return new object[] { (int)PInvokeCore.WM_CHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
            yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
            yield return new object[] { (int)PInvokeCore.WM_SYSCHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
            yield return new object[] { (int)PInvokeCore.WM_IME_CHAR, '2', handled, 1, 0, 0, (IntPtr)50 };
            yield return new object[] { (int)PInvokeCore.WM_IME_CHAR, '1', handled, 1, 0, 0, (IntPtr)49 };
            yield return new object[] { (int)PInvokeCore.WM_KEYDOWN, '2', handled, 0, 1, 0, (IntPtr)2 };
            yield return new object[] { (int)PInvokeCore.WM_SYSKEYDOWN, '2', handled, 0, 1, 0, (IntPtr)2 };
            yield return new object[] { (int)PInvokeCore.WM_KEYUP, '2', handled, 0, 0, 1, (IntPtr)2 };
            yield return new object[] { (int)PInvokeCore.WM_SYSKEYUP, '2', handled, 0, 0, 1, (IntPtr)2 };
            yield return new object[] { 0, '2', handled, 0, 0, 1, (IntPtr)2 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ProcessKeyEventArgs_TestData))]
    public void Control_ProcessKeyEventArgs_InvokeWithoutParent_ReturnsFalse(int msg, char newChar, bool handled, int expectedKeyPressCallCount, int expectedKeyDownCallCount, int expectedKeyUpCallCount, IntPtr expectedWParam)
    {
        using SubControl control = new();
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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
        using Control parent = new();
        using SubControl control = new()
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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

        using CustomProcessKeyEventArgsControl parent = new()
        {
            ProcessKeyEventArgsAction = action
        };
        using SubControl control = new()
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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
    [InlineData((int)PInvokeCore.WM_CHAR)]
    [InlineData((int)PInvokeCore.WM_SYSCHAR)]
    public void Control_ProcessKeyEventArgs_InvokeCharAfterImeChar_Success(int msg)
    {
        using SubControl control = new();
        int keyPressCallCount = 0;
        control.KeyPress += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(0, e.KeyChar);
            e.Handled = true;
            keyPressCallCount++;
        };
        Message charM = new()
        {
            Msg = msg
        };
        Message imeM = new()
        {
            Msg = (int)PInvokeCore.WM_IME_CHAR
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
    [InlineData((int)PInvokeCore.WM_KEYDOWN)]
    [InlineData((int)PInvokeCore.WM_SYSKEYDOWN)]
    [InlineData((int)PInvokeCore.WM_KEYUP)]
    [InlineData((int)PInvokeCore.WM_SYSKEYUP)]
    public void Control_ProcessKeyEventArgs_InvokeNonCharAfterImeChar_Success(int msg)
    {
        using SubControl control = new();
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
        Message charM = new()
        {
            Msg = msg
        };
        Message imeM = new()
        {
            Msg = (int)PInvokeCore.WM_IME_CHAR
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
        using SubControl control = new();
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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
        using Control parent = new();
        using SubControl control = new()
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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

        using CustomProcessKeyEventArgsControl parent = new()
        {
            ProcessKeyEventArgsAction = action
        };
        using SubControl control = new()
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
        Message m = new()
        {
            Msg = msg,
            WParam = 2
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
    [BoolData]
    public void Control_ProcessKeyMessage_InvokeWithCustomParentProcessKeyPreview_ReturnsExpected(bool result)
    {
        int callCount = 0;
        bool action(Message actualM)
        {
            Assert.Equal(1, actualM.Msg);
            callCount++;
            return result;
        }

        using CustomProcessControl parent = new()
        {
            ProcessKeyPreviewAction = action
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Message m = new()
        {
            Msg = 1
        };
        Assert.Equal(result, control.ProcessKeyMessage(ref m));
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ProcessKeyMessage_InvokeWithCustomProcessKeyEventArgs_ReturnsExpected(bool result)
    {
        int callCount = 0;
        bool action(Message actualM)
        {
            Assert.Equal(1, actualM.Msg);
            callCount++;
            return result;
        }

        using CustomProcessKeyEventArgsControl control = new()
        {
            ProcessKeyEventArgsAction = action
        };
        Message m = new()
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

        using CustomProcessControl parent = new()
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

        using CustomProcessKeyEventArgsControl control = new()
        {
            Parent = parent,
            ProcessKeyEventArgsAction = action
        };
        Message m = new()
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
        using SubControl control = new();
        Message m = default;
        Assert.False(control.ProcessKeyPreview(ref m));
    }

    [WinFormsFact]
    public void Control_ProcessKeyPreview_InvokeWithParent_ReturnsFalse()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Message m = default;
        Assert.False(control.ProcessKeyPreview(ref m));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ProcessKeyPreview_InvokeWithCustomParent_ReturnsExpected(bool result)
    {
        int callCount = 0;
        bool action(Message actualM)
        {
            Assert.Equal(1, actualM.Msg);
            callCount++;
            return result;
        }

        using CustomProcessControl parent = new()
        {
            ProcessKeyPreviewAction = action
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Message m = new()
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
        using SubControl control = new();
        Assert.False(control.ProcessMnemonic(charCode));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_RecreateHandle_InvokeWithHandle_Success()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_RecreateHandle_InvokeWithoutHandle_Nop()
    {
        using SubControl control = new();
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);

        // Invoke again.
        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_Refresh_InvokeWithoutHandle_Nop()
    {
        using Control control = new();
        control.Refresh();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Refresh();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_Refresh_InvokeWithHandle_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Refresh();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Refresh();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(1, 2)]
    [InlineData(0, 0)]
    [InlineData(-1, -2)]
    public void Control_RescaleConstantsForDpi_Invoke_Nop(int deviceDpiOld, int deviceDpiNew)
    {
        using SubControl control = new();
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_ResetBackColor_Invoke_Success()
    {
        using Control control = new();

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
        using SubControl control = new();

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
        using SubControl control = new();
        using Cursor cursor = new(1);

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
        using Control control = new();
        using Font font = new("Arial", 8.25f);

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
        using Control control = new();

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
        using SubControl control = new();

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
        using SubControl control = new();
        control.ResetMouseEventArgs();
        Assert.False(control.IsHandleCreated);

        // Invoke again.
        control.ResetMouseEventArgs();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_ResetMouseEventArgs_InvokeWithHandle_Success()
    {
        using SubControl control = new();
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
        using SubControl control = new();

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
        using SubControl control = new();

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

    public static IEnumerable<object[]> ResumeLayout_TestData()
    {
        yield return new object[] { null, null, true, 1 };
        yield return new object[] { new Control(), string.Empty, true, 1 };
        yield return new object[] { new Control(), "AffectedProperty", true, 1 };

        yield return new object[] { null, null, false, 0 };
        yield return new object[] { new Control(), string.Empty, false, 0 };
        yield return new object[] { new Control(), "AffectedProperty", false, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ResumeLayout_TestData))]
    public void Control_ResumeLayout_InvokeSuspendedWithLayoutRequest_Success(Control affectedControl, string affectedProperty, bool performLayout, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Same(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ResumeLayout_TestData))]
    public void Control_ResumeLayout_InvokeSuspendedWithMultipleLayoutRequests_Success(Control affectedControl, string affectedProperty, bool performLayout, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Same(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(new Control(), "AnotherAffectedProperty");
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ResumeLayout_InvokeSuspendedWithoutLayoutRequests_Success(bool performLayout)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ResumeLayout_TestData))]
    public void Control_ResumeLayout_InvokeSuspendedMultipleTimesWithLayoutRequest_Success(Control affectedControl, string affectedProperty, bool performLayout, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Same(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ResumeLayout_TestData))]
    public void Control_ResumeLayout_InvokeSuspendedMultipleTimesWithMultipleLayoutRequests_Success(Control affectedControl, string affectedProperty, bool performLayout, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Same(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        control.PerformLayout(new Control(), "AnotherAffectedProperty");
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ResumeLayout_InvokeSuspendedMultipleTimesWithoutLayoutRequests_Success(bool performLayout)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void Control_ResumeLayout_InvokeSuspendedWithChildren_Nop(bool performLayout, int expectedInitLayoutCallCount)
    {
        using Control child = new();
        using CustomLayoutEngineControl control = new();
        control.Controls.Add(child);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.InitLayout(child, BoundsSpecified.All))
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        mockLayoutEngine.Verify(e => e.InitLayout(child, BoundsSpecified.All), Times.Exactly(expectedInitLayoutCallCount));
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        mockLayoutEngine.Verify(e => e.InitLayout(child, BoundsSpecified.All), Times.Exactly(expectedInitLayoutCallCount * 2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ResumeLayout_InvokeNotSuspended_Nop(bool performLayout)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void Control_ResumeLayout_InvokeNotSuspendedWithChildren_Nop(bool performLayout, int expectedInitLayoutCallCount)
    {
        using Control child = new();
        using CustomLayoutEngineControl control = new();
        control.Controls.Add(child);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.InitLayout(child, BoundsSpecified.All))
            .Verifiable();
        control.SetLayoutEngine(mockLayoutEngine.Object);

        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        mockLayoutEngine.Verify(e => e.InitLayout(child, BoundsSpecified.All), Times.Exactly(expectedInitLayoutCallCount));
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(0, layoutCallCount);
        mockLayoutEngine.Verify(e => e.InitLayout(child, BoundsSpecified.All), Times.Exactly(expectedInitLayoutCallCount * 2));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ResumeLayout_TestData))]
    public void Control_ResumeLayout_InvokeSuspendedWithLayoutRequestWithHandle_Success(Control affectedControl, string affectedProperty, bool performLayout, int expectedLayoutCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Same(affectedProperty, e.AffectedProperty);
            layoutCallCount++;
        };

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        control.PerformLayout(affectedControl, affectedProperty);
        Assert.Equal(0, layoutCallCount);
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.ResumeLayout(performLayout);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateContent_TestData))]
    public void Control_RtlTranslateAlignment_InvokeContentAlignment_ReturnsExpected(RightToLeft rightToLeft, ContentAlignment align, ContentAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateLeftRight_TestData))]
    public void Control_RtlTranslateAlignment_InvokeLeftRightAlignment_ReturnsExpected(RightToLeft rightToLeft, LeftRightAlignment align, LeftRightAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateHorizontal_TestData))]
    public void Control_RtlTranslateAlignment_InvokeHorizontalAlignment_ReturnsExpected(RightToLeft rightToLeft, HorizontalAlignment align, HorizontalAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateAlignment(align));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RtlTranslateContent_TestData()
    {
        foreach (ContentAlignment align in Enum.GetValues(typeof(ContentAlignment)))
        {
            yield return new object[] { RightToLeft.No, align, align };
            yield return new object[] { RightToLeft.Inherit, align, align };
        }

        yield return new object[] { RightToLeft.Yes, ContentAlignment.BottomCenter, ContentAlignment.BottomCenter };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.BottomLeft, ContentAlignment.BottomRight };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.BottomRight, ContentAlignment.BottomLeft };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.MiddleCenter, ContentAlignment.MiddleCenter };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.MiddleRight, ContentAlignment.MiddleLeft };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.TopCenter, ContentAlignment.TopCenter };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.TopLeft, ContentAlignment.TopRight };
        yield return new object[] { RightToLeft.Yes, ContentAlignment.TopRight, ContentAlignment.TopLeft };
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateContent_TestData))]
    public void Control_RtlTranslateContent_Invoke_ReturnsExpected(RightToLeft rightToLeft, ContentAlignment align, ContentAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateContent(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateContent(align));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RtlTranslateLeftRight_TestData()
    {
        foreach (LeftRightAlignment align in Enum.GetValues(typeof(LeftRightAlignment)))
        {
            yield return new object[] { RightToLeft.No, align, align };
            yield return new object[] { RightToLeft.Inherit, align, align };
        }

        yield return new object[] { RightToLeft.Yes, LeftRightAlignment.Left, LeftRightAlignment.Right };
        yield return new object[] { RightToLeft.Yes, LeftRightAlignment.Right, LeftRightAlignment.Left };
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateLeftRight_TestData))]
    public void Control_RtlTranslateLeftRight_Invoke_ReturnsExpected(RightToLeft rightToLeft, LeftRightAlignment align, LeftRightAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateLeftRight(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateLeftRight(align));
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RtlTranslateHorizontal_TestData()
    {
        foreach (HorizontalAlignment align in Enum.GetValues(typeof(HorizontalAlignment)))
        {
            yield return new object[] { RightToLeft.No, align, align };
            yield return new object[] { RightToLeft.Inherit, align, align };
        }

        yield return new object[] { RightToLeft.Yes, HorizontalAlignment.Center, HorizontalAlignment.Center };
        yield return new object[] { RightToLeft.Yes, HorizontalAlignment.Left, HorizontalAlignment.Right };
        yield return new object[] { RightToLeft.Yes, HorizontalAlignment.Right, HorizontalAlignment.Left };
    }

    [WinFormsTheory]
    [MemberData(nameof(RtlTranslateHorizontal_TestData))]
    public void Control_RtlTranslateHorizontal_Invoke_ReturnsExpected(RightToLeft rightToLeft, HorizontalAlignment align, HorizontalAlignment expected)
    {
        using SubControl control = new()
        {
            RightToLeft = rightToLeft
        };
        Assert.Equal(expected, control.RtlTranslateHorizontal(align));
        Assert.False(control.IsHandleCreated);

        // Call again.
        Assert.Equal(expected, control.RtlTranslateHorizontal(align));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_SendToBack_InvokeWithoutHandleWithoutParent_Nop(bool topLevel)
    {
        using SubControl control = new();
        control.SetTopLevel(topLevel);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SendToBack();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(topLevel, control.IsHandleCreated);

        // Call again.
        control.SendToBack();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(topLevel, control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_SendToBack_InvokeWithoutHandleWithParent_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child1.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child1, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(2, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_SendToBack_InvokeWithHandleWithParentWithoutHandle_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child1.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child1, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, child1.Handle);
        int invalidatedCallCount = 0;
        child1.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child1.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child1.HandleCreated += (sender, e) => createdCallCount++;

        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(2, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(child2.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_SendToBack_InvokeWithHandleWithParentWithHandle_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child1.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child1, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        child1.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child1.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child1.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(2, parentLayoutCallCount);
        Assert.True(child1.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_SendToBack_InvokeWithoutHandleWithParentWithHandle_Success()
    {
        using Control parent = new();
        using SubControl child1 = new();
        using Control child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);
        Assert.Equal(new Control[] { child1, child2 }, parent.Controls.Cast<Control>());
        int layoutCallCount = 0;
        child1.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(child1, e.AffectedControl);
            Assert.Equal("ChildIndex", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        child1.DestroyHandle();
        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child1.SendToBack();
        Assert.Equal(new Control[] { child2, child1 }, parent.Controls.Cast<Control>());
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(2, parentLayoutCallCount);
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_SendToBack_InvokeWithHandleWithoutParent_Success(bool enabled, bool topLevel)
    {
        using SubControl control = new()
        {
            Enabled = enabled
        };
        control.SetTopLevel(topLevel);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SendToBack();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SendToBack();
        Assert.Equal(topLevel, control.GetTopLevel());
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
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
        using SubControl control = new();
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
    public void Control_SetAutoSizeMode_InvokeWithParent_GetAutoSizeModeReturnsExpected(AutoSizeMode mode, AutoSizeMode expected)
    {
        using Control parent = new();
        using SubControl control = new()
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
        using SubControl control = new();
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

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_TestData()
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
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_TestData))]
    public void Control_SetBounds_InvokeIntIntIntInt_Success(int x, int y, int width, int height, int expectedLayoutCallCount)
    {
        using SubControl control = new();
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

        control.SetBounds(x, y, width, height);
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
        control.SetBounds(x, y, width, height);
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

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, 1, 2, 30, 40, 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, 1, 2, 30, 40, 31, 40, 0 };
        yield return new object[] { new Size(30, 41), Size.Empty, 1, 2, 30, 40, 30, 41, 0 };
        yield return new object[] { new Size(40, 50), Size.Empty, 1, 2, 30, 40, 40, 50, 0 };
        yield return new object[] { Size.Empty, new Size(20, 10), 1, 2, 30, 40, 20, 10, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), 1, 2, 30, 40, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, 20, 30, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, 20, 30, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), 1, 2, 30, 40, 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), 1, 2, 30, 40, 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), 1, 2, 30, 40, 40, 50, 0 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), 1, 2, 30, 40, 40, 50, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithConstrainedSize_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntWithConstrainedSize_Success(Size minimumSize, Size maximumSize, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
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

        control.SetBounds(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBounds(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_WithCustomStyle_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, -7, -8, 1 };
        yield return new object[] { 1, 0, 0, 0, -4, -4, 1 };
        yield return new object[] { 0, 2, 0, 0, -4, -4, 1 };
        yield return new object[] { 1, 2, 0, 0, -4, -4, 1 };
        yield return new object[] { 0, 0, 1, 0, -3, -4, 1 };
        yield return new object[] { 0, 0, 0, 2, -4, -2, 1 };
        yield return new object[] { 0, 0, 1, 2, -3, -2, 1 };
        yield return new object[] { 1, 2, 30, 40, 26, 36, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithCustomStyle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntWithCustomStyle_Success(int x, int y, int width, int height, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
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

        control.SetBounds(x, y, width, height);
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
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBounds(x, y, width, height);
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
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_WithParent_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, 1, 2 };
        yield return new object[] { 1, 0, 0, 0, 0, 1 };
        yield return new object[] { 0, 2, 0, 0, 0, 1 };
        yield return new object[] { 1, 2, 0, 0, 0, 1 };
        yield return new object[] { 0, 0, 1, 0, 1, 2 };
        yield return new object[] { 0, 0, 0, 2, 1, 2 };
        yield return new object[] { 0, 0, 1, 2, 1, 2 };
        yield return new object[] { 1, 2, 30, 40, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParent_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntWithParent_Success(int x, int y, int width, int height, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.SetBounds(x, y, width, height);
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
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetBounds(x, y, width, height);
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
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_WithHandle_TestData()
    {
        yield return new object[] { true, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, -1, -2, -3, -4, 0, 0, 0, 0 };
        yield return new object[] { true, 1, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 0, 2, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 1, 2, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 0, 0, 1, 0, 1, 0, 1, 1 };
        yield return new object[] { true, 0, 0, 0, 2, 0, 2, 1, 1 };
        yield return new object[] { true, 0, 0, 1, 2, 1, 2, 1, 1 };
        yield return new object[] { true, 1, 2, 30, 40, 30, 40, 1, 1 };

        yield return new object[] { false, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, -1, -2, -3, -4, 0, 0, 0, 0 };
        yield return new object[] { false, 1, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 0, 2, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 1, 2, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 0, 0, 1, 0, 1, 0, 1, 0 };
        yield return new object[] { false, 0, 0, 0, 2, 0, 2, 1, 0 };
        yield return new object[] { false, 0, 0, 1, 2, 1, 2, 1, 0 };
        yield return new object[] { false, 1, 2, 30, 40, 30, 40, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithHandle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
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

        control.SetBounds(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SetBounds(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_WithParentWithHandle_TestData()
    {
        yield return new object[] { true, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, -1, -2, -3, -4, 0, 0, 0, 0, 1, 2 };
        yield return new object[] { true, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 1, 2, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 0, 0, 1, 0, 1, 0, 1, 1, 2, 2 };
        yield return new object[] { true, 0, 0, 0, 2, 0, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 0, 0, 1, 2, 1, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 1, 2, 30, 40, 30, 40, 1, 1, 2, 2 };

        yield return new object[] { false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, -1, -2, -3, -4, 0, 0, 0, 0, 1, 2 };
        yield return new object[] { false, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 1, 2, 0, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 0, 0, 1, 0, 1, 0, 1, 0, 2, 2 };
        yield return new object[] { false, 0, 0, 0, 2, 0, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 0, 0, 1, 2, 1, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 1, 2, 30, 40, 30, 40, 1, 0, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParentWithHandle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
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
            parentLayoutCallCount++;
        }

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

        try
        {
            control.SetBounds(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
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
            control.SetBounds(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.All, -1, -2, -3, -4, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.X, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.X, -1, 0, 0, 0, 0 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.X, 1, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.X, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.X, 1, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.X, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.X, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.X, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.X, 1, 0, 0, 0, 0 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Y, 0, -2, 0, 0, 0 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Y, 0, 2, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Y, 0, 2, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Y, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Y, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Y, 0, 2, 0, 0, 0 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Location, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Location, -1, -2, 0, 0, 0 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Location, 1, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Location, 0, 2, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Location, 1, 2, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Location, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Location, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Location, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Location, 1, 2, 0, 0, 0 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Width, 0, 0, -3, 0, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Width, 0, 0, 1, 0, 1 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Width, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Width, 0, 0, 1, 0, 1 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Width, 0, 0, 30, 0, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Height, 0, 0, 0, -4, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Height, 0, 0, 0, 2, 1 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Height, 0, 0, 0, 2, 1 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Height, 0, 0, 0, 40, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Size, 0, 0, -3, -4, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Size, 0, 0, 1, 0, 1 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Size, 0, 0, 0, 2, 1 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Size, 0, 0, 1, 2, 1 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Size, 0, 0, 30, 40, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.None, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.None, 0, 0, 0, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecified_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new();
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

        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(expectedX + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(expectedY + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(expectedX + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(expectedY + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 31, 40, 0 };
        yield return new object[] { new Size(30, 41), Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 41, 0 };
        yield return new object[] { new Size(40, 50), Size.Empty, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 40, 50, 0 };
        yield return new object[] { Size.Empty, new Size(20, 10), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 20, 10, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 20, 30, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 20, 30, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 40, 50, 0 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 40, 50, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_WithConstrainedSize_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecifiedWithConstrainedSize_Success(Size minimumSize, Size maximumSize, int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
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

        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_WithCustomStyle_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.All, -1, -2, -3, -4, -7, -8, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, -4, -4, 1 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, -4, -4, 1 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, -4, -4, 1 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, -3, -4, 1 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, -4, -2, 1 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, -3, -2, 1 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 26, 36, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_WithCustomStyle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecifiedWithCustomStyle_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
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

        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(expectedX + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(expectedY + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(expectedX + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(expectedY + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_WithParent_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.All, -1, -2, -3, -4, 1, 2 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0, 1 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0, 1 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0, 1 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1, 2 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1, 2 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1, 2 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1, 2 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.X, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.X, -1, 0, 0, 0, 0, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.X, 1, 0, 0, 0, 0, 1 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.X, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.X, 1, 0, 0, 0, 0, 1 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.X, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.X, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.X, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.X, 1, 0, 0, 0, 0, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Y, 0, -2, 0, 0, 0, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Y, 0, 2, 0, 0, 0, 1 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Y, 0, 2, 0, 0, 0, 1 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Y, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Y, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Y, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Y, 0, 2, 0, 0, 0, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Location, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Location, -1, -2, 0, 0, 0, 1 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Location, 1, 0, 0, 0, 0, 1 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Location, 0, 2, 0, 0, 0, 1 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Location, 1, 2, 0, 0, 0, 1 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Location, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Location, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Location, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Location, 1, 2, 0, 0, 0, 1 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Width, 0, 0, -3, 0, 1, 2 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Width, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Width, 0, 0, 1, 0, 1, 2 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Width, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Width, 0, 0, 1, 0, 1, 2 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Width, 0, 0, 30, 0, 1, 2 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Height, 0, 0, 0, -4, 1, 2 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Height, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Height, 0, 0, 0, 2, 1, 2 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Height, 0, 0, 0, 2, 1, 2 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Height, 0, 0, 0, 40, 1, 2 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.Size, 0, 0, -3, -4, 1, 2 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.Size, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.Size, 0, 0, 1, 0, 1, 2 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.Size, 0, 0, 0, 2, 1, 2 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.Size, 0, 0, 1, 2, 1, 2 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.Size, 0, 0, 30, 40, 1, 2 };

        yield return new object[] { 0, 0, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { -1, -2, -3, -4, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 0, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 2, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 0, 0, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 0, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 0, 2, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 0, 0, 1, 2, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { 1, 2, 30, 40, BoundsSpecified.None, 0, 0, 0, 0, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_WithParent_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecifiedWithParent_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.SetBounds(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(expectedX, control.Left);
            Assert.Equal(expectedX + expectedWidth, control.Right);
            Assert.Equal(expectedY, control.Top);
            Assert.Equal(expectedY + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetBounds(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(expectedX, control.Left);
            Assert.Equal(expectedX + expectedWidth, control.Right);
            Assert.Equal(expectedY, control.Top);
            Assert.Equal(expectedY + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_WithHandle_TestData()
    {
        yield return new object[] { true, 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, -1, -2, -3, -4, BoundsSpecified.All, -1, -2, 0, 0, 0, 0 };
        yield return new object[] { true, 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0, 0 };
        yield return new object[] { true, 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0, 0 };
        yield return new object[] { true, 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1, 1 };
        yield return new object[] { true, 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1, 1 };
        yield return new object[] { true, 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1, 1 };
        yield return new object[] { true, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1, 1 };

        yield return new object[] { false, 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, -1, -2, -3, -4, BoundsSpecified.All, -1, -2, 0, 0, 0, 0 };
        yield return new object[] { false, 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0, 0 };
        yield return new object[] { false, 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0, 0 };
        yield return new object[] { false, 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1, 0 };
        yield return new object[] { false, 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1, 0 };
        yield return new object[] { false, 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1, 0 };
        yield return new object[] { false, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_WithHandle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecifiedWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
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

        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SetBounds(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(expectedX, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(expectedY, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> SetBounds_Int_Int_Int_Int_BoundsSpecified_WithParentWithHandle_TestData()
    {
        yield return new object[] { true, 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, -1, -2, -3, -4, BoundsSpecified.All, -1, -2, 0, 0, 0, 0, 1, 2 };
        yield return new object[] { true, 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { true, 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1, 1, 2, 2 };
        yield return new object[] { true, 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1, 1, 2, 2 };

        yield return new object[] { false, 0, 0, 0, 0, BoundsSpecified.All, 0, 0, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, -1, -2, -3, -4, BoundsSpecified.All, -1, -2, 0, 0, 0, 0, 1, 2 };
        yield return new object[] { false, 1, 0, 0, 0, BoundsSpecified.All, 1, 0, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 0, 2, 0, 0, BoundsSpecified.All, 0, 2, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 1, 2, 0, 0, BoundsSpecified.All, 1, 2, 0, 0, 0, 0, 1, 1 };
        yield return new object[] { false, 0, 0, 1, 0, BoundsSpecified.All, 0, 0, 1, 0, 1, 0, 2, 2 };
        yield return new object[] { false, 0, 0, 0, 2, BoundsSpecified.All, 0, 0, 0, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 0, 0, 1, 2, BoundsSpecified.All, 0, 0, 1, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 1, 2, 30, 40, BoundsSpecified.All, 1, 2, 30, 40, 1, 0, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_BoundsSpecified_WithParentWithHandle_TestData))]
    public void Control_SetBounds_InvokeIntIntIntIntBoundsSpecifiedWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, BoundsSpecified specified, int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
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
            parentLayoutCallCount++;
        }

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

        try
        {
            control.SetBounds(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(expectedX, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(expectedY, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
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
            control.SetBounds(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(expectedX, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(expectedY, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(expectedX, expectedY, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetBoundsCore_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { 0, 0, 0, 0, specified, 0, 0 };
            yield return new object[] { -1, -2, -3, -4, specified, 1, 1 };
            yield return new object[] { 1, 0, 0, 0, specified, 1, 0 };
            yield return new object[] { 0, 2, 0, 0, specified, 1, 0 };
            yield return new object[] { 1, 2, 0, 0, specified, 1, 0 };
            yield return new object[] { 0, 0, 1, 0, specified, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, specified, 0, 1 };
            yield return new object[] { 0, 0, 1, 2, specified, 0, 1 };
            yield return new object[] { 1, 2, 30, 40, specified, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void Control_SetBoundsCore_Invoke_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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

        control.SetBoundsCore(x, y, width, height, specified);
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
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
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
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBoundsCore_WithConstrainedSize_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { Size.Empty, Size.Empty, 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { new Size(10, 20), Size.Empty, 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { new Size(30, 40), Size.Empty, 1, 2, 30, 40, specified, 30, 40, 1, 0 };
            yield return new object[] { new Size(31, 40), Size.Empty, 1, 2, 30, 40, specified, 31, 40, 1, 0 };
            yield return new object[] { new Size(30, 41), Size.Empty, 1, 2, 30, 40, specified, 30, 41, 1, 0 };
            yield return new object[] { new Size(40, 50), Size.Empty, 1, 2, 30, 40, specified, 40, 50, 1, 0 };
            yield return new object[] { Size.Empty, new Size(20, 10), 1, 2, 30, 40, specified, 20, 10, 1, 1 };
            yield return new object[] { Size.Empty, new Size(30, 40), 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { Size.Empty, new Size(31, 40), 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { Size.Empty, new Size(30, 41), 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { Size.Empty, new Size(40, 50), 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { new Size(10, 20), new Size(40, 50), 1, 2, 30, 40, specified, 30, 40, 1, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, specified, 20, 30, 1, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, specified, 20, 30, 1, 1 };
            yield return new object[] { new Size(30, 40), new Size(20, 30), 1, 2, 30, 40, specified, 30, 40, 1, 0 };
            yield return new object[] { new Size(30, 40), new Size(40, 50), 1, 2, 30, 40, specified, 30, 40, 1, 0 };
            yield return new object[] { new Size(40, 50), new Size(20, 30), 1, 2, 30, 40, specified, 40, 50, 1, 0 };
            yield return new object[] { new Size(40, 50), new Size(40, 50), 1, 2, 30, 40, specified, 40, 50, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_WithConstrainedSize_TestData))]
    public void Control_SetBoundsCore_InvokeWithConstrainedSize_Success(Size minimumSize, Size maximumSize, int x, int y, int width, int height, BoundsSpecified specified, int expectedWidth, int expectedHeight, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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

        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetBoundsCore_WithCustomStyle_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { 0, 0, 0, 0, specified, 0, 0, 0, 0 };
            yield return new object[] { -1, -2, -3, -4, specified, -7, -8, 1, 1 };
            yield return new object[] { 1, 0, 0, 0, specified, -4, -4, 1, 1 };
            yield return new object[] { 0, 2, 0, 0, specified, -4, -4, 1, 1 };
            yield return new object[] { 1, 2, 0, 0, specified, -4, -4, 1, 1 };
            yield return new object[] { 0, 0, 1, 0, specified, -3, -4, 0, 1 };
            yield return new object[] { 0, 0, 0, 2, specified, -4, -2, 0, 1 };
            yield return new object[] { 0, 0, 1, 2, specified, -3, -2, 0, 1 };
            yield return new object[] { 1, 2, 30, 40, specified, 26, 36, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_WithCustomStyle_TestData))]
    public void Control_SetBoundsCore_InvokeWithCustomStyle_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedClientWidth, int expectedClientHeight, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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

        control.SetBoundsCore(x, y, width, height, specified);
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
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
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
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_TestData))]
    public void Control_SetBoundsCore_InvokeWithParent_Success(int x, int y, int width, int height, BoundsSpecified specified, int expectedLocationChangedCallCount, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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
        }

        parent.Layout += parentHandler;

        try
        {
            control.SetBoundsCore(x, y, width, height, specified);
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
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.SetBoundsCore(x, y, width, height, specified);
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
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetBoundsCore_WithHandle_TestData()
    {
        foreach (BoundsSpecified specified in Enum.GetValues(typeof(BoundsSpecified)))
        {
            yield return new object[] { true, 0, 0, 0, 0, specified, 0, 0, 0, 0, 0 };
            yield return new object[] { true, -1, -2, -3, -4, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { true, 1, 0, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { true, 0, 2, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { true, 1, 2, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { true, 0, 0, 1, 0, specified, 1, 0, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 0, 2, specified, 0, 2, 0, 1, 1 };
            yield return new object[] { true, 0, 0, 1, 2, specified, 1, 2, 0, 1, 1 };
            yield return new object[] { true, 1, 2, 30, 40, specified, 30, 40, 1, 1, 1 };

            yield return new object[] { false, 0, 0, 0, 0, specified, 0, 0, 0, 0, 0 };
            yield return new object[] { false, -1, -2, -3, -4, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { false, 1, 0, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { false, 0, 2, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { false, 1, 2, 0, 0, specified, 0, 0, 1, 0, 0 };
            yield return new object[] { false, 0, 0, 1, 0, specified, 1, 0, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 0, 2, specified, 0, 2, 0, 1, 0 };
            yield return new object[] { false, 0, 0, 1, 2, specified, 1, 2, 0, 1, 0 };
            yield return new object[] { false, 1, 2, 30, 40, specified, 30, 40, 1, 1, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBoundsCore_WithHandle_TestData))]
    public void Control_SetBoundsCore_InvokeWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, BoundsSpecified specified, int expectedWidth, int expectedHeight, int expectedLocationChangedCallCount, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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

        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SetBoundsCore(x, y, width, height, specified);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
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
    [MemberData(nameof(SetBoundsCore_WithHandle_TestData))]
    public void Control_SetBoundsCore_InvokeWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, BoundsSpecified specified, int expectedWidth, int expectedHeight, int expectedLocationChangedCallCount, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            Assert.Equal(layoutCallCount, moveCallCount);
            Assert.Equal(resizeCallCount, moveCallCount);
            Assert.Equal(sizeChangedCallCount, moveCallCount);
            Assert.Equal(clientSizeChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            Assert.Equal(layoutCallCount, locationChangedCallCount);
            Assert.Equal(resizeCallCount, locationChangedCallCount);
            Assert.Equal(sizeChangedCallCount, locationChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, locationChangedCallCount);
            locationChangedCallCount++;
        };
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
        }

        parent.Layout += parentHandler;

        try
        {
            control.SetBoundsCore(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
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
            control.SetBoundsCore(x, y, width, height, specified);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ClientSize_Set_TestData))]
    public void Control_SetClientSizeCore_Invoke_GetReturnsExpected(Size value, int expectedLayoutCallCount)
    {
        using SubControl control = new();
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
        using BorderedControl control = new();
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
        using SubControl control = new();
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
        using SubControl control = new();
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
        using SubControl control = new();
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
        using SubControl control = new();
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
        using SubControl control = new()
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
        using SubControl control = new()
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
        using Control parent = new();
        using SubControl control = new()
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
        using SubControl control = new();
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
        using BorderedControl control = new();
        Size expected = new(clientSize.Width + 4, clientSize.Height + 4);
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
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_STATICEDGE;
                return cp;
            }
        }

        public new Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => base.GetScaledBounds(bounds, factor, specified);

        public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified); public new void SetClientSizeCore(int width, int height) => base.SetClientSizeCore(width, height);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

        public new Size SizeFromClientSize(Size clientSize) => base.SizeFromClientSize(clientSize);

        public new void UpdateBounds() => base.UpdateBounds();

        public new void UpdateBounds(int x, int y, int width, int height) => base.UpdateBounds(x, y, width, height);

        public new void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight) => base.UpdateBounds(x, y, width, height, clientWidth, clientHeight);
    }

    [WinFormsTheory]
    [MemberData(nameof(SizeFromClientSize_TestData))]
    public void Control_SizeFromClientSize_InvokeWithHandle_ReturnsExpected(Size clientSize)
    {
        using SubControl control = new();
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

    [WinFormsTheory]
    [BoolData]
    public void Control_Show_Invoke_Success(bool visible)
    {
        using Control control = new()
        {
            Visible = visible
        };
        control.Show();
        Assert.True(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Show();
        Assert.True(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Show_InvokeWithHandle_Success(bool visible)
    {
        using Control control = new()
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

        control.Show();
        Assert.True(control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Show();
        Assert.True(control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_SuspendLayout_Invoke_Success()
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_SuspendLayout_InvokeWithHandle_Success()
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.SuspendLayout();
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_ToString_Invoke_ReturnsExpected()
    {
        using Control control = new();
        Assert.Equal("System.Windows.Forms.Control", control.ToString());
    }

    [WinFormsFact]
    public void Control_Update_InvokeWithoutHandle_Nop()
    {
        using Control control = new();
        control.Update();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Update();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_Update_InvokeWithHandle_Success()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Update();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Update();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_UpdateBounds_Invoke_Success()
    {
        using SubControl control = new();
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
        using SubControl control = new();
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
        using SubControl control = new()
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
        using SubControl parent = new()
        {
            Bounds = new Rectangle(10, 20, 30, 40)
        };
        using SubControl control = new()
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
        using SubControl control = new();
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
        using SubControl control = new()
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
        using SubControl parent = new()
        {
            Bounds = new Rectangle(10, 20, 30, 40)
        };
        using SubControl control = new()
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
    public void Control_UpdateBounds_InvokeIntIntIntInt_Success(int x, int y, int width, int height, int expectedLayoutCallCount)
    {
        using SubControl control = new();
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

    public static IEnumerable<object[]> UpdateBounds_Int_Int_Int_Int_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(30, 41), Size.Empty, 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(40, 50), Size.Empty, 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(20, 10), 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), 1, 2, 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), 1, 2, 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), 1, 2, 30, 40, 1 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), 1, 2, 30, 40, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(UpdateBounds_Int_Int_Int_Int_WithConstrainedSize_TestData))]
    public void Control_UpdateBounds_InvokeIntIntIntIntWithConstrainedSize_Success(Size minimumSize, Size maximumSize, int x, int y, int width, int height, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
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
    public void Control_UpdateBounds_InvokeIntIntIntIntWithCustomStyle_Success(int x, int y, int width, int height, int expectedClientWidth, int expectedClientHeight)
    {
        using BorderedControl control = new();
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
    public void Control_UpdateBounds_InvokeIntIntIntIntWithParent_Success(int x, int y, int width, int height, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
        }

        parent.Layout += parentHandler;

        try
        {
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
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
    public void Control_UpdateBounds_InvokeIntIntIntIntWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
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
    public void Control_UpdateBounds_InvokeIntIntIntIntWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
        }

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

        try
        {
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
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
    public void Control_UpdateBounds_InvokeIntIntIntIntIntInt_Success(int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new();
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
    public void Control_UpdateBounds_InvokeIntIntIntIntIntIntWithParent_Success(int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
        }

        parent.Layout += parentHandler;

        try
        {
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
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
    public void Control_UpdateBounds_InvokeIntIntIntIntIntIntWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
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
    public void Control_UpdateBounds_InvokeIntIntIntIntIntIntWithParentWithHandle_Success(bool resizeRedraw, int x, int y, int width, int height, int clientWidth, int clientHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
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
        }

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

        try
        {
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
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void Control_UpdateStyles_InvokeWithoutHandle_Success()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_UpdateStyles_InvokeWithHandle_Success()
    {
        using SubControl control = new();
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

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithoutParent_Nop()
    {
        using SubControl control = new();
        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithoutHandleWithParentWithoutHandle_Nop()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };

        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithHandleWithParentWithoutHandle_Nop()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UpdateZOrder();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        control.UpdateZOrder();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithoutHandleWithParentWithHandle_Success()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;
        control.DestroyHandle();

        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        control.UpdateZOrder();
        Assert.False(control.IsHandleCreated);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithHandleWithParentWithHandleOnlyChild_Success()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
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

        control.UpdateZOrder();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        control.UpdateZOrder();
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithHandleWithParentWithHandleMultipleChildren_Success()
    {
        using Control parent = new();
        using Control child1 = new();
        using SubControl child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);

        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        child2.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child2.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child2.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        child2.UpdateZOrder();
        Assert.True(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child2.UpdateZOrder();
        Assert.True(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_UpdateZOrder_InvokeWithHandleWithParentWithHandleMultipleChildrenWithoutHandle_Success()
    {
        using Control parent = new();
        using SubControl child1 = new();
        using SubControl child2 = new();
        parent.Controls.Add(child1);
        parent.Controls.Add(child2);

        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        child2.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        child2.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        child2.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;
        child1.DestroyHandle();

        child2.UpdateZOrder();
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Call again.
        child2.UpdateZOrder();
        Assert.False(child1.IsHandleCreated);
        Assert.True(child2.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeCaptureChangedWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int callCount = 0;
            control.MouseCaptureChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeCaptureChangedWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseCaptureChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CAPTURECHANGED,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeCancelMode_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int callCount = 0;
            control.LostFocus += (sender, e) => callCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CANCELMODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeCancelModeWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        control.LostFocus += (sender, e) => callCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CANCELMODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithoutContextMenuStrip_TestData()
    {
        yield return new object[] { new Size(10, 20), (IntPtr)(-1) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2) };

        yield return new object[] { Size.Empty, (IntPtr)(-1) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void Control_WndProc_InvokeContextMenuWithoutContextMenuStripWithoutHandle_Success(Size size, IntPtr lParam)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new()
            {
                Size = size
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        yield return new object[] { new Size(10, 20), (IntPtr)(-1), (IntPtr)250, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0), IntPtr.Zero, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2), IntPtr.Zero, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2), IntPtr.Zero, true };

        yield return new object[] { Size.Empty, (IntPtr)(-1), IntPtr.Zero, false };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(p.X, p.Y), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData))]
    public void Control_WndProc_InvokeContextMenuWithContextMenuStripWithoutHandle_Success(Size size, IntPtr lParam, IntPtr expectedResult, bool expectedHandleCreated)
    {
        using (new NoAssertContext())
        {
            using ContextMenuStrip menu = new();
            using SubControl control = new()
            {
                ContextMenuStrip = menu,
                Size = size
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(menu.Visible);
            Assert.Equal(expectedResult == 250, menu.SourceControl == control);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void Control_WndProc_InvokeContextMenuWithoutContextMenuStripWithHandle_Success(Size size, IntPtr lParam)
    {
        using SubControl control = new()
        {
            Size = size
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        yield return new object[] { new Size(10, 20), (IntPtr)(-1), (IntPtr)250 };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0), IntPtr.Zero };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2), IntPtr.Zero };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250 };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2), IntPtr.Zero };

        yield return new object[] { Size.Empty, (IntPtr)(-1), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(p.X, p.Y), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2), IntPtr.Zero };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithHandle_TestData))]
    public void Control_WndProc_InvokeContextMenuWithContextMenuStripWithHandle_Success(Size size, IntPtr lParam, IntPtr expectedResult)
    {
        using ContextMenuStrip menu = new();
        using SubControl control = new()
        {
            ContextMenuStrip = menu,
            Size = size
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.False(menu.Visible);
        Assert.Equal(expectedResult == 250, menu.SourceControl == control);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeDpiChangedAfterParentWithoutHandle_Success()
    {
        // Set thread awareness context to PermonitorV2(PMv2).
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using (new NoAssertContext())
            {
                using SubControl control = new();
                int callCount = 0;
                control.DpiChangedAfterParent += (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(EventArgs.Empty, e);
                    callCount++;
                };
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_DPICHANGED_AFTERPARENT,
                    WParam = PARAM.FromLowHigh(192, 192),
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(0, m.Result);
                Assert.Equal(1, callCount);
                Assert.False(control.IsHandleCreated);
            }
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeDpiChangedAfterParentWithHandle_Success()
    {
        // Set thread awareness context to PermonitorV2(PMv2).
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using SubControl control = new();
            Assert.NotEqual(0, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int callCount = 0;
            control.DpiChangedAfterParent += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_DPICHANGED_AFTERPARENT,
                WParam = PARAM.FromLowHigh(192, 192),
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(0, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeDpiChangedBeforeParentWithoutHandle_Success()
    {
        // Set thread awareness context to PermonitorV2(PMv2).
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using (new NoAssertContext())
            {
                using SubControl control = new();
                int callCount = 0;
                control.DpiChangedBeforeParent += (sender, e) =>
                {
                    Assert.Same(control, sender);
                    Assert.Same(EventArgs.Empty, e);
                    callCount++;
                };
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT,
                    WParam = PARAM.FromLowHigh(192, 192),
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(0, m.Result);
                Assert.Equal(1, callCount);
                Assert.False(control.IsHandleCreated);
            }
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeDpiChangedBeforeParentWithHandle_Success()
    {
        // Set thread awareness context to PermonitorV2(PMv2).
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using SubControl control = new();
            Assert.NotEqual(0, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            int callCount = 0;
            control.DpiChangedBeforeParent += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT,
                WParam = PARAM.FromLowHigh(192, 192),
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(0, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    public static IEnumerable<object[]> WndProc_EraseBkgndWithoutHandleWithoutWParam_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { true, true, opaque, (IntPtr)1, false };
            yield return new object[] { true, false, opaque, IntPtr.Zero, false };
            yield return new object[] { false, true, opaque, IntPtr.Zero, false };
            yield return new object[] { false, false, opaque, IntPtr.Zero, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_EraseBkgndWithoutHandleWithoutWParam_TestData))]
    public void Control_WndProc_InvokeEraseBkgndWithoutHandleWithoutWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque, IntPtr expectedResult, bool expectedIsHandleCreated)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_ERASEBKGND,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
            Assert.Equal(0, paintCallCount);
        }
    }

    public static IEnumerable<object[]> WndProc_EraseBkgndWithoutHandleWithWParam_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { true, true, opaque, (IntPtr)1, false };
            yield return new object[] { true, false, opaque, (IntPtr)1, true };
            yield return new object[] { false, true, opaque, IntPtr.Zero, false };
            yield return new object[] { false, false, opaque, IntPtr.Zero, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_EraseBkgndWithoutHandleWithWParam_TestData))]
    public void Control_WndProc_InvokeEraseBkgndWithoutHandleWithWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque, IntPtr expectedResult, bool expectedIsHandleCreated)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            using Bitmap image = new(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_ERASEBKGND,
                    WParam = hdc,
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(expectedResult, m.Result);
                Assert.Equal(expectedIsHandleCreated, control.IsHandleCreated);
                Assert.Equal(0, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    public static IEnumerable<object[]> WndProc_EraseBkgndWithHandleWithoutWParam_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { true, true, opaque, (IntPtr)1 };
            yield return new object[] { true, false, opaque, IntPtr.Zero };
            yield return new object[] { false, true, opaque, IntPtr.Zero };
            yield return new object[] { false, false, opaque, IntPtr.Zero };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_EraseBkgndWithHandleWithoutWParam_TestData))]
    public void Control_WndProc_InvokeEraseBkgndWithHandleWithoutWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque, IntPtr expectedResult)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
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

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_ERASEBKGND,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, paintCallCount);
    }

    public static IEnumerable<object[]> WndProc_EraseBkgndWithHandleWithWParam_TestData()
    {
        foreach (bool opaque in new bool[] { true, false })
        {
            yield return new object[] { true, true, opaque, (IntPtr)1 };
            yield return new object[] { true, false, opaque, (IntPtr)1 };
            yield return new object[] { false, true, opaque, IntPtr.Zero };
            yield return new object[] { false, false, opaque, IntPtr.Zero };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_EraseBkgndWithHandleWithWParam_TestData))]
    public void Control_WndProc_InvokeEraseBkgndWithHandleWithWParam_Success(bool userPaint, bool allPaintingInWmPaint, bool opaque, IntPtr expectedResult)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.AllPaintingInWmPaint, allPaintingInWmPaint);
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

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_ERASEBKGND,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, paintCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeGetDlgCodeWithoutHandle_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_GETDLGCODE,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeGetDlgCodeWithHandle_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_GETDLGCODE,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeImeNotifyCallCountWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int imeModeChangedCallCount = 0;
            control.ImeModeChanged += (sender, e) => imeModeChangedCallCount++;
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_IME_NOTIFY,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, imeModeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeImeNotifyCallCountWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int imeModeChangedCallCount = 0;
        control.ImeModeChanged += (sender, e) => imeModeChangedCallCount++;
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_IME_NOTIFY,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, imeModeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeKillFocusWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int callCount = 0;
            control.LostFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_KILLFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeKillFocusWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.LostFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_KILLFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_PrintClientWithoutHandleWithoutWParam_TestData()
    {
        yield return new object[] { true, true, (IntPtr)250, 0 };
        yield return new object[] { false, true, IntPtr.Zero, 0 };
        yield return new object[] { false, false, IntPtr.Zero, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PrintClientWithoutHandleWithoutWParam_TestData))]
    public void Control_WndProc_InvokePrintClientWithoutHandleWithoutWParam_Success(bool userPaint, bool opaque, IntPtr expectedResult, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PRINTCLIENT,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokePrintClientWithoutHandleWithoutWParamUserPaint_DoesNotThrow()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserPaint, true);
            control.SetStyle(ControlStyles.Opaque, false);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PRINTCLIENT,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(250, m.Result);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, paintCallCount);
        }
    }

    public static IEnumerable<object[]> WndProc_PrintClientWithoutHandleWithWParam_TestData()
    {
        yield return new object[] { true, true, (IntPtr)250, 1 };
        yield return new object[] { true, false, (IntPtr)250, 1 };
        yield return new object[] { false, true, IntPtr.Zero, 0 };
        yield return new object[] { false, false, IntPtr.Zero, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PrintClientWithoutHandleWithWParam_TestData))]
    public void Control_WndProc_InvokePrintClientWithoutHandleWithWParam_Success(bool userPaint, bool opaque, IntPtr expectedResult, int expectedPaintCallCount)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            control.SetStyle(ControlStyles.Opaque, opaque);
            int paintCallCount = 0;
            control.Paint += (sender, e) => paintCallCount++;

            using Bitmap image = new(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                Message m = new()
                {
                    Msg = (int)PInvokeCore.WM_PRINTCLIENT,
                    WParam = hdc,
                    Result = 250
                };
                control.WndProc(ref m);
                Assert.Equal(expectedResult, m.Result);
                Assert.False(control.IsHandleCreated);
                Assert.Equal(expectedPaintCallCount, paintCallCount);
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    public static IEnumerable<object[]> WndProc_PrintClientWithHandleWithoutWParam_TestData()
    {
        yield return new object[] { true, true, (IntPtr)250, 0 };
        yield return new object[] { false, true, IntPtr.Zero, 0 };
        yield return new object[] { false, false, IntPtr.Zero, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PrintClientWithHandleWithoutWParam_TestData))]
    public void Control_WndProc_InvokePrintClientWithHandleWithoutWParam_Success(bool userPaint, bool opaque, IntPtr expectedResult, int expectedPaintCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.Opaque, opaque);
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_PRINTCLIENT,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(expectedPaintCallCount, paintCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokePrintClientWithHandleWithoutWParamUserPaint_DoesNotThrow()
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, true);
        control.SetStyle(ControlStyles.Opaque, false);
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_PRINTCLIENT,
            Result = 250
        };

        control.WndProc(ref m);
        Assert.Equal(250, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, paintCallCount);
    }

    public static IEnumerable<object[]> WndProc_PrintClientWithHandleWithWParam_TestData()
    {
        yield return new object[] { true, true, (IntPtr)250, 1 };
        yield return new object[] { true, false, (IntPtr)250, 1 };
        yield return new object[] { false, true, IntPtr.Zero, 0 };
        yield return new object[] { false, false, IntPtr.Zero, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_PrintClientWithHandleWithWParam_TestData))]
    public void Control_WndProc_InvokePrintClientWithHandleWithWParam_Success(bool userPaint, bool opaque, IntPtr expectedResult, int expectedPaintCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        control.SetStyle(ControlStyles.Opaque, opaque);
        int paintCallCount = 0;
        control.Paint += (sender, e) => paintCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_PRINTCLIENT,
                WParam = hdc,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(expectedPaintCallCount, paintCallCount);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    public static IEnumerable<object[]> WndProc_MouseDown_TestData()
    {
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDOWN, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 2, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 2, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONDBLCLK, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 2, -1, -2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void Control_WndProc_InvokeMouseDownWithoutHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            int callCount = 0;
            control.MouseDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void Control_WndProc_InvokeMouseDownWithoutHandleNotSelectable_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            control.SetStyle(ControlStyles.Selectable, false);
            int callCount = 0;
            control.MouseDown += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_LBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDBLCLK)]
    public void Control_WndProc_InvokeMouseDownWithoutHandleNotEnabled_DoesNotCallMouseDown(int msg)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new()
            {
                Enabled = false
            };
            int callCount = 0;
            control.MouseDown += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = msg,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(0, callCount);
            Assert.True(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void Control_WndProc_InvokeMouseDownWithHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseDown_TestData))]
    public void Control_WndProc_InvokeMouseDownWithHandleNotSelectable_DoesNotCallMouseDown(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        control.SetStyle(ControlStyles.Selectable, false);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_LBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_MBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_RBUTTONDBLCLK)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDOWN)]
    [InlineData((int)PInvokeCore.WM_XBUTTONDBLCLK)]
    public void Control_WndProc_InvokeMouseDownWithHandleNotEnabled_DoesNotCallMouseDown(int msg)
    {
        using SubControl control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseDown += (sender, e) => callCount++;
        Message m = new()
        {
            Msg = msg,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(0, callCount);
        Assert.True(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeMouseHoverWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int callCount = 0;
            control.MouseHover += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_MOUSEHOVER,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseHover += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_MOUSEHOVER,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_MouseUp_TestData()
    {
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Left, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_LBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Left, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Middle, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_MBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Middle, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.Right, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_RBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.Right, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, (IntPtr)250, MouseButtons.None, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, IntPtr.Zero, MouseButtons.None, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), (IntPtr)250, MouseButtons.XButton1, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(2, 1), IntPtr.Zero, MouseButtons.XButton1, 1, -1, -2 };

        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { true, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), (IntPtr)250, MouseButtons.XButton2, 1, -1, -2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 0, 0 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(1, 2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, 1, 2 };
        yield return new object[] { false, (int)PInvokeCore.WM_XBUTTONUP, PARAM.FromLowHigh(-1, -2), PARAM.FromLowHigh(1, 2), IntPtr.Zero, MouseButtons.XButton2, 1, -1, -2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void Control_WndProc_InvokeMouseUpWithoutHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            int callCount = 0;
            control.MouseUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void Control_WndProc_InvokeMouseUpWithoutHandleNotSelectable_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            control.SetStyle(ControlStyles.UserMouse, userMouse);
            control.SetStyle(ControlStyles.Selectable, false);
            int callCount = 0;
            control.MouseUp += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(expectedButton, e.Button);
                Assert.Equal(expectedClicks, e.Clicks);
                Assert.Equal(expectedX, e.X);
                Assert.Equal(expectedY, e.Y);
                Assert.Equal(0, e.Delta);
                callCount++;
            };
            Message m = new()
            {
                Msg = msg,
                LParam = lParam,
                WParam = wParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_MBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_RBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_XBUTTONUP)]
    public void Control_WndProc_InvokeMouseUpWithoutHandleNotEnabled_CallsMouseUp(int msg)
    {
        using (new NoAssertContext())
        {
            using SubControl control = new()
            {
                Enabled = false
            };
            int callCount = 0;
            control.MouseUp += (sender, e) => callCount++;
            Message m = new()
            {
                Msg = msg,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.False(control.Capture);
            Assert.False(control.Focused);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void Control_WndProc_InvokeMouseUpWithHandle_Success(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseUp += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_MouseUp_TestData))]
    public void Control_WndProc_InvokeMouseUpWithHandleNotSelectable_DoesNotCallMouseUp(bool userMouse, int msg, IntPtr lParam, IntPtr wParam, IntPtr expectedResult, MouseButtons expectedButton, int expectedClicks, int expectedX, int expectedY)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserMouse, userMouse);
        control.SetStyle(ControlStyles.Selectable, false);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseUp += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expectedButton, e.Button);
            Assert.Equal(expectedClicks, e.Clicks);
            Assert.Equal(expectedX, e.X);
            Assert.Equal(expectedY, e.Y);
            Assert.Equal(0, e.Delta);
            callCount++;
        };
        Message m = new()
        {
            Msg = msg,
            LParam = lParam,
            WParam = wParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData((int)PInvokeCore.WM_LBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_MBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_RBUTTONUP)]
    [InlineData((int)PInvokeCore.WM_XBUTTONUP)]
    public void Control_WndProc_InvokeMouseUpWithHandleNotEnabled_CallsMouseUp(int msg)
    {
        using SubControl control = new()
        {
            Enabled = false
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.MouseUp += (sender, e) => callCount++;
        Message m = new()
        {
            Msg = msg,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.False(control.Capture);
        Assert.False(control.Focused);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithoutHandle_Success()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            int callCount = 0;
            control.GotFocus += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_SETFOCUS,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithHandle_Success()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithParentContainerControlWithHandle_Success()
    {
        using ContainerControl parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Null(parent.ActiveControl);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.Same(control, parent.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_SetFocusWithParentIContainer_TestData()
    {
        yield return new object[] { true, true, IntPtr.Zero, 1, 1 };
        yield return new object[] { true, false, (IntPtr)250, 1, 0 };
        yield return new object[] { false, true, IntPtr.Zero, 0, 1 };
        yield return new object[] { false, false, IntPtr.Zero, 0, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_SetFocusWithParentIContainer_TestData))]
    public void Control_WndProc_InvokeSetFocusWithParentIContainerControlWithHandle_Success(bool containerControl, bool enabled, IntPtr expectedResult, int expectedActivateControlCallCount, int expectedCallCount)
    {
        using CustomContainerControl parent = new();
        parent.SetStyle(ControlStyles.ContainerControl, containerControl);
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Null(parent.ActiveControl);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int activateControlCallCount = 0;
        parent.ActivateControlAction = (active) =>
        {
            Assert.Same(control, active);
            activateControlCallCount++;
            return enabled;
        };

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.Equal(expectedCallCount, callCount);
        Assert.Equal(expectedActivateControlCallCount, activateControlCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    private class CustomContainerControl : Control, IContainerControl
    {
        public CustomContainerControl()
        {
            SetStyle(ControlStyles.ContainerControl, true);
        }

        public Control ActiveControl { get; set; }

        public Func<Control, bool> ActivateControlAction { get; set; }

        public bool ActivateControl(Control active) => ActivateControlAction(active);

        public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithParentContainerControlWithParentContainerControlWithHandle_Success()
    {
        using ContainerControl grandparent = new();
        using ContainerControl parent = new()
        {
            Parent = grandparent
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Null(parent.ActiveControl);
        Assert.Null(grandparent.ActiveControl);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.Same(control, parent.ActiveControl);
        Assert.Same(parent, grandparent.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithParentContainerControlWithParentControlWithHandle_Success()
    {
        using Control grandparent = new();
        using ContainerControl parent = new()
        {
            Parent = grandparent
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Null(parent.ActiveControl);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.Same(control, parent.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithParentContainerControlWithParentContainerControlAlreadyActiveWithHandle_Success()
    {
        using ContainerControl grandparent = new();
        using ContainerControl parent = new()
        {
            Parent = grandparent
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        parent.ActiveControl = control;
        grandparent.ActiveControl = parent;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.Same(control, parent.ActiveControl);
        Assert.Same(parent, grandparent.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFocusWithParentContainerControlWithParentWithHandle_Success()
    {
        using Control grandparent = new();
        using ContainerControl parent = new()
        {
            Parent = grandparent
        };
        using SubControl control = new()
        {
            Parent = parent
        };
        Assert.Null(parent.ActiveControl);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        control.GotFocus += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFOCUS,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.Same(control, parent.ActiveControl);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFontWithoutHandle_ReturnsExpected()
    {
        using (new NoAssertContext())
        {
            using SubControl control = new();
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_SETFONT,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    [WinFormsFact]
    public void Control_WndProc_InvokeSetFontWithHandle_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_SETFONT,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    private class NoCreateControl : Control
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvokeCore.WM_NCCREATE)
            {
                m.Result = IntPtr.Zero;
                return;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        public new void CreateHandle() => base.CreateHandle();
    }
}
