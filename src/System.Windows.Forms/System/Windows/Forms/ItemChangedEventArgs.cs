// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class ItemChangedEventArgs : EventArgs
{
    internal ItemChangedEventArgs(int index)
    {
        Index = index;
    }

    public int Index { get; }
}
