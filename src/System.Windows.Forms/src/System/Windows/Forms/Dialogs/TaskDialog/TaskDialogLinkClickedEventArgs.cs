// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="TaskDialogPage.LinkClicked"/> event.
/// </summary>
public class TaskDialogLinkClickedEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="TaskDialogLinkClickedEventArgs"/> class.
    /// </summary>
    public TaskDialogLinkClickedEventArgs(string linkHref)
    {
        LinkHref = linkHref;
    }

    /// <summary>
    ///  Gets the value of the <c>href</c> attribute of the link that the user clicked.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Note: In order to avoid possible security vulnerabilities when showing content
    ///   from unsafe sources in a task dialog, you should always verify the value of this
    ///   property before actually opening the link.
    ///  </para>
    /// </remarks>
    public string LinkHref { get; }
}
