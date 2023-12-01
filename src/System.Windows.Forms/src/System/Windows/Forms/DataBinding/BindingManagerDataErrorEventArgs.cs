// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class BindingManagerDataErrorEventArgs : EventArgs
{
    public BindingManagerDataErrorEventArgs(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
