// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates insertion-mark information
    /// </summary>
    public sealed class ListViewInsertionMark
    {
        private readonly ListView listView;

        private int index = 0;
        private Color color = Color.Empty;
        private bool appearsAfterItem = false;

        internal ListViewInsertionMark(ListView listView)
        {
            this.listView = listView;
        }

        /// <summary>
        ///  Specifies whether the insertion mark appears
    	///  after the item - otherwise it appears
    	///  before the item (the default).
        /// </summary>
        ///
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
        ///
        public Rectangle Bounds
        {
            get
            {
                var rect = new RECT();
                User32.SendMessageW(listView, (User32.WindowMessage)LVM.GETINSERTMARKRECT, IntPtr.Zero, ref rect);
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }

        /// <summary>
        ///  The color of the insertion-mark.
        /// </summary>
        ///
        public Color Color
        {
            get
            {
                if (color.IsEmpty)
                {
                    color = COLORREF.COLORREFToColor((int)listView.SendMessage((int)LVM.GETINSERTMARKCOLOR, 0, 0));
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
                        listView.SendMessage((int)LVM.SETINSERTMARKCOLOR, 0, COLORREF.ColorToCOLORREF(color));
                    }
                }
            }
        }

        /// <summary>
        ///  Item next to which the insertion-mark appears.
        /// </summary>
        ///
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
        ///
        public int NearestIndex(Point pt)
        {
            NativeMethods.LVINSERTMARK lvInsertMark = new NativeMethods.LVINSERTMARK();
            UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), (int)LVM.INSERTMARKHITTEST, ref pt, lvInsertMark);
            return lvInsertMark.iItem;
        }

        internal void UpdateListView()
        {
            Debug.Assert(listView.IsHandleCreated, "ApplySavedState Precondition: List-view handle must be created");
            NativeMethods.LVINSERTMARK lvInsertMark = new NativeMethods.LVINSERTMARK
            {
                dwFlags = appearsAfterItem ? NativeMethods.LVIM_AFTER : 0,
                iItem = index
            };
            UnsafeNativeMethods.SendMessage(new HandleRef(listView, listView.Handle), (int)LVM.SETINSERTMARK, 0, lvInsertMark);

            if (!color.IsEmpty)
            {
                listView.SendMessage((int)LVM.SETINSERTMARKCOLOR, 0, COLORREF.ColorToCOLORREF(color));
            }
        }
    }
}
