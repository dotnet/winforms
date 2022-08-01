// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests
{
    public class ListViewInsertionMarkTests : ControlTestBase
    {
        public ListViewInsertionMarkTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMark_Success()
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;

            // Set same.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.AppearsAfterItem = false;
            var insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set true.
            control.InsertionMark.AppearsAfterItem = true;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000001, (uint)insertMark.dwFlags);
            Assert.Equal(0, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set false.
            control.InsertionMark.AppearsAfterItem = false;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(0, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));
        }

        [WinFormsFact]
        public unsafe void ListViewInsertionMark_AppearsAfterItem_GetInsertMarkWithColor_Success()
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            control.InsertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);

            // Set same.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.AppearsAfterItem = false;
            var insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set true.
            control.InsertionMark.AppearsAfterItem = true;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000001, (uint)insertMark.dwFlags);
            Assert.Equal(0, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0x785634, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set false.
            control.InsertionMark.AppearsAfterItem = false;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(0, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0x785634, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));
        }

        [WinFormsFact]
        public unsafe void ListViewInsertionMark_Color_GetInsertMarkColor_Success()
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            // Set same.
            control.InsertionMark.Color = Color.Empty;
            var insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(00, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set different.
            control.InsertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(00, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0x785634, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public unsafe void ListViewInsertionMark_Index_GetInsertMark_Success(int index)
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;

            // Set same.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.Index = 0;
            var insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set negative one.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.Index = -1;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set different.
            control.InsertionMark.Index = index;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(index, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public unsafe void ListViewInsertionMark_Index_GetInsertMarkWithColor_Success(int index)
        {
            Application.EnableVisualStyles();

            using var control = new ListView();
            ListViewInsertionMark insertionMark = control.InsertionMark;
            insertionMark.Color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);

            // Set same.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.Index = 0;
            var insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set negative one.
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.InsertionMark.Index = -1;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(0, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(-1, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0x785634, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));

            // Set different.
            control.InsertionMark.Index = index;
            insertMark = new ComCtl32.LVINSERTMARK
            {
                cbSize = (uint)sizeof(ComCtl32.LVINSERTMARK)
            };
            Assert.Equal(1, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARK, 0, ref insertMark));
            Assert.Equal(0x80000000, (uint)insertMark.dwFlags);
            Assert.Equal(index, insertMark.iItem);
            Assert.Equal(0u, insertMark.dwReserved);
            Assert.Equal(0x785634, User32.SendMessageW(control.Handle, (User32.WM)ComCtl32.LVM.GETINSERTMARKCOLOR));
        }
    }
}
