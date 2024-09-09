// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for <see cref="Form.OnFormClosing"/> and <see cref="Form.OnClosing"/> events.
/// </summary>
public class FormClosedEventArgs : EventArgs
{
    public FormClosedEventArgs(CloseReason closeReason)
    {
        CloseReason = closeReason;
    }

    /// <summary>
    ///  Provides the reason for the Form Close.
    /// </summary>
    public CloseReason CloseReason { get; }
}
