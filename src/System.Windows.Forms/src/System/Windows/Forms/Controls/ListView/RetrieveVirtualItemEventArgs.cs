// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class RetrieveVirtualItemEventArgs : EventArgs
{
    public RetrieveVirtualItemEventArgs(int itemIndex)
    {
        ItemIndex = itemIndex;
    }

    public int ItemIndex { get; }

    public ListViewItem? Item { get; set; }
}
