// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Microsoft.DotNet.RemoteExecutor;
using Point = System.Drawing.Point;

namespace System.Windows.Forms.Tests;

public class ListViewInsertionMarkTests
{
    [WinFormsFact]
    public void ListViewInsertionMark_AppearsAfterItem_Get_ReturnsExpected()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.False(insertionMark.AppearsAfterItem);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListViewInsertionMark_AppearsAfterItem_Set_GetReturnsExpected(bool value)
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;

        insertionMark.AppearsAfterItem = value;
        Assert.Equal(value, insertionMark.AppearsAfterItem);
        Assert.False(control.IsHandleCreated);

        // Set same.
        insertionMark.AppearsAfterItem = value;
        Assert.Equal(value, insertionMark.AppearsAfterItem);
        Assert.False(control.IsHandleCreated);

        // Set different.
        insertionMark.AppearsAfterItem = !value;
        Assert.Equal(!value, insertionMark.AppearsAfterItem);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListViewInsertionMark_AppearsAfterItem_SetWithHandle_GetReturnsExpected(bool value)
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        insertionMark.AppearsAfterItem = value;
        Assert.Equal(value, insertionMark.AppearsAfterItem);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        insertionMark.AppearsAfterItem = value;
        Assert.Equal(value, insertionMark.AppearsAfterItem);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        insertionMark.AppearsAfterItem = !value;
        Assert.Equal(!value, insertionMark.AppearsAfterItem);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMark_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();

