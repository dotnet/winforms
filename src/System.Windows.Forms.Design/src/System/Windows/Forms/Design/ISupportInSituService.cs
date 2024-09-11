// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an interface for a designer to support InSitu editing for selected components.
/// </summary>
internal interface ISupportInSituService
{
    /// <summary>
    ///  Returns if the service is interested in InSitu Edit on Key Messages..
    /// </summary>
    bool IgnoreMessages { get; }

    /// <summary>
    ///  This method allows the service to handle the first WM_CHAR message.
    ///  The implementer for this service can perform any tasks that it wants when it gets this message.
    ///  e.g : ToolStripInSituService shows the Editor for each ToolStripItem in HandleKeyChar()
    /// </summary>
    void HandleKeyChar();

    /// <summary>
    ///  Returns the Window Handle that gets all the Keyboard messages once in InSitu.
    /// </summary>
    IntPtr GetEditWindow();
}
