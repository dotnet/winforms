// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStrip_RestoreFocusMessageFilterTests
{
    public static TheoryData<int> MouseDownMessages => new()
    {
        (int)PInvokeCore.WM_LBUTTONDOWN,
        (int)PInvokeCore.WM_RBUTTONDOWN,
        (int)PInvokeCore.WM_MBUTTONDOWN,
        (int)PInvokeCore.WM_NCLBUTTONDOWN,
        (int)PInvokeCore.WM_NCRBUTTONDOWN,
        (int)PInvokeCore.WM_NCMBUTTONDOWN,
    };

    [WinFormsFact]
    public void Ctor_StoresOwnerToolStrip()
    {
        using ToolStrip toolStrip = new();
        ToolStrip.RestoreFocusMessageFilter filter = new(toolStrip);

        ToolStrip owner = filter.TestAccessor.Dynamic._ownerToolStrip;
        owner.Should().BeSameAs(toolStrip);
    }

    [WinFormsFact]
    public void RestoreFocusFilter_ReturnsSameInstance()
    {
        using ToolStrip toolStrip = new();

        ToolStrip.RestoreFocusMessageFilter first = toolStrip.RestoreFocusFilter;
        ToolStrip.RestoreFocusMessageFilter second = toolStrip.RestoreFocusFilter;

        first.Should().BeSameAs(second);
    }

    [WinFormsFact]
    public void PreFilterMessage_NonMouseMessage_ReturnsFalse()
    {
        using ToolStrip toolStrip = new();
        ToolStrip.RestoreFocusMessageFilter filter = new(toolStrip);
        // Use the public Create(IntPtr, int, IntPtr, IntPtr) overload to avoid MessageId accessibility
        // and uint/MessageId internal overload ambiguity.
        Message message = Message.Create(IntPtr.Zero, (int)PInvokeCore.WM_MOUSEMOVE, IntPtr.Zero, IntPtr.Zero);

        filter.PreFilterMessage(ref message).Should().BeFalse();
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseDownMessages))]
    public void PreFilterMessage_WhenOwnerDisposed_ReturnsFalse(int msg)
    {
        ToolStrip toolStrip = new();
        ToolStrip.RestoreFocusMessageFilter filter = new(toolStrip);
        toolStrip.Dispose();

        Message message = Message.Create(IntPtr.Zero, msg, IntPtr.Zero, IntPtr.Zero);

        filter.PreFilterMessage(ref message).Should().BeFalse();
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseDownMessages))]
    public void PreFilterMessage_WhenOwnerIsDropDown_ReturnsFalse(int msg)
    {
        using ToolStripDropDown dropDown = new();
        ToolStrip.RestoreFocusMessageFilter filter = new(dropDown);
        dropDown.IsDropDown.Should().BeTrue();

        Message message = Message.Create(IntPtr.Zero, msg, IntPtr.Zero, IntPtr.Zero);

        filter.PreFilterMessage(ref message).Should().BeFalse();
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseDownMessages))]
    public void PreFilterMessage_WhenToolStripDoesNotContainFocus_ReturnsFalse(int msg)
    {
        using Form form = new() { ShowInTaskbar = false };
        using ToolStrip toolStrip = new();
        using Button sibling = new() { Text = "Sibling" };
        form.Controls.Add(toolStrip);
        form.Controls.Add(sibling);
        form.Show();

        toolStrip.ContainsFocus.Should().BeFalse();

        ToolStrip.RestoreFocusMessageFilter filter = new(toolStrip);
        Message message = Message.Create(sibling.Handle, msg, IntPtr.Zero, IntPtr.Zero);

        filter.PreFilterMessage(ref message).Should().BeFalse();
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseDownMessages))]
    public void PreFilterMessage_WhenClickIsOnToolStripChild_DoesNotRestoreFocus(int msg)
    {
        using Form form = new() { ShowInTaskbar = false };
        using TrackingToolStrip toolStrip = new();
        using TextBox hostedTextBox = new() { Width = 80 };
        toolStrip.Items.Add(new ToolStripControlHost(hostedTextBox));
        form.Controls.Add(toolStrip);
        form.Show();

        hostedTextBox.Focus();
        toolStrip.ContainsFocus.Should().BeTrue();

        ToolStrip.RestoreFocusMessageFilter filter = new(toolStrip);
        Application.AddMessageFilter(filter);

        try
        {
            Message message = Message.Create(hostedTextBox.Handle, msg, IntPtr.Zero, IntPtr.Zero);

            filter.PreFilterMessage(ref message).Should().BeFalse();

            // Click is on a child of the toolstrip — restore must not be scheduled.
            Application.DoEvents();
            toolStrip.RestoreFocusCallCount.Should().Be(0);
        }
        finally
        {
            Application.RemoveMessageFilter(filter);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MouseDownMessages))]
    public void PreFilterMessage_WhenClickIsOutsideToolStripOnSameRoot_RestoresFocus(int msg)
    {
        using Form form = new() { ShowInTaskbar = false };
        using TrackingToolStrip toolStrip = new();
        using TextBox hostedTextBox = new() { Width = 80 };
        using Button sibling = new() { Text = "Sibling", Location = new Point(0, 40) };
        toolStrip.Items.Add(new ToolStripControlHost(hostedTextBox));
        form.Controls.Add(toolStrip);
        form.Controls.Add(sibling);
        form.Show();

        hostedTextBox.Focus();
        toolStrip.ContainsFocus.Should().BeTrue();
        toolStrip.TabStop.Should().BeFalse();

        ToolStrip.RestoreFocusMessageFilter filter = toolStrip.RestoreFocusFilter;
        Application.AddMessageFilter(filter);

        try
        {
            Message message = Message.Create(sibling.Handle, msg, IntPtr.Zero, IntPtr.Zero);

            // Filter never consumes the message.
            filter.PreFilterMessage(ref message).Should().BeFalse();

            // Restore is posted via BeginInvoke; pump so it runs.
            Application.DoEvents();
            toolStrip.RestoreFocusCallCount.Should().Be(1);
        }
        finally
        {
            Application.RemoveMessageFilter(filter);
            ToolStripManager.ModalMenuFilter.ExitMenuMode();
        }
    }

    private sealed class TrackingToolStrip : ToolStrip
    {
        public int RestoreFocusCallCount { get; private set; }

        protected override void RestoreFocus()
        {
            RestoreFocusCallCount++;
            base.RestoreFocus();
        }
    }
}
