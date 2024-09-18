// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for <see cref="TabControl.Selecting"/> and <see cref="TabControl.Selected"/> events.
/// </summary>
public class TabControlEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="TabControlEventArgs"/> class.
    /// </summary>
    public TabControlEventArgs(TabPage? tabPage, int tabPageIndex, TabControlAction action)
    {
        TabPage = tabPage;
        TabPageIndex = tabPageIndex;
        Action = action;
    }

    /// <summary>
    ///  Stores the reference to the TabPage that is undergoing the event.
    /// </summary>
    public TabPage? TabPage { get; }

    /// <summary>
    ///  Stores the index to the TabPage that is undergoing the event.
    /// </summary>
    public int TabPageIndex { get; }

    /// <summary>
    ///  Stores the action which instigated the event.
    /// </summary>
    public TabControlAction Action { get; }
}
