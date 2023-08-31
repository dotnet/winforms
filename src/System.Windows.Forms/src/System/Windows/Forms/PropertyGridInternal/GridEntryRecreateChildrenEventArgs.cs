// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal class GridEntryRecreateChildrenEventArgs : EventArgs
{
    public int NewChildCount { get; }
    public int OldChildCount { get; }

    public GridEntryRecreateChildrenEventArgs(int oldCount, int newCount)
    {
        OldChildCount = oldCount;
        NewChildCount = newCount;
    }
}