            using ListView control = new();
            ListViewInsertionMark insertionMark = control.InsertionMark;

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
        }).Dispose();
    }

    [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMarkWithColor_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();

            using ListView control = new();
            ListViewInsertionMark insertionMark = control.InsertionMark;
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
        }).Dispose();
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Bounds_GetWithoutHandle_ReturnsEqual()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.Equal(Rectangle.Empty, insertionMark.Bounds);
        Assert.Equal(insertionMark.Bounds, insertionMark.Bounds);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Bounds_GetWithHandle_ReturnsEqual()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Equal(Rectangle.Empty, insertionMark.Bounds);
        Assert.Equal(insertionMark.Bounds, insertionMark.Bounds);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Bounds_GetCustomInsertMarkRect_TestData()
    {
        yield return new object[] { default(RECT), Rectangle.Empty };
        yield return new object[] { new RECT(1, 2, 3, 4), new Rectangle(1, 2, 2, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Bounds_GetCustomInsertMarkRect_TestData))]
    public void ListViewInsertionMark_Bounds_GetCustomGetInsertMarkRect_ReturnsExpected(object getInsertMarkRectResult, Rectangle expected)
    {
        using CustomGetInsertMarkRectListView control = new()
        {
            GetInsertMarkRectResult = (RECT)getInsertMarkRectResult
        };
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, insertionMark.Bounds);
    }

    private class CustomGetInsertMarkRectListView : ListView
    {
        public RECT GetInsertMarkRectResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.LVM_GETINSERTMARKRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = GetInsertMarkRectResult;
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Bounds_GetInvalidGetInsertMarkRect_ReturnsExpected()
    {
        using InvalidGetInsertMarkRectListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.MakeInvalid = true;
        Assert.Equal(new Rectangle(1, 2, 2, 2), insertionMark.Bounds);
    }

    private class InvalidGetInsertMarkRectListView : ListView
    {
        public bool MakeInvalid { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (MakeInvalid && m.Msg == (int)PInvoke.LVM_GETINSERTMARKRECT)
            {
                RECT* pRect = (RECT*)m.LParam;
                *pRect = new RECT(1, 2, 3, 4);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Color_GetWithoutHandle_ReturnsExpected()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(Color.Empty, insertionMark.Color);
        Assert.Equal(insertionMark.Color, insertionMark.Color);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Color_GetWithHandle_ReturnsExpected()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.NotEqual(Color.Empty, insertionMark.Color);
        Assert.Equal(insertionMark.Color, insertionMark.Color);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
    public void ListViewInsertionMark_Color_SetWithoutHandle_ReturnsExpected(Color value)
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;

        insertionMark.Color = value;
        Assert.Equal(value, insertionMark.Color);
        Assert.False(control.IsHandleCreated);

        // Set again.
        insertionMark.Color = value;
        Assert.Equal(value, insertionMark.Color);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
    public void ListViewInsertionMark_Color_SetWithHandle_ReturnsExpected(Color value)
    {
        using ListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        ListViewInsertionMark insertionMark = control.InsertionMark;
        insertionMark.Color = value;
        Assert.Equal(value, insertionMark.Color);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set again.
        insertionMark.Color = value;
        Assert.Equal(value, insertionMark.Color);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    public unsafe void ListViewInsertionMark_Color_GetInsertMarkColor_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();

            using ListView control = new();
            ListViewInsertionMark insertionMark = control.InsertionMark;
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
        }).Dispose();
    }

    [WinFormsFact]
    public void ListViewInsertionMark_Index_Get_ReturnsExpected()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.Equal(0, insertionMark.Index);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ListViewInsertionMark_Index_SetWithoutHandle_GetReturnsExpected(int value)
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;

        insertionMark.Index = value;
        Assert.Equal(value, insertionMark.Index);
        Assert.False(control.IsHandleCreated);

        // Set again.
        insertionMark.Index = value;
        Assert.Equal(value, insertionMark.Index);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ListViewInsertionMark_Index_SetWithHandle_GetReturnsExpected(int value)
    {
        using ListView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        ListViewInsertionMark insertionMark = control.InsertionMark;
        insertionMark.Index = value;
        Assert.Equal(value, insertionMark.Index);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set again.
        insertionMark.Index = value;
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(value, insertionMark.Index);
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [InlineData(-2)]
    [InlineData(1)]
    public unsafe void ListViewInsertionMark_Index_GetInsertMark_Success(int indexParam)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke((indexString) =>
        {
            int index = int.Parse(indexString);
            Application.EnableVisualStyles();

            using ListView control = new();
            ListViewInsertionMark insertionMark = control.InsertionMark;

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
        }, indexParam.ToString()).Dispose();
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [InlineData(-2)]
    [InlineData(1)]
    public unsafe void ListViewInsertionMark_Index_GetInsertMarkWithColor_Success(int indexParam)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke((indexString) =>
        {
            int index = int.Parse(indexString);
            Application.EnableVisualStyles();

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
        }, indexParam.ToString()).Dispose();
    }

    [WinFormsFact]
    public void ListViewInsertionMark_NearestIndex_NoSuchPointWithoutHandle_ReturnsInvalid()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.True(insertionMark.NearestIndex(new Point(-10, -11)) >= -1);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewInsertionMark_NearestIndex_NoSuchPointWithHandle_ReturnsInvalid()
    {
        using ListView control = new();
        ListViewInsertionMark insertionMark = control.InsertionMark;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.True(insertionMark.NearestIndex(new Point(-10, -11)) >= -1);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListViewInsertionMark_NearestIndex_InvokeCustomInsertMarkHitTest_ReturnsExpected(int result)
    {
        using CustomInsertMarkHitTestListView control = new()
        {
            InsertMarkHitTestResult = result
        };
        ListViewInsertionMark insertionMark = control.InsertionMark;

        Assert.Equal(result, insertionMark.NearestIndex(new Point(1, 2)));
    }

    private class CustomInsertMarkHitTestListView : ListView
    {
        public int InsertMarkHitTestResult { get; set; }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.LVM_INSERTMARKHITTEST)
            {
                Point* pPt = (Point*)m.WParam;
                Assert.Equal(1, pPt->X);
                Assert.Equal(2, pPt->Y);
                LVINSERTMARK* pInsertMark = (LVINSERTMARK*)m.LParam;
                pInsertMark->iItem = InsertMarkHitTestResult;
                m.Result = 1;
                return;
            }

            base.WndProc(ref m);
        }
    }
}
