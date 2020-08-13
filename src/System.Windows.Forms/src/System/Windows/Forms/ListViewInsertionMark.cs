// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates insertion-mark information
    /// </summary>
    public sealed class ListViewInsertionMark
    {
        private readonly ListView listView;

        private int index;
        private Color color = Color.Empty;
        private bool appearsAfterItem;

        internal ListViewInsertionMark(ListView listView)
        {
            this.listView = listView;
        }

        /// <summary>
        ///  Specifies whether the insertion mark appears
        ///  after the item - otherwise it appears
        ///  before the item (the default).
        /// </summary>
        public bool AppearsAfterItem
        {
            get
            {
                return appearsAfterItem;
            }
            set
            {
                if (appearsAfterItem != value)
                {
                    appearsAfterItem = value;

                    if (listView.IsHandleCreated)
                    {
                        UpdateListView();
                    }
                }
            }
        }

        /// <summary>
        ///  Returns bounds of the insertion-mark.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                var rect = new RECT();
                User32.SendMessageW(listView, (User32.WM)LVM.GETINSERTMARKRECT, IntPtr.Zero, ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }

        /// <summary>
        ///  The color of the insertion-mark.
        /// </summary>
        public Color Color
        {
            get
            {
                if (color.IsEmpty)
                {
                    color = new COLORREF((uint)User32.SendMessageW(listView, (User32.WM)LVM.GETINSERTMARKCOLOR));
                }
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    if (listView.IsHandleCreated)
                    {
                        User32.SendMessageW(listView, (User32.WM)LVM.SETINSERTMARKCOLOR, IntPtr.Zero, PARAM.FromColor(color));
                    }
                }
            }
        }

        /// <summary>
        ///  Item next to which the insertion-mark appears.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                if (index != value)
                {
                    index = value;
                    if (listView.IsHandleCreated)
                    {
                        UpdateListView();
                    }
                }
            }
        }

        /// <summary>
        ///  Performs a hit-test at the specified insertion point
        ///  and returns the closest item.
        /// </summary>
        public unsafe int NearestIndex(Point pt)
        {
            var lvInsertMark = new LVINSERTMARK
            {
                cbSize = (uint)sizeof(LVINSERTMARK)
            };
            User32.SendMessageW(listView, (User32.WM)LVM.INSERTMARKHITTEST, (IntPtr)(&pt), ref lvInsertMark);

            return lvInsertMark.iItem;
        }

        internal unsafe void UpdateListView()
        {
            Debug.Assert(listView.IsHandleCreated, "ApplySavedState Precondition: List-view handle must be created");
            var lvInsertMark = new LVINSERTMARK
            {
                cbSize = (uint)sizeof(LVINSERTMARK),
                dwFlags = appearsAfterItem ? LVIM.AFTER : LVIM.BEFORE,
                iItem = index
            };
            User32.SendMessageW(listView, (User32.WM)LVM.SETINSERTMARK, IntPtr.Zero, ref lvInsertMark);

            if (!color.IsEmpty)
            {
                User32.SendMessageW(listView, (User32.WM)LVM.SETINSERTMARKCOLOR, IntPtr.Zero, PARAM.FromColor(color));
            }
        }
    }
}
