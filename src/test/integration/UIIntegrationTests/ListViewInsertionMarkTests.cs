// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500.
public class ListViewInsertionMarkTests : ControlTestBase
{
    public ListViewInsertionMarkTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMark_Success()
    {
        using ListView control = new();

        // Set same.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.AppearsAfterItem = false;
        LVINSERTMARK insertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set true.
        control.InsertionMark.AppearsAfterItem = true;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000001, insertMark.dwFlags);
        Assert.Equal(0, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set false.
        control.InsertionMark.AppearsAfterItem = false;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(0, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));
    }

    [WinFormsFact]
    public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMarkWithColor_Success()
    {
        using ListView control = new();
        control.InsertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);

        // Set same.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.AppearsAfterItem = false;
        LVINSERTMARK insertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set true.
        control.InsertionMark.AppearsAfterItem = true;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000001, insertMark.dwFlags);
        Assert.Equal(0, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set false.
        control.InsertionMark.AppearsAfterItem = false;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(0, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));
    }

    [WinFormsFact]
    public unsafe void ListViewInsertionMark_Color_GetInsertMarkColor_Success()
    {
        using ListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        // Set same.
        control.InsertionMark.Color = Color.Empty;
        LVINSERTMARK insertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set different.
        control.InsertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    public unsafe void ListViewInsertionMark_Index_GetInsertMark_Success(int index)
    {
        using ListView control = new();

        // Set same.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.Index = 0;
        LVINSERTMARK insertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set negative one.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.Index = -1;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set different.
        control.InsertionMark.Index = index;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(index, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(1)]
    public unsafe void ListViewInsertionMark_Index_GetInsertMarkWithColor_Success(int index)
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        insertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);

        // Set same.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.Index = 0;
        LVINSERTMARK insertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set negative one.
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        control.InsertionMark.Index = -1;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(0, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(-1, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));

        // Set different.
        control.InsertionMark.Index = index;
        insertMark = new LVINSERTMARK
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARK, 0, ref insertMark));
        Assert.Equal(0x80000000, insertMark.dwFlags);
        Assert.Equal(index, insertMark.iItem);
        Assert.Equal(0u, insertMark.dwReserved);
        Assert.Equal(0x785634, (int)PInvokeCore.SendMessage(control, PInvoke.LVM_GETINSERTMARKCOLOR));
    }
}
