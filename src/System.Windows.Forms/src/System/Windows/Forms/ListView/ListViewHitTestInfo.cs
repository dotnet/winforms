// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the return value for HITTEST on ListView.
/// </summary>
public class ListViewHitTestInfo
{
    /// <summary>
    ///  Creates a ListViewHitTestInfo instance.
    /// </summary>
    public ListViewHitTestInfo(ListViewItem? hitItem, ListViewItem.ListViewSubItem? hitSubItem, ListViewHitTestLocations hitLocation)
    {
        Item = hitItem;
        SubItem = hitSubItem;
        Location = hitLocation;
    }

    /// <summary>
    ///  This gives the exact location returned by hit test on listview.
    /// </summary>
    public ListViewHitTestLocations Location { get; }

    /// <summary>
    ///  This gives the ListViewItem returned by hit test on listview.
    /// </summary>
    public ListViewItem? Item { get; }

    /// <summary>
    ///  This gives the ListViewSubItem returned by hit test on listview.
    /// </summary>
    public ListViewItem.ListViewSubItem? SubItem { get; }
}
