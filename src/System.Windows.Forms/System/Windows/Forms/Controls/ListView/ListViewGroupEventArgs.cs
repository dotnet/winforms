// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="ListView.OnGroupCollapsedStateChanged"/> and
///  <see cref="ListView.OnGroupTaskLinkClick"/> event.
/// </summary>
public class ListViewGroupEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instances of the <see cref="ListViewGroupEventArgs"/> class.
    /// </summary>
    /// <param name="groupIndex">The index of the <see cref="ListViewGroup"/> associated with the event.</param>
    public ListViewGroupEventArgs(int groupIndex)
    {
        GroupIndex = groupIndex;
    }

    /// <summary>
    ///  Gets the index of the <see cref="ListViewGroup"/> associated with the event.
    /// </summary>
    public int GroupIndex { get; }
}
