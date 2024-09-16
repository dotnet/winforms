// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides data for <see cref="Form.OnFormClosing"/> and <see cref="Form.OnClosing"/> events.
/// </summary>
public class FormClosingEventArgs : CancelEventArgs
{
    public FormClosingEventArgs(CloseReason closeReason, bool cancel) : base(cancel)
    {
        CloseReason = closeReason;
    }

    /// <summary>
    ///  Provides the reason for the Form close.
    /// </summary>
    public CloseReason CloseReason { get; }
}
