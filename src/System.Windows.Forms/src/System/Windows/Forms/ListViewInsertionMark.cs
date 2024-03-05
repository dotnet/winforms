﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Encapsulates insertion-mark information
    /// </summary>
    public sealed class ListViewInsertionMark
    {
        private readonly ListView _listView;

        private int _index;
        private Color _color = Color.Empty;
        private bool _appearsAfterItem;

        internal ListViewInsertionMark(ListView listView)
        {
            _listView = listView;
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
                return _appearsAfterItem;
            }
            set
            {
                if (_appearsAfterItem != value)
                {
                    _appearsAfterItem = value;

                    if (_listView.IsHandleCreated)
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
                User32.SendMessageW(_listView, (User32.WM)LVM.GETINSERTMARKRECT, 0, ref rect);
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
                if (_color.IsEmpty)
                {
                    _color = new COLORREF((uint)User32.SendMessageW(_listView, (User32.WM)LVM.GETINSERTMARKCOLOR));
                }

                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    if (_listView.IsHandleCreated)
                    {
                        User32.SendMessageW(_listView, (User32.WM)LVM.SETINSERTMARKCOLOR, 0, _color.ToWin32());
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
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    if (_listView.IsHandleCreated)
                    {
                        UpdateListView();
                    }
                }
            }
        }

        /// <summary>
        ///  Performs a hit-test at the specified insertion point and returns the closest item.
        /// </summary>
        public unsafe int NearestIndex(Point pt)
        {
            var lvInsertMark = new LVINSERTMARK
            {
                cbSize = (uint)sizeof(LVINSERTMARK)
            };

            User32.SendMessageW(_listView, (User32.WM)LVM.INSERTMARKHITTEST, (nint)(&pt), ref lvInsertMark);

            return lvInsertMark.iItem;
        }

        internal unsafe void UpdateListView()
        {
            Debug.Assert(_listView.IsHandleCreated, "ApplySavedState Precondition: List-view handle must be created");
            var lvInsertMark = new LVINSERTMARK
            {
                cbSize = (uint)sizeof(LVINSERTMARK),
                dwFlags = _appearsAfterItem ? LVIM.AFTER : LVIM.BEFORE,
                iItem = _index
            };

            User32.SendMessageW(_listView, (User32.WM)LVM.SETINSERTMARK, 0, ref lvInsertMark);

            if (!_color.IsEmpty)
            {
                User32.SendMessageW(_listView, (User32.WM)LVM.SETINSERTMARKCOLOR, 0, _color.ToWin32());
            }
        }
    }
}