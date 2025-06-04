// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class NavigateEventArgs : EventArgs
{
    public NavigateEventArgs(bool isForward)
    {
        Forward = isForward;
    }

    public bool Forward { get; }
}
