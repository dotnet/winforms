// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

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
            RECT bounds = default;
            PInvokeCore.SendMessage(_listView, PInvoke.LVM_GETINSERTMARKRECT, (WPARAM)0, ref bounds);
            return bounds;
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
                _color = new COLORREF((uint)PInvokeCore.SendMessage(_listView, PInvoke.LVM_GETINSERTMARKCOLOR));
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
                    PInvokeCore.SendMessage(_listView, PInvoke.LVM_SETINSERTMARKCOLOR, 0, _color.ToWin32());
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
        LVINSERTMARK lvInsertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK)
        };

        PInvokeCore.SendMessage(_listView, PInvoke.LVM_INSERTMARKHITTEST, (WPARAM)(&pt), ref lvInsertMark);

        return lvInsertMark.iItem;
    }

    internal unsafe void UpdateListView()
    {
        Debug.Assert(_listView.IsHandleCreated, "ApplySavedState Precondition: List-view handle must be created");
        LVINSERTMARK lvInsertMark = new()
        {
            cbSize = (uint)sizeof(LVINSERTMARK),
            dwFlags = _appearsAfterItem ? PInvoke.LVIM_AFTER : PInvoke.LVIM_BEFORE,
            iItem = _index
        };

        PInvokeCore.SendMessage(_listView, PInvoke.LVM_SETINSERTMARK, 0, ref lvInsertMark);

        if (!_color.IsEmpty)
        {
            PInvokeCore.SendMessage(_listView, PInvoke.LVM_SETINSERTMARKCOLOR, 0, _color.ToWin32());
        }
    }
}
