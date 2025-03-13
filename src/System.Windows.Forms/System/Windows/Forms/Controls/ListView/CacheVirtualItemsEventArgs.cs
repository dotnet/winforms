// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class CacheVirtualItemsEventArgs : EventArgs
{
    public CacheVirtualItemsEventArgs(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    public int StartIndex { get; }

    public int EndIndex { get; }
}
