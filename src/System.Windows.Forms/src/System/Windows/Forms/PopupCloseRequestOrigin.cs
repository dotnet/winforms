// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the closing request origin in the context of the <see cref="PopupCloseRequestEventArgs"/>.
/// </summary>
public enum PopupCloseRequestOrigin
{
    /// <summary>
    ///  An action by the user caused the popup to close.
    /// </summary>
    ExternalByUser = 0,

    /// <summary>
    ///  The popup was closed by the component itself.
    /// </summary>
    InternalByComponent = 1
}
