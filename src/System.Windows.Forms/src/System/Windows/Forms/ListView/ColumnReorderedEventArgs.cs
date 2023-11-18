// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public class ColumnReorderedEventArgs : CancelEventArgs
{
    public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader? header) : base()
    {
        OldDisplayIndex = oldDisplayIndex;
        NewDisplayIndex = newDisplayIndex;
        Header = header;
    }

    public int OldDisplayIndex { get; }

    public int NewDisplayIndex { get; }

    public ColumnHeader? Header { get; }
}
