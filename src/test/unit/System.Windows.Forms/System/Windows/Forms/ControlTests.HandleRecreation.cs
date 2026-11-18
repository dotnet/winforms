// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    // Scenario: RecreateHandle destroys the old window and creates a new one; RecreatingHandle is true
    // while both HandleDestroyed and HandleCreated run (old window still valid, new window valid) and
    // false before and after; each event is raised once and managed state is preserved.
    [WinFormsFact]
    public void Control_RecreateHandle_RaisesEventsWhileRecreatingAndPreservesState()
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Name = "recreated",
            Text = "Recreate text",
            Location = new Point(5, 6),
            Size = new Size(70, 80),
            Parent = parent
        };

        IntPtr oldHandle = parent.Handle;
        control.CreateControl();
        Assert.True(control.IsHandleCreated);
        IntPtr controlOldHandle = control.Handle;

        int destroyedCount = 0;
        int createdCount = 0;
        bool recreatingInDestroyed = false;
        bool oldWindowValidInDestroyed = false;
        bool recreatingInCreated = false;
        bool newWindowValidInCreated = false;
        control.HandleDestroyed += (sender, e) =>
        {
            destroyedCount++;
            recreatingInDestroyed = control.RecreatingHandle;
            oldWindowValidInDestroyed = PInvoke.IsWindow((HWND)controlOldHandle);
        };
        control.HandleCreated += (sender, e) =>
        {
            createdCount++;
            recreatingInCreated = control.RecreatingHandle;
            newWindowValidInCreated = control.IsHandleCreated && PInvoke.IsWindow((HWND)control.Handle);
        };

        Assert.False(control.RecreatingHandle);
        control.RecreateHandle();

        Assert.False(control.RecreatingHandle);
        Assert.Equal(1, destroyedCount);
        Assert.Equal(1, createdCount);
        Assert.True(recreatingInDestroyed);
        Assert.True(oldWindowValidInDestroyed);
        Assert.True(recreatingInCreated);
        Assert.True(newWindowValidInCreated);

        Assert.True(control.IsHandleCreated);
        IntPtr newHandle = control.Handle;
        Assert.True(PInvoke.IsWindow((HWND)newHandle));
        Assert.Same(control, Control.FromHandle(newHandle));

        Assert.Equal("recreated", control.Name);
        Assert.Equal("Recreate text", control.Text);
        Assert.Equal(new Rectangle(5, 6, 70, 80), control.Bounds);
        Assert.Same(parent, control.Parent);
        Assert.Equal(oldHandle, parent.Handle);
        Assert.Equal(parent.Handle, (IntPtr)PInvoke.GetParent(control));
    }

    // Scenario: recreating a parent's handle keeps child controls attached with valid windows and
    // re-parents the child windows (and grandchildren) to the new parent window.
    [WinFormsFact]
    public void Control_RecreateHandle_Parent_KeepsChildWindowsAndReparentsThem()
    {
        using SubControl parent = new();
        using Control child = new() { Parent = parent };
        using Control grandChild = new() { Parent = child };

        _ = parent.Handle;
        child.CreateControl();
        grandChild.CreateControl();
        Assert.True(child.IsHandleCreated);
        Assert.True(grandChild.IsHandleCreated);

        parent.RecreateHandle();

        Assert.True(parent.IsHandleCreated);
        IntPtr newParentHandle = parent.Handle;
        Assert.True(PInvoke.IsWindow((HWND)newParentHandle));

        Assert.True(child.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)child.Handle));
        Assert.Equal(newParentHandle, (IntPtr)PInvoke.GetParent(child));
        Assert.Same(parent, child.Parent);
        Assert.Contains(child, parent.Controls.Cast<Control>());

        Assert.True(grandChild.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)grandChild.Handle));
        Assert.Equal(child.Handle, (IntPtr)PInvoke.GetParent(grandChild));
        Assert.Same(child, grandChild.Parent);
    }

    // Scenario: RecreateHandle on a control without a handle, and on a disposed control, does nothing
    // and raises no handle events.
    [WinFormsFact]
    public void Control_RecreateHandle_NoHandleOrDisposed_RaisesNoEvents()
    {
        using SubControl control = new();
        int eventCount = 0;
        control.HandleCreated += (sender, e) => eventCount++;
        control.HandleDestroyed += (sender, e) => eventCount++;

        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);
        Assert.False(control.RecreatingHandle);

        _ = control.Handle;
        Assert.Equal(1, eventCount);
        control.Dispose();
        Assert.Equal(2, eventCount);

        control.RecreateHandle();
        Assert.False(control.IsHandleCreated);
        Assert.False(control.RecreatingHandle);
        Assert.Equal(2, eventCount);
    }

    // Scenario: when handle creation fails during recreation, the exception propagates, the control
    // is left without a handle and RecreatingHandle is cleared.
    [WinFormsFact]
    public void Control_RecreateHandle_CreateHandleThrows_ClearsRecreatingState()
    {
        using ThrowingOnCreateControl control = new();
        _ = control.Handle;
        control.ThrowOnCreate = true;

        Assert.Throws<InvalidOperationException>(control.RecreateHandle);

        Assert.False(control.RecreatingHandle);
        Assert.False(control.IsHandleCreated);
    }

    private class ThrowingOnCreateControl : Control
    {
        public bool ThrowOnCreate { get; set; }

        public new void RecreateHandle() => base.RecreateHandle();

        protected override CreateParams CreateParams
        {
            get
            {
                if (ThrowOnCreate)
                {
                    throw new InvalidOperationException();
                }

                return base.CreateParams;
            }
        }
    }

    // Scenario: a style-dependent TextBox setter (Multiline) recreates the window while preserving text,
    // selection, bounds and parent, and RecreatingHandle is true during the callbacks.
    [WinFormsFact]
    public void TextBox_Multiline_Change_RecreatesHandlePreservingTextAndSelection()
    {
        using Control parent = new();
        using TextBox textBox = new()
        {
            Text = "Hello world",
            Location = new Point(2, 3),
            Size = new Size(120, 20),
            Parent = parent
        };
        _ = textBox.Handle;
        textBox.SelectionStart = 2;
        textBox.SelectionLength = 4;

        int createdCount = 0;
        int destroyedCount = 0;
        bool recreatingInCreated = false;
        textBox.HandleDestroyed += (sender, e) => destroyedCount++;
        textBox.HandleCreated += (sender, e) =>
        {
            createdCount++;
            recreatingInCreated = textBox.RecreatingHandle;
        };

        textBox.Multiline = true;

        Assert.Equal(1, destroyedCount);
        Assert.Equal(1, createdCount);
        Assert.True(recreatingInCreated);
        Assert.False(textBox.RecreatingHandle);
        Assert.True(textBox.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)textBox.Handle));
        Assert.Equal("Hello world", textBox.Text);
        Assert.Equal(2, textBox.SelectionStart);
        Assert.Equal(4, textBox.SelectionLength);
        Assert.Equal(2, textBox.Location.X);
        Assert.Equal(3, textBox.Location.Y);
        Assert.Same(parent, textBox.Parent);

        // Setting the same value does not recreate the window again.
        IntPtr handle = textBox.Handle;
        textBox.Multiline = true;
        Assert.Equal(handle, textBox.Handle);
        Assert.Equal(1, createdCount);
    }

    // Scenario: a style-dependent ListBox setter (MultiColumn) recreates the window while preserving
    // items and the selected items.
    [WinFormsFact]
    public void ListBox_MultiColumn_Change_RecreatesHandlePreservingItemsAndSelection()
    {
        using ListBox listBox = new() { SelectionMode = SelectionMode.MultiExtended };
        listBox.Items.AddRange(["a", "b", "c", "d"]);
        _ = listBox.Handle;
        listBox.SetSelected(1, true);
        listBox.SetSelected(3, true);
        int destroyedCount = 0;
        listBox.HandleDestroyed += (sender, e) => destroyedCount++;

        listBox.MultiColumn = true;

        Assert.Equal(1, destroyedCount);
        Assert.True(listBox.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)listBox.Handle));
        Assert.Equal(["a", "b", "c", "d"], listBox.Items.Cast<string>());
        Assert.Equal([1, 3], listBox.SelectedIndices.Cast<int>());
    }

    // Scenario: a handle-recreating ListView setter (Scrollable) recreates the window each time while
    // preserving items, checked state and selection.
    [WinFormsFact]
    public void ListView_Scrollable_Change_RecreatesHandlePreservingItems()
    {
        using ListView listView = new() { View = View.List, CheckBoxes = true };
        listView.Items.Add(new ListViewItem("one"));
        listView.Items.Add(new ListViewItem("two"));
        listView.Items.Add(new ListViewItem("three"));
        _ = listView.Handle;
        listView.Items[1].Selected = true;
        listView.Items[2].Checked = true;
        int destroyedCount = 0;
        listView.HandleDestroyed += (sender, e) => destroyedCount++;

        listView.Scrollable = false;
        Assert.Equal(1, destroyedCount);
        Assert.True(listView.IsHandleCreated);

        listView.Scrollable = true;

        Assert.Equal(2, destroyedCount);
        Assert.True(listView.IsHandleCreated);
        Assert.True(PInvoke.IsWindow((HWND)listView.Handle));
        Assert.Equal(["one", "two", "three"], listView.Items.Cast<ListViewItem>().Select(i => i.Text));
        Assert.Equal([false, false, true], listView.Items.Cast<ListViewItem>().Select(i => i.Checked));
        Assert.Equal(1, listView.SelectedIndices.Cast<int>().Single());
    }

    // Scenario: recreating the handle of a shown main form (ShowInTaskbar change) does not make the
    // application context request thread exit and the form stays visible, while really closing it does.
    [WinFormsFact]
    public void Form_ShowInTaskbar_Change_RecreatesHandleWithoutExitingApplicationContext()
    {
        using Form form = new();
        using ApplicationContext context = new(form);
        int exitCount = 0;
        context.ThreadExit += (sender, e) => exitCount++;
        int destroyedCount = 0;
        bool recreatingInDestroyed = false;
        form.HandleDestroyed += (sender, e) =>
        {
            destroyedCount++;
            recreatingInDestroyed = form.RecreatingHandle;
        };

        form.Show();

        form.ShowInTaskbar = false;

        Assert.Equal(1, destroyedCount);
        Assert.True(recreatingInDestroyed);
        Assert.False(form.RecreatingHandle);
        Assert.Equal(0, exitCount);
        Assert.True(form.IsHandleCreated);
        Assert.True(form.Visible);
        Assert.True(PInvoke.IsWindow((HWND)form.Handle));

        // A second recreation is still not treated as the form closing.
        form.ShowInTaskbar = true;
        Assert.Equal(0, exitCount);

        // Genuinely destroying the handle does request thread exit.
        form.Dispose();
        Assert.Equal(1, exitCount);
    }
}
