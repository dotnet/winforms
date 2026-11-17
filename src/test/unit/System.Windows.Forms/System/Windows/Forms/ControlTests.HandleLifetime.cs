// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    // Scenario: reading Handle on a control with no window creates exactly one real window owned by the
    // current process and UI thread, raises HandleCreated once, and maps back to the control.
    [WinFormsFact]
    public unsafe void Control_Handle_Access_CreatesSingleOwnedWindow()
    {
        using SubControl control = new();
        int createdCount = 0;
        bool windowValidInsideEvent = false;
        control.HandleCreated += (sender, e) =>
        {
            createdCount++;
            windowValidInsideEvent = control.IsHandleCreated && PInvoke.IsWindow((HWND)control.Handle);
        };

        Assert.False(control.IsHandleCreated);
        IntPtr handle = control.Handle;
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, createdCount);
        Assert.True(windowValidInsideEvent);
        Assert.True(PInvoke.IsWindow((HWND)handle));
        Assert.Same(control, Control.FromHandle(handle));

        uint processId;
        uint threadId = PInvokeCore.GetWindowThreadProcessId((HWND)handle, &processId);
        Assert.Equal(PInvokeCore.GetCurrentThreadId(), threadId);
        Assert.Equal((uint)Environment.ProcessId, processId);

        // Further reads reuse the window and do not raise HandleCreated again.
        Assert.Equal(handle, control.Handle);
        Assert.Equal(1, createdCount);
    }

    // Scenario: DestroyHandle raises HandleDestroyed while the window still exists, then destroys the
    // window and detaches it from the control; a new access creates a different window.
    [WinFormsFact]
    public void Control_DestroyHandle_DestroysWindowAndDetachesControl()
    {
        using SubControl control = new();
        IntPtr oldHandle = control.Handle;
        int destroyedCount = 0;
        bool windowValidInsideEvent = false;
        control.HandleDestroyed += (sender, e) =>
        {
            destroyedCount++;
            windowValidInsideEvent = PInvoke.IsWindow((HWND)oldHandle);
        };

        control.DestroyHandle();

        Assert.Equal(1, destroyedCount);
        Assert.True(windowValidInsideEvent);
        Assert.False(control.IsHandleCreated);
        Assert.False(PInvoke.IsWindow((HWND)oldHandle));
        Assert.Null(Control.FromHandle(oldHandle));

        IntPtr newHandle = control.Handle;
        Assert.NotEqual(IntPtr.Zero, newHandle);
        Assert.True(PInvoke.IsWindow((HWND)newHandle));
        Assert.Same(control, Control.FromHandle(newHandle));
    }

    // Scenario: disposing a control that has a handle raises HandleDestroyed once, destroys the window
    // and does not allow a new handle to be created.
    [WinFormsFact]
    public void Control_Dispose_WithHandle_DestroysWindowAndBlocksRecreation()
    {
        using SubControl control = new();
        IntPtr handle = control.Handle;
        int destroyedCount = 0;
        control.HandleDestroyed += (sender, e) => destroyedCount++;

        control.Dispose();

        Assert.Equal(1, destroyedCount);
        Assert.True(control.IsDisposed);
        Assert.False(control.IsHandleCreated);
        Assert.False(PInvoke.IsWindow((HWND)handle));
        Assert.Null(Control.FromHandle(handle));
        Assert.Throws<ObjectDisposedException>(() => control.Handle);
        Assert.Throws<ObjectDisposedException>(control.CreateHandle);

        // A second Dispose is a no-op and does not raise the event again.
        control.Dispose();
        Assert.Equal(1, destroyedCount);
    }

    // Scenario: repeated create/destroy cycles keep a distinct, valid window each time, raise matching
    // HandleCreated/HandleDestroyed counts and preserve managed state, parent and children.
    [WinFormsFact]
    public void Control_CreateDestroyCycles_PreserveManagedState()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Name = "cycled",
            Text = "Cycle text",
            Tag = "tag",
            BackColor = Color.Red,
            Enabled = false,
            Location = new Point(3, 4),
            Size = new Size(50, 60),
            Parent = parent
        };
        using Control child = new() { Parent = control };

        int createdCount = 0;
        int destroyedCount = 0;
        control.HandleCreated += (sender, e) => createdCount++;
        control.HandleDestroyed += (sender, e) => destroyedCount++;

        for (int i = 0; i < 4; i++)
        {
            IntPtr handle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.True(PInvoke.IsWindow((HWND)handle));
            Assert.True(control.IsHandleCreated);

            control.DestroyHandle();
            Assert.False(PInvoke.IsWindow((HWND)handle));
            Assert.False(control.IsHandleCreated);

            Assert.Equal(i + 1, createdCount);
            Assert.Equal(i + 1, destroyedCount);
            Assert.Equal("cycled", control.Name);
            Assert.Equal("Cycle text", control.Text);
            Assert.Equal("tag", control.Tag);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.False(control.Enabled);
            Assert.Equal(new Rectangle(3, 4, 50, 60), control.Bounds);
            Assert.Same(parent, control.Parent);
            Assert.Same(control, child.Parent);
            Assert.Contains(child, control.Controls.Cast<Control>());
        }
    }

    // Scenario: destroying a parent's handle destroys the child windows too, keeps the child in the
    // Controls collection, and CreateControl on the visible parent recreates the child window.
    [WinFormsFact]
    public void Control_DestroyHandle_Parent_DestroysChildWindowsAndCreateControlRecreatesThem()
    {
        using SubControl parent = new();
        using Control child = new() { Parent = parent };
        IntPtr parentHandle = parent.Handle;
        IntPtr childHandle = child.Handle;
        int childDestroyedCount = 0;
        child.HandleDestroyed += (sender, e) => childDestroyedCount++;

        parent.DestroyHandle();

        Assert.Equal(1, childDestroyedCount);
        Assert.False(PInvoke.IsWindow((HWND)parentHandle));
        Assert.False(PInvoke.IsWindow((HWND)childHandle));
        Assert.False(child.IsHandleCreated);
        Assert.Same(parent, child.Parent);
        Assert.Contains(child, parent.Controls.Cast<Control>());

        parent.CreateControl();

        Assert.True(parent.Created);
        Assert.True(child.Created);
        Assert.True(child.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)child.Handle));
    }

    // Scenario: disposing a parent with created children destroys every child window.
    [WinFormsFact]
    public void Control_Dispose_Parent_DestroysChildWindows()
    {
        using Control parent = new();
        using Control child = new() { Parent = parent };
        IntPtr parentHandle = parent.Handle;
        IntPtr childHandle = child.Handle;
        Assert.True(parent.IsHandleCreated);
        Assert.True(child.IsHandleCreated);

        parent.Dispose();

        Assert.True(child.IsDisposed);
        Assert.False(parent.IsHandleCreated);
        Assert.False(child.IsHandleCreated);
        Assert.False(PInvoke.IsWindow((HWND)parentHandle));
        Assert.False(PInvoke.IsWindow((HWND)childHandle));
    }
}
