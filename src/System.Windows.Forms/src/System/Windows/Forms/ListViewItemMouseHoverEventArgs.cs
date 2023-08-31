// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="ListView.OnItemMouseHover"/> event.
/// </summary>
public class ListViewItemMouseHoverEventArgs : EventArgs
{
    public ListViewItemMouseHoverEventArgs(ListViewItem? item)
    {
        Item = item;
    }

    public ListViewItem? Item { get; }
}
